# Next.js + Azure Key Vault Integration Guide

## Overview
This document describes how to securely handle environment variables and secrets in a Next.js application deployed to Azure App Service, using Azure Key Vault for secret management.

## Architecture

### Components
1. **Next.js Application**
   - Custom server setup for proper environment variable handling
   - Separation of public and private environment variables
   - TypeScript support

2. **Azure Key Vault**
   - Stores all sensitive secrets
   - Free tier available (10,000 transactions/month)
   - Automatic secret rotation support

3. **Azure App Service**
   - Hosts the Next.js application
   - Integrates with Key Vault
   - Resolves Key Vault references at runtime

## Setup Steps

### 1. Local Development Setup

#### Environment Files
- Create `.env.local` for local development (not committed to git)
- Create `.env.local.example` as a template (committed to git)
- Add `.env.local` to `.gitignore`

Example `.env.local`:
```env
# Public variables (available in browser)
NEXT_PUBLIC_API_URL=http://localhost:5000

# Private variables (server-side only)
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=your-development-secret

# Azure AD Configuration
AZURE_AD_CLIENT_ID=your-client-id
AZURE_AD_CLIENT_SECRET=your-client-secret
AZURE_AD_TENANT_ID=your-tenant-id
```

### 2. Next.js Configuration

#### Custom Server (server.js)
```javascript
const { createServer } = require('http');
const { parse } = require('url');
const next = require('next');

const dev = process.env.NODE_ENV !== 'production';
const app = next({ dev });
const handle = app.getRequestHandler();

// Ensure all environment variables are loaded
require('dotenv').config();

app.prepare().then(() => {
  createServer((req, res) => {
    const parsedUrl = parse(req.url, true);
    handle(req, res, parsedUrl);
  }).listen(process.env.PORT || 3000, (err) => {
    if (err) throw err;
    console.log('> Ready on http://localhost:' + (process.env.PORT || 3000));
  });
});
```

#### Next.js Config (next.config.js)
```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  
  // Public variables (available in browser)
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
  },
  
  // Server-side only variables
  serverRuntimeConfig: {
    NEXTAUTH_SECRET: process.env.NEXTAUTH_SECRET,
    NEXTAUTH_URL: process.env.NEXTAUTH_URL,
  },
}

module.exports = nextConfig
```

### 3. Azure Key Vault Setup

1. Create Key Vault in Azure Portal
2. Add secrets to Key Vault:
   - `nextauth-secret`
   - `azure-ad-client-secret`
   - Other sensitive configuration

### 4. App Service Configuration

In Azure Portal, configure App Service Application Settings with Key Vault references:

```
NEXTAUTH_SECRET=@Microsoft.KeyVault(SecretUri=https://your-vault.vault.azure.net/secrets/nextauth-secret)
NEXTAUTH_URL=https://your-app.azurewebsites.net
AZURE_AD_CLIENT_SECRET=@Microsoft.KeyVault(SecretUri=https://your-vault.vault.azure.net/secrets/azure-ad-client-secret)
```

## Security Considerations

1. **Secret Management**
   - All secrets stored in Key Vault
   - No secrets in code or container images
   - Automatic secret rotation support

2. **Environment Variables**
   - Public variables prefixed with `NEXT_PUBLIC_`
   - Private variables only available server-side
   - Clear separation of concerns

3. **Access Control**
   - Key Vault access controlled via Azure RBAC
   - App Service managed identity for secure access
   - Audit logging available

## Cost Considerations

### Key Vault Free Tier
- 10,000 transactions per month free
- Typical usage for demo project:
  - 2-3 reads per app startup
  - 1-2 writes per deployment
  - ~100-200 transactions per month

### Optimization Tips
1. Cache secrets where possible
2. Use App Service Configuration for non-sensitive values
3. Monitor usage in Azure Portal

## Troubleshooting

### Common Issues

1. **Environment Variables Not Available**
   - Check if using `serverRuntimeConfig` correctly
   - Verify Key Vault references in App Service
   - Check Key Vault access permissions

2. **Key Vault Access Issues**
   - Verify App Service managed identity
   - Check Key Vault access policies
   - Review Azure RBAC assignments

3. **Build/Deployment Issues**
   - Ensure all required environment variables are set
   - Check build logs for missing variables
   - Verify deployment configuration

## Best Practices

1. **Development**
   - Use `.env.local` for local development
   - Never commit sensitive values to git
   - Share `.env.local.example` with team

2. **Production**
   - Use Key Vault for all secrets
   - Enable audit logging
   - Regular secret rotation

3. **Monitoring**
   - Set up alerts for Key Vault usage
   - Monitor transaction counts
   - Review audit logs regularly

## References

- [Next.js Environment Variables](https://nextjs.org/docs/basic-features/environment-variables)
- [Azure Key Vault Documentation](https://docs.microsoft.com/en-us/azure/key-vault/)
- [App Service Key Vault Integration](https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references) 