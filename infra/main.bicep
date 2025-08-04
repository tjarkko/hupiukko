// main.bicep - Entry point for Hupiukko infra
// 'deployKeyVault' and 'deployAppService' are controlled by the 'services' input in the pipeline.
// 'all' means both are true; 'keyvault' or 'appservice' means only that service is deployed.

param environment string
param frontendAppServiceName string
param backendAppServiceName string
param keyVaultName string
param NEXT_PUBLIC_API_URL string
param NEXTAUTH_URL string
param AZURE_AD_CLIENT_ID string
param AZURE_AD_TENANT_ID string
param frontendIdentityResourceIds array = []
param backendIdentityResourceIds array = []
param backendAppServiceAzureAd object
param appServicePlanName string
param keyVaultUri string
param managedIdentities array
@description('Startup command for the App Service (e.g., node server.js for Next.js standalone output)')
param startupCommand string = 'node server.js'
@description('Resource ID of the Key Vault to use for SQL secrets')
param keyVaultResourceId string
@description('Resource ID of the managed identity for SQL operations')
param sqlIdentityResourceId string
@description('Name of the SQL Server')
param sqlServerName string
@description('Name of the SQL Database')
param sqlDbName string
@secure()
@description('Name of the SQL connection string secret in Key Vault')
param sqlConnectionStringSecret string
@secure()
@description('Name of the SQL connection string secret for backend app in Key Vault')
param sqlConnectionStringBackendSecret string
@description('Array of secret names to create in Key Vault')
param keyVaultSecretNames array = []

@description('Set to true to deploy the Key Vault module')
param deployKeyVault bool = true
@description('Set to true to deploy the App Service module')
param deployAppService bool = true
@description('Set to true to deploy both managed identities and RBAC assignments')
param deployManagedIdentitiesAndRbac bool = true
@description('Set to true to deploy the Azure SQL module')
param deployAzureSql bool = false

output environment string = environment
output frontendAppServiceName string = frontendAppServiceName
output backendAppServiceName string = backendAppServiceName
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
    secretNames: keyVaultSecretNames
  }
}

module appService 'modules/appservice/appservice.bicep' = if (deployAppService) {
  name: 'appService'
  params: {
    location: resourceGroup().location
    frontendAppServiceName: frontendAppServiceName
    backendAppServiceName: backendAppServiceName
    appServicePlanName: appServicePlanName
    keyVaultUri: keyVaultUri
    environment: environment
    NEXT_PUBLIC_API_URL: NEXT_PUBLIC_API_URL
    NEXTAUTH_URL: NEXTAUTH_URL
    AZURE_AD_CLIENT_ID: AZURE_AD_CLIENT_ID
    AZURE_AD_TENANT_ID: AZURE_AD_TENANT_ID
    frontendIdentityResourceIds: frontendIdentityResourceIds
    backendIdentityResourceIds: backendIdentityResourceIds
    backendAppServiceAzureAd: backendAppServiceAzureAd
    sqlConnectionStringSecret: sqlConnectionStringSecret
    sqlConnectionStringBackendSecret: sqlConnectionStringBackendSecret
    startupCommand: startupCommand
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

module azuresql 'modules/azuresql.bicep' = if (deployAzureSql) {
  name: 'azuresql'
  params: {
    location: resourceGroup().location
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
    keyVaultResourceId: keyVaultResourceId
    sqlIdentityResourceId: sqlIdentityResourceId
    sqlConnectionStringSecret: sqlConnectionStringSecret
  }
}
