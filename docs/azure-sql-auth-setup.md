# Azure SQL Authentication: Managed Identity & Service Principal Setup

This guide explains how to securely connect to Azure SQL from your .NET backend (using Managed Identity) and from your CI/CD pipeline (using a Service Principal, e.g., for DbUp migrations).

---

## 1. Managed Identity for .NET App (App Service)

### **A. Enable Managed Identity**
1. Go to your App Service in the Azure Portal.
2. Under **Identity** > **System assigned**, set Status to **On** and save.
3. Azure will create an identity in Azure AD for your App Service.

### **B. Grant Access to Azure SQL**
1. Connect to your Azure SQL Database as an admin (e.g., with Azure Data Studio or `sqlcmd`).
2. Run the following SQL to create a user for your App Service's managed identity and grant it a role (replace `<app-service-name>` with the name of your App Service):
   ```sql
   CREATE USER [<app-service-name>] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_owner ADD MEMBER [<app-service-name>];
   ```
   - You can use a more restrictive role if you prefer (e.g., `db_datareader`, `db_datawriter`).

### **C. Configure Connection String in App Service**
- Use the following format (no username or password needed):
  ```
  Server=tcp:<your-server>.database.windows.net,1433;Database=<your-db>;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;
  ```
- Set this as an environment variable or in App Service configuration.

### **D. How it Works**
- The .NET app running in App Service uses its managed identity to acquire a token from Azure AD.
- The token is used to authenticate to Azure SQL—no secrets required.

---

## 2. Service Principal for CI/CD Pipeline (DbUp Migrations)

### **A. Create a Service Principal (App Registration)**
1. In Azure Portal, go to **Azure Active Directory** > **App registrations** > **New registration**.
2. Register a new app (e.g., `my-ci-cd-pipeline-sp`).
3. Note the **Application (client) ID** and **Directory (tenant) ID**.
4. Create a **client secret** and note the value.

### **B. Grant Access to Azure SQL**
1. In your Azure SQL Database, run:
   ```sql
   CREATE USER [<service-principal-client-id>] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_owner ADD MEMBER [<service-principal-client-id>];
   ```
   - Use the **client ID** of your service principal as the username.

### **C. Configure Pipeline to Use Service Principal**
- In your CI/CD pipeline (GitHub Actions, Azure DevOps, etc.):
  1. Use the Azure CLI to log in with the service principal:
     ```sh
     az login --service-principal -u <client-id> -p <client-secret> --tenant <tenant-id>
     ```
  2. Use `SqlPackage`, `DbUp`, or other tools to run migrations, specifying AAD authentication.
  3. For DbUp, you may need to acquire an access token and pass it to the connection (see DbUp docs for AAD support).

### **D. How it Works**
- The pipeline logs in as the service principal, acquires a token, and uses it to authenticate to Azure SQL for migrations—no passwords in code or config.

---

## 3. Local Development
- For local dev, you can use SQL authentication, your own AAD user, or configure your dev environment to use your AAD identity.

---

## 4. Summary Table
| Scenario         | Auth Method         | How it works                        | Secretless? |
|------------------|--------------------|-------------------------------------|-------------|
| .NET backend     | Managed Identity   | Token from Azure AD                 | Yes         |
| CI/CD pipeline   | Service Principal  | Token from Azure AD                 | Yes         |
| Local dev        | AAD user or SQL    | Use your own AAD login or password  | No*         |

\* For local dev, you may use SQL auth or your own AAD login.

---

## 5. References
- [Azure SQL Managed Identity Docs](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-overview)
- [Connection Strings for AAD](https://learn.microsoft.com/en-us/azure/azure-sql/database/connect-azure-ad-authentication-configure)
- [Granting Access to Managed Identity](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-configure)

---

**In summary:**
- Use Managed Identity for your backend in Azure.
- Use a Service Principal for your CI/CD pipeline.
- Grant both identities access to your Azure SQL DB as AAD users.
- No passwords are needed—just tokens! 