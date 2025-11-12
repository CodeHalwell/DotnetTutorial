# Module 04: Collections & LINQ - Advanced LINQ

## 📘 Mastering Complex Query Patterns

Advanced LINQ techniques for real-world data manipulation, performance optimization, and complex querying scenarios.

## 🎯 Learning Objectives

- Master complex LINQ queries (joins, grouping, aggregation)
- Understand query performance and optimization
- Use deferred execution strategically
- Implement custom LINQ operators
- Apply functional programming patterns
- Handle real-world data processing scenarios

## 🔗 Advanced Join Operations

### Inner Join

```csharp
// Sample data
var customers = new[]
{
    new { Id = 1, Name = "Alice", City = "New York" },
    new { Id = 2, Name = "Bob", City = "London" },
    new { Id = 3, Name = "Charlie", City = "Tokyo" }
};

var orders = new[]
{
    new { Id = 101, CustomerId = 1, Product = "Laptop", Amount = 1200m },
    new { Id = 102, CustomerId = 1, Product = "Mouse", Amount = 25m },
    new { Id = 103, CustomerId = 2, Product = "Keyboard", Amount = 75m },
    new { Id = 104, CustomerId = 3, Product = "Monitor", Amount = 300m }
};

// Inner join - only customers with orders
var customerOrders = customers.Join(
    orders,                             // Inner collection
    customer => customer.Id,            // Outer key selector
    order => order.CustomerId,          // Inner key selector
    (customer, order) => new            // Result selector
    {
        customer.Name,
        order.Product,
        order.Amount
    });

foreach (var item in customerOrders)
{
    Console.WriteLine($"{item.Name} ordered {item.Product} for ${item.Amount}");
}

// Output:
// Alice ordered Laptop for $1200
// Alice ordered Mouse for $25
// Bob ordered Keyboard for $75
// Charlie ordered Monitor for $300
```

**When to use:**
- Need data from both collections
- Only interested in matching records
- Similar to SQL INNER JOIN

### Group Join (Left Outer Join)

```csharp
// Group join - all customers with their orders (if any)
var customerOrderGroups = customers.GroupJoin(
    orders,
    customer => customer.Id,
    order => order.CustomerId,
    (customer, orderGroup) => new
    {
        customer.Name,
        Orders = orderGroup.ToList(),
        TotalSpent = orderGroup.Sum(o => o.Amount)
    });

foreach (var customer in customerOrderGroups)
{
    Console.WriteLine($"{customer.Name}: {customer.Orders.Count} orders, ${customer.TotalSpent:F2} total");
    foreach (var order in customer.Orders)
    {
        Console.WriteLine($"  - {order.Product}: ${order.Amount}");
    }
}

// Output:
// Alice: 2 orders, $1225.00 total
//   - Laptop: $1200
//   - Mouse: $25
// Bob: 1 orders, $75.00 total
//   - Keyboard: $75
// Charlie: 1 orders, $300.00 total
//   - Monitor: $300
```

**When to use:**
- Need all records from left collection
- Related records grouped together
- Similar to SQL LEFT OUTER JOIN

### Multiple Joins

```csharp
var products = new[]
{
    new { Id = 1, Name = "Laptop", CategoryId = 1 },
    new { Id = 2, Name = "Mouse", CategoryId = 2 },
    new { Id = 3, Name = "Keyboard", CategoryId = 2 }
};

var categories = new[]
{
    new { Id = 1, Name = "Computers" },
    new { Id = 2, Name = "Accessories" }
};

var sales = new[]
{
    new { ProductId = 1, Quantity = 5, Date = DateTime.Now.AddDays(-1) },
    new { ProductId = 2, Quantity = 10, Date = DateTime.Now.AddDays(-2) },
    new { ProductId = 1, Quantity = 3, Date = DateTime.Now.AddDays(-3) }
};

// Join three collections
var productSales = products
    .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { Product = p, Category = c })
    .Join(sales, pc => pc.Product.Id, s => s.ProductId, (pc, s) => new
    {
        Product = pc.Product.Name,
        Category = pc.Category.Name,
        s.Quantity,
        s.Date
    });

foreach (var sale in productSales)
{
    Console.WriteLine($"{sale.Product} ({sale.Category}): {sale.Quantity} units on {sale.Date:yyyy-MM-dd}");
}
```

## 👥 Advanced Grouping

### Simple Grouping

```csharp
var people = new[]
{
    new { Name = "Alice", Age = 30, Department = "IT" },
    new { Name = "Bob", Age = 25, Department = "HR" },
    new { Name = "Charlie", Age = 35, Department = "IT" },
    new { Name = "Diana", Age = 28, Department = "HR" },
    new { Name = "Eve", Age = 32, Department = "IT" }
};

// Group by department
var byDepartment = people.GroupBy(p => p.Department);

foreach (var group in byDepartment)
{
    Console.WriteLine($"\n{group.Key} Department:");
    foreach (var person in group)
    {
        Console.WriteLine($"  {person.Name}, {person.Age}");
    }
}

// Output:
// IT Department:
//   Alice, 30
//   Charlie, 35
//   Eve, 32
// HR Department:
//   Bob, 25
//   Diana, 28
```

### Grouping with Aggregation

```csharp
var stats = people
    .GroupBy(p => p.Department)
    .Select(g => new
    {
        Department = g.Key,
        Count = g.Count(),
        AverageAge = g.Average(p => p.Age),
        MinAge = g.Min(p => p.Age),
        MaxAge = g.Max(p => p.Age),
        People = string.Join(", ", g.Select(p => p.Name))
    });

foreach (var stat in stats)
{
    Console.WriteLine($"{stat.Department}:");
    Console.WriteLine($"  Count: {stat.Count}");
    Console.WriteLine($"  Average Age: {stat.AverageAge:F1}");
    Console.WriteLine($"  Age Range: {stat.MinAge}-{stat.MaxAge}");
    Console.WriteLine($"  People: {stat.People}");
}
```

### Multi-Level Grouping

```csharp
var sales = new[]
{
    new { Product = "Laptop", Category = "Electronics", Region = "North", Amount = 1200m },
    new { Product = "Mouse", Category = "Electronics", Region = "North", Amount = 25m },
    new { Product = "Chair", Category = "Furniture", Region = "North", Amount = 150m },
    new { Product = "Laptop", Category = "Electronics", Region = "South", Amount = 1100m },
    new { Product = "Desk", Category = "Furniture", Region = "South", Amount = 300m }
};

// Group by region, then by category
var regionalSales = sales
    .GroupBy(s => s.Region)
    .Select(regionGroup => new
    {
        Region = regionGroup.Key,
        Categories = regionGroup
            .GroupBy(s => s.Category)
            .Select(categoryGroup => new
            {
                Category = categoryGroup.Key,
                Total = categoryGroup.Sum(s => s.Amount),
                Count = categoryGroup.Count()
            })
    });

foreach (var region in regionalSales)
{
    Console.WriteLine($"\n{region.Region} Region:");
    foreach (var category in region.Categories)
    {
        Console.WriteLine($"  {category.Category}: ${category.Total:F2} ({category.Count} items)");
    }
}
```

### Grouping with Custom Key

```csharp
// Group by multiple properties
var byAgeGroup = people.GroupBy(p => new
{
    Department = p.Department,
    AgeGroup = p.Age < 30 ? "Under 30" : "30 and over"
});

foreach (var group in byAgeGroup)
{
    Console.WriteLine($"{group.Key.Department} - {group.Key.AgeGroup}:");
    foreach (var person in group)
    {
        Console.WriteLine($"  {person.Name}, {person.Age}");
    }
}
```

## 🔄 Complex Transformations

### SelectMany for Flattening

```csharp
var companies = new[]
{
    new
    {
        Name = "TechCorp",
        Employees = new[] { "Alice", "Bob", "Charlie" }
    },
    new
    {
        Name = "FinanceInc",
        Employees = new[] { "Diana", "Eve" }
    }
};

// Flatten nested collections
var allEmployees = companies.SelectMany(c => c.Employees);
// ["Alice", "Bob", "Charlie", "Diana", "Eve"]

// With company information
var employeeDetails = companies.SelectMany(
    c => c.Employees,
    (company, employee) => new { Company = company.Name, Employee = employee }
);

foreach (var detail in employeeDetails)
{
    Console.WriteLine($"{detail.Employee} works at {detail.Company}");
}
```

**Real-world use case:**
```csharp
var orders = new[]
{
    new { OrderId = 1, Items = new[] { "Item A", "Item B" } },
    new { OrderId = 2, Items = new[] { "Item C" } },
    new { OrderId = 3, Items = new[] { "Item D", "Item E", "Item F" } }
};

// Get all items across all orders
var allItems = orders.SelectMany(o => o.Items);
// ["Item A", "Item B", "Item C", "Item D", "Item E", "Item F"]

// Count occurrences
var itemCounts = orders
    .SelectMany(o => o.Items)
    .GroupBy(item => item)
    .Select(g => new { Item = g.Key, Count = g.Count() });
```

### Zip - Combine Two Sequences

```csharp
var names = new[] { "Alice", "Bob", "Charlie" };
var ages = new[] { 30, 25, 35 };

var people = names.Zip(ages, (name, age) => new { Name = name, Age = age });

foreach (var person in people)
{
    Console.WriteLine($"{person.Name} is {person.Age} years old");
}

// Output:
// Alice is 30 years old
// Bob is 25 years old
// Charlie is 35 years old
```

**Note:** Zip stops at the shorter sequence length.

### Aggregate for Complex Calculations

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

// Calculate product (1 * 2 * 3 * 4 * 5)
int product = numbers.Aggregate((acc, n) => acc * n);
// Result: 120

// With seed value
int productWithSeed = numbers.Aggregate(1, (acc, n) => acc * n);
// Result: 120

// Build complex object
var sentence = new[] { "LINQ", "is", "powerful" };
string result = sentence.Aggregate(
    new { Length = 0, Words = new List<string>() },
    (acc, word) => new
    {
        Length = acc.Length + word.Length,
        Words = acc.Words.Concat(new[] { word }).ToList()
    },
    acc => $"{acc.Words.Count} words, {acc.Length} characters"
);
// Result: "3 words, 15 characters"
```

**Real-world: Build SQL query**
```csharp
var conditions = new[] { "Age > 18", "City = 'New York'", "Active = 1" };

string sqlWhere = conditions.Aggregate(
    "WHERE ",
    (query, condition) => query + condition + " AND ",
    query => query.TrimEnd(' ', 'A', 'N', 'D', ' ')
);
// Result: "WHERE Age > 18 AND City = 'New York' AND Active = 1"
```

## 🚀 Performance Optimization

### Deferred vs Immediate Execution

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };

// Deferred execution - query not executed yet
var query = numbers.Where(n => n % 2 == 0);
Console.WriteLine("Query defined");

// Modify source
numbers.Add(6);
numbers.Add(8);

// NOW executed when enumerated
foreach (int n in query)  // Executes here!
{
    Console.WriteLine(n);  // 2, 4, 6, 8 (includes newly added items)
}

// Immediate execution - executed immediately
var list = numbers.Where(n => n % 2 == 0).ToList();
Console.WriteLine("List created");

numbers.Add(10);  // Won't affect list
Console.WriteLine($"List count: {list.Count}");  // Still 4, doesn't include 10
```

**Methods that force immediate execution:**
- `ToList()`, `ToArray()`, `ToDictionary()`, `ToHashSet()`
- `Count()`, `Sum()`, `Average()`, `Min()`, `Max()`
- `First()`, `Last()`, `Single()`, `ElementAt()`
- `Any()`, `All()`, `Contains()`

### Avoiding Multiple Enumeration

```csharp
// ❌ BAD: Multiple enumeration
var query = GetExpensiveQuery();

int count = query.Count();           // Executes query #1
var first = query.FirstOrDefault(); // Executes query #2
var list = query.ToList();          // Executes query #3
// Query executed 3 times!

// ✅ GOOD: Execute once
var list = GetExpensiveQuery().ToList();

int count = list.Count;
var first = list.FirstOrDefault();
// Query executed only once
```

**When to materialize (ToList/ToArray):**
1. Need to enumerate multiple times
2. Modifying source collection
3. Passing to method that enumerates
4. Preventing re-execution of expensive operation

### Query Optimization

```csharp
var data = GetLargeDataset();  // 1 million records

// ❌ BAD: Multiple filters separately
var result1 = data
    .Where(x => x.Age > 18)
    .Where(x => x.City == "New York")
    .Where(x => x.IsActive);

// ✅ BETTER: Combined filter (single pass)
var result2 = data.Where(x => x.Age > 18 && x.City == "New York" && x.IsActive);

// ❌ BAD: OrderBy before filtering
var result3 = data
    .OrderBy(x => x.Name)     // Sorts 1 million records
    .Where(x => x.Age > 65);  // Then filters to 10,000

// ✅ GOOD: Filter before OrderBy
var result4 = data
    .Where(x => x.Age > 65)   // Filter to 10,000 first
    .OrderBy(x => x.Name);    // Then sort only 10,000

// Performance difference: ~100x faster
```

### Efficient Lookups

```csharp
var users = GetUsers();  // 100,000 users

// ❌ BAD: Linear search (O(n)) on every lookup
string FindUserName(int id)
{
    return users.FirstOrDefault(u => u.Id == id)?.Name;
}
// Each call iterates through users

// ✅ GOOD: Create dictionary once (O(1)) lookups
var userDict = users.ToDictionary(u => u.Id, u => u.Name);

string FindUserNameFast(int id)
{
    return userDict.TryGetValue(id, out string name) ? name : null;
}

// Benchmark (1000 lookups):
// FirstOrDefault approach: ~5000 ms
// Dictionary approach:     ~    0.5 ms
// 10,000x faster!
```

## 🎨 Custom LINQ Operators

### Extension Methods

```csharp
public static class LinqExtensions
{
    // WhereIf - conditional filtering
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    // Batch - split into batches
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source,
        int batchSize)
    {
        var batch = new List<T>(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }

    // DistinctBy - distinct by property
    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();

        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }
}

// Usage
var data = GetData();

// Conditional filtering
var result = data
    .WhereIf(includeInactive, x => x.IsActive)
    .WhereIf(!string.IsNullOrEmpty(searchTerm), x => x.Name.Contains(searchTerm));

// Batching
var batches = Enumerable.Range(1, 100).Batch(10);
foreach (var batch in batches)
{
    ProcessBatch(batch);  // Process 10 items at a time
}

// Distinct by property
var people = new[]
{
    new { Name = "Alice", City = "NYC" },
    new { Name = "Bob", City = "LA" },
    new { Name = "Charlie", City = "NYC" }
};

var uniqueCities = people.DistinctBy(p => p.City);
// Alice and Bob (unique cities)
```

### Memoization for Expensive Operations

```csharp
public static class MemoizedExtensions
{
    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
    {
        var cache = new Dictionary<T, TResult>();

        return arg =>
        {
            if (cache.TryGetValue(arg, out TResult result))
            {
                return result;
            }

            result = func(arg);
            cache[arg] = result;
            return result;
        };
    }
}

// Expensive calculation
Func<int, int> fibonacci = null;
fibonacci = n => n <= 1 ? n : fibonacci(n - 1) + fibonacci(n - 2);

// Without memoization: fib(40) takes ~2 seconds
// With memoization: fib(40) takes ~0.001 seconds

var memoizedFib = fibonacci.Memoize();
var result = memoizedFib(40);  // Fast!
```

## 🎯 Real-World Patterns

### Repository Query Pattern

```csharp
public interface IQueryOptions<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}

public class FilterBy<T> : IQueryOptions<T>
{
    private readonly Expression<Func<T, bool>> _predicate;

    public FilterBy(Expression<Func<T, bool>> predicate)
    {
        _predicate = predicate;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Where(_predicate);
    }
}

public class OrderBy<T, TKey> : IQueryOptions<T>
{
    private readonly Expression<Func<T, TKey>> _keySelector;
    private readonly bool _descending;

    public OrderBy(Expression<Func<T, TKey>> keySelector, bool descending = false)
    {
        _keySelector = keySelector;
        _descending = descending;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return _descending
            ? query.OrderByDescending(_keySelector)
            : query.OrderBy(_keySelector);
    }
}

// Usage
public class ProductRepository
{
    public IEnumerable<Product> GetProducts(params IQueryOptions<Product>[] options)
    {
        IQueryable<Product> query = _context.Products;

        foreach (var option in options)
        {
            query = option.Apply(query);
        }

        return query.ToList();
    }
}

// Clean, composable queries
var products = repository.GetProducts(
    new FilterBy<Product>(p => p.Price > 100),
    new FilterBy<Product>(p => p.InStock),
    new OrderBy<Product, decimal>(p => p.Price, descending: true)
);
```

### Specification Pattern

```csharp
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    Expression<Func<T, bool>> ToExpression();
}

public class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}

// Usage
public class ActiveProductSpec : ISpecification<Product>
{
    public bool IsSatisfiedBy(Product entity) => entity.IsActive;
    public Expression<Func<Product, bool>> ToExpression() => p => p.IsActive;
}

public class InStockSpec : ISpecification<Product>
{
    public bool IsSatisfiedBy(Product entity) => entity.Stock > 0;
    public Expression<Func<Product, bool>> ToExpression() => p => p.Stock > 0;
}

// Combine specifications
var activeAndInStock = new AndSpecification<Product>(
    new ActiveProductSpec(),
    new InStockSpec()
);

var products = _context.Products.Where(activeAndInStock.ToExpression()).ToList();
```

## ⚡ Performance Benchmarks

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class LinqBenchmarks
{
    private List<int> _numbers;

    [GlobalSetup]
    public void Setup()
    {
        _numbers = Enumerable.Range(1, 10000).ToList();
    }

    [Benchmark(Baseline = true)]
    public int ForLoop()
    {
        int sum = 0;
        for (int i = 0; i < _numbers.Count; i++)
        {
            if (_numbers[i] % 2 == 0)
            {
                sum += _numbers[i];
            }
        }
        return sum;
    }

    [Benchmark]
    public int LinqQuery()
    {
        return _numbers.Where(n => n % 2 == 0).Sum();
    }

    [Benchmark]
    public int LinqQueryOptimized()
    {
        return _numbers.Sum(n => n % 2 == 0 ? n : 0);
    }
}

// Results:
// | Method              | Mean      | Allocated |
// |-------------------- |----------:|----------:|
// | ForLoop             |  12.50 μs |       0 B |
// | LinqQuery           |  45.23 μs |     104 B |
// | LinqQueryOptimized  |  25.67 μs |      32 B |
```

## ✅ Best Practices

1. **Use deferred execution strategically**
   - Don't call `.ToList()` unless necessary
   - But do materialize if enumerating multiple times

2. **Filter early, sort late**
   - Apply `Where` before `OrderBy`
   - Reduces items to sort

3. **Avoid N+1 queries**
   - Use `Join` or `GroupJoin` instead of nested queries
   - With EF Core, use `.Include()` for eager loading

4. **Choose appropriate methods**
   - Use `Any()` instead of `Count() > 0`
   - Use `FirstOrDefault()` instead of `Where().First()`

5. **Consider memory**
   - `ToList()` loads everything into memory
   - Use `yield return` for large datasets

6. **Use compiled queries for hot paths**
   ```csharp
   var compiledQuery = EF.CompileQuery(
       (MyDbContext ctx, int id) => ctx.Users.Where(u => u.Id == id)
   );
   ```

## 📚 Additional Resources

- [LINQ Performance](https://learn.microsoft.com/en-us/dotnet/standard/linq/write-performant-linq-queries)
- [Expression Trees](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/expression-trees/)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)

## ⏭️ Next Lesson

Proceed to **[Lesson 03: IEnumerable vs IQueryable](03-ienumerable-vs-iqueryable.md)** to learn about:
- Differences between IEnumerable and IQueryable
- When to use each
- Expression trees
- Database query translation
- Performance implications

---

*"LINQ is like a Swiss Army knife for collections - powerful, versatile, and essential." - Anonymous Developer*
