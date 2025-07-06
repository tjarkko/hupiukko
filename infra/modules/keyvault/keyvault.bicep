// the scope, the deployment deploys resources to
targetScope = 'resourceGroup'

param location string
param keyVaultName string
@description('Disable for development deployments.')
param enablePurgeProtection bool = true
@description('Array of secret names to create in Key Vault')
param secretNames array = []

// Use AVM Key Vault module
module keyVault 'br/public:avm/res/key-vault/vault:0.13.0' = {
  name: 'keyVaultAVM'
  params: {
    name: keyVaultName
    location: location
    sku: 'standard'
    enablePurgeProtection: enablePurgeProtection
    secrets: map(secretNames, secretName => {
      name: secretName
      value: ''
    })
  }
}

output keyVaultId string = keyVault.outputs.resourceId
output keyVaultUri string = keyVault.outputs.uri
