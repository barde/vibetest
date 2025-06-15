using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace CopilotBlazor.Shared.Models;

/// <summary>
/// Represents the health status of the application and its dependencies.
/// Follows Azure Health Check patterns and standardized status reporting.
/// </summary>
public record HealthCheckResponse
{
    /// <summary>
    /// Overall health status of the application.
    /// </summary>
    /// <example>Healthy</example>
    [JsonPropertyName("status")]
    [Required]
    public required string Status { get; init; }

    /// <summary>
    /// Total duration taken to perform all health checks.
    /// </summary>
    /// <example>00:00:00.1234567</example>
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Timestamp when the health check was performed.
    /// </summary>
    /// <example>2025-06-14T10:30:00Z</example>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Individual health check results for each component.
    /// </summary>
    [JsonPropertyName("checks")]
    public IEnumerable<HealthCheckEntry> Checks { get; init; } = [];
}

/// <summary>
/// Represents the health status of an individual component or dependency.
/// </summary>
public record HealthCheckEntry
{
    /// <summary>
    /// Name or identifier of the component being checked.
    /// </summary>
    /// <example>database</example>
    [JsonPropertyName("name")]
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }

    /// <summary>
    /// Health status of this specific component.
    /// </summary>
    /// <example>Healthy</example>
    [JsonPropertyName("status")]
    [Required]
    public required string Status { get; init; }

    /// <summary>
    /// Optional description providing additional context about the health status.
    /// </summary>
    /// <example>Successfully connected to database</example>
    [JsonPropertyName("description")]
    [StringLength(500)]
    public string? Description { get; init; }

    /// <summary>
    /// Duration taken to perform this specific health check.
    /// </summary>
    /// <example>00:00:00.0234567</example>
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Additional data related to this health check (optional).
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; init; }
}
