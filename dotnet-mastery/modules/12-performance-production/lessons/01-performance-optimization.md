# Lesson 01: Performance Optimization

## 🎯 Learning Objectives

- **Performance Profiling** - Finding bottlenecks
- **Memory Optimization** - Reducing allocations
- **Database Performance** - Indexes, query optimization
- **Caching Strategies** - In-Memory, Redis, CDN
- **Asynchronous Programming** - Non-blocking I/O
- **Monitoring & Logging** - Production observability
- **Deployment Strategies** - Zero-downtime deployments

## 📚 Table of Contents

1. [Performance Profiling](#performance-profiling)
2. [Memory Optimization](#memory-optimization)
3. [Database Performance](#database-performance)
4. [Caching Strategies](#caching-strategies)
5. [Production Best Practices](#production-best-practices)
6. [Monitoring & Logging](#monitoring--logging)
7. [Deployment Strategies](#deployment-strategies)

---

## Performance Profiling

### BenchmarkDotNet

```bash
dotnet add package BenchmarkDotNet
```

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
[RankColumn]
public class StringBenchmarks
{
    [Benchmark(Baseline = true)]
    public string StringConcatenation()
    {
        string result = "";
        for (int i = 0; i < 1000; i++)
        {
            result += i.ToString();
        }
        return result;
    }

    [Benchmark]
    public string StringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.Append(i);
        }
        return sb.ToString();
    }

    [Benchmark]
    public string StringCreate()
    {
        return string.Create(4000, 0, (span, state) =>
        {
            for (int i = 0; i < 1000; i++)
            {
                i.TryFormat(span, out int written);
                span = span.Slice(written);
            }
        });
    }
}

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<StringBenchmarks>();
    }
}

// Results:
// | Method               |     Mean |   Gen0 |   Gen1 | Allocated |
// |--------------------- |---------:|-------:|-------:|----------:|
// | StringConcatenation  | 124.5 μs | 125.00 |  62.50 |   510 KB  |
// | StringBuilder        |   4.2 μs |   0.48 |   0.00 |     2 KB  |
// | StringCreate         |   3.8 μs |   0.24 |   0.00 |     1 KB  |
```

### Identifying Bottlenecks

```csharp
// Use Stopwatch for manual profiling
public async Task<List<Product>> GetProductsAsync()
{
    var sw = Stopwatch.StartNew();

    var products = await _context.Products
        .Include(p => p.Category)
        .ToListAsync();

    sw.Stop();

    _logger.LogInformation(
        "GetProducts took {ElapsedMs}ms, returned {Count} products",
        sw.ElapsedMilliseconds,
        products.Count);

    return products;
}
```

---

## Memory Optimization

### 1. Use Struct for Small Data

```csharp
// ❌ BAD: Class allocates on heap
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

// ✅ GOOD: Struct allocates on stack
public readonly struct Point
{
    public int X { get; init; }
    public int Y { get; init; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

### 2. ArrayPool for Large Arrays

```csharp
// ❌ BAD: Allocate new array every time
public byte[] ProcessData()
{
    byte[] buffer = new byte[1024 * 1024]; // 1 MB allocation
    // Process data...
    return buffer;
}

// ✅ GOOD: Rent from pool
public byte[] ProcessData()
{
    byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);
    try
    {
        // Process data...
        return buffer;
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

### 3. Span<T> for Zero-Allocation Slicing

```csharp
// ❌ BAD: Allocates substring
public string GetFirstWord(string text)
{
    int spaceIndex = text.IndexOf(' ');
    return text.Substring(0, spaceIndex); // Allocates new string
}

// ✅ GOOD: No allocation
public ReadOnlySpan<char> GetFirstWord(ReadOnlySpan<char> text)
{
    int spaceIndex = text.IndexOf(' ');
    return text.Slice(0, spaceIndex); // No allocation!
}
```

### 4. Object Pooling

```csharp
public class ObjectPool<T> where T : class, new()
{
    private readonly ConcurrentBag<T> _objects = new();

    public T Rent()
    {
        if (_objects.TryTake(out var item))
            return item;

        return new T();
    }

    public void Return(T item)
    {
        _objects.Add(item);
    }
}

// Usage
private static readonly ObjectPool<StringBuilder> _pool = new();

public string BuildString()
{
    var sb = _pool.Rent();
    try
    {
        sb.Clear();
        sb.Append("Hello");
        sb.Append(" World");
        return sb.ToString();
    }
    finally
    {
        _pool.Return(sb);
    }
}
```

---

## Database Performance

### 1. Add Indexes

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        // Single column index
        entity.HasIndex(p => p.Name);

        // Composite index
        entity.HasIndex(p => new { p.CategoryId, p.Price });

        // Unique index
        entity.HasIndex(p => p.SKU).IsUnique();

        // Filtered index (SQL Server)
        entity.HasIndex(p => p.Price)
            .HasFilter("[IsActive] = 1");
    });
}
```

### 2. Use AsNoTracking for Read-Only Queries

```csharp
// ❌ BAD: Change tracking overhead
public async Task<List<ProductDto>> GetProductsAsync()
{
    return await _context.Products
        .Select(p => new ProductDto { ... })
        .ToListAsync();
}

// ✅ GOOD: No tracking
public async Task<List<ProductDto>> GetProductsAsync()
{
    return await _context.Products
        .AsNoTracking()
        .Select(p => new ProductDto { ... })
        .ToListAsync();
}
```

### 3. Project to DTOs Early

```csharp
// ❌ BAD: Loads all columns, all rows
public async Task<List<ProductDto>> GetProductsAsync()
{
    var products = await _context.Products.ToListAsync();
    return products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name
    }).ToList();
}

// ✅ GOOD: Projects in database
public async Task<List<ProductDto>> GetProductsAsync()
{
    return await _context.Products
        .Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name
        })
        .ToListAsync();
}

// SQL Generated:
// SELECT [p].[Id], [p].[Name] FROM [Products] AS [p]
```

### 4. Batch Operations

```csharp
// ❌ BAD: Multiple round trips
public async Task UpdatePricesAsync(List<int> productIds, decimal newPrice)
{
    foreach (var id in productIds)
    {
        var product = await _context.Products.FindAsync(id);
        product.Price = newPrice;
    }
    await _context.SaveChangesAsync(); // 1000 SELECT queries!
}

// ✅ GOOD: Single query
public async Task UpdatePricesAsync(List<int> productIds, decimal newPrice)
{
    await _context.Products
        .Where(p => productIds.Contains(p.Id))
        .ExecuteUpdateAsync(p => p.SetProperty(x => x.Price, newPrice));
    // Single UPDATE query!
}
```

### 5. Use Compiled Queries

```csharp
private static readonly Func<ApplicationDbContext, int, Task<Product?>> _getProductById =
    EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
        context.Products.FirstOrDefault(p => p.Id == id));

public async Task<Product?> GetProductByIdAsync(int id)
{
    return await _getProductById(_context, id);
    // Query plan cached, ~10-20% faster
}
```

---

## Caching Strategies

### 1. In-Memory Cache

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly IProductRepository _repository;

    public async Task<Product> GetProductAsync(int id)
    {
        string cacheKey = $"product_{id}";

        if (_cache.TryGetValue(cacheKey, out Product? product))
        {
            return product!; // Cache hit
        }

        // Cache miss - load from database
        product = await _repository.GetByIdAsync(id);

        if (product != null)
        {
            _cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));
        }

        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _repository.UpdateAsync(product);

        // Invalidate cache
        string cacheKey = $"product_{product.Id}";
        _cache.Remove(cacheKey);
    }
}
```

### 2. Distributed Cache (Redis)

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";
});

// Service
public class ProductService
{
    private readonly IDistributedCache _cache;

    public async Task<Product?> GetProductAsync(int id)
    {
        string cacheKey = $"product_{id}";

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedData);
        }

        var product = await _repository.GetByIdAsync(id);

        if (product != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        return product;
    }
}
```

### 3. Response Caching

```csharp
// Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// Controller
[HttpGet]
[ResponseCache(Duration = 60)] // Cache for 60 seconds
public async Task<IActionResult> GetProducts()
{
    var products = await _service.GetProductsAsync();
    return Ok(products);
}

// Or use cache profiles
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30",
        new CacheProfile { Duration = 30 });
});

[ResponseCache(CacheProfileName = "Default30")]
public async Task<IActionResult> GetProducts() { }
```

---

## Production Best Practices

### 1. Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!)
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});
```

### 2. Graceful Shutdown

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHostedService<GracefulShutdownService>();
    }
}

public class GracefulShutdownService : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<GracefulShutdownService> _logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStopping.Register(OnStopping);
        _lifetime.ApplicationStopped.Register(OnStopped);
        return Task.CompletedTask;
    }

    private void OnStopping()
    {
        _logger.LogInformation("Application is stopping. Draining connections...");
        Thread.Sleep(5000); // Wait for in-flight requests
    }

    private void OnStopped()
    {
        _logger.LogInformation("Application stopped");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

### 3. Configuration Management

```csharp
// appsettings.json (dev)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Dev"
  }
}

// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "" // Overridden by environment variable
  }
}

// Environment variables in production
// ConnectionStrings__DefaultConnection=Server=prod-db;Database=Prod

// Azure Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## Monitoring & Logging

### Structured Logging with Serilog

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
```

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // Centralized logging
    .CreateLogger();

builder.Host.UseSerilog();

// Usage
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        _logger.LogInformation(
            "Creating product {ProductName} in category {CategoryId}",
            dto.Name, dto.CategoryId);

        try
        {
            var product = await _repository.CreateAsync(dto);

            _logger.LogInformation(
                "Product created with ID {ProductId}",
                product.Id);

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating product {ProductName}",
                dto.Name);
            throw;
        }
    }
}
```

### Application Insights

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]);

// Automatic telemetry:
// - Request/Response times
// - Dependency calls (HTTP, SQL, Redis)
// - Exceptions
// - Custom events
```

---

## Deployment Strategies

### 1. Blue-Green Deployment

```
Current (Blue)           New (Green)
Load Balancer
     │
     ├──→ App v1.0 ──┐
     │                │
     └──→ App v1.0    │
                      │
      App v2.0 ←──────┘ (ready but not receiving traffic)

# Deploy v2.0
# Test v2.0
# Switch traffic to v2.0

Load Balancer
     │
     ├──→ App v2.0 ──┐
     │                │
     └──→ App v2.0    │
                      │
      App v1.0 ←──────┘ (kept as rollback option)
```

### 2. Canary Deployment

```
Load Balancer
     │
     ├──→ App v1.0 (90% traffic)
     ├──→ App v1.0
     ├──→ App v1.0
     ├──→ App v1.0
     └──→ App v2.0 (10% traffic) ← Monitor for errors

# If stable, gradually increase v2.0 traffic
# 10% → 25% → 50% → 100%
```

### 3. Rolling Deployment (Kubernetes)

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: my-app
spec:
  replicas: 4
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1        # Max 1 extra pod during update
      maxUnavailable: 1  # Max 1 pod down during update
  template:
    spec:
      containers:
      - name: my-app
        image: myapp:2.0
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5

# Update sequence:
# 1. Create new pod (v2.0)
# 2. Wait for readiness
# 3. Terminate old pod (v1.0)
# 4. Repeat for remaining pods
```

### 4. Feature Flags

```csharp
public interface IFeatureFlagService
{
    bool IsEnabled(string featureName);
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IFeatureFlagService _featureFlags;

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        if (_featureFlags.IsEnabled("NewProductDisplay"))
        {
            return Ok(await _service.GetProductsWithNewLayout());
        }

        return Ok(await _service.GetProductsWithOldLayout());
    }
}
```

---

## Summary

**Performance Optimization:**
1. **Profile First**: Use BenchmarkDotNet to find bottlenecks
2. **Memory**: Use structs, Span<T>, ArrayPool, ObjectPool
3. **Database**: Indexes, AsNoTracking, projections, batch operations
4. **Caching**: In-Memory → Redis → CDN layers
5. **Async/Await**: Non-blocking I/O operations

**Production Readiness:**
1. **Health Checks**: Monitor service and dependency health
2. **Graceful Shutdown**: Drain connections before stopping
3. **Configuration**: Environment-specific settings, secrets in Key Vault
4. **Logging**: Structured logging with Serilog + centralized logs
5. **Monitoring**: Application Insights for telemetry
6. **Deployment**: Blue-Green, Canary, or Rolling updates

**Golden Rules:**
- ✅ Measure before optimizing
- ✅ Optimize hot paths first
- ✅ Cache aggressively but invalidate correctly
- ✅ Monitor everything in production
- ✅ Deploy frequently with safety nets

---

## Congratulations! 🎉

You've completed the comprehensive .NET Mastery tutorial covering:
- Modules 01-12: C# through Production
- 15 Projects: Beginner to Enterprise
- Advanced Patterns: CQRS, Microservices, Performance
- Testing, Quality, and Deployment strategies

**You're now ready to build production-grade .NET applications!**
