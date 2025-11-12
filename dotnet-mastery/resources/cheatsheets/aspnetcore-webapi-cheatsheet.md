# ASP.NET Core Web API Cheatsheet

## 📋 Quick Reference for Building RESTful APIs

Complete reference for ASP.NET Core Web API development with .NET 10.

## 🚀 Project Setup

### Create New Web API

```bash
# Minimal API (lightweight)
dotnet new web -n MyApi

# Web API with controllers
dotnet new webapi -n MyApi

# With authentication
dotnet new webapi -n MyApi --auth IndividualB2C
```

### Program.cs (Minimal API - .NET 10)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Define endpoints
app.MapGet("/", () => "Hello World!");

app.Run();
```

### Program.cs (Controller-based)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 🎯 Minimal APIs

### Basic Endpoints

```csharp
// GET endpoint
app.MapGet("/api/products", () =>
{
    return Results.Ok(new[] { "Product1", "Product2" });
});

// GET with parameter
app.MapGet("/api/products/{id}", (int id) =>
{
    return Results.Ok(new { Id = id, Name = $"Product {id}" });
});

// POST endpoint
app.MapPost("/api/products", (Product product) =>
{
    // Save product
    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT endpoint
app.MapPut("/api/products/{id}", (int id, Product product) =>
{
    // Update product
    return Results.NoContent();
});

// DELETE endpoint
app.MapDelete("/api/products/{id}", (int id) =>
{
    // Delete product
    return Results.NoContent();
});
```

### Parameter Binding

```csharp
// Route parameter
app.MapGet("/users/{id}", (int id) => $"User {id}");

// Query string
app.MapGet("/search", (string query, int page = 1) =>
    $"Search: {query}, Page: {page}");
// URL: /search?query=test&page=2

// Request body (JSON)
app.MapPost("/users", (User user) => Results.Ok(user));

// Multiple sources
app.MapGet("/api/{category}/items", (
    string category,           // From route
    [FromQuery] int page,      // From query string
    [FromHeader] string auth,  // From header
    [FromServices] ILogger logger // From DI
) => {
    logger.LogInformation($"Category: {category}");
    return Results.Ok();
});
```

### Async Endpoints

```csharp
app.MapGet("/api/data", async (HttpContext context) =>
{
    await Task.Delay(100);
    return Results.Ok("Data");
});

// With services
app.MapGet("/api/users", async (IUserService userService) =>
{
    var users = await userService.GetAllAsync();
    return Results.Ok(users);
});
```

### Response Types

```csharp
app.MapGet("/api/result", () =>
{
    // 200 OK
    return Results.Ok(data);

    // 201 Created
    return Results.Created("/api/resource/1", data);

    // 204 No Content
    return Results.NoContent();

    // 400 Bad Request
    return Results.BadRequest("Invalid input");

    // 401 Unauthorized
    return Results.Unauthorized();

    // 404 Not Found
    return Results.NotFound();

    // 500 Internal Server Error
    return Results.Problem("Error occurred");

    // Custom status code
    return Results.StatusCode(418);

    // File response
    return Results.File(bytes, "application/pdf");

    // Redirect
    return Results.Redirect("/new-url");
});
```

## 🎮 Controllers

### Basic Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        var created = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        await _productService.UpdateAsync(product);
        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
```

### Action Results

```csharp
// Return types
public ActionResult<Product> Get()
{
    // 200 OK
    return Ok(product);

    // 201 Created
    return CreatedAtAction(nameof(Get), new { id = 1 }, product);

    // 204 No Content
    return NoContent();

    // 400 Bad Request
    return BadRequest("Error message");

    // 401 Unauthorized
    return Unauthorized();

    // 403 Forbidden
    return Forbid();

    // 404 Not Found
    return NotFound();

    // 409 Conflict
    return Conflict();

    // 500 Internal Server Error
    return StatusCode(500, "Internal error");

    // Custom status
    return StatusCode(418, "I'm a teapot");
}
```

### Route Attributes

```csharp
// Controller route
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // GET: api/users
    [HttpGet]
    public IActionResult GetAll() { }

    // GET: api/users/5
    [HttpGet("{id}")]
    public IActionResult GetById(int id) { }

    // GET: api/users/5/orders
    [HttpGet("{id}/orders")]
    public IActionResult GetOrders(int id) { }

    // POST: api/users
    [HttpPost]
    public IActionResult Create([FromBody] User user) { }

    // Custom route
    [HttpGet("active")]
    [Route("api/users/active")]
    public IActionResult GetActive() { }
}
```

## 🔐 Authentication & Authorization

### JWT Authentication

```csharp
// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// In middleware pipeline
app.UseAuthentication();
app.UseAuthorization();
```

### Generate JWT Token

```csharp
public string GenerateJwtToken(User user)
{
    var securityKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    var credentials = new SigningCredentials(
        securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### Authorization Attributes

```csharp
// Require authentication
[Authorize]
public class SecureController : ControllerBase
{
    // All actions require auth
}

// Require specific role
[Authorize(Roles = "Admin")]
public IActionResult DeleteUser(int id) { }

// Require specific policy
[Authorize(Policy = "RequireAdminRole")]
public IActionResult AdminOnly() { }

// Allow anonymous (override controller [Authorize])
[AllowAnonymous]
public IActionResult PublicEndpoint() { }

// Multiple roles (OR)
[Authorize(Roles = "Admin,Manager")]
public IActionResult AdminOrManager() { }
```

### Authorization Policies

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    // Policy based on role
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    // Policy based on claim
    options.AddPolicy("RequireEmailVerified", policy =>
        policy.RequireClaim("EmailVerified", "true"));

    // Custom requirement
    options.AddPolicy("MinimumAge", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));

    // Complex policy
    options.AddPolicy("AtLeast21", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "age" &&
                 int.TryParse(c.Value, out int age) &&
                 age >= 21))));
});
```

## ✅ Input Validation

### Data Annotations

```csharp
public class CreateUserRequest
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, and digit")]
    public string Password { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }

    [Url]
    public string Website { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
```

### FluentValidation

```csharp
// Install: dotnet add package FluentValidation.AspNetCore

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(BeUniqueEmail)
            .WithMessage("Email already exists");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
            .Matches(@"\d").WithMessage("Password must contain digit");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18)
            .LessThanOrEqualTo(120);
    }

    private bool BeUniqueEmail(string email)
    {
        // Check database
        return !_userRepository.ExistsByEmail(email);
    }
}

// Register in Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
```

### Manual Validation

```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateUserRequest request)
{
    // Manual validation
    if (string.IsNullOrWhiteSpace(request.Username))
    {
        ModelState.AddModelError(nameof(request.Username), "Username is required");
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Process request
    return Ok();
}
```

## 📄 Pagination

### Pagination Model

```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
```

### Controller Implementation

```csharp
[HttpGet]
public async Task<ActionResult<PagedResult<Product>>> GetProducts(
    [FromQuery] PaginationParams pagination,
    [FromQuery] string? searchTerm = null,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool sortDescending = false)
{
    var query = _context.Products.AsQueryable();

    // Search
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(p =>
            p.Name.Contains(searchTerm) ||
            p.Description.Contains(searchTerm));
    }

    // Total count before pagination
    var totalCount = await query.CountAsync();

    // Sorting
    query = sortBy?.ToLower() switch
    {
        "name" => sortDescending
            ? query.OrderByDescending(p => p.Name)
            : query.OrderBy(p => p.Name),
        "price" => sortDescending
            ? query.OrderByDescending(p => p.Price)
            : query.OrderBy(p => p.Price),
        _ => query.OrderBy(p => p.Id)
    };

    // Pagination
    var items = await query
        .Skip((pagination.PageNumber - 1) * pagination.PageSize)
        .Take(pagination.PageSize)
        .ToListAsync();

    var result = new PagedResult<Product>
    {
        Items = items,
        PageNumber = pagination.PageNumber,
        PageSize = pagination.PageSize,
        TotalCount = totalCount
    };

    return Ok(result);
}
```

## 🔧 Dependency Injection

### Register Services

```csharp
// Program.cs

// Transient - new instance every time
builder.Services.AddTransient<IEmailService, EmailService>();

// Scoped - one instance per request
builder.Services.AddScoped<IUserService, UserService>();

// Singleton - one instance for app lifetime
builder.Services.AddSingleton<ICacheService, CacheService>();

// Register with factory
builder.Services.AddScoped<IProductService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<ProductService>>();
    var config = provider.GetRequiredService<IConfiguration>();
    return new ProductService(logger, config["ConnectionString"]);
});

// Register generic
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

### Constructor Injection

```csharp
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    private readonly IMapper _mapper;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger,
        IMapper mapper)
    {
        _productService = productService;
        _logger = logger;
        _mapper = mapper;
    }
}
```

## 🌐 CORS

### Configure CORS

```csharp
// Program.cs

// Add CORS services
builder.Services.AddCors(options =>
{
    // Allow specific origin
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://example.com", "https://www.example.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Allow any origin (development only!)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Custom policy
    options.AddPolicy("CustomPolicy", policy =>
    {
        policy.WithOrigins("https://example.com")
              .WithMethods("GET", "POST")
              .WithHeaders("Content-Type", "Authorization")
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// Apply CORS middleware
app.UseCors("AllowSpecificOrigin");

// Or apply to specific endpoints
app.MapGet("/api/data", () => "Data")
   .RequireCors("AllowSpecificOrigin");
```

## 📝 Logging

### ILogger Usage

```csharp
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);

        try
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", id);
                return NotFound();
            }

            _logger.LogDebug("Product {ProductId} retrieved successfully", id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
```

### Log Levels

```csharp
_logger.LogTrace("Trace message");       // Most verbose
_logger.LogDebug("Debug message");       // Debug info
_logger.LogInformation("Info message");  // General info
_logger.LogWarning("Warning message");   // Warning
_logger.LogError("Error message");       // Error
_logger.LogCritical("Critical message"); // Critical error
```

### Configure Logging (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MyApp.Controllers": "Debug"
    }
  }
}
```

## 🎯 Common Patterns

### Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    // ... other methods
}
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<User> Users { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(context);
        Users = new Repository<User>(context);
    }

    public IRepository<Product> Products { get; }
    public IRepository<User> Users { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## 🔥 Error Handling

### Global Exception Handler

```csharp
// Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionFeature.Error, "Unhandled exception");

            await context.Response.WriteAsJsonAsync(new
            {
                error = "An error occurred processing your request",
                detail = exceptionFeature.Error.Message
            });
        }
    });
});
```

### Custom Middleware

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            KeyNotFoundException => 404,
            _ => 500
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsJsonAsync(new
        {
            error = exception.Message,
            statusCode
        });
    }
}

// Register middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
```

---

**Pro Tip:** Use Swagger for API documentation and testing during development!

**Next:** [Entity Framework Core Cheatsheet](efcore-cheatsheet.md)
