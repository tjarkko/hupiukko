targetScope = 'subscription'

@description('The name of the resource group to create, e.g., hupiukko-dev or hupiukko-prod')
param resourceGroupName string
@description('The Azure region for the resource group, e.g., westeurope')
param location string

module hupiukkoResourceGroup 'br/public:avm/res/resources/resource-group:0.4.1' = {
  name: 'hupiukkoResourceGroup'
  params: {
    name: resourceGroupName
    location: location
  }
} 
