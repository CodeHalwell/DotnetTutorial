# Exercise 01: Library Management System

## 🎯 Objective

Build a library management system that demonstrates Object-Oriented Programming principles. This exercise practices:

- **Encapsulation**: Protecting data and providing controlled access
- **Inheritance**: Creating class hierarchies
- **Polymorphism**: Method overriding and interfaces
- **Abstraction**: Abstract classes and interfaces
- **SOLID Principles**: Especially Single Responsibility and Open/Closed

## 📋 Requirements

### Core Classes

Create a library system with the following components:

#### 1. **Library Item Base**

Create an abstract `LibraryItem` class:
- Properties: `Id`, `Title`, `PublicationYear`, `IsAvailable`
- Abstract method: `GetItemInfo()`
- Virtual method: `CheckOut()`, `Return()`
- Method: `GetAgeInYears()`

#### 2. **Derived Item Types**

Implement concrete classes:

**Book** (inherits from `LibraryItem`):
- Additional properties: `Author`, `ISBN`, `PageCount`
- Override `GetItemInfo()` to include book-specific details
- Implement `ISearchable` interface

**Magazine** (inherits from `LibraryItem`):
- Additional properties: `IssueNumber`, `Publisher`
- Override `GetItemInfo()` for magazine details
- Implement `ISearchable` interface

**DVD** (inherits from `LibraryItem`):
- Additional properties: `Director`, `DurationMinutes`, `Rating`
- Override `GetItemInfo()` for DVD details
- Override `CheckOut()` to limit checkout to 7 days (books get 14 days)

#### 3. **Member System**

**Member** class:
- Properties: `MemberId`, `Name`, `Email`, `JoinDate`, `MembershipType` (enum)
- Method: `CanBorrow()` - checks if member can borrow more items
- Method: `GetBorrowingLimit()` - returns limit based on membership type
- Collection: `BorrowedItems` (List of LibraryItem)

**MembershipType** enum:
- `Basic`: Can borrow 3 items
- `Standard`: Can borrow 5 items
- `Premium`: Can borrow 10 items

#### 4. **Library Class**

Main `Library` class:
- Collections: `Items` (List<LibraryItem>), `Members` (List<Member>)
- Methods:
  - `AddItem(LibraryItem item)`
  - `AddMember(Member member)`
  - `BorrowItem(string memberId, string itemId)`
  - `ReturnItem(string memberId, string itemId)`
  - `SearchByTitle(string title)` (uses ISearchable)
  - `GetAvailableItems()`
  - `GetOverdueItems()` (bonus)

#### 5. **Interfaces**

**ISearchable**:
```csharp
public interface ISearchable
{
    bool MatchesSearchTerm(string searchTerm);
}
```

**INotifiable** (bonus):
```csharp
public interface INotifiable
{
    void SendNotification(string message);
}
```

### Validation Rules

1. **Cannot borrow if:**
   - Member has reached their borrowing limit
   - Item is not available
   - Member ID or Item ID doesn't exist

2. **Cannot return if:**
   - Member doesn't have the item
   - Item ID doesn't exist

3. **ISBN validation:**
   - Must be 13 digits
   - Must start with "978" or "979"

4. **Email validation:**
   - Must contain @ symbol
   - Must have domain

### Console Application

Create an interactive console app with menu:

```
=== Library Management System ===
1. Add new book
2. Add new magazine
3. Add new DVD
4. Register new member
5. Borrow item
6. Return item
7. Search items
8. View available items
9. View member details
10. Exit

Select option:
```

## 📊 Example Interactions

```
Select option: 1
Enter book title: The Great Gatsby
Enter author: F. Scott Fitzgerald
Enter ISBN: 9780743273565
Enter publication year: 1925
Enter page count: 180
✓ Book added successfully! ID: BOOK001

Select option: 4
Enter member name: John Doe
Enter email: john@example.com
Select membership type (1=Basic, 2=Standard, 3=Premium): 2
✓ Member registered! ID: MEM001

Select option: 5
Enter member ID: MEM001
Enter item ID: BOOK001
✓ Item borrowed successfully!
Due date: 2025-11-26

Select option: 6
Enter member ID: MEM001
Enter item ID: BOOK001
✓ Item returned successfully!

Select option: 7
Enter search term: gatsby
Found 1 item(s):
- [BOOK001] The Great Gatsby by F. Scott Fitzgerald (1925) - Available

Select option: 9
Enter member ID: MEM001
=== Member Details ===
Name: John Doe
Email: john@example.com
Membership: Standard
Borrowed items: 0/5
Join date: 2025-01-15
```

## ✅ Acceptance Criteria

1. **All tests pass** (`dotnet test`)
2. **OOP Principles demonstrated:**
   - Encapsulation: Private fields, public properties with validation
   - Inheritance: LibraryItem base class with derived classes
   - Polymorphism: Overridden methods work correctly
   - Abstraction: Abstract class and interfaces used
3. **SOLID Principles followed:**
   - Single Responsibility: Each class has one job
   - Open/Closed: Can add new item types without modifying Library class
   - Liskov Substitution: Derived classes can replace base class
   - Interface Segregation: Small, focused interfaces
   - Dependency Inversion: Depend on abstractions (interfaces)
4. **Proper error handling** with custom exceptions
5. **XML documentation** on all public members
6. **Input validation** for all user inputs

## 💡 Hints

### Inheritance Structure

```csharp
public abstract class LibraryItem
{
    public string Id { get; protected set; }
    public string Title { get; set; }
    public int PublicationYear { get; set; }
    public bool IsAvailable { get; protected set; }
    public DateTime? CheckOutDate { get; protected set; }
    public DateTime? DueDate { get; protected set; }

    protected LibraryItem(string id, string title, int year)
    {
        Id = id;
        Title = title;
        PublicationYear = year;
        IsAvailable = true;
    }

    public abstract string GetItemInfo();

    public virtual void CheckOut(int days)
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Item is not available");

        IsAvailable = false;
        CheckOutDate = DateTime.Now;
        DueDate = DateTime.Now.AddDays(days);
    }

    public virtual void Return()
    {
        IsAvailable = true;
        CheckOutDate = null;
        DueDate = null;
    }
}
```

### Polymorphism Example

```csharp
public class Book : LibraryItem, ISearchable
{
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int PageCount { get; set; }

    public Book(string id, string title, int year, string author, string isbn, int pageCount)
        : base(id, title, year)
    {
        Author = author;
        ISBN = isbn;
        PageCount = pageCount;
    }

    public override string GetItemInfo()
    {
        return $"[{Id}] {Title} by {Author} ({PublicationYear}) - {PageCount} pages - ISBN: {ISBN}";
    }

    public bool MatchesSearchTerm(string searchTerm)
    {
        return Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
               Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
               ISBN.Contains(searchTerm);
    }
}
```

### Encapsulation Example

```csharp
public class Member
{
    private List<LibraryItem> _borrowedItems = new();

    public string MemberId { get; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime JoinDate { get; }
    public MembershipType MembershipType { get; set; }

    // Encapsulation: Read-only access to borrowed items
    public IReadOnlyCollection<LibraryItem> BorrowedItems => _borrowedItems.AsReadOnly();

    public Member(string memberId, string name, string email, MembershipType membershipType)
    {
        MemberId = memberId;
        Name = name;
        Email = email;
        MembershipType = membershipType;
        JoinDate = DateTime.Now;
    }

    public int GetBorrowingLimit()
    {
        return MembershipType switch
        {
            MembershipType.Basic => 3,
            MembershipType.Standard => 5,
            MembershipType.Premium => 10,
            _ => 0
        };
    }

    public bool CanBorrow()
    {
        return _borrowedItems.Count < GetBorrowingLimit();
    }

    internal void AddBorrowedItem(LibraryItem item)
    {
        _borrowedItems.Add(item);
    }

    internal void RemoveBorrowedItem(LibraryItem item)
    {
        _borrowedItems.Remove(item);
    }
}
```

## 🎓 Learning Goals

### OOP Concepts
- **Encapsulation**: Private fields, controlled access through properties/methods
- **Inheritance**: Building class hierarchies with base and derived classes
- **Polymorphism**: Method overriding for type-specific behavior
- **Abstraction**: Using abstract classes and interfaces

### SOLID Principles
- **Single Responsibility**: Each class has one clear purpose
- **Open/Closed**: Easy to extend with new item types without modifying existing code
- **Liskov Substitution**: Derived classes can substitute base class
- **Interface Segregation**: Small, focused interfaces (ISearchable)
- **Dependency Inversion**: Library depends on abstractions (LibraryItem, ISearchable)

### Design Patterns
- **Template Method**: Base class defines algorithm structure, derived classes fill in details
- **Strategy Pattern**: Different checkout strategies for different item types
- **Factory Pattern** (bonus): ItemFactory for creating items

## ⏱️ Estimated Time

- **Class design**: 30 minutes
- **Implementation**: 60-90 minutes
- **Console application**: 30 minutes
- **Testing**: 20-30 minutes
- **Total: ~2.5-3 hours**

## 🚀 Bonus Challenges

1. **Late Fees**: Calculate late fees for overdue items
2. **Reservation System**: Allow members to reserve checked-out items
3. **Notification System**: Implement INotifiable to send emails/SMS
4. **Transaction History**: Track all borrowing/returning activities
5. **Persistence**: Save/load library data to/from JSON file
6. **Rating System**: Allow members to rate items
7. **Advanced Search**: Search by multiple criteria (author, year range, etc.)
8. **Member Suspension**: Suspend members with overdue items
9. **Item Categories**: Add category/genre system with filtering
10. **Statistics**: Generate reports (most borrowed items, active members, etc.)

## 📝 Testing Requirements

Write unit tests for:
- `Book.GetItemInfo()` returns correct format
- `Member.CanBorrow()` respects membership limits
- `Library.BorrowItem()` throws exception if item unavailable
- `Library.ReturnItem()` makes item available again
- `Book.MatchesSearchTerm()` finds items correctly
- `Member.GetBorrowingLimit()` returns correct limits for each membership type

Example test:
```csharp
[Fact]
public void BorrowItem_WhenItemAvailable_UpdatesAvailability()
{
    // Arrange
    var library = new Library();
    var book = new Book("B001", "Test Book", 2020, "Author", "9780000000001", 200);
    var member = new Member("M001", "John Doe", "john@example.com", MembershipType.Standard);
    library.AddItem(book);
    library.AddMember(member);

    // Act
    library.BorrowItem("M001", "B001");

    // Assert
    Assert.False(book.IsAvailable);
    Assert.Single(member.BorrowedItems);
}
```

---

**Ready?** Open `starter/` directory and begin implementation!

**Need help?** Check the `hints/` directory for additional guidance.

**Finished?** Compare your solution with `solution/` directory.
