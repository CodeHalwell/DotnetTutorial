using TemperatureConverter;

// Main program: Interactive temperature converter
var converter = new TemperatureConverter.TemperatureConverter();
bool running = true;

Console.WriteLine("╔═══════════════════════════════════╗");
Console.WriteLine("║   Temperature Converter v1.0      ║");
Console.WriteLine("╚═══════════════════════════════════╝");
Console.WriteLine();

while (running)
{
    DisplayMenu();

    // Read user choice with validation
    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("❌ Invalid input. Please enter a number.");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case 1:
                ConvertCelsiusToFahrenheit();
                break;
            case 2:
                ConvertFahrenheitToCelsius();
                break;
            case 3:
                ConvertCelsiusToKelvin();
                break;
            case 4:
                ConvertKelvinToCelsius();
                break;
            case 5:
                UniversalConverter();
                break;
            case 6:
                running = false;
                Console.WriteLine("👋 Thank you for using Temperature Converter!");
                break;
            default:
                Console.WriteLine("❌ Invalid option. Please select 1-6.");
                break;
        }
    }
    catch (ArgumentOutOfRangeException ex)
    {
        // Handle temperatures below absolute zero
        Console.WriteLine($"❌ {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        // Handle invalid units
        Console.WriteLine($"❌ {ex.Message}");
    }
    catch (Exception ex)
    {
        // Catch-all for unexpected errors
        Console.WriteLine($"❌ Unexpected error: {ex.Message}");
    }

    Console.WriteLine();
}

// Display main menu
void DisplayMenu()
{
    Console.WriteLine("═══ Main Menu ═══");
    Console.WriteLine("1. Celsius → Fahrenheit");
    Console.WriteLine("2. Fahrenheit → Celsius");
    Console.WriteLine("3. Celsius → Kelvin");
    Console.WriteLine("4. Kelvin → Celsius");
    Console.WriteLine("5. Universal Converter");
    Console.WriteLine("6. Exit");
    Console.Write("\nSelect option: ");
}

// Convert Celsius to Fahrenheit
void ConvertCelsiusToFahrenheit()
{
    double celsius = PromptForTemperature("Celsius");
    double fahrenheit = converter.CelsiusToFahrenheit(celsius);
    Console.WriteLine($"✓ {celsius:F2}°C = {fahrenheit:F2}°F");
}

// Convert Fahrenheit to Celsius
void ConvertFahrenheitToCelsius()
{
    double fahrenheit = PromptForTemperature("Fahrenheit");
    double celsius = converter.FahrenheitToCelsius(fahrenheit);
    Console.WriteLine($"✓ {fahrenheit:F2}°F = {celsius:F2}°C");
}

// Convert Celsius to Kelvin
void ConvertCelsiusToKelvin()
{
    double celsius = PromptForTemperature("Celsius");
    double kelvin = converter.CelsiusToKelvin(celsius);
    Console.WriteLine($"✓ {celsius:F2}°C = {kelvin:F2} K");
}

// Convert Kelvin to Celsius
void ConvertKelvinToCelsius()
{
    double kelvin = PromptForTemperature("Kelvin");
    double celsius = converter.KelvinToCelsius(kelvin);
    Console.WriteLine($"✓ {kelvin:F2} K = {celsius:F2}°C");
}

// Universal converter (any unit to any unit)
void UniversalConverter()
{
    double temperature = PromptForTemperature("any unit");

    Console.Write("From unit (C/F/K): ");
    string fromUnit = Console.ReadLine()?.Trim() ?? "";

    Console.Write("To unit (C/F/K): ");
    string toUnit = Console.ReadLine()?.Trim() ?? "";

    double result = converter.Convert(temperature, fromUnit, toUnit);

    // Format unit symbols for display
    string fromSymbol = GetUnitSymbol(fromUnit);
    string toSymbol = GetUnitSymbol(toUnit);

    Console.WriteLine($"✓ {temperature:F2}{fromSymbol} = {result:F2}{toSymbol}");
}

// Prompt user for temperature value with validation
double PromptForTemperature(string unit)
{
    while (true)
    {
        Console.Write($"Enter temperature in {unit}: ");
        string input = Console.ReadLine() ?? "";

        if (double.TryParse(input, out double temperature))
        {
            return temperature;
        }

        Console.WriteLine("❌ Invalid input. Please enter a numeric value.");
    }
}

// Get display symbol for unit code
string GetUnitSymbol(string unit)
{
    return unit.ToUpperInvariant() switch
    {
        "C" => "°C",
        "F" => "°F",
        "K" => " K",
        _ => $" {unit}"
    };
}
