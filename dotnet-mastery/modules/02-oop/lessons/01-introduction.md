# Module 02: Object-Oriented Programming - Introduction

## 📘 Welcome to Object-Oriented Programming

Object-Oriented Programming (OOP) is the foundation of modern software development. Master these principles to build scalable, maintainable, and testable applications.

## 🎯 Module Objectives

- Master the four pillars of OOP: Encapsulation, Inheritance, Polymorphism, Abstraction
- Design classes with proper responsibilities
- Apply SOLID principles to your code
- Understand composition vs inheritance
- Implement interfaces effectively
- Use abstract classes appropriately

## 🗺️ Module Structure

### Lessons
1. **Introduction** (this document) - OOP overview and benefits
2. **Classes and Objects** - Instance members, constructors, properties
3. **Encapsulation** - Access modifiers, information hiding, properties
4. **Inheritance** - Base classes, derived classes, method overriding
5. **Polymorphism** - Virtual methods, abstract classes, interfaces
6. **Interfaces** - Contract programming, multiple interfaces
7. **SOLID Principles** - Professional design principles
8. **Best Practices** - Composition over inheritance, when to use what

### Time Commitment
- **Estimated Time**: 2-3 weeks
- **Lessons**: 8-12 hours
- **Exercises**: 15-20 hours
- **Projects**: 10-15 hours

## 🎨 Why Object-Oriented Programming?

### The Problem: Procedural Code at Scale

```csharp
// Procedural approach (hard to maintain)
string customerFirstName = "John";
string customerLastName = "Doe";
string customerEmail = "john@example.com";
decimal customerBalance = 1000.00m;

string vendorFirstName = "Jane";
string vendorLastName = "Smith";
string vendorEmail = "jane@vendor.com";
decimal vendorCommission = 0.15m;

// As application grows:
// - 100+ variables for 10 customers
// - Easy to mix up related data
// - Hard to find all code related to "customer"
// - No way to enforce business rules
```

### The Solution: Objects

```csharp
// Object-oriented approach (scalable, maintainable)
public class Customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; private set; }

    // Business logic encapsulated with data
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        Balance += amount;
    }
}

// Usage
var customer1 = new Customer { FirstName = "John", LastName = "Doe" };
var customer2 = new Customer { FirstName = "Jane", LastName = "Smith" };

// Benefits:
// - Data and behavior grouped together
// - Easy to create multiple instances
// - Business rules enforced
// - Clear organization
```

## 🏛️ The Four Pillars of OOP

### 1. Encapsulation

**Definition**: Bundling data and methods that operate on that data within a single unit (class), and controlling access to that data.

```csharp
public class BankAccount
{
    // Private field - hidden from outside
    private decimal _balance;

    // Public property - controlled access
    public decimal Balance
    {
        get => _balance;
        private set  // Can only be set internally
        {
            if (value < 0)
                throw new InvalidOperationException("Balance cannot be negative");
            _balance = value;
        }
    }

    // Public method - controlled modification
    public void Withdraw(decimal amount)
    {
        if (amount > _balance)
            throw new InvalidOperationException("Insufficient funds");

        _balance -= amount;
    }
}

// Usage
var account = new BankAccount();
// account._balance = -1000;  // ❌ Compile error: private field
// account.Balance = -1000;   // ❌ Compile error: private setter
account.Withdraw(100);  // ✅ Controlled through method
```

**Benefits**:
- Data integrity (can't be corrupted from outside)
- Flexibility (can change internal implementation)
- Maintainability (changes contained within class)

### 2. Inheritance

**Definition**: Creating new classes based on existing classes, inheriting their properties and methods.

```csharp
// Base class
public class Vehicle
{
    public string Brand { get; set; }
    public string Model { get; set; }

    public virtual void Start()
    {
        Console.WriteLine($"{Brand} {Model} is starting...");
    }
}

// Derived class - inherits from Vehicle
public class Car : Vehicle
{
    public int Doors { get; set; }

    // Override base method
    public override void Start()
    {
        Console.WriteLine("Checking doors...");
        base.Start();  // Call base implementation
    }
}

// Usage
Car myCar = new Car
{
    Brand = "Toyota",
    Model = "Camry",
    Doors = 4
};

myCar.Start();
// Output:
// Checking doors...
// Toyota Camry is starting...
```

**Benefits**:
- Code reuse (don't repeat common code)
- Hierarchical organization (models real-world relationships)
- Polymorphism enabler (see below)

### 3. Polymorphism

**Definition**: Ability to treat objects of different types uniformly through a common interface or base class.

```csharp
public abstract class Shape
{
    public abstract double Area();
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double Area()
    {
        return Math.PI * Radius * Radius;
    }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double Area()
    {
        return Width * Height;
    }
}

// Polymorphism in action
Shape[] shapes = new Shape[]
{
    new Circle { Radius = 5 },
    new Rectangle { Width = 10, Height = 20 },
    new Circle { Radius = 3 }
};

// Same code works with different types!
foreach (Shape shape in shapes)
{
    Console.WriteLine($"Area: {shape.Area():F2}");
    // Correct Area() method called based on actual type
}

// Output:
// Area: 78.54 (Circle)
// Area: 200.00 (Rectangle)
// Area: 28.27 (Circle)
```

**Benefits**:
- Flexibility (add new types without changing existing code)
- Extensibility (plug in new implementations)
- Clean code (no type-checking if statements)

### 4. Abstraction

**Definition**: Hiding complex implementation details and showing only necessary features.

```csharp
// Abstract interface - what user sees
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

// Implementation 1 - complex details hidden
public class GmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        // Complex SMTP configuration
        // Authentication
        // TLS/SSL setup
        // Actual sending logic
        // Error handling
        // ... 100+ lines of code ...
    }
}

// Implementation 2 - different complex details hidden
public class SendGridService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        // HTTP API calls
        // API key management
        // Rate limiting
        // Retry logic
        // ... 150+ lines of code ...
    }
}

// User code - simple!
public class OrderService
{
    private readonly IEmailService _emailService;

    public OrderService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void ProcessOrder(Order order)
    {
        // Process order...

        // Send confirmation - don't care about the details!
        _emailService.SendEmail(
            order.CustomerEmail,
            "Order Confirmation",
            $"Your order #{order.Id} has been processed");
    }
}
```

**Benefits**:
- Simplicity (use without understanding internals)
- Maintenance (change implementation without affecting users)
- Testing (mock complex dependencies)

## 🎯 Real-World Example: E-commerce System

Let's see how OOP principles work together in a realistic scenario:

```csharp
// Abstraction: Define contracts
public interface IPaymentProcessor
{
    PaymentResult Process(Payment payment);
}

// Encapsulation: Data + behavior in one place
public class CreditCardPayment : IPaymentProcessor
{
    // Private fields - implementation detail
    private readonly string _merchantId;
    private readonly ILogger _logger;

    // Constructor injection (dependency)
    public CreditCardPayment(string merchantId, ILogger logger)
    {
        _merchantId = merchantId;
        _logger = logger;
    }

    // Public interface - what clients can use
    public PaymentResult Process(Payment payment)
    {
        try
        {
            // Encapsulated validation
            ValidatePayment(payment);

            // Complex processing logic hidden from caller
            var result = ProcessWithGateway(payment);

            _logger.Log($"Payment processed: {payment.Amount:C}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.Log($"Payment failed: {ex.Message}");
            return PaymentResult.Failed(ex.Message);
        }
    }

    // Private helper methods - implementation detail
    private void ValidatePayment(Payment payment)
    {
        // Validation logic...
    }

    private PaymentResult ProcessWithGateway(Payment payment)
    {
        // Gateway communication...
        return PaymentResult.Success();
    }
}

// Inheritance + Polymorphism: Base order class
public abstract class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal Total { get; set; }

    // Template method pattern - subclasses customize
    public void Process()
    {
        ValidateOrder();
        CalculateTotal();
        ApplyDiscounts();  // Virtual - can be overridden
        ProcessPayment();
        SendConfirmation();
    }

    protected virtual void ApplyDiscounts()
    {
        // Default: no discounts
    }

    private void ValidateOrder() { /* ... */ }
    private void CalculateTotal() { /* ... */ }
    private void ProcessPayment() { /* ... */ }
    private void SendConfirmation() { /* ... */ }
}

// Polymorphism: Different order types
public class RetailOrder : Order
{
    protected override void ApplyDiscounts()
    {
        // Retail-specific discounts
        if (Total > 100)
        {
            Total *= 0.9m;  // 10% off orders > $100
        }
    }
}

public class WholesaleOrder : Order
{
    protected override void ApplyDiscounts()
    {
        // Wholesale-specific discounts
        Total *= 0.7m;  // 30% off all wholesale orders
    }
}

// Usage - polymorphism in action
public class OrderProcessor
{
    public void ProcessOrders(List<Order> orders)
    {
        foreach (Order order in orders)
        {
            // Works with any Order type!
            // Correct discount logic applied based on actual type
            order.Process();
        }
    }
}
```

## 📊 OOP Benefits

| Benefit | Description | Example |
|---------|-------------|---------|
| **Modularity** | Code organized into discrete units | Customer class contains all customer logic |
| **Reusability** | Write once, use many times | Base Vehicle class reused by Car, Truck, Motorcycle |
| **Maintainability** | Changes isolated to specific classes | Change email logic only in EmailService |
| **Scalability** | Add features without breaking existing code | Add new payment type by implementing interface |
| **Testability** | Mock dependencies for unit testing | Mock IPaymentProcessor to test OrderService |

## 🚀 OOP in the .NET Ecosystem

### Everything is an Object

```csharp
// Even primitives have methods!
int number = 42;
string text = number.ToString();       // int.ToString()
bool isEven = (number % 2 == 0);       // Uses int operators

// Strings are objects
string message = "Hello";
string upper = message.ToUpper();      // String.ToUpper()
int length = message.Length;           // String.Length property

// Arrays are objects
int[] numbers = { 1, 2, 3 };
int count = numbers.Length;            // Array.Length property
Array.Sort(numbers);                   // Array.Sort() method
```

### Built-in Classes

.NET provides thousands of classes:

```csharp
// Collections
var list = new List<int>();
var dict = new Dictionary<string, int>();

// Date/Time
var now = DateTime.Now;
var future = now.AddDays(7);

// File I/O
var text = File.ReadAllText("data.txt");
File.WriteAllText("output.txt", text);

// HTTP
using var client = new HttpClient();
var response = await client.GetAsync("https://api.example.com");

// All using OOP principles!
```

## ✅ Prerequisites Check

Before proceeding, ensure you have completed:

- [ ] Module 01: C# Fundamentals
- [ ] Understand types, variables, methods
- [ ] Comfortable with control flow and error handling
- [ ] Completed Module 01 exercises

## ⏭️ Next Steps

Ready to dive into OOP? Proceed to:
- **[Lesson 02: Classes and Objects](02-classes-and-objects.md)**

You'll learn about:
- Defining classes
- Creating instances (objects)
- Constructors and initialization
- Fields vs properties
- Instance vs static members

## 📚 Additional Resources

- [Object-Oriented Programming (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/)
- [Classes (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes)
- [Inheritance (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/inheritance)

---

*"Object-oriented programming is not just a technology; it's a way of thinking." - Grady Booch*
