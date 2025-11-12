# Lesson 01: Entity Framework Core Introduction

## 🎯 Learning Objectives

By the end of this lesson, you will understand:

- **What** Entity Framework Core (EF Core) is and why use it
- **DbContext** and **DbSet** fundamentals
- **Code First** approach to database development
- **Entity relationships**: One-to-One, One-to-Many, Many-to-Many
- **Migrations** for database schema management
- **CRUD operations** with EF Core
- **Querying** data with LINQ to Entities
- **Change tracking** and how EF Core monitors entities
- **Loading strategies**: Eager, Lazy, and Explicit loading
- **Best practices** for production applications

## 📚 Table of Contents

1. [What is Entity Framework Core?](#what-is-entity-framework-core)
2. [Setting Up EF Core](#setting-up-ef-core)
3. [DbContext and DbSet](#dbcontext-and-dbset)
4. [Defining Entities](#defining-entities)
5. [Relationships](#relationships)
6. [Migrations](#migrations)
7. [CRUD Operations](#crud-operations)
8. [Querying Data](#querying-data)
9. [Change Tracking](#change-tracking)
10. [Loading Strategies](#loading-strategies)
11. [Best Practices](#best-practices)

---

## What is Entity Framework Core?

**Entity Framework Core (EF Core)** is a modern object-relational mapper (ORM) that enables .NET developers to work with databases using .NET objects, eliminating the need for most data-access code.

### The Problem: Manual SQL

```csharp
// ❌ Without EF Core: Manual SQL (tedious, error-prone)
public class ProductRepository
{
    private readonly SqlConnection _connection;

    public async Task<Product> GetByIdAsync(int id)
    {
        using var command = new SqlCommand(
            "SELECT Id, Name, Price, CategoryId FROM Products WHERE Id = @Id",
            _connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                CategoryId = reader.GetInt32(3)
            };
        }
        return null;
    }

    public async Task AddAsync(Product product)
    {
        using var command = new SqlCommand(
            "INSERT INTO Products (Name, Price, CategoryId) VALUES (@Name, @Price, @CategoryId); SELECT SCOPE_IDENTITY();",
            _connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

        product.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    // Lots more boilerplate code...
}
```

**Problems:**
- ❌ SQL strings are error-prone (typos, SQL injection)
- ❌ Manual mapping between database and objects
- ❌ No compile-time checking
- ❌ Lots of repetitive code
- ❌ Difficult to maintain relationships
- ❌ No automatic change tracking

### The Solution: EF Core

```csharp
// ✅ With EF Core: Clean, type-safe, maintainable
public class ProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)  // Load related data
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;  // Id automatically populated!
    }

    public async Task<IEnumerable<Product>> SearchAsync(string term, decimal? minPrice)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(term))
            .Where(p => !minPrice.HasValue || p.Price >= minPrice.Value)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
```

**Benefits:**
- ✅ Type-safe LINQ queries
- ✅ Automatic SQL generation
- ✅ Compile-time checking
- ✅ Automatic change tracking
- ✅ Easy relationship navigation
- ✅ Database migrations
- ✅ Cross-platform (SQL Server, PostgreSQL, SQLite, MySQL, etc.)

### EF Core vs EF 6

| Feature | EF Core | EF 6 |
|---------|---------|------|
| **Platform** | Cross-platform | Windows only |
| **Performance** | Much faster | Slower |
| **Lightweight** | Yes | No |
| **Supported DBs** | Many (SQL Server, PostgreSQL, SQLite, etc.) | SQL Server, MySQL |
| **Lazy Loading** | Opt-in | Default |
| **Spatial Data** | Yes | Limited |
| **Global Query Filters** | Yes | No |
| **Table Per Hierarchy** | Yes | Yes |
| **Table Per Type** | Yes | Yes |
| **Many-to-Many** | Direct support (EF Core 5+) | Requires join entity |

**Recommendation:** Use EF Core for all new projects.

---

## Setting Up EF Core

### Step 1: Install NuGet Packages

```bash
# Core EF Core package
dotnet add package Microsoft.EntityFrameworkCore

# Database provider (choose one)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer  # SQL Server
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL    # PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Sqlite     # SQLite
dotnet add package Pomelo.EntityFrameworkCore.MySql         # MySQL

# Design-time tools for migrations
dotnet add package Microsoft.EntityFrameworkCore.Design

# Optional: For ASP.NET Core apps
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### Step 2: Define Entity Classes

```csharp
// Models/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    // Navigation property
    public Category Category { get; set; } = null!;
}

// Models/Category.cs
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property (collection)
    public List<Product> Products { get; set; } = new();
}
```

### Step 3: Create DbContext

```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // DbSet represents a table
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    // Constructor for dependency injection
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Configure model (optional - conventions work for most cases)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });
    }
}
```

### Step 4: Register DbContext in DI Container

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register DbContext with dependency injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
```

### Step 5: Add Connection String

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Step 6: Create and Apply Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update

# Database is now created with Products and Categories tables!
```

---

## DbContext and DbSet

### DbContext: The Gateway to Database

**DbContext** is the primary class for interacting with the database. It:
- Represents a session with the database
- Manages entity instances and their state
- Provides querying capabilities
- Handles change tracking
- Manages database connections
- Executes database operations

```csharp
public class AppDbContext : DbContext
{
    // DbSets - each represents a table
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Configuration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entities using Fluent API
    }

    // Optional: Override SaveChanges for logging
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Log changes
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            Console.WriteLine($"{entry.State}: {entry.Entity.GetType().Name}");
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### DbSet: Table Access

**DbSet<TEntity>** represents a collection of entities (a database table).

```csharp
public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ExampleOperations()
    {
        // Query
        var products = await _context.Products.ToListAsync();

        // Add
        _context.Products.Add(new Product { Name = "New Product", Price = 99.99m });

        // Update (after retrieving)
        var product = await _context.Products.FindAsync(1);
        if (product != null)
        {
            product.Price = 109.99m;
        }

        // Delete
        var toDelete = await _context.Products.FindAsync(2);
        if (toDelete != null)
        {
            _context.Products.Remove(toDelete);
        }

        // Save all changes
        await _context.SaveChangesAsync();
    }
}
```

### DbContext Lifetime

**Important:** DbContext should be short-lived (scoped per request in web apps).

```csharp
// ✅ GOOD: Scoped lifetime (per HTTP request)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);  // Default

// ❌ BAD: Singleton lifetime
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Singleton);  // DON'T DO THIS!

// Why scoped?
// - DbContext is NOT thread-safe
// - Change tracking accumulates over time (memory leak)
// - Database connections should be short-lived
```

---

## Defining Entities

### Basic Entity

```csharp
/// <summary>
/// Product entity mapped to Products table.
/// </summary>
public class Product
{
    // Primary key (by convention: Id or {ClassName}Id)
    public int Id { get; set; }

    // Required string (non-nullable)
    public string Name { get; set; } = string.Empty;

    // Optional string (nullable)
    public string? Description { get; set; }

    // Decimal property
    public decimal Price { get; set; }

    // DateTime property
    public DateTime CreatedAt { get; set; }

    // Boolean property
    public bool IsActive { get; set; }

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation property
    public Category Category { get; set; } = null!;
}
```

### Data Annotations

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Products")]  // Specify table name
public class Product
{
    [Key]  // Explicit primary key
    public int Id { get; set; }

    [Required]  // NOT NULL constraint
    [StringLength(200)]  // VARCHAR(200)
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]  // VARCHAR(1000)
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]  // Precision and scale
    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Column("SKU")]  // Map to different column name
    [StringLength(50)]
    public string StockKeepingUnit { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-increment
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]  // Computed column
    public DateTime LastModified { get; set; }

    [NotMapped]  // Don't map to database
    public string DisplayName => $"{Name} (${Price})";

    [ForeignKey("Category")]  // Explicit foreign key
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
```

### Fluent API (Preferred ✅)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        // Table name
        entity.ToTable("Products");

        // Primary key
        entity.HasKey(p => p.Id);

        // Properties
        entity.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        entity.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");  // SQL Server default

        // Ignore property
        entity.Ignore(p => p.DisplayName);

        // Index
        entity.HasIndex(p => p.Name);

        // Unique constraint
        entity.HasIndex(p => p.SKU).IsUnique();

        // Composite index
        entity.HasIndex(p => new { p.CategoryId, p.Name });
    });
}
```

**Why Fluent API over Data Annotations?**
- ✅ Keeps entity classes clean (no infrastructure attributes)
- ✅ More powerful configuration options
- ✅ Better for complex scenarios
- ✅ Easier to maintain large models

---

## Relationships

### One-to-Many (1:N)

**Example:** One Category has many Products

```csharp
// Principal entity (One side)
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property: Collection
    public List<Product> Products { get; set; } = new();
}

// Dependent entity (Many side)
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation property: Reference
    public Category Category { get; set; } = null!;
}

// Configuration (optional - conventions handle this)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)         // Product has one Category
        .WithMany(c => c.Products)       // Category has many Products
        .HasForeignKey(p => p.CategoryId) // Foreign key property
        .OnDelete(DeleteBehavior.Cascade); // Delete behavior
}
```

### One-to-One (1:1)

**Example:** One User has one UserProfile

```csharp
// Principal entity
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    // Navigation property
    public UserProfile Profile { get; set; } = null!;
}

// Dependent entity
public class UserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }

    // Foreign key (same as PK in 1:1)
    public int UserId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}

// Configuration
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasOne(u => u.Profile)           // User has one Profile
        .WithOne(p => p.User)             // Profile has one User
        .HasForeignKey<UserProfile>(p => p.UserId); // FK on dependent side
}
```

### Many-to-Many (M:N)

**Example:** Students and Courses (many students take many courses)

```csharp
// EF Core 5+ : Direct Many-to-Many (No join entity needed!)
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property: Collection
    public List<Course> Courses { get; set; } = new();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // Navigation property: Collection
    public List<Student> Students { get; set; } = new();
}

// Configuration (optional - conventions handle this)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Student>()
        .HasMany(s => s.Courses)
        .WithMany(c => c.Students);
    // EF Core automatically creates join table: CourseStudent
}

// Explicit Join Entity (when you need extra properties)
public class StudentCourse
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    // Additional properties
    public DateTime EnrollmentDate { get; set; }
    public int? Grade { get; set; }
}

// Configuration with join entity
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<StudentCourse>()
        .HasKey(sc => new { sc.StudentId, sc.CourseId }); // Composite key

    modelBuilder.Entity<StudentCourse>()
        .HasOne(sc => sc.Student)
        .WithMany(s => s.StudentCourses)
        .HasForeignKey(sc => sc.StudentId);

    modelBuilder.Entity<StudentCourse>()
        .HasOne(sc => sc.Course)
        .WithMany(c => c.StudentCourses)
        .HasForeignKey(sc => sc.CourseId);
}
```

---

## Migrations

**Migrations** create and update the database schema based on your model.

### Creating Migrations

```bash
# Add a new migration
dotnet ef migrations add InitialCreate

# Generated files in Migrations folder:
# - 20251112120000_InitialCreate.cs (Up/Down methods)
# - 20251112120000_InitialCreate.Designer.cs (metadata)
# - AppDbContextModelSnapshot.cs (current model state)
```

### Migration File

```csharp
public partial class InitialCreate : Migration
{
    // Apply migration (migrate up)
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CategoryId = table.Column<int>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    // Rollback migration (migrate down)
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Products");
        migrationBuilder.DropTable(name: "Categories");
    }
}
```

### Applying Migrations

```bash
# Apply all pending migrations
dotnet ef database update

# Apply to specific migration
dotnet ef database update InitialCreate

# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Rollback all migrations (delete database)
dotnet ef database update 0

# Remove last migration (if not applied yet)
dotnet ef migrations remove
```

### Seeding Data

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Seed initial data
    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Books" },
        new Category { Id = 3, Name = "Clothing" }
    );

    modelBuilder.Entity<Product>().HasData(
        new Product { Id = 1, Name = "Laptop", Price = 999.99m, CategoryId = 1 },
        new Product { Id = 2, Name = "C# Programming", Price = 49.99m, CategoryId = 2 },
        new Product { Id = 3, Name = "T-Shirt", Price = 19.99m, CategoryId = 3 }
    );
}

// Create migration for seed data
// dotnet ef migrations add SeedData
// dotnet ef database update
```

---

## CRUD Operations

### Create (Insert)

```csharp
public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Create single entity
    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;  // Id is now populated
    }

    // Create multiple entities
    public async Task<IEnumerable<Product>> CreateManyAsync(List<Product> products)
    {
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        return products;
    }
}
```

### Read (Query)

```csharp
// Get all
public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products.ToListAsync();
}

// Get by ID
public async Task<Product?> GetByIdAsync(int id)
{
    return await _context.Products.FindAsync(id);
    // Or:
    // return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
}

// Get with filter
public async Task<List<Product>> GetByCategoryAsync(int categoryId)
{
    return await _context.Products
        .Where(p => p.CategoryId == categoryId)
        .ToListAsync();
}

// Get with complex query
public async Task<List<Product>> SearchAsync(string searchTerm, decimal? minPrice, decimal? maxPrice)
{
    IQueryable<Product> query = _context.Products;

    if (!string.IsNullOrEmpty(searchTerm))
    {
        query = query.Where(p => p.Name.Contains(searchTerm));
    }

    if (minPrice.HasValue)
    {
        query = query.Where(p => p.Price >= minPrice.Value);
    }

    if (maxPrice.HasValue)
    {
        query = query.Where(p => p.Price <= maxPrice.Value);
    }

    return await query
        .OrderBy(p => p.Name)
        .ToListAsync();
}
```

### Update

```csharp
// Update entire entity
public async Task UpdateAsync(Product product)
{
    _context.Products.Update(product);
    await _context.SaveChangesAsync();
}

// Update specific properties
public async Task UpdatePriceAsync(int id, decimal newPrice)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
        throw new KeyNotFoundException($"Product {id} not found");

    product.Price = newPrice;
    // No need to call Update() - entity is tracked!
    await _context.SaveChangesAsync();
}

// Bulk update (EF Core 7+)
public async Task IncreaseAllPricesAsync(decimal percentage)
{
    await _context.Products
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(p => p.Price, p => p.Price * (1 + percentage / 100)));
}
```

### Delete

```csharp
// Delete by entity
public async Task DeleteAsync(Product product)
{
    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
}

// Delete by ID
public async Task DeleteByIdAsync(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
        throw new KeyNotFoundException($"Product {id} not found");

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
}

// Bulk delete (EF Core 7+)
public async Task DeleteDiscontinuedAsync()
{
    await _context.Products
        .Where(p => !p.IsActive)
        .ExecuteDeleteAsync();
}
```

---

## Querying Data

### LINQ to Entities

```csharp
// Filtering
var expensiveProducts = await _context.Products
    .Where(p => p.Price > 100)
    .ToListAsync();

// Ordering
var sortedProducts = await _context.Products
    .OrderBy(p => p.Name)
    .ThenByDescending(p => p.Price)
    .ToListAsync();

// Projection (Select)
var productNames = await _context.Products
    .Select(p => p.Name)
    .ToListAsync();

var productDtos = await _context.Products
    .Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name
    })
    .ToListAsync();

// Aggregation
var totalProducts = await _context.Products.CountAsync();
var averagePrice = await _context.Products.AverageAsync(p => p.Price);
var maxPrice = await _context.Products.MaxAsync(p => p.Price);
var totalValue = await _context.Products.SumAsync(p => p.Price);

// Grouping
var productsByCategory = await _context.Products
    .GroupBy(p => p.CategoryId)
    .Select(g => new
    {
        CategoryId = g.Key,
        Count = g.Count(),
        AveragePrice = g.Average(p => p.Price)
    })
    .ToListAsync();

// Pagination
var page = 1;
var pageSize = 10;
var pagedProducts = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Any / All
var hasExpensiveProducts = await _context.Products.AnyAsync(p => p.Price > 1000);
var allInStock = await _context.Products.AllAsync(p => p.IsActive);

// First / Single
var firstProduct = await _context.Products.FirstAsync();  // Throws if empty
var firstOrDefault = await _context.Products.FirstOrDefaultAsync();  // Returns null if empty
var singleProduct = await _context.Products.SingleAsync(p => p.Id == 1);  // Throws if 0 or >1
```

### Raw SQL Queries

```csharp
// FromSqlRaw for queries
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 100)
    .ToListAsync();

// Can combine with LINQ
var expensiveElectronics = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > 100")
    .Where(p => p.CategoryId == 1)
    .OrderBy(p => p.Name)
    .ToListAsync();

// ExecuteSqlRaw for non-query operations
var affectedRows = await _context.Database.ExecuteSqlRawAsync(
    "UPDATE Products SET Price = Price * 1.1 WHERE CategoryId = {0}", 1);

// Stored procedures
var products = await _context.Products
    .FromSqlRaw("EXEC GetProductsByCategory @CategoryId = {0}", categoryId)
    .ToListAsync();
```

---

## Change Tracking

EF Core tracks changes to entities automatically.

```csharp
public async Task DemonstrateChangeTracking()
{
    // Load entity
    var product = await _context.Products.FindAsync(1);
    Console.WriteLine(_context.Entry(product).State);  // Output: Unchanged

    // Modify entity
    product.Price = 199.99m;
    Console.WriteLine(_context.Entry(product).State);  // Output: Modified

    // SaveChanges generates UPDATE statement
    await _context.SaveChangesAsync();
    Console.WriteLine(_context.Entry(product).State);  // Output: Unchanged
}

// View tracked entities
public void ViewTrackedEntities()
{
    var trackedEntities = _context.ChangeTracker.Entries()
        .Where(e => e.State != EntityState.Detached)
        .Select(e => new
        {
            Entity = e.Entity.GetType().Name,
            State = e.State
        });

    foreach (var entry in trackedEntities)
    {
        Console.WriteLine($"{entry.Entity}: {entry.State}");
    }
}

// Disable tracking for read-only queries (performance optimization)
public async Task<List<Product>> GetAllReadOnlyAsync()
{
    return await _context.Products
        .AsNoTracking()  // Don't track these entities
        .ToListAsync();
}
```

### Entity States

| State | Description |
|-------|-------------|
| **Detached** | Not tracked by context |
| **Unchanged** | Tracked, no changes since loaded |
| **Added** | New entity, will be inserted |
| **Modified** | Tracked, has changes, will be updated |
| **Deleted** | Tracked, will be deleted |

---

## Loading Strategies

### Eager Loading (Include)

Load related data upfront with `.Include()`.

```csharp
// Load products with categories
public async Task<List<Product>> GetProductsWithCategories()
{
    return await _context.Products
        .Include(p => p.Category)  // JOIN to Categories table
        .ToListAsync();
}

// Load multiple levels
public async Task<List<Order>> GetOrdersWithDetails()
{
    return await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)  // Load nested relationship
        .ToListAsync();
}

// SQL generated:
// SELECT * FROM Orders o
// LEFT JOIN Customers c ON o.CustomerId = c.Id
// LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
// LEFT JOIN Products p ON oi.ProductId = p.Id
```

### Explicit Loading

Load related data on demand.

```csharp
public async Task ExplicitLoadingExample()
{
    // Load product without category
    var product = await _context.Products.FindAsync(1);

    // Later, explicitly load category
    await _context.Entry(product)
        .Reference(p => p.Category)  // For single navigation property
        .LoadAsync();

    // For collections
    var category = await _context.Categories.FindAsync(1);
    await _context.Entry(category)
        .Collection(c => c.Products)  // For collection navigation property
        .LoadAsync();

    // With query
    await _context.Entry(category)
        .Collection(c => c.Products)
        .Query()
        .Where(p => p.Price > 100)
        .LoadAsync();
}
```

### Lazy Loading (Not Recommended)

⚠️ Lazy loading loads related data automatically when accessed, but can cause N+1 query problems.

```bash
# Enable lazy loading
dotnet add package Microsoft.EntityFrameworkCore.Proxies
```

```csharp
// Enable in DbContext
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseLazyLoadingProxies();
}

// Make navigation properties virtual
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;  // virtual enables lazy loading
}

// Usage - Category loaded automatically when accessed
var product = await _context.Products.FindAsync(1);
Console.WriteLine(product.Category.Name);  // Triggers separate query!

// ⚠️ N+1 Problem:
var products = await _context.Products.ToListAsync();  // 1 query
foreach (var product in products)
{
    Console.WriteLine(product.Category.Name);  // N queries (one per product!)
}
// Solution: Use .Include() for eager loading
```

---

## Best Practices

### 1. Use Async Methods

```csharp
// ✅ GOOD: Async for I/O operations
public async Task<Product?> GetByIdAsync(int id)
{
    return await _context.Products.FindAsync(id);
}

// ❌ BAD: Sync methods block threads
public Product? GetById(int id)
{
    return _context.Products.Find(id);
}
```

### 2. Use AsNoTracking for Read-Only Queries

```csharp
// ✅ GOOD: No tracking overhead for read-only data
public async Task<List<ProductDto>> GetProductDtosAsync()
{
    return await _context.Products
        .AsNoTracking()
        .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
        .ToListAsync();
}
```

### 3. Project to DTOs

```csharp
// ✅ GOOD: Only select needed columns
public async Task<List<ProductListDto>> GetProductListAsync()
{
    return await _context.Products
        .Select(p => new ProductListDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync();
}

// ❌ BAD: Loading entire entities with all columns
public async Task<List<Product>> GetAllProductsAsync()
{
    return await _context.Products.ToListAsync();  // Loads all columns!
}
```

### 4. Dispose DbContext Properly

```csharp
// ✅ GOOD: Use dependency injection (automatically disposed)
public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;  // DI container handles disposal
    }
}

// ✅ GOOD: Manual creation with using
public async Task ManualExample()
{
    using var context = new AppDbContext();
    var products = await context.Products.ToListAsync();
}

// ❌ BAD: Not disposing
public async Task BadExample()
{
    var context = new AppDbContext();
    var products = await context.Products.ToListAsync();
    // Context never disposed - memory leak!
}
```

### 5. Use Include Wisely

```csharp
// ✅ GOOD: Include when you need related data
public async Task<Product?> GetProductWithCategoryAsync(int id)
{
    return await _context.Products
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.Id == id);
}

// ❌ BAD: Over-including (loading too much data)
public async Task<List<Product>> GetAllWithEverything()
{
    return await _context.Products
        .Include(p => p.Category)
            .ThenInclude(c => c.Subcategories)
        .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
        .Include(p => p.Images)
        .ToListAsync();
    // Massive JOIN query - very slow!
}
```

### 6. Batch Operations

```csharp
// ✅ GOOD: Batch operations
public async Task AddManyAsync(List<Product> products)
{
    _context.Products.AddRange(products);
    await _context.SaveChangesAsync();  // Single round-trip
}

// ❌ BAD: Multiple SaveChanges calls
public async Task AddManyBadAsync(List<Product> products)
{
    foreach (var product in products)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();  // Round-trip per product!
    }
}
```

### 7. Handle Concurrency

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [Timestamp]  // Concurrency token
    public byte[] RowVersion { get; set; } = null!;
}

public async Task UpdateWithConcurrencyCheck(Product product)
{
    try
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // Handle concurrency conflict
        var entry = ex.Entries.Single();
        var databaseValues = await entry.GetDatabaseValuesAsync();

        if (databaseValues == null)
        {
            // Entity was deleted
            throw new Exception("Product was deleted by another user");
        }
        else
        {
            // Entity was modified by another user
            throw new Exception("Product was modified by another user");
        }
    }
}
```

---

## Summary

**Key Takeaways:**

1. **EF Core** is a modern, cross-platform ORM
2. **DbContext** represents a database session
3. **DbSet<TEntity>** represents a table
4. **Migrations** manage database schema changes
5. **Relationships**: One-to-Many, One-to-One, Many-to-Many
6. **CRUD operations** are simple with EF Core
7. **LINQ to Entities** provides type-safe queries
8. **Change tracking** monitors entity state
9. **Loading strategies**: Eager (Include), Explicit, Lazy
10. **Best practices**: Async, AsNoTracking, projections, proper disposal

**Performance Tips:**
- Use `AsNoTracking()` for read-only queries
- Project to DTOs instead of loading full entities
- Use `.Include()` for eager loading (avoid N+1)
- Batch operations with `AddRange()` / `SaveChanges()`
- Avoid lazy loading in production

---

## What's Next?

In the next lessons, we'll explore:
- **Lesson 02**: Advanced Querying (Complex LINQ, SQL, Specifications)
- **Lesson 03**: Performance Optimization (Indexes, Query Plans, Batching)
- **Lesson 04**: Advanced Patterns (Repository, Unit of Work, CQRS)

---

**Ready to practice?** Check out the exercises in `exercises/` directory!
