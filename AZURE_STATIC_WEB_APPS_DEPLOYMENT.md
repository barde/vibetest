# Azure Static Web Apps Deployment Guide

This document provides instructions for deploying your Blazor WebAssembly application to Azure Static Web Apps with integrated Azure Functions backend.

## Overview

Your solution now includes:
- **Blazor WebAssembly Client** - Hosted on Azure Static Web Apps
- **Azure Functions Backend** - Linked to Static Web Apps for seamless API integration
- **Infrastructure as Code** - Bicep templates for consistent deployments
- **CI/CD Workflows** - GitHub Actions for automated deployments

## Prerequisites

Before deploying, ensure you have:
1. Azure subscription with appropriate permissions
2. GitHub repository with the following secrets configured:
   - `AZURE_CLIENT_ID`
   - `AZURE_TENANT_ID` 
   - `AZURE_SUBSCRIPTION_ID`
   - `AZURE_BASE_NAME` (will be set during infrastructure deployment)

Note: The `AZURE_STATIC_WEB_APPS_API_TOKEN` is automatically retrieved from Azure during deployment - no manual secret configuration needed!

## Deployment Steps

### 1. Deploy Infrastructure

1. Navigate to your GitHub repository
2. Go to **Actions** tab
3. Select **Deploy Azure Infra (Bicep)** workflow
4. Click **Run workflow** and confirm

This will:
- Create a new resource group with unique naming
- Deploy Azure Functions, Storage Account, Key Vault, and Application Insights
- Deploy Azure Static Web Apps resource
- Set up API linking between Static Web Apps and Azure Functions
- Provide you with the deployment token for Static Web Apps

### 2. Configure Secrets

After the infrastructure deployment completes:

1. Copy the `AZURE_BASE_NAME` value from the workflow output
2. Go to your repository **Settings** > **Secrets and variables** > **Actions**
3. Add/update the secret:
   - `AZURE_BASE_NAME`: Use the base name from the infrastructure deployment output

The deployment token is automatically retrieved from Azure during the Static Web Apps deployment - no manual configuration needed!

### 3. Deploy Blazor Application

1. Go to **Actions** tab in your repository
2. The **Deploy Blazor to Azure Static Web Apps** workflow should automatically trigger on the next push to main/master
3. Alternatively, you can manually trigger it from the Actions tab

### 4. Deploy Azure Functions (Optional)

If you want to deploy custom Azure Functions:
1. Use the **Build, Test, and Deploy Azure Functions** workflow
2. This will deploy your Server project to the Azure Functions resource

## Architecture Benefits

### Azure Static Web Apps Advantages
- ✅ **Global CDN** - Content delivered from edge locations worldwide
- ✅ **Built-in Authentication** - Integration with Microsoft Entra ID, GitHub, etc.
- ✅ **Custom Domains & SSL** - Free SSL certificates with automatic renewal
- ✅ **Staging Environments** - Automatic preview deployments for pull requests
- ✅ **API Integration** - Seamless connection to Azure Functions backend
- ✅ **Custom Headers** - Security headers and CORS configuration
- ✅ **Zero Configuration** - No server management required

### vs. Storage Account Static Hosting
Unlike Storage Account static hosting, Azure Static Web Apps provides:
- Custom headers support (security, caching, CORS)
- Built-in authentication and authorization
- Advanced routing rules and fallbacks
- CI/CD integration with GitHub/Azure DevOps
- Staging environments for pull requests
- API proxy without CORS issues

## Monorepo Workflow Optimization

The CI/CD workflows are designed for efficient monorepo operations:

### Path-Based Triggers
Each workflow only runs when relevant files change:
- **Client changes** (`Client/`, `Client.Tests/`) → Static Web Apps deployment
- **Server changes** (`Server/`, `Server.Tests/`) → Azure Functions deployment  
- **Infrastructure changes** (`Infra/`) → Bicep template deployment

### Workflow Types
- **Pull Requests**: Run tests only for fast feedback
- **Push to main**: Full build, test, and deployment
- **Manual trigger**: Infrastructure deployment via `workflow_dispatch`

### Benefits
- **Reduced build times** - Only affected components build
- **Lower costs** - Less compute usage on both GitHub Actions and Azure
- **Parallel deployments** - Independent workflows can run simultaneously
- **Clear separation** - Each workflow focuses on its specific component

This ensures you only deploy what's changed, reducing build times and Azure consumption.

## Configuration Files

### `/Client/wwwroot/staticwebapp.config.json`
This file configures:
- **Route handling** for your Blazor SPA
- **Security headers** for protection against common attacks
- **MIME types** for proper content serving
- **Fallback routing** for client-side navigation

### GitHub Workflows
- **`.github/workflows/deploy-azure-infra.yml`** - Infrastructure deployment
- **`.github/workflows/deploy-static-web-apps.yml`** - Blazor client deployment
- **`.github/workflows/azure-functions-deploy.yml`** - Azure Functions deployment

## Monitoring & Troubleshooting

### Application Insights
Your deployment includes Application Insights for monitoring:
- Application performance and usage
- Custom events and metrics
- Dependency tracking
- Error tracking and diagnostics

### Static Web Apps Logs
Monitor your Static Web Apps deployment:
1. Go to Azure Portal
2. Navigate to your Static Web Apps resource
3. Check **Functions** section for API logs
4. Use **Environments** section to view deployment history

### Keep-Alive Configuration
The infrastructure includes:
- Web tests that ping your Functions every 5 minutes
- Alerts when availability drops below 80%
- This helps minimize cold starts for your Azure Functions

## Cost Optimization

- **Static Web Apps**: Free tier supports unlimited static sites
- **Azure Functions**: Consumption plan charges only for execution time
- **Storage & Key Vault**: Minimal costs for configuration storage
- **Application Insights**: Pay-per-use for telemetry data

## Next Steps

1. **Custom Domain**: Configure a custom domain in Static Web Apps settings
2. **Authentication**: Enable authentication providers as needed
3. **API Development**: Extend your Azure Functions with additional endpoints
4. **Monitoring**: Set up alerts and dashboards in Application Insights
5. **Performance**: Enable compression and caching optimizations

## Support

For issues or questions:
- Check Azure Static Web Apps documentation: https://docs.microsoft.com/azure/static-web-apps/
- Review GitHub Actions logs for deployment issues
- Use Azure Portal metrics and logs for runtime issues
