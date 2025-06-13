# CopilotBlazor

A modern web application built with Blazor WebAssembly and Azure Functions, featuring automatic deployment and monitoring.

## Architecture

This solution contains:
- `Client`: Blazor WebAssembly frontend
- `Server`: Azure Functions v4 backend (.NET 9)
- `Client.Tests`: xUnit tests for the frontend
- `Server.Tests`: xUnit tests for the backend
- `Infra`: Bicep infrastructure as code for Azure deployment

## Features

- **Modern Stack**: .NET 9, Blazor WebAssembly, Azure Functions v4
- **Serverless Backend**: Azure Functions with HTTP triggers
- **Keep-Alive Mechanism**: Timer-based function to prevent cold starts
- **Monitoring**: Application Insights with availability testing
- **CI/CD**: GitHub Actions for automated build, test, and deployment
- **Infrastructure as Code**: Bicep templates for consistent deployments

## Prerequisites

- .NET 9 SDK
- Azure CLI (for deployment)
- Azure subscription (for cloud deployment)

## Local Development

### Build
```bash
dotnet build CopilotBlazor.sln
```

### Run Backend (Azure Functions)
```bash
cd Server
func start
# or
dotnet run
```

### Run Frontend
```bash
dotnet run --project Client/Client.csproj
```

### Run All Tests
```bash
dotnet test CopilotBlazor.sln
```

## Azure Deployment

The application automatically deploys to Azure Functions using GitHub Actions. See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed configuration instructions.

### Quick Setup

1. **Configure GitHub Secrets** (required):
   - `AZURE_BASE_NAME`: Base name for Azure resources (e.g., `copilotblazor`)
   - `AZURE_CLIENT_ID`: Azure service principal client ID
   - `AZURE_TENANT_ID`: Azure tenant ID
   - `AZURE_SUBSCRIPTION_ID`: Azure subscription ID

2. **Push to main/master branch** to trigger automatic deployment

3. **Get publish profile** after first deployment and add as `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` secret

### API Endpoints

- `GET /api/weatherforecast` - Returns weather forecast data
- `GET /api/keepalive` - Keep-alive endpoint for monitoring

## Project Structure

```
CopilotBlazor/
├── Client/                 # Blazor WebAssembly app
├── Server/                 # Azure Functions app
│   ├── Functions/         # HTTP trigger functions
│   ├── host.json          # Functions host configuration
│   └── local.settings.json # Local development settings
├── Infra/                 # Azure infrastructure (Bicep)
├── .github/workflows/     # CI/CD pipelines
└── Tests/                 # Unit tests
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests: `dotnet test`
5. Submit a pull request

## License

This project is licensed under the MIT License.
