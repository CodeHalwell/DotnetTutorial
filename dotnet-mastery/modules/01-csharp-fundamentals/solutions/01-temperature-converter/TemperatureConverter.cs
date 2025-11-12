namespace TemperatureConverter;

/// <summary>
/// Provides methods for converting temperatures between Celsius, Fahrenheit, and Kelvin.
/// All conversion methods validate that input temperatures are above absolute zero.
/// </summary>
public class TemperatureConverter
{
    // Absolute zero constants for validation
    private const double AbsoluteZeroCelsius = -273.15;
    private const double AbsoluteZeroFahrenheit = -459.67;
    private const double AbsoluteZeroKelvin = 0.0;

    /// <summary>
    /// Converts temperature from Celsius to Fahrenheit.
    /// </summary>
    /// <param name="celsius">Temperature in Celsius</param>
    /// <returns>Temperature in Fahrenheit</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero (-273.15°C)
    /// </exception>
    /// <remarks>
    /// Formula: F = (C × 9/5) + 32
    ///
    /// Why this formula?
    /// - The freezing point of water is 0°C and 32°F
    /// - The boiling point of water is 100°C and 212°F
    /// - The difference is 100°C and 180°F
    /// - Ratio: 180/100 = 9/5
    /// - Offset: 32°F
    ///
    /// Performance: O(1) - constant time operation
    /// </remarks>
    public double CelsiusToFahrenheit(double celsius)
    {
        ValidateTemperature(celsius, "C");
        return (celsius * 9.0 / 5.0) + 32.0;
    }

    /// <summary>
    /// Converts temperature from Fahrenheit to Celsius.
    /// </summary>
    /// <param name="fahrenheit">Temperature in Fahrenheit</param>
    /// <returns>Temperature in Celsius</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero (-459.67°F)
    /// </exception>
    /// <remarks>
    /// Formula: C = (F - 32) × 5/9
    ///
    /// This is the inverse of CelsiusToFahrenheit:
    /// - Subtract the offset (32)
    /// - Multiply by the inverse ratio (5/9)
    ///
    /// Performance: O(1) - constant time operation
    /// </remarks>
    public double FahrenheitToCelsius(double fahrenheit)
    {
        ValidateTemperature(fahrenheit, "F");
        return (fahrenheit - 32.0) * 5.0 / 9.0;
    }

    /// <summary>
    /// Converts temperature from Celsius to Kelvin.
    /// </summary>
    /// <param name="celsius">Temperature in Celsius</param>
    /// <returns>Temperature in Kelvin</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero (-273.15°C)
    /// </exception>
    /// <remarks>
    /// Formula: K = C + 273.15
    ///
    /// Kelvin is an absolute temperature scale where:
    /// - 0 K is absolute zero (the coldest possible temperature)
    /// - Water freezes at 273.15 K (0°C)
    /// - Water boils at 373.15 K (100°C)
    ///
    /// The Kelvin scale has the same degree size as Celsius,
    /// just shifted by 273.15 degrees.
    ///
    /// Performance: O(1) - constant time operation
    /// </remarks>
    public double CelsiusToKelvin(double celsius)
    {
        ValidateTemperature(celsius, "C");
        return celsius + 273.15;
    }

    /// <summary>
    /// Converts temperature from Kelvin to Celsius.
    /// </summary>
    /// <param name="kelvin">Temperature in Kelvin</param>
    /// <returns>Temperature in Celsius</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero (0 K)
    /// </exception>
    /// <remarks>
    /// Formula: C = K - 273.15
    ///
    /// This is the inverse of CelsiusToKelvin.
    ///
    /// Performance: O(1) - constant time operation
    /// </remarks>
    public double KelvinToCelsius(double kelvin)
    {
        ValidateTemperature(kelvin, "K");
        return kelvin - 273.15;
    }

    /// <summary>
    /// Converts temperature between any two supported units.
    /// </summary>
    /// <param name="temperature">The temperature value to convert</param>
    /// <param name="fromUnit">Source unit: "C" (Celsius), "F" (Fahrenheit), or "K" (Kelvin)</param>
    /// <param name="toUnit">Target unit: "C" (Celsius), "F" (Fahrenheit), or "K" (Kelvin)</param>
    /// <returns>Converted temperature value</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when fromUnit or toUnit is not a valid unit code
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero for the given unit
    /// </exception>
    /// <remarks>
    /// This method uses a two-step conversion strategy:
    /// 1. Convert from source unit to Celsius (universal intermediate)
    /// 2. Convert from Celsius to target unit
    ///
    /// Alternative approach would be direct conversion for each pair,
    /// but that would require 6 cases (C→F, F→C, C→K, K→C, F→K, K→F)
    /// instead of 4 (3 to Celsius + 3 from Celsius).
    ///
    /// Performance: O(1) - at most two conversions
    /// Memory: O(1) - no allocation
    ///
    /// Design decision: Using string for units instead of enum to keep
    /// the API simple for beginners. In production code, prefer enum.
    /// </remarks>
    public double Convert(double temperature, string fromUnit, string toUnit)
    {
        // Normalize unit strings (case-insensitive, trim whitespace)
        fromUnit = fromUnit?.Trim().ToUpperInvariant() ?? "";
        toUnit = toUnit?.Trim().ToUpperInvariant() ?? "";

        // Validate units
        if (!IsValidUnit(fromUnit))
        {
            throw new ArgumentException(
                $"Invalid source unit '{fromUnit}'. Valid units are: C, F, K",
                nameof(fromUnit));
        }

        if (!IsValidUnit(toUnit))
        {
            throw new ArgumentException(
                $"Invalid target unit '{toUnit}'. Valid units are: C, F, K",
                nameof(toUnit));
        }

        // Validate temperature for source unit
        ValidateTemperature(temperature, fromUnit);

        // If units are the same, no conversion needed
        if (fromUnit == toUnit)
        {
            return temperature;
        }

        // Convert to Celsius as intermediate step
        // This avoids needing 6 direct conversion methods (N×N-N combinations)
        double celsius = fromUnit switch
        {
            "C" => temperature,
            "F" => FahrenheitToCelsius(temperature),
            "K" => KelvinToCelsius(temperature),
            _ => throw new InvalidOperationException("This should never happen due to validation above")
        };

        // Convert from Celsius to target unit
        return toUnit switch
        {
            "C" => celsius,
            "F" => CelsiusToFahrenheit(celsius),
            "K" => CelsiusToKelvin(celsius),
            _ => throw new InvalidOperationException("This should never happen due to validation above")
        };
    }

    /// <summary>
    /// Validates that a unit code is valid.
    /// </summary>
    /// <param name="unit">The unit code to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    /// <remarks>
    /// Private helper method to encapsulate validation logic.
    /// Uses pattern matching for concise code.
    ///
    /// Design note: In production code, consider using an enum:
    /// public enum TemperatureUnit { Celsius, Fahrenheit, Kelvin }
    /// This provides compile-time type safety.
    /// </remarks>
    private static bool IsValidUnit(string unit)
    {
        return unit is "C" or "F" or "K";  // C# 9+ pattern matching
    }

    /// <summary>
    /// Validates that a temperature is above absolute zero for the given unit.
    /// </summary>
    /// <param name="temperature">The temperature to validate</param>
    /// <param name="unit">The temperature unit ("C", "F", or "K")</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when temperature is below absolute zero
    /// </exception>
    /// <remarks>
    /// Absolute zero is the coldest possible temperature in the universe.
    /// Nothing can be colder than absolute zero because it represents the
    /// state where molecules have minimal possible energy.
    ///
    /// Values:
    /// - Celsius: -273.15°C
    /// - Fahrenheit: -459.67°F
    /// - Kelvin: 0 K
    ///
    /// Real-world note: The coldest temperature ever achieved in a lab
    /// is about 0.0000000001 K (100 picokelvins), still above absolute zero!
    /// </remarks>
    private static void ValidateTemperature(double temperature, string unit)
    {
        var (absoluteZero, unitSymbol) = unit switch
        {
            "C" => (AbsoluteZeroCelsius, "°C"),
            "F" => (AbsoluteZeroFahrenheit, "°F"),
            "K" => (AbsoluteZeroKelvin, " K"),
            _ => throw new ArgumentException($"Invalid unit: {unit}", nameof(unit))
        };

        if (temperature < absoluteZero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(temperature),
                temperature,
                $"Temperature cannot be below absolute zero ({absoluteZero:F2}{unitSymbol})");
        }
    }
}
