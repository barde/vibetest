# Azure CAF Naming Conventions Implementation

## Overview
This document outlines the Azure Cloud Adoption Framework (CAF) naming conventions implemented in the CopilotBlazor project, replacing the previous random suffix-based naming with predictable, standardized resource names.

## Naming Pattern
All Azure resources follow the Azure CAF recommended pattern:
```
<resource-type>-<workload>-<environment>-<region-abbreviation>-<instance>
```

## Parameters
- **Workload**: `cpltst` (CopilotTest - shortened to fit naming constraints)
- **Environment**: `prod`, `dev`, or `test`
- **Region Abbreviations**: 
  - `westeurope` → `we`
  - `eastus` → `eus`
  - `westus2` → `wus2`
  - `centralus` → `cus`
  - `northeurope` → `ne`
- **Instance**: `001` (incrementable for multiple deployments)

## Resource Naming Examples

### General Resources
| Resource Type | Example Name | Pattern |
|---------------|--------------|---------|
| Resource Group | `rg-cpltst-prod-we` | `rg-{workload}-{environment}-{region}` |
| Function App | `func-cpltst-prod-we-001` | Standard CAF pattern |
| App Service Plan | `asp-cpltst-prod-we-001` | Standard CAF pattern |
| Application Insights | `appi-cpltst-prod-we-001` | Standard CAF pattern |
| Static Web App | `swa-cpltst-prod-we-001` | Standard CAF pattern |

### Storage & Security
| Resource Type | Example Name | Pattern | Notes |
|---------------|--------------|---------|-------|
| Storage Account | `stcpltstprodwe001` | No hyphens (Azure requirement) | Must be globally unique |
| Key Vault | `kv-cpltst-we001` | Shortened (24 char limit) | Environment omitted for brevity |

## Benefits

### ✅ **Predictability**
- No random suffixes that make resource tracking difficult
- Consistent naming across all environments
- Easy to identify resource purpose and environment

### ✅ **Compliance**
- Follows Microsoft Azure CAF best practices
- Meets enterprise naming standards
- Supports automated governance and tagging

### ✅ **Manageability**
- Resources are easily identifiable in Azure Portal
- Simplified automation and scripting
- Better cost tracking and resource organization

### ✅ **Scalability**
- Instance numbers allow for multiple deployments
- Environment-specific naming supports proper SDLC
- Region abbreviations support multi-region deployments

## Implementation

### Bicep Template (`main.bicep`)
- Parameters validate naming constraints
- Location abbreviation mapping for consistency
- Resource names generated using CAF patterns

### GitHub Actions (`deploy-azure-infra.yml`)
- Workflow inputs for environment and location
- Automatic region abbreviation mapping
- Predictable resource group naming

### Parameters (`main.parameters.json`)
- Default values aligned with CAF conventions
- Environment-specific parameter override support

## Migration Impact

### Before (Random Suffixes)
```
rg-myapp-123456
func-myapp-789012
st-myapp-345678
```

### After (Azure CAF)
```
rg-cpltst-prod-we
func-cpltst-prod-we-001
stcpltstprodwe001
```

## Next Steps

1. **Deploy Infrastructure**: Use the updated workflow to deploy with CAF naming
2. **Update Documentation**: Ensure all documentation reflects new naming
3. **Monitor Compliance**: Regular audits to ensure naming consistency
4. **Scale Implementation**: Apply patterns to additional environments/regions

## References

- [Azure CAF Naming Conventions](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming)
- [Azure Resource Abbreviations](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations)
- [Azure Naming Tool](https://github.com/microsoft/CloudAdoptionFramework/tree/master/ready/AzNamingTool)
