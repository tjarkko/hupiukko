// main.bicep - Entry point for Hupiukko infra

param location string = resourceGroup().location
param environment string
param appServiceName string
param keyVaultName string
param resourceGroupName string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param appServicePlanName string

@description('Set to true to deploy the Key Vault module')
param deployKeyVault bool = true
@description('Set to true to deploy the App Service module')
param deployAppService bool = true

module keyVault 'modules/keyvault/keyvault.bicep' = if (deployKeyVault) {
  name: 'keyVault'
  params: {
    location: location
    keyVaultName: keyVaultName
  }
}

module appService 'modules/appservice/appservice.bicep' = if (deployAppService) {
  name: 'appService'
  params: {
    location: location
    appServiceName: appServiceName
    appServicePlanName: appServicePlanName
    keyVaultId: keyVault.outputs.keyVaultId
    environment: environment
    NEXT_PUBLIC_API_URL: NEXT_PUBLIC_API_URL
    NEXTAUTH_URL: NEXTAUTH_URL
    AZURE_AD_CLIENT_ID: AZURE_AD_CLIENT_ID
    AZURE_AD_TENANT_ID: AZURE_AD_TENANT_ID
  }
} 
