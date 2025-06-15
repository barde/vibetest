using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CopilotBlazor.Shared.Models;

/// <summary>
/// Represents a weather forecast for a specific date with temperature and conditions.
/// Follows Azure API Guidelines for consistent data representation.
/// </summary>
public record WeatherForecast
{    /// <summary>
    /// The date for which the weather forecast is provided.
    /// </summary>
    /// <example>2025-06-14</example>
    [JsonPropertyName("date")]
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Temperature in degrees Celsius.
    /// </summary>
    /// <example>22</example>
    [JsonPropertyName("temperatureC")]
    [Range(-50, 60, ErrorMessage = "Temperature must be between -50°C and 60°C")]
    public int TemperatureC { get; init; }

    /// <summary>
    /// Temperature in degrees Fahrenheit (automatically calculated from Celsius).
    /// </summary>
    /// <example>72</example>
    [JsonPropertyName("temperatureF")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Brief description of the weather conditions.
    /// </summary>
    /// <example>Sunny</example>
    [JsonPropertyName("summary")]
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
