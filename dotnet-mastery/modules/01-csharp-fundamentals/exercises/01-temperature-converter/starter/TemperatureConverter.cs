namespace TemperatureConverter;

/// <summary>
/// Provides methods for converting temperatures between Celsius, Fahrenheit, and Kelvin.
/// </summary>
public class TemperatureConverter
{
    // TODO: Implement CelsiusToFahrenheit method
    // Formula: F = (C × 9/5) + 32
    public double CelsiusToFahrenheit(double celsius)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement FahrenheitToCelsius method
    // Formula: C = (F - 32) × 5/9
    public double FahrenheitToCelsius(double fahrenheit)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement CelsiusToKelvin method
    // Formula: K = C + 273.15
    public double CelsiusToKelvin(double celsius)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement KelvinToCelsius method
    // Formula: C = K - 273.15
    public double KelvinToCelsius(double kelvin)
    {
        throw new NotImplementedException();
    }

    // TODO: Implement universal Convert method
    // Use switch expression to handle different unit combinations
    // Validate units and throw ArgumentException for invalid units
    // Validate temperature is above absolute zero
    public double Convert(double temperature, string fromUnit, string toUnit)
    {
        throw new NotImplementedException();
    }

    // HINT: Consider adding private validation methods
    // private void ValidateTemperature(double temperature, string unit) { }
}
