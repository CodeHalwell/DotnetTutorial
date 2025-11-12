# Project 02: Task Manager CLI

## 🎯 Project Overview

Build a command-line task manager application that helps users organize their daily tasks. This project focuses on:

- **File I/O**: Reading/writing JSON data
- **LINQ**: Complex queries and data manipulation
- **Data structures**: Lists, dictionaries, enums
- **DateTime handling**: Due dates, priorities, filtering
- **Error handling**: Input validation, file operations
- **OOP**: Classes, interfaces, inheritance
- **Design patterns**: Repository pattern, Command pattern

## 📋 Functional Requirements

### Core Features

#### 1. Task Management
- **Create tasks** with title, description, due date, priority, category
- **List tasks** with various filters (all, pending, completed, overdue)
- **Update tasks** (edit title, description, priority, etc.)
- **Delete tasks** with confirmation
- **Mark tasks** as complete/incomplete
- **Search tasks** by keyword in title or description

#### 2. Categories & Priorities
- **Categories**: Work, Personal, Shopping, Health, Learning (customizable)
- **Priorities**: Low, Medium, High, Urgent
- **Color-coded output** based on priority in console
- **Filter by category** and priority

#### 3. Due Date Management
- **Set due dates** for tasks
- **View overdue tasks** highlighted in red
- **View tasks due today** highlighted in yellow
- **View upcoming tasks** (next 7 days)
- **Sort by due date**

#### 4. Data Persistence
- **Save to JSON file** automatically after each operation
- **Load from JSON file** on startup
- **Backup functionality** to create timestamped backups
- **Import/Export** tasks to/from JSON

#### 5. Statistics & Reports
- **Summary dashboard** showing:
  - Total tasks
  - Completed vs pending
  - Overdue tasks count
  - Tasks by category breakdown
  - Tasks by priority breakdown
- **Productivity report**: Completion rate over time
- **Export reports** to text file

## 🎨 User Interface

### Main Menu

```
╔════════════════════════════════════════════════╗
║          TASK MANAGER v1.0                     ║
╚════════════════════════════════════════════════╝

📊 Dashboard: 15 total | 8 pending | 7 completed | 2 overdue

[1] Add new task
[2] View all tasks
[3] View pending tasks
[4] View completed tasks
[5] View overdue tasks
[6] View today's tasks
[7] Search tasks
[8] Update task
[9] Delete task
[10] Toggle task completion
[11] View by category
[12] View by priority
[13] Statistics & Reports
[14] Settings
[0] Exit

Enter your choice:
```

### Task Display

```
╔════════════════════════════════════════════════╗
║ [1] Fix production bug                         ║
╠════════════════════════════════════════════════╣
║ Priority: 🔴 URGENT                           ║
║ Category: Work                                 ║
║ Due: 2025-11-13 (Tomorrow)                    ║
║ Status: ⚪ Pending                            ║
║ Description:                                   ║
║   Critical bug in payment processing system    ║
║ Created: 2025-11-12 09:30 AM                  ║
╚════════════════════════════════════════════════╝
```

### Color Scheme

- **🔴 Urgent priority / Overdue**: Red text
- **🟠 High priority**: Yellow text
- **🟢 Medium priority**: Green text
- **⚪ Low priority**: White text
- **✅ Completed**: Gray with strikethrough
- **⏰ Due today**: Yellow background

## 💾 Data Model

### Task Class

```csharp
public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Computed properties
    public bool IsOverdue => !IsCompleted &&
                             DueDate.HasValue &&
                             DueDate.Value.Date < DateTime.Today;

    public bool IsDueToday => !IsCompleted &&
                              DueDate.HasValue &&
                              DueDate.Value.Date == DateTime.Today;

    public int DaysUntilDue => DueDate.HasValue
        ? (DueDate.Value.Date - DateTime.Today).Days
        : int.MaxValue;
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}
```

### Repository Interface

```csharp
public interface ITaskRepository
{
    Task Add(Task task);
    Task<Task?> GetById(Guid id);
    IEnumerable<Task> GetAll();
    IEnumerable<Task> GetPending();
    IEnumerable<Task> GetCompleted();
    IEnumerable<Task> GetOverdue();
    IEnumerable<Task> GetDueToday();
    IEnumerable<Task> GetByCategory(string category);
    IEnumerable<Task> GetByPriority(Priority priority);
    IEnumerable<Task> Search(string keyword);
    void Update(Task task);
    void Delete(Guid id);
    void Save();
}
```

## 📊 Example Interactions

### Adding a Task

```
Enter your choice: 1

=== Add New Task ===

Task title: Complete project documentation
Task description: Write comprehensive README and API docs
Category: Work
Priority (1=Low, 2=Medium, 3=High, 4=Urgent): 3
Due date (yyyy-mm-dd) or press Enter to skip: 2025-11-15

✓ Task added successfully! (ID: a1b2c3d4)

Press any key to continue...
```

### Viewing Tasks

```
Enter your choice: 2

=== All Tasks (15) ===

┌─────┬──────────┬────────────────────────┬──────────┬──────────────┬────────┐
│ ID  │ Priority │ Title                  │ Category │ Due Date     │ Status │
├─────┼──────────┼────────────────────────┼──────────┼──────────────┼────────┤
│ 1   │ 🔴 URGENT│ Fix production bug     │ Work     │ Tomorrow     │ ⚪ Pen │
│ 2   │ 🟠 HIGH  │ Complete documentation │ Work     │ In 3 days    │ ⚪ Pen │
│ 3   │ 🟢 MED   │ Buy groceries          │ Shopping │ Today        │ ⚪ Pen │
│ 4   │ ⚪ LOW   │ Read chapter 5         │ Learning │ In 1 week    │ ✅ Com │
│ 5   │ 🟠 HIGH  │ Submit expense report  │ Work     │ 2 days ago   │ ⚪ OVR │
└─────┴──────────┴────────────────────────┴──────────┴──────────────┴────────┘

[V] View details  [U] Update  [D] Delete  [T] Toggle completion  [B] Back

Enter task ID or action:
```

### Task Details

```
Enter task ID: 1

╔════════════════════════════════════════════════╗
║ Fix production bug                             ║
╠════════════════════════════════════════════════╣
║ ID: a1b2c3d4-5e6f-7890-abcd-ef1234567890      ║
║ Priority: 🔴 URGENT                           ║
║ Category: Work                                 ║
║ Due: 2025-11-13 (Tomorrow)                    ║
║ Status: ⚪ Pending                            ║
║                                                ║
║ Description:                                   ║
║   Critical bug in payment processing that      ║
║   affects all credit card transactions         ║
║                                                ║
║ Created: 2025-11-12 09:30 AM                  ║
║ Days until due: 1                              ║
╚════════════════════════════════════════════════╝

[U] Update  [D] Delete  [T] Toggle completion  [B] Back
```

### Statistics Dashboard

```
Enter your choice: 13

╔════════════════════════════════════════════════╗
║          STATISTICS & REPORTS                  ║
╚════════════════════════════════════════════════╝

📊 Task Overview
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Total Tasks:       15
Pending:           8  (53%)
Completed:         7  (47%)
Overdue:           2  (13%)

📂 By Category
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Work:             6  ████████████████░░░░  (40%)
Personal:         4  ███████████░░░░░░░░░  (27%)
Shopping:         3  ████████░░░░░░░░░░░░  (20%)
Learning:         2  █████░░░░░░░░░░░░░░░  (13%)

🔥 By Priority
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔴 Urgent:        2  █████░░░░░░░░░░░░░░░  (13%)
🟠 High:          5  █████████████░░░░░░░  (33%)
🟢 Medium:        6  ███████████████░░░░░  (40%)
⚪ Low:           2  █████░░░░░░░░░░░░░░░  (13%)

📈 Productivity
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
This week:        12 tasks completed
Last week:        8 tasks completed
Completion rate:  50% improvement!

⏰ Upcoming Deadlines
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Today:            1 task
Tomorrow:         2 tasks
This week:        4 tasks

[E] Export report  [B] Back
```

## ✅ Technical Requirements

### 1. Architecture

```
TaskManager/
├── Models/
│   ├── Task.cs
│   ├── Priority.cs
│   └── TaskStatistics.cs
├── Repositories/
│   ├── ITaskRepository.cs
│   └── JsonTaskRepository.cs
├── Services/
│   ├── ITaskService.cs
│   ├── TaskService.cs
│   └── StatisticsService.cs
├── UI/
│   ├── ConsoleUI.cs
│   ├── MenuRenderer.cs
│   └── TaskRenderer.cs
├── Utilities/
│   ├── ConsoleHelper.cs
│   └── ValidationHelper.cs
├── Data/
│   └── tasks.json
└── Program.cs
```

### 2. JSON Storage Format

```json
{
  "tasks": [
    {
      "id": "a1b2c3d4-5e6f-7890-abcd-ef1234567890",
      "title": "Fix production bug",
      "description": "Critical bug in payment processing",
      "priority": 4,
      "category": "Work",
      "createdAt": "2025-11-12T09:30:00",
      "dueDate": "2025-11-13T17:00:00",
      "isCompleted": false,
      "completedAt": null
    }
  ],
  "categories": ["Work", "Personal", "Shopping", "Health", "Learning"],
  "settings": {
    "defaultCategory": "Personal",
    "defaultPriority": 2,
    "autoBackup": true
  }
}
```

### 3. LINQ Queries Required

Implement these LINQ operations:

```csharp
// Overdue tasks
var overdueTasks = tasks
    .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < DateTime.Today)
    .OrderBy(t => t.DueDate);

// Tasks by priority, then due date
var prioritizedTasks = tasks
    .Where(t => !t.IsCompleted)
    .OrderByDescending(t => t.Priority)
    .ThenBy(t => t.DueDate ?? DateTime.MaxValue);

// Tasks grouped by category
var tasksByCategory = tasks
    .GroupBy(t => t.Category)
    .Select(g => new
    {
        Category = g.Key,
        Count = g.Count(),
        CompletedCount = g.Count(t => t.IsCompleted),
        CompletionRate = g.Count() > 0 ? g.Count(t => t.IsCompleted) * 100.0 / g.Count() : 0
    });

// Search in title and description
var searchResults = tasks
    .Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));

// Upcoming tasks (next 7 days)
var upcomingTasks = tasks
    .Where(t => !t.IsCompleted &&
                t.DueDate.HasValue &&
                t.DueDate.Value >= DateTime.Today &&
                t.DueDate.Value <= DateTime.Today.AddDays(7))
    .OrderBy(t => t.DueDate);
```

### 4. Error Handling

```csharp
// File I/O errors
try
{
    var json = File.ReadAllText(filePath);
    tasks = JsonSerializer.Deserialize<List<Task>>(json) ?? new();
}
catch (FileNotFoundException)
{
    Console.WriteLine("No saved tasks found. Starting fresh.");
    tasks = new List<Task>();
}
catch (JsonException ex)
{
    Console.WriteLine($"Error reading tasks file: {ex.Message}");
    Console.WriteLine("Would you like to start fresh? (y/n): ");
    // Handle corrupt file
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Permission denied accessing tasks file.");
    // Handle permission issues
}

// Input validation
public static DateTime? GetDueDateFromUser()
{
    while (true)
    {
        Console.Write("Due date (yyyy-mm-dd) or press Enter to skip: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (DateTime.TryParse(input, out var date))
        {
            if (date >= DateTime.Today)
                return date;
            else
                Console.WriteLine("❌ Due date cannot be in the past.");
        }
        else
        {
            Console.WriteLine("❌ Invalid date format. Use yyyy-mm-dd");
        }
    }
}
```

## 🎓 Learning Objectives

### Core Concepts
1. **File I/O**: Reading/writing JSON files
2. **JSON Serialization**: Using System.Text.Json
3. **LINQ**: Complex queries, grouping, aggregation
4. **DateTime**: Date arithmetic, comparisons
5. **Collections**: Lists, LINQ operations
6. **Enums**: Priority levels
7. **Repository Pattern**: Data access abstraction
8. **Console UI**: Formatted output, color, tables

### Advanced Concepts
9. **Computed Properties**: IsOverdue, DaysUntilDue
10. **Null handling**: Nullable DateTime
11. **GUID**: Unique identifiers
12. **Sorting**: Multiple criteria
13. **Filtering**: Dynamic queries
14. **Statistics**: Aggregations, percentages
15. **Error Recovery**: Graceful failure handling

## ⏱️ Estimated Time

- **Project setup**: 30 minutes
- **Core CRUD**: 2-3 hours
- **Filtering & search**: 1-2 hours
- **Statistics & reports**: 1-2 hours
- **UI polish**: 1-2 hours
- **Testing & refinement**: 1 hour
- **Total: ~8-12 hours**

## 🚀 Bonus Challenges

1. **Recurring Tasks**: Daily, weekly, monthly tasks
2. **Subtasks**: Break tasks into smaller steps
3. **Tags**: Multiple tags per task
4. **Task Templates**: Create tasks from templates
5. **Time Tracking**: Track time spent on tasks
6. **Reminders**: Console notification for due tasks
7. **Calendar View**: Display tasks in calendar format
8. **Cloud Sync**: Sync with cloud storage (OneDrive, Dropbox)
9. **Multiple Lists**: Support for multiple task lists
10. **Collaboration**: Share tasks with others (export/import)
11. **Task Dependencies**: Tasks that depend on others
12. **Custom Fields**: User-defined task properties
13. **Themes**: Customizable color schemes
14. **Keyboard Shortcuts**: Quick actions with hotkeys
15. **Task History**: Track all changes to tasks

## 📝 Testing Requirements

### Unit Tests

```csharp
[Fact]
public void IsOverdue_TaskPastDueDate_ReturnsTrue()
{
    // Arrange
    var task = new Task
    {
        Title = "Test Task",
        DueDate = DateTime.Today.AddDays(-1),
        IsCompleted = false
    };

    // Act & Assert
    Assert.True(task.IsOverdue);
}

[Fact]
public void GetOverdue_ReturnsOnlyOverdueTasks()
{
    // Arrange
    var repository = new JsonTaskRepository("test_tasks.json");
    repository.Add(new Task { Title = "Overdue", DueDate = DateTime.Today.AddDays(-1) });
    repository.Add(new Task { Title = "Future", DueDate = DateTime.Today.AddDays(1) });
    repository.Add(new Task { Title = "No due date" });

    // Act
    var overdueTasks = repository.GetOverdue().ToList();

    // Assert
    Assert.Single(overdueTasks);
    Assert.Equal("Overdue", overdueTasks[0].Title);
}

[Fact]
public void Search_FindsTasksByKeyword()
{
    // Arrange
    var repository = new JsonTaskRepository("test_tasks.json");
    repository.Add(new Task { Title = "Buy groceries", Description = "Milk and bread" });
    repository.Add(new Task { Title = "Write report", Description = "Q4 financial report" });

    // Act
    var results = repository.Search("report").ToList();

    // Assert
    Assert.Single(results);
    Assert.Equal("Write report", results[0].Title);
}
```

## 💡 Implementation Hints

### Repository Pattern

```csharp
public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private List<Task> _tasks;

    public JsonTaskRepository(string filePath = "tasks.json")
    {
        _filePath = filePath;
        LoadTasks();
    }

    private void LoadTasks()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _tasks = JsonSerializer.Deserialize<List<Task>>(json) ?? new();
            }
            else
            {
                _tasks = new List<Task>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
            _tasks = new List<Task>();
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    public Task Add(Task task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.Now;
        _tasks.Add(task);
        Save();
        return task;
    }

    public IEnumerable<Task> GetOverdue()
    {
        return _tasks
            .Where(t => t.IsOverdue)
            .OrderBy(t => t.DueDate);
    }

    // Implement other methods...
}
```

### Console Formatting

```csharp
public static class ConsoleHelper
{
    public static void WriteHeader(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n╔{'═'.Repeat(text.Length + 2)}╗");
        Console.WriteLine($"║ {text} ║");
        Console.WriteLine($"╚{'═'.Repeat(text.Length + 2)}╝\n");
        Console.ResetColor();
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    public static void WritePriority(Priority priority)
    {
        var (emoji, color) = priority switch
        {
            Priority.Urgent => ("🔴", ConsoleColor.Red),
            Priority.High => ("🟠", ConsoleColor.Yellow),
            Priority.Medium => ("🟢", ConsoleColor.Green),
            Priority.Low => ("⚪", ConsoleColor.Gray),
            _ => ("⚪", ConsoleColor.White)
        };

        Console.ForegroundColor = color;
        Console.Write($"{emoji} {priority}");
        Console.ResetColor();
    }
}
```

---

## 📚 Resources

- **System.Text.Json**: [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json)
- **DateTime in C#**: [DateTime Fundamentals](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)
- **LINQ**: [LINQ Overview](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)
- **Repository Pattern**: [Martin Fowler](https://martinfowler.com/eaaCatalog/repository.html)

---

**Ready to build?** Start in `starter/` directory with the basic structure provided!

**Need help?** Check `hints/` directory for implementation guidance.

**Finished?** Compare with `solution/` directory and explore bonus challenges!
