# Module 05: Asynchronous Programming - Introduction

## 📘 Modern Asynchronous Programming in .NET

Async/await is fundamental to modern .NET development. Master asynchronous programming to build responsive, scalable applications that efficiently handle I/O operations.

## 🎯 Module Objectives

- Understand synchronous vs asynchronous execution
- Master async/await syntax and patterns
- Work with Task and Task<T>
- Handle cancellation and timeouts
- Implement parallel programming patterns
- Avoid common async pitfalls
- Write performant asynchronous code

## 🗺️ Module Structure

### Lessons
1. **Introduction** (this document) - Async fundamentals
2. **Task and Task<T>** - Working with tasks
3. **Async Patterns** - Best practices and patterns
4. **Cancellation** - CancellationToken and timeouts
5. **Parallel Programming** - Task.WhenAll, Task.WhenAny, Parallel
6. **Advanced Topics** - ConfigureAwait, ValueTask, async streams

### Time Commitment
- **Estimated Time**: 2-3 weeks
- **Lessons**: 8-10 hours
- **Exercises**: 10-15 hours
- **Projects**: 8-12 hours

## 🤔 Why Asynchronous Programming?

### The Problem: Blocked Threads

```csharp
// ❌ Synchronous I/O (blocks thread)
public string DownloadData(string url)
{
    using var client = new HttpClient();
    var response = client.GetStringAsync(url).Result;  // Blocks thread!
    return response;
}

// On web server with 100 concurrent requests:
// - 100 threads blocked waiting for I/O
// - Each thread: ~1MB stack memory
// - Total: 100MB wasted on blocked threads
// - Thread pool exhaustion = server can't handle more requests
```

**The cost of blocking:**
1. **Thread overhead**: Each thread uses ~1MB of memory
2. **Context switching**: CPU time wasted switching between threads
3. **Scalability limit**: Thread pool has finite threads (typically 100-1000)
4. **Wasted resources**: Threads sit idle waiting for I/O

### The Solution: Async/Await

```csharp
// ✅ Asynchronous I/O (releases thread)
public async Task<string> DownloadDataAsync(string url)
{
    using var client = new HttpClient();
    var response = await client.GetStringAsync(url);  // Releases thread!
    return response;
}

// On web server with 100 concurrent requests:
// - Threads released while waiting for I/O
// - Available for other work
// - Can handle 1000s of concurrent requests
// - Minimal memory overhead
```

**Benefits of async:**
1. **Scalability**: Handle more concurrent operations
2. **Responsiveness**: UI doesn't freeze during I/O
3. **Resource efficiency**: Better thread pool utilization
4. **Performance**: Higher throughput for I/O-bound operations

## 🎨 Synchronous vs Asynchronous

### Synchronous Execution (Blocking)

```csharp
public void DownloadFiles()
{
    Console.WriteLine("Starting downloads...");

    // Download file 1 (takes 2 seconds)
    DownloadFile("file1.txt");
    Console.WriteLine("File 1 downloaded");

    // Download file 2 (takes 2 seconds)
    DownloadFile("file2.txt");
    Console.WriteLine("File 2 downloaded");

    // Download file 3 (takes 2 seconds)
    DownloadFile("file3.txt");
    Console.WriteLine("File 3 downloaded");

    Console.WriteLine("All downloads complete!");
    // Total time: 6 seconds (2 + 2 + 2)
}

// Timeline:
// 0s ████████████████████ File 1 (2s) [Thread blocked]
// 2s ████████████████████ File 2 (2s) [Thread blocked]
// 4s ████████████████████ File 3 (2s) [Thread blocked]
// 6s Complete
```

### Asynchronous Execution (Non-Blocking)

```csharp
public async Task DownloadFilesAsync()
{
    Console.WriteLine("Starting downloads...");

    // Start all downloads simultaneously
    var task1 = DownloadFileAsync("file1.txt");
    var task2 = DownloadFileAsync("file2.txt");
    var task3 = DownloadFileAsync("file3.txt");

    // Wait for all to complete
    await Task.WhenAll(task1, task2, task3);

    Console.WriteLine("All downloads complete!");
    // Total time: 2 seconds (all parallel)
}

// Timeline:
// 0s ████████████████████ File 1 (2s) [No thread blocked]
// 0s ████████████████████ File 2 (2s) [No thread blocked]
// 0s ████████████████████ File 3 (2s) [No thread blocked]
// 2s Complete
// 3x faster!
```

## 📊 async/await Basics

### Basic async Method

```csharp
// Synchronous method
public string GetData()
{
    Thread.Sleep(1000);  // Simulate work
    return "Data";
}

// Asynchronous method
public async Task<string> GetDataAsync()
{
    await Task.Delay(1000);  // Simulate async work
    return "Data";
}

// Method signature rules:
// 1. Add 'async' modifier
// 2. Return Task or Task<T>
// 3. Name ends with 'Async' (convention)
// 4. Use 'await' inside
```

### Calling async Methods

```csharp
// ❌ WRONG: Blocking async call
public void BadExample()
{
    var result = GetDataAsync().Result;  // Blocks thread - defeats purpose!
    // or
    GetDataAsync().Wait();  // Also blocks - don't do this!
}

// ✅ CORRECT: Awaiting async call
public async Task GoodExample()
{
    var result = await GetDataAsync();  // Properly async
    Console.WriteLine(result);
}

// ✅ CORRECT: Fire and forget (rare cases)
public void FireAndForget()
{
    _ = GetDataAsync();  // Starts async operation without waiting
    // Note: Be careful with exceptions - they'll be lost!
}
```

### Return Types

```csharp
// Task<T> - async method with return value
public async Task<int> CalculateAsync()
{
    await Task.Delay(100);
    return 42;
}

// Task - async method without return value (like void)
public async Task ProcessAsync()
{
    await Task.Delay(100);
    Console.WriteLine("Done");
}

// void - ONLY for event handlers (avoid otherwise)
private async void Button_Click(object sender, EventArgs e)
{
    await ProcessAsync();
    // Exception handling is tricky with async void!
}

// ValueTask<T> - performance optimization (advanced)
public async ValueTask<int> OptimizedAsync()
{
    await Task.Delay(100);
    return 42;
}
```

## 🔍 How async/await Works Internally

### State Machine

When you write:
```csharp
public async Task<string> DownloadAsync(string url)
{
    Console.WriteLine("Starting download");
    var data = await HttpClient.GetStringAsync(url);
    Console.WriteLine("Download complete");
    return data.ToUpper();
}
```

The compiler generates (simplified):
```csharp
public Task<string> DownloadAsync(string url)
{
    var stateMachine = new DownloadAsyncStateMachine();
    stateMachine.url = url;
    stateMachine.builder = AsyncTaskMethodBuilder<string>.Create();
    stateMachine.state = -1;
    stateMachine.builder.Start(ref stateMachine);
    return stateMachine.builder.Task;
}

private struct DownloadAsyncStateMachine : IAsyncStateMachine
{
    public int state;
    public string url;
    public AsyncTaskMethodBuilder<string> builder;
    private TaskAwaiter<string> awaiter;

    public void MoveNext()
    {
        string result;
        try
        {
            if (state == -1)  // Initial state
            {
                Console.WriteLine("Starting download");
                awaiter = HttpClient.GetStringAsync(url).GetAwaiter();

                if (!awaiter.IsCompleted)
                {
                    state = 0;
                    builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                    return;  // Thread released here!
                }
            }

            if (state == 0)  // Resumed after await
            {
                var data = awaiter.GetResult();
                Console.WriteLine("Download complete");
                result = data.ToUpper();
                builder.SetResult(result);
                return;
            }
        }
        catch (Exception ex)
        {
            builder.SetException(ex);
        }
    }
}
```

**Key points:**
1. Compiler generates state machine
2. Method returns immediately with Task
3. State machine captures local variables
4. Thread released during await
5. Continuation scheduled when operation completes
6. Method resumes on available thread

## 🎯 Common Async Patterns

### Pattern 1: I/O-Bound Operations

```csharp
// File I/O
public async Task<string> ReadFileAsync(string path)
{
    return await File.ReadAllTextAsync(path);
}

// Network I/O
public async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}

// Database I/O (with Entity Framework)
public async Task<User> GetUserAsync(int id)
{
    return await _context.Users.FindAsync(id);
}
```

**When to use:**
- File operations
- Network calls
- Database queries
- Any I/O that would block

### Pattern 2: Sequential async Operations

```csharp
public async Task<OrderResult> ProcessOrderAsync(Order order)
{
    // Step 1: Validate
    await ValidateOrderAsync(order);

    // Step 2: Calculate (depends on step 1)
    var total = await CalculateTotalAsync(order);

    // Step 3: Process payment (depends on step 2)
    var payment = await ProcessPaymentAsync(total);

    // Step 4: Save (depends on step 3)
    await SaveOrderAsync(order, payment);

    return new OrderResult { Success = true, OrderId = order.Id };
}
```

### Pattern 3: Parallel async Operations

```csharp
public async Task<DashboardData> GetDashboardDataAsync()
{
    // Start all operations simultaneously
    var usersTask = GetUsersAsync();
    var ordersTask = GetOrdersAsync();
    var revenueTask = GetRevenueAsync();
    var statsTask = GetStatsAsync();

    // Wait for all to complete
    await Task.WhenAll(usersTask, ordersTask, revenueTask, statsTask);

    // Get results
    return new DashboardData
    {
        Users = await usersTask,
        Orders = await ordersTask,
        Revenue = await revenueTask,
        Stats = await statsTask
    };
}
```

### Pattern 4: Timeout

```csharp
public async Task<string> FetchWithTimeoutAsync(string url, int timeoutMs)
{
    using var cts = new CancellationTokenSource(timeoutMs);

    try
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(url, cts.Token);
    }
    catch (OperationCanceledException)
    {
        throw new TimeoutException($"Request timed out after {timeoutMs}ms");
    }
}
```

### Pattern 5: Retry Logic

```csharp
public async Task<T> RetryAsync<T>(
    Func<Task<T>> operation,
    int maxRetries = 3,
    int delayMs = 1000)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
            await Task.Delay(delayMs * (i + 1));  // Exponential backoff
        }
    }

    // Last attempt without catch
    return await operation();
}

// Usage
var data = await RetryAsync(() => FetchDataAsync(url), maxRetries: 3);
```

## ⚠️ Common Pitfalls

### 1. async void

```csharp
// ❌ WRONG: async void (exceptions are lost)
public async void ProcessDataAsync()
{
    await Task.Delay(100);
    throw new Exception("Boom!");  // Exception crashes app!
}

// ✅ CORRECT: async Task
public async Task ProcessDataAsync()
{
    await Task.Delay(100);
    throw new Exception("Boom!");  // Exception can be caught
}

try
{
    await ProcessDataAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Caught: {ex.Message}");
}
```

**Only use async void for:**
- Event handlers
- Nothing else!

### 2. Blocking on async Code

```csharp
// ❌ WRONG: Deadlock potential
public string GetDataSync()
{
    return GetDataAsync().Result;  // Can deadlock!
}

// Why deadlock?
// 1. GetDataAsync() captures SynchronizationContext
// 2. .Result blocks current thread waiting
// 3. Async continuation tries to return to captured context
// 4. Context is blocked by .Result
// 5. Deadlock!

// ✅ CORRECT: Async all the way
public async Task<string> GetDataAsync()
{
    return await FetchDataAsync();
}
```

### 3. Not awaiting Tasks

```csharp
// ❌ WRONG: Forgot await
public async Task ProcessAsync()
{
    SaveDataAsync();  // Forgot await - runs but doesn't wait!
    Console.WriteLine("Done");  // Prints before save completes
}

// ✅ CORRECT: Await task
public async Task ProcessAsync()
{
    await SaveDataAsync();  // Wait for completion
    Console.WriteLine("Done");  // Prints after save completes
}
```

### 4. Using Task.Run for I/O

```csharp
// ❌ WRONG: Unnecessary thread pool work
public Task<string> ReadFileAsync(string path)
{
    return Task.Run(() => File.ReadAllText(path));
    // Creates extra thread pool thread for no reason
}

// ✅ CORRECT: Use async I/O
public async Task<string> ReadFileAsync(string path)
{
    return await File.ReadAllTextAsync(path);
    // No extra thread - true async I/O
}
```

**When to use Task.Run:**
- CPU-bound work (calculations, image processing)
- Offloading work from UI thread

**When NOT to use Task.Run:**
- I/O-bound work (has async APIs already)
- On server (ASP.NET Core) - wastes thread pool threads

## 📊 Performance Comparison

### Benchmark: Sync vs Async

```csharp
// Simulate 1000 concurrent web requests

// Synchronous (blocking)
public void SyncRequests()
{
    for (int i = 0; i < 1000; i++)
    {
        var response = HttpClient.GetStringAsync(url).Result;  // Blocks
        ProcessResponse(response);
    }
}
// Time: ~100 seconds
// Threads: Up to 1000 (thread pool exhaustion)
// Memory: ~1000 MB (thread stacks)

// Asynchronous (non-blocking)
public async Task AsyncRequests()
{
    var tasks = new List<Task>(1000);

    for (int i = 0; i < 1000; i++)
    {
        tasks.Add(ProcessRequestAsync());
    }

    await Task.WhenAll(tasks);
}

private async Task ProcessRequestAsync()
{
    var response = await HttpClient.GetStringAsync(url);  // Async
    ProcessResponse(response);
}
// Time: ~2 seconds (all parallel)
// Threads: ~10 (released during I/O)
// Memory: ~10 MB
// 50x faster, 100x less memory!
```

## 🎓 async/await Rules

### The Golden Rules

1. **async all the way**: Don't block async calls
   ```csharp
   // ✅ Good
   await SomeAsync();

   // ❌ Bad
   SomeAsync().Result;
   SomeAsync().Wait();
   ```

2. **Await in try/catch**: Exceptions only caught if awaited
   ```csharp
   try
   {
       await SomeAsync();  // ✅ Exception caught
   }
   catch (Exception ex)
   {
       // Handle
   }
   ```

3. **Return Task directly when possible**
   ```csharp
   // ✅ Good - no async overhead
   public Task<string> GetDataAsync()
   {
       return _client.GetStringAsync(url);
   }

   // Less efficient (but sometimes necessary for try/catch)
   public async Task<string> GetDataAsync()
   {
       return await _client.GetStringAsync(url);
   }
   ```

4. **Configure await when needed**
   ```csharp
   // Library code
   await SomeAsync().ConfigureAwait(false);  // Don't capture context
   ```

## ✅ Best Practices Summary

1. **Use async for I/O**: File, network, database
2. **Name async methods with 'Async' suffix**: `GetDataAsync()`
3. **Return Task<T>, not async void**: Except event handlers
4. **Await all the way**: Don't block with `.Result` or `.Wait()`
5. **Use cancellation tokens**: Allow operation cancellation
6. **Handle exceptions properly**: Use try/catch with await
7. **Consider ConfigureAwait**: In library code
8. **Avoid async void**: Exceptions can crash app
9. **Use Task.WhenAll for parallel**: Not sequential awaits
10. **Don't mix sync and async**: Choose one pattern

## ⏭️ Next Steps

Ready to dive deeper? Proceed to:
- **[Lesson 02: Task and Task<T>](02-task-and-task-t.md)**

You'll learn about:
- Creating and manipulating tasks
- Task.Run, Task.FromResult
- Continuation with ContinueWith
- Task status and properties
- Task combinators

## 📚 Additional Resources

- [Async/Await Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [Task-based Asynchronous Pattern (TAP)](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)
- [Stephen Cleary's Blog](https://blog.stephencleary.com/)

---

*"Async/await is not just a feature, it's a programming model that changes how we think about I/O."*
