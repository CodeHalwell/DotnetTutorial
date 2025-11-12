# Lesson 01: Testing Fundamentals

## 🎯 Learning Objectives

By the end of this lesson, you will understand:

- **Testing Pyramid** - Unit, Integration, E2E tests
- **xUnit** - Modern .NET testing framework
- **Mocking** with Moq - Isolating dependencies
- **Test-Driven Development (TDD)** - Red-Green-Refactor
- **Integration Testing** - Testing with real dependencies
- **Code Coverage** - Measuring test effectiveness
- **Testing Best Practices** - AAA pattern, naming, assertions

## 📚 Table of Contents

1. [Why Test?](#why-test)
2. [Testing Pyramid](#testing-pyramid)
3. [Unit Testing with xUnit](#unit-testing-with-xunit)
4. [Mocking with Moq](#mocking-with-moq)
5. [Integration Testing](#integration-testing)
6. [Test-Driven Development](#test-driven-development)
7. [Code Coverage](#code-coverage)
8. [Best Practices](#best-practices)

---

## Why Test?

**Automated testing** ensures code quality and catches bugs early.

### The Cost of Bugs

```
Finding bugs in:
- Unit tests:        $1    (1 minute to fix)
- Integration tests: $10   (10 minutes to fix)
- QA testing:        $100  (1 hour to fix)
- Production:        $10,000+ (customer impact, rollback, hotfix)

⬆ Finding bugs earlier = exponentially cheaper
```

### Benefits of Testing

✅ **Confidence** - Refactor without fear
✅ **Documentation** - Tests show how code works
✅ **Design** - Testable code = better design
✅ **Regression** - Catch bugs when changing code
✅ **Speed** - Fast feedback loop

---

## Testing Pyramid

```
        /\
       /E2E\        Few (slow, expensive, brittle)
      /------\
     /  Int   \     Some (medium speed/cost)
    /----------\
   /    Unit    \   Many (fast, cheap, reliable)
  /--------------\
```

### Test Types

| Type | What | Speed | Cost | Quantity |
|------|------|-------|------|----------|
| **Unit** | Single class/method | ⚡ Very Fast | 💰 Cheap | 🔢 Many (70-80%) |
| **Integration** | Multiple components | ⏱️ Medium | 💰💰 Medium | 🔢 Some (15-25%) |
| **E2E** | Full system | 🐌 Slow | 💰💰💰 Expensive | 🔢 Few (5-10%) |

---

## Unit Testing with xUnit

**Unit tests** test a single unit of code in isolation.

### Setup

```bash
# Create test project
dotnet new xunit -n MyProject.Tests
cd MyProject.Tests

# Add reference to project under test
dotnet add reference ../MyProject/MyProject.csproj

# Add Moq for mocking
dotnet add package Moq

# Run tests
dotnet test
```

### Basic Test

```csharp
// Calculator.cs (code under test)
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;
    public double Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Cannot divide by zero");
        return (double)a / b;
    }
}

// CalculatorTests.cs
public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        // Arrange
        int a = 10;
        int b = 3;

        // Act
        int result = _calculator.Subtract(a, b);

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        // Arrange
        int a = 10;
        int b = 0;

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _calculator.Divide(a, b));
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(20, 4, 5)]
    [InlineData(15, 3, 5)]
    public void Divide_ValidInputs_ReturnsQuotient(int a, int b, double expected)
    {
        // Act
        double result = _calculator.Divide(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

### xUnit Attributes

```csharp
// [Fact] - Single test
[Fact]
public void MyTest() { }

// [Theory] - Parameterized test
[Theory]
[InlineData(1, 2, 3)]
[InlineData(5, 5, 10)]
public void Add_Theory(int a, int b, int expected)
{
    Assert.Equal(expected, _calculator.Add(a, b));
}

// [Theory] with MemberData
public static IEnumerable<object[]> TestData =>
    new List<object[]>
    {
        new object[] { 1, 2, 3 },
        new object[] { 5, 5, 10 }
    };

[Theory]
[MemberData(nameof(TestData))]
public void Add_MemberData(int a, int b, int expected)
{
    Assert.Equal(expected, _calculator.Add(a, b));
}

// [Trait] - Categorize tests
[Fact]
[Trait("Category", "Unit")]
public void MyUnitTest() { }

// Run specific category:
// dotnet test --filter "Category=Unit"

// [Skip] - Temporarily skip test
[Fact(Skip = "Not implemented yet")]
public void FutureTest() { }
```

### Common Assertions

```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Boolean
Assert.True(condition);
Assert.False(condition);

// Null checks
Assert.Null(obj);
Assert.NotNull(obj);

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);
Assert.Single(collection); // Exactly one item
Assert.Equal(3, collection.Count);

// Strings
Assert.StartsWith("Hello", actual);
Assert.EndsWith("World", actual);
Assert.Contains("middle", actual);
Assert.Matches(@"\d{3}", actual); // Regex

// Exceptions
Assert.Throws<InvalidOperationException>(() => obj.Method());

var ex = Assert.Throws<ArgumentException>(() => obj.Method());
Assert.Equal("param", ex.ParamName);

// Ranges
Assert.InRange(actual, low, high);

// Type checks
Assert.IsType<Customer>(obj);
Assert.IsAssignableFrom<ICustomer>(obj);
```

---

## Mocking with Moq

**Mocking** creates fake objects to isolate the code under test.

### Installing Moq

```bash
dotnet add package Moq
```

### Basic Mocking

```csharp
// Interfaces
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    bool IsValidEmail(string email);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User> AddAsync(User user);
}

// Service under test
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task<User> CreateUserAsync(string name, string email)
    {
        if (!_emailService.IsValidEmail(email))
            throw new ArgumentException("Invalid email");

        var user = new User { Name = name, Email = email };
        await _repository.AddAsync(user);
        await _emailService.SendEmailAsync(email, "Welcome", "Welcome to our service!");

        return user;
    }
}

// Tests with mocks
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _userService = new UserService(_repositoryMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task CreateUser_ValidEmail_CreatesUserAndSendsEmail()
    {
        // Arrange
        var email = "john@example.com";

        _emailServiceMock
            .Setup(x => x.IsValidEmail(email))
            .Returns(true);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var user = await _userService.CreateUserAsync("John Doe", email);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("John Doe", user.Name);
        Assert.Equal(email, user.Email);

        // Verify email was validated
        _emailServiceMock.Verify(
            x => x.IsValidEmail(email),
            Times.Once);

        // Verify email was sent
        _emailServiceMock.Verify(
            x => x.SendEmailAsync(email, "Welcome", "Welcome to our service!"),
            Times.Once);

        // Verify user was saved
        _repositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => u.Name == "John Doe" && u.Email == email)),
            Times.Once);
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ThrowsException()
    {
        // Arrange
        var email = "invalid-email";

        _emailServiceMock
            .Setup(x => x.IsValidEmail(email))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _userService.CreateUserAsync("John Doe", email));

        // Verify repository was never called
        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<User>()),
            Times.Never);
    }
}
```

### Moq Setup Methods

```csharp
// Returns value
mock.Setup(x => x.GetValue()).Returns(42);

// Returns async value
mock.Setup(x => x.GetValueAsync()).ReturnsAsync(42);

// Returns based on input
mock.Setup(x => x.Add(It.IsAny<int>(), It.IsAny<int>()))
    .Returns((int a, int b) => a + b);

// Throws exception
mock.Setup(x => x.GetValue()).Throws<InvalidOperationException>();
mock.Setup(x => x.GetValue()).Throws(new InvalidOperationException("Error"));

// Callback
mock.Setup(x => x.Save(It.IsAny<string>()))
    .Callback<string>(s => Console.WriteLine($"Saving: {s}"));

// Sequential returns
mock.SetupSequence(x => x.GetValue())
    .Returns(1)
    .Returns(2)
    .Returns(3);

// Property setup
mock.SetupProperty(x => x.Name);
mock.Object.Name = "Test"; // Can set/get

// Setup all properties
mock.SetupAllProperties();
```

### Moq Verification

```csharp
// Verify method was called
mock.Verify(x => x.Save(It.IsAny<string>()), Times.Once);

// Verify with specific argument
mock.Verify(x => x.Save("test.txt"), Times.Exactly(2));

// Verify never called
mock.Verify(x => x.Delete(It.IsAny<int>()), Times.Never);

// Verify call order
mock.Verify(x => x.First(), Times.Once);
mock.Verify(x => x.Second(), Times.Once);

// It matchers
mock.Verify(x => x.Process(It.IsAny<int>())); // Any value
mock.Verify(x => x.Process(It.Is<int>(n => n > 0))); // Condition
mock.Verify(x => x.Process(It.IsIn(1, 2, 3))); // In list
mock.Verify(x => x.Process(It.IsInRange(1, 10, Moq.Range.Inclusive)));

// Verify all
mock.VerifyAll(); // All setups were called
mock.VerifyNoOtherCalls(); // No unexpected calls
```

---

## Integration Testing

**Integration tests** test multiple components together with real dependencies.

### WebApplicationFactory

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
```

```csharp
// Test database context
public class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

// Custom WebApplicationFactory
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Add in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Build service provider
            var sp = services.BuildServiceProvider();

            // Seed test data
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        context.Products.AddRange(
            new Product { Id = 1, Name = "Product 1", Price = 10.00m },
            new Product { Id = 2, Name = "Product 2", Price = 20.00m }
        );
        context.SaveChanges();
    }
}

// Integration test
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetProductById_ExistingId_ReturnsProduct()
    {
        // Act
        var response = await _client.GetAsync("/api/products/1");

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Product 1", product.Name);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new CreateProductDto
        {
            Name = "New Product",
            Price = 30.00m
        };

        var content = new StringContent(
            JsonSerializer.Serialize(newProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var location = response.Headers.Location;
        Assert.NotNull(location);

        // Verify product was created
        var getResponse = await _client.GetAsync(location);
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateProduct_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new CreateProductDto
        {
            Name = "", // Invalid: empty name
            Price = -10.00m // Invalid: negative price
        };

        var content = new StringContent(
            JsonSerializer.Serialize(invalidProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_NonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

### Testing with Authentication

```csharp
public class AuthenticatedProductsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private string _token = string.Empty;

    public AuthenticatedProductsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new
        {
            Username = "admin",
            Password = "Admin123!"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

        _token = loginResponse!.Token;
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);
    }

    [Fact]
    public async Task DeleteProduct_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.DeleteAsync("/api/products/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Unauthorized_ReturnsUnauthorized()
    {
        // Don't authenticate

        // Act
        var response = await _client.DeleteAsync("/api/products/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
```

---

## Test-Driven Development

**TDD** follows the **Red-Green-Refactor** cycle.

### The TDD Cycle

```
1. 🔴 RED: Write failing test
   ↓
2. 🟢 GREEN: Write minimal code to pass
   ↓
3. 🔵 REFACTOR: Improve code quality
   ↓
   Repeat
```

### TDD Example

```csharp
// Step 1: RED - Write failing test
[Fact]
public void IsPrime_InputIs2_ReturnsTrue()
{
    // Arrange
    var calculator = new PrimeCalculator();

    // Act
    var result = calculator.IsPrime(2);

    // Assert
    Assert.True(result);
}

// Step 2: GREEN - Minimal code to pass
public class PrimeCalculator
{
    public bool IsPrime(int number)
    {
        return number == 2; // Hardcoded to pass test
    }
}

// Step 3: Add more tests (RED again)
[Theory]
[InlineData(2, true)]
[InlineData(3, true)]
[InlineData(4, false)]
[InlineData(5, true)]
[InlineData(6, false)]
[InlineData(7, true)]
[InlineData(8, false)]
[InlineData(9, false)]
[InlineData(10, false)]
[InlineData(11, true)]
public void IsPrime_VariousNumbers_ReturnsCorrectResult(int number, bool expected)
{
    // Arrange
    var calculator = new PrimeCalculator();

    // Act
    var result = calculator.IsPrime(number);

    // Assert
    Assert.Equal(expected, result);
}

// Step 4: GREEN - Implement real logic
public class PrimeCalculator
{
    public bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }

        return true;
    }
}

// Step 5: REFACTOR - Improve code
public class PrimeCalculator
{
    public bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (IsEven(number)) return false;

        return !HasOddDivisor(number);
    }

    private static bool IsEven(int number) => number % 2 == 0;

    private static bool HasOddDivisor(int number)
    {
        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return true;
        }
        return false;
    }
}
```

---

## Code Coverage

**Code coverage** measures how much of your code is tested.

### Install Coverage Tool

```bash
dotnet add package coverlet.collector
```

### Run Tests with Coverage

```bash
# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Generate HTML report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report

# Open coverage-report/index.html in browser
```

### Coverage Metrics

```
Line Coverage:     85%  ✅ Good (aim for 70-90%)
Branch Coverage:   80%  ✅ Good
Method Coverage:   90%  ✅ Excellent
```

### Example Coverage Report

```
┌─────────────────────────────────────────────┐
│ Calculator.cs                        100%   │
├─────────────────────────────────────────────┤
│   Add(int a, int b)                  100%   │
│   Subtract(int a, int b)             100%   │
│   Multiply(int a, int b)             100%   │
│   Divide(int a, int b)               100%   │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│ UserService.cs                       75%    │
├─────────────────────────────────────────────┤
│   CreateUser(...)                    100%   │
│   UpdateUser(...)                    80%    │ ⚠️
│   DeleteUser(...)                    0%     │ ❌
└─────────────────────────────────────────────┘
```

---

## Best Practices

### 1. AAA Pattern (Arrange-Act-Assert)

```csharp
// ✅ GOOD: Clear AAA structure
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    // Arrange - Setup test data
    var calculator = new Calculator();
    int a = 5;
    int b = 3;

    // Act - Execute the code being tested
    int result = calculator.Add(a, b);

    // Assert - Verify the result
    Assert.Equal(8, result);
}

// ❌ BAD: No clear structure
[Fact]
public void Test1()
{
    var calc = new Calculator();
    Assert.Equal(8, calc.Add(5, 3));
}
```

### 2. Descriptive Test Names

```csharp
// ✅ GOOD: Descriptive names
[Fact]
public void CreateUser_WithInvalidEmail_ThrowsArgumentException()

[Fact]
public void GetProductById_WhenProductNotFound_ReturnsNull()

[Fact]
public void ProcessPayment_WithInsufficientFunds_ReturnsFalse()

// ❌ BAD: Unclear names
[Fact]
public void Test1()

[Fact]
public void UserTest()

[Fact]
public void TestMethod()
```

### 3. One Assert Per Test (When Possible)

```csharp
// ✅ GOOD: Single logical assertion
[Fact]
public void CreateUser_ValidInput_ReturnsUser()
{
    var user = _service.CreateUser("John", "john@example.com");
    Assert.NotNull(user);
}

[Fact]
public void CreateUser_ValidInput_SetsCorrectName()
{
    var user = _service.CreateUser("John", "john@example.com");
    Assert.Equal("John", user.Name);
}

// ⚠️ ACCEPTABLE: Related assertions for one concept
[Fact]
public void CreateUser_ValidInput_SetsAllProperties()
{
    var user = _service.CreateUser("John", "john@example.com");

    Assert.NotNull(user);
    Assert.Equal("John", user.Name);
    Assert.Equal("john@example.com", user.Email);
    Assert.True(user.IsActive);
}

// ❌ BAD: Too many unrelated assertions
[Fact]
public void MegaTest()
{
    var user = _service.CreateUser(...);
    Assert.NotNull(user);

    var product = _service.GetProduct(...);
    Assert.NotNull(product);

    var order = _service.CreateOrder(...);
    Assert.NotNull(order);
}
```

### 4. Test Isolation

```csharp
// ✅ GOOD: Tests are independent
public class UserServiceTests
{
    [Fact]
    public void Test1()
    {
        var service = new UserService();
        // Test logic
    }

    [Fact]
    public void Test2()
    {
        var service = new UserService(); // Fresh instance
        // Test logic
    }
}

// ❌ BAD: Tests share state
public class UserServiceTests
{
    private readonly UserService _service = new UserService();
    private User _testUser; // Shared state!

    [Fact]
    public void Test1()
    {
        _testUser = _service.CreateUser("John");
        // _testUser is now set for other tests
    }

    [Fact]
    public void Test2()
    {
        // Depends on Test1 running first!
        _service.UpdateUser(_testUser);
    }
}
```

### 5. Don't Test Framework Code

```csharp
// ❌ BAD: Testing Entity Framework
[Fact]
public void DbContext_SaveChanges_SavesData()
{
    var context = new ApplicationDbContext();
    context.Users.Add(new User());
    context.SaveChanges(); // Testing EF Core, not your code
}

// ✅ GOOD: Test your business logic
[Fact]
public void UserService_CreateUser_SavesUserToRepository()
{
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);

    service.CreateUser("John");

    mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
}
```

### 6. Fast Tests

```csharp
// ✅ GOOD: Fast test
[Fact]
public void Calculator_Add_ReturnsSum()
{
    Assert.Equal(8, new Calculator().Add(5, 3));
}
// Runs in < 1ms

// ❌ BAD: Slow test
[Fact]
public async Task SlowTest()
{
    await Task.Delay(5000); // 5 seconds!
    // ...
}
```

### 7. Use Test Fixtures for Expensive Setup

```csharp
// ✅ GOOD: Shared fixture for expensive setup
public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; }

    public DatabaseFixture()
    {
        // Expensive: create database once
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

public class UserServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly ApplicationDbContext _context;

    public UserServiceTests(DatabaseFixture fixture)
    {
        _context = fixture.Context; // Reuse database
    }

    [Fact]
    public void Test1() { /* Use _context */ }

    [Fact]
    public void Test2() { /* Use _context */ }
}
```

---

## Summary

**Key Takeaways:**

1. **Testing Pyramid**: Most unit tests, some integration, few E2E
2. **xUnit**: Modern .NET testing framework with [Fact] and [Theory]
3. **Moq**: Mock dependencies for isolated unit tests
4. **Integration Tests**: Test with WebApplicationFactory and in-memory DB
5. **TDD**: Red-Green-Refactor cycle
6. **Code Coverage**: Aim for 70-90%, 100% is overkill
7. **AAA Pattern**: Arrange, Act, Assert structure
8. **Test Isolation**: Each test is independent

**Best Practices:**
- ✅ Descriptive test names
- ✅ One logical assertion per test
- ✅ Fast, isolated tests
- ✅ Mock external dependencies
- ✅ Test behavior, not implementation
- ✅ Don't test framework code
- ✅ Maintain tests like production code

---

**Ready to test?** Start with unit tests for critical business logic!
