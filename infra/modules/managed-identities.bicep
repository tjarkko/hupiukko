param managedIdentities array
param location string

module identities 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.1' = [for mi in managedIdentities: {
  name: mi.name
  params: {
    name: mi.name
    location: location
  }
}]



output identitiesInfo array = [for (mi, i) in managedIdentities: {
  name: mi.name
  resourceId: identities[i].outputs.resourceId
  principalId: identities[i].outputs.principalId
  rbacAssignments: mi.rbacAssignments
}] 
