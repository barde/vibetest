using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using CopilotBlazor.Shared.Models;
using CopilotBlazor.Shared.Constants;
using CopilotBlazor.Shared.Extensions;

namespace Server.Functions;

public class KeepAliveFunction
{
    private readonly ILogger<KeepAliveFunction> _logger;

    public KeepAliveFunction(ILogger<KeepAliveFunction> logger)
    {
        _logger = logger;
    }    [Function("KeepAlive")]
    public HttpResponseData KeepAlive(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiRoutes.Health.KeepAlive)] HttpRequestData req)
    {
        _logger.LogInformation("Keep-alive ping received at {time}", DateTime.UtcNow);

        var keepAliveResponse = ResponseExtensions.CreateKeepAliveResponse(new Dictionary<string, object>
        {
            ["functionName"] = "KeepAlive",
            ["version"] = "1.0.0"
        });

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", ContentTypes.Json);
        
        // Add CORS headers
        response.Headers.Add("Access-Control-Allow-Origin", CorsPolicy.AllowAllOrigins);
        response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        response.WriteString(keepAliveResponse.ToJson(compact: true));

        return response;
    }

    [Function("TimerKeepAlive")]
    public void TimerKeepAlive([TimerTrigger("0 */4 * * * *")] MyTimerInfo myTimer)
    {
        _logger.LogInformation("Timer keep-alive executed at {time}", DateTime.UtcNow);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {time}", myTimer.ScheduleStatus.Next);
        }
    }
}

public class MyTimerInfo
{
    public MyScheduleStatus? ScheduleStatus { get; set; }
}

public class MyScheduleStatus
{
    public DateTime Last { get; set; }
    public DateTime Next { get; set; }
    public DateTime LastUpdated { get; set; }
}