# Lesson 01: ASP.NET Core Web API Introduction

## 🎯 Learning Objectives

By the end of this lesson, you will understand:

- **What** ASP.NET Core Web API is and when to use it
- **REST** principles and HTTP methods
- **Creating** your first Web API from scratch
- **Routing** patterns (attribute and conventional)
- **Controllers and Actions** structure
- **Model Binding** and validation
- **Response types** and status codes
- **Minimal APIs** (modern approach)
- **Middleware pipeline** and request processing
- **Best practices** for production APIs

## 📚 Table of Contents

1. [What is ASP.NET Core Web API?](#what-is-aspnet-core-web-api)
2. [REST Principles](#rest-principles)
3. [Creating Your First API](#creating-your-first-api)
4. [Routing](#routing)
5. [Controllers and Actions](#controllers-and-actions)
6. [Model Binding and Validation](#model-binding-and-validation)
7. [Response Types](#response-types)
8. [HTTP Status Codes](#http-status-codes)
9. [Minimal APIs](#minimal-apis)
10. [Middleware Pipeline](#middleware-pipeline)
11. [Best Practices](#best-practices)

---

## What is ASP.NET Core Web API?

**ASP.NET Core Web API** is a framework for building HTTP-based services that can be consumed by various clients (web browsers, mobile apps, IoT devices, other services).

### Key Characteristics

- **Cross-Platform**: Runs on Windows, Linux, macOS
- **High Performance**: One of the fastest web frameworks
- **Built-in DI**: Dependency injection out of the box
- **Middleware Pipeline**: Flexible request processing
- **Content Negotiation**: Automatic JSON/XML serialization
- **Model Validation**: Automatic input validation
- **OpenAPI/Swagger**: Built-in API documentation
- **Asynchronous**: Async/await throughout

### When to Use Web APIs

✅ **Use Web APIs for:**
- Mobile app backends
- Single Page Application (SPA) backends
- Microservices
- IoT device communication
- Third-party integrations
- Public APIs

❌ **Don't use Web APIs for:**
- Server-rendered web pages (use MVC or Razor Pages)
- Real-time bidirectional communication (use SignalR)
- File serving (use static files middleware)

### Web API vs MVC vs Razor Pages

| Feature | Web API | MVC | Razor Pages |
|---------|---------|-----|-------------|
| **Purpose** | Return data (JSON/XML) | Return views (HTML) | Page-focused HTML |
| **Client** | Any HTTP client | Web browsers | Web browsers |
| **Response** | Structured data | HTML pages | HTML pages |
| **Routing** | Attribute-based | Convention-based | Page-based |
| **Use Case** | APIs, mobile backends | Full web apps | Simple web pages |

---

## REST Principles

**REST** (Representational State Transfer) is an architectural style for designing networked applications.

### Core Principles

1. **Client-Server**: Separation of concerns
2. **Stateless**: Each request contains all information needed
3. **Cacheable**: Responses must define cacheability
4. **Uniform Interface**: Consistent resource addressing
5. **Layered System**: Client can't tell if connected directly to server
6. **Code on Demand** (optional): Server can send executable code

### HTTP Methods (Verbs)

| Method | Purpose | Idempotent | Safe | Example |
|--------|---------|------------|------|---------|
| **GET** | Retrieve resource(s) | ✅ | ✅ | `GET /products` |
| **POST** | Create new resource | ❌ | ❌ | `POST /products` |
| **PUT** | Update entire resource | ✅ | ❌ | `PUT /products/123` |
| **PATCH** | Partial update | ❌ | ❌ | `PATCH /products/123` |
| **DELETE** | Delete resource | ✅ | ❌ | `DELETE /products/123` |
| **HEAD** | Get headers only | ✅ | ✅ | `HEAD /products` |
| **OPTIONS** | Get allowed methods | ✅ | ✅ | `OPTIONS /products` |

**Idempotent**: Multiple identical requests have same effect as single request
**Safe**: Does not modify server state

### RESTful URL Design

```
✅ GOOD RESTful URLs:
GET    /products              # Get all products
GET    /products/123          # Get product with ID 123
POST   /products              # Create new product
PUT    /products/123          # Update product 123
DELETE /products/123          # Delete product 123
GET    /products/123/reviews  # Get reviews for product 123
POST   /products/123/reviews  # Add review to product 123

❌ BAD URLs (Not RESTful):
GET    /getAllProducts
GET    /getProduct?id=123
POST   /createProduct
POST   /updateProduct
GET    /deleteProduct?id=123
GET    /product123reviews
```

### REST Constraints

```csharp
// ✅ GOOD: RESTful design
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // GET /api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        // Return collection
    }

    // GET /api/products/5
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        // Return single resource
    }

    // POST /api/products
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        // Create and return created resource
    }

    // PUT /api/products/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        // Update entire resource
    }

    // DELETE /api/products/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Delete resource
    }
}

// ❌ BAD: Not RESTful
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("getAllProducts")]  // ❌ Redundant
    public IActionResult GetAllProducts() { }

    [HttpPost("createProduct")]  // ❌ Verb in URL
    public IActionResult CreateProduct() { }

    [HttpGet("delete/{id}")]     // ❌ GET for deletion
    public IActionResult DeleteProduct(int id) { }
}
```

---

## Creating Your First API

### Step 1: Create Project

```bash
# Create new Web API project
dotnet new webapi -n MyFirstApi
cd MyFirstApi

# Project structure created:
# MyFirstApi/
# ├── Controllers/
# │   └── WeatherForecastController.cs
# ├── appsettings.json
# ├── Program.cs
# └── MyFirstApi.csproj
```

### Step 2: Examine Program.cs

```csharp
// Program.cs - The entry point
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();  // Register MVC controllers
builder.Services.AddEndpointsApiExplorer();  // Enable API explorer for Swagger
builder.Services.AddSwaggerGen();  // Add Swagger/OpenAPI

var app = builder.Build();

// Configure the HTTP request pipeline (middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Enable Swagger JSON endpoint
    app.UseSwaggerUI();     // Enable Swagger UI (GUI for testing API)
}

app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
app.UseAuthorization();     // Enable authorization
app.MapControllers();       // Map controller routes

app.Run();  // Start the application
```

### Step 3: Create Your First Controller

```csharp
// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;

namespace MyFirstApi.Controllers;

/// <summary>
/// Controller for managing products.
/// [ApiController] enables API-specific behaviors:
/// - Automatic HTTP 400 responses for validation errors
/// - Automatic model binding from request body
/// - Required [FromBody], [FromQuery], etc. attributes
/// </summary>
[ApiController]
[Route("api/[controller]")]  // Route template: api/products
public class ProductsController : ControllerBase
{
    // In-memory storage for demo (use database in real apps)
    private static List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m },
        new Product { Id = 3, Name = "Keyboard", Price = 79.99m }
    };

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>Collection of products</returns>
    /// <response code="200">Returns all products</response>
    [HttpGet]  // GET /api/products
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_products);  // 200 OK with product list
    }

    /// <summary>
    /// Gets a specific product by ID.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with specified ID</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]  // GET /api/products/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return Ok(product);  // 200 OK with product
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    [HttpPost]  // POST /api/products
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> Create(Product product)
    {
        // Generate new ID
        product.Id = _products.Max(p => p.Id) + 1;

        // Add to collection
        _products.Add(product);

        // Return 201 Created with Location header
        // Location: /api/products/4
        return CreatedAtAction(
            nameof(GetById),           // Action name
            new { id = product.Id },   // Route values
            product);                  // Response body
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product data</param>
    /// <returns>No content</returns>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">ID mismatch</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]  // PUT /api/products/5
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, Product product)
    {
        // Validate ID matches
        if (id != product.Id)
        {
            return BadRequest(new { Message = "ID mismatch" });
        }

        // Find existing product
        var existing = _products.FirstOrDefault(p => p.Id == id);
        if (existing == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        // Update properties
        existing.Name = product.Name;
        existing.Price = product.Price;

        // Return 204 No Content (successful update, no body)
        return NoContent();
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]  // DELETE /api/products/5
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        _products.Remove(product);

        return NoContent();  // 204 No Content
    }
}

// Models/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

### Step 4: Run and Test

```bash
# Run the API
dotnet run

# Output:
# Now listening on: https://localhost:7001
# Now listening on: http://localhost:5000

# Open Swagger UI in browser:
# https://localhost:7001/swagger

# Test with curl:
# Get all products
curl https://localhost:7001/api/products

# Get specific product
curl https://localhost:7001/api/products/1

# Create product
curl -X POST https://localhost:7001/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Monitor","price":299.99}'

# Update product
curl -X PUT https://localhost:7001/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Gaming Laptop","price":1499.99}'

# Delete product
curl -X DELETE https://localhost:7001/api/products/1
```

---

## Routing

### Attribute Routing (Recommended ✅)

```csharp
[ApiController]
[Route("api/[controller]")]  // Template: api/{ControllerName}
public class ProductsController : ControllerBase
{
    [HttpGet]                  // GET /api/products
    public IActionResult GetAll() { }

    [HttpGet("{id}")]          // GET /api/products/5
    public IActionResult GetById(int id) { }

    [HttpGet("{id}/reviews")]  // GET /api/products/5/reviews
    public IActionResult GetReviews(int id) { }

    [HttpPost]                 // POST /api/products
    public IActionResult Create() { }

    [HttpPut("{id}")]          // PUT /api/products/5
    public IActionResult Update(int id) { }

    [HttpDelete("{id}")]       // DELETE /api/products/5
    public IActionResult Delete(int id) { }
}
```

### Route Tokens

```csharp
[Route("api/[controller]")]   // [controller] → Controller name without "Controller"
[Route("api/[action]")]       // [action] → Action method name
[Route("api/v{version:apiVersion}/[controller]")]  // With constraints
```

### Route Constraints

```csharp
[HttpGet("{id:int}")]              // Only integers
[HttpGet("{id:int:min(1)}")]       // Integer >= 1
[HttpGet("{id:int:range(1,100)}")] // Integer between 1-100
[HttpGet("{name:alpha}")]          // Only letters
[HttpGet("{code:length(5)}")]      // Exactly 5 characters
[HttpGet("{slug:regex(^[a-z]+$)}")] // Regex pattern
[HttpGet("{date:datetime}")]       // Valid DateTime
[HttpGet("{id:guid}")]             // Valid GUID
```

### Multiple Routes

```csharp
[HttpGet]
[Route("")]                        // GET /api/products
[Route("all")]                     // GET /api/products/all
public IActionResult GetAll() { }

[HttpGet("{id}")]
[Route("by-id/{id}")]              // GET /api/products/by-id/5
public IActionResult GetById(int id) { }
```

### Complex Routing

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    // GET /api/v1/products
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetAllV1() { }

    // GET /api/v2/products
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetAllV2() { }

    // GET /api/v1/products/5/reviews
    [HttpGet("{productId}/reviews")]
    public IActionResult GetReviews(int productId) { }

    // GET /api/v1/products/search?term=laptop&minPrice=500
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string term, [FromQuery] decimal? minPrice) { }

    // GET /api/v1/products/category/electronics/page/1
    [HttpGet("category/{categoryName}/page/{pageNumber:int}")]
    public IActionResult GetByCategory(string categoryName, int pageNumber) { }
}
```

---

## Controllers and Actions

### Controller Base Classes

```csharp
// ControllerBase (for APIs) - Recommended ✅
// No view support, lighter weight
public class ProductsController : ControllerBase
{
    // API-specific helpers:
    // - Ok(), NotFound(), BadRequest(), etc.
    // - CreatedAtAction(), AcceptedAtAction()
    // - File(), Redirect()
}

// Controller (for MVC with views) - For web pages ❌
public class HomeController : Controller
{
    // Includes view-related methods:
    // - View(), PartialView(), ViewComponent()
    // Not needed for APIs!
}
```

### Action Method Return Types

```csharp
public class ProductsController : ControllerBase
{
    // 1. Specific type - No status code control
    [HttpGet("simple")]
    public Product GetSimple()
    {
        return new Product { Id = 1, Name = "Laptop" };
        // Always returns 200 OK
        // Cannot return 404, 400, etc.
    }

    // 2. IActionResult - Flexible, but no compile-time type checking
    [HttpGet("flexible")]
    public IActionResult GetFlexible()
    {
        var product = GetProduct();
        if (product == null)
            return NotFound();  // 404

        return Ok(product);  // 200
        // Type information lost
    }

    // 3. ActionResult<T> - Best of both worlds ✅
    [HttpGet("best/{id}")]
    public ActionResult<Product> GetBest(int id)
    {
        var product = GetProduct(id);
        if (product == null)
            return NotFound();  // 404

        return Ok(product);  // 200 with type safety
        // Or simply: return product;
        // Swagger knows return type!
    }

    // 4. Task<ActionResult<T>> - Async version ✅
    [HttpGet("async/{id}")]
    public async Task<ActionResult<Product>> GetAsync(int id)
    {
        var product = await GetProductAsync(id);
        if (product == null)
            return NotFound();

        return product;  // Implicit conversion to ActionResult<Product>
    }
}
```

### Action Method Parameters

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // From route
    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        // id comes from URL path
    }

    // From query string
    [HttpGet("search")]
    public IActionResult Search(
        [FromQuery] string term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // GET /api/products/search?term=laptop&page=1&pageSize=20
    }

    // From body (default for complex types with [ApiController])
    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        // Product deserialized from JSON request body
    }

    // From header
    [HttpGet]
    public IActionResult GetAll([FromHeader(Name = "X-API-Key")] string apiKey)
    {
        // apiKey from HTTP header
    }

    // From form (for multipart/form-data)
    [HttpPost("upload")]
    public IActionResult Upload([FromForm] IFormFile file, [FromForm] string description)
    {
        // For file uploads
    }

    // Multiple sources
    [HttpPut("{id}")]
    public IActionResult Update(
        [FromRoute] int id,
        [FromBody] Product product,
        [FromHeader(Name = "If-Match")] string etag)
    {
        // id from route, product from body, etag from header
    }
}
```

---

## Model Binding and Validation

### Model Validation with Data Annotations

```csharp
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3-100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1000000, ErrorMessage = "Price must be between $0.01 and $1,000,000")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "SKU format: ABC123")]
    public string SKU { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? SupplierEmail { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? ProductUrl { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? ContactPhone { get; set; }
}

// Controller automatically validates with [ApiController]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        // [ApiController] automatically checks ModelState
        // If validation fails, automatically returns 400 Bad Request with errors

        // No need for this with [ApiController]:
        // if (!ModelState.IsValid)
        //     return BadRequest(ModelState);

        // Validation passed - proceed
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // Manual validation (without [ApiController])
    [HttpPost("manual")]
    public ActionResult<Product> CreateManual(Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
            // Returns:
            // {
            //   "errors": {
            //     "Name": ["Product name is required"],
            //     "Price": ["Price must be between $0.01 and $1,000,000"]
            //   }
            // }
        }

        // Continue...
    }
}
```

### Custom Validation

```csharp
// Custom validation attribute
public class ValidSKUAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string sku)
        {
            // Custom logic: SKU must start with product category code
            if (!sku.StartsWith("ELEC") && !sku.StartsWith("FURN") && !sku.StartsWith("CLTH"))
            {
                return new ValidationResult("SKU must start with ELEC, FURN, or CLTH");
            }
        }

        return ValidationResult.Success;
    }
}

// Usage
public class Product
{
    [Required]
    [ValidSKU]
    public string SKU { get; set; } = string.Empty;
}

// IValidatableObject for complex validation
public class Order : IValidatableObject
{
    public DateTime OrderDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Shipping date must be after order date
        if (ShippingDate.HasValue && ShippingDate.Value < OrderDate)
        {
            yield return new ValidationResult(
                "Shipping date must be after order date",
                new[] { nameof(ShippingDate) });
        }

        // Total must equal subtotal + tax
        if (Math.Abs(Total - (Subtotal + Tax)) > 0.01m)
        {
            yield return new ValidationResult(
                "Total must equal Subtotal + Tax",
                new[] { nameof(Total) });
        }
    }
}
```

---

## Response Types

### Common Response Helpers

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // 200 OK
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _repository.GetById(id);
        return Ok(product);  // 200 with product
        // Or: return product; (implicit conversion)
    }

    // 201 Created
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        _repository.Add(product);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
        // Response includes Location header: /api/products/5
    }

    // 204 No Content
    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        _repository.Update(product);
        return NoContent();  // 204 (success, no body)
    }

    // 400 Bad Request
    [HttpPost("validate")]
    public IActionResult ValidateProduct(Product product)
    {
        if (product.Price < 0)
            return BadRequest("Price cannot be negative");

        return Ok();
    }

    // 404 Not Found
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _repository.GetById(id);
        if (product == null)
            return NotFound($"Product with ID {id} not found");

        return product;
    }

    // 409 Conflict
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        if (_repository.Exists(product.SKU))
            return Conflict($"Product with SKU {product.SKU} already exists");

        _repository.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // 500 Internal Server Error (handled by exception middleware)
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        try
        {
            var product = _repository.GetById(id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
```

### Custom Response Models

```csharp
// API response wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

// Pagination response
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

// Usage
[HttpGet]
public ActionResult<PagedResponse<Product>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var totalItems = _repository.Count();
    var items = _repository.GetPage(page, pageSize);

    var response = new PagedResponse<Product>
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalItems = totalItems
    };

    return Ok(response);
}
```

---

## HTTP Status Codes

### Complete Status Code Guide

| Code | Name | When to Use |
|------|------|-------------|
| **200** | OK | Successful GET, PUT, PATCH |
| **201** | Created | Successful POST (resource created) |
| **204** | No Content | Successful DELETE, PUT (no response body) |
| **400** | Bad Request | Invalid request data |
| **401** | Unauthorized | Authentication required |
| **403** | Forbidden | Authenticated but not authorized |
| **404** | Not Found | Resource doesn't exist |
| **405** | Method Not Allowed | Invalid HTTP method for endpoint |
| **409** | Conflict | Resource conflict (duplicate) |
| **422** | Unprocessable Entity | Validation failed |
| **429** | Too Many Requests | Rate limit exceeded |
| **500** | Internal Server Error | Server error |
| **503** | Service Unavailable | Temporary server unavailability |

### Example: Complete CRUD with Proper Status Codes

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    // GET /api/products
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);  // 200 OK
    }

    // GET /api/products/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound();  // 404 Not Found

        return Ok(product);  // 200 OK
    }

    // POST /api/products
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        // Check for duplicates
        if (await _repository.ExistsAsync(product.SKU))
            return Conflict($"Product with SKU {product.SKU} already exists");  // 409 Conflict

        var created = await _repository.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);  // 201 Created
    }

    // PUT /api/products/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest("ID mismatch");  // 400 Bad Request

        if (!await _repository.ExistsAsync(id))
            return NotFound();  // 404 Not Found

        await _repository.UpdateAsync(product);
        return NoContent();  // 204 No Content
    }

    // DELETE /api/products/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repository.ExistsAsync(id))
            return NotFound();  // 404 Not Found

        await _repository.DeleteAsync(id);
        return NoContent();  // 204 No Content
    }
}
```

---

## Minimal APIs

**Minimal APIs** (introduced in .NET 6) provide a simplified approach without controllers.

### Basic Minimal API

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory storage
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 999.99m },
    new() { Id = 2, Name = "Mouse", Price = 29.99m }
};

// GET /products
app.MapGet("/products", () => products);

// GET /products/5
app.MapGet("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

// POST /products
app.MapPost("/products", (Product product) =>
{
    product.Id = products.Max(p => p.Id) + 1;
    products.Add(product);
    return Results.Created($"/products/{product.Id}", product);
});

// PUT /products/5
app.MapPut("/products/{id}", (int id, Product product) =>
{
    var existing = products.FirstOrDefault(p => p.Id == id);
    if (existing is null)
        return Results.NotFound();

    existing.Name = product.Name;
    existing.Price = product.Price;
    return Results.NoContent();
});

// DELETE /products/5
app.MapDelete("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null)
        return Results.NotFound();

    products.Remove(product);
    return Results.NoContent();
});

app.Run();
```

### Minimal API with Dependency Injection

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Services are automatically injected
app.MapGet("/products", async (IProductService service) =>
{
    var products = await service.GetAllAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/products", async (Product product, IProductService service) =>
{
    var created = await service.CreateAsync(product);
    return Results.Created($"/products/{created.Id}", created);
});

app.Run();
```

### Minimal API with Grouping

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Group related endpoints
var productsGroup = app.MapGroup("/api/products")
    .WithTags("Products")
    .WithOpenApi();

productsGroup.MapGet("/", async (IProductRepository repo) =>
    await repo.GetAllAsync());

productsGroup.MapGet("/{id}", async (int id, IProductRepository repo) =>
    await repo.GetByIdAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound());

productsGroup.MapPost("/", async (Product product, IProductRepository repo) =>
{
    var created = await repo.AddAsync(product);
    return Results.Created($"/api/products/{created.Id}", created);
});

app.Run();
```

### Controllers vs Minimal APIs

| Feature | Controllers | Minimal APIs |
|---------|-------------|--------------|
| **Syntax** | Class-based | Functional |
| **Verbosity** | More verbose | Concise |
| **Organization** | File per controller | All in Program.cs or extensions |
| **Features** | Full MVC pipeline | Lightweight |
| **Filters** | Many built-in | Limited |
| **Model Binding** | Rich | Basic |
| **Best For** | Large APIs | Microservices, simple APIs |

---

## Middleware Pipeline

### Understanding the Pipeline

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware executes in order added
app.Use(async (context, next) =>
{
    Console.WriteLine("1. Before");
    await next();  // Call next middleware
    Console.WriteLine("1. After");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("2. Before");
    await next();
    Console.WriteLine("2. After");
});

app.MapGet("/", () => "Hello!");

app.Run();

// Request flow:
// 1. Before
// 2. Before
// Execute endpoint: "Hello!"
// 2. After
// 1. After
```

### Common Middleware

```csharp
var app = builder.Build();

// Exception handling (must be first!)
app.UseExceptionHandler("/error");

// HTTPS redirection
app.UseHttpsRedirection();

// Static files (wwwroot)
app.UseStaticFiles();

// Routing
app.UseRouting();

// CORS
app.UseCors("AllowAll");

// Authentication (who are you?)
app.UseAuthentication();

// Authorization (what can you do?)
app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.Run();
```

### Custom Middleware

```csharp
// Middleware class
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;

        _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);  // Call next middleware

        var duration = DateTime.UtcNow - start;
        _logger.LogInformation(
            "Response: {StatusCode} in {Duration}ms",
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}

// Extension method
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}

// Usage
app.UseRequestLogging();
```

---

## Best Practices

### 1. Use [ApiController] Attribute

```csharp
// ✅ GOOD: Enables automatic validation, binding, etc.
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase { }
```

### 2. Use ActionResult<T>

```csharp
// ✅ GOOD: Type-safe with flexibility
[HttpGet("{id}")]
public async Task<ActionResult<Product>> GetById(int id)
{
    var product = await _service.GetByIdAsync(id);
    return product ?? NotFound();
}
```

### 3. Use Async/Await

```csharp
// ✅ GOOD: Async for I/O operations
[HttpGet]
public async Task<ActionResult<IEnumerable<Product>>> GetAll()
{
    var products = await _repository.GetAllAsync();
    return Ok(products);
}
```

### 4. Use ProducesResponseType

```csharp
// ✅ GOOD: Documents responses for Swagger
[HttpGet("{id}")]
[ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<Product>> GetById(int id) { }
```

### 5. Version Your API

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ProductsV1Controller : ControllerBase { }

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class ProductsV2Controller : ControllerBase { }
```

### 6. Use DTOs (Data Transfer Objects)

```csharp
// ✅ GOOD: Separate API models from domain models
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }
}

[HttpPost]
public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
{
    var product = _mapper.Map<Product>(dto);
    var created = await _repository.AddAsync(product);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ProductDto>(created));
}
```

### 7. Handle Exceptions Globally

```csharp
// Program.cs
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var response = new
        {
            Error = exception?.Message ?? "An error occurred",
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    });
});
```

---

## Summary

**Key Takeaways:**

1. **ASP.NET Core Web API** builds HTTP services consumed by diverse clients
2. **REST principles** guide URL design and HTTP method usage
3. **Attribute routing** is the modern, recommended approach
4. **[ApiController]** enables automatic validation and model binding
5. **ActionResult<T>** provides type safety with flexibility
6. **Async/await** is essential for scalable APIs
7. **Minimal APIs** offer a lightweight alternative to controllers
8. **Middleware pipeline** processes requests in order
9. **Status codes** communicate operation results clearly
10. **Best practices** include DTOs, versioning, and global exception handling

---

## What's Next?

In the next lessons, we'll explore:
- **Lesson 02**: Authentication & Authorization (JWT, OAuth)
- **Lesson 03**: Advanced API Features (Filtering, Sorting, Pagination)
- **Lesson 04**: API Documentation with Swagger/OpenAPI
- **Lesson 05**: Testing Web APIs

---

**Ready to practice?** Check out the exercises in `exercises/` directory!
