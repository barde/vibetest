using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace CopilotBlazor.Shared.Models;

/// <summary>
/// Represents a keep-alive status response indicating the service is operational.
/// Used for monitoring and cold-start prevention in Azure Functions.
/// </summary>
public record KeepAliveResponse
{
    /// <summary>
    /// Status indicator confirming the service is alive.
    /// </summary>
    /// <example>alive</example>
    [JsonPropertyName("status")]
    [Required]
    public required string Status { get; init; } = "alive";

    /// <summary>
    /// UTC timestamp when the keep-alive response was generated.
    /// </summary>
    /// <example>2025-06-14T10:30:00Z</example>
    [JsonPropertyName("timestamp")]
    [Required]
    public required DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Optional additional metadata about the service state.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Represents a standardized API error response following Azure API Guidelines.
/// Provides consistent error reporting across all endpoints.
/// </summary>
public record ApiErrorResponse
{
    /// <summary>
    /// Error response container following Azure error format.
    /// </summary>
    [JsonPropertyName("error")]
    [Required]
    public required ApiError Error { get; init; }
}

/// <summary>
/// Detailed error information following Azure API error format.
/// </summary>
public record ApiError
{
    /// <summary>
    /// A service-defined error code that provides a more specific error than the HTTP status code.
    /// </summary>
    /// <example>ValidationFailed</example>
    [JsonPropertyName("code")]
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Code { get; init; }

    /// <summary>
    /// A human-readable description of the error.
    /// </summary>
    /// <example>The request parameters are invalid</example>
    [JsonPropertyName("message")]
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public required string Message { get; init; }

    /// <summary>
    /// The target of the error (optional).
    /// </summary>
    /// <example>temperatureC</example>
    [JsonPropertyName("target")]
    [StringLength(100)]
    public string? Target { get; init; }

    /// <summary>
    /// Additional error details (optional).
    /// </summary>
    [JsonPropertyName("details")]
    public IEnumerable<ApiErrorDetail>? Details { get; init; }

    /// <summary>
    /// Inner error with more specific information (optional).
    /// </summary>
    [JsonPropertyName("innererror")]
    public ApiInnerError? InnerError { get; init; }
}

/// <summary>
/// Additional error detail information.
/// </summary>
public record ApiErrorDetail
{
    /// <summary>
    /// Specific error code for this detail.
    /// </summary>
    [JsonPropertyName("code")]
    [Required]
    public required string Code { get; init; }

    /// <summary>
    /// Error message for this detail.
    /// </summary>
    [JsonPropertyName("message")]
    [Required]
    public required string Message { get; init; }

    /// <summary>
    /// Target of this specific error.
    /// </summary>
    [JsonPropertyName("target")]
    public string? Target { get; init; }
}

/// <summary>
/// Inner error providing additional technical context.
/// </summary>
public record ApiInnerError
{
    /// <summary>
    /// More specific error code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Technical error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Nested inner error for additional context.
    /// </summary>
    [JsonPropertyName("innererror")]
    public ApiInnerError? InnerError { get; init; }
}
