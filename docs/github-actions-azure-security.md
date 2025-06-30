# GitHub Actions & Azure Security: Service Principal Authentication

This document explains how authentication and security are handled when deploying Azure resources from GitHub Actions, focusing on the use of a service principal and best practices for permissions and secret management.

---

## 1. Service Principal Authentication

To allow GitHub Actions to deploy resources to Azure, a **service principal** (an Azure AD application identity) is created and granted permissions to the relevant Azure resources.

### **Service Principal Creation Command**

The following command creates a service principal with the **Contributor** role, scoped only to a specific resource group (replace the placeholders with your actual values):

```sh
az ad sp create-for-rbac \
  --name "github-actions-<project>" \
  --role Contributor \
  --scopes /subscriptions/<SUBSCRIPTION_ID>/resourceGroups/<RESOURCE_GROUP_NAME> \
  --sdk-auth
```

- `<SUBSCRIPTION_ID>`: Your Azure subscription ID (get it with `az account show --query id -o tsv`)
- `<RESOURCE_GROUP_NAME>`: The name of your resource group (created beforehand in the Azure Portal)

**Example:**
```sh
az ad sp create-for-rbac \
  --name "github-actions-hupiukko" \
  --role Contributor \
  --scopes /subscriptions/12345678-90ab-cdef-1234-567890abcdef/resourceGroups/hupiukko-dev-rg \
  --sdk-auth
```

The output is a JSON object containing credentials. **Copy the entire JSON and add it as a GitHub Actions secret named `AZURE_CREDENTIALS`.**

---

## 2. Permissions and Scope

- **Role:** `Contributor` — allows the service principal to create, update, and delete resources within the specified resource group, but not manage access (RBAC) or subscription-level settings.
- **Scope:** The role is assigned only to the resource group, following the principle of least privilege.
- **Resource Group:** The resource group is created manually in the Azure Portal before running deployments. The service principal cannot create or delete resource groups, only manage resources within the assigned group.

---

## 3. GitHub Actions Secret Management

- The service principal credentials (JSON output) are stored as a **GitHub Actions secret** (`AZURE_CREDENTIALS`).
- **Secrets are encrypted and never visible to anyone via the GitHub UI or API, even in public repositories.**
- Only trusted users (with write access) can run workflows that access secrets.
- **Secrets are not available to workflows triggered by external pull requests from forks.**
- Never print secrets in logs or commit them to the repository.

---

## 4. Security Best Practices

- **Principle of Least Privilege:** Grant only the permissions and scope needed for deployments (resource group Contributor, not subscription-wide).
- **Manual Resource Group Creation:** The resource group is created manually to allow for tighter permission control.
- **Secret Hygiene:** Store credentials only in GitHub Actions secrets, not in code or local files.
- **Monitor and Rotate:** Regularly review access, monitor usage, and rotate secrets if needed.

---

For more details on the overall deployment process, see [`infra-deployment-overview.md`](./infra-deployment-overview.md). 