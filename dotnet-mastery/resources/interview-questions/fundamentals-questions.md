# C# Fundamentals Interview Questions

## 📋 Overview

This document contains common interview questions for C# fundamentals, covering topics from Modules 01-03. Each question includes:
- The question
- Expected answer
- Follow-up questions
- Difficulty level

## 🎯 Basic Questions (Entry Level)

### Q1: What is the difference between value types and reference types?

**Answer:**

**Value Types:**
- Stored on the stack
- Contains the actual data
- Copied when assigned
- Cannot be null (without Nullable<T>)
- Examples: int, double, bool, struct

**Reference Types:**
- Stored on the heap
- Variable contains a reference (memory address)
- Reference is copied when assigned
- Can be null
- Garbage collected
- Examples: class, interface, array, string

**Example:**
```csharp
// Value type
int a = 10;
int b = a;  // Copy of value
b = 20;
Console.WriteLine(a);  // Output: 10 (unchanged)

// Reference type
var person1 = new Person { Name = "John" };
var person2 = person1;  // Copy of reference (same object)
person2.Name = "Jane";
Console.WriteLine(person1.Name);  // Output: "Jane" (changed!)
```

**Follow-up:** What happens with boxing and unboxing?

**Difficulty:** ⭐ Basic

---

### Q2: What is the difference between `==` and `.Equals()`?

**Answer:**

- `==`: Compares references for reference types (unless overloaded)
- `.Equals()`: Can be overridden to compare values

**Example:**
```csharp
string s1 = "hello";
string s2 = "hello";
string s3 = new string(new[] { 'h', 'e', 'l', 'l', 'o' });

// String overloads == to compare values
s1 == s2;        // true (value comparison)
s1 == s3;        // true (value comparison)

// Equals also compares values
s1.Equals(s3);   // true

// ReferenceEquals compares references
ReferenceEquals(s1, s2);  // true (interning)
ReferenceEquals(s1, s3);  // false (different objects)
```

**Follow-up:** What is string interning?

**Difficulty:** ⭐⭐ Intermediate

---

### Q3: Explain the `var` keyword. When should you use it?

**Answer:**

`var` is implicitly typed - the compiler infers the type from the right-hand side.

**Rules:**
- Must be initialized
- Type determined at compile-time (not runtime)
- Cannot be null without explicit type
- Not dynamic typing

**Use when:**
✅ Type is obvious from right side
```csharp
var customer = new Customer();  // Obvious
var items = new List<int>();    // Obvious
```

**Don't use when:**
❌ Type is not obvious
```csharp
var result = GetSomething();  // What type is result?
```

**Follow-up:** What's the difference between `var` and `dynamic`?

**Difficulty:** ⭐ Basic

---

### Q4: What is the difference between `throw` and `throw ex`?

**Answer:**

- `throw;`: Rethrows the original exception, preserving stack trace
- `throw ex;`: Throws exception as if it originated here, losing original stack trace

**Example:**
```csharp
try
{
    // Code that throws exception
}
catch (Exception ex)
{
    LogError(ex);

    throw;      // ✅ Good: Preserves stack trace
    // throw ex;   // ❌ Bad: Loses original stack trace
}
```

**Stack trace with `throw;`:**
```
at OriginalMethod() in File.cs:line 10
at CallingMethod() in File.cs:line 20
```

**Stack trace with `throw ex;`:**
```
at CallingMethod() in File.cs:line 23  // Lost original location!
```

**Follow-up:** When would you want to throw a new exception?

**Difficulty:** ⭐⭐ Intermediate

---

## 🎯 Intermediate Questions

### Q5: Explain boxing and unboxing. Why are they expensive?

**Answer:**

**Boxing:** Converting value type to reference type (object)
```csharp
int i = 123;
object obj = i;  // Boxing - allocates heap memory
```

**Unboxing:** Converting reference type back to value type
```csharp
int j = (int)obj;  // Unboxing - type check + copy
```

**Why expensive?**
1. **Memory allocation**: Boxing allocates on heap (slower than stack)
2. **Garbage collection**: Creates objects that need to be collected
3. **Type checking**: Unboxing requires runtime type check
4. **Copy overhead**: Data copied twice (value → heap → value)

**Performance impact:**
```csharp
// ❌ BAD: Boxing in loop
var list = new ArrayList();  // Stores objects
for (int i = 0; i < 1000; i++)
{
    list.Add(i);  // 1000 boxing operations!
}

// ✅ GOOD: No boxing
var list = new List<int>();  // Generic - no boxing
for (int i = 0; i < 1000; i++)
{
    list.Add(i);  // No boxing - stored directly
}
```

**Benchmark:**
- ArrayList (boxing): ~10,000 ns, 16 KB allocated
- List<int>: ~500 ns, 4 KB allocated
- **20x faster, 4x less memory**

**Follow-up:** How do generics help avoid boxing?

**Difficulty:** ⭐⭐ Intermediate

---

### Q6: What is the difference between `string` and `StringBuilder`? When would you use each?

**Answer:**

**string:**
- Immutable (cannot be modified)
- Every modification creates a new string
- Good for: Few modifications, thread-safe sharing

**StringBuilder:**
- Mutable (can be modified)
- Modifies internal buffer
- Good for: Many concatenations, loops

**Performance comparison:**
```csharp
// ❌ BAD: String concatenation in loop
string result = "";
for (int i = 0; i < 1000; i++)
{
    result += i;  // Creates 1000 new strings!
}
// Time: ~5,000 μs
// Memory: 1000+ string objects

// ✅ GOOD: StringBuilder
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    sb.Append(i);  // Modifies buffer
}
string result = sb.ToString();  // One final string
// Time: ~50 μs
// Memory: One StringBuilder + final string
```

**Rule of thumb:**
- **string**: < 5 concatenations
- **StringBuilder**: 5+ concatenations or loops
- **string interpolation**: Most other cases

**Follow-up:** Why is string immutability important?

**Difficulty:** ⭐⭐ Intermediate

---

### Q7: Explain method overloading and overriding. What's the difference?

**Answer:**

**Overloading (Compile-time polymorphism):**
- Multiple methods with same name, different parameters
- Resolved at compile-time

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public int Add(int a, int b, int c) => a + b + c;
}
```

**Overriding (Runtime polymorphism):**
- Derived class provides new implementation of base class method
- Base method must be `virtual` or `abstract`
- Derived method must use `override`
- Resolved at runtime based on actual object type

```csharp
public class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("Some sound");
    }
}

public class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
}

Animal animal = new Dog();
animal.MakeSound();  // Output: "Woof!" (runtime polymorphism)
```

**Key differences:**

| Overloading | Overriding |
|-------------|------------|
| Same class | Different classes (inheritance) |
| Different parameters | Same parameters |
| Compile-time | Runtime |
| Any methods | Virtual/abstract methods |

**Follow-up:** What is method hiding with `new`?

**Difficulty:** ⭐⭐ Intermediate

---

### Q8: What are nullable reference types (C# 8+)? Why were they introduced?

**Answer:**

**Problem:** NullReferenceException - the "billion dollar mistake"
```csharp
string name = GetName();
Console.WriteLine(name.Length);  // May throw NullReferenceException!
```

**Solution:** Nullable reference types
```csharp
// Enable in .csproj: <Nullable>enable</Nullable>

// Non-nullable (default)
string name = GetName();  // Warning if GetName() might return null

// Nullable (explicit)
string? optionalName = GetOptionalName();  // Can be null
Console.WriteLine(optionalName.Length);     // Warning: possible null

// Safe patterns
if (optionalName != null)
{
    Console.WriteLine(optionalName.Length);  // ✅ Safe
}

// Or use null operators
int length = optionalName?.Length ?? 0;
```

**Benefits:**
1. Catches potential null reference bugs at compile-time
2. Makes intent explicit (nullable vs non-nullable)
3. Better documentation
4. Helps prevent NullReferenceException

**Important:** It's a compile-time feature, not runtime enforcement!

**Follow-up:** What are null operators (?, ??, ??=)?

**Difficulty:** ⭐⭐ Intermediate

---

## 🎯 Advanced Questions

### Q9: Explain the difference between `IEnumerable<T>` and `IQueryable<T>`.

**Answer:**

**IEnumerable<T>:**
- In-memory collection
- LINQ to Objects
- Executes on client side
- Filters after retrieval

**IQueryable<T>:**
- Database/external source
- LINQ to SQL/EF
- Builds expression tree
- Translates to SQL
- Filters at source (database)

**Example:**
```csharp
// IEnumerable - in memory
List<Customer> customers = GetAllCustomers();  // Loads ALL customers
var activeCustomers = customers.Where(c => c.IsActive);  // Filters in memory

// IQueryable - database
IQueryable<Customer> customers = dbContext.Customers;  // No data loaded yet
var activeCustomers = customers.Where(c => c.IsActive);  // Builds SQL WHERE clause
var result = activeCustomers.ToList();  // NOW executes: SELECT * FROM Customers WHERE IsActive = 1
```

**Performance impact:**
- IEnumerable: Load 1 million records, filter in memory → Slow, memory-intensive
- IQueryable: Filter in database, load only matching records → Fast, efficient

**When to use:**
- **IEnumerable**: In-memory collections (List, Array)
- **IQueryable**: Database queries (EF Core, LINQ to SQL)

**Follow-up:** What is deferred execution?

**Difficulty:** ⭐⭐⭐ Advanced

---

### Q10: What is the difference between `async`/`await` and `Task.Run()`?

**Answer:**

**async/await:**
- For I/O-bound operations
- Doesn't create new thread
- Releases thread while waiting
- Continuation on available thread

```csharp
// I/O-bound: Reading file, API call, database query
public async Task<string> ReadFileAsync(string path)
{
    return await File.ReadAllTextAsync(path);
    // Thread released while waiting for disk I/O
}
```

**Task.Run():**
- For CPU-bound operations
- Creates task on thread pool
- Occupies thread for duration
- For offloading work from UI thread

```csharp
// CPU-bound: Heavy computation, image processing
public Task<int> CalculatePrimesAsync(int max)
{
    return Task.Run(() =>
    {
        // CPU-intensive work on background thread
        return FindPrimes(max);
    });
}
```

**Common mistake:**
```csharp
// ❌ BAD: Unnecessary Task.Run with async API
public Task<string> ReadFileAsync(string path)
{
    return Task.Run(async () =>
    {
        return await File.ReadAllTextAsync(path);
    });
    // Extra thread pool overhead for no benefit!
}

// ✅ GOOD: Direct async
public async Task<string> ReadFileAsync(string path)
{
    return await File.ReadAllTextAsync(path);
    // No extra thread - efficient
}
```

**Rule:**
- **async/await**: API already async (File I/O, HttpClient, EF Core)
- **Task.Run()**: Synchronous CPU-intensive code that needs to be async

**Follow-up:** What is ConfigureAwait(false)?

**Difficulty:** ⭐⭐⭐ Advanced

---

## 🎯 Expert Questions

### Q11: Explain garbage collection in .NET. How does it work?

**Answer:**

**Generations:**
- **Gen 0**: Short-lived objects (new allocations)
- **Gen 1**: Medium-lived objects (survived one GC)
- **Gen 2**: Long-lived objects (survived multiple GCs)

**How it works:**

1. **Allocation**: Objects allocated in Gen 0
2. **GC Trigger**: Gen 0 full, memory pressure, or explicit GC.Collect()
3. **Mark Phase**: Mark reachable objects (via roots: stack, static variables)
4. **Sweep Phase**: Reclaim unreachable objects
5. **Compact Phase**: Move objects together (reduce fragmentation)
6. **Promotion**: Surviving objects promoted to next generation

**Performance implications:**
```csharp
// ❌ BAD: Many short-lived objects (frequent Gen 0 GC)
for (int i = 0; i < 1_000_000; i++)
{
    var temp = new LargeObject();  // Allocates on heap
    // temp immediately eligible for collection
}
// Causes many GC pauses

// ✅ GOOD: Object pooling or value types
var pool = new ObjectPool<LargeObject>();
for (int i = 0; i < 1_000_000; i++)
{
    var obj = pool.Get();
    // Use obj
    pool.Return(obj);
}
// Fewer allocations → fewer GC pauses
```

**GC Modes:**
- **Workstation GC**: Lower latency, smaller heap
- **Server GC**: Higher throughput, larger heap, one heap per CPU

**Best practices:**
1. Avoid allocations in hot paths
2. Use object pooling for frequently allocated objects
3. Prefer value types for small data
4. Dispose of IDisposable resources properly
5. Avoid finalizers (use IDisposable instead)

**Follow-up:** What is LOH (Large Object Heap)?

**Difficulty:** ⭐⭐⭐⭐ Expert

---

### Q12: What are Span<T> and Memory<T>? When would you use them?

**Answer:**

**Span<T>:** (C# 7.2+)
- Stack-only type (ref struct)
- Zero-allocation view over contiguous memory
- Cannot be boxed or used as generic type argument
- Cannot cross async boundaries

**Memory<T>:**
- Heap-allocated
- Can be used in async methods
- Slower than Span<T> but more flexible

**Benefits:**
1. **Zero allocations**: No heap allocation for slices
2. **Performance**: Direct memory access
3. **Unified API**: Arrays, strings, stackalloc

**Example:**
```csharp
// Traditional approach (allocates)
string text = "Hello, World!";
string hello = text.Substring(0, 5);  // Allocates new string

// Span approach (zero-allocation)
ReadOnlySpan<char> text = "Hello, World!";
ReadOnlySpan<char> hello = text.Slice(0, 5);  // No allocation!

// Performance-critical parsing
public static int ParseInt(ReadOnlySpan<char> text)
{
    int result = 0;
    foreach (char c in text)
    {
        result = result * 10 + (c - '0');
    }
    return result;
}

// Usage
ReadOnlySpan<char> span = "12345";
int number = ParseInt(span);  // No allocation!
```

**When to use:**
- High-performance scenarios
- Parsing/processing large data
- Avoiding string/array allocations
- Buffer management

**Benchmark:**
```
Method          | Mean     | Allocated
----------------|----------|----------
Substring       | 45.2 ns  | 56 B
Span.Slice      | 0.8 ns   | 0 B
```

**Follow-up:** What is stackalloc?

**Difficulty:** ⭐⭐⭐⭐ Expert

---

## 📚 Study Tips

1. **Practice coding**: Theory is important, but practice more
2. **Explain to others**: If you can explain it, you understand it
3. **Build projects**: Apply concepts in real scenarios
4. **Read source code**: Study .NET runtime source on GitHub
5. **Use debugger**: Step through code to understand flow
6. **Benchmark**: Measure actual performance differences
7. **Stay current**: Follow .NET blog and C# updates

## 🎯 Additional Question Categories

See also:
- [OOP Interview Questions](oop-questions.md)
- [LINQ Interview Questions](linq-questions.md)
- [ASP.NET Core Interview Questions](aspnet-questions.md)
- [Design Patterns Interview Questions](patterns-questions.md)

---

**Pro Tip:** For interviews, be ready to write code on a whiteboard or in an online editor. Practice explaining your thought process!
