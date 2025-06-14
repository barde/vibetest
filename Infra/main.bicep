// main.bicep
// Deploys an Azure Function App, Storage Account, and Key Vault with best practices
// Following Azure CAF naming conventions: <resource-type>-<workload>-<environment>-<instance>

param location string = resourceGroup().location
@minLength(3)
@maxLength(12) // Ensure storage account name stays within 24 char limit: st(2) + baseName(12) + environment(4) + locationAbbr(4) + instance(3) = 25, need to optimize
param baseName string = 'cpltst' // Shortened to fit naming constraints
@allowed(['dev', 'test', 'prod'])
param environment string = 'prod'
param skuName string = 'Y1' // Consumption plan for Functions
@minLength(3)
@maxLength(3)
param instance string = '001' // Instance identifier for resource naming

// Location abbreviation mapping for consistent naming
var locationAbbreviations = {
  westeurope: 'we'
  eastus: 'eus'
  westus2: 'wus2'
  centralus: 'cus'
  northeurope: 'ne'
  southcentralus: 'scus'
}
var locationAbbr = locationAbbreviations[?location] ?? 'unk'

// Azure CAF compliant resource names
var functionAppName = 'func-${baseName}-${environment}-${locationAbbr}-${instance}'
var storageAccountName = 'st${baseName}${environment}${locationAbbr}${instance}' // Storage: no hyphens, 3-24 chars
var keyVaultName = 'kv-${baseName}-${locationAbbr}${instance}' // Key Vault: 3-24 chars, simplified for length
var staticWebAppName = 'swa-${baseName}-${environment}-${locationAbbr}-${instance}'

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-${baseName}-${environment}-${locationAbbr}-${instance}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    WorkspaceResourceId: ''
  }
  tags: {
    environment: 'production'
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
    }
  }
  tags: {
    environment: 'production'
  }
}

// App Service Plan for Function
resource plan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: 'asp-${baseName}-${environment}-${locationAbbr}-${instance}'
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
          value: '~3'
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
// Keep-alive availability test to prevent cold starts
resource keepAliveTest 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${functionAppName}-keepalive-test'
  location: location
  kind: 'ping'
  properties: {
    SyntheticMonitorId: '${functionAppName}-keepalive-test'
    Name: '${functionAppName} Keep Alive Test'
    Description: 'Pings the keep-alive endpoint every 5 minutes to prevent cold starts'
    Enabled: true
    Frequency: 300 // 5 minutes
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'us-tx-sn1-azr'
      }
      {
        Id: 'us-il-ch1-azr'
      }
    ]
    Configuration: {
      WebTest: '<WebTest Name="${functionAppName} Keep Alive Test" Id="ABD48585-0831-40CB-9069-682EA6BB3583" Enabled="True" CssProjectStructure="" CssIteration="" Timeout="30" WorkItemIds="" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010" Description="" CredentialUserName="" CredentialPassword="" PreAuthenticate="True" Proxy="default" StopOnError="False" RecordedResultFile="" ResultsLocale=""><Items><Request Method="GET" Guid="a5f10126-e4cd-570d-961c-cea43999a200" Version="1.1" Url="https://${functionAppName}.azurewebsites.net/api/keepalive" ThinkTime="0" Timeout="30" ParseDependentRequests="False" FollowRedirects="True" RecordResult="True" Cache="False" ResponseTimeGoal="0" Encoding="utf-8" ExpectedHttpStatusCode="200" ExpectedResponseUrl="" ReportingName="" IgnoreHttpStatusCode="False" /></Items></WebTest>'
    }
  }
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
    environment: 'production'
  }
}

// Alert rule for keep-alive test failures
resource keepAliveAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: '${functionAppName}-keepalive-alert'
  location: 'global'
  properties: {
    description: 'Alert when keep-alive test fails'
    severity: 2
    enabled: true
    scopes: [
      keepAliveTest.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'KeepAliveFailure'
          metricName: 'availabilityResults/availabilityPercentage'
          operator: 'LessThan'
          threshold: 80
          timeAggregation: 'Average'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
    }
  }
  tags: {
    environment: 'production'
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

// Azure Static Web Apps
resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: staticWebAppName
  location: 'Central US' // Static Web Apps has limited region availability
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    repositoryUrl: 'https://github.com/barde/vibetest' // Set actual repository URL
    branch: 'master' // Use master instead of main
    buildProperties: {
      appLocation: '/Client'
      apiLocation: '' // We'll link to existing Azure Functions
      outputLocation: 'wwwroot'
    }
    allowConfigFileUpdates: true
    stagingEnvironmentPolicy: 'Enabled'
  }
  tags: {
    environment: 'production'
  }
}

// Link Static Web Apps to existing Azure Functions for API
resource staticWebAppApiLink 'Microsoft.Web/staticSites/linkedBackends@2023-01-01' = {
  name: 'azure-functions-backend'
  parent: staticWebApp
  properties: {
    backendResourceId: functionApp.id
    region: location
  }
}

output staticWebAppName string = staticWebApp.name
output staticWebAppUrl string = staticWebApp.properties.defaultHostname
