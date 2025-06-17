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

#### Migration Naming Convention
Database migration scripts follow a strict timestamp-based naming convention:

**Format**: `YYYY_MM_DD_HHMM_DescriptiveAction.sql`

**Examples**:
- `2025_06_17_1117_InitialSchema.sql` - Initial database schema creation
- `2025_06_18_0930_INSERT_INTO_ExerciseCategories_TestData.sql` - Seed data insertion
- `2025_06_20_1445_ALTER_TABLE_Users_ADD_ProfileImage.sql` - Schema changes
- `2025_06_22_1630_CREATE_INDEX_WorkoutSessions_Performance.sql` - Performance improvements

**Guidelines**:
- Use 24-hour format for time (HHMM)
- Use descriptive action names (CREATE, ALTER, INSERT, DROP, etc.)
- Include table names when relevant
- Keep descriptions concise but clear
- Scripts run in chronological order automatically

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
│   │   ├── src/
│   │   │   ├── api/           # Generated API client
│   │   │   └── components/    # React components
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

## 🛠️ Local Development: Running the Database

For local development, only the SQL Server database runs in Docker. The backend and frontend are run directly on your machine for easier debugging and hot reload.

### Utility Scripts

You can use the provided scripts to manage the SQL Server container:

- **Start/build the SQL Server container:**
  ```sh
  ./start-db.sh
  ```
  _This will start SQL Server with the `Finnish_Swedish_CI_AS` collation for correct Finnish/Swedish sorting and comparison._
- **Stop and remove the SQL Server container:**
  ```sh
  ./stop-db.sh
  ```

The scripts will build the image if needed, start the container if not running, and clean up as appropriate.

### Collation Note
- The SQL Server container is started with `Finnish_Swedish_CI_AS` collation by default. This ensures correct alphabetical order for Å, Ä, Ö and is recommended for Finnish/Swedish users.
- If you need a different collation, edit the `MSSQL_COLLATION` environment variable in `start-db.sh`.

### Manual Commands (if you prefer)

#### Build the SQL Server Docker image
```sh
docker build -f Dockerfile.sqlserver -t hupiukko-sqlserver .
```

#### Run the SQL Server container (with Finnish/Swedish collation)
```sh
docker run -d \
  --name hupiukko-sqlserver \
  -e ACCEPT_EULA=Y \
  -e SA_PASSWORD=YourStrong!Passw0rd \
  -e MSSQL_PID=Developer \
  -e MSSQL_COLLATION=Finnish_Swedish_CI_AS \
  -p 1433:1433 \
  -v sqlserver_data:/var/opt/mssql \
  hupiukko-sqlserver
```

#### Stop and remove the SQL Server container
```sh
docker stop hupiukko-sqlserver && docker rm hupiukko-sqlserver
```

#### View SQL Server logs
```sh
docker logs -f hupiukko-sqlserver
```

#### Check SQL Server container status
```sh
docker ps -a | grep hupiukko-sqlserver
```

- The backend API and frontend should be started using their respective `dotnet run` and `npm run dev`/`yarn dev` commands.
- Database connection string in your app should point to `localhost,1433` as configured.
- The `-v sqlserver_data:/var/opt/mssql` flag ensures your database files persist between container runs.
- To remove the persistent data volume: `docker volume rm sqlserver_data` (only do this if you want to delete all data). 