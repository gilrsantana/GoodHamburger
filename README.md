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

**Steps:**

1.  **Clone the repository:**
    ```bash
    git clone [repository_url]
    cd GoodHamburger
    ```

2.  **Start the Docker services:**
    Navigate to the root of the project where `docker-compose.yml` is located and run:
    ```bash
    docker-compose up --build -d
    ```
    This will build the API image, and start PostgreSQL, PGAdmin, Grafana, and the API.

3.  **Apply Database Migrations:**
    Once the PostgreSQL container is running, apply the database migrations. You can do this by executing the following commands from the `BackEnd/4-Presentation/GoodHamburger.Api` directory:
    ```bash
    dotnet ef database update --project ../../3-Infrastructure/GoodHamburger.Database/GoodHamburger.Database.csproj --startup-project .
    dotnet ef database update --project ../../3-Infrastructure/GoodHamburger.Database/GoodHamburger.Database.csproj --startup-project . --context IdentityDbContext
    ```
    *(Note: You might need to run `dotnet tool install --global dotnet-ef` if `dotnet ef` command is not found.)*

4.  **Access the API Documentation:**
    The API will be available at `http://localhost:5000` (or the port configured in `launchSettings.json` and `docker-compose.yml`).
    The Swagger UI documentation can be accessed at `http://localhost:5000/swagger`.

5.  **Access PGAdmin:**
    PGAdmin will be available at `http://localhost:5050`. You can log in with the credentials defined in `docker-compose.yml`.

6.  **Access Grafana (OpenTelemetry):**
    Grafana will be available at `http://localhost:3000`. You can log in with the credentials defined in `docker-compose.yml`.

7.  **Stop the Docker services:**
    To stop and remove the containers, networks, and volumes created by `docker-compose`, run:
    ```bash
    docker-compose down
    ```
