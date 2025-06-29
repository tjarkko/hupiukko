# Hupiukko Infrastructure

This folder contains the Bicep templates and configuration for deploying the Hupiukko Azure infrastructure.

## Folder Structure

```
infra/
  main.bicep                # Entry point for deployment
  parameters/               # Environment-specific parameter files (.bicepparam)
    dev.bicepparam
  modules/                  # Bicep modules for each resource type
    appservice/
      appservice.bicep
    keyvault/
      keyvault.bicep
    resourcegroup/
      resourcegroup.bicep
  README.md                 # This file
```

At the repo root:
- `environments.yml` — Central environment configuration for all workflows and deployments
- `.github/workflows/deploy-infra.yml` — GitHub Actions workflow for infra deployment
- `docs/infra-deployment-overview.md` — Detailed documentation of the deployment approach

## Deployment Instructions

### 1. Prerequisites
- Install Azure CLI and Bicep
- Login to Azure:
  ```sh
  az login
  ```

### 2. Deploy the Resource Group (subscription scope)
```sh
az deployment sub create \
  --location <location> \
  --template-file modules/resourcegroup/resourcegroup.bicep \
  --parameters resourceGroupName=<your-resource-group> location=<location>
```

### 3. Deploy the Infrastructure (resource group scope)
```sh
az deployment group create \
  --resource-group <your-resource-group> \
  --template-file main.bicep \
  --parameters @parameters/dev.bicepparam
```

### 4. (Recommended) Use GitHub Actions
- See `.github/workflows/deploy-infra.yml` for automated, environment-aware deployment using `environments.yml`.

## Configuration
- **environments.yml** at the repo root holds all environment-specific values (resource group, location, app service name, etc.) for dev, prod, etc.
- **.bicepparam files** in `infra/parameters/` provide parameters for Bicep templates per environment.
- **No secrets** should be stored in parameter files; use Key Vault for secrets.

## Current Modules
- **App Service**: Hosts the Next.js frontend (AVM module)
- **Key Vault**: Stores secrets and configuration for the app (AVM module)
- **Resource Group**: Creates the resource group (AVM module)

---

For a detailed overview of the deployment process, configuration, and best practices, see [`../docs/infra-deployment-overview.md`](../docs/infra-deployment-overview.md). 