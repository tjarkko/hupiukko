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
    // Add backend-specific app settings here if needed
    configs: [
      {
        name: 'appsettings'
        properties: {
          ENVIRONMENT: environment
          AzureAd__Instance: 'https://login.microsoftonline.com/'
          AzureAd__Domain: 'jarkkotuorilagmail.onmicrosoft.com'
          AzureAd__TenantId: '287fc32d-9911-40b2-a2c2-a6419fd5e4c9'
          AzureAd__ClientId: 'cbc42c1f-4aa7-4bec-b803-2a9efb0211e0'
          AzureAd__Audience: 'api://cbc42c1f-4aa7-4bec-b803-2a9efb0211e0'
          // swagger not used in azure for now at least
          // AzureAd__SwaggerClientId: '08716fe7-db8b-45ad-b905-7ce1b2fe0c35'
          // ConnectionStrings__DefaultConnection should be set as a secret or parameter in production
        }
      }
    ]
    // Do not set container image here; deployment pipeline will handle it
  }
}
