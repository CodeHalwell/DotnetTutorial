# LINQ Cheatsheet

## 📋 Quick Reference for Language Integrated Query

Complete reference for LINQ methods with examples and use cases.

## 🔍 Filtering

### Where
Filter elements based on condition

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6 };

// Even numbers
var evens = numbers.Where(n => n % 2 == 0);
// [2, 4, 6]

// Multiple conditions
var result = numbers.Where(n => n > 2 && n < 6);
// [3, 4, 5]

// With index
var result2 = numbers.Where((n, index) => index % 2 == 0);
// [1, 3, 5] (elements at even indices)
```

### OfType
Filter by type

```csharp
object[] mixed = { 1, "hello", 2, "world", 3.14 };

var strings = mixed.OfType<string>();
// ["hello", "world"]

var integers = mixed.OfType<int>();
// [1, 2]
```

## 🔄 Projection

### Select
Transform each element

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Square each number
var squared = numbers.Select(n => n * n);
// [1, 4, 9, 16, 25]

// Create anonymous objects
var objects = numbers.Select(n => new { Number = n, Square = n * n });

// With index
var indexed = numbers.Select((n, index) => $"[{index}] = {n}");
// ["[0] = 1", "[1] = 2", ...]
```

### SelectMany
Flatten nested collections

```csharp
var orders = new[]
{
    new { Customer = "Alice", Items = new[] { "Apple", "Banana" } },
    new { Customer = "Bob", Items = new[] { "Cherry", "Date" } }
};

// Flatten all items
var allItems = orders.SelectMany(o => o.Items);
// ["Apple", "Banana", "Cherry", "Date"]

// With transformation
var result = orders.SelectMany(
    o => o.Items,
    (order, item) => $"{order.Customer}: {item}");
// ["Alice: Apple", "Alice: Banana", "Bob: Cherry", "Bob: Date"]
```

## 📊 Ordering

### OrderBy / OrderByDescending
Sort elements

```csharp
var numbers = new[] { 5, 2, 8, 1, 9 };

var ascending = numbers.OrderBy(n => n);
// [1, 2, 5, 8, 9]

var descending = numbers.OrderByDescending(n => n);
// [9, 8, 5, 2, 1]

// Order by property
var people = new[]
{
    new { Name = "Alice", Age = 30 },
    new { Name = "Bob", Age = 25 }
};

var byAge = people.OrderBy(p => p.Age);
```

### ThenBy / ThenByDescending
Secondary sorting

```csharp
var people = new[]
{
    new { Name = "Alice", Age = 30 },
    new { Name = "Bob", Age = 25 },
    new { Name = "Charlie", Age = 30 }
};

var sorted = people
    .OrderBy(p => p.Age)        // Primary: by age
    .ThenBy(p => p.Name);       // Secondary: by name
// Bob (25), Alice (30), Charlie (30)
```

### Reverse
Reverse order

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

var reversed = numbers.Reverse();
// [5, 4, 3, 2, 1]
```

## 📦 Partitioning

### Take / Skip
Take or skip elements

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

var first3 = numbers.Take(3);
// [1, 2, 3]

var skip2 = numbers.Skip(2);
// [3, 4, 5, 6, 7, 8, 9, 10]

// Pagination
int page = 2;
int pageSize = 3;
var pageData = numbers.Skip((page - 1) * pageSize).Take(pageSize);
// [4, 5, 6]
```

### TakeWhile / SkipWhile
Take/skip while condition is true

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 1, 2, 3 };

var takeWhile = numbers.TakeWhile(n => n < 4);
// [1, 2, 3] (stops at first 4)

var skipWhile = numbers.SkipWhile(n => n < 4);
// [4, 5, 1, 2, 3] (skips until condition false)
```

### Chunk (C# 6+)
Split into chunks

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

var chunks = numbers.Chunk(3);
// [[1, 2, 3], [4, 5, 6], [7, 8, 9]]
```

## 🎯 Element Operations

### First / FirstOrDefault
Get first element

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

var first = numbers.First();
// 1

var firstEven = numbers.First(n => n % 2 == 0);
// 2

// Safe version (returns default if not found)
var firstOver10 = numbers.FirstOrDefault(n => n > 10);
// 0 (default for int)

var numbers2 = new int[] { };
// var first = numbers2.First();  // ❌ Throws exception
var first = numbers2.FirstOrDefault();  // ✅ Returns 0
```

### Last / LastOrDefault
Get last element

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

var last = numbers.Last();
// 5

var lastEven = numbers.Last(n => n % 2 == 0);
// 4
```

### Single / SingleOrDefault
Get single element (throws if 0 or >1)

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

var single = numbers.Single(n => n == 3);
// 3

// var single2 = numbers.Single(n => n > 3);  // ❌ Throws (multiple matches)
// var single3 = numbers.Single(n => n > 10); // ❌ Throws (no matches)

var singleOrDefault = numbers.SingleOrDefault(n => n > 10);
// 0 (default)
```

### ElementAt / ElementAtOrDefault
Get element at index

```csharp
var numbers = new[] { 10, 20, 30, 40, 50 };

var third = numbers.ElementAt(2);
// 30

var tenth = numbers.ElementAtOrDefault(10);
// 0 (out of range)
```

## ✅ Quantifiers

### Any
Check if any element matches

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

bool hasEvens = numbers.Any(n => n % 2 == 0);
// true

bool hasAny = numbers.Any();
// true (has elements)

var empty = new int[] { };
bool emptyHasAny = empty.Any();
// false
```

### All
Check if all elements match

```csharp
var numbers = new[] { 2, 4, 6, 8, 10 };

bool allEven = numbers.All(n => n % 2 == 0);
// true

bool allPositive = numbers.All(n => n > 0);
// true
```

### Contains
Check if contains element

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

bool hasThree = numbers.Contains(3);
// true

bool hasTen = numbers.Contains(10);
// false
```

## 🔢 Aggregation

### Count
Count elements

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

int count = numbers.Count();
// 5

int evenCount = numbers.Count(n => n % 2 == 0);
// 2
```

### Sum / Average / Min / Max
Numeric operations

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

int sum = numbers.Sum();
// 15

double average = numbers.Average();
// 3.0

int min = numbers.Min();
// 1

int max = numbers.Max();
// 5

// With selector
var people = new[]
{
    new { Name = "Alice", Age = 30 },
    new { Name = "Bob", Age = 25 }
};

double avgAge = people.Average(p => p.Age);
// 27.5
```

### Aggregate
Custom aggregation

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Product of all numbers
int product = numbers.Aggregate((acc, n) => acc * n);
// 120 (1*2*3*4*5)

// With seed value
int productWithSeed = numbers.Aggregate(1, (acc, n) => acc * n);
// 120

// With result selector
string result = numbers.Aggregate(
    0,                              // seed
    (acc, n) => acc + n,           // accumulator
    acc => $"Sum is {acc}");       // result selector
// "Sum is 15"
```

## 🔗 Join

### Join
Inner join two sequences

```csharp
var customers = new[]
{
    new { Id = 1, Name = "Alice" },
    new { Id = 2, Name = "Bob" }
};

var orders = new[]
{
    new { CustomerId = 1, Product = "Apple" },
    new { CustomerId = 1, Product = "Banana" },
    new { CustomerId = 2, Product = "Cherry" }
};

var result = customers.Join(
    orders,
    c => c.Id,
    o => o.CustomerId,
    (c, o) => new { c.Name, o.Product });
// [
//   { Name = "Alice", Product = "Apple" },
//   { Name = "Alice", Product = "Banana" },
//   { Name = "Bob", Product = "Cherry" }
// ]
```

### GroupJoin
Left outer join

```csharp
var result = customers.GroupJoin(
    orders,
    c => c.Id,
    o => o.CustomerId,
    (c, orderGroup) => new
    {
        c.Name,
        Orders = orderGroup.Select(o => o.Product)
    });
```

## 👥 Grouping

### GroupBy
Group elements

```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6 };

var grouped = numbers.GroupBy(n => n % 2 == 0 ? "Even" : "Odd");
// [
//   { Key = "Odd", Values = [1, 3, 5] },
//   { Key = "Even", Values = [2, 4, 6] }
// ]

foreach (var group in grouped)
{
    Console.WriteLine($"{group.Key}: {string.Join(", ", group)}");
}
// Odd: 1, 3, 5
// Even: 2, 4, 6

// With element selector
var people = new[]
{
    new { Name = "Alice", Department = "IT" },
    new { Name = "Bob", Department = "HR" },
    new { Name = "Charlie", Department = "IT" }
};

var byDept = people.GroupBy(
    p => p.Department,
    p => p.Name);
// [
//   { Key = "IT", Values = ["Alice", "Charlie"] },
//   { Key = "HR", Values = ["Bob"] }
// ]
```

## 🎭 Set Operations

### Distinct
Remove duplicates

```csharp
var numbers = new[] { 1, 2, 2, 3, 3, 3, 4, 5, 5 };

var distinct = numbers.Distinct();
// [1, 2, 3, 4, 5]
```

### Union
Combine sequences (distinct)

```csharp
var set1 = new[] { 1, 2, 3 };
var set2 = new[] { 3, 4, 5 };

var union = set1.Union(set2);
// [1, 2, 3, 4, 5]
```

### Intersect
Common elements

```csharp
var set1 = new[] { 1, 2, 3, 4 };
var set2 = new[] { 3, 4, 5, 6 };

var common = set1.Intersect(set2);
// [3, 4]
```

### Except
Difference (in first, not in second)

```csharp
var set1 = new[] { 1, 2, 3, 4 };
var set2 = new[] { 3, 4, 5, 6 };

var difference = set1.Except(set2);
// [1, 2]
```

## 🔄 Conversion

### ToList / ToArray
Convert to collection

```csharp
var query = Enumerable.Range(1, 5).Where(n => n % 2 == 0);

var list = query.ToList();
// List<int> [2, 4]

var array = query.ToArray();
// int[] [2, 4]
```

### ToDictionary
Convert to dictionary

```csharp
var people = new[]
{
    new { Id = 1, Name = "Alice" },
    new { Id = 2, Name = "Bob" }
};

var dict = people.ToDictionary(p => p.Id, p => p.Name);
// { 1: "Alice", 2: "Bob" }
```

### ToHashSet (C# 7.3+)
Convert to hash set

```csharp
var numbers = new[] { 1, 2, 2, 3, 3, 3 };

var hashSet = numbers.ToHashSet();
// HashSet<int> { 1, 2, 3 }
```

### ToLookup
Create lookup (one key, multiple values)

```csharp
var people = new[]
{
    new { Name = "Alice", Department = "IT" },
    new { Name = "Bob", Department = "HR" },
    new { Name = "Charlie", Department = "IT" }
};

var lookup = people.ToLookup(p => p.Department);

foreach (var name in lookup["IT"])
{
    Console.WriteLine(name.Name);
}
// Alice
// Charlie
```

## 🆕 Generation

### Range
Generate sequence of numbers

```csharp
var numbers = Enumerable.Range(1, 10);
// [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]

var evens = Enumerable.Range(0, 50).Where(n => n % 2 == 0);
// [0, 2, 4, 6, ..., 98]
```

### Repeat
Repeat element

```csharp
var repeated = Enumerable.Repeat("Hello", 3);
// ["Hello", "Hello", "Hello"]

var zeros = Enumerable.Repeat(0, 100).ToList();
// List with 100 zeros
```

### Empty
Create empty sequence

```csharp
var empty = Enumerable.Empty<int>();
// []
```

## 🔗 Concatenation

### Concat
Concatenate sequences

```csharp
var first = new[] { 1, 2, 3 };
var second = new[] { 4, 5, 6 };

var combined = first.Concat(second);
// [1, 2, 3, 4, 5, 6]
```

### Append / Prepend
Add element

```csharp
var numbers = new[] { 2, 3, 4 };

var withEnd = numbers.Append(5);
// [2, 3, 4, 5]

var withStart = numbers.Prepend(1);
// [1, 2, 3, 4]
```

## 💡 Advanced Patterns

### Method Chaining

```csharp
var result = Enumerable.Range(1, 100)
    .Where(n => n % 2 == 0)           // Evens
    .Select(n => n * n)               // Square
    .Where(n => n < 1000)             // Less than 1000
    .OrderByDescending(n => n)        // Sort desc
    .Take(5)                          // Top 5
    .ToList();
```

### Complex Grouping

```csharp
var orders = GetOrders();

var summary = orders
    .GroupBy(o => o.CustomerId)
    .Select(g => new
    {
        CustomerId = g.Key,
        TotalOrders = g.Count(),
        TotalAmount = g.Sum(o => o.Amount),
        AverageAmount = g.Average(o => o.Amount),
        LatestOrder = g.Max(o => o.OrderDate)
    })
    .OrderByDescending(x => x.TotalAmount)
    .ToList();
```

### Pagination

```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public PagedResult<T> GetPage<T>(IQueryable<T> query, int page, int pageSize)
{
    var totalCount = query.Count();
    var items = query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return new PagedResult<T>
    {
        Items = items,
        TotalCount = totalCount,
        PageNumber = page,
        PageSize = pageSize
    };
}
```

## 🚨 Common Pitfalls

### 1. Multiple Enumeration

```csharp
// ❌ BAD: Query executed twice
var query = numbers.Where(n => ExpensiveOperation(n));
int count = query.Count();        // Executes query
var list = query.ToList();        // Executes query AGAIN

// ✅ GOOD: Execute once
var list = numbers.Where(n => ExpensiveOperation(n)).ToList();
int count = list.Count;
```

### 2. Deferred vs Immediate Execution

```csharp
// Deferred (not executed until enumerated)
var query = numbers.Where(n => n > 5);

// Immediate (executed now)
var list = numbers.Where(n => n > 5).ToList();
var array = numbers.Where(n => n > 5).ToArray();
var count = numbers.Count(n => n > 5);
```

### 3. Modifying Source During Enumeration

```csharp
// ❌ BAD: Modifying collection during foreach
var list = new List<int> { 1, 2, 3, 4, 5 };
foreach (var n in list)
{
    list.Remove(n);  // ❌ InvalidOperationException
}

// ✅ GOOD: ToList() creates copy
foreach (var n in list.ToList())
{
    list.Remove(n);
}

// ✅ BETTER: Use RemoveAll
list.RemoveAll(n => true);  // Remove all
```

---

**Pro Tip:** Use LINQPad for testing LINQ queries interactively!

**Next:** [ASP.NET Core Cheatsheet](aspnetcore-cheatsheet.md)
