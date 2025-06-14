using System.Text.Json;
using CopilotBlazor.Shared.Models;

namespace CopilotBlazor.Shared.Extensions;

/// <summary>
/// Extension methods for JSON serialization with consistent options.
/// Provides standardized JSON handling across the application following Azure best practices.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Default JSON serializer options following Azure API Guidelines.
    /// Uses camelCase property naming, includes indentation for readability,
    /// and handles DateOnly/TimeOnly types properly.
    /// </summary>
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = 
        {
            new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Compact JSON serializer options for production APIs.
    /// Uses camelCase property naming without indentation for smaller payloads.
    /// </summary>
    public static readonly JsonSerializerOptions CompactOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = 
        {
            new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Serializes an object to JSON string using default options.
    /// </summary>
    /// <typeparam name="T">Type of object to serialize</typeparam>
    /// <param name="obj">Object to serialize</param>
    /// <param name="compact">Use compact formatting (default: false)</param>
    /// <returns>JSON string representation</returns>
    public static string ToJson<T>(this T obj, bool compact = false)
    {
        var options = compact ? CompactOptions : DefaultOptions;
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Deserializes JSON string to specified type using default options.
    /// </summary>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="json">JSON string to deserialize</param>
    /// <returns>Deserialized object</returns>
    /// <exception cref="JsonException">Thrown when JSON is invalid</exception>
    public static T? FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultOptions);
    }

    /// <summary>
    /// Safely deserializes JSON string to specified type with error handling.
    /// </summary>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="json">JSON string to deserialize</param>
    /// <returns>Tuple containing success flag and result</returns>
    public static (bool Success, T? Result, string? Error) TryFromJson<T>(this string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
            return (true, result, null);
        }
        catch (JsonException ex)
        {
            return (false, default, ex.Message);
        }
    }
}

/// <summary>
/// Extension methods for HTTP responses and error handling.
/// Provides standardized response creation following Azure API Guidelines.
/// </summary>
public static class ResponseExtensions
{
    /// <summary>
    /// Creates a standardized error response following Azure API error format.
    /// </summary>
    /// <param name="code">Service-specific error code</param>
    /// <param name="message">Human-readable error description</param>
    /// <param name="target">Optional target of the error</param>
    /// <param name="details">Optional additional error details</param>
    /// <returns>Standardized API error response</returns>
    public static ApiErrorResponse CreateErrorResponse(
        string code,
        string message,
        string? target = null,
        IEnumerable<ApiErrorDetail>? details = null)
    {
        return new ApiErrorResponse
        {
            Error = new ApiError
            {
                Code = code,
                Message = message,
                Target = target,
                Details = details
            }
        };
    }

    /// <summary>
    /// Creates a validation error response with multiple validation failures.
    /// </summary>
    /// <param name="validationErrors">Dictionary of field names and their validation errors</param>
    /// <returns>Standardized validation error response</returns>
    public static ApiErrorResponse CreateValidationErrorResponse(
        Dictionary<string, string[]> validationErrors)
    {
        var details = validationErrors.SelectMany(kvp =>
            kvp.Value.Select(error => new ApiErrorDetail
            {
                Code = "ValidationFailed",
                Message = error,
                Target = kvp.Key
            }));

        return new ApiErrorResponse
        {
            Error = new ApiError
            {
                Code = "ValidationFailed",
                Message = "One or more validation errors occurred.",
                Details = details
            }
        };
    }

    /// <summary>
    /// Creates a keep-alive response with current timestamp.
    /// </summary>
    /// <param name="metadata">Optional additional metadata</param>
    /// <returns>Keep-alive response</returns>
    public static KeepAliveResponse CreateKeepAliveResponse(
        Dictionary<string, object>? metadata = null)
    {
        return new KeepAliveResponse
        {
            Status = "alive",
            Timestamp = DateTime.UtcNow,
            Metadata = metadata
        };
    }
}

/// <summary>
/// Extension methods for DateTime handling in APIs.
/// Provides consistent datetime formatting following ISO 8601 standards.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Formats DateTime to ISO 8601 string (UTC).
    /// </summary>
    /// <param name="dateTime">DateTime to format</param>
    /// <returns>ISO 8601 formatted string</returns>
    public static string ToIso8601String(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    /// <summary>
    /// Formats DateOnly to ISO 8601 date string.
    /// </summary>
    /// <param name="date">DateOnly to format</param>
    /// <returns>ISO 8601 date string (YYYY-MM-DD)</returns>
    public static string ToIso8601String(this DateOnly date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Converts UTC DateTime to specified timezone.
    /// </summary>
    /// <param name="utcDateTime">UTC DateTime to convert</param>
    /// <param name="timeZoneId">Target timezone ID</param>
    /// <returns>DateTime in specified timezone</returns>
    public static DateTime ToTimeZone(this DateTime utcDateTime, string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
    }
}

/// <summary>
/// Extension methods for validation and data integrity.
/// Provides common validation patterns used across the API.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates that a temperature value is within reasonable bounds.
    /// </summary>
    /// <param name="temperatureC">Temperature in Celsius</param>
    /// <returns>True if temperature is valid</returns>
    public static bool IsValidTemperature(this int temperatureC)
    {
        return temperatureC >= -50 && temperatureC <= 60;
    }

    /// <summary>
    /// Validates that a string is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">String to validate</param>
    /// <returns>True if string has content</returns>
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Validates that a collection is not null or empty.
    /// </summary>
    /// <typeparam name="T">Type of collection items</typeparam>
    /// <param name="collection">Collection to validate</param>
    /// <returns>True if collection has items</returns>
    public static bool HasItems<T>(this IEnumerable<T>? collection)
    {
        return collection?.Any() == true;
    }
}
