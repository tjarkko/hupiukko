# Hupiukko - System Architecture Diagram

## Overview

Hupiukko is a full-stack fitness application built with Next.js frontend, .NET 9 backend, and Azure infrastructure. The system uses a BFF (Backend for Frontend) pattern with Azure AD authentication.

## High-Level Architecture

```mermaid
graph TB
    subgraph "Client Layer"
        User[User Browser]
    end

    subgraph "Frontend Layer (Next.js)"
        NextJS[Next.js App Service]
        Auth[NextAuth.js]
        Proxy[API Proxy Routes]
        UI[React Components]
    end

    subgraph "Backend Layer (.NET 9)"
        DotNet[ASP.NET Core API]
        Controllers[API Controllers]
        BusinessLogic[Business Logic Layer]
        DbContext[EF Core DbContext]
    end

    subgraph "Data Layer"
        AzureSQL[Azure SQL Database]
        Migrations[DbUp Migrations]
    end

    subgraph "Infrastructure Layer"
        KeyVault[Azure Key Vault]
        ManagedIdentities[Managed Identities]
        RBAC[RBAC & Permissions]
    end

    subgraph "Authentication Layer"
        AzureAD[Azure AD / Entra ID]
    end

    User --> NextJS
    NextJS --> Auth
    Auth --> AzureAD
    NextJS --> Proxy
    Proxy --> DotNet
    DotNet --> BusinessLogic
    BusinessLogic --> DbContext
    DbContext --> AzureSQL
    DotNet --> KeyVault
    DotNet --> ManagedIdentities
    NextJS --> KeyVault
    NextJS --> ManagedIdentities
```

## Detailed Component Architecture

### Frontend (Next.js)

```mermaid
graph LR
    subgraph "Next.js Application"
        Pages[App Router Pages]
        Components[React Components]
        API[API Routes]
        Auth[NextAuth.js]
        Proxy[Proxy Routes]
        Generated[Generated API Client]
    end

    subgraph "Dependencies"
        MUI[Material-UI]
        ReactQuery[React Query]
        Tailwind[Tailwind CSS]
        Orval[Orval API Generator]
    end

    Pages --> Components
    Components --> Generated
    Generated --> Proxy
    Proxy --> API
    Auth --> API
    Components --> MUI
    Components --> ReactQuery
    Components --> Tailwind
```

### Backend (.NET 9)

```mermaid
graph LR
    subgraph "ASP.NET Core API"
        Controllers[API Controllers]
        DTOs[Data Transfer Objects]
        BusinessLogic[Business Logic]
        Models[EF Core Models]
        Profiles[AutoMapper Profiles]
        Validation[FluentValidation]
    end

    subgraph "Services"
        UsersManager[Users Manager]
        ExercisesManager[Exercises Manager]
        WorkoutManager[Workout Manager]
    end

    subgraph "Data Access"
        DbContext[ApplicationDbContext]
        Repository[IRepository Pattern]
        Migrations[DbUp Migrations]
    end

    Controllers --> DTOs
    Controllers --> BusinessLogic
    BusinessLogic --> Services
    Services --> Models
    Models --> DbContext
    DbContext --> Repository
    Validation --> DTOs
    Profiles --> DTOs
```

## Authentication Flow (BFF Pattern)

```mermaid
sequenceDiagram
    participant User
    participant NextJS as Next.js Frontend
    participant Auth as NextAuth.js
    participant AzureAD as Azure AD
    participant Proxy as API Proxy
    participant Backend as .NET Backend
    participant KeyVault as Azure Key Vault

    User->>NextJS: Access protected page
    NextJS->>Auth: Check session
    Auth->>AzureAD: Validate token/Get new token
    AzureAD-->>Auth: Access token
    Auth-->>NextJS: Session with token

    User->>NextJS: Make API call
    NextJS->>Proxy: Forward request with token
    Proxy->>Backend: HTTP request + Bearer token
    Backend->>Backend: Validate JWT token
    Backend->>KeyVault: Access secrets (if needed)
    Backend-->>Proxy: API response
    Proxy-->>NextJS: Response
    NextJS-->>User: Display data
```

## Infrastructure Architecture

### Azure Resources

```mermaid
graph TB
    subgraph "Resource Group"
        subgraph "Compute"
            FrontendApp[Frontend App Service]
            BackendApp[Backend App Service]
            AppServicePlan[App Service Plan]
        end

        subgraph "Data"
            SQLServer[Azure SQL Server]
            SQLDB[Azure SQL Database]
        end

        subgraph "Security"
            KeyVault[Key Vault]
            FrontendMI[Frontend Managed Identity]
            BackendMI[Backend Managed Identity]
            SQLMI[SQL Managed Identity]
        end

        subgraph "Networking"
            VNet[Virtual Network]
            Subnets[Subnets]
            NSG[Network Security Groups]
        end
    end

    subgraph "External Services"
        AzureAD[Azure AD]
        GitHubActions[GitHub Actions]
    end

    FrontendApp --> AppServicePlan
    BackendApp --> AppServicePlan
    FrontendApp --> FrontendMI
    BackendApp --> BackendMI
    SQLServer --> SQLMI
    FrontendMI --> KeyVault
    BackendMI --> KeyVault
    SQLMI --> KeyVault
    GitHubActions --> FrontendApp
    GitHubActions --> BackendApp
    GitHubActions --> SQLServer
```

### Deployment Pipeline

```mermaid
graph LR
    subgraph "Source Control"
        GitHub[GitHub Repository]
    end

    subgraph "CI/CD"
        Actions[GitHub Actions]
        Build[Build Pipeline]
        Deploy[Deploy Pipeline]
    end

    subgraph "Infrastructure"
        Bicep[Bicep Templates]
        ARM[ARM Templates]
    end

    subgraph "Azure Resources"
        AppServices[App Services]
        SQL[Azure SQL]
        KeyVault[Key Vault]
    end

    GitHub --> Actions
    Actions --> Build
    Build --> Deploy
    Deploy --> Bicep
    Bicep --> ARM
    ARM --> AppServices
    ARM --> SQL
    ARM --> KeyVault
```

## Security Architecture

### Authentication & Authorization

- **Frontend**: NextAuth.js with Azure AD provider
- **Backend**: JWT Bearer token validation
- **Tokens**: Stored in HTTP-only cookies (secure)
- **Scopes**: `api://{client-id}/user_impersonation`

### Secrets Management

- **Key Vault**: Centralized secrets storage
- **Managed Identities**: Service-to-service authentication
- **RBAC**: Role-based access control
- **Connection Strings**: Stored as Key Vault secrets

### Network Security

- **HTTPS Only**: All endpoints use TLS
- **CORS**: Configured for development/production
- **Firewall Rules**: Azure SQL allows Azure IPs
- **Private Endpoints**: Available for production (cost consideration)

## Data Flow

### API Request Flow

1. **Client Request**: React component calls generated API hook
2. **Proxy Layer**: Next.js API route forwards to backend
3. **Authentication**: Token automatically attached to request
4. **Backend Processing**: JWT validation, business logic execution
5. **Data Access**: Entity Framework Core with SQL Server
6. **Response**: Data returned through proxy to frontend

### Database Operations

1. **Migrations**: DbUp manages schema changes
2. **Connection**: Managed identity authenticates to SQL
3. **Transactions**: EF Core handles data consistency
4. **Validation**: FluentValidation ensures data integrity

## Technology Stack

### Frontend

- **Framework**: Next.js 15 with App Router
- **UI Library**: Material-UI + Tailwind CSS
- **State Management**: React Query (TanStack Query)
- **Authentication**: NextAuth.js v4
- **API Client**: Orval-generated React Query hooks

### Backend

- **Framework**: ASP.NET Core 9
- **ORM**: Entity Framework Core
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI
- **Authentication**: JWT Bearer tokens

### Infrastructure

- **IaC**: Bicep templates
- **CI/CD**: GitHub Actions
- **Hosting**: Azure App Service (Linux)
- **Database**: Azure SQL Database (Serverless)
- **Secrets**: Azure Key Vault
- **Identity**: Azure AD + Managed Identities

## Development Workflow

### Local Development

1. **Database**: SQL Server in Docker container
2. **Backend**: `dotnet run` on local machine
3. **Frontend**: `npm run dev` with hot reload
4. **API Client**: Generated from running backend

### Deployment

1. **Infrastructure**: Bicep deployment via GitHub Actions
2. **Backend**: Container deployment to App Service
3. **Frontend**: Node.js deployment to App Service
4. **Database**: Migrations run via DbUp
5. **Secrets**: Automatically configured in Key Vault

## Monitoring & Observability

### Logging

- **Frontend**: Next.js built-in logging
- **Backend**: ASP.NET Core logging with structured logs
- **Infrastructure**: Azure Monitor and Application Insights

### Health Checks

- **API Endpoints**: Swagger documentation
- **Database**: Connection health monitoring
- **Authentication**: Token validation logging

## Scalability Considerations

### Current Setup

- **App Service Plan**: F1 (Free tier) - suitable for development
- **Database**: Serverless tier with auto-pause
- **Static Assets**: Served from App Service

### Production Scaling

- **App Service**: Premium plans for better performance
- **Database**: Provisioned capacity for consistent performance
- **CDN**: Azure CDN for static assets
- **Load Balancing**: Application Gateway for multiple instances
- **Monitoring**: Full Application Insights integration
