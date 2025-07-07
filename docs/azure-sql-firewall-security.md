# Azure SQL Firewall Security

This document explains the Azure SQL firewall security approach used in the Hupiukko demo project.

## Current Approach: Allow All Azure IPs

### Firewall Rule Configuration

```bicep
firewallRules: [
  {
    name: 'AllowAllAzureIPs'
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
]
```

### What This Allows

- ✅ **All Azure services** can connect to the SQL database
- ✅ **GitHub Actions runners** (they run on Azure infrastructure)
- ✅ **Azure App Services** (your backend API)
- ✅ **Azure Container Instances** (if used)
- ✅ **Any other Azure service**

### What This Does NOT Allow

- ❌ **Direct internet access** from non-Azure sources
- ❌ **On-premises networks** (unless they have Azure connectivity)
- ❌ **Personal computers** (unless using Azure VPN or ExpressRoute)

## Security Considerations

### Pros of Current Approach

- **Cost-effective** - No additional Azure services needed
- **Simple setup** - Single firewall rule covers all Azure services
- **Sufficient for demo** - Meets the project's security requirements
- **Azure AD authentication** - Still requires proper authentication

### Cons of Current Approach

- **Less secure** - Any Azure service can attempt connection
- **No network isolation** - Traffic goes through Azure's public network
- **Not enterprise-grade** - Would need private endpoints for production

## Authentication vs Network Security

### Current Security Model

```
GitHub Actions → Azure Network → SQL Database
     ↓              ↓              ↓
Service Principal → Firewall → Azure AD Auth
```

- **Network level**: Allow Azure IPs (0.0.0.0)
- **Authentication level**: Azure AD (service principal + managed identity)
- **Database level**: SQL users with specific permissions

### Why This Works for Demo

1. **Service Principal** has Azure AD authentication
2. **Managed Identity** has Azure AD authentication
3. **SQL users** have specific database permissions
4. **No passwords** stored in code or configuration

## Production Considerations

### For Enterprise Use

If this were a production system, consider:

1. **Private Endpoints**

   - Traffic stays within Azure backbone
   - No public IP exposure
   - Higher cost (~$7.50/month per endpoint)

2. **VPN Gateway**

   - Secure tunnel from on-premises
   - Point-to-Site for individual users
   - Site-to-Site for office networks

3. **Network Security Groups (NSGs)**

   - Fine-grained traffic control
   - Subnet-level security rules
   - Logging and monitoring

4. **Azure Bastion**
   - Browser-based RDP/SSH access
   - No client software needed
   - Higher cost (~$140/month)

## Cost Comparison

| Approach                      | Monthly Cost | Security Level | Complexity |
| ----------------------------- | ------------ | -------------- | ---------- |
| **Current (Allow Azure IPs)** | Free         | Medium         | Low        |
| **Private Endpoint**          | ~$7.50       | High           | Medium     |
| **VPN Gateway**               | ~$27         | High           | High       |
| **Azure Bastion**             | ~$140        | High           | Low        |

## Best Practices for Current Setup

### 1. **Use Azure AD Authentication**

- No SQL authentication (username/password)
- Service principals and managed identities only
- Automatic token rotation

### 2. **Principle of Least Privilege**

- Service principal: `db_ddladmin`, `db_datareader`, `db_datawriter`
- Managed identity: `db_datareader`, `db_datawriter`
- No `db_owner` access

### 3. **Monitor Access**

- Enable SQL auditing
- Review access logs regularly
- Set up alerts for unusual activity

### 4. **Regular Security Reviews**

- Review firewall rules quarterly
- Update service principal permissions as needed
- Monitor for new Azure services that might need access

## Migration Path to Production

When ready to move to production:

1. **Add Private Endpoint** for SQL Database
2. **Remove public firewall rules**
3. **Update connection strings** to use private endpoint
4. **Add monitoring and alerting**
5. **Implement backup and disaster recovery**

## References

- [Azure SQL Database firewall rules](https://learn.microsoft.com/en-us/azure/azure-sql/database/firewall-configure)
- [Azure SQL Database security overview](https://learn.microsoft.com/en-us/azure/azure-sql/database/security-overview)
- [Private endpoints for Azure SQL](https://learn.microsoft.com/en-us/azure/azure-sql/database/private-endpoint-overview)

---

_This approach balances security and cost for a demo project. For production environments, consider implementing private endpoints and more restrictive network policies._
