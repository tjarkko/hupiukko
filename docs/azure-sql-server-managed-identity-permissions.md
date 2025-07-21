# Granting Microsoft Graph Permissions to Azure SQL Server Managed Identity

This guide explains how to grant the necessary Microsoft Graph permissions to your Azure SQL Server's managed identity so it can resolve Azure AD (Microsoft Entra) principals for user creation and authentication.

## Required Permissions

Grant the following **application permissions** to the managed identity:

- `User.Read.All`
- `GroupMember.Read.All`
- `Application.Read.All`

These permissions allow the SQL Server to look up users, groups, and service principals in Microsoft Entra ID (Azure AD).

## Assign Permissions via Azure Portal

1. **Find the Managed Identity**

   - Go to the [Azure Portal](https://portal.azure.com/).
   - Navigate to **Microsoft Entra ID** (Azure Active Directory).
   - Select **Enterprise applications**.
   - Search for your managed identity (e.g., `hupiukko-sql-identity`).

2. **Assign Microsoft Graph Permissions**

   - Click on your managed identity in the list.
   - In the left menu, select **Permissions** (under Security).
   - Click **Add a permission**.
   - Select **Microsoft Graph**.
   - Choose **Application permissions**.
   - Add:
     - `User.Read.All`
     - `GroupMember.Read.All`
     - `Application.Read.All`
   - Click **Add permissions**.

3. **Grant Admin Consent**

   - After adding, click **Grant admin consent for <tenant>**.
   - You must be a Privileged Role Administrator or Global Administrator to do this.

4. **Verify**
   - Ensure the permissions show as "Granted for <tenant>".

## Notes

- You must have sufficient directory permissions to add and consent to these permissions.
- Setting yourself as "Owner" of the managed identity is not enough; you need Entra ID admin rights.
- These permissions are required for the SQL Server to resolve Azure AD users, groups, and service principals for `CREATE USER FROM EXTERNAL PROVIDER` and similar operations.

## References

- [Microsoft Docs: Managed identities in Microsoft Entra for Azure SQL](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-azure-ad-user-assigned-managed-identity?view=azuresql)
