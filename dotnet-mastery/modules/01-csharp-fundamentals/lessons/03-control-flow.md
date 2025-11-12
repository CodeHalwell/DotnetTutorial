# Module 01: C# Fundamentals - Control Flow

## 📘 Controlling Program Execution

Control flow determines the order in which code executes. Master these structures to write logic that responds to conditions, repeats operations, and handles complex decision-making.

## 🎯 Learning Objectives

- Master conditional statements (if, else, switch)
- Use pattern matching for powerful type checking
- Implement loops efficiently (for, foreach, while)
- Understand jump statements (break, continue, return, goto)
- Write clean, readable control flow code

## 🔀 Conditional Statements

### If-Else Statements

```csharp
int age = 25;

if (age >= 18)
{
    Console.WriteLine("Adult");
}

// With else
if (age >= 18)
{
    Console.WriteLine("Adult");
}
else
{
    Console.WriteLine("Minor");
}

// Multiple conditions
if (age < 13)
{
    Console.WriteLine("Child");
}
else if (age < 18)
{
    Console.WriteLine("Teenager");
}
else if (age < 65)
{
    Console.WriteLine("Adult");
}
else
{
    Console.WriteLine("Senior");
}

// Logical operators
bool hasLicense = true;
bool hasInsurance = true;

if (age >= 16 && hasLicense && hasInsurance)
{
    Console.WriteLine("Can drive");
}

// Short-circuiting: && and || stop evaluating when result is determined
if (hasLicense && CheckInsurance())  // CheckInsurance() NOT called if hasLicense is false
{
    Console.WriteLine("Authorized");
}

// Null-conditional with if
string? name = GetName();
if (name != null && name.Length > 0)  // Safe: null check first
{
    Console.WriteLine(name.ToUpper());
}
```

**Best Practices:**
```csharp
// ❌ BAD: Too deeply nested (hard to read)
if (condition1)
{
    if (condition2)
    {
        if (condition3)
        {
            if (condition4)
            {
                // Do something
            }
        }
    }
}

// ✅ GOOD: Early returns (guard clauses)
if (!condition1)
{
    return;
}

if (!condition2)
{
    return;
}

if (!condition3)
{
    return;
}

if (!condition4)
{
    return;
}

// Do something
```

### Ternary Operator

```csharp
// Syntax: condition ? trueValue : falseValue
int age = 20;
string category = age >= 18 ? "Adult" : "Minor";

// Equivalent to:
string category;
if (age >= 18)
{
    category = "Adult";
}
else
{
    category = "Minor";
}

// Nested ternary (use sparingly - can be hard to read)
string category = age < 13 ? "Child" :
                  age < 18 ? "Teen" :
                  age < 65 ? "Adult" : "Senior";

// When to use:
// ✅ Simple, single-line assignments
// ❌ Complex logic (use if-else instead)
```

### Switch Statements (Traditional)

```csharp
int dayOfWeek = 3;

switch (dayOfWeek)
{
    case 1:
        Console.WriteLine("Monday");
        break;  // Required (or return, throw)
    case 2:
        Console.WriteLine("Tuesday");
        break;
    case 3:
        Console.WriteLine("Wednesday");
        break;
    case 4:
        Console.WriteLine("Thursday");
        break;
    case 5:
        Console.WriteLine("Friday");
        break;
    case 6:
    case 7:  // Fall-through for multiple cases
        Console.WriteLine("Weekend");
        break;
    default:
        Console.WriteLine("Invalid day");
        break;
}

// With when clause (C# 7+)
int number = 42;

switch (number)
{
    case int n when n < 0:
        Console.WriteLine("Negative");
        break;
    case int n when n == 0:
        Console.WriteLine("Zero");
        break;
    case int n when n > 0 && n <= 100:
        Console.WriteLine("Positive (1-100)");
        break;
    default:
        Console.WriteLine("Positive (> 100)");
        break;
}
```

### Switch Expressions (C# 8+)

**More concise, functional style:**

```csharp
// Traditional switch statement
string day;
switch (dayNumber)
{
    case 1: day = "Monday"; break;
    case 2: day = "Tuesday"; break;
    case 3: day = "Wednesday"; break;
    case 4: day = "Thursday"; break;
    case 5: day = "Friday"; break;
    case 6: day = "Saturday"; break;
    case 7: day = "Sunday"; break;
    default: day = "Invalid"; break;
}

// Switch expression (C# 8+)
string day = dayNumber switch
{
    1 => "Monday",
    2 => "Tuesday",
    3 => "Wednesday",
    4 => "Thursday",
    5 => "Friday",
    6 => "Saturday",
    7 => "Sunday",
    _ => "Invalid"  // Discard pattern (default)
};

// With complex conditions
decimal shipping = orderTotal switch
{
    <= 0 => throw new ArgumentException("Invalid total"),
    < 25 => 5.99m,
    < 50 => 3.99m,
    < 100 => 1.99m,
    _ => 0m  // Free shipping for $100+
};

// Multiple conditions
string classification = (age, income) switch
{
    (< 18, _) => "Minor",
    (>= 18, < 30000) => "Young Adult - Low Income",
    (>= 18, < 60000) => "Young Adult - Middle Income",
    (>= 18, _) => "Young Adult - High Income",
    _ => "Unknown"
};
```

### Pattern Matching

**Type Patterns:**
```csharp
object obj = "Hello";

// Type pattern with 'is'
if (obj is string text)
{
    Console.WriteLine($"String: {text.ToUpper()}");
}

// Type pattern in switch
string description = obj switch
{
    string s => $"String of length {s.Length}",
    int i => $"Integer: {i}",
    double d => $"Double: {d:F2}",
    Person p => $"Person: {p.Name}",
    null => "Null",
    _ => "Unknown type"
};
```

**Property Patterns (C# 8+):**
```csharp
public record Person(string Name, int Age, string City);

Person person = new("John", 30, "New York");

// Property pattern
string message = person switch
{
    { Age: < 18 } => "Minor",
    { Age: >= 18, Age: < 65 } => "Adult",
    { Age: >= 65 } => "Senior",
    _ => "Unknown"
};

// Multiple properties
decimal discount = person switch
{
    { Age: < 18 } => 0.50m,  // 50% off for minors
    { Age: >= 65 } => 0.30m,  // 30% off for seniors
    { City: "New York" } => 0.10m,  // 10% off for NY residents
    _ => 0m
};

// Nested properties
string description = person switch
{
    { Age: < 18, City: "New York" } => "Young New Yorker",
    { Age: >= 65, City: "New York" } => "Senior New Yorker",
    { Age: >= 18 and < 65 } => "Adult",  // 'and' combinator
    _ => "Person"
};
```

**List Patterns (C# 11+):**
```csharp
int[] numbers = { 1, 2, 3, 4, 5 };

// List pattern
string description = numbers switch
{
    [] => "Empty",
    [1] => "Single element: 1",
    [1, 2] => "Two elements: 1 and 2",
    [1, 2, ..] => "Starts with 1, 2",  // .. = any number of elements
    [.., 5] => "Ends with 5",
    [var first, .., var last] => $"First: {first}, Last: {last}",
    _ => "Other"
};

// Output: "Starts with 1, 2"
```

## 🔁 Loops

### For Loop

```csharp
// Basic for loop
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}

// Breakdown:
// 1. int i = 0         - Initialization (runs once)
// 2. i < 10            - Condition (checked before each iteration)
// 3. Console.WriteLine - Body (executes if condition is true)
// 4. i++               - Increment (runs after each iteration)

// Multiple variables
for (int i = 0, j = 10; i < j; i++, j--)
{
    Console.WriteLine($"i={i}, j={j}");
}

// Infinite loop
for (;;)  // Or: for(; true; )
{
    Console.WriteLine("Press Ctrl+C to stop");
    Thread.Sleep(1000);
}

// Reverse iteration
for (int i = 10; i >= 0; i--)
{
    Console.WriteLine(i);
}
Console.WriteLine("Liftoff!");

// Step by 2
for (int i = 0; i < 10; i += 2)
{
    Console.WriteLine(i);  // 0, 2, 4, 6, 8
}
```

**Performance Consideration:**
```csharp
// ❌ BAD: Length calculated on every iteration
for (int i = 0; i < GetExpensiveLength(); i++)
{
    // Process
}

// ✅ GOOD: Cache length
int length = GetExpensiveLength();
for (int i = 0; i < length; i++)
{
    // Process
}

// For arrays, JIT optimizer often handles this
int[] array = new int[1000];
for (int i = 0; i < array.Length; i++)  // Safe: JIT optimizes
{
    // Process array[i]
}
```

### Foreach Loop

```csharp
string[] names = { "Alice", "Bob", "Charlie" };

// Foreach: Iterate over collection
foreach (string name in names)
{
    Console.WriteLine(name);
}

// Equivalent for loop:
for (int i = 0; i < names.Length; i++)
{
    Console.WriteLine(names[i]);
}

// With index using tuple deconstruction (C# 9+)
foreach ((string name, int index) in names.Select((n, i) => (n, i)))
{
    Console.WriteLine($"{index}: {name}");
}

// Works with any IEnumerable<T>
List<int> numbers = new() { 1, 2, 3, 4, 5 };
foreach (int num in numbers)
{
    Console.WriteLine(num);
}

// Read-only: Cannot modify collection during iteration
List<int> numbers = new() { 1, 2, 3 };
foreach (int num in numbers)
{
    // numbers.Add(4);  // ❌ Throws InvalidOperationException
    // numbers.Remove(num);  // ❌ Throws InvalidOperationException
}
```

**When to Use Foreach vs For:**

```csharp
// ✅ Use foreach when:
// - You need all elements
// - You don't need the index
// - You're not modifying the collection

foreach (var item in collection)
{
    Process(item);
}

// ✅ Use for when:
// - You need the index
// - You're modifying the collection
// - You need to iterate backwards
// - You need to skip elements

for (int i = 0; i < collection.Count; i++)
{
    if (i % 2 == 0)  // Process only even indices
    {
        Process(collection[i]);
    }
}
```

### While Loop

```csharp
// While: Check condition before executing body
int count = 0;
while (count < 5)
{
    Console.WriteLine(count);
    count++;
}

// May never execute if condition is initially false
int count = 10;
while (count < 5)
{
    Console.WriteLine("This never prints");
}

// Common pattern: Read until end
string? line;
while ((line = Console.ReadLine()) != null)
{
    Console.WriteLine($"You entered: {line}");
    if (line == "exit")
    {
        break;
    }
}
```

### Do-While Loop

```csharp
// Do-While: Execute body at least once, then check condition
int count = 0;
do
{
    Console.WriteLine(count);
    count++;
} while (count < 5);

// Always executes at least once
int count = 10;
do
{
    Console.WriteLine("This prints once");  // Executes!
} while (count < 5);

// Use case: Input validation
int age;
do
{
    Console.Write("Enter your age (1-120): ");
    int.TryParse(Console.ReadLine(), out age);
} while (age < 1 || age > 120);

Console.WriteLine($"Valid age: {age}");
```

## 🚀 Jump Statements

### Break

```csharp
// Exit loop immediately
for (int i = 0; i < 10; i++)
{
    if (i == 5)
    {
        break;  // Exit loop when i is 5
    }
    Console.WriteLine(i);  // Prints: 0, 1, 2, 3, 4
}

// Search pattern
int[] numbers = { 10, 20, 30, 40, 50 };
int searchValue = 30;
int foundIndex = -1;

for (int i = 0; i < numbers.Length; i++)
{
    if (numbers[i] == searchValue)
    {
        foundIndex = i;
        break;  // Stop searching when found
    }
}

Console.WriteLine(foundIndex >= 0
    ? $"Found at index {foundIndex}"
    : "Not found");
```

### Continue

```csharp
// Skip current iteration, continue with next
for (int i = 0; i < 10; i++)
{
    if (i % 2 == 0)
    {
        continue;  // Skip even numbers
    }
    Console.WriteLine(i);  // Prints only odd: 1, 3, 5, 7, 9
}

// Filter and process
string[] names = { "Alice", "Bob", "", "Charlie", null, "Dave" };
foreach (string name in names)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        continue;  // Skip empty/null names
    }
    Console.WriteLine(name.ToUpper());
}
```

### Return

```csharp
// Exit method immediately
public int FindFirstNegative(int[] numbers)
{
    for (int i = 0; i < numbers.Length; i++)
    {
        if (numbers[i] < 0)
        {
            return i;  // Exit method with result
        }
    }
    return -1;  // Not found
}

// Early return pattern (guard clauses)
public void ProcessOrder(Order order)
{
    if (order == null)
    {
        return;  // Exit early
    }

    if (order.Items.Count == 0)
    {
        return;  // Exit early
    }

    // Main logic only if validations pass
    CalculateTotal(order);
    SaveOrder(order);
}
```

### Goto (Rarely Used)

```csharp
// ⚠️ Generally considered bad practice, but has rare valid uses

// Example: Breaking out of nested loops
bool found = false;
for (int i = 0; i < 10; i++)
{
    for (int j = 0; j < 10; j++)
    {
        if (i * j == 42)
        {
            goto Found;  // Jump to label
        }
    }
}

Found:
Console.WriteLine("Found or finished");

// Better alternative: Extract to method and use return
bool TryFind(out int i, out int j)
{
    for (i = 0; i < 10; i++)
    {
        for (j = 0; j < 10; j++)
        {
            if (i * j == 42)
            {
                return true;
            }
        }
    }
    return false;
}
```

## 🎯 Real-World Examples

### Example 1: Input Validation

```csharp
public static int GetValidAge()
{
    while (true)
    {
        Console.Write("Enter your age: ");
        string input = Console.ReadLine() ?? "";

        // Validate input
        if (!int.TryParse(input, out int age))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        if (age < 0 || age > 150)
        {
            Console.WriteLine("Age must be between 0 and 150.");
            continue;
        }

        // Valid input
        return age;
    }
}
```

### Example 2: Processing Business Rules

```csharp
public decimal CalculateDiscount(Customer customer, Order order)
{
    // Guard clauses
    if (customer == null || order == null)
    {
        return 0m;
    }

    if (order.Total <= 0)
    {
        return 0m;
    }

    // Business rules with pattern matching
    decimal discount = (customer.Type, order.Total) switch
    {
        (CustomerType.VIP, > 1000) => 0.25m,      // 25% off for VIP on orders > $1000
        (CustomerType.VIP, _) => 0.15m,           // 15% off for VIP
        (CustomerType.Premium, > 500) => 0.15m,   // 15% off for Premium on orders > $500
        (CustomerType.Premium, _) => 0.10m,       // 10% off for Premium
        (_, > 1000) => 0.10m,                     // 10% off for all on orders > $1000
        (_, > 500) => 0.05m,                      // 5% off for all on orders > $500
        _ => 0m
    };

    return discount;
}
```

### Example 3: State Machine

```csharp
public enum State { Start, Reading, Processing, Complete, Error }

public void ProcessData(string[] data)
{
    State currentState = State.Start;
    int lineNumber = 0;

    while (currentState != State.Complete && currentState != State.Error)
    {
        switch (currentState)
        {
            case State.Start:
                Console.WriteLine("Starting...");
                currentState = State.Reading;
                break;

            case State.Reading:
                if (lineNumber >= data.Length)
                {
                    currentState = State.Complete;
                    break;
                }

                string line = data[lineNumber++];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;  // Skip empty lines
                }

                currentState = State.Processing;
                break;

            case State.Processing:
                try
                {
                    // Process line
                    ProcessLine(data[lineNumber - 1]);
                    currentState = State.Reading;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    currentState = State.Error;
                }
                break;
        }
    }

    Console.WriteLine($"Final state: {currentState}");
}
```

## ✅ Best Practices

1. **Use guard clauses**: Return early for invalid cases
2. **Avoid deep nesting**: Max 3 levels deep
3. **Prefer switch expressions**: More concise than switch statements
4. **Use pattern matching**: More readable than type checking + casting
5. **Cache loop conditions**: Don't recalculate expensive expressions
6. **Use foreach for collections**: Cleaner than manual indexing
7. **Avoid goto**: Almost always a better alternative
8. **Use meaningful variable names**: `i` is fine for simple loops, otherwise use descriptive names

## 🚫 Common Pitfalls

```csharp
// ❌ Off-by-one errors
for (int i = 0; i <= array.Length; i++)  // Goes beyond array bounds!
{
    Console.WriteLine(array[i]);  // IndexOutOfRangeException when i == Length
}

// ✅ Correct
for (int i = 0; i < array.Length; i++)
{
    Console.WriteLine(array[i]);
}

// ❌ Infinite loops
int count = 0;
while (count < 10)
{
    Console.WriteLine(count);
    // Forgot to increment!  Infinite loop
}

// ❌ Modifying collection during iteration
List<int> numbers = new() { 1, 2, 3, 4, 5 };
foreach (int num in numbers)
{
    if (num % 2 == 0)
    {
        numbers.Remove(num);  // ❌ InvalidOperationException
    }
}

// ✅ Use for loop when modifying
for (int i = numbers.Count - 1; i >= 0; i--)  // Iterate backwards
{
    if (numbers[i] % 2 == 0)
    {
        numbers.RemoveAt(i);
    }
}
```

## ⏭️ Next Lesson

Proceed to **[Lesson 04: Methods and Parameters](04-methods-and-parameters.md)** to learn about:
- Method declaration and invocation
- Parameters (value, ref, out, in)
- Return values and tuples
- Method overloading
- Expression-bodied members

## 📚 Additional Resources

- [Selection Statements](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/selection-statements)
- [Iteration Statements](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements)
- [Pattern Matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)

---

*"Control flow is the art of making your program do the right thing at the right time."*
