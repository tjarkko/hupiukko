# VPN + Azure SQL Security Setup

This document explains how enterprise-grade VPN + Azure SQL security configurations work, providing maximum security for database access.

## Architecture Overview

```
Your Machine → VPN → Azure VNet → Private Endpoint → Azure SQL
```

This setup creates a secure tunnel from your local machine through Azure's private network to access SQL databases without exposing them to the public internet.

## Components

### 1. Azure Virtual Network (VNet)

- **Private network space** in Azure (e.g., `10.0.0.0/16`)
- **Subnets** for different resources (e.g., `10.0.1.0/24` for databases)
- **Network Security Groups (NSGs)** to control traffic flow
- **DNS resolution** within the private network

### 2. VPN Gateway

- **Point-to-Site (P2S)** VPN for individual users
- **Site-to-Site (S2S)** VPN for office networks
- **Azure AD authentication** for VPN access
- **Certificate-based authentication**
- **Encrypted tunnel** for all traffic

### 3. Private Endpoint for Azure SQL

- **Private IP** in your VNet (e.g., `10.0.1.5`)
- **No public IP exposure**
- **DNS resolution** within VNet
- **Direct connection** to Azure SQL service

### 4. Network Security Groups (NSGs)

```bicep
// Example NSG rule allowing SQL traffic only from VNet
{
  name: 'AllowSQLFromVNet'
  priority: 100
  sourceAddressPrefix: 'VirtualNetwork'
  destinationPortRange: '1433'
  access: 'Allow'
  protocol: 'Tcp'
}
```

## How It Works

### Connection Flow

1. **Connect to VPN** from your machine
2. **Get private IP** in Azure VNet (e.g., `10.0.2.10`)
3. **SSMS connects** to private endpoint (`10.0.1.5:1433`)
4. **Azure AD authentication** for SQL access
5. **Traffic stays** within Azure backbone

### Security Benefits

- **No public IP exposure** for SQL databases
- **Encrypted VPN tunnel** for all traffic
- **Azure AD authentication** (no passwords stored)
- **Network-level isolation**
- **Compliance-friendly** for enterprise requirements

## Implementation Options

### Option 1: Azure SQL Database + Private Endpoint

```bicep
// Private Endpoint for SQL Database
resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: 'sql-private-endpoint'
  location: location
  properties: {
    subnet: {
      id: vnet.properties.subnets[0].id
    }
    privateLinkServiceConnections: [
      {
        name: 'sql-connection'
        properties: {
          privateLinkServiceId: sqlServer.outputs.resourceId
          groupIds: ['sqlserver']
        }
      }
    ]
  }
}
```

### Option 2: SQL Managed Instance (Native VNet Support)

```bicep
// SQL Managed Instance can be deployed directly in VNet
resource managedInstance 'Microsoft.Sql/managedInstances@2021-11-01' = {
  name: 'my-sql-mi'
  location: location
  properties: {
    subnetId: vnet.properties.subnets[0].id  // Direct VNet placement!
    administratorLogin: 'sqladmin'
    administratorLoginPassword: adminPassword
    licenseType: 'LicenseIncluded'
    sku: {
      name: 'GP_Gen5'
      tier: 'GeneralPurpose'
      family: 'Gen5'
      capacity: 2
    }
  }
}
```

## Cost Considerations

| Component                | Monthly Cost | Notes                                    |
| ------------------------ | ------------ | ---------------------------------------- |
| **VPN Gateway**          | ~$27         | Basic SKU (VpnGw1)                       |
| **Private Endpoint**     | ~$7.50       | Per endpoint                             |
| **VNet**                 | Free         | Basic networking                         |
| **SQL Managed Instance** | ~$146        | Alternative to SQL DB + Private Endpoint |
| **Data transfer**        | Minimal      | VPN traffic                              |

## Comparison with Other Approaches

| Approach                   | Security | Cost         | Complexity | Use Case   |
| -------------------------- | -------- | ------------ | ---------- | ---------- |
| **Firewall Rules**         | Medium   | Free         | Low        | Dev/Test   |
| **Private Endpoint**       | High     | ~$7.50/month | Medium     | Production |
| **VPN + Private Endpoint** | Highest  | ~$35/month   | High       | Enterprise |
| **SQL Managed Instance**   | High     | ~$146/month  | Medium     | Enterprise |

## Why Private Endpoints Are Needed

### PaaS vs IaaS Services

**IaaS Services (Can Be Put Directly in VNet):**

- Virtual Machines
- Virtual Machine Scale Sets
- Load Balancers
- Application Gateways

**PaaS Services (Cannot Be Put Directly in VNet):**

- Azure SQL Database (requires Private Endpoint)
- Azure Storage (requires Private Endpoint)
- Azure Key Vault (requires Private Endpoint)
- Azure App Service (VNet-integrated but still public)

### The Problem Without Private Endpoints

```
Your VNet (10.0.0.0/16)
├── VM (10.0.1.5) ✅ Can be private
├── App Service (10.0.1.10) ✅ Can be private
└── Azure SQL ❌ Always public (server.database.windows.net)
```

**Without Private Endpoint:**

- Traffic goes: `VNet → Internet → Azure SQL public endpoint`
- Less secure - traffic traverses internet

**With Private Endpoint:**

- Traffic stays: `VNet → Private IP (10.0.1.100) → Azure SQL`
- More secure - traffic never leaves Azure backbone

## Alternative: Azure Bastion

If VPN is too expensive, **Azure Bastion** is another option:

- **Browser-based RDP/SSH** to VMs in VNet
- **No client software needed**
- **Azure AD authentication**
- **~$140/month** (more expensive but easier)

## Best Practices

### For Development/Testing

- Use **firewall rules** with Azure-only access
- **Free tier** SQL Database
- **Managed identity** for app authentication
- **Azure Portal Query Editor** for debugging

### For Production/Enterprise

- **Private endpoints** for all PaaS services
- **VPN Gateway** for secure remote access
- **SQL Managed Instance** if budget allows
- **Comprehensive monitoring** and auditing

### For Maximum Security

- **VPN + Private Endpoints**
- **Azure AD admin only** (no SQL authentication)
- **Auditing and threat detection**
- **Network security groups** with least privilege

## References

- [Azure VPN Gateway documentation](https://learn.microsoft.com/en-us/azure/vpn-gateway/)
- [Azure Private Endpoints](https://learn.microsoft.com/en-us/azure/private-link/private-endpoint-overview)
- [Azure SQL Managed Instance overview](https://learn.microsoft.com/en-us/azure/azure-sql/managed-instance/sql-managed-instance-paas-overview)
- [Azure SQL Database security best practices](https://learn.microsoft.com/en-us/azure/azure-sql/database/security-overview)

---

_This setup provides enterprise-grade security but comes with increased complexity and cost. For development and testing environments, simpler approaches like firewall rules with Azure-only access are often sufficient._
