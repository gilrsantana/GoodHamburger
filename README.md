# Good Hamburger - Technical Challenge

This project implements a backend system for "Good Hamburger" to manage orders, menu items, and discounts, as per the technical challenge requirements.

## Table of Contents
1.  [Project Overview](#project-overview)
2.  [Architectural Decisions](#architectural-decisions)
3.  [Instructions to Run the Project](#instructions-to-run-the-project)

## Project Overview
The "Good Hamburger" system is designed to register customer orders, manage a dynamic menu (sandwiches, side dishes, drinks), and apply various discount rules. It provides a RESTful API for full CRUD operations on orders and menu items, along with robust discount calculation and validation mechanisms.

## Architectural Decisions

### 1. Layered Architecture
The project is segmented into distinct layers:
*   **Domain**: Contains the core business logic, entities, value objects, and interfaces for repositories. It is independent of any specific technology.
*   **Application**: Implements the application-specific business rules, orchestrates domain objects, and handles use cases (commands and queries). It depends on the Domain layer.
*   **Infrastructure**: Provides concrete implementations for interfaces defined in the Domain layer, such as database access (repositories), external services, and other technical concerns.
*   **Presentation (API)**: Exposes the application's functionality through a RESTful API, handling HTTP requests and responses. It depends on the Application layer.

This separation of concerns promotes maintainability, testability, and scalability.

### 2. Domain-Driven Design (DDD) Principles
*   **Bounded Contexts**: The domain is built around limited contexts, ensuring that entities and business rules are cohesive within their specific areas (e.g., `Ordering` and `Catalog`).
*   **Entity Validations**: Entities incorporate their own validation logic using libraries like `Flunt`, ensuring that domain objects are always in a valid state.

### 3. Database Management
*   **Code First with Migrations**: Entity Framework Core's Code First approach is used to define the database schema based on C# models. Migrations are employed for version control of the database schema.
*   **Separate DbContexts**: Distinct `DbContext` instances are used for the core domain data and for identity/authorization data (`IdentityDbContext`). This provides a clear separation of concerns and can optimize database operations.
*   **PostgreSQL**: Chosen as the primary database for its flexibility in handling both relational data and non-relational structures, which is beneficial for logs and event sourcing.

### 4. Authentication and Authorization
*   **ASP.NET Core Identity**: The `Microsoft.AspNetCore.Identity` package is utilized for managing user authentication, roles, and claims, providing a robust and secure identity management system.
*   **Role-Based Authorization**: Policies are defined to control access based on user roles (e.g., "AdminOnly", "Management", "Operate", "Authenticated").

### 5. Observability
*   **OpenTelemetry Integration**: An observability project is included to provide application health insights through logs, metrics, and traces. This helps in monitoring, debugging, and understanding application behavior in production environments.

### 6. Containerization
*   **Docker Compose**: A `docker-compose.yml` file is provided to orchestrate the entire project stack, including:
    *   PostgreSQL database
    *   PGAdmin (for database management)
    *   Grafana (for visualizing metrics and traces from OpenTelemetry)
    *   Blazor WebAssembly Frontend (served by Nginx)
    *   The Good Hamburger API itself.
    This simplifies setup and ensures a consistent development and deployment environment.

### 7. Result Pattern
*   **Result Pattern for Operations**: A `Result` pattern is used for command and query handlers to explicitly represent the success or failure of an operation, along with associated values or errors. This improves error handling and makes the API's outcomes more predictable.

### 8. Global Exception Handling
*   **Global Exception Handler**: A centralized exception handling mechanism (`GlobalExceptionHandler`) is implemented to gracefully manage unhandled exceptions across the application, providing consistent error responses and preventing sensitive information leakage.

## Instructions to Run the Project

**Prerequisites:**
*   .NET SDK (version specified in `global.json` or project files)
*   Docker and Docker Compose

### Docker Setup (Recommended)

The project includes a complete Docker setup with all services orchestrated via `docker-compose.yml`.

#### Services

**Frontend (Blazor WebAssembly)**
- **Container**: `goodhamburger_frontend`
- **Port**: `3001:80`
- **Technology**: .NET 10.0 Blazor WebAssembly served by Nginx
- **Access**: http://localhost:3001

**Backend API**
- **Container**: `goodhamburger_api`
- **Port**: `8500:8080`, `8501:8081`
- **Technology**: .NET 10.0 Web API
- **Access**: http://localhost:8500 (Swagger UI)

**Database**
- **Container**: `goodhamburger_db`
- **Port**: `5432:5432`
- **Technology**: PostgreSQL 15 Alpine
- **Credentials**: admin/password123

**PgAdmin**
- **Container**: `goodhamburger_pgadmin`
- **Port**: `8432:80`
- **Technology**: PgAdmin 4
- **Access**: http://localhost:8432
- **Credentials**: admin@admin.com/admin

**Observability (LGTM)**
- **Container**: `goodhamburger_lgtm`
- **Ports**: 
  - `3000:3000` (Grafana UI)
  - `4317:4317` (OTLP gRPC)
  - `4318:4318` (OTLP HTTP)
  - `9411:9411` (Zipkin)
- **Access**: http://localhost:3000

#### Quick Start

1. **Start all services:**
   ```bash
   docker-compose up -d
   ```

2. **Stop all services:**
   ```bash
   docker-compose down
   ```

3. **View logs:**
   ```bash
   docker-compose logs -f [service_name]
   ```

4. **Rebuild specific service:**
   ```bash
   docker-compose up -d --build [service_name]
   ```

#### Network Configuration

All services are connected to a custom Docker network `goodhamburger-network` for secure inter-service communication. The frontend communicates with the API using the configured base URL.

#### Environment Variables

**Frontend**
- `ApiBaseUrl=http://localhost:8500` - API endpoint URL

**API**
- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=GoodHamburgerDb;Username=admin;Password=password123`
- `SRE__OtlpEndpointUrl=http://lgtm:4317`
- `SRE__Nome=GoodHamburger.Api.Docker`
- `SRE__Habilitado=true`
- `SRE__UtilizaConsoleLog=true`

#### CORS Configuration

The API is configured to allow requests from:
- `http://localhost:5100` (Development)
- `http://localhost:3001` (Docker Frontend)

#### Troubleshooting

**Frontend API Connection Issues**

If you encounter `ERR_NAME_NOT_RESOLVED` errors:

1. **Check API URL in appsettings.json** - Ensure it points to `http://localhost:8500` for Docker
2. **Verify CORS policy** - Make sure frontend URL is included in API CORS configuration
3. **Check container networking** - All containers should be on the same Docker network

**Common Issues**
- **CORS errors**: Update CORS policy in `AppConfiguration.cs` to include frontend URL
- **Network resolution**: Use `localhost:port` instead of container hostnames for browser-based requests
- **Port conflicts**: Ensure no other services are using the same ports

### Manual Development Setup

If you prefer to run the project manually without Docker:

1.  **Clone the repository:**
    ```bash
    git clone [repository_url]
    cd GoodHamburger
    ```

2.  **Apply Database Migrations:**
    Execute the following commands from the `BackEnd/4-Presentation/GoodHamburger.Api` directory:
    ```bash
    dotnet ef database update --project ../../3-Infrastructure/GoodHamburger.Database/GoodHamburger.Database.csproj --startup-project .
    dotnet ef database update --project ../../3-Infrastructure/GoodHamburger.Database/GoodHamburger.Database.csproj --startup-project . --context IdentityDbContext
    ```
    *(Note: You might need to run `dotnet tool install --global dotnet-ef` if `dotnet ef` command is not found.)*

3.  **Run the API:**
    ```bash
    cd BackEnd/4-Presentation/GoodHamburger.Api
    dotnet run
    ```

4.  **Run the Frontend:**
    ```bash
    cd FrontEnd/GoodHamburger.FrontEnd
    dotnet run
    ```

5.  **Access the Applications:**
    - **Frontend**: http://localhost:5100 (or as configured in launchSettings.json)
    - **API Swagger**: http://localhost:5275/swagger (or as configured in launchSettings.json)
