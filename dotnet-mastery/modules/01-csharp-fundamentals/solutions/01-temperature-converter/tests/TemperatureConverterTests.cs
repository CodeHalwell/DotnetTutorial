using Xunit;
using TemperatureConverter;

namespace TemperatureConverter.Tests;

/// <summary>
/// Comprehensive test suite for TemperatureConverter class.
/// Demonstrates unit testing best practices:
/// - Arrange-Act-Assert pattern
/// - Theory tests for multiple inputs
/// - Edge case coverage
/// - Exception testing
/// </summary>
public class TemperatureConverterTests
{
    private readonly TemperatureConverter _converter;

    public TemperatureConverterTests()
    {
        _converter = new TemperatureConverter();
    }

    #region CelsiusToFahrenheit Tests

    [Theory]
    [InlineData(0, 32)]           // Freezing point
    [InlineData(100, 212)]        // Boiling point
    [InlineData(-40, -40)]        // Equal point
    [InlineData(37, 98.6)]        // Body temperature
    [InlineData(-273.15, -459.67)] // Absolute zero
    public void CelsiusToFahrenheit_ValidInputs_ReturnsCorrectValue(double celsius, double expectedFahrenheit)
    {
        // Act
        double actual = _converter.CelsiusToFahrenheit(celsius);

        // Assert
        Assert.Equal(expectedFahrenheit, actual, precision: 2);
    }

    [Fact]
    public void CelsiusToFahrenheit_BelowAbsoluteZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double belowAbsoluteZero = -300;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => _converter.CelsiusToFahrenheit(belowAbsoluteZero));

        Assert.Contains("absolute zero", exception.Message.ToLower());
    }

    #endregion

    #region FahrenheitToCelsius Tests

    [Theory]
    [InlineData(32, 0)]           // Freezing point
    [InlineData(212, 100)]        // Boiling point
    [InlineData(-40, -40)]        // Equal point
    [InlineData(98.6, 37)]        // Body temperature
    [InlineData(-459.67, -273.15)] // Absolute zero
    public void FahrenheitToCelsius_ValidInputs_ReturnsCorrectValue(double fahrenheit, double expectedCelsius)
    {
        // Act
        double actual = _converter.FahrenheitToCelsius(fahrenheit);

        // Assert
        Assert.Equal(expectedCelsius, actual, precision: 2);
    }

    [Fact]
    public void FahrenheitToCelsius_BelowAbsoluteZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double belowAbsoluteZero = -500;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _converter.FahrenheitToCelsius(belowAbsoluteZero));
    }

    #endregion

    #region CelsiusToKelvin Tests

    [Theory]
    [InlineData(0, 273.15)]       // Freezing point
    [InlineData(100, 373.15)]     // Boiling point
    [InlineData(-273.15, 0)]      // Absolute zero
    [InlineData(25, 298.15)]      // Room temperature
    public void CelsiusToKelvin_ValidInputs_ReturnsCorrectValue(double celsius, double expectedKelvin)
    {
        // Act
        double actual = _converter.CelsiusToKelvin(celsius);

        // Assert
        Assert.Equal(expectedKelvin, actual, precision: 2);
    }

    [Fact]
    public void CelsiusToKelvin_BelowAbsoluteZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double belowAbsoluteZero = -300;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _converter.CelsiusToKelvin(belowAbsoluteZero));
    }

    #endregion

    #region KelvinToCelsius Tests

    [Theory]
    [InlineData(273.15, 0)]       // Freezing point
    [InlineData(373.15, 100)]     // Boiling point
    [InlineData(0, -273.15)]      // Absolute zero
    [InlineData(298.15, 25)]      // Room temperature
    public void KelvinToCelsius_ValidInputs_ReturnsCorrectValue(double kelvin, double expectedCelsius)
    {
        // Act
        double actual = _converter.KelvinToCelsius(kelvin);

        // Assert
        Assert.Equal(expectedCelsius, actual, precision: 2);
    }

    [Fact]
    public void KelvinToCelsius_BelowAbsoluteZero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double belowAbsoluteZero = -1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _converter.KelvinToCelsius(belowAbsoluteZero));
    }

    #endregion

    #region Convert Universal Method Tests

    [Theory]
    [InlineData(0, "C", "F", 32)]
    [InlineData(100, "C", "F", 212)]
    [InlineData(32, "F", "C", 0)]
    [InlineData(0, "C", "K", 273.15)]
    [InlineData(273.15, "K", "C", 0)]
    public void Convert_ValidConversions_ReturnsCorrectValue(
        double temperature, string fromUnit, string toUnit, double expected)
    {
        // Act
        double actual = _converter.Convert(temperature, fromUnit, toUnit);

        // Assert
        Assert.Equal(expected, actual, precision: 2);
    }

    [Theory]
    [InlineData("C", "C")]
    [InlineData("F", "F")]
    [InlineData("K", "K")]
    public void Convert_SameUnit_ReturnsOriginalValue(string unit1, string unit2)
    {
        // Arrange
        double temperature = 100;

        // Act
        double actual = _converter.Convert(temperature, unit1, unit2);

        // Assert
        Assert.Equal(temperature, actual);
    }

    [Theory]
    [InlineData("X", "C")]
    [InlineData("C", "Y")]
    [InlineData("celsius", "fahrenheit")]
    public void Convert_InvalidUnits_ThrowsArgumentException(string fromUnit, string toUnit)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _converter.Convert(100, fromUnit, toUnit));
    }

    [Theory]
    [InlineData("c", "f")]  // Lowercase
    [InlineData("C ", " F")] // With spaces
    public void Convert_CaseInsensitiveUnits_WorksCorrectly(string fromUnit, string toUnit)
    {
        // Arrange
        double celsius = 0;

        // Act
        double actual = _converter.Convert(celsius, fromUnit, toUnit);

        // Assert
        Assert.Equal(32, actual, precision: 2);
    }

    [Fact]
    public void Convert_NullFromUnit_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _converter.Convert(100, null, "C"));
    }

    [Fact]
    public void Convert_NullToUnit_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => _converter.Convert(100, "C", null));
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public void AllConversions_AtAbsoluteZero_AreConsistent()
    {
        // Absolute zero in different units
        double celsiusZero = -273.15;
        double fahrenheitZero = -459.67;
        double kelvinZero = 0;

        // Convert Celsius absolute zero
        var cToF = _converter.CelsiusToFahrenheit(celsiusZero);
        var cToK = _converter.CelsiusToKelvin(celsiusZero);

        // Convert Fahrenheit absolute zero
        var fToC = _converter.FahrenheitToCelsius(fahrenheitZero);
        var fToK = _converter.Convert(fahrenheitZero, "F", "K");

        // Convert Kelvin absolute zero
        var kToC = _converter.KelvinToCelsius(kelvinZero);
        var kToF = _converter.Convert(kelvinZero, "K", "F");

        // All should be consistent
        Assert.Equal(fahrenheitZero, cToF, precision: 1);
        Assert.Equal(kelvinZero, cToK, precision: 1);
        Assert.Equal(celsiusZero, fToC, precision: 1);
        Assert.Equal(celsiusZero, kToC, precision: 1);
    }

    [Fact]
    public void RoundTrip_Conversions_ReturnOriginalValue()
    {
        // Arrange
        double originalCelsius = 25;

        // Act: C -> F -> C
        var fahrenheit = _converter.CelsiusToFahrenheit(originalCelsius);
        var backToCelsius = _converter.FahrenheitToCelsius(fahrenheit);

        // Assert
        Assert.Equal(originalCelsius, backToCelsius, precision: 10);
    }

    [Theory]
    [InlineData(1000000)]  // Very high temperature
    [InlineData(0.0001)]   // Very low positive temperature
    public void Convert_ExtremeValidValues_HandlesCorrectly(double temperature)
    {
        // Act
        var fahrenheit = _converter.CelsiusToFahrenheit(temperature);
        var kelvin = _converter.CelsiusToKelvin(temperature);

        // Assert - should not throw, should be reasonable values
        Assert.True(fahrenheit > temperature); // F is always higher than C for positive values
        Assert.True(kelvin > temperature);     // K is always higher than C
    }

    #endregion

    #region Performance Tests (Optional - for demonstration)

    [Fact]
    public void Convert_PerformanceTest_HandlesLargeVolume()
    {
        // Arrange
        const int iterations = 100_000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            _converter.Convert(i % 100, "C", "F");
        }

        stopwatch.Stop();

        // Assert - should complete in reasonable time (< 100ms)
        Assert.True(stopwatch.ElapsedMilliseconds < 100,
            $"Performance test failed: {stopwatch.ElapsedMilliseconds}ms for {iterations} iterations");
    }

    #endregion
}
