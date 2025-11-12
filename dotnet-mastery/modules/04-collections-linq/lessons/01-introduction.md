# Module 04: Collections & LINQ - Introduction

## 📘 Mastering Data Manipulation in .NET

Collections and LINQ (Language Integrated Query) are fundamental to working with data in .NET. Master these to write elegant, efficient code for data processing, filtering, transformation, and querying.

## 🎯 Module Objectives

By the end of this module, you will:

- ✅ Master all major collection types (List, Dictionary, HashSet, Queue, Stack)
- ✅ Understand when to use each collection type
- ✅ Write powerful LINQ queries
- ✅ Understand deferred vs immediate execution
- ✅ Optimize collection operations for performance
- ✅ Use advanced LINQ features (grouping, joining, aggregation)
- ✅ Implement custom collections and iterators
- ✅ Apply functional programming concepts

## 🗺️ Module Structure

### Lessons
1. **Introduction** (this document) - Collections and LINQ overview
2. **Arrays and Lists** - Fixed and dynamic collections
3. **Dictionaries and Sets** - Key-value pairs and unique values
4. **Specialized Collections** - Queue, Stack, immutable collections
5. **LINQ Fundamentals** - Query syntax and method syntax
6. **Advanced LINQ** - Complex queries, performance, custom operators
7. **Functional Programming** - Select, Where, Aggregate, and more
8. **Best Practices** - Performance, readability, common patterns

### Time Commitment
- **Estimated Time**: 2-3 weeks
- **Lessons**: 10-12 hours
- **Exercises**: 15-20 hours
- **Projects**: 10-15 hours

## 🎯 Why Collections & LINQ?

### The Problem: Working with Data

```csharp
// Without collections & LINQ (procedural, verbose)
int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Find even numbers greater than 5
List<int> result = new List<int>();
for (int i = 0; i < numbers.Length; i++)
{
    if (numbers[i] % 2 == 0 && numbers[i] > 5)
    {
        result.Add(numbers[i]);
    }
}

// Calculate sum
int sum = 0;
for (int i = 0; i < result.Count; i++)
{
    sum += result[i];
}

Console.WriteLine($"Sum: {sum}");
// Output: Sum: 24 (6 + 8 + 10)
```

### The Solution: LINQ

```csharp
// With LINQ (declarative, concise, readable)
int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

int sum = numbers
    .Where(n => n % 2 == 0)  // Even numbers
    .Where(n => n > 5)        // Greater than 5
    .Sum();                   // Calculate sum

Console.WriteLine($"Sum: {sum}");
// Output: Sum: 24

// Even more concise
int sum2 = numbers.Where(n => n % 2 == 0 && n > 5).Sum();
```

**Benefits:**
- ✅ **Readable**: Clear intent ("where even and > 5")
- ✅ **Concise**: One line vs 10+ lines
- ✅ **Composable**: Chain operations easily
- ✅ **Type-safe**: Compile-time checking
- ✅ **Consistent**: Same syntax for arrays, lists, databases

## 📊 Collection Types Overview

### Hierarchy

```mermaid
graph TD
    A[IEnumerable<T>] --> B[ICollection<T>]
    B --> C[IList<T>]
    B --> D[ISet<T>]
    B --> E[IDictionary<K,V>]

    C --> F[List<T>]
    C --> G[Array T[]]

    D --> H[HashSet<T>]
    D --> I[SortedSet<T>]

    E --> J[Dictionary<K,V>]
    E --> K[SortedDictionary<K,V>]

    A --> L[IQueryable<T>]
```

### Quick Comparison

| Collection | Use When | Time Complexity | Key Features |
|------------|----------|-----------------|--------------|
| **Array `T[]`** | Size fixed, fast access | O(1) get/set | Fixed size, fastest |
| **List\<T\>** | Dynamic size, indexed access | O(1) get/set, O(n) insert | Most versatile |
| **Dictionary\<K,V\>** | Key-value lookups | O(1) average | Fast lookups |
| **HashSet\<T\>** | Unique values, membership tests | O(1) average | No duplicates |
| **Queue\<T\>** | FIFO (First In, First Out) | O(1) enqueue/dequeue | Task queues |
| **Stack\<T\>** | LIFO (Last In, First Out) | O(1) push/pop | Undo operations |
| **LinkedList\<T\>** | Frequent insertions/deletions | O(1) insert/remove | No indexing |
| **SortedSet\<T\>** | Sorted unique values | O(log n) | Maintained order |

## 🎨 Arrays

### Declaration and Initialization

```csharp
// Declaration
int[] numbers;

// Initialization
numbers = new int[5];  // [0, 0, 0, 0, 0]

// Declaration + initialization
int[] numbers2 = new int[5];

// Initialize with values
int[] numbers3 = new int[] { 1, 2, 3, 4, 5 };

// Shorter syntax (type inference)
int[] numbers4 = { 1, 2, 3, 4, 5 };

// Using target-typed new (C# 9+)
int[] numbers5 = new[] { 1, 2, 3, 4, 5 };
```

### Array Operations

```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

// Access by index
int first = numbers[0];     // 1
int last = numbers[^1];     // 5 (index from end, C# 8+)

// Length
int length = numbers.Length;  // 5

// Iterate
foreach (int num in numbers)
{
    Console.WriteLine(num);
}

// Array methods
Array.Sort(numbers);          // Sort in-place
Array.Reverse(numbers);       // Reverse in-place
int index = Array.IndexOf(numbers, 3);  // Find index of value
Array.Clear(numbers, 0, 2);   // Clear first 2 elements

// Copy
int[] copy = new int[numbers.Length];
Array.Copy(numbers, copy, numbers.Length);

// Resize (creates new array)
Array.Resize(ref numbers, 10);
```

### Multi-dimensional Arrays

```csharp
// 2D array (matrix)
int[,] matrix = new int[3, 3]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

// Access
int value = matrix[1, 1];  // 5

// Jagged array (array of arrays)
int[][] jagged = new int[3][];
jagged[0] = new int[] { 1, 2 };
jagged[1] = new int[] { 3, 4, 5 };
jagged[2] = new int[] { 6 };

// Access
int value2 = jagged[1][2];  // 5
```

### Arrays vs Lists

```csharp
// Array - Fixed size
int[] array = new int[5];
// array.Add(6);  // ❌ Cannot add - fixed size

// List - Dynamic size
List<int> list = new List<int>();
list.Add(1);
list.Add(2);
list.Add(3);  // ✅ Can grow dynamically

// When to use Array:
// ✅ Size known at creation
// ✅ Performance-critical (slightly faster)
// ✅ Interop with native code

// When to use List:
// ✅ Size changes dynamically
// ✅ Need Add/Remove/Insert
// ✅ Most common scenarios
```

## 📝 List\<T\>

### Common Operations

```csharp
// Create
var numbers = new List<int>();

// Add elements
numbers.Add(1);
numbers.Add(2);
numbers.AddRange(new[] { 3, 4, 5 });

// Initialize with values
var numbers2 = new List<int> { 1, 2, 3, 4, 5 };

// Collection initializer (C# 12+)
var numbers3 = new List<int> { 1, 2, 3, 4, 5 };

// Access
int first = numbers[0];
int last = numbers[^1];  // C# 8+ index from end

// Count
int count = numbers.Count;

// Insert
numbers.Insert(0, 0);  // Insert at beginning

// Remove
numbers.Remove(3);           // Remove first occurrence
numbers.RemoveAt(0);         // Remove at index
numbers.RemoveAll(n => n > 3);  // Remove matching condition

// Check existence
bool contains = numbers.Contains(5);
bool any = numbers.Any(n => n > 10);

// Find
int found = numbers.Find(n => n > 3);          // First matching
int foundLast = numbers.FindLast(n => n > 3);  // Last matching
int index = numbers.FindIndex(n => n == 5);    // Index of match

// Clear
numbers.Clear();
```

### Performance Characteristics

```csharp
List<int> list = new List<int>();

// Add to end: O(1) amortized
list.Add(1);

// Insert at beginning: O(n) - shifts all elements
list.Insert(0, 0);

// Access by index: O(1)
int value = list[5];

// Remove by value: O(n) - searches then shifts
list.Remove(5);

// Remove by index: O(n) - shifts elements
list.RemoveAt(0);

// Contains: O(n) - linear search
bool exists = list.Contains(5);
```

**Capacity Management:**
```csharp
// List grows by doubling capacity
var list = new List<int>();  // Capacity: 0
list.Add(1);  // Capacity: 4
list.Add(2);  // Capacity: 4
list.Add(3);  // Capacity: 4
list.Add(4);  // Capacity: 4
list.Add(5);  // Capacity: 8 (doubled!)

// Pre-allocate for known size
var list2 = new List<int>(1000);  // Capacity: 1000
// Much faster if you know you'll add 1000 items

// Trim excess capacity
list.TrimExcess();  // Reduces capacity to count
```

## 🗂️ Dictionary\<TKey, TValue\>

### Common Operations

```csharp
// Create
var dict = new Dictionary<string, int>();

// Add
dict.Add("apple", 1);
dict.Add("banana", 2);

// Add or update (indexer)
dict["cherry"] = 3;
dict["apple"] = 10;  // Updates existing

// Initialize with values
var dict2 = new Dictionary<string, int>
{
    { "apple", 1 },
    { "banana", 2 },
    { "cherry", 3 }
};

// C# 9+ collection expressions
var dict3 = new Dictionary<string, int>
{
    ["apple"] = 1,
    ["banana"] = 2,
    ["cherry"] = 3
};

// Get value
int value = dict["apple"];  // Throws if key doesn't exist

// Safe get with TryGetValue
if (dict.TryGetValue("apple", out int appleValue))
{
    Console.WriteLine($"Apple: {appleValue}");
}

// Check existence
bool hasApple = dict.ContainsKey("apple");
bool hasValue = dict.ContainsValue(1);

// Remove
dict.Remove("apple");

// Iterate
foreach (var kvp in dict)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

// Keys and values
foreach (string key in dict.Keys)
{
    Console.WriteLine(key);
}

foreach (int value in dict.Values)
{
    Console.WriteLine(value);
}
```

### Performance & Use Cases

```csharp
// Dictionary: O(1) average for all operations
var dict = new Dictionary<string, int>();

dict.Add("key", 1);           // O(1)
int value = dict["key"];      // O(1)
dict.Remove("key");           // O(1)
bool exists = dict.ContainsKey("key");  // O(1)

// Use cases:
// ✅ Fast lookups by key
// ✅ Caching
// ✅ Counting occurrences
// ✅ Index building
```

**Common Pattern: Counting**
```csharp
string text = "hello world";
var charCount = new Dictionary<char, int>();

foreach (char c in text)
{
    if (charCount.ContainsKey(c))
    {
        charCount[c]++;
    }
    else
    {
        charCount[c] = 1;
    }
}

// Or with TryGetValue (more efficient)
foreach (char c in text)
{
    charCount[c] = charCount.TryGetValue(c, out int count) ? count + 1 : 1;
}

// Or with LINQ (most concise)
var charCount2 = text.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
```

## 🎯 LINQ Introduction

### What is LINQ?

**LINQ (Language Integrated Query):** A set of extension methods for querying and transforming collections.

**Key concept:** Declarative programming - specify *what* you want, not *how* to get it.

### Query Syntax vs Method Syntax

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Query syntax (SQL-like)
var evenNumbers = from n in numbers
                  where n % 2 == 0
                  select n;

// Method syntax (lambda expressions)
var evenNumbers2 = numbers.Where(n => n % 2 == 0);

// Both produce: [2, 4, 6, 8, 10]
```

**Which to use?**
- **Method syntax:** More common, more flexible, supports all operations
- **Query syntax:** More readable for complex queries with multiple from clauses

**This tutorial uses method syntax** (industry standard).

### Basic LINQ Operations

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Where - Filter
var evens = numbers.Where(n => n % 2 == 0);
// [2, 4, 6, 8, 10]

// Select - Transform
var doubled = numbers.Select(n => n * 2);
// [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]

// OrderBy - Sort
var sorted = numbers.OrderByDescending(n => n);
// [10, 9, 8, 7, 6, 5, 4, 3, 2, 1]

// Take - Limit
var firstThree = numbers.Take(3);
// [1, 2, 3]

// Skip - Skip elements
var skipTwo = numbers.Skip(2);
// [3, 4, 5, 6, 7, 8, 9, 10]

// Aggregate - Reduce
int sum = numbers.Sum();
double average = numbers.Average();
int max = numbers.Max();
int min = numbers.Min();
int count = numbers.Count();

// Any - Check existence
bool hasEvens = numbers.Any(n => n % 2 == 0);  // true

// All - Check all match
bool allPositive = numbers.All(n => n > 0);  // true

// First - Get first element
int first = numbers.First();  // 1
int firstEven = numbers.First(n => n % 2 == 0);  // 2

// FirstOrDefault - Safe get first
int? firstOver100 = numbers.FirstOrDefault(n => n > 100);  // 0 (default)
```

### Method Chaining

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Chain multiple operations
var result = numbers
    .Where(n => n % 2 == 0)       // Filter evens
    .Select(n => n * n)           // Square them
    .OrderByDescending(n => n)    // Sort descending
    .Take(3);                     // Take top 3

// [100, 64, 36]

// Equivalent to:
// 1. Filter: [2, 4, 6, 8, 10]
// 2. Square: [4, 16, 36, 64, 100]
// 3. Sort descending: [100, 64, 36, 16, 4]
// 4. Take 3: [100, 64, 36]
```

### Deferred Execution

**Key concept:** LINQ queries don't execute until you enumerate them.

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

// Query defined but NOT executed yet
var evenNumbers = numbers.Where(n => n % 2 == 0);

// Add more numbers
numbers.Add(6);
numbers.Add(7);
numbers.Add(8);

// NOW execute query by enumerating
foreach (int n in evenNumbers)  // Executes here!
{
    Console.WriteLine(n);  // 2, 4, 6, 8
}

// Query includes newly added numbers!
```

**Force immediate execution:**
```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

// ToList() forces execution NOW
var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();

// Add more numbers
numbers.Add(6);
numbers.Add(8);

// evenNumbers still [2, 4] - snapshot at execution time
```

## 🚀 Real-World Example

### Problem: Process Customer Orders

```csharp
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
}

var orders = new List<Order>
{
    new() { Id = 1, CustomerName = "Alice", Total = 100m, OrderDate = DateTime.Now.AddDays(-5), Status = "Completed" },
    new() { Id = 2, CustomerName = "Bob", Total = 250m, OrderDate = DateTime.Now.AddDays(-3), Status = "Completed" },
    new() { Id = 3, CustomerName = "Alice", Total = 150m, OrderDate = DateTime.Now.AddDays(-2), Status = "Pending" },
    new() { Id = 4, CustomerName = "Charlie", Total = 50m, OrderDate = DateTime.Now.AddDays(-1), Status = "Completed" },
};

// 1. Find completed orders over $100
var largeCompletedOrders = orders
    .Where(o => o.Status == "Completed")
    .Where(o => o.Total > 100)
    .ToList();

// 2. Total revenue from completed orders
decimal totalRevenue = orders
    .Where(o => o.Status == "Completed")
    .Sum(o => o.Total);

// 3. Top 3 customers by total spending
var topCustomers = orders
    .Where(o => o.Status == "Completed")
    .GroupBy(o => o.CustomerName)
    .Select(g => new
    {
        Customer = g.Key,
        TotalSpent = g.Sum(o => o.Total),
        OrderCount = g.Count()
    })
    .OrderByDescending(x => x.TotalSpent)
    .Take(3)
    .ToList();

// 4. Recent pending orders
var recentPending = orders
    .Where(o => o.Status == "Pending")
    .Where(o => o.OrderDate > DateTime.Now.AddDays(-7))
    .OrderBy(o => o.OrderDate)
    .Select(o => new { o.Id, o.CustomerName, o.Total })
    .ToList();
```

## ✅ Prerequisites Check

Before proceeding, ensure you have:

- [ ] Completed Modules 01-03
- [ ] Comfortable with generics (`List<T>`, `Dictionary<K,V>`)
- [ ] Understand lambda expressions
- [ ] Familiar with foreach loops

## ⏭️ Next Steps

Ready to master collections? Proceed to:
- **[Lesson 02: Arrays and Lists](02-arrays-and-lists.md)**

You'll learn about:
- Array performance optimization
- List internals and capacity management
- When to use arrays vs lists
- Collection initialization patterns
- Common algorithms (search, sort, filter)

## 📚 Additional Resources

- [LINQ Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- [Collections Documentation](https://learn.microsoft.com/en-us/dotnet/standard/collections/)
- [LINQ Samples](https://learn.microsoft.com/en-us/samples/dotnet/try-samples/101-linq-samples/)

---

*"LINQ is not just a feature; it's a way of thinking about data transformation." - Eric Lippert*
