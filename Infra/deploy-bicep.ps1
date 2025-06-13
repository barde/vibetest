# Usage: .\deploy-bicep.ps1 -Location westeurope
param(
    [Parameter(Mandatory=$true)]
    [string]$Location
)

# Generate a random 8-character suffix (lowercase letters and digits)
$suffix = -join ((48..57) + (97..122) | Get-Random -Count 8 | ForEach-Object {[char]$_})
$baseName = "copilotblazor$suffix"
$resourceGroup = $baseName
$functionAppName = $baseName
# Storage account must be lowercase, 3-24 chars, only letters/numbers
$storageAccountName = ($baseName -replace "[^a-z0-9]", "")
if ($storageAccountName.Length -lt 3) { $storageAccountName = $storageAccountName + "001" }
$storageAccountName = $storageAccountName.Substring(0, [Math]::Min(24, $storageAccountName.Length))
$keyVaultName = $baseName

az group create --name $resourceGroup --location $Location
az deployment group create `
    --resource-group $resourceGroup `
    --template-file Infra/main.bicep `
    --parameters @Infra/main.parameters.json `
    --parameters location=$Location functionAppName=$functionAppName storageAccountName=$storageAccountName keyVaultName=$keyVaultName `
    --output table
