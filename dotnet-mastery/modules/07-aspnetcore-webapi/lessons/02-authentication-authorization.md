# Lesson 02: Authentication & Authorization

## 🎯 Learning Objectives

By the end of this lesson, you will understand:

- **Authentication vs Authorization** - Critical difference
- **JWT (JSON Web Tokens)** - Industry standard for APIs
- **Identity Framework** - ASP.NET Core Identity system
- **OAuth 2.0 & OpenID Connect** - Third-party authentication
- **Role-Based Authorization** - Organizing permissions
- **Policy-Based Authorization** - Fine-grained access control
- **API Key Authentication** - Simple authentication for APIs
- **Security Best Practices** - Protecting your API

## 📚 Table of Contents

1. [Authentication vs Authorization](#authentication-vs-authorization)
2. [JWT Authentication](#jwt-authentication)
3. [ASP.NET Core Identity](#aspnet-core-identity)
4. [OAuth 2.0 & OpenID Connect](#oauth-20--openid-connect)
5. [Role-Based Authorization](#role-based-authorization)
6. [Policy-Based Authorization](#policy-based-authorization)
7. [API Keys](#api-keys)
8. [Security Best Practices](#security-best-practices)

---

## Authentication vs Authorization

### The Critical Difference

**Authentication (AuthN)**: *Who are you?*
- Verifying identity
- User provides credentials (username/password, token, etc.)
- Answer: "This is John Doe"

**Authorization (AuthZ)**: *What can you do?*
- Verifying permissions
- Checking if user has access to resource
- Answer: "John Doe can edit posts but not delete users"

### Real-World Analogy

```
🏢 Office Building Security

Authentication (Front Desk):
- Show your employee badge
- Security guard verifies: "Yes, you work here"
- You're authenticated ✅

Authorization (Department Doors):
- Try to enter Finance department
- Card reader checks: "Do you have Finance access?"
- Authorization determines if you can enter
```

### Code Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    // No authentication or authorization - anyone can access
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok("All posts");
    }

    // Authentication required - must be logged in
    [HttpPost]
    [Authorize]  // Must be authenticated
    public IActionResult Create(Post post)
    {
        return Ok("Post created");
    }

    // Authorization required - must be admin
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]  // Must be authenticated AND be an Admin
    public IActionResult Delete(int id)
    {
        return Ok("Post deleted");
    }
}
```

---

## JWT Authentication

**JWT (JSON Web Token)** is the industry standard for API authentication.

### Why JWT?

✅ **Stateless**: Server doesn't need to store sessions
✅ **Scalable**: Works across multiple servers
✅ **Cross-domain**: Works with CORS
✅ **Mobile-friendly**: Perfect for mobile apps
✅ **Self-contained**: Contains all user info

### JWT Structure

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

├─ Header (algorithm & token type)
│  eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
│  Decoded: {"alg":"HS256","typ":"JWT"}
│
├─ Payload (claims - user data)
│  eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ
│  Decoded: {"sub":"1234567890","name":"John Doe","iat":1516239022}
│
└─ Signature (verification)
   SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
   HMACSHA256(base64(header) + "." + base64(payload), secret)
```

### Implementing JWT Authentication

#### Step 1: Install Package

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

#### Step 2: Configure JWT in Program.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Add authentication middleware (BEFORE authorization!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
```

#### Step 3: Add JWT Settings to appsettings.json

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "ExpiryMinutes": 60
  }
}
```

⚠️ **Security Warning:** Never commit secrets to source control! Use:
- **Development**: User Secrets (`dotnet user-secrets set "Jwt:Key" "your-key"`)
- **Production**: Environment variables or Azure Key Vault

#### Step 4: Create JWT Service

```csharp
public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims - user information embedded in token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero // No tolerance for expiry
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

// Register service
builder.Services.AddSingleton<IJwtService, JwtService>();
```

#### Step 5: Create Authentication Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// User login - returns JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate credentials
        var user = await _userService.ValidateCredentialsAsync(request.Username, request.Password);
        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]) * 60,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role
            }
        });
    }

    /// <summary>
    /// User registration
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Check if username already exists
        if (await _userService.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new { message = "Username already taken" });
        }

        // Check if email already exists
        if (await _userService.EmailExistsAsync(request.Email))
        {
            return BadRequest(new { message = "Email already registered" });
        }

        // Create user (password will be hashed by service)
        var user = await _userService.CreateUserAsync(new User
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = "User" // Default role
        }, request.Password);

        // Generate token
        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("New user registered: {Username}", user.Username);

        return CreatedAtAction(nameof(GetCurrentUser), new LoginResponse
        {
            Token = token,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]) * 60,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role
            }
        });
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        // User.Identity is populated from JWT claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var user = await _userService.GetByIdAsync(int.Parse(userId));
        if (user == null)
            return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role
        });
    }

    /// <summary>
    /// Refresh token (extend session)
    /// </summary>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetByIdAsync(int.Parse(userId!));

        if (user == null)
            return Unauthorized();

        var newToken = _jwtService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Token = newToken,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]) * 60
        });
    }
}

// DTOs
public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // Seconds
    public UserDto? User { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

#### Step 6: Use in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // All actions require authentication
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    // Anyone authenticated can view
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postService.GetAllAsync();
        return Ok(posts);
    }

    // Anyone authenticated can create
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        // Get current user ID from JWT claims
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var post = await _postService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    // Only the post owner or admin can update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        var post = await _postService.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        // Check ownership or admin role
        if (post.UserId != userId && userRole != "Admin")
            return Forbid(); // 403 Forbidden

        await _postService.UpdateAsync(id, request);
        return NoContent();
    }

    // Only admins can delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _postService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Override [Authorize] on controller
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _postService.GetByIdAsync(id);
        return post != null ? Ok(post) : NotFound();
    }
}
```

#### Step 7: Testing with Postman/curl

```bash
# 1. Register new user
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
  }'

# Response:
# {
#   "token": "eyJhbGci...",
#   "expiresIn": 3600,
#   "user": {
#     "id": 1,
#     "username": "john_doe",
#     "email": "john@example.com",
#     "fullName": "John Doe",
#     "role": "User"
#   }
# }

# 2. Login
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "password": "SecurePassword123!"
  }'

# 3. Access protected endpoint
curl https://localhost:7001/api/posts \
  -H "Authorization: Bearer eyJhbGci..."

# 4. Access admin-only endpoint (will fail with 403)
curl -X DELETE https://localhost:7001/api/posts/1 \
  -H "Authorization: Bearer eyJhbGci..."
```

---

## ASP.NET Core Identity

**ASP.NET Core Identity** is a comprehensive membership system with built-in user management.

### Features

- User registration/login
- Password hashing (bcrypt)
- Email confirmation
- Two-factor authentication (2FA)
- External login providers (Google, Facebook)
- Account lockout
- Password recovery
- Role management
- Claims-based authorization

### Setting Up Identity

```bash
# Install packages
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

```csharp
// User entity
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// DbContext
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize Identity tables if needed
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
        });

        // Seed roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
        );
    }
}

// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Migrations
// dotnet ef migrations add AddIdentity
// dotnet ef database update
```

### Using Identity in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        // Add to role
        await _userManager.AddToRoleAsync(user, "User");

        // Generate email confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // Send confirmation email...

        return Ok(new { message = "Registration successful. Please confirm your email." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new { message = "Email not confirmed" });

            var jwtToken = _jwtService.GenerateToken(user);
            return Ok(new { token = jwtToken });
        }

        if (result.IsLockedOut)
            return Unauthorized(new { message = "Account locked due to multiple failed attempts" });

        return Unauthorized(new { message = "Invalid credentials" });
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Ok(new { message = "Email confirmed successfully" });

        return BadRequest(new { errors = result.Errors });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Ok(new { message = "If email exists, reset link has been sent" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Send reset email with token...

        return Ok(new { message = "If email exists, reset link has been sent" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request" });

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (result.Succeeded)
            return Ok(new { message = "Password reset successful" });

        return BadRequest(new { errors = result.Errors });
    }
}
```

---

## OAuth 2.0 & OpenID Connect

**OAuth 2.0**: Authorization framework (what you can access)
**OpenID Connect**: Authentication layer on top of OAuth 2.0 (who you are)

### Adding Google Authentication

```bash
dotnet add package Microsoft.AspNetCore.Authentication.Google
```

```csharp
// Program.cs
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    });
```

```csharp
// Controller for external login
[HttpGet("external-login")]
public IActionResult ExternalLogin(string provider, string returnUrl = "/")
{
    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl });
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    return Challenge(properties, provider);
}

[HttpGet("external-login-callback")]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
{
    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
        return Redirect("/login?error=external");

    // Sign in with external provider
    var result = await _signInManager.ExternalLoginSignInAsync(
        info.LoginProvider, info.ProviderKey, isPersistent: false);

    if (result.Succeeded)
    {
        // Generate JWT and return
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        var token = _jwtService.GenerateToken(user!);
        return Ok(new { token });
    }

    // User doesn't exist - create account
    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };

    var createResult = await _userManager.CreateAsync(user);
    if (createResult.Succeeded)
    {
        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token });
    }

    return BadRequest(new { errors = createResult.Errors });
}
```

---

## Role-Based Authorization

**Roles**: Group users by function (Admin, Moderator, User)

### Creating Roles

```csharp
// Seed roles in DbContext
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.Entity<IdentityRole>().HasData(
        new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
        new IdentityRole { Id = "2", Name = "Moderator", NormalizedName = "MODERATOR" },
        new IdentityRole { Id = "3", Name = "User", NormalizedName = "USER" }
    );
}

// Or create programmatically
public class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Moderator", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}

// In Program.cs (after app.Build())
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}
```

### Using Roles

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Must be authenticated
public class UsersController : ControllerBase
{
    // Only admins can access
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllUsers()
    {
        return Ok("All users");
    }

    // Admins or moderators can access
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult GetUser(int id)
    {
        return Ok($"User {id}");
    }

    // Anyone authenticated can access (inherits from controller)
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok("Current user");
    }

    // Check role programmatically
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (User.IsInRole("Admin"))
        {
            // Admin can create any user
        }
        else if (User.IsInRole("Moderator"))
        {
            // Moderator has limited permissions
            if (request.Role == "Admin")
                return Forbid(); // Can't create admins
        }
        else
        {
            return Forbid(); // Regular users can't create users
        }

        return Ok();
    }
}
```

---

## Policy-Based Authorization

**Policies**: Fine-grained, flexible authorization beyond simple roles.

### Defining Policies

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    // Simple role-based policy
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Multiple roles
    options.AddPolicy("AdminOrModerator", policy =>
        policy.RequireRole("Admin", "Moderator"));

    // Claim-based policy
    options.AddPolicy("MustBeOver18", policy =>
        policy.RequireClaim("DateOfBirth", dob =>
        {
            var age = DateTime.Today.Year - DateTime.Parse(dob).Year;
            return age >= 18;
        }));

    // Custom requirement
    options.AddPolicy("MustOwnResource", policy =>
        policy.Requirements.Add(new ResourceOwnerRequirement()));

    // Combined requirements
    options.AddPolicy("SeniorAdmin", policy =>
    {
        policy.RequireRole("Admin");
        policy.RequireClaim("Seniority", "Senior");
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "YearsOfService" &&
                                      int.Parse(c.Value) >= 5));
    });
});
```

### Custom Authorization Requirements

```csharp
// Requirement
public class ResourceOwnerRequirement : IAuthorizationRequirement
{
}

// Handler
public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        Post resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null && resource.UserId == int.Parse(userId))
        {
            context.Succeed(requirement);
        }
        else if (context.User.IsInRole("Admin"))
        {
            // Admins can access any resource
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

// Register handler
builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerHandler>();
```

### Using Policies

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPostService _postService;

    public PostsController(
        IAuthorizationService authorizationService,
        IPostService postService)
    {
        _authorizationService = authorizationService;
        _postService = postService;
    }

    // Declarative authorization
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAdminPosts()
    {
        return Ok("Admin posts");
    }

    // Imperative authorization (runtime check)
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest request)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        // Check if user can edit this post
        var authResult = await _authorizationService.AuthorizeAsync(
            User, post, "MustOwnResource");

        if (!authResult.Succeeded)
        {
            return Forbid(); // 403 Forbidden
        }

        await _postService.UpdateAsync(id, request);
        return NoContent();
    }
}
```

---

## API Keys

**API Keys**: Simple authentication for service-to-service communication.

### Implementation

```csharp
// Middleware
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for certain paths
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }

        // Check for API key in header
        if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        // Validate API key
        var apiKey = _configuration["ApiKey"];
        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        await _next(context);
    }
}

// Register middleware
app.UseMiddleware<ApiKeyMiddleware>();

// Usage
curl https://localhost:7001/api/data \
  -H "X-API-Key: your-secret-api-key"
```

### Database-Backed API Keys

```csharp
public class ApiKey
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _context;

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        var key = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Key == apiKey && k.IsActive);

        if (key == null)
            return false;

        if (key.ExpiresAt.HasValue && key.ExpiresAt.Value < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<ApiKey> GenerateApiKeyAsync(string name, string owner)
    {
        var apiKey = new ApiKey
        {
            Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            Name = name,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        return apiKey;
    }
}
```

---

## Security Best Practices

### 1. Use HTTPS

```csharp
// Enforce HTTPS
app.UseHttpsRedirection();

// HSTS (HTTP Strict Transport Security)
app.UseHsts();
```

### 2. Secure Password Storage

```csharp
// ✅ GOOD: Use Identity (automatic hashing)
var result = await _userManager.CreateAsync(user, password);

// ❌ BAD: Never store plain text passwords
user.Password = password; // DON'T DO THIS!

// Manual hashing (if not using Identity)
public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### 3. Protect Against CSRF

```csharp
// Add anti-forgery services
builder.Services.AddAntiforgery();

// Use in forms
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult SubmitForm()
{
    return Ok();
}
```

### 4. Rate Limiting

```bash
dotnet add package AspNetCoreRateLimit
```

```csharp
// appsettings.json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}

// Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

app.UseIpRateLimiting();
```

### 5. Input Validation

```csharp
// Always validate input
public class CreatePostRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;

    [Url]
    public string? ImageUrl { get; set; }
}

// Sanitize HTML input
public class HtmlSanitizer
{
    public static string Sanitize(string html)
    {
        var sanitizer = new HtmlSanitizer();
        return sanitizer.Sanitize(html);
    }
}
```

### 6. Secure Headers

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});
```

### 7. Secrets Management

```bash
# Development: User Secrets
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-key"

# Production: Environment Variables
export JWT_KEY="your-super-secret-key"

# Azure: Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### 8. CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // ❌ DON'T use in production:
    // policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.UseCors("AllowSpecificOrigins");
```

---

## Summary

**Key Takeaways:**

1. **Authentication** = Who you are (login)
2. **Authorization** = What you can do (permissions)
3. **JWT** = Token-based authentication for APIs
4. **Identity** = Full-featured membership system
5. **OAuth/OIDC** = Third-party authentication (Google, Facebook)
6. **Roles** = Group users by function
7. **Policies** = Fine-grained authorization rules
8. **Security** = HTTPS, password hashing, rate limiting, input validation

**Authentication Flow:**
```
1. User sends credentials → POST /api/auth/login
2. Server validates credentials
3. Server generates JWT with claims
4. Server returns JWT to client
5. Client stores JWT (localStorage/sessionStorage)
6. Client sends JWT in Authorization header
7. Server validates JWT on each request
8. Server grants/denies access based on claims
```

**Best Practices:**
- ✅ Use HTTPS always
- ✅ Hash passwords (never store plain text)
- ✅ Use strong JWT secrets
- ✅ Implement rate limiting
- ✅ Validate all input
- ✅ Use role-based or policy-based authorization
- ✅ Implement token refresh
- ✅ Log authentication failures
- ✅ Use secure headers
- ✅ Store secrets in Key Vault/environment variables

---

## What's Next?

In the next lessons, we'll explore:
- **Lesson 03**: Advanced API Features (Filtering, Sorting, Pagination, Versioning)
- **Lesson 04**: API Documentation with Swagger/OpenAPI
- **Lesson 05**: Testing Web APIs (Unit, Integration, E2E)

---

**Ready to practice?** Build a secure API with JWT authentication!
