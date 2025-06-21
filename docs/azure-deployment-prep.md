# Preparing for Azure Deployment: Checklist & Best Practices

This document summarizes key topics and best practices to review before deploying your app to Azure. Use this as a pre-deployment checklist for a smooth, secure, and maintainable launch.

---

## 1. Environment Variables & Secrets
- **Never** hardcode secrets (client IDs, secrets, DB connection strings) in code or config files.
- Use **Azure Key Vault** or App Service configuration for secrets.
- Ensure local `.env`/`appsettings.Development.json` is not used in production.

---

## 2. Production-Ready Configurations
- **CORS:** Restrict allowed origins in production.
- **HTTPS:** Enforce HTTPS everywhere.
- **Logging:** Use Application Insights or another logging provider for production logs.
- **Error Handling:** Ensure user-friendly error pages and no stack traces are leaked to users.

---

## 3. Database
- **Connection Strings:** Use Azure SQL connection strings in production.
- **Migrations:** Automate migrations (e.g., run on startup or via pipeline).
- **Backups:** Set up automated backups for your Azure SQL database.

---

## 4. Authentication & Security
- **Azure AD App Registrations:** Ensure redirect URIs, logout URIs, and permissions are correct for your deployed domains.
- **Token Lifetime:** Consider if you want to adjust token lifetimes for production.
- **Session Management:** Review session timeout and refresh logic for user experience.

---

## 5. Frontend
- **Base URLs:** Make sure API/proxy URLs are environment-aware (local vs. Azure).
- **Static Assets:** If using images/videos, ensure they're hosted in a way that works in Azure (e.g., Blob Storage, public folder).

---

## 6. Deployment Pipeline
- Use GitHub Actions, Azure DevOps, or similar for CI/CD.
- Automate build, test, and deploy steps for both frontend and backend.
- Consider staging and production environments.

---

## 7. Monitoring & Alerts
- Set up Application Insights or another monitoring tool for both frontend and backend.
- Configure alerts for errors, downtime, or performance issues.

---

## 8. Cost Management
- Set up budgets and alerts in Azure Cost Management to avoid surprises.

---

## 9. Documentation
- Document your deployment process, environment variables, and any manual steps.

---

## 10. Final Local Checks
- Test with production-like settings (e.g., HTTPS, production DB, Azure AD config) locally if possible.
- Try a full login → API call → logout flow end-to-end.

---

**Summary:**
- Review this checklist before deploying to Azure for a smoother, more secure, and maintainable launch.
- Many of these topics can be addressed iteratively, but the more you prepare now, the fewer surprises you'll have in production. 