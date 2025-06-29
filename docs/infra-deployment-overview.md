# Infrastructure & App Deployment Overview

## 1. Infrastructure Deployment Flow

### A. Resource Group Creation (Subscription Scope)
- **Why:**  Azure requires resource groups to be created at the subscription level, not within a resource group. This is because a resource group must exist before you can deploy resources into it.
- **How:**  
  - You use a Bicep module (`infra/modules/resourcegroup/resourcegroup.bicep`) that references the [Azure Verified Module (AVM)](https://azure.github.io/verified-modules/) for resource group creation.
  - This step is run using the Azure CLI with `az deployment sub create ...`.
  - The resource group name and location are provided as parameters, sourced from your `environments.yml` file.

### B. Main Infrastructure Deployment (Resource Group Scope)
- **Why:**  Once the resource group exists, you can deploy all your Azure resources (App Service, Key Vault, etc.) into it.
- **How:**  
  - You use your main Bicep file (`infra/main.bicep`), which orchestrates the deployment of all resources.
  - This file uses AVM modules for each resource (e.g., App Service Plan, Web App, Key Vault), ensuring best practices and maintainability.
  - The deployment is run with `az deployment group create ...`, targeting the resource group created in the previous step.
  - All configuration values (resource names, locations, secrets, etc.) are passed as parameters, again sourced from your `environments.yml` file and environment-specific `.bicepparam` files.

---

## 2. App Deployment Flow (Future)

- **How:**  
  - When deploying a new version of your app (frontend or backend), you'll use a separate GitHub Actions workflow.
  - This workflow will also load environment-specific values from `environments.yml` (e.g., resource group name, app service name).
  - The deployment step (e.g., `az webapp deployment source config-zip ...`) will use these variables to target the correct Azure resources for the environment.

---

## 3. Configuration Management

### A. `environments.yml`
- Central YAML file at the repo root.
- Contains all environment-specific values (resource group, location, app service name, etc.) for `dev`, `prod`, etc.
- Loaded generically in GitHub Actions using `yq`, exporting all key-value pairs as environment variables.

### B. `.bicepparam` Files
- Used for passing parameters to Bicep templates.
- One per environment (e.g., `infra/parameters/dev.bicepparam`, `infra/parameters/prod.bicepparam`).
- Contains values for Bicep parameters that are not derived from the resource group (e.g., app settings, secret names).

### C. GitHub Actions Workflows
- Use a matrix to deploy to multiple environments.
- Use `yq` to load all environment variables from `environments.yml` for the selected environment.
- Pass these variables to Azure CLI commands for both infra and app deployments.

---

## 4. Why Separate Resource Group Creation?

- **Azure Limitation:**  Resource groups can only be created at the subscription scope, not within a resource group deployment.
- **Best Practice:**  Separating resource group creation ensures idempotency and clarity. You can safely run the creation step every time (it's a no-op if the group exists), and then deploy resources into it.
- **Modularity:**  This separation allows you to manage environments (dev, prod, etc.) cleanly and makes your deployments more robust and maintainable.

---

## 5. Use of AVM Modules

- **What:**  AVM modules are pre-built, best-practice Bicep modules for common Azure resources, maintained by Microsoft and the community.
- **Why:**  
  - Ensures your infra is built according to Azure best practices.
  - Reduces boilerplate and maintenance.
  - Makes upgrades and security fixes easier.
- **How:**  Your Bicep files import and use AVM modules for resources like App Service Plan, Web App, and Key Vault.

---

## 6. Azure Active Directory (Entra ID) and Resource Groups

- **Azure AD (now called Microsoft Entra ID) is a tenant-level (directory-level) service.**
- It does **not** reside in any resource group or subscription; instead, it exists at the directory (tenant) level, above all subscriptions and resource groups.
- All Azure subscriptions are associated with an Azure AD tenant (directory), which manages users, app registrations, service principals, and groups.
- When you sign up for Azure, you get a "default directory" (your Azure AD tenant), which is free for basic use (user management, app registrations, etc.).
- Premium features (like Conditional Access, Identity Protection, etc.) require a paid license, but for most app scenarios, the free tier is sufficient.

### **Resource Group vs. Directory**

| Feature/Resource         | Resource Group? | Directory (Tenant)? |
|-------------------------|-----------------|---------------------|
| App Service, Key Vault  | Yes             | No                  |
| Azure AD Users/Groups   | No              | Yes                 |
| App Registrations       | No              | Yes                 |
| Service Principals      | No              | Yes                 |
| Azure Subscriptions     | No              | Yes (linked)        |

**In summary:**
- Azure AD/Entra ID is not a resource group resource.
- It exists at the tenant (directory) level, above subscriptions and resource groups.
- You can use your default directory for authentication and app registrations, and it's free for most use cases.

---

## Summary Diagram

```mermaid
flowchart TD
    A[GitHub Actions Workflow] --> B[Load environments.yml for selected env]
    B --> C[Create Resource Group (az deployment sub create)]
    C --> D[Deploy Infra (az deployment group create)]
    D --> E[App Service, Key Vault, etc. via AVM modules]
    B --> F[App Deployment Workflow (future)]
    F --> G[Deploy App to App Service using env vars]
```

---

## In Practice

- **Infra deployment:**  
  1. Load config from `environments.yml`
  2. Create resource group (subscription scope)
  3. Deploy infra (resource group scope, using AVM modules)
- **App deployment:**  
  1. Load config from `environments.yml`
  2. Deploy app to correct App Service using loaded variables

---

**This setup gives you:**
- Clean separation of concerns
- DRY, maintainable configuration
- Best-practice, modular, and repeatable deployments for both infra and apps 