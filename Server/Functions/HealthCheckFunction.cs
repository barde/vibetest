using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Server.Functions;

public class HealthCheckFunction : IHealthCheck
{
    private readonly ILogger<HealthCheckFunction> _logger;

    public HealthCheckFunction(ILogger<HealthCheckFunction> logger)
    {
        _logger = logger;
    }    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Performing health check");
        // Add actual health check logic here
        return Task.FromResult(HealthCheckResult.Healthy("All systems operational"));
    }[Function("HealthCheck")]
    public async Task<HttpResponseData> HttpHealthCheck(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req,
        HealthCheckService healthCheckService)
    {
        _logger.LogInformation("Health check request received");

        var healthReport = await healthCheckService.CheckHealthAsync();
        var status = healthReport.Status == HealthStatus.Healthy
            ? HttpStatusCode.OK
            : HttpStatusCode.ServiceUnavailable;

        var response = req.CreateResponse(status);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var result = new {
            status = healthReport.Status.ToString(),
            duration = healthReport.TotalDuration,
            checks = healthReport.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration
            })
        };

        await response.WriteStringAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));

        return response;
    }
}