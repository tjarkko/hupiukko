param location string
param frontendAppServiceName string
param backendAppServiceName string
param keyVaultUri string
param environment string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param appServicePlanName string
@description('Array of user-assigned managed identity resource IDs for the frontend app. Leave empty for none.')
param frontendIdentityResourceIds array = []
@description('Array of user-assigned managed identity resource IDs for the backend app. Leave empty for none.')
param backendIdentityResourceIds array = []
@description('Azure AD configuration for the backend app service')
param backendAppServiceAzureAd object
@description('Name of the SQL connection string secret in Key Vault')
@secure()
param sqlConnectionStringSecret string

@description('Startup command for the App Service (e.g., node server.js for Next.js standalone output)')
param startupCommand string = 'node server.js'

var azureAdClientSecretUri = '${keyVaultUri}secrets/AZURE-AD-CLIENT-SECRET'
var nextAuthSecretUri = '${keyVaultUri}secrets/NEXTAUTH-SECRET'

// Use AVM App Service Plan (Linux, Free tier)
module hupiukkoAppServicePlan 'br/public:avm/res/web/serverfarm:0.4.1' = {
  name: 'hupiukkoAppServicePlan'
  params: {
    name: appServicePlanName
    location: location
    skuName: 'F1'
    kind: 'linux'
    reserved: true
    skuCapacity: 1
    // Add other optional params as needed
  }
}

// Use AVM Web App (Linux, Node.js)
module hupiukkoFrontendAppService 'br/public:avm/res/web/site:0.16.0' = {
  name: 'hupiukkoFrontendAppService'
  params: {
    name: frontendAppServiceName
    location: resourceGroup().location
    serverFarmResourceId: hupiukkoAppServicePlan.outputs.resourceId
    kind: 'app,linux'
    siteConfig: {
      linuxFxVersion: 'NODE|22-lts'
      appCommandLine: startupCommand
    }
    httpsOnly: true
    managedIdentities: {
      userAssignedResourceIds: frontendIdentityResourceIds
    }
    keyVaultAccessIdentityResourceId: frontendIdentityResourceIds[0]
    configs: [
      {
        name: 'appsettings'
        properties: {
          NEXT_PUBLIC_API_URL: NEXT_PUBLIC_API_URL
          BACKEND_API_URL: NEXT_PUBLIC_API_URL
          NEXTAUTH_URL: NEXTAUTH_URL
          AZURE_AD_CLIENT_ID: AZURE_AD_CLIENT_ID
          AZURE_AD_TENANT_ID: AZURE_AD_TENANT_ID
          ENVIRONMENT: environment
          AZURE_AD_CLIENT_SECRET: '@Microsoft.KeyVault(SecretUri=${azureAdClientSecretUri})'
          NEXTAUTH_SECRET: '@Microsoft.KeyVault(SecretUri=${nextAuthSecretUri})'
        }
      }
    ]
    // Add other optional params as needed
  }
}

// Backend App Service (Docker, image set by deployment pipeline)
module hupiukkoBackendAppService 'br/public:avm/res/web/site:0.16.0' = {
  name: 'hupiukkoBackendAppService'
  params: {
    name: backendAppServiceName
    location: resourceGroup().location
    serverFarmResourceId: hupiukkoAppServicePlan.outputs.resourceId
    kind: 'app,linux'
    siteConfig: {
      // For Docker deployment, do not set linuxFxVersion or appCommandLine here.
      // The deployment pipeline will set the image, and the container's ENTRYPOINT will be used.
    }
    httpsOnly: true
    managedIdentities: {
      userAssignedResourceIds: backendIdentityResourceIds
    }
    keyVaultAccessIdentityResourceId: backendIdentityResourceIds[0]
    // Add backend-specific app settings here if needed
    configs: [
      {
        name: 'appsettings'
        properties: union(
          {
            ENVIRONMENT: environment
            ConnectionStrings__DefaultConnection: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/${sqlConnectionStringSecret}/)'
          },
          backendAppServiceAzureAd
        )
      }
    ]
    // Do not set container image here; deployment pipeline will handle it
  }
}
