# Lesson 02: Advanced DI Patterns

## 🎯 Learning Objectives

By the end of this lesson, you will master:

- **Decorator Pattern** with DI
- **Factory Pattern** for dynamic object creation
- **Options Pattern** for configuration
- **Named Services** and service resolution
- **Conditional Registration** based on environment
- **Module/Extension Methods** for clean registration
- **Scrutor** for assembly scanning and decoration
- **Generic Registration** patterns

## 📚 Table of Contents

1. [Decorator Pattern](#decorator-pattern)
2. [Factory Pattern](#factory-pattern)
3. [Options Pattern](#options-pattern)
4. [Named Services](#named-services)
5. [Conditional Registration](#conditional-registration)
6. [Extension Methods for Registration](#extension-methods-for-registration)
7. [Assembly Scanning with Scrutor](#assembly-scanning-with-scrutor)
8. [Generic Registration](#generic-registration)

---

## Decorator Pattern

The **Decorator Pattern** wraps a service with additional behavior without modifying the original implementation.

### Use Cases
- **Logging**: Log method calls
- **Caching**: Cache method results
- **Retry Logic**: Retry failed operations
- **Validation**: Validate inputs/outputs
- **Performance Monitoring**: Measure execution time

### Basic Decorator Example

```csharp
// Core interface
public interface IOrderService
{
    Task<Order> GetOrderAsync(int id);
    Task<int> CreateOrderAsync(Order order);
}

// Core implementation
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        return await _repository.CreateAsync(order);
    }
}

// Decorator: Adds logging
public class LoggingOrderServiceDecorator : IOrderService
{
    private readonly IOrderService _inner;
    private readonly ILogger<LoggingOrderServiceDecorator> _logger;

    public LoggingOrderServiceDecorator(
        IOrderService inner,  // The service being decorated
        ILogger<LoggingOrderServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        _logger.LogInformation("Getting order {OrderId}", id);
        try
        {
            var order = await _inner.GetOrderAsync(id);
            _logger.LogInformation("Successfully retrieved order {OrderId}", id);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            throw;
        }
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        _logger.LogInformation("Creating order for customer {Customer}", order.CustomerEmail);
        try
        {
            var orderId = await _inner.CreateOrderAsync(order);
            _logger.LogInformation("Successfully created order {OrderId}", orderId);
            return orderId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            throw;
        }
    }
}

// Registration
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderService>(serviceProvider =>
{
    var orderService = serviceProvider.GetRequiredService<OrderService>();
    var logger = serviceProvider.GetRequiredService<ILogger<LoggingOrderServiceDecorator>>();
    return new LoggingOrderServiceDecorator(orderService, logger);
});
```

### Caching Decorator

```csharp
/// <summary>
/// Decorator that caches GetOrderAsync results.
/// </summary>
public class CachingOrderServiceDecorator : IOrderService
{
    private readonly IOrderService _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CachingOrderServiceDecorator(
        IOrderService inner,
        IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        string cacheKey = $"order_{id}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out Order? cachedOrder))
        {
            return cachedOrder!;
        }

        // Not in cache - get from inner service
        var order = await _inner.GetOrderAsync(id);

        // Cache the result
        _cache.Set(cacheKey, order, _cacheDuration);

        return order;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        // Don't cache writes - delegate to inner
        var orderId = await _inner.CreateOrderAsync(order);

        // Invalidate cache for this order
        string cacheKey = $"order_{orderId}";
        _cache.Remove(cacheKey);

        return orderId;
    }
}
```

### Retry Decorator with Polly

```csharp
/// <summary>
/// Decorator that retries operations on transient failures.
/// Uses Polly for retry logic.
/// </summary>
public class RetryOrderServiceDecorator : IOrderService
{
    private readonly IOrderService _inner;
    private readonly ILogger<RetryOrderServiceDecorator> _logger;
    private readonly IAsyncPolicy _retryPolicy;

    public RetryOrderServiceDecorator(
        IOrderService inner,
        ILogger<RetryOrderServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;

        // Retry 3 times with exponential backoff
        _retryPolicy = Policy
            .Handle<SqlException>()  // Only retry on SQL exceptions
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount} after {Delay}s due to {Exception}",
                        retryCount, timeSpan.TotalSeconds, exception.GetType().Name);
                });
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _retryPolicy.ExecuteAsync(() => _inner.GetOrderAsync(id));
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        return await _retryPolicy.ExecuteAsync(() => _inner.CreateOrderAsync(order));
    }
}
```

### Chaining Multiple Decorators

```csharp
// Registration: Chain decorators in order
builder.Services.AddScoped<OrderService>();  // Core implementation

builder.Services.AddScoped<IOrderService>(serviceProvider =>
{
    // 1. Start with core implementation
    IOrderService service = serviceProvider.GetRequiredService<OrderService>();

    // 2. Add retry logic
    var logger = serviceProvider.GetRequiredService<ILogger<RetryOrderServiceDecorator>>();
    service = new RetryOrderServiceDecorator(service, logger);

    // 3. Add caching
    var cache = serviceProvider.GetRequiredService<IMemoryCache>();
    service = new CachingOrderServiceDecorator(service, cache);

    // 4. Add logging (outermost - logs everything)
    var loggingLogger = serviceProvider.GetRequiredService<ILogger<LoggingOrderServiceDecorator>>();
    service = new LoggingOrderServiceDecorator(service, loggingLogger);

    return service;
});

// Call flow:
// Controller -> Logging -> Caching -> Retry -> Core Implementation
//            <- Logging <- Caching <- Retry <- Core Implementation
```

### Performance Monitoring Decorator

```csharp
/// <summary>
/// Decorator that measures execution time.
/// </summary>
public class PerformanceMonitoringDecorator<T> : DispatchProxy
{
    private T _decorated = default!;
    private ILogger<PerformanceMonitoringDecorator<T>> _logger = default!;

    public static T Create(T decorated, ILogger<PerformanceMonitoringDecorator<T>> logger)
    {
        object proxy = Create<T, PerformanceMonitoringDecorator<T>>();
        ((PerformanceMonitoringDecorator<T>)proxy).SetParameters(decorated, logger);
        return (T)proxy;
    }

    private void SetParameters(T decorated, ILogger<PerformanceMonitoringDecorator<T>> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = targetMethod!.Invoke(_decorated, args);
            stopwatch.Stop();

            _logger.LogInformation(
                "{MethodName} executed in {ElapsedMs}ms",
                targetMethod.Name, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "{MethodName} failed after {ElapsedMs}ms",
                targetMethod.Name, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

// Usage
var service = serviceProvider.GetRequiredService<OrderService>();
var logger = serviceProvider.GetRequiredService<ILogger<PerformanceMonitoringDecorator<IOrderService>>>();
var monitoredService = PerformanceMonitoringDecorator<IOrderService>.Create(service, logger);
```

---

## Factory Pattern

The **Factory Pattern** creates objects based on runtime conditions.

### Simple Factory

```csharp
// Different notification implementations
public interface INotificationSender
{
    Task SendAsync(string recipient, string message);
}

public class EmailNotificationSender : INotificationSender
{
    public Task SendAsync(string recipient, string message)
    {
        // Send email
        Console.WriteLine($"Email to {recipient}: {message}");
        return Task.CompletedTask;
    }
}

public class SmsNotificationSender : INotificationSender
{
    public Task SendAsync(string recipient, string message)
    {
        // Send SMS
        Console.WriteLine($"SMS to {recipient}: {message}");
        return Task.CompletedTask;
    }
}

public class PushNotificationSender : INotificationSender
{
    public Task SendAsync(string recipient, string message)
    {
        // Send push notification
        Console.WriteLine($"Push to {recipient}: {message}");
        return Task.CompletedTask;
    }
}

// Factory interface
public interface INotificationFactory
{
    INotificationSender Create(NotificationType type);
}

// Factory implementation
public class NotificationFactory : INotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationSender Create(NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => _serviceProvider.GetRequiredService<EmailNotificationSender>(),
            NotificationType.Sms => _serviceProvider.GetRequiredService<SmsNotificationSender>(),
            NotificationType.Push => _serviceProvider.GetRequiredService<PushNotificationSender>(),
            _ => throw new ArgumentException($"Unknown notification type: {type}")
        };
    }
}

// Registration
builder.Services.AddTransient<EmailNotificationSender>();
builder.Services.AddTransient<SmsNotificationSender>();
builder.Services.AddTransient<PushNotificationSender>();
builder.Services.AddSingleton<INotificationFactory, NotificationFactory>();

// Usage
public class NotificationService
{
    private readonly INotificationFactory _factory;

    public NotificationService(INotificationFactory factory)
    {
        _factory = factory;
    }

    public async Task SendNotificationAsync(string recipient, string message, NotificationType type)
    {
        var sender = _factory.Create(type);
        await sender.SendAsync(recipient, message);
    }
}
```

### Named Factory Pattern

```csharp
// Better approach: Use dictionary-based factory
public class NotificationFactory : INotificationFactory
{
    private readonly Dictionary<NotificationType, Func<INotificationSender>> _factories;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _factories = new Dictionary<NotificationType, Func<INotificationSender>>
        {
            [NotificationType.Email] = () => serviceProvider.GetRequiredService<EmailNotificationSender>(),
            [NotificationType.Sms] = () => serviceProvider.GetRequiredService<SmsNotificationSender>(),
            [NotificationType.Push] = () => serviceProvider.GetRequiredService<PushNotificationSender>()
        };
    }

    public INotificationSender Create(NotificationType type)
    {
        if (!_factories.TryGetValue(type, out var factory))
        {
            throw new ArgumentException($"Unknown notification type: {type}");
        }

        return factory();
    }
}
```

### Generic Factory

```csharp
/// <summary>
/// Generic factory for creating services by type.
/// </summary>
public interface IFactory<T>
{
    T Create();
}

public class Factory<T> : IFactory<T>
{
    private readonly IServiceProvider _serviceProvider;

    public Factory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}

// Registration
builder.Services.AddTransient(typeof(IFactory<>), typeof(Factory<>));

// Usage
public class OrderController : ControllerBase
{
    private readonly IFactory<IOrderService> _orderServiceFactory;

    public OrderController(IFactory<IOrderService> orderServiceFactory)
    {
        _orderServiceFactory = orderServiceFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        // Create service on demand
        var orderService = _orderServiceFactory.Create();
        var orderId = await orderService.CreateOrderAsync(order);
        return Ok(new { orderId });
    }
}
```

---

## Options Pattern

The **Options Pattern** provides strongly-typed configuration.

### Basic Options Pattern

```csharp
// Configuration class
public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string FromAddress { get; set; } = string.Empty;
}

// appsettings.json
/*
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "myapp@gmail.com",
    "Password": "app_password",
    "EnableSsl": true,
    "FromAddress": "noreply@myapp.com"
  }
}
*/

// Registration
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Usage: IOptions<T>
public class EmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;  // Get current value
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        var message = new MailMessage(_settings.FromAddress, to, subject, body);
        await client.SendMailAsync(message);

        _logger.LogInformation("Email sent to {Recipient}", to);
    }
}
```

### IOptionsSnapshot for Reloadable Configuration

```csharp
/// <summary>
/// IOptionsSnapshot re-reads configuration on each request.
/// Use for scoped services that need to pick up config changes.
/// </summary>
public class DynamicEmailService
{
    private readonly IOptionsSnapshot<EmailSettings> _options;
    private readonly ILogger<DynamicEmailService> _logger;

    public DynamicEmailService(
        IOptionsSnapshot<EmailSettings> options,
        ILogger<DynamicEmailService> logger)
    {
        _options = options;  // Snapshot per request
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Gets current settings (may have changed since last request)
        var settings = _options.Value;

        // Use settings...
        _logger.LogInformation(
            "Sending email using SMTP host: {SmtpHost}",
            settings.SmtpHost);
    }
}

// Registration - must be Scoped for IOptionsSnapshot
builder.Services.AddScoped<DynamicEmailService>();
```

### IOptionsMonitor for Real-Time Changes

```csharp
/// <summary>
/// IOptionsMonitor notifies when configuration changes.
/// Use for singleton services that need to react to config changes.
/// </summary>
public class MonitoredEmailService : IDisposable
{
    private readonly IOptionsMonitor<EmailSettings> _monitor;
    private readonly ILogger<MonitoredEmailService> _logger;
    private IDisposable? _changeToken;
    private EmailSettings _currentSettings;

    public MonitoredEmailService(
        IOptionsMonitor<EmailSettings> monitor,
        ILogger<MonitoredEmailService> logger)
    {
        _monitor = monitor;
        _logger = logger;
        _currentSettings = monitor.CurrentValue;

        // Subscribe to changes
        _changeToken = _monitor.OnChange(settings =>
        {
            _logger.LogInformation("Email settings changed!");
            _currentSettings = settings;
            // Could reconnect SMTP, update connection pool, etc.
        });
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Always uses latest settings
        var settings = _monitor.CurrentValue;
        // Or use cached: _currentSettings

        // Send email...
    }

    public void Dispose()
    {
        _changeToken?.Dispose();
    }
}

// Registration - can be Singleton
builder.Services.AddSingleton<MonitoredEmailService>();
```

### Validating Options

```csharp
// Options class with validation
public class PaymentSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public decimal MaxTransactionAmount { get; set; }
    public int TimeoutSeconds { get; set; }
}

// Validation using Data Annotations
public class PaymentSettings
{
    [Required]
    [StringLength(100, MinimumLength = 10)]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string ApiSecret { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal MaxTransactionAmount { get; set; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; }
}

// Registration with validation
builder.Services.AddOptions<PaymentSettings>()
    .Bind(builder.Configuration.GetSection("PaymentSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();  // Validate at startup, not first use

// Custom validation
builder.Services.AddOptions<PaymentSettings>()
    .Bind(builder.Configuration.GetSection("PaymentSettings"))
    .Validate(settings =>
    {
        // Custom validation logic
        if (string.IsNullOrEmpty(settings.ApiKey))
            return false;

        if (settings.MaxTransactionAmount <= 0)
            return false;

        return true;
    }, "Invalid payment settings")
    .ValidateOnStart();
```

### Named Options

```csharp
// Multiple configurations of same type
public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetries { get; set; }
}

// appsettings.json
/*
{
  "Databases": {
    "Orders": {
      "ConnectionString": "Server=orders-db;Database=Orders",
      "MaxRetries": 3
    },
    "Customers": {
      "ConnectionString": "Server=customers-db;Database=Customers",
      "MaxRetries": 5
    }
  }
}
*/

// Registration
builder.Services.Configure<DatabaseSettings>("Orders",
    builder.Configuration.GetSection("Databases:Orders"));
builder.Services.Configure<DatabaseSettings>("Customers",
    builder.Configuration.GetSection("Databases:Customers"));

// Usage
public class MultiDatabaseService
{
    private readonly DatabaseSettings _ordersSettings;
    private readonly DatabaseSettings _customersSettings;

    public MultiDatabaseService(IOptionsSnapshot<DatabaseSettings> options)
    {
        _ordersSettings = options.Get("Orders");
        _customersSettings = options.Get("Customers");
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        // Use _ordersSettings.ConnectionString
    }

    public async Task ProcessCustomerAsync(int customerId)
    {
        // Use _customersSettings.ConnectionString
    }
}
```

---

## Named Services

Register multiple implementations with different names.

```csharp
// Service that needs different implementations
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessAsync(decimal amount, PaymentMethod method);
}

public class StripePaymentGateway : IPaymentGateway
{
    public Task<PaymentResult> ProcessAsync(decimal amount, PaymentMethod method)
    {
        Console.WriteLine($"Processing ${amount} via Stripe");
        return Task.FromResult(new PaymentResult { Success = true });
    }
}

public class PayPalPaymentGateway : IPaymentGateway
{
    public Task<PaymentResult> ProcessAsync(decimal amount, PaymentMethod method)
    {
        Console.WriteLine($"Processing ${amount} via PayPal");
        return Task.FromResult(new PaymentResult { Success = true });
    }
}

// Factory to resolve by name
public interface IPaymentGatewayFactory
{
    IPaymentGateway Create(string gatewayName);
}

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _gateways;

    public PaymentGatewayFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _gateways = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["stripe"] = typeof(StripePaymentGateway),
            ["paypal"] = typeof(PayPalPaymentGateway)
        };
    }

    public IPaymentGateway Create(string gatewayName)
    {
        if (!_gateways.TryGetValue(gatewayName, out var gatewayType))
        {
            throw new ArgumentException($"Unknown payment gateway: {gatewayName}");
        }

        return (IPaymentGateway)_serviceProvider.GetRequiredService(gatewayType);
    }
}

// Registration
builder.Services.AddScoped<StripePaymentGateway>();
builder.Services.AddScoped<PayPalPaymentGateway>();
builder.Services.AddSingleton<IPaymentGatewayFactory, PaymentGatewayFactory>();

// Usage
public class PaymentService
{
    private readonly IPaymentGatewayFactory _factory;

    public PaymentService(IPaymentGatewayFactory factory)
    {
        _factory = factory;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        decimal amount,
        PaymentMethod method,
        string preferredGateway)
    {
        var gateway = _factory.Create(preferredGateway);
        return await gateway.ProcessAsync(amount, method);
    }
}
```

---

## Conditional Registration

Register services based on environment or configuration.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register different services based on environment
if (builder.Environment.IsDevelopment())
{
    // Development: Use mock services
    builder.Services.AddScoped<IEmailSender, MockEmailSender>();
    builder.Services.AddScoped<IPaymentGateway, MockPaymentGateway>();
    builder.Services.AddSingleton<ILogger, ConsoleLogger>();
}
else if (builder.Environment.IsStaging())
{
    // Staging: Use test services
    builder.Services.AddScoped<IEmailSender, TestEmailSender>();
    builder.Services.AddScoped<IPaymentGateway, StripeTestPaymentGateway>();
    builder.Services.AddSingleton<ILogger, FileLogger>();
}
else // Production
{
    // Production: Use real services
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
    builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
    builder.Services.AddSingleton<ILogger, CloudLogger>();
}

// Conditional registration based on configuration
var useCache = builder.Configuration.GetValue<bool>("Features:UseCache");
if (useCache)
{
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<IOrderService, CachedOrderService>();
}
else
{
    builder.Services.AddScoped<IOrderService, OrderService>();
}

// Register feature flags
builder.Services.AddSingleton<IFeatureFlags>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    return new FeatureFlags
    {
        EnableNewCheckout = config.GetValue<bool>("Features:EnableNewCheckout"),
        EnableRecommendations = config.GetValue<bool>("Features:EnableRecommendations")
    };
});
```

---

## Extension Methods for Registration

Keep `Program.cs` clean with extension methods.

```csharp
// ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Core services
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        // Infrastructure
        services.AddInfrastructure(configuration);

        // External services
        services.AddExternalServices(configuration);

        return services;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Email
        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));
        services.AddTransient<IEmailSender, SmtpEmailSender>();

        // Payment
        services.Configure<PaymentSettings>(
            configuration.GetSection("PaymentSettings"));
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();

        // Caching
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}

// Program.cs - Clean and simple!
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Run();
```

---

## Assembly Scanning with Scrutor

**Scrutor** is a library for assembly scanning and decoration.

```bash
dotnet add package Scrutor
```

### Auto-Registration by Convention

```csharp
// Automatically register all services ending with "Service"
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Automatically register all repositories
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Register specific interfaces
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.AssignableTo<IValidator>())
    .AsImplementedInterfaces()
    .WithTransientLifetime());
```

### Decoration with Scrutor

```csharp
// Register core service
builder.Services.AddScoped<IOrderService, OrderService>();

// Decorate with logging
builder.Services.Decorate<IOrderService, LoggingOrderServiceDecorator>();

// Decorate with caching
builder.Services.Decorate<IOrderService, CachingOrderServiceDecorator>();

// Decorate with retry logic
builder.Services.Decorate<IOrderService, RetryOrderServiceDecorator>();

// Result: RetryOrderServiceDecorator(CachingOrderServiceDecorator(LoggingOrderServiceDecorator(OrderService)))
```

### Generic Decoration

```csharp
// Decorate all ICommandHandler<T> with logging
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
```

---

## Generic Registration

Register open generic types.

```csharp
// Generic interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Generic implementation
public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

// Registration - registers for ALL types!
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Usage - automatically resolved for any entity type
public class ProductService
{
    private readonly IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;  // Automatically gets Repository<Product>
    }
}

public class OrderService
{
    private readonly IRepository<Order> _orderRepository;

    public OrderService(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;  // Automatically gets Repository<Order>
    }
}
```

---

## Summary

**Key Patterns Covered:**

1. **Decorator Pattern**: Add behavior without modifying original class
   - Logging, caching, retry logic, performance monitoring

2. **Factory Pattern**: Create objects dynamically at runtime
   - Simple factories, named factories, generic factories

3. **Options Pattern**: Strongly-typed configuration
   - IOptions, IOptionsSnapshot, IOptionsMonitor
   - Validation and named options

4. **Named Services**: Multiple implementations resolved by name

5. **Conditional Registration**: Environment-based registration

6. **Extension Methods**: Clean, organized service registration

7. **Assembly Scanning**: Automatic registration with Scrutor

8. **Generic Registration**: Register open generic types

**Benefits:**
- ✅ Clean, maintainable code
- ✅ Easy to extend and modify
- ✅ Testable (decorators can be swapped)
- ✅ Follows SOLID principles
- ✅ Reduces boilerplate

---

## What's Next?

- **Module 07**: ASP.NET Core Web API development
- **Module 08**: Entity Framework Core
- **Exercise**: Implement decorator pattern for a real service

---

**Ready to practice?** Check out the exercises in `exercises/` directory!
