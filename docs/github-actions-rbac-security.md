# Security Aspects of RBAC Assignment in CI/CD (GitHub Actions + Azure Bicep)

## Why Elevated Permissions Are Needed

To automate the creation of Azure RBAC assignments (e.g., granting your managed identities access to Key Vault or other resources) in your CI/CD pipeline, the service principal (SP) used by GitHub Actions must have the ability to create and manage role assignments. This requires the **User Access Administrator** role (or Owner, but UAA is preferred for least privilege) at the resource group or subscription scope.

## Risks of Granting Elevated Permissions to CI/CD

- **Privilege Escalation:** If the CI/CD SP is compromised, an attacker could grant themselves or others access to sensitive resources.
- **Broad Access:** Assigning at the subscription level increases risk; prefer resource group scope.
- **Standing Privilege:** Permanent assignment of UAA means the SP always has the ability to change access, increasing the attack surface.

## Best Practices for Minimizing Risk

1. **Scope Minimization:**
   - Assign the UAA role at the **resource group** level, not subscription, unless absolutely necessary.
2. **Principle of Least Privilege:**
   - Only assign UAA if your pipeline needs to manage RBAC. Use Contributor for resource deployment only.
3. **Secrets Management:**
   - Store SP credentials in GitHub Secrets or a secure vault. Rotate regularly.
4. **Monitoring and Auditing:**
   - Enable Azure Activity Logs. Set up alerts for role assignment changes.
5. **Dedicated Service Principal:**
   - Use a dedicated SP for CI/CD, not a personal or shared one. Separate SPs for dev/prod if possible.
6. **Remove When Not Needed:**
   - Remove the UAA role after initial setup if ongoing RBAC changes are not required.
7. **Privileged Identity Management (PIM):**
   - For production, use Azure AD PIM to make UAA assignments just-in-time and require approval.

## Recommendations for This Project

- Assign **User Access Administrator** to your GitHub Actions SP **only at the resource group level** (e.g., `hupiukko-dev-rg`).
- Store the SP credentials securely in GitHub Secrets.
- Monitor and review RBAC changes regularly.
- Remove the UAA role if you no longer need to automate RBAC assignments.

## Example: Assigning UAA Role at Resource Group Level

```sh
az role assignment create \
  --assignee <servicePrincipalObjectId> \
  --role "User Access Administrator" \
  --scope /subscriptions/<subscriptionId>/resourceGroups/hupiukko-dev-rg
```

## Further Reading
- [Azure RBAC documentation](https://learn.microsoft.com/en-us/azure/role-based-access-control/overview)
- [User Access Administrator role](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#user-access-administrator)
- [Azure AD Privileged Identity Management](https://learn.microsoft.com/en-us/azure/active-directory/privileged-identity-management/pim-configure) 