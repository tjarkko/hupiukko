param location string
param keyVaultName string

// Use AVM Key Vault module
module keyVault 'br/public:avm/res/key-vault/vault:0.13.0' = {
  name: 'keyVault'
  params: {
    name: keyVaultName
    location: location
    sku: 'standard'
    // Add more AVM params as needed
  }
}

output keyVaultId string = keyVault.outputs.resourceId 
