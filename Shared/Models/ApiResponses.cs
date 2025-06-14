using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace CopilotBlazor.Shared.Models;

/// <summary>
/// Represents a keep-alive status response indicating the service is operational.
/// Used for monitoring and cold-start prevention in Azure Functions.
/// </summary>
[SwaggerSchema(
    Title = "Keep Alive Response",
    Description = "Simple status response to indicate the service is alive and operational"
)]
public record KeepAliveResponse
{
    /// <summary>
    /// Status indicator confirming the service is alive.
    /// </summary>
    /// <example>alive</example>
    [JsonPropertyName("status")]
    [SwaggerSchema("Service status indicator")]
    [Required]
    public required string Status { get; init; } = "alive";

    /// <summary>
    /// UTC timestamp when the keep-alive response was generated.
    /// </summary>
    /// <example>2025-06-14T10:30:00Z</example>
    [JsonPropertyName("timestamp")]
    [SwaggerSchema("UTC timestamp when response was generated")]
    [Required]
    public required DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Optional additional metadata about the service state.
    /// </summary>
    [JsonPropertyName("metadata")]
    [SwaggerSchema("Optional service metadata")]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Represents a standardized API error response following Azure API Guidelines.
/// Provides consistent error reporting across all endpoints.
/// </summary>
[SwaggerSchema(
    Title = "API Error Response",
    Description = "Standardized error response format following Azure API Guidelines"
)]
public record ApiErrorResponse
{
    /// <summary>
    /// Error response container following Azure error format.
    /// </summary>
    [JsonPropertyName("error")]
    [SwaggerSchema("Error details container")]
    [Required]
    public required ApiError Error { get; init; }
}

/// <summary>
/// Detailed error information following Azure API error format.
/// </summary>
[SwaggerSchema(
    Title = "API Error",
    Description = "Detailed error information following Azure standards"
)]
public record ApiError
{
    /// <summary>
    /// A service-defined error code that provides a more specific error than the HTTP status code.
    /// </summary>
    /// <example>ValidationFailed</example>
    [JsonPropertyName("code")]
    [SwaggerSchema("Service-specific error code")]
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Code { get; init; }

    /// <summary>
    /// A human-readable description of the error.
    /// </summary>
    /// <example>The request parameters are invalid</example>
    [JsonPropertyName("message")]
    [SwaggerSchema("Human-readable error description")]
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string Message { get; init; }

    /// <summary>
    /// The target of the error (optional).
    /// </summary>
    /// <example>temperatureC</example>
    [JsonPropertyName("target")]
    [SwaggerSchema("The target of the error")]
    [StringLength(100)]
    public string? Target { get; init; }

    /// <summary>
    /// Additional error details (optional).
    /// </summary>
    [JsonPropertyName("details")]
    [SwaggerSchema("Additional error details")]
    public IEnumerable<ApiErrorDetail>? Details { get; init; }

    /// <summary>
    /// Inner error with more specific information (optional).
    /// </summary>
    [JsonPropertyName("innererror")]
    [SwaggerSchema("Inner error with additional context")]
    public ApiInnerError? InnerError { get; init; }
}

/// <summary>
/// Additional error detail information.
/// </summary>
[SwaggerSchema(
    Title = "API Error Detail",
    Description = "Additional specific error detail"
)]
public record ApiErrorDetail
{
    /// <summary>
    /// Specific error code for this detail.
    /// </summary>
    [JsonPropertyName("code")]
    [SwaggerSchema("Specific error code")]
    [Required]
    public required string Code { get; init; }

    /// <summary>
    /// Error message for this detail.
    /// </summary>
    [JsonPropertyName("message")]
    [SwaggerSchema("Error message")]
    [Required]
    public required string Message { get; init; }

    /// <summary>
    /// Target of this specific error.
    /// </summary>
    [JsonPropertyName("target")]
    [SwaggerSchema("Target of the error")]
    public string? Target { get; init; }
}

/// <summary>
/// Inner error providing additional technical context.
/// </summary>
[SwaggerSchema(
    Title = "API Inner Error",
    Description = "Inner error with technical details for debugging"
)]
public record ApiInnerError
{
    /// <summary>
    /// More specific error code.
    /// </summary>
    [JsonPropertyName("code")]
    [SwaggerSchema("Specific inner error code")]
    public string? Code { get; init; }

    /// <summary>
    /// Technical error message.
    /// </summary>
    [JsonPropertyName("message")]
    [SwaggerSchema("Technical error message")]
    public string? Message { get; init; }

    /// <summary>
    /// Nested inner error for additional context.
    /// </summary>
    [JsonPropertyName("innererror")]
    [SwaggerSchema("Nested inner error")]
    public ApiInnerError? InnerError { get; init; }
}
