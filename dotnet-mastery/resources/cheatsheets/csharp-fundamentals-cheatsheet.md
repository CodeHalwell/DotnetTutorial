# C# Fundamentals Cheatsheet

## 📋 Quick Reference Guide

This cheatsheet provides quick access to essential C# syntax and concepts from Modules 01-03.

## 🔢 Types and Variables

### Value Types

```csharp
// Integer types
byte    u8 = 255;           // 0 to 255
sbyte   s8 = -128;          // -128 to 127
short   s16 = 32767;        // -32,768 to 32,767
ushort  u16 = 65535;        // 0 to 65,535
int     s32 = 2147483647;   // -2.1B to 2.1B (most common)
uint    u32 = 4294967295u;  // 0 to 4.3B
long    s64 = 9223372036854775807L;
ulong   u64 = 18446744073709551615UL;

// Floating-point types
float   f32 = 3.14f;        // 6-9 digits precision
double  f64 = 3.14159;      // 15-17 digits (default)
decimal dec = 79.99m;       // 28-29 digits (financial)

// Other
bool    flag = true;        // true or false
char    ch = 'A';           // Single character
```

### Reference Types

```csharp
string  text = "Hello";     // Immutable text
object  obj = new object(); // Base type
int[]   arr = new int[5];   // Array
```

### Type Conversion

```csharp
// Implicit (safe)
int i = 42;
long l = i;                 // int → long

// Explicit (cast)
double d = 3.14;
int i = (int)d;             // Loses decimal

// Parse/TryParse
int num = int.Parse("123");
if (int.TryParse("456", out int result))
{
    // Use result
}

// Convert class
int i = Convert.ToInt32("789");
```

## 🔄 Control Flow

### Conditional Statements

```csharp
// If-else
if (condition)
{
    // Code
}
else if (condition2)
{
    // Code
}
else
{
    // Code
}

// Ternary operator
string result = (age >= 18) ? "Adult" : "Minor";

// Switch statement
switch (value)
{
    case 1:
        // Code
        break;
    case 2:
    case 3:
        // Multiple cases
        break;
    default:
        // Default case
        break;
}

// Switch expression (C# 8+)
string day = dayNum switch
{
    1 => "Monday",
    2 => "Tuesday",
    _ => "Unknown"
};
```

### Loops

```csharp
// For loop
for (int i = 0; i < 10; i++)
{
    // Code
}

// Foreach loop
foreach (var item in collection)
{
    // Code
}

// While loop
while (condition)
{
    // Code
}

// Do-while loop
do
{
    // Code
} while (condition);
```

### Jump Statements

```csharp
break;      // Exit loop/switch
continue;   // Skip to next iteration
return;     // Exit method
goto label; // Jump to label (avoid)
```

## 📝 Methods

### Basic Method

```csharp
public int Add(int a, int b)
{
    return a + b;
}
```

### Parameters

```csharp
// Value parameter (default)
void Method(int x) { }

// Reference parameter
void Method(ref int x) { }

// Out parameter
bool TryParse(string s, out int result) { }

// In parameter (readonly reference)
void Method(in LargeStruct x) { }

// Params (variable arguments)
int Sum(params int[] numbers) { }

// Optional parameters
void Method(int required, int optional = 10) { }
```

### Expression-Bodied Members

```csharp
// Method
public int Add(int a, int b) => a + b;

// Property
public string FullName => $"{First} {Last}";

// Get/Set (C# 7+)
public string Name
{
    get => _name;
    set => _name = value;
}
```

## 🚨 Exception Handling

### Try-Catch-Finally

```csharp
try
{
    // Risky code
}
catch (SpecificException ex)
{
    // Handle specific exception
}
catch (Exception ex)
{
    // Handle all exceptions
}
finally
{
    // Always executes
}
```

### Throw

```csharp
// Throw exception
throw new ArgumentException("Invalid argument");

// Rethrow (preserves stack trace)
catch (Exception ex)
{
    throw;  // Good
    // throw ex;  Bad - loses stack trace
}

// Throw expression (C# 7+)
string name = input ?? throw new ArgumentNullException();
```

### Common Exceptions

```csharp
ArgumentException
ArgumentNullException
ArgumentOutOfRangeException
InvalidOperationException
NotImplementedException
NotSupportedException
NullReferenceException
IndexOutOfRangeException
DivideByZeroException
```

## 🎨 String Handling

```csharp
// String interpolation
string msg = $"Hello, {name}!";

// Verbatim string
string path = @"C:\Users\John\file.txt";

// Raw string literal (C# 11+)
string json = """
{
    "name": "value"
}
""";

// StringBuilder (for concatenation)
var sb = new StringBuilder();
sb.Append("Hello");
sb.Append(" World");
string result = sb.ToString();

// String methods
text.ToUpper();
text.ToLower();
text.Trim();
text.Contains("sub");
text.StartsWith("pre");
text.EndsWith("suf");
text.Replace("old", "new");
text.Split(',');
string.Join(", ", array);
```

## 🔢 Math Operations

```csharp
// Arithmetic
int sum = a + b;
int diff = a - b;
int prod = a * b;
int quot = a / b;
int rem = a % b;

// Math class
Math.Abs(-5);           // 5
Math.Max(10, 20);       // 20
Math.Min(10, 20);       // 10
Math.Pow(2, 8);         // 256
Math.Sqrt(16);          // 4
Math.Round(3.7);        // 4
Math.Floor(3.7);        // 3
Math.Ceiling(3.1);      // 4
Math.PI;                // 3.14159...
Math.E;                 // 2.71828...
```

## 📦 Collections (Quick Reference)

```csharp
// Array
int[] arr = new int[5];
int[] arr = { 1, 2, 3, 4, 5 };

// List
var list = new List<int>();
list.Add(1);
list.Remove(1);
list.Count;
list.Clear();

// Dictionary
var dict = new Dictionary<string, int>();
dict["key"] = 42;
dict.TryGetValue("key", out int value);
dict.ContainsKey("key");
dict.Remove("key");
```

## 🎯 Nullable Types

```csharp
// Nullable value type
int? nullable = null;
if (nullable.HasValue)
{
    int value = nullable.Value;
}

// Nullable reference type (C# 8+)
string? nullableString = null;

// Null operators
string result = nullable ?? "default";     // Null-coalescing
nullable ??= 10;                           // Null-coalescing assignment
int length = text?.Length ?? 0;            // Null-conditional
```

## 🏛️ OOP Basics

### Class

```csharp
public class Person
{
    // Field
    private string _name;

    // Property
    public string Name { get; set; }

    // Constructor
    public Person(string name)
    {
        Name = name;
    }

    // Method
    public void SayHello()
    {
        Console.WriteLine($"Hello, {Name}!");
    }
}
```

### Inheritance

```csharp
public class Employee : Person
{
    public int EmployeeId { get; set; }

    public Employee(string name, int id) : base(name)
    {
        EmployeeId = id;
    }
}
```

### Interface

```csharp
public interface IRepository<T>
{
    T GetById(int id);
    void Save(T entity);
    void Delete(int id);
}

public class CustomerRepository : IRepository<Customer>
{
    // Implementation
}
```

## 🔧 Console I/O

```csharp
// Output
Console.WriteLine("Hello");
Console.Write("No newline");

// Input
string input = Console.ReadLine();
ConsoleKeyInfo key = Console.ReadKey();

// Formatting
Console.WriteLine($"Value: {value}");
Console.WriteLine("Name: {0}, Age: {1}", name, age);

// Color
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Green text");
Console.ResetColor();
```

## 📁 File I/O

```csharp
// Read
string text = File.ReadAllText("file.txt");
string[] lines = File.ReadAllLines("file.txt");

// Write
File.WriteAllText("file.txt", "content");
File.WriteAllLines("file.txt", new[] { "line1", "line2" });

// Append
File.AppendAllText("file.txt", "more content");

// Check existence
bool exists = File.Exists("file.txt");

// Using stream
using (var stream = File.OpenRead("file.txt"))
{
    // Read from stream
}
```

## ⏱️ DateTime

```csharp
// Current date/time
DateTime now = DateTime.Now;        // Local time
DateTime utc = DateTime.UtcNow;     // UTC time
DateOnly date = DateOnly.FromDateTime(now);  // C# 10+
TimeOnly time = TimeOnly.FromDateTime(now);  // C# 10+

// Create
DateTime dt = new DateTime(2025, 11, 12);
DateTime dt = new DateTime(2025, 11, 12, 14, 30, 0);

// Formatting
dt.ToString("yyyy-MM-dd");          // 2025-11-12
dt.ToString("HH:mm:ss");            // 14:30:00
dt.ToString("yyyy-MM-dd HH:mm");    // 2025-11-12 14:30

// Arithmetic
DateTime future = now.AddDays(7);
DateTime past = now.AddHours(-2);
TimeSpan diff = future - now;
```

## 🎨 Formatting

```csharp
// Currency
decimal price = 123.45m;
price.ToString("C");                // $123.45

// Number
int num = 1234567;
num.ToString("N0");                 // 1,234,567
num.ToString("N2");                 // 1,234,567.00

// Percentage
double percent = 0.1234;
percent.ToString("P2");             // 12.34%

// Custom
int num = 42;
num.ToString("D5");                 // 00042

// Date
DateTime dt = DateTime.Now;
dt.ToString("yyyy-MM-dd");          // 2025-11-12
dt.ToString("MMMM dd, yyyy");       // November 12, 2025
```

## 🚀 Modern C# Features

### Pattern Matching (C# 7+)

```csharp
// Type pattern
if (obj is string text)
{
    Console.WriteLine(text.ToUpper());
}

// Property pattern
if (person is { Age: >= 18 })
{
    Console.WriteLine("Adult");
}

// Switch expression
string result = value switch
{
    < 0 => "Negative",
    0 => "Zero",
    > 0 and <= 100 => "Positive (1-100)",
    _ => "Large"
};
```

### Tuples (C# 7+)

```csharp
// Create
(int, string) tuple = (1, "one");
var tuple = (Id: 1, Name: "John");

// Deconstruct
(int id, string name) = tuple;
var (id, name) = tuple;

// Return multiple values
public (int Min, int Max) GetRange(int[] nums)
{
    return (nums.Min(), nums.Max());
}
```

### Records (C# 9+)

```csharp
// Immutable data class
public record Person(string Name, int Age);

// Usage
var person = new Person("John", 30);
var older = person with { Age = 31 };

// Comparison (value-based)
person1 == person2  // Compares content
```

## 💡 Best Practices

```csharp
// ✅ Use var when type is obvious
var list = new List<int>();

// ✅ Use explicit type when not obvious
string result = CalculateSomething();

// ✅ Use const for compile-time constants
const int MaxRetries = 3;

// ✅ Use readonly for runtime constants
private readonly string _connectionString;

// ✅ Use null-coalescing
string name = input ?? "Default";

// ✅ Use string interpolation
string msg = $"Hello, {name}!";

// ✅ Use expression-bodied members
public int Square(int x) => x * x;

// ✅ Use pattern matching
if (obj is string { Length: > 0 } text) { }
```

## 📚 Naming Conventions

```csharp
// PascalCase
public class CustomerService { }
public void ProcessOrder() { }
public string FirstName { get; set; }

// camelCase
private string _connectionString;
int localVariable = 42;

// UPPER_CASE
public const int MAX_RETRY_COUNT = 3;

// Interfaces start with 'I'
public interface ICustomerRepository { }

// Async methods end with 'Async'
public async Task<Customer> GetCustomerAsync() { }
```

---

**Print this cheatsheet** for quick reference while coding!

**Next**: [LINQ Cheatsheet](linq-cheatsheet.md) | [ASP.NET Core Cheatsheet](aspnetcore-cheatsheet.md)
