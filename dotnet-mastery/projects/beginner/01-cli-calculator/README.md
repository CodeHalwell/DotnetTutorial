# Project 01: CLI Calculator with Unit Conversion

## 🎯 Project Overview

Build a feature-rich command-line calculator that performs basic arithmetic operations and unit conversions. This project integrates all concepts from Module 01-03: types, methods, control flow, error handling, and OOP basics.

## 🏆 Learning Objectives

By completing this project, you will:

- Design a multi-feature application from scratch
- Organize code into logical classes and modules
- Implement a REPL (Read-Eval-Print Loop) interface
- Handle user input validation comprehensively
- Apply exception handling in a real application
- Write testable, maintainable code
- Document code professionally

## 📋 Requirements

### Phase 1: Basic Calculator

Implement a calculator with these operations:

#### Arithmetic Operations
- **Addition**: `5 + 3` → `8`
- **Subtraction**: `10 - 4` → `6`
- **Multiplication**: `6 * 7` → `42`
- **Division**: `15 / 3` → `5`
- **Modulo**: `17 % 5` → `2`
- **Power**: `2 ^ 8` → `256`
- **Square Root**: `sqrt(16)` → `4`

#### Special Features
- **Order of operations**: `2 + 3 * 4` → `14` (not 20)
- **Parentheses**: `(2 + 3) * 4` → `20`
- **Negative numbers**: `-5 + 3` → `-2`
- **Decimal support**: `10.5 / 2` → `5.25`

### Phase 2: Unit Conversions

Add unit conversion capabilities:

#### Length
- Meters ↔ Feet
- Meters ↔ Miles
- Kilometers ↔ Miles
- Centimeters ↔ Inches

#### Weight
- Kilograms ↔ Pounds
- Grams ↔ Ounces

#### Temperature
- Celsius ↔ Fahrenheit
- Celsius ↔ Kelvin

#### Area
- Square meters ↔ Square feet
- Acres ↔ Hectares

#### Volume
- Liters ↔ Gallons
- Milliliters ↔ Fluid ounces

### Phase 3: Advanced Features

#### History
- Store last 10 calculations
- Display history
- Recall previous results using `ans` keyword
- Clear history

#### Memory Functions
- `M+`: Add to memory
- `M-`: Subtract from memory
- `MR`: Recall memory
- `MC`: Clear memory

#### Variables
- Store results: `x = 5 + 3` → `x = 8`
- Use in expressions: `x * 2` → `16`
- List all variables

## 🎨 User Interface Examples

### Basic Mode

```
╔═══════════════════════════════════╗
║   CLI Calculator v1.0             ║
║   Type 'help' for commands        ║
╚═══════════════════════════════════╝

> 5 + 3
= 8

> 10 * 2.5
= 25

> sqrt(16)
= 4

> (2 + 3) * 4
= 20

> help
Commands:
  calc        Basic calculator mode (default)
  convert     Unit conversion mode
  history     Show calculation history
  clear       Clear screen
  exit        Exit calculator

> exit
Goodbye!
```

### Conversion Mode

```
> convert

Conversion Mode
Available conversions:
  length      Length conversions
  weight      Weight conversions
  temp        Temperature conversions
  area        Area conversions
  volume      Volume conversions

> temp
Temperature Conversions:
  c to f      Celsius to Fahrenheit
  f to c      Fahrenheit to Celsius
  c to k      Celsius to Kelvin
  k to c      Kelvin to Celsius

> 100 c to f
= 212.00 °F

> back
Exited conversion mode
```

### Variable Mode

```
> x = 5 + 3
x = 8

> y = x * 2
y = 16

> x + y
= 24

> vars
Variables:
  x = 8
  y = 16
  ans = 24

> clear vars
Variables cleared
```

## 🏗️ Architecture

### Recommended Structure

```
CLICalculator/
├── Program.cs                    # Entry point
├── Calculator/
│   ├── ArithmeticCalculator.cs  # Basic math operations
│   ├── ExpressionParser.cs      # Parse and evaluate expressions
│   └── OperatorPrecedence.cs    # Handle order of operations
├── Converters/
│   ├── IUnitConverter.cs        # Interface for converters
│   ├── LengthConverter.cs
│   ├── WeightConverter.cs
│   ├── TemperatureConverter.cs
│   ├── AreaConverter.cs
│   └── VolumeConverter.cs
├── Core/
│   ├── History.cs               # Calculation history
│   ├── Memory.cs                # Calculator memory
│   └── Variables.cs             # Variable storage
├── UI/
│   ├── ConsoleUI.cs             # Console interface
│   ├── CommandParser.cs         # Parse user commands
│   └── HelpSystem.cs            # Help text
└── Tests/
    ├── CalculatorTests.cs
    ├── ConverterTests.cs
    └── ParserTests.cs
```

### Design Patterns

Use these patterns:

1. **Strategy Pattern**: Different conversion strategies
2. **Command Pattern**: User commands (history, clear, etc.)
3. **Singleton Pattern**: Calculator memory
4. **Factory Pattern**: Create converters

## 🧪 Testing Requirements

Write comprehensive unit tests:

### Calculator Tests
```csharp
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
    var calc = new ArithmeticCalculator();
    Assert.Equal(8, calc.Calculate("5 + 3"));
}

[Fact]
public void Divide_ByZero_ThrowsException()
{
    var calc = new ArithmeticCalculator();
    Assert.Throws<DivideByZeroException>(() => calc.Calculate("5 / 0"));
}

[Fact]
public void Calculate_WithParentheses_RespectsOrderOfOperations()
{
    var calc = new ArithmeticCalculator();
    Assert.Equal(20, calc.Calculate("(2 + 3) * 4"));
}
```

### Converter Tests
```csharp
[Theory]
[InlineData(0, 32)]
[InlineData(100, 212)]
[InlineData(-40, -40)]
public void CelsiusToFahrenheit_ValidInput_ReturnsCorrectValue(
    double celsius, double expectedFahrenheit)
{
    var converter = new TemperatureConverter();
    Assert.Equal(expectedFahrenheit, converter.CelsiusToFahrenheit(celsius));
}
```

## ✅ Acceptance Criteria

Your calculator must:

1. **Functionality**
   - ✅ Perform all basic arithmetic operations
   - ✅ Handle order of operations correctly
   - ✅ Support parentheses
   - ✅ Perform all listed unit conversions
   - ✅ Store and recall history
   - ✅ Support memory operations
   - ✅ Support variables

2. **Error Handling**
   - ✅ Validate all user input
   - ✅ Provide clear error messages
   - ✅ Never crash from user input
   - ✅ Handle edge cases (division by zero, invalid expressions, etc.)

3. **Code Quality**
   - ✅ Follow C# naming conventions
   - ✅ Include XML documentation comments
   - ✅ Organize code into logical classes
   - ✅ Use appropriate access modifiers
   - ✅ No code duplication (DRY principle)

4. **Testing**
   - ✅ Minimum 80% code coverage
   - ✅ All critical paths tested
   - ✅ Edge cases covered

5. **User Experience**
   - ✅ Clear, intuitive interface
   - ✅ Helpful error messages
   - ✅ Responsive (< 100ms for calculations)
   - ✅ Help system available

## 🎯 Milestones

### Milestone 1: Basic Calculator (Week 1)
- Arithmetic operations
- Expression parsing
- Error handling
- Basic tests

### Milestone 2: Unit Conversions (Week 2)
- Converter classes
- Conversion UI
- Comprehensive tests

### Milestone 3: Advanced Features (Week 3)
- History system
- Memory operations
- Variable support
- Polish and refine

## 💡 Implementation Tips

### Parsing Expressions

Use the **Shunting Yard Algorithm** to convert infix to postfix notation:

```csharp
// Infix: 3 + 4 * 2
// Postfix: 3 4 2 * +
// Easier to evaluate!

public class ExpressionParser
{
    public double Evaluate(string expression)
    {
        var postfix = ConvertToPostfix(expression);
        return EvaluatePostfix(postfix);
    }

    private List<string> ConvertToPostfix(string infix)
    {
        // Shunting Yard Algorithm implementation
        // ...
    }

    private double EvaluatePostfix(List<string> postfix)
    {
        var stack = new Stack<double>();

        foreach (var token in postfix)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else // Operator
            {
                double b = stack.Pop();
                double a = stack.Pop();
                double result = ApplyOperator(token, a, b);
                stack.Push(result);
            }
        }

        return stack.Pop();
    }
}
```

### Converter Interface

```csharp
public interface IUnitConverter
{
    double Convert(double value, string fromUnit, string toUnit);
    string[] GetSupportedUnits();
    string GetDescription();
}

public class LengthConverter : IUnitConverter
{
    public double Convert(double value, string fromUnit, string toUnit)
    {
        // Convert to base unit (meters) first
        double meters = fromUnit.ToLower() switch
        {
            "m" => value,
            "km" => value * 1000,
            "cm" => value / 100,
            "ft" => value * 0.3048,
            "mi" => value * 1609.34,
            _ => throw new ArgumentException($"Unknown unit: {fromUnit}")
        };

        // Convert from base unit to target
        return toUnit.ToLower() switch
        {
            "m" => meters,
            "km" => meters / 1000,
            "cm" => meters * 100,
            "ft" => meters / 0.3048,
            "mi" => meters / 1609.34,
            _ => throw new ArgumentException($"Unknown unit: {toUnit}")
        };
    }
}
```

## 🚀 Bonus Challenges

Once you complete the basic requirements, try these:

1. **Scientific Functions**: sin, cos, tan, log, ln, etc.
2. **Binary/Hex/Octal**: Convert between number bases
3. **Complex Numbers**: Support imaginary numbers
4. **Graphing**: Plot simple functions in ASCII
5. **Expression Builder**: Interactive formula builder
6. **Configuration**: Save user preferences
7. **Scripting**: Load and execute calculation scripts
8. **Performance**: Handle very large numbers (BigInteger)
9. **Statistics**: Mean, median, mode, standard deviation
10. **Financial**: Interest calculations, loan payments

## 📊 Evaluation Rubric

| Category | Weight | Criteria |
|----------|--------|----------|
| Functionality | 40% | All required features working correctly |
| Code Quality | 25% | Clean, organized, documented code |
| Error Handling | 15% | Comprehensive validation and error messages |
| Testing | 10% | Good test coverage and edge cases |
| User Experience | 10% | Clear interface, helpful feedback |

## ⏱️ Estimated Time

- **Planning & Design**: 3-4 hours
- **Phase 1 (Basic Calculator)**: 8-10 hours
- **Phase 2 (Conversions)**: 6-8 hours
- **Phase 3 (Advanced)**: 6-8 hours
- **Testing**: 4-6 hours
- **Refinement**: 3-4 hours
- **Total**: 30-40 hours over 3 weeks

## 📚 Resources

### Relevant Modules
- Module 01: C# Fundamentals
- Module 02: Object-Oriented Programming
- Module 03: .NET Core Basics

### External Resources
- [Shunting Yard Algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm)
- [Expression Parsing in C#](https://www.codeproject.com/Articles/345888/How-to-write-a-simple-interpreter-in-JavaScript)
- [Unit Conversion Formulas](https://www.unitconverters.net/)

## 🎓 Learning Outcomes

After completing this project, you should be able to:

- ✅ Design a multi-component application
- ✅ Implement complex algorithms (expression parsing)
- ✅ Create intuitive user interfaces
- ✅ Write comprehensive tests
- ✅ Handle edge cases and errors gracefully
- ✅ Apply OOP principles in practice
- ✅ Organize code into maintainable modules

---

**Ready to build?** Start by creating a design document outlining your class structure!

**Need inspiration?** Check out the reference implementation in the solutions directory after attempting on your own.

**Have questions?** Review the module lessons or consult the official C# documentation.

Good luck! 🚀
