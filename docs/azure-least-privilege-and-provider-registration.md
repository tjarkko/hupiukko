# Azure Least Privilege and Resource Provider Registration in Production

This document explains how to apply the principle of least privilege for CI/CD in Azure, why resource provider registration is required at the subscription level, and how a secure production workflow is typically structured.

---

## 1. Why Least Privilege?

- **Least privilege** means granting only the minimum permissions necessary for a user or service to do its job.
- In Azure, this reduces the risk of accidental or malicious changes to resources outside the intended scope (e.g., other environments, the whole subscription).
- For CI/CD, this means the service principal (SP) used by your pipelines should only have access to the resource group(s) it needs to manage.

---

## 2. Resource Provider Registration

- Azure resources (like Key Vault, App Service, etc.) are managed by **resource providers** (e.g., `Microsoft.KeyVault`).
- **Resource providers must be registered at the subscription level** before you can deploy resources of that type.
- Registration is a one-time, subscription-wide operation and requires Owner or Contributor rights at the subscription scope.
- CI/CD service principals with only resource group rights **cannot register providers**—this must be done by a subscription admin.

---

## 3. Typical Production Workflow

| Step                        | Who does it?             | Scope/Permissions         |
|-----------------------------|--------------------------|--------------------------|
| Register resource providers | Subscription admin       | Subscription             |
| Create resource groups      | Admin or pipeline        | Subscription             |
| Deploy resources            | CI/CD service principal  | Resource group only      |

### **Step-by-step:**
1. **Initial Setup (One-Time, Manual):**
   - Subscription admin registers all required resource providers (e.g., `Microsoft.KeyVault`, `Microsoft.Web`).
2. **Resource Group Creation:**
   - Admin creates resource groups for each environment (dev, staging, prod).
3. **Service Principal with Least Privilege:**
   - CI/CD SP is granted Contributor (or custom) role only on the required resource group(s).
4. **Ongoing Deployments:**
   - CI/CD SP can deploy/update/delete resources in its assigned group(s), but cannot register providers or affect other groups.
   - If a new resource type is needed, an admin registers the provider at the subscription level.

---

## 4. Best Practices
- **Document required resource providers** for your solution and register them up front.
- **Limit CI/CD permissions** to only the resource groups needed.
- **Rotate and monitor SP credentials** regularly.
- **Have a process for requesting new provider registration** if needed in the future.

---

## 5. References
- [Azure Resource Providers and Types](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-providers-and-types)
- [Register resource provider (CLI)](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-providers-and-types#register-resource-provider)

---

**In summary:**
- Use least privilege for CI/CD in production.
- Register resource providers once, at the subscription level, as an admin.
- After setup, your pipelines are secure and limited in scope. 