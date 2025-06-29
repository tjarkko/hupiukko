param location string
param appServiceName string
param keyVaultId string
param environment string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param appServicePlanName string

var azureAdClientSecretUri = 'https://${split(keyVaultId, '/')[8]}.${az.environment().suffixes.keyvaultDns}/secrets/AZURE_AD_CLIENT_SECRET'
var nextAuthSecretUri = 'https://${split(keyVaultId, '/')[8]}.${az.environment().suffixes.keyvaultDns}/secrets/NEXTAUTH_SECRET'

// Use AVM App Service Plan (Linux, Free tier)
module hupiukkoAppServicePlan 'br/public:avm/res/web/serverfarm:0.4.1' = {
  name: 'hupiukkoAppServicePlan'
  params: {
    name: appServicePlanName
    location: resourceGroup().location
    skuName: 'F1'
    kind: 'linux'
    reserved: true
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
    }
    httpsOnly: true
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
