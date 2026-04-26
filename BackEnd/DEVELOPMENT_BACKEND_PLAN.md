# GoodHamburger Backend Development Plan

## Overview

This document outlines the spec-driven development plan for the GoodHamburger backend system, a comprehensive restaurant management platform built with .NET 10.0 following Clean Architecture principles.

## Architecture

### Layer Structure

The project follows a numbered Clean Architecture approach with clear separation of concerns:

```
0-Shared/          # Shared abstractions and base types
1-Domain/          # Domain entities and business logic
2-Application/     # Application services and orchestration
3-Infrastructure/  # External concerns (database, observability)
4-Presentation/    # API controllers and configuration
5-Test/            # Test projects
```

### Technology Stack

- **Framework**: .NET 10.0
- **Language**: C# 13
- **Database**: PostgreSQL 16+
- **ORM**: Entity Framework Core 10.0
- **Authentication**: ASP.NET Core Identity + JWT
- **Validation**: Flunt 2.0.5
- **Observability**: OpenTelemetry
- **Testing**: xUnit, EF Core InMemory
- **API Documentation**: Swagger/OpenAPI

### Design Patterns

- **Clean Architecture**: Layered separation with dependency inversion
- **CQRS**: Command Query Responsibility Segregation
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Domain-Driven Design (DDD)**: Rich domain models
- **Result Pattern**: Type-safe error handling
- **Specification Pattern**: Business rule encapsulation

---

## Spec-Driven Development Methodology

### Principles

1. **Specification First**: Write specifications before implementation
2. **Testable Specifications**: Each spec must be verifiable through tests
3. **Incremental Delivery**: Build features in small, testable increments
4. **Documentation as Code**: Specifications live alongside implementation
5. **Continuous Validation**: Automated testing against specifications

### Development Workflow

```
1. Define Specification → 2. Write Tests → 3. Implement → 4. Validate → 5. Document
```

### Specification Template

Each feature specification includes:

- **Business Requirement**: What problem does it solve?
- **Functional Requirements**: What must it do?
- **Non-Functional Requirements**: Performance, security, etc.
- **Acceptance Criteria**: Definition of done
- **API Contract**: Request/Response schemas
- **Domain Rules**: Business logic constraints
- **Test Cases**: Verification scenarios

---

## Domain Areas

### 1. Catalog Management

**Purpose**: Manage menu items, categories, and ingredients

#### Entities
- `Category`: Menu categorization (Burger, SideDish, Drink, Dessert, Combo)
- `MenuItem`: Product catalog items with pricing
- `Ingredient`: Recipe components
- `MenuItemIngredient`: Many-to-many relationship

#### Business Rules
- Categories must have unique slugs
- Menu items require valid category association
- Ingredients can be marked as removable
- Prices must be greater than zero
- Active items can be temporarily unavailable

#### API Endpoints
- `POST /api/categories` - Create category
- `GET /api/categories` - List categories
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Deactivate category
- `POST /api/menu-items` - Create menu item
- `GET /api/menu-items` - List menu items
- `PUT /api/menu-items/{id}` - Update menu item
- `DELETE /api/menu-items/{id}` - Deactivate menu item
- `POST /api/ingredients` - Create ingredient
- `GET /api/ingredients` - List ingredients

---

### 2. Order Management

**Purpose**: Handle customer orders through lifecycle

#### Entities
- `Order`: Customer order with status tracking
- `OrderItem`: Individual items within orders
- `OrderItemDetail`: Item modifications and extras
- `OrderDiscount`: Applied discounts
- `OrderCoupon`: Coupon usage tracking

#### Business Rules
- Orders start in `Pending` status
- Status workflow: Pending → Confirmed → InPreparation → Ready → InDelivery → Completed
- Orders can be cancelled with reason
- Discounts cannot exceed total items price
- Coupons can only be applied to pending orders
- Delivery fee is added to final amount

#### Order Status Workflow
```
Pending → Confirmed → InPreparation → Ready → InDelivery → Completed
    ↓
Cancelled
```

#### API Endpoints
- `POST /api/orders` - Create order
- `GET /api/orders` - List orders
- `GET /api/orders/{id}` - Get order details
- `PUT /api/orders/{id}/confirm` - Confirm payment
- `PUT /api/orders/{id}/prepare` - Start preparation
- `PUT /api/orders/{id}/ready` - Mark as ready
- `PUT /api/orders/{id}/dispatch` - Dispatch for delivery
- `PUT /api/orders/{id}/complete` - Complete order
- `PUT /api/orders/{id}/cancel` - Cancel order

---

### 3. Coupon Management

**Purpose**: Manage promotional codes and discounts

#### Entities
- `Coupon`: Promotional codes with rules
- `OrderCoupon`: Coupon usage tracking

#### Business Rules
- Coupons can be percentage or fixed amount
- Minimum order value may apply
- Usage limits per coupon
- Expiration dates
- One-time use per customer

#### API Endpoints
- `POST /api/coupons` - Create coupon
- `GET /api/coupons` - List coupons
- `GET /api/coupons/{code}` - Validate coupon
- `PUT /api/coupons/{id}` - Update coupon
- `DELETE /api/coupons/{id}` - Deactivate coupon

---

### 4. Account Management

**Purpose**: Manage customer and employee profiles

#### Entities
- `CustomerProfile`: Customer information and preferences
- `EmployeeProfile`: Staff profiles and roles

#### Business Rules
- Customers require valid contact information
- Employees have role-based permissions
- Profiles include address information
- Document validation (CPF/CNPJ)

#### API Endpoints
- `POST /api/customers` - Create customer profile
- `GET /api/customers` - List customers
- `GET /api/customers/{id}` - Get customer details
- `PUT /api/customers/{id}` - Update customer profile
- `POST /api/employees` - Create employee profile
- `GET /api/employees` - List employees
- `PUT /api/employees/{id}` - Update employee profile

---

### 5. Location Management

**Purpose**: Geographic data for addresses

#### Entities
- `Country`: Country information
- `State`: State/Province information
- `City`: City information
- `Neighborhood`: Neighborhood information
- `StreetType`: Street type classifications

#### Business Rules
- Hierarchical relationship: Country → State → City → Neighborhood
- Street types: Rua, Avenida, Praça, etc.
- Used for address validation

#### API Endpoints
- `GET /api/locations/countries` - List countries
- `GET /api/locations/states/{countryId}` - List states
- `GET /api/locations/cities/{stateId}` - List cities
- `GET /api/locations/neighborhoods/{cityId}` - List neighborhoods
- `GET /api/locations/street-types` - List street types

---

### 6. Identity & Authentication

**Purpose**: User authentication and authorization

#### Entities
- `ApplicationUser`: Identity user
- `ApplicationRole`: Identity roles
- `RefreshToken`: Token refresh mechanism

#### Business Rules
- Password complexity requirements (8+ chars, mixed case, digit, special)
- JWT token expiration (15 minutes access token)
- Refresh token rotation
- Role-based access control
- Account lockout after 5 failed attempts

#### Roles & Policies
- `Admin`: Full system access
- `Manager`: User management and operational tasks
- `Employee`: Operational tasks only
- `Customer`: Order placement and tracking

#### API Endpoints
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - Logout
- `POST /api/roles` - Create role
- `GET /api/roles` - List roles
- `POST /api/users/{userId}/roles` - Assign role to user

---

## Development Phases

### Phase 1: Foundation (Week 1-2)

**Objective**: Establish core infrastructure and base patterns

#### Specifications

##### 1.1 Database Schema
- **Requirement**: Create PostgreSQL database schema
- **Acceptance Criteria**:
  - All tables created with proper relationships
  - Indexes on foreign keys and frequently queried fields
  - Cascade delete behavior configured
  - Seed data for locations and initial roles

##### 1.2 Repository Implementation
- **Requirement**: Implement repository pattern for all entities
- **Acceptance Criteria**:
  - All repository interfaces implemented
  - Unit of Work pattern functional
  - Transaction management working
  - Async operations throughout

##### 1.3 Base API Controllers
- **Requirement**: Create base controller with common functionality
- **Acceptance Criteria**:
  - Result pattern integration
  - Error handling middleware
  - Validation response formatting
  - Authentication/Authorization integration

#### Deliverables
- [ ] Database migrations applied
- [ ] All repositories implemented and tested
- [ ] Base controller with common patterns
- [ ] Global exception handler
- [ ] JWT authentication working
- [ ] Swagger documentation configured

---

### Phase 2: Catalog Management (Week 3-4)

**Objective**: Implement menu catalog functionality

#### Specifications

##### 2.1 Category Management
- **Business Requirement**: Admins must be able to create and manage menu categories
- **Functional Requirements**:
  - Create categories with name, description, slug, type
  - Update category information
  - Activate/deactivate categories
  - List categories with filtering
- **Non-Functional Requirements**:
  - Response time < 200ms for list operations
  - Unique slug enforcement
- **Acceptance Criteria**:
  - Categories can be created via API
  - Slugs are auto-generated and unique
  - Categories can be filtered by type
  - Deactivated categories don't appear in lists

##### 2.2 Menu Item Management
- **Business Requirement**: Admins must be able to manage menu items
- **Functional Requirements**:
  - Create menu items with pricing, calories, images
  - Associate with categories
  - Add/remove ingredients
  - Mark as available/unavailable
- **Non-Functional Requirements**:
  - Image URL validation
  - Price validation (> 0)
- **Acceptance Criteria**:
  - Menu items can be created with ingredients
  - Ingredients can be marked as removable
  - Availability status affects order creation
  - Price updates are tracked

##### 2.3 Ingredient Management
- **Business Requirement**: Admins must be able to manage recipe ingredients
- **Functional Requirements**:
  - Create ingredients with descriptions
  - List ingredients for menu item association
- **Acceptance Criteria**:
  - Ingredients can be created and listed
  - Ingredients can be associated with multiple items

#### Deliverables
- [ ] Category CRUD endpoints
- [ ] MenuItem CRUD endpoints
- [ ] Ingredient CRUD endpoints
- [ ] MenuItem-Ingredient association
- [ ] Unit tests for all operations
- [ ] Integration tests for API endpoints

---

### Phase 3: Order Management (Week 5-7)

**Objective**: Implement core order processing

#### Specifications

##### 3.1 Order Creation
- **Business Requirement**: Customers must be able to place orders
- **Functional Requirements**:
  - Create order with customer, address, items
  - Calculate totals including delivery fee
  - Apply discounts and coupons
  - Validate inventory availability
- **Business Rules**:
  - Orders start in Pending status
  - Delivery fee added to total
  - Items cannot be negative quantity
- **Non-Functional Requirements**:
  - Order creation < 500ms
  - Transaction consistency
- **Acceptance Criteria**:
  - Orders can be created with multiple items
  - Totals calculated correctly
  - Delivery address validated
  - Order number generated

##### 3.2 Order Status Workflow
- **Business Requirement**: Orders must progress through preparation lifecycle
- **Functional Requirements**:
  - Confirm payment
  - Start preparation
  - Mark as ready
  - Dispatch for delivery
  - Complete order
  - Cancel with reason
- **Business Rules**:
  - Status transitions must follow workflow
  - Cancellation requires reason
  - Cannot modify completed orders
- **Acceptance Criteria**:
  - Status transitions enforced
  - Audit trail of status changes
  - Cancellation prevents further modifications

##### 3.3 Order Discounts
- **Business Requirement**: System must support various discount types
- **Functional Requirements**:
  - Apply category combo discounts
  - Apply coupon codes
  - Calculate final totals
- **Business Rules**:
  - Discounts cannot exceed item total
  - Coupons validated before application
  - Only one coupon per order
- **Acceptance Criteria**:
  - Discounts reduce total correctly
  - Invalid coupons rejected
  - Discount rules enforced

#### Deliverables
- [ ] Order creation endpoint
- [ ] Order status workflow endpoints
- [ ] Discount calculation logic
- [ ] Coupon validation
- [ ] Order listing with filters
- [ ] Unit tests for order logic
- [ ] Integration tests for order workflow

---

### Phase 4: Coupon Management (Week 8)

**Objective**: Implement promotional code system

#### Specifications

##### 4.1 Coupon Creation
- **Business Requirement**: Admins must be able to create promotional codes
- **Functional Requirements**:
  - Create coupons with codes, values, expiration
  - Set minimum order requirements
  - Set usage limits
- **Business Rules**:
  - Codes must be unique
  - Cannot modify active coupons
  - Expired coupons cannot be used
- **Acceptance Criteria**:
  - Coupons can be created with all parameters
  - Coupon validation works correctly
  - Usage tracking functional

##### 4.2 Coupon Validation
- **Business Requirement**: System must validate coupons before application
- **Functional Requirements**:
  - Check coupon existence
  - Validate expiration
  - Check usage limits
  - Verify minimum order value
- **Acceptance Criteria**:
  - Invalid coupons rejected with clear messages
  - Expired coupons not accepted
  - Usage limits enforced

#### Deliverables
- [ ] Coupon CRUD endpoints
- [ ] Coupon validation logic
- [ ] Usage tracking
- [ ] Unit tests for coupon logic
- [ ] Integration tests for coupon workflow

---

### Phase 5: Account Management (Week 9-10)

**Objective**: Implement customer and employee profiles

#### Specifications

##### 5.1 Customer Profiles
- **Business Requirement**: System must store customer information
- **Functional Requirements**:
  - Create customer profiles with contact info
  - Update customer information
  - Link to identity user
  - Store delivery addresses
- **Business Rules**:
  - Email must be unique
  - Phone validation
  - Document validation (CPF)
- **Non-Functional Requirements**:
  - PII data protection
  - Address validation
- **Acceptance Criteria**:
  - Customers can register profiles
  - Contact information validated
  - Addresses stored correctly

##### 5.2 Employee Profiles
- **Business Requirement**: System must manage employee information
- **Functional Requirements**:
  - Create employee profiles
  - Assign roles
  - Track employment status
- **Business Rules**:
  - Employees require valid roles
  - Role-based access enforced
- **Acceptance Criteria**:
  - Employees can be created with roles
  - Access control works correctly

#### Deliverables
- [ ] Customer profile endpoints
- [ ] Employee profile endpoints
- [ ] Profile validation logic
- [ ] Address validation
- [ ] Unit tests for profile logic
- [ ] Integration tests for profile endpoints

---

### Phase 6: Location Management (Week 11)

**Objective**: Implement geographic data management

#### Specifications

##### 6.1 Location Data
- **Business Requirement**: System must provide location data for addresses
- **Functional Requirements**:
  - Load country/state/city/neighborhood data
  - Provide street type options
  - Hierarchical queries
- **Non-Functional Requirements**:
  - Cached for performance
  - Query response < 100ms
- **Acceptance Criteria**:
  - Location data accessible via API
  - Hierarchical queries work
  - Street types available

#### Deliverables
- [ ] Location data endpoints
- [ ] Data seeding scripts
- [ ] Caching implementation
- [ ] Unit tests for location queries

---

### Phase 7: Identity & Authentication (Week 12)

**Objective**: Implement user authentication and authorization

#### Specifications

##### 7.1 User Registration
- **Business Requirement**: Users must be able to register accounts
- **Functional Requirements**:
  - Register with email and password
  - Password complexity validation
  - Email uniqueness check
- **Business Rules**:
  - Password: 8+ chars, mixed case, digit, special
  - Email must be unique
  - Account requires email confirmation (optional)
- **Acceptance Criteria**:
  - Users can register with valid credentials
  - Invalid passwords rejected
  - Duplicate emails prevented

##### 7.2 Authentication
- **Business Requirement**: Users must authenticate to access protected resources
- **Functional Requirements**:
  - Login with email/password
  - JWT token generation
  - Token refresh mechanism
  - Logout functionality
- **Non-Functional Requirements**:
  - Token expiration: 15 minutes
  - Refresh token rotation
  - Secure token storage
- **Acceptance Criteria**:
  - Valid credentials return JWT
  - Invalid credentials return 401
  - Expired tokens rejected
  - Refresh tokens work correctly

##### 7.3 Authorization
- **Business Requirement**: System must enforce role-based access control
- **Functional Requirements**:
  - Role assignment
  - Policy enforcement
  - Permission checks
- **Business Rules**:
  - Admin: Full access
  - Manager: Management tasks
  - Employee: Operational tasks
  - Customer: Order placement
- **Acceptance Criteria**:
  - Unauthorized access returns 403
  - Role-based restrictions enforced
  - Policies work correctly

#### Deliverables
- [ ] Registration endpoint
- [ ] Login endpoint
- [ ] Token refresh endpoint
- [ ] Logout endpoint
- [ ] Role management endpoints
- [ ] JWT configuration
- [ ] Authorization policies
- [ ] Unit tests for auth logic
- [ ] Integration tests for auth endpoints

---

### Phase 8: Testing & Quality (Week 13-14)

**Objective**: Ensure system quality and reliability

#### Specifications

##### 8.1 Unit Testing
- **Requirement**: All business logic must have unit tests
- **Acceptance Criteria**:
  - > 80% code coverage
  - All domain entities tested
  - All repository methods tested
  - All application services tested

##### 8.2 Integration Testing
- **Requirement**: All API endpoints must have integration tests
- **Acceptance Criteria**:
  - All endpoints tested
  - Authentication flows tested
  - Error scenarios tested
  - Database transactions tested

##### 8.3 Performance Testing
- **Requirement**: System must meet performance targets
- **Acceptance Criteria**:
  - API response < 200ms (p95)
  - Database queries optimized
  - N+1 query problems eliminated

##### 8.4 Security Testing
- **Requirement**: System must be secure against common vulnerabilities
- **Acceptance Criteria**:
  - SQL injection prevention
  - XSS prevention
  - CSRF protection
  - Input validation
  - Secure headers

#### Deliverables
- [ ] Unit test suite
- [ ] Integration test suite
- [ ] Performance test results
- [ ] Security audit report
- [ ] Code coverage report

---

### Phase 9: Deployment & Documentation (Week 15)

**Objective**: Prepare for production deployment

#### Specifications

##### 9.1 Deployment Configuration
- **Requirement**: System must be deployable to production
- **Acceptance Criteria**:
  - Docker containerization
  - Environment-specific configurations
  - Database migration scripts
  - Health check endpoints

##### 9.2 Documentation
- **Requirement**: System must have comprehensive documentation
- **Acceptance Criteria**:
  - API documentation (Swagger)
  - Architecture documentation
  - Deployment guide
  - Troubleshooting guide

#### Deliverables
- [ ] Docker configuration
- [ ] Deployment scripts
- [ ] Environment configurations
- [ ] API documentation
- [ ] Architecture documentation
- [ ] Deployment guide

---

## Testing Strategy

### Unit Testing
- **Framework**: xUnit
- **Scope**: Domain logic, repository methods, application services
- **Coverage Target**: > 80%
- **Tools**: Moq for mocking, FluentAssertions for assertions

### Integration Testing
- **Framework**: xUnit
- **Database**: EF Core InMemory
- **Scope**: API endpoints, database operations, authentication flows
- **Tools**: WebApplicationFactory for API testing

### Performance Testing
- **Tools**: BenchmarkDotNet, k6
- **Metrics**: Response time, throughput, resource usage
- **Targets**: p95 < 200ms for API endpoints

### Security Testing
- **Tools**: OWASP ZAP, SonarQube
- **Focus**: SQL injection, XSS, CSRF, authentication bypass

---

## Quality Standards

### Code Quality
- Follow C# coding conventions
- Use meaningful variable and method names
- Keep methods small and focused
- Avoid code duplication
- Use async/await correctly

### Architecture Standards
- Respect layer boundaries
- Dependency inversion principle
- Single responsibility principle
- Open/closed principle
- Interface segregation principle

### Documentation Standards
- XML documentation for public APIs
- Inline comments for complex logic
- README for each project
- Architecture decision records (ADRs)

---

## Monitoring & Observability

### Metrics
- Request rate and error rate
- Response time percentiles
- Database query performance
- Memory and CPU usage

### Logging
- Structured logging with correlation IDs
- Log levels: Debug, Information, Warning, Error
- Sensitive data redaction
- Log aggregation

### Tracing
- Distributed tracing for request flows
- Database query tracing
- External service calls

### Health Checks
- Database connectivity
- External service availability
- Disk space and memory
- Custom health indicators

---

## Deployment Strategy

### Environments
- **Development**: Local development with hot reload
- **Staging**: Pre-production testing environment
- **Production**: Live environment

### Deployment Process
1. Run all tests
2. Build Docker image
3. Push to registry
4. Deploy to staging
5. Run smoke tests
6. Deploy to production
7. Verify health checks

### Rollback Strategy
- Maintain previous version
- Database migration rollback scripts
- Feature flags for gradual rollout

---

## Risk Management

### Technical Risks
- **Database migration failures**: Test migrations on staging first
- **Performance issues**: Load testing before deployment
- **Security vulnerabilities**: Regular security audits

### Process Risks
- **Scope creep**: Strict adherence to specifications
- **Timeline delays**: Buffer time in estimates
- **Quality issues**: Code reviews and automated testing

---

## Success Criteria

### Functional
- [ ] All specified features implemented
- [ ] All acceptance criteria met
- [ ] All tests passing
- [ ] Zero critical bugs

### Non-Functional
- [ ] API response time < 200ms (p95)
- [ ] Code coverage > 80%
- [ ] Zero security vulnerabilities
- [ ] 99.9% uptime

### Documentation
- [ ] Complete API documentation
- [ ] Architecture documentation
- [ ] Deployment guide
- [ ] Troubleshooting guide

---

## Appendix

### A. Database Schema

#### Key Tables
- `cat_categories`: Menu categories
- `cat_menu_items`: Product catalog
- `cat_ingredients`: Recipe components
- `cat_menu_item_ingredients`: Item-ingredient relationships
- `ord_orders`: Customer orders
- `ord_order_items`: Order items
- `ord_order_discounts`: Applied discounts
- `ord_order_coupons`: Coupon usage
- `sales_coupons`: Promotional codes
- `customer_profiles`: Customer information
- `employee_profiles`: Employee information
- `AspNetUsers`: Identity users
- `AspNetRoles`: Identity roles

### B. API Documentation

All API endpoints are documented via Swagger at `/swagger` in development.

### C. Configuration

#### Required Environment Variables
```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=GoodHamburgerDb;Username=admin;Password=password
JWT__Key=<your-secret-key>
JWT__Issuer=https://your-api.example.com
JWT__Audience=https://your-api.example.com
```

### D. Development Commands

#### Database Migrations
```bash
# Create migration
dotnet ef migrations add MigrationName --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api -o "Migrations/AppApplicationDbContext"

# Apply migration
dotnet ef database update --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

#### Run Tests
```bash
# Run all tests
dotnet test ./BackEnd/5-Test/GoodHamburger.Test

# Run with coverage
dotnet test ./BackEnd/5-Test/GoodHamburger.Test --collect:"XPlat Code Coverage"
```

#### Build and Run
```bash
# Build solution
dotnet build ./BackEnd

# Run API
dotnet run --project ./BackEnd/4-Presentation/GoodHamburger.Api
```

---

## Version History

- **v1.0** - Initial development plan (April 2026)

## References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://domainlanguage.com/ddd/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
