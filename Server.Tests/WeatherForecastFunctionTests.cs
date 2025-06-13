using Microsoft.Extensions.Logging;
using Moq;
using Server.Functions;

namespace Server.Tests;

public class WeatherForecastFunctionTests
{
    private readonly Mock<ILogger<WeatherForecastFunction>> _mockLogger;
    private readonly WeatherForecastFunction _function;

    public WeatherForecastFunctionTests()
    {
        _mockLogger = new Mock<ILogger<WeatherForecastFunction>>();
        _function = new WeatherForecastFunction(_mockLogger.Object);
    }

    [Fact]
    public void WeatherForecast_HasCorrectProperties()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var temperatureC = 25;
        var summary = "Warm";

        // Act
        var forecast = new WeatherForecast(date, temperatureC, summary);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(temperatureC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
        Assert.Equal(76, forecast.TemperatureF); // 25C using the formula: 32 + (int)(25 / 0.5556) = 76
    }

    [Fact]
    public void WeatherForecast_TemperatureConversion_IsCorrect()
    {
        // Test various temperature conversions using the actual formula: 32 + (int)(C / 0.5556)
        var testCases = new[]
        {
            new { Celsius = 0, ExpectedFahrenheit = 32 },    // 32 + (int)(0 / 0.5556) = 32 + 0 = 32
            new { Celsius = 100, ExpectedFahrenheit = 211 }, // 32 + (int)(100 / 0.5556) = 32 + 179 = 211
            new { Celsius = -40, ExpectedFahrenheit = -39 }, // 32 + (int)(-40 / 0.5556) = 32 + (-71) = -39
            new { Celsius = 20, ExpectedFahrenheit = 67 }    // 32 + (int)(20 / 0.5556) = 32 + 35 = 67
        };

        foreach (var testCase in testCases)
        {
            // Arrange & Act
            var forecast = new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now),
                testCase.Celsius,
                "Test");

            // Assert
            Assert.Equal(testCase.ExpectedFahrenheit, forecast.TemperatureF);
        }
    }

    [Fact]
    public void WeatherForecastFunction_CanBeInstantiated()
    {
        // Arrange & Act
        var function = new WeatherForecastFunction(_mockLogger.Object);

        // Assert
        Assert.NotNull(function);
    }
}