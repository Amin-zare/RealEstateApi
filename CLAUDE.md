# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RealEstateApi is an ASP.NET Core Web API (.NET 8) for managing real estate companies and their apartments. It uses Entity Framework Core with Code First approach and SQL Server as the database.

## Essential Commands

### Build and Run
```bash
dotnet restore
dotnet build
dotnet run --launch-profile "https"
```

### Database Migrations
```bash
# Install EF Core CLI globally (if not already installed)
dotnet tool install --global dotnet-ef

# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations to database
dotnet ef database update
```

### Configuration Setup
Configuration uses `appsettings.Development.json` (not in source control). Copy from `appsettings.Template.json`:
- ConnectionStrings:DefaultConnection - SQL Server connection
- ApiToken - Bearer token for API authentication
- WebhookSecret - Secret for webhook endpoint authentication
- Cors:AllowedOrigins - Array of allowed CORS origins

Alternative: Use User Secrets (recommended for development):
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>"
dotnet user-secrets set "ApiToken" "<token>"
dotnet user-secrets set "WebhookSecret" "<secret>"
```

## Architecture

### Minimal API with Static Endpoint Mapping
The application uses ASP.NET Core Minimal APIs. Endpoints are organized in static extension methods:
- [‎Endpoints/CompanyEndpoints.cs](‎Endpoints/CompanyEndpoints.cs) - Company listing with pagination
- [‎Endpoints/ApartmentEndpoints.cs](‎Endpoints/ApartmentEndpoints.cs) - Apartment listing and expiring leases
- [‎Endpoints/WebhookEndpoints.cs](‎Endpoints/WebhookEndpoints.cs) - Webhook for apartment updates

Endpoints are registered in [Program.cs](Program.cs) using `MapCompanyEndpoints()`, `MapApartmentEndpoints()`, `MapWebhookEndpoints()`.

### Authentication Strategy
[Middleware/ApiTokenMiddleware.cs](Middleware/ApiTokenMiddleware.cs) handles dual authentication:
- **Bearer Token** via `Authorization: Bearer <token>` header (for standard API calls)
- **Webhook Secret** via `X-Webhook-Secret: <secret>` header (for webhook endpoint)

Both use constant-time comparison to prevent timing attacks. At least one authentication method must be configured. The middleware accepts either authentication type.

### Data Models
- [Models/Company.cs](Models/Company.cs) - Company entity with one-to-many relationship to Apartments
- [Models/Apartment.cs](Models/Apartment.cs) - Apartment entity with Address (max 200 chars), LeaseEnd (nullable), IsRenovated flag, and CompanyId foreign key

### Database Context
[Data/AppDbContext.cs](Data/AppDbContext.cs) - Primary constructor pattern with DbSet properties for Companies and Apartments.

### CORS Configuration
[‎Configuration/CorsSetup.cs](‎Configuration/CorsSetup.cs) - Extension method that configures CORS from appsettings:
- Validates and normalizes allowed origins from `Cors:AllowedOrigins`
- Supports optional `Cors:AllowCredentials` (defaults to false)
- Explicitly allows headers: Authorization, Accept, Content-Type, X-Webhook-Secret
- Allows methods: GET, POST

### Pagination Pattern
Company and apartment listing endpoints use consistent pagination:
- `skip` and `take` query parameters
- Default take: 50, max take: 200
- Always ordered by Id for consistency

### API Endpoints
- `GET /companies` - List companies (paginated)
- `GET /companies/{companyId}/apartments` - List apartments for a company (paginated)
- `GET /companies/{companyId}/apartments/expiring` - Apartments with LeaseEnd within 3 months
- `POST /webhook/apartment-updated` - Update apartment via webhook (requires X-Webhook-Secret header)

### Swagger Access
Available in Development environment at: `https://localhost:7055/swagger/index.html` (port may vary, check console or launchSettings.json)
