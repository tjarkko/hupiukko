param location string
param appServiceName string
param keyVaultUri string
param environment string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param appServicePlanName string
@description('Array of user-assigned managed identity resource IDs for the frontend app. Leave empty for none.')
param frontendIdentityResourceIds array = []

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
    name: appServiceName
    location: resourceGroup().location
    serverFarmResourceId: hupiukkoAppServicePlan.outputs.resourceId
    kind: 'app,linux'
    siteConfig: {
      linuxFxVersion: 'NODE|18-lts'
      appCommandLine: startupCommand
    }
    httpsOnly: true
    managedIdentities: {
      userAssignedResourceIds: frontendIdentityResourceIds
    }
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
