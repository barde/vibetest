# CopilotBlazor

A modern web application built with Blazor WebAssembly and Azure Functions, featuring automatic deployment to Azure Static Web Apps.

## Architecture

This solution contains:
- `Client`: Blazor WebAssembly frontend (deployed to Azure Static Web Apps)
- `Server`: Azure Functions v4 backend (.NET 9) 
- `Client.Tests`: xUnit tests for the frontend
- `Server.Tests`: xUnit tests for the backend
- `Infra`: Bicep infrastructure as code for Azure deployment

## Features

- **Modern Stack**: .NET 9, Blazor WebAssembly, Azure Functions v4
- **Global CDN**: Azure Static Web Apps with worldwide edge distribution
- **Serverless Backend**: Azure Functions with HTTP triggers (linked to Static Web Apps)
- **Keep-Alive Mechanism**: Timer-based availability tests to prevent cold starts
- **Monitoring**: Application Insights with availability testing and alerts
- **CI/CD**: GitHub Actions for automated build, test, and deployment
- **Infrastructure as Code**: Bicep templates for consistent deployments
- **Security**: Custom headers, CORS configuration, and built-in authentication support

## Hosting Comparison

| Feature | Azure Static Web Apps | Storage Account Static Hosting |
|---------|----------------------|--------------------------------|
| Custom Headers | ✅ Full support | ❌ Requires Azure CDN |
| Authentication | ✅ Built-in providers | ❌ Not supported |
| CI/CD Integration | ✅ Native GitHub/Azure DevOps | ❌ Manual setup required |
| API Integration | ✅ Seamless proxy to Functions | ❌ CORS complexity |
| Staging Environments | ✅ Automatic PR previews | ❌ Not available |
| Custom Domains | ✅ Free SSL included | ✅ Manual SSL setup |
| Global CDN | ✅ Built-in | ❌ Requires separate CDN |

**Recommendation**: Azure Static Web Apps provides superior developer experience and features for modern web applications.

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

The application deploys to Azure Static Web Apps for the frontend and Azure Functions for the backend. See [AZURE_STATIC_WEB_APPS_DEPLOYMENT.md](AZURE_STATIC_WEB_APPS_DEPLOYMENT.md) for detailed setup instructions.

### Quick Setup

1. **Configure GitHub Secrets** (required):
   ```
   AZURE_CLIENT_ID          # Azure service principal client ID
   AZURE_TENANT_ID          # Azure tenant ID  
   AZURE_SUBSCRIPTION_ID    # Azure subscription ID
   ```

2. **Deploy Infrastructure**:
   - Go to Actions → "Deploy Azure Infra (Bicep)" → Run workflow
   - Copy the deployment outputs to configure additional secrets

3. **Add Static Web Apps Secret**:
   ```
   AZURE_STATIC_WEB_APPS_API_TOKEN    # From infrastructure deployment output
   AZURE_BASE_NAME                    # From infrastructure deployment output
   ```

4. **Deploy Application**:
   - Push to main/master branch to trigger automatic deployment
   - Both frontend and backend will deploy automatically

### Deployment Workflows

- 🏗️ **Infrastructure**: `.github/workflows/deploy-azure-infra.yml`
- 🌐 **Frontend**: `.github/workflows/deploy-static-web-apps.yml` 
- ⚡ **Backend**: `.github/workflows/azure-functions-deploy.yml`

### Live URLs

After deployment, your application will be available at:
- **Frontend**: `https://<static-web-app-name>.azurestaticapps.net`
- **API**: Integrated through Static Web Apps proxy (no CORS issues)
- **Monitoring**: Azure Portal → Application Insights

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
