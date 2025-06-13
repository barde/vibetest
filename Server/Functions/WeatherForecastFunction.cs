using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Server.Functions;

public class WeatherForecastFunction
{
    private readonly ILogger<WeatherForecastFunction> _logger;

    public WeatherForecastFunction(ILogger<WeatherForecastFunction> logger)
    {
        _logger = logger;
    }

    [Function("GetWeatherForecast")]
    public async Task<HttpResponseData> GetWeatherForecast(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weatherforecast")] HttpRequestData req)
    {
        _logger.LogInformation("Getting weather forecast data.");

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        // Add CORS headers
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        await response.WriteStringAsync(JsonSerializer.Serialize(forecast, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));

        return response;
    }

    [Function("OptionsWeatherForecast")]
    public HttpResponseData OptionsWeatherForecast(
        [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "weatherforecast")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        
        // Add CORS headers for preflight requests
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        response.Headers.Add("Access-Control-Max-Age", "86400");

        return response;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}