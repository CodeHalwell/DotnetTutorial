# Project 04: RESTful Blog API

## 🎯 Project Overview

Build a production-grade RESTful API for a blogging platform with ASP.NET Core. This intermediate project introduces web API development, authentication, database integration, and RESTful design principles.

## 🏆 Learning Objectives

- Design and implement RESTful APIs
- Apply CRUD operations with Entity Framework Core
- Implement JWT authentication and authorization
- Validate user input
- Handle errors and return appropriate HTTP status codes
- Write API documentation with Swagger/OpenAPI
- Implement filtering, sorting, and pagination
- Write integration tests for APIs

## 📋 Requirements

### Phase 1: Core API

#### Data Models

**User**
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Post> Posts { get; set; }
    public List<Comment> Comments { get; set; }
}
```

**Post**
```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public List<Comment> Comments { get; set; }
    public List<PostTag> PostTags { get; set; }
}
```

**Comment**
```csharp
public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Tag**
```csharp
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PostTag> PostTags { get; set; }
}
```

#### API Endpoints

**Authentication Endpoints**
```
POST   /api/auth/register       Register new user
POST   /api/auth/login          Login and get JWT token
POST   /api/auth/refresh        Refresh access token
GET    /api/auth/me             Get current user info
```

**Post Endpoints**
```
GET    /api/posts                Get all posts (with pagination)
GET    /api/posts/{id}           Get single post
POST   /api/posts                Create new post (auth required)
PUT    /api/posts/{id}           Update post (auth required, author only)
DELETE /api/posts/{id}           Delete post (auth required, author only)
GET    /api/posts/{id}/comments  Get comments for post
GET    /api/posts/user/{userId}  Get posts by user
GET    /api/posts/tag/{tag}      Get posts by tag
```

**Comment Endpoints**
```
GET    /api/comments/{id}        Get single comment
POST   /api/posts/{id}/comments  Create comment (auth required)
PUT    /api/comments/{id}        Update comment (auth required, author only)
DELETE /api/comments/{id}        Delete comment (auth required, author only)
```

**Tag Endpoints**
```
GET    /api/tags                 Get all tags
GET    /api/tags/{id}            Get single tag
POST   /api/tags                 Create tag (auth required)
```

### Phase 2: Advanced Features

#### Filtering, Sorting, and Pagination

```
GET /api/posts?page=1&pageSize=10&sortBy=createdAt&sortOrder=desc
GET /api/posts?author=john&tag=technology&published=true
GET /api/posts?search=asp.net
```

**Query Parameters:**
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)
- `sortBy` - Field to sort by (createdAt, title, etc.)
- `sortOrder` - asc or desc
- `search` - Search in title and content
- `tag` - Filter by tag
- `author` - Filter by author username
- `published` - Filter by published status

#### Response Format

**Success Response:**
```json
{
  "data": {
    "id": 1,
    "title": "Getting Started with ASP.NET Core",
    "content": "...",
    "author": {
      "id": 1,
      "username": "john_doe"
    },
    "createdAt": "2025-11-12T10:30:00Z",
    "tags": ["aspnet", "tutorial"]
  },
  "success": true
}
```

**Paginated Response:**
```json
{
  "data": {
    "items": [...],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 150,
    "totalPages": 15,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "success": true
}
```

**Error Response:**
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "title",
        "message": "Title is required"
      },
      {
        "field": "content",
        "message": "Content must be at least 100 characters"
      }
    ]
  }
}
```

### Phase 3: Authentication & Security

#### JWT Authentication

```csharp
// Login request
POST /api/auth/login
{
  "username": "john_doe",
  "password": "SecurePassword123!"
}

// Login response
{
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  },
  "success": true
}

// Use token in requests
GET /api/posts/my-posts
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Authorization Rules

- **Public**: Anyone can view published posts and comments
- **Authenticated**: Can create posts and comments
- **Author only**: Can edit/delete own posts and comments
- **Admin** (bonus): Can moderate any content

#### Security Requirements

1. **Password Security**
   - Minimum 8 characters
   - At least one uppercase, lowercase, number, special character
   - Hash with BCrypt or PBKDF2

2. **Input Validation**
   - Validate all user input
   - Use FluentValidation or Data Annotations
   - Sanitize HTML in content

3. **Rate Limiting**
   - Limit API requests per user/IP
   - Prevent brute force attacks

4. **CORS**
   - Configure appropriate CORS policies
   - Only allow specific origins in production

## 🏗️ Architecture

### Project Structure

```
BlogApi/
├── BlogApi.API/                 # Web API project
│   ├── Controllers/
│   ├── Middleware/
│   ├── Filters/
│   └── Program.cs
├── BlogApi.Core/                # Core domain
│   ├── Entities/
│   ├── Interfaces/
│   ├── DTOs/
│   └── Exceptions/
├── BlogApi.Infrastructure/      # Data access
│   ├── Data/
│   │   └── BlogDbContext.cs
│   ├── Repositories/
│   └── Migrations/
├── BlogApi.Application/         # Business logic
│   ├── Services/
│   ├── Mappings/
│   └── Validators/
└── BlogApi.Tests/               # Tests
    ├── Unit/
    └── Integration/
```

### Layered Architecture

```mermaid
graph TB
    A[API Layer Controllers] --> B[Application Layer Services]
    B --> C[Domain Layer Entities]
    B --> D[Infrastructure Layer Repositories]
    D --> C
    D --> E[Database]
```

### Design Patterns

1. **Repository Pattern**: Abstract data access
2. **Unit of Work**: Manage transactions
3. **Service Layer**: Business logic
4. **DTOs**: Data transfer objects
5. **AutoMapper**: Object mapping
6. **Dependency Injection**: Loose coupling

## 🧪 Testing Requirements

### Unit Tests

```csharp
[Fact]
public async Task CreatePost_ValidPost_ReturnsCreatedPost()
{
    // Arrange
    var service = new PostService(_mockRepository.Object);
    var newPost = new CreatePostDto
    {
        Title = "Test Post",
        Content = "This is test content..."
    };

    // Act
    var result = await service.CreatePostAsync(newPost, userId: 1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test Post", result.Title);
    _mockRepository.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Once);
}
```

### Integration Tests

```csharp
public class PostsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PostsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPosts_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/posts");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }
}
```

## 📊 Database Schema

### Entity Relationships

```sql
Users (1) ----< (M) Posts
Posts (1) ----< (M) Comments
Users (1) ----< (M) Comments
Posts (M) >----< (M) Tags (via PostTags)
```

### Sample Migration

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Username = table.Column<string>(maxLength: 50, nullable: false),
                Email = table.Column<string>(maxLength: 255, nullable: false),
                PasswordHash = table.Column<string>(nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false,
                    defaultValueSql: "GETUTCDATE()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.HasIndex(x => x.Username).IsUnique();
                table.HasIndex(x => x.Email).IsUnique();
            });

        // ... more tables
    }
}
```

## ✅ Acceptance Criteria

### Functionality
- ✅ All CRUD operations work correctly
- ✅ Authentication and authorization function properly
- ✅ Pagination, filtering, and sorting work
- ✅ Input validation prevents invalid data
- ✅ Appropriate HTTP status codes returned

### Code Quality
- ✅ Clean architecture with separated concerns
- ✅ Dependency injection used throughout
- ✅ Comprehensive error handling
- ✅ Logging implemented
- ✅ Code follows C# conventions

### Documentation
- ✅ Swagger/OpenAPI documentation complete
- ✅ README with setup instructions
- ✅ API usage examples
- ✅ Database schema documented

### Testing
- ✅ Unit tests: >80% code coverage
- ✅ Integration tests for all endpoints
- ✅ All tests pass

### Performance
- ✅ Response time < 200ms for simple queries
- ✅ Efficient database queries (no N+1)
- ✅ Proper use of async/await

## 🎯 Milestones

### Week 1: Foundation
- [ ] Project setup and architecture
- [ ] Database models and DbContext
- [ ] Basic CRUD endpoints (Posts)
- [ ] Unit tests for services

### Week 2: Features
- [ ] Authentication (JWT)
- [ ] Comments and Tags
- [ ] Filtering, sorting, pagination
- [ ] Input validation

### Week 3: Polish
- [ ] Integration tests
- [ ] Error handling and logging
- [ ] Swagger documentation
- [ ] Deployment preparation

## 💡 Implementation Tips

### Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class PostRepository : IRepository<Post>
{
    private readonly BlogDbContext _context;

    public PostRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<Post> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // ... other methods
}
```

### Pagination Helper

```csharp
public class PagedList<T>
{
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count
        };
    }
}
```

### JWT Configuration

```csharp
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKeyHere-MinLength32Chars",
    "Issuer": "BlogApi",
    "Audience": "BlogApiClients",
    "ExpirationMinutes": 60
  }
}

// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });
```

## 🚀 Bonus Features

1. **Likes/Reactions**: Add like system for posts
2. **Search**: Full-text search with Elasticsearch
3. **Image Upload**: Support image uploads for posts
4. **Email Notifications**: Send emails on new comments
5. **Caching**: Redis caching for frequently accessed data
6. **Versioning**: API versioning (v1, v2)
7. **GraphQL**: Alternative GraphQL endpoint
8. **Real-time**: SignalR for real-time comments
9. **Admin Panel**: Admin endpoints for moderation
10. **Analytics**: Track post views and engagement

## ⏱️ Estimated Time

- **Week 1**: Foundation (15-20 hours)
- **Week 2**: Features (15-20 hours)
- **Week 3**: Polish & Testing (10-15 hours)
- **Total**: 40-55 hours over 3 weeks

## 📚 Resources

### Documentation
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://jwt.io/introduction)

### Tutorials
- Module 07: Web APIs with ASP.NET Core
- Module 08: Entity Framework Core
- Module 10: Testing & Quality

### Tools
- [Swagger/OpenAPI](https://swagger.io/)
- [Postman](https://www.postman.com/)
- [DB Browser for SQLite](https://sqlitebrowser.org/)

## 🎓 Learning Outcomes

After completing this project:
- ✅ Design RESTful APIs following best practices
- ✅ Implement authentication and authorization
- ✅ Work with Entity Framework Core
- ✅ Apply layered architecture
- ✅ Write comprehensive tests
- ✅ Handle errors gracefully
- ✅ Document APIs professionally
- ✅ Deploy to production

---

**Ready to build?** Start with project setup and database design!

**Stuck?** Review Module 07 (Web APIs) and Module 08 (EF Core) lessons.

**Want to compare?** Check the reference implementation after attempting on your own.

Good luck! 🚀
