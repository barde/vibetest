namespace CopilotBlazor.Shared.Constants;

/// <summary>
/// API route constants following Azure API Guidelines and RESTful conventions.
/// Provides centralized route management and versioning support.
/// </summary>
public static class ApiRoutes
{
    /// <summary>
    /// Base API prefix for all endpoints.
    /// </summary>
    public const string ApiPrefix = "api";

    /// <summary>
    /// Current API version.
    /// </summary>
    public const string ApiVersion = "v1";

    /// <summary>
    /// Full API base path including version.
    /// </summary>
    public const string ApiBase = $"{ApiPrefix}/{ApiVersion}";

    /// <summary>
    /// Weather forecast related endpoints.
    /// </summary>
    public static class Weather
    {
        /// <summary>
        /// Base route for weather endpoints.
        /// </summary>
        public const string Base = "weatherforecast";

        /// <summary>
        /// Get weather forecast data.
        /// GET /api/v1/weatherforecast
        /// </summary>
        public const string GetForecast = Base;

        /// <summary>
        /// Get weather forecast for specific location (future enhancement).
        /// GET /api/v1/weatherforecast/{location}
        /// </summary>
        public const string GetForecastByLocation = $"{Base}/{{location}}";
    }

    /// <summary>
    /// Health check and monitoring endpoints.
    /// </summary>
    public static class Health
    {
        /// <summary>
        /// Health check endpoint.
        /// GET /api/v1/health
        /// </summary>
        public const string Check = "health";

        /// <summary>
        /// Keep-alive endpoint for monitoring and cold-start prevention.
        /// GET /api/v1/keepalive
        /// </summary>
        public const string KeepAlive = "keepalive";

        /// <summary>
        /// Detailed health information (future enhancement).
        /// GET /api/v1/health/detailed
        /// </summary>
        public const string Detailed = $"{Check}/detailed";
    }

    /// <summary>
    /// Future API endpoints for extensibility.
    /// </summary>
    public static class Future
    {
        /// <summary>
        /// User management endpoints (placeholder).
        /// </summary>
        public const string Users = "users";

        /// <summary>
        /// Configuration endpoints (placeholder).
        /// </summary>
        public const string Config = "config";
    }
}

/// <summary>
/// HTTP status codes commonly used in the API with their meanings.
/// Provides standardized status code usage following Azure API Guidelines.
/// </summary>
public static class ApiStatusCodes
{
    /// <summary>
    /// Successful responses (2xx).
    /// </summary>
    public static class Success
    {
        /// <summary>
        /// 200 OK - Request succeeded.
        /// </summary>
        public const int Ok = 200;

        /// <summary>
        /// 201 Created - Resource created successfully.
        /// </summary>
        public const int Created = 201;

        /// <summary>
        /// 202 Accepted - Request accepted for processing.
        /// </summary>
        public const int Accepted = 202;

        /// <summary>
        /// 204 No Content - Request succeeded with no content to return.
        /// </summary>
        public const int NoContent = 204;
    }

    /// <summary>
    /// Client error responses (4xx).
    /// </summary>
    public static class ClientError
    {
        /// <summary>
        /// 400 Bad Request - Invalid request format or parameters.
        /// </summary>
        public const int BadRequest = 400;

        /// <summary>
        /// 401 Unauthorized - Authentication required.
        /// </summary>
        public const int Unauthorized = 401;

        /// <summary>
        /// 403 Forbidden - Access denied.
        /// </summary>
        public const int Forbidden = 403;

        /// <summary>
        /// 404 Not Found - Resource not found.
        /// </summary>
        public const int NotFound = 404;

        /// <summary>
        /// 409 Conflict - Request conflicts with current state.
        /// </summary>
        public const int Conflict = 409;

        /// <summary>
        /// 422 Unprocessable Entity - Validation failed.
        /// </summary>
        public const int UnprocessableEntity = 422;

        /// <summary>
        /// 429 Too Many Requests - Rate limit exceeded.
        /// </summary>
        public const int TooManyRequests = 429;
    }

    /// <summary>
    /// Server error responses (5xx).
    /// </summary>
    public static class ServerError
    {
        /// <summary>
        /// 500 Internal Server Error - General server error.
        /// </summary>
        public const int InternalServerError = 500;

        /// <summary>
        /// 502 Bad Gateway - Invalid response from upstream server.
        /// </summary>
        public const int BadGateway = 502;

        /// <summary>
        /// 503 Service Unavailable - Service temporarily unavailable.
        /// </summary>
        public const int ServiceUnavailable = 503;

        /// <summary>
        /// 504 Gateway Timeout - Upstream server timeout.
        /// </summary>
        public const int GatewayTimeout = 504;
    }
}

/// <summary>
/// Content type constants for HTTP responses.
/// </summary>
public static class ContentTypes
{
    /// <summary>
    /// JSON content type with UTF-8 encoding.
    /// </summary>
    public const string Json = "application/json; charset=utf-8";

    /// <summary>
    /// Plain text content type with UTF-8 encoding.
    /// </summary>
    public const string Text = "text/plain; charset=utf-8";

    /// <summary>
    /// XML content type with UTF-8 encoding.
    /// </summary>
    public const string Xml = "application/xml; charset=utf-8";

    /// <summary>
    /// HTML content type with UTF-8 encoding.
    /// </summary>
    public const string Html = "text/html; charset=utf-8";
}

/// <summary>
/// CORS policy constants for cross-origin resource sharing.
/// </summary>
public static class CorsPolicy
{
    /// <summary>
    /// Allow all origins (for development only).
    /// </summary>
    public const string AllowAllOrigins = "*";

    /// <summary>
    /// Allowed HTTP methods.
    /// </summary>
    public const string AllowedMethods = "GET, POST, PUT, DELETE, OPTIONS";

    /// <summary>
    /// Allowed headers.
    /// </summary>
    public const string AllowedHeaders = "Content-Type, Authorization, X-Requested-With";

    /// <summary>
    /// CORS preflight cache duration (24 hours).
    /// </summary>
    public const string MaxAge = "86400";
}
