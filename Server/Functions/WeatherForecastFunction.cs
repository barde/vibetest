using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using CopilotBlazor.Shared.Models;
using CopilotBlazor.Shared.Constants;
using CopilotBlazor.Shared.Extensions;

namespace Server.Functions;

public class WeatherForecastFunction
{
    private readonly ILogger<WeatherForecastFunction> _logger;

    public WeatherForecastFunction(ILogger<WeatherForecastFunction> logger)
    {
        _logger = logger;
    }    [Function("GetWeatherForecast")]
    public async Task<HttpResponseData> GetWeatherForecast(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiRoutes.Weather.GetForecast)] HttpRequestData req)
    {
        _logger.LogInformation("Getting weather forecast data.");

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", ContentTypes.Json);
        
        // Add CORS headers
        response.Headers.Add("Access-Control-Allow-Origin", CorsPolicy.AllowAllOrigins);
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        await response.WriteStringAsync(forecast.ToJson(compact: true));

        return response;
    }    [Function("OptionsWeatherForecast")]
    public HttpResponseData OptionsWeatherForecast(
        [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = ApiRoutes.Weather.GetForecast)] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        
        // Add CORS headers for preflight requests
        response.Headers.Add("Access-Control-Allow-Origin", CorsPolicy.AllowAllOrigins);
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        response.Headers.Add("Access-Control-Max-Age", "86400");

        return response;
    }
}