# Module 01: C# Fundamentals - Methods and Parameters

## 📘 Methods: Building Blocks of Programs

Methods are reusable blocks of code that perform specific tasks. They're fundamental to writing clean, maintainable, and testable code.

## 🎯 Learning Objectives

- Define and invoke methods
- Understand parameter types (value, ref, out, in, params)
- Return single and multiple values
- Implement method overloading
- Use expression-bodied members
- Apply local functions
- Follow method design best practices

## 🔧 Method Basics

### Method Anatomy

```csharp
// [access modifier] [return type] [method name]([parameters])
// {
//     [method body]
//     [return statement]
// }

public int Add(int a, int b)
{
    return a + b;
}

// Breakdown:
// - public: Access modifier (visible from anywhere)
// - int: Return type (method returns an integer)
// - Add: Method name (PascalCase)
// - (int a, int b): Parameters (inputs)
// - return a + b: Return statement (output)
```

### Method Invocation

```csharp
public class Calculator
{
    // Method definition
    public int Add(int a, int b)
    {
        return a + b;
    }

    // Method with void return (no return value)
    public void PrintResult(int result)
    {
        Console.WriteLine($"Result: {result}");
    }

    // Using the methods
    public void Demo()
    {
        int sum = Add(5, 3);      // Call method, store result
        PrintResult(sum);         // Call void method

        // Or inline:
        PrintResult(Add(10, 20)); // Call Add, pass result to PrintResult
    }
}
```

### Access Modifiers

```csharp
public class AccessModifierExamples
{
    // public: Accessible from anywhere
    public void PublicMethod() { }

    // private: Accessible only within this class (default for members)
    private void PrivateMethod() { }

    // protected: Accessible within this class and derived classes
    protected void ProtectedMethod() { }

    // internal: Accessible within the same assembly
    internal void InternalMethod() { }

    // protected internal: Accessible within same assembly or derived classes
    protected internal void ProtectedInternalMethod() { }

    // private protected: Accessible within same assembly AND only in derived classes
    private protected void PrivateProtectedMethod() { }
}

// Common patterns:
// - Public methods: API surface (what others can call)
// - Private methods: Implementation details (helper methods)
// - Protected methods: Extensibility points for inheritance
```

## 📦 Parameters

### Value Parameters (Default)

```csharp
// Value parameters: Copy of the value is passed
public void Increment(int number)
{
    number++;  // Modifies the COPY, not the original
    Console.WriteLine($"Inside method: {number}");
}

// Usage
int value = 5;
Increment(value);
Console.WriteLine($"Outside method: {value}");

// Output:
// Inside method: 6
// Outside method: 5  (unchanged!)

// Why? Value types (int, double, bool, struct) are copied
```

### Reference Parameters (ref)

```csharp
// ref: Pass by reference - changes affect original
public void Increment(ref int number)
{
    number++;  // Modifies the ORIGINAL
    Console.WriteLine($"Inside method: {number}");
}

// Usage
int value = 5;
Increment(ref value);  // Must use 'ref' keyword
Console.WriteLine($"Outside method: {value}");

// Output:
// Inside method: 6
// Outside method: 6  (changed!)

// Important: Variable must be initialized before passing
int uninit;
// Increment(ref uninit);  // ❌ Compile error: Use of unassigned local variable
```

**Real-World Example:**
```csharp
// Swap two values
public void Swap(ref int a, ref int b)
{
    int temp = a;
    a = b;
    b = temp;
}

int x = 10, y = 20;
Console.WriteLine($"Before: x={x}, y={y}");
Swap(ref x, ref y);
Console.WriteLine($"After: x={x}, y={y}");

// Output:
// Before: x=10, y=20
// After: x=20, y=10
```

### Out Parameters

```csharp
// out: Output parameter - method assigns value
public bool TryParse(string input, out int result)
{
    if (int.TryParse(input, out result))
    {
        return true;
    }

    result = 0;  // Must assign before returning
    return false;
}

// Usage - variable doesn't need to be initialized
if (TryParse("123", out int number))
{
    Console.WriteLine($"Parsed: {number}");
}
else
{
    Console.WriteLine("Parse failed");
}

// C# 7+: Inline out variable declaration
if (int.TryParse("456", out var num))  // Declare inline
{
    Console.WriteLine($"Parsed: {num}");
}

// Discard out parameters you don't need (C# 7+)
if (int.TryParse("789", out _))  // _ = discard
{
    Console.WriteLine("Valid number (but we don't care about the value)");
}
```

**Multiple Out Parameters:**
```csharp
public void GetMinMax(int[] numbers, out int min, out int max)
{
    if (numbers == null || numbers.Length == 0)
    {
        min = 0;
        max = 0;
        return;
    }

    min = numbers[0];
    max = numbers[0];

    foreach (int num in numbers)
    {
        if (num < min) min = num;
        if (num > max) max = num;
    }
}

// Usage
int[] data = { 5, 2, 9, 1, 7 };
GetMinMax(data, out int minimum, out int maximum);
Console.WriteLine($"Min: {minimum}, Max: {maximum}");
// Output: Min: 1, Max: 9
```

### In Parameters (C# 7.2+)

```csharp
// in: Pass by reference (like ref) but READ-ONLY
// Used for performance - avoid copying large structs
public struct LargeStruct
{
    public int Value1;
    public int Value2;
    // ... many more fields
}

// ❌ Expensive: Copies entire struct
public void Process(LargeStruct data)
{
    // Works with copy
}

// ✅ Efficient: Passes reference, no copy
public void ProcessEfficient(in LargeStruct data)
{
    // data.Value1 = 10;  // ❌ Compile error: Cannot modify 'in' parameter
    Console.WriteLine(data.Value1);  // ✅ Can read
}

// Real-world example: Large value types
public readonly struct Vector3
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public Vector3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

// Efficient: No copying, no modification
public double CalculateDistance(in Vector3 point1, in Vector3 point2)
{
    double dx = point2.X - point1.X;
    double dy = point2.Y - point1.Y;
    double dz = point2.Z - point1.Z;
    return Math.Sqrt(dx * dx + dy * dy + dz * dz);
}
```

**When to Use:**
- `value`: Default, for small types (int, bool, char, small structs)
- `ref`: When method needs to modify the parameter
- `out`: When method returns multiple values
- `in`: For large readonly structs (performance optimization)

### Params Parameters

```csharp
// params: Accept variable number of arguments
public int Sum(params int[] numbers)
{
    int total = 0;
    foreach (int num in numbers)
    {
        total += num;
    }
    return total;
}

// Usage - call with any number of arguments
int result1 = Sum(1);              // 1
int result2 = Sum(1, 2);           // 3
int result3 = Sum(1, 2, 3, 4, 5);  // 15

// Or pass an array directly
int[] values = { 1, 2, 3, 4, 5 };
int result4 = Sum(values);         // 15

// params must be the LAST parameter
public void Log(string message, params object[] args)
{
    Console.WriteLine(message, args);
}

Log("Hello {0}, you are {1} years old", "John", 30);
```

### Optional Parameters

```csharp
// Optional parameters: Provide default values
public void CreateUser(string name, int age = 18, string role = "User")
{
    Console.WriteLine($"Name: {name}, Age: {age}, Role: {role}");
}

// Usage - omit optional parameters
CreateUser("Alice");                      // Uses defaults: age=18, role="User"
CreateUser("Bob", 25);                    // Uses default: role="User"
CreateUser("Charlie", 30, "Admin");       // No defaults

// Named arguments (C# 4+): Specify parameter by name
CreateUser("David", role: "Admin");       // age uses default, role specified
CreateUser(age: 40, name: "Eve");         // Order doesn't matter with named args

// Rules:
// 1. Optional parameters must come AFTER required parameters
// 2. Default values must be compile-time constants
```

**Best Practice with Optional Parameters:**
```csharp
// ❌ BAD: Too many optional parameters (hard to read)
public void ConfigureServer(string host, int port = 80, bool useSsl = false,
    int timeout = 30, int maxConnections = 100, bool logging = true,
    string logPath = "/logs", int retries = 3)
{
    // ...
}

// ✅ GOOD: Use a configuration object
public class ServerConfig
{
    public string Host { get; set; }
    public int Port { get; set; } = 80;
    public bool UseSsl { get; set; } = false;
    public int Timeout { get; set; } = 30;
    public int MaxConnections { get; set; } = 100;
    public bool Logging { get; set; } = true;
    public string LogPath { get; set; } = "/logs";
    public int Retries { get; set; } = 3;
}

public void ConfigureServer(ServerConfig config)
{
    // ...
}

// Usage
ConfigureServer(new ServerConfig
{
    Host = "example.com",
    Port = 443,
    UseSsl = true
});
```

## 🔙 Return Values

### Single Return Value

```csharp
// Return value
public int Add(int a, int b)
{
    return a + b;
}

// Early return
public int FindIndex(int[] array, int value)
{
    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] == value)
        {
            return i;  // Exit immediately when found
        }
    }
    return -1;  // Not found
}

// Multiple return statements (different code paths)
public string GetGrade(int score)
{
    if (score >= 90) return "A";
    if (score >= 80) return "B";
    if (score >= 70) return "C";
    if (score >= 60) return "D";
    return "F";
}
```

### Multiple Return Values

**Option 1: Out Parameters**
```csharp
public bool TryDivide(int dividend, int divisor, out int quotient, out int remainder)
{
    if (divisor == 0)
    {
        quotient = 0;
        remainder = 0;
        return false;
    }

    quotient = dividend / divisor;
    remainder = dividend % divisor;
    return true;
}
```

**Option 2: Tuples (Recommended)**
```csharp
// Return tuple with named elements (C# 7+)
public (int Quotient, int Remainder) Divide(int dividend, int divisor)
{
    if (divisor == 0)
    {
        throw new DivideByZeroException();
    }

    return (dividend / divisor, dividend % divisor);
}

// Usage
var result = Divide(17, 5);
Console.WriteLine($"Quotient: {result.Quotient}, Remainder: {result.Remainder}");
// Output: Quotient: 3, Remainder: 2

// Deconstruction
(int quotient, int remainder) = Divide(17, 5);
Console.WriteLine($"Quotient: {quotient}, Remainder: {remainder}");

// Discard values you don't need
(int q, _) = Divide(17, 5);  // Only care about quotient
Console.WriteLine($"Quotient: {q}");
```

**Option 3: Custom Class/Struct**
```csharp
// For complex return data
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public ValidationResult ValidateUser(User user)
{
    var result = new ValidationResult { IsValid = true };

    if (string.IsNullOrWhiteSpace(user.Name))
    {
        result.IsValid = false;
        result.Errors.Add("Name is required");
    }

    if (user.Age < 0 || user.Age > 150)
    {
        result.IsValid = false;
        result.Errors.Add("Age must be between 0 and 150");
    }

    return result;
}
```

## 🎨 Expression-Bodied Members (C# 6+)

```csharp
// Traditional method
public int Add(int a, int b)
{
    return a + b;
}

// Expression-bodied method (C# 6+)
public int Add(int a, int b) => a + b;

// More examples
public string GetFullName(string first, string last) => $"{first} {last}";
public bool IsAdult(int age) => age >= 18;
public void PrintMessage(string msg) => Console.WriteLine(msg);

// Read-only property
public string FullName => $"{FirstName} {LastName}";

// Property with get/set (C# 7+)
private string _name;
public string Name
{
    get => _name;
    set => _name = value ?? throw new ArgumentNullException(nameof(value));
}

// Constructor (C# 7+)
public Person(string name) => Name = name;

// When to use:
// ✅ Simple, single-expression methods/properties
// ❌ Complex logic (use full syntax for readability)
```

## 🔄 Method Overloading

```csharp
// Multiple methods with same name, different parameters
public class Printer
{
    // Print int
    public void Print(int value)
    {
        Console.WriteLine($"Integer: {value}");
    }

    // Print string
    public void Print(string value)
    {
        Console.WriteLine($"String: {value}");
    }

    // Print double
    public void Print(double value)
    {
        Console.WriteLine($"Double: {value:F2}");
    }

    // Print array
    public void Print(int[] values)
    {
        Console.WriteLine($"Array: [{string.Join(", ", values)}]");
    }

    // Different number of parameters
    public void Print(string label, int value)
    {
        Console.WriteLine($"{label}: {value}");
    }
}

// Usage - compiler chooses correct overload
var printer = new Printer();
printer.Print(42);              // Calls Print(int)
printer.Print("Hello");         // Calls Print(string)
printer.Print(3.14);            // Calls Print(double)
printer.Print(new[] { 1, 2 });  // Calls Print(int[])
printer.Print("Count", 5);      // Calls Print(string, int)
```

**Overload Resolution Rules:**
```csharp
public class OverloadExample
{
    public void Method(int value) => Console.WriteLine("int");
    public void Method(long value) => Console.WriteLine("long");
    public void Method(double value) => Console.WriteLine("double");

    public void Demo()
    {
        Method(42);     // Exact match: int
        Method(42L);    // Exact match: long
        Method(42.0);   // Exact match: double

        byte b = 10;
        Method(b);      // Implicit conversion byte → int
    }
}

// Ambiguous overloads (compile error)
public void Ambiguous(int a, double b) { }
public void Ambiguous(double a, int b) { }

// Ambiguous(1, 1);  // ❌ Compile error: Both overloads are valid
```

## 🔧 Local Functions (C# 7+)

```csharp
// Functions defined inside other methods
public int[] GetPrimes(int max)
{
    var primes = new List<int>();

    for (int i = 2; i <= max; i++)
    {
        if (IsPrime(i))  // Call local function
        {
            primes.Add(i);
        }
    }

    return primes.ToArray();

    // Local function - only accessible within GetPrimes
    bool IsPrime(int number)
    {
        if (number < 2) return false;

        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }

        return true;
    }
}

// Benefits:
// 1. Encapsulation - IsPrime is not visible outside GetPrimes
// 2. Clarity - Helper function is close to where it's used
// 3. Performance - Can be inlined by JIT compiler
```

**Local Functions with Captures:**
```csharp
public void ProcessItems(List<int> items, int threshold)
{
    var processed = 0;

    // Local function captures 'threshold' and 'processed'
    void ProcessItem(int item)
    {
        if (item > threshold)  // Accesses parameter
        {
            Console.WriteLine($"Processing: {item}");
            processed++;  // Modifies local variable
        }
    }

    foreach (var item in items)
    {
        ProcessItem(item);
    }

    Console.WriteLine($"Processed {processed} items");
}
```

**Static Local Functions (C# 8+):**
```csharp
public int Calculate(int a, int b)
{
    // Static local function - cannot capture variables
    static int Add(int x, int y)
    {
        // Cannot access 'a' or 'b' here
        return x + y;
    }

    return Add(a, b);
}

// Benefits:
// - Prevents accidental capture (better performance)
// - Makes intent clear (function doesn't depend on outer scope)
```

## ✅ Best Practices

### 1. Single Responsibility Principle
```csharp
// ❌ BAD: Method does too much
public void ProcessOrder(Order order)
{
    // Validate
    if (order == null) throw new ArgumentNullException();
    if (order.Items.Count == 0) throw new InvalidOperationException();

    // Calculate
    decimal total = 0;
    foreach (var item in order.Items)
    {
        total += item.Price * item.Quantity;
    }

    // Apply discount
    if (order.Customer.Type == CustomerType.VIP)
    {
        total *= 0.9m;
    }

    // Save to database
    _database.Orders.Add(order);
    _database.SaveChanges();

    // Send email
    _emailService.SendOrderConfirmation(order);
}

// ✅ GOOD: Each method has one responsibility
public void ProcessOrder(Order order)
{
    ValidateOrder(order);
    decimal total = CalculateTotal(order);
    order.Total = ApplyDiscounts(order, total);
    SaveOrder(order);
    SendConfirmation(order);
}

private void ValidateOrder(Order order) { /* ... */ }
private decimal CalculateTotal(Order order) { /* ... */ }
private decimal ApplyDiscounts(Order order, decimal total) { /* ... */ }
private void SaveOrder(Order order) { /* ... */ }
private void SendConfirmation(Order order) { /* ... */ }
```

### 2. Method Length
```csharp
// Keep methods short (ideally < 20 lines)
// If longer, extract helper methods
```

### 3. Parameter Count
```csharp
// ❌ BAD: Too many parameters (hard to use)
public void CreateUser(string firstName, string lastName, string email,
    string phone, string address, string city, string state, string zip)
{ }

// ✅ GOOD: Use a parameter object
public class CreateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Address Address { get; set; }
}

public void CreateUser(CreateUserRequest request) { }
```

### 4. Naming
```csharp
// ✅ Use verbs for methods
public void SaveOrder() { }
public int CalculateTotal() { }
public bool ValidateUser() { }

// ✅ Use async suffix for async methods
public async Task<Order> GetOrderAsync(int id) { }

// ✅ Use Try prefix for methods that return bool
public bool TryParse(string input, out int result) { }
```

## ⏭️ Next Lesson

Proceed to **[Lesson 05: Error Handling](05-error-handling.md)** to learn about:
- Exceptions and exception handling
- Try-catch-finally blocks
- Custom exceptions
- Exception best practices

## 📚 Additional Resources

- [Methods (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/methods)
- [Parameters](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters)
- [Expression-Bodied Members](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members)

---

*"A method should do one thing, do it well, and do it only." - Adapted from Unix Philosophy*
