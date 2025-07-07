param location string
param sqlServerName string
param sqlDbName string
param keyVaultResourceId string
param sqlIdentityResourceId string
@secure()
param sqlConnectionStringSecret string

// SQL identity with Key Vault Secrets Officer role can write the connection string secret
// The AVM SQL Server module will automatically export the connection string to Key Vault

// AVM SQL Server with secretsExportConfiguration
module sqlServer 'br/public:avm/res/sql/server:0.19.1' = {
  name: 'sqlServer'
  params: {
    name: sqlServerName
    location: location
    primaryUserAssignedIdentityResourceId: sqlIdentityResourceId
    databases: [
      {
        name: sqlDbName
        sku: {
          name: 'GP_S_Gen5_2'
          tier: 'GeneralPurpose'
          family: 'Gen5'
          capacity: 2
        }
        autoPauseDelay: 60
        minCapacity: '0.5'
        collation: 'Finnish_Swedish_CI_AS'
        availabilityZone: -1
        freeLimitExhaustionBehavior: 'AutoPause'
        useFreeLimit: true
      }
    ]
    secretsExportConfiguration: {
      keyVaultResourceId: keyVaultResourceId
      sqlAzureConnectionStringSecretName: sqlConnectionStringSecret
    }
    // For a production environment, we should use private endpoint and private DNS zone
    // Since these cost money, we'll just allow traffic from all Azure IPs to be able to
    // run migrations and access db from backend. The access is secured by using
    // managed-identity and Azure AD Authentication to Azure SQL.
    firewallRules: [
      {
        name: 'AllowAllAzureIPs'
        startIpAddress: '0.0.0.0'
        endIpAddress: '0.0.0.0'
      }
    ]
  }
}
