# Script to assign permissions to an existing UMI 
# The following required Microsoft Graph permissions will be assigned: 
#   User.Read.All
#   GroupMember.Read.All
#   Application.Read.All

Import-Module Microsoft.Graph.Authentication
Import-Module Microsoft.Graph.Applications

$tenantId = "287fc32d-9911-40b2-a2c2-a6419fd5e4c9"        # Your tenant ID
$MSIName = "hupiukko-sql-identity"; # Name of your managed identity

# Log in as a user with the "Privileged Role Administrator" role
Connect-MgGraph -TenantId $tenantId -Scopes "AppRoleAssignment.ReadWrite.All,Application.Read.All"

# Search for Microsoft Graph
$MSGraphSP = Get-MgServicePrincipal -Filter "DisplayName eq 'Microsoft Graph'";
$MSGraphSP

# Sample Output

# DisplayName     Id                                   AppId                                SignInAudience      ServicePrincipalType
# -----------     --                                   -----                                --------------      --------------------
# Microsoft Graph 47d73278-e43c-4cc2-a606-c500b66883ef 00000003-0000-0000-c000-000000000000 AzureADMultipleOrgs Application

$MSI = Get-MgServicePrincipal -Filter "DisplayName eq '$MSIName'" 
if($MSI.Count -gt 1)
{ 
Write-Output "More than 1 principal found with that name, please find your principal and copy its object ID. Replace the above line with the syntax $MSI = Get-MgServicePrincipal -ServicePrincipalId <your_object_id>"
Exit
}

# Get required permissions
$Permissions = @(
  "User.Read.All"
  "GroupMember.Read.All"
  "Application.Read.All"
)

# Find app permissions within Microsoft Graph application
$MSGraphAppRoles = $MSGraphSP.AppRoles | Where-Object {($_.Value -in $Permissions)}

# Assign the managed identity app roles for each permission
foreach($AppRole in $MSGraphAppRoles)
{
    $AppRoleAssignment = @{
	    principalId = $MSI.Id
	    resourceId = $MSGraphSP.Id
	    appRoleId = $AppRole.Id
    }

New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $AppRoleAssignment.PrincipalId -BodyParameter $AppRoleAssignment -Verbose
}