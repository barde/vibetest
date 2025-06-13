// main.bicep
// Deploys an Azure Function App, Storage Account, and Key Vault with best practices

param location string = resourceGroup().location
param functionAppName string
param storageAccountName string
param keyVaultName string
param skuName string = 'Y1' // Consumption plan for Functions

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
    }
  }
  tags: {
    environment: 'production'
  }
}

// App Service Plan for Function
resource plan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${functionAppName}-plan'
  location: location
  sku: {
    name: skuName
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
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storage.properties.primaryEndpoints.blob
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
    // AlwaysOn is not set (Consumption plan does not support it)
  }
  tags: {
    environment: 'production'
  }
}

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
