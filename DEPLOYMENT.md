# Azure Deployment Configuration

This document explains how to configure and deploy the CopilotBlazor application to Azure Functions.

## Resource Naming Strategy

The deployment uses a consistent naming strategy to ensure resource stability across deployments:

### Naming Convention
- **Base Name**: Set via GitHub secret `AZURE_BASE_NAME` for consistent resource naming
- **Resource Group**: Configurable via GitHub repository variable `AZURE_RESOURCE_GROUP` (default: `rg-copilotblazor`)
- **Function App**: `{baseName}-func-{environment}-{uniqueSuffix}`
- **Storage Account**: `{baseName}st{environment}{uniqueSuffix}`
- **Key Vault**: `{baseName}-kv-{environment}-{uniqueSuffix}`

Where:
- `{baseName}`: Fixed base name from GitHub secret (e.g., `copilotblazor`)
- `{environment}`: Environment name (default: `prod`)
- `{uniqueSuffix}`: 8-character unique string based on resource group ID

### Fallback Behavior
If `AZURE_BASE_NAME` secret is not set, the deployment will use `{repositoryName}prod` as fallback.

### Configuration Options

#### 1. GitHub Repository Variables
Set these in your GitHub repository settings under Settings > Secrets and variables > Actions > Variables:

- `AZURE_RESOURCE_GROUP`: Name of the Azure resource group (default: `rg-copilotblazor`)

#### 2. Bicep Parameters
Modify `Infra/parameters.json` to customize:

```json
{
  "parameters": {
    "baseName": {
      "value": "your-custom-name"
    },
    "environment": {
      "value": "dev|staging|prod"
    },
    "location": {
      "value": "East US"
    },
    "skuName": {
      "value": "Y1"
    }
  }
}
```

#### 3. Manual Deployment
For manual deployment using Azure CLI:

```bash
# Create resource group
az group create --name rg-copilotblazor --location "East US"

# Deploy infrastructure
az deployment group create \
  --resource-group rg-copilotblazor \
  --template-file Infra/main.bicep \
  --parameters @Infra/parameters.json

# Or with custom parameters
az deployment group create \
  --resource-group rg-copilotblazor \
  --template-file Infra/main.bicep \
  --parameters baseName=myapp environment=dev location="West US 2"
```

## Required GitHub Secrets

Configure these secrets in your GitHub repository:

- `AZURE_BASE_NAME`: Base name for Azure resources (e.g., `copilotblazor`) - ensures consistent resource naming across deployments
- `AZURE_CLIENT_ID`: Azure service principal client ID
- `AZURE_TENANT_ID`: Azure tenant ID
- `AZURE_SUBSCRIPTION_ID`: Azure subscription ID
- `AZURE_FUNCTIONAPP_PUBLISH_PROFILE`: Function app publish profile (generated after infrastructure deployment)

**Important**: The `AZURE_BASE_NAME` should be set to a consistent value to prevent resource recreation on each deployment. Choose a unique name that follows Azure naming conventions (lowercase letters and numbers only, 3-24 characters).

## Deployment Process

The deployment is separated into two workflows for better control:

### 1. Infrastructure Deployment (Manual)
- **Workflow**: `infrastructure-deploy.yml`
- **Trigger**: Manual dispatch via GitHub Actions UI
- **Purpose**: Creates all Azure resources using Bicep template
- **Run this first** to set up the infrastructure

### 2. Application Deployment (Automatic)
- **Workflow**: `azure-functions-deploy.yml`
- **Trigger**: Push to master/main branch
- **Purpose**: Builds, tests, and deploys application code only
- **Requires**: Infrastructure to be deployed first

### Deployment Steps

1. **First Time Setup**:
   - Configure GitHub secrets (see Required GitHub Secrets section)
   - Run "Deploy Azure Infrastructure" workflow manually
   - Note the Function App name from the infrastructure deployment
   - Download the publish profile from Azure portal and add as `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` secret

2. **Ongoing Deployments**:
   - Push code changes to master/main branch
   - Application deployment runs automatically
   - Infrastructure remains stable and unchanged

## Environment-Specific Deployments

To deploy to different environments, modify the workflow or create separate workflows:

```yaml
- name: Deploy Infrastructure
  run: |
    az deployment group create \
      --resource-group ${{ env.AZURE_RESOURCE_GROUP }} \
      --template-file Infra/main.bicep \
      --parameters baseName=${{ env.AZURE_BASE_NAME }} environment=staging
```

## Monitoring and Alerts

The deployment automatically configures:
- Application Insights for monitoring
- Keep-alive availability tests (every 5 minutes)
- Alert rules for availability failures

## Clean Up

To remove all resources:

```bash
az group delete --name rg-copilotblazor --yes --no-wait