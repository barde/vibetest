using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#if !BROWSER_WASM
using Swashbuckle.AspNetCore.Annotations;
#endif

namespace CopilotBlazor.Shared.Models;

/// <summary>
/// Represents a weather forecast for a specific date with temperature and conditions.
/// Follows Azure API Guidelines for consistent data representation.
/// </summary>
#if !BROWSER_WASM
[SwaggerSchema(
    Title = "Weather Forecast",
    Description = "Weather forecast data including temperature in both Celsius and Fahrenheit, date, and weather summary"
)]
#endif
public record WeatherForecast
{    /// <summary>
    /// The date for which the weather forecast is provided.
    /// </summary>
    /// <example>2025-06-14</example>
    [JsonPropertyName("date")]
#if !BROWSER_WASM
    [SwaggerSchema("Date for the weather forecast in ISO 8601 format (YYYY-MM-DD)")]
#endif
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Temperature in degrees Celsius.
    /// </summary>
    /// <example>22</example>
    [JsonPropertyName("temperatureC")]
#if !BROWSER_WASM
    [SwaggerSchema("Temperature in degrees Celsius", Format = "int32")]
#endif
    [Range(-50, 60, ErrorMessage = "Temperature must be between -50°C and 60°C")]
    public int TemperatureC { get; init; }

    /// <summary>
    /// Temperature in degrees Fahrenheit (automatically calculated from Celsius).
    /// </summary>
    /// <example>72</example>
    [JsonPropertyName("temperatureF")]
#if !BROWSER_WASM
    [SwaggerSchema("Temperature in degrees Fahrenheit (calculated from Celsius)", Format = "int32")]
#endif
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Brief description of the weather conditions.
    /// </summary>
    /// <example>Sunny</example>
    [JsonPropertyName("summary")]
#if !BROWSER_WASM
    [SwaggerSchema("Brief description of weather conditions")]
#endif
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Summary must be between 3 and 50 characters")]
    public string? Summary { get; init; }

    /// <summary>
    /// Creates a new WeatherForecast instance.
    /// </summary>
    /// <param name="date">The forecast date</param>
    /// <param name="temperatureC">Temperature in Celsius</param>
    /// <param name="summary">Weather summary description</param>
    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    /// <summary>
    /// Parameterless constructor for JSON deserialization.
    /// </summary>
    public WeatherForecast() { }
}
