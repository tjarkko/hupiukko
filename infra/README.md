# Hupiukko Infrastructure

This folder contains the Bicep templates and configuration for deploying the Hupiukko Azure infrastructure.

## Folder Structure

```
infra/
  main.bicep                # Entry point for deployment
  parameters/               # Environment-specific parameter files
    dev.parameters.json
    prod.parameters.json
  modules/                  # Bicep modules for each resource type
    appservice/
      appservice.bicep
    keyvault/
      keyvault.bicep
  README.md                 # This file
  pipeline/                 # CI/CD pipeline definitions
    azure-pipelines.yml
```

## Deployment Instructions

1. **Install Azure CLI and Bicep**
2. **Login to Azure:**
   ```sh
   az login
   ```
3. **Deploy the infrastructure:**
   ```sh
   az deployment group create \
     --resource-group <your-resource-group> \
     --template-file main.bicep \
     --parameters @parameters/dev.parameters.json
   ```

## Adding New Resources
- Add a new module under `modules/` (e.g., `modules/storage/storage.bicep`).
- Reference the new module in `main.bicep`.
- Add any required parameters to the relevant parameter files.

## Parameters
- Store environment-specific values in `parameters/`.
- Do **not** store secrets in parameter files; use Key Vault for secrets.

## Current Modules
- **App Service**: Hosts the Next.js frontend.
- **Key Vault**: Stores secrets and configuration for the app.

---

For more details, see the comments in each Bicep file. 