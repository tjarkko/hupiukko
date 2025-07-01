// main.bicep - Entry point for Hupiukko infra
// 'deployKeyVault' and 'deployAppService' are controlled by the 'services' input in the pipeline.
// 'all' means both are true; 'keyvault' or 'appservice' means only that service is deployed.

param environment string
param appServiceName string
param keyVaultName string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param frontendIdentityResourceIds array = []
param appServicePlanName string
param keyVaultUri string
param keyVaultResourceId string
param managedIdentities array

@description('Set to true to deploy the Key Vault module')
param deployKeyVault bool = true
@description('Set to true to deploy the App Service module')
param deployAppService bool = true
@description('Set to true to deploy both managed identities and RBAC assignments')
param deployManagedIdentitiesAndRbac bool = true

output environment string = environment
output appServiceName string = appServiceName
output keyVaultName string = keyVaultName
output NEXT_PUBLIC_API_URL string = NEXT_PUBLIC_API_URL
output NEXTAUTH_URL string = NEXTAUTH_URL
output AZURE_AD_CLIENT_ID string = AZURE_AD_CLIENT_ID
output AZURE_AD_TENANT_ID string = AZURE_AD_TENANT_ID
output appServicePlanName string = appServicePlanName 
module keyVault 'modules/keyvault/keyvault.bicep' = if (deployKeyVault) {
  name: 'keyVault'
  params: {
    location: resourceGroup().location
    keyVaultName: keyVaultName
    enablePurgeProtection: false
  }
}

module appService 'modules/appservice/appservice.bicep' = if (deployAppService) {
  name: 'appService'
  params: {
    location: resourceGroup().location
    appServiceName: appServiceName
    appServicePlanName: appServicePlanName
    keyVaultUri: keyVaultUri
    environment: environment
    NEXT_PUBLIC_API_URL: NEXT_PUBLIC_API_URL
    NEXTAUTH_URL: NEXTAUTH_URL
    AZURE_AD_CLIENT_ID: AZURE_AD_CLIENT_ID
    AZURE_AD_TENANT_ID: AZURE_AD_TENANT_ID
    frontendIdentityResourceIds: frontendIdentityResourceIds
  }
}

// Deploy managed identities and RBAC only if enabled
module managedIdentitiesModule 'modules/managed-identities.bicep' = if (deployManagedIdentitiesAndRbac) {
  name: 'managedIdentitiesModule'
  params: {
    managedIdentities: managedIdentities
    location: resourceGroup().location
  }
}

module rbacModule 'modules/rbac.bicep' = if (deployManagedIdentitiesAndRbac) {
  name: 'rbacModule'
  params: {
    managedIdentities: managedIdentitiesModule.outputs.identitiesInfo
  }
}




