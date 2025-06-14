using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using CopilotBlazor.Shared.Models;
using CopilotBlazor.Shared.Constants;
using CopilotBlazor.Shared.Extensions;

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
    }    [Function("HealthCheck")]
    public async Task<HttpResponseData> HttpHealthCheck(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiRoutes.Health.Check)] HttpRequestData req,
        HealthCheckService healthCheckService)
    {
        _logger.LogInformation("Health check request received");

        var healthReport = await healthCheckService.CheckHealthAsync();
        var status = healthReport.Status == HealthStatus.Healthy
            ? HttpStatusCode.OK
            : HttpStatusCode.ServiceUnavailable;

        var response = req.CreateResponse(status);
        response.Headers.Add("Content-Type", ContentTypes.Json);
        
        var healthCheckResponse = new HealthCheckResponse
        {
            Status = healthReport.Status.ToString(),
            Duration = healthReport.TotalDuration,
            Timestamp = DateTime.UtcNow,
            Checks = healthReport.Entries.Select(e => new HealthCheckEntry
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration,
                Data = e.Value.Data?.ToDictionary(x => x.Key, x => x.Value)
            })
        };

        await response.WriteStringAsync(healthCheckResponse.ToJson(compact: true));

        return response;
    }
}