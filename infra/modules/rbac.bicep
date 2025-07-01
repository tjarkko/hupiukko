param managedIdentities array

// Flatten and map to include principalId and all needed fields for RBAC
var rbacAssignments = flatten(
  map(managedIdentities, identity => 
    map(identity.rbacAssignments, assignment => 
      {
        identityName: identity.name
        principalId: identity.principalId
        roleDefinitionIdOrName: assignment.roleDefinitionIdOrName
        description: assignment.description
      }
    )
  )
) 

resource rbacAssignmentsRes 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for assignment in rbacAssignments: {
  name: guid(assignment.identityName, assignment.roleDefinitionIdOrName)
  scope: resourceGroup()
  properties: {
    principalId: assignment.principalId
    roleDefinitionId: assignment.roleDefinitionIdOrName
    principalType: 'ServicePrincipal'
    description: assignment.description
  }
}]

output rbacAssignments array = rbacAssignments
