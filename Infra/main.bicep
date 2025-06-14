// main.bicep
// Deploys an Azure Function App, Storage Account, and Key Vault with best practices
// Simplified naming convention for West Europe deployment

@description('The workload name identifier')
@minLength(3)
@maxLength(8)
param workloadName string = 'cpltst'

@description('The deployment environment')
@allowed(['dev', 'test', 'prod'])
param environment string = 'prod'

@description('The SKU name for the Function App service plan')
param functionAppSku string = 'Y1' // Consumption plan for Functions

// Fixed location: West Europe
var location = 'westeurope'

// Simplified Azure CAF compliant resource names (no location abbreviation or instance)
var functionAppName = 'func-${workloadName}-${environment}'
var storageAccountName = 'st${workloadName}${environment}' // Storage: no hyphens, 3-24 chars
var keyVaultName = 'kv-${workloadName}-${environment}' // Key Vault: 3-24 chars

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-${workloadName}-${environment}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: 90
    DisableIpMasking: false
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  tags: {
    environment: environment
  }
}

// Storage Account
resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        blob: { enabled: true }
        file: { enabled: true }
      }
      keySource: 'Microsoft.Storage'
    }
  }
  tags: {
    environment: 'production'
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [] // Use RBAC
    enableRbacAuthorization: true
    enableSoftDelete: true
    enablePurgeProtection: true
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }  }
  tags: {
    environment: environment
  }
}

// App Service Plan for Function
resource plan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: 'asp-${workloadName}-${environment}'
  location: location
  sku: {
    name: functionAppSku
    tier: 'Dynamic'
  }
  kind: 'functionapp'
}

// Azure Function App
resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    siteConfig: {      appSettings: [        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'        }      ]
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
    // AlwaysOn is not set (Consumption plan does not support it)
  }
  tags: {
    environment: environment
  }
}

output appInsightsName string = appInsights.name

// Key Vault Access Policy for Function App (using parent property for clarity)
resource kvAccess 'Microsoft.KeyVault/vaults/accessPolicies@2023-02-01' = {
  name: 'add'
  parent: keyVault
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: functionApp.identity.principalId
        permissions: {
          secrets: [ 'get', 'list' ]
        }
      }
    ]
  }
}

output functionAppName string = functionApp.name
output storageAccountName string = storage.name
output keyVaultName string = keyVault.name
