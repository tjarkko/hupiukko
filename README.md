# Full-Stack Web Application

A modern full-stack web application built with Next.js, .NET 8, and Azure services.

## 🏗️ Architecture

### Frontend (Next.js)
- **Framework**: Next.js with App Router
- **Authentication**: 
  - next-auth with Azure AD provider
  - Server-side token management
  - Secure HTTP-only session cookies
- **Deployment**: Azure App Service (Linux)
  - Full Node.js support
  - Server-Side Rendering (SSR)
  - API Routes support
- **API Client**:
  - Generated from OpenAPI spec
  - Uses orval (React Query integration)
  - Type-safe API calls
  - Automated generation via GitHub Actions

### Backend (.NET)
- **Framework**: ASP.NET Core Web API
- **Authentication**:
  - Azure AD integration
  - Access token validation
  - Protected API endpoints
- **API Documentation**:
  - OpenAPI/Swagger specification
  - Available at `/swagger/v1/swagger.json`
  - Generated using Swashbuckle
- **Deployment**: Azure Container Apps

### Frontend-Backend Communication
- **API Proxy Pattern**:
  - Client components use generated API hooks
  - All requests go through Next.js API routes (`/api/proxy/*`)
  - Automatic token forwarding
  - Type-safe end-to-end
- **Token Flow**:
  1. User authenticates via next-auth
  2. Access token stored in HTTP-only cookie
  3. API calls proxied through Next.js
  4. Token automatically attached to backend requests

### Database
- Azure SQL Database (Serverless)
- Managed through DbUp migrations
- Raw SQL scripts for version control

### Infrastructure
- Azure services for all components
- Infrastructure as Code with Bicep
- Automated deployments via GitHub Actions

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- Azure CLI
- Git
- VS Code or Cursor IDE

### Development Setup

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd [project-name]
   ```

2. Frontend Setup:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

3. Backend Setup:
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```

4. Database Migrations:
   ```bash
   cd db-migrations
   dotnet run
   ```

### Environment Variables

#### Frontend (.env.local):
```
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=your-secret
AZURE_AD_CLIENT_ID=your-client-id
AZURE_AD_CLIENT_SECRET=your-client-secret
AZURE_AD_TENANT_ID=your-tenant-id
```

#### Backend (appsettings.Development.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "Audience": "api://your-api-client-id"
  }
}
```

## 📁 Project Structure

```
/
├── frontend/                # Next.js application
│   ├── pages/
│   │   └── api/
│   │       └── proxy/      # API proxy routes
│   ├── src/
│   │   ├── api/           # Generated API client
│   │   └── components/    # React components
├── backend/                # ASP.NET Core API (Hupiukko.Api)
│   ├── Controllers/       # API endpoints
│   ├── Dtos/             # Data Transfer Objects
│   ├── BusinessLogic/     # Business Logic Layer
│   │   ├── Profiles/      # AutoMapper profiles
│   │   ├── Models/        # EF Core entities
│   │   ├── Managers/      # Business logic managers
│   │   └── Utility/       # Helper classes
│   └── Tests/            # Test projects
│       ├── Unit/         # Unit tests
│       └── Integration/  # Integration tests
├── db-migrations/         # Database migration scripts
├── infrastructure/        # Bicep templates
└── .github/workflows/     # CI/CD pipelines
```

## 🔄 CI/CD Pipeline

The project uses GitHub Actions for continuous integration and deployment:

- **Frontend**:
  - Build and deploy to Azure App Service
  - Generate API client from OpenAPI spec
  - Run type checks and tests

- **Backend**:
  - Build and deploy to Azure Container Apps
  - Generate OpenAPI specification
  - Run unit tests

- **Database**:
  - Run migrations using DbUp
  - Version control for schema changes

- **Infrastructure**:
  - Deploy Azure resources using Bicep
  - Configure networking and security

## 🔒 Security

- **Authentication**:
  - Azure AD / Entra ID integration
  - Secure token handling
  - HTTP-only cookies
  - CSRF protection

- **API Security**:
  - Token validation
  - Rate limiting
  - CORS configuration
  - API key management

- **Infrastructure**:
  - Azure Key Vault for secrets
  - Network security groups
  - Private endpoints
  - SSL/TLS encryption

## 📝 License

[Your chosen license]

## 🤝 Contributing

[Contribution guidelines] 