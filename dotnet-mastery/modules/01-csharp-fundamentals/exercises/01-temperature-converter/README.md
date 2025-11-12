# Exercise 01: Temperature Converter

## 🎯 Objective

Build a temperature converter that can convert between Celsius, Fahrenheit, and Kelvin. This exercise will help you practice:

- Variables and data types
- Methods and parameters
- Control flow (switch expressions)
- Error handling
- User input validation

## 📋 Requirements

### Core Functionality

Create a `TemperatureConverter` class with the following methods:

1. **`CelsiusToFahrenheit(double celsius)`**
   - Convert Celsius to Fahrenheit
   - Formula: F = (C × 9/5) + 32
   - Return type: `double`

2. **`FahrenheitToCelsius(double fahrenheit)`**
   - Convert Fahrenheit to Celsius
   - Formula: C = (F - 32) × 5/9
   - Return type: `double`

3. **`CelsiusToKelvin(double celsius)`**
   - Convert Celsius to Kelvin
   - Formula: K = C + 273.15
   - Return type: `double`

4. **`KelvinToCelsius(double kelvin)`**
   - Convert Kelvin to Celsius
   - Formula: C = K - 273.15
   - Return type: `double`

5. **`Convert(double temperature, string fromUnit, string toUnit)`**
   - Universal converter method
   - Parameters:
     - `temperature`: The temperature value to convert
     - `fromUnit`: Source unit ("C", "F", or "K")
     - `toUnit`: Target unit ("C", "F", or "K")
   - Return type: `double`
   - Throw `ArgumentException` for invalid units

### Validation

- Temperatures below absolute zero are invalid:
  - Celsius: < -273.15°C
  - Fahrenheit: < -459.67°F
  - Kelvin: < 0 K
- Throw `ArgumentOutOfRangeException` for invalid temperatures
- Provide meaningful error messages

### Console Application

Create a console application that:
1. Displays a menu of conversion options
2. Prompts user for temperature value and units
3. Performs conversion
4. Displays result with 2 decimal places
5. Handles invalid input gracefully
6. Allows multiple conversions (loop until user exits)

## 📊 Example Interactions

```
=== Temperature Converter ===
1. Celsius to Fahrenheit
2. Fahrenheit to Celsius
3. Celsius to Kelvin
4. Kelvin to Celsius
5. Universal Converter
6. Exit

Select option: 1
Enter temperature in Celsius: 100
Result: 100.00°C = 212.00°F

Select option: 5
Enter temperature: 32
From unit (C/F/K): F
To unit (C/F/K): C
Result: 32.00°F = 0.00°C

Select option: 1
Enter temperature in Celsius: -300
Error: Temperature cannot be below absolute zero (-273.15°C)

Select option: 6
Goodbye!
```

## ✅ Acceptance Criteria

Your solution should:

1. **Pass all unit tests** (run `dotnet test` in the tests directory)
2. **Handle edge cases:**
   - Absolute zero conversions
   - Very large temperatures
   - Invalid units
3. **Use appropriate data types** (`double` for temperature values)
4. **Follow C# naming conventions** (PascalCase for methods, camelCase for parameters)
5. **Include XML documentation comments** for public methods
6. **Implement proper error handling** with specific exception types

## 🧪 Testing

Run the included unit tests:

```bash
cd tests
dotnet test
```

All 15 tests should pass.

## 💡 Hints

Need help? Check the [hints.md](hints/hints.md) file for guidance on:
- Temperature conversion formulas
- Input validation approach
- Switch expression syntax
- Exception handling patterns

## 🎓 Learning Goals

After completing this exercise, you should understand:

- How to design methods with clear responsibilities
- When to use `double` vs `decimal` (use `double` for scientific calculations)
- How to validate input and throw appropriate exceptions
- How to use switch expressions for cleaner conditional logic
- How to format output using string interpolation

## ⏱️ Estimated Time

- Reading requirements: 5 minutes
- Implementation: 30-45 minutes
- Testing and debugging: 15 minutes
- **Total: ~1 hour**

## 📚 Reference Materials

- [Lesson 02: Types and Variables](../../lessons/02-types-and-variables.md)
- [Lesson 03: Control Flow](../../lessons/03-control-flow.md)
- [Lesson 04: Methods and Parameters](../../lessons/04-methods-and-parameters.md)
- [Lesson 05: Error Handling](../../lessons/05-error-handling.md)

## 🚀 Bonus Challenges

Once you complete the basic requirements, try these enhancements:

1. **Add more units:** Rankine, Réaumur
2. **Batch conversion:** Convert a range of temperatures
3. **Table output:** Display a conversion table (e.g., Celsius 0-100 to Fahrenheit)
4. **Configuration:** Save user's preferred units
5. **Validation:** Prevent obviously wrong inputs (e.g., human body temperature > 100°C)

---

**Ready to start?** Open the `starter/` directory and begin implementing your solution!

**Stuck?** Check `hints/hints.md` for guidance.

**Want to compare?** See the `solutions/` directory after completing your attempt.
