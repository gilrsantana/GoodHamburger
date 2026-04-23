# GoodHamburger.Database

This project contains the Entity Framework Core database context, mappings, and migrations for the GoodHamburger application.

## Project Structure

- **Context/**: Database context implementations
- **Mappings/**: Entity type configurations for EF Core
- **Migrations/**: Database migration files
- **Extensions/**: Database-related extensions and configurations

## Database Contexts

### ApplicationDbContext
Main application database context containing:
- **Accounts**: Customer profiles, employee profiles
- **Catalog**: Categories, menu items, ingredients
- **Ordering**: Orders, order items, discounts, coupons
- **Sales**: Coupons and promotional entities

### IdentityDbContext
ASP.NET Core Identity database context containing:
- **Users**: Application users and authentication
- **Roles**: Role-based access control
- **Claims**: User claims and permissions

## Database Configuration

### Connection String
Configure your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=goodhamburger;Username=postgres;Password=your_password"
  }
}
```

### PostgreSQL Setup
1. Install PostgreSQL on your system
2. Create a database:
   ```sql
   CREATE DATABASE goodhamburger;
   ```
3. Update connection string with your credentials

## Migration Commands

### Initial Setup
Run these commands to create and apply initial migrations:

#### ApplicationDbContext (Main Database)
```bash
# Create initial migration
dotnet ef migrations add InitialDomain --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api -o "Migrations/AppApplicationDbContext"

# Apply migration to database
dotnet ef database update --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

#### IdentityDbContext (Authentication)
```bash
# Create identity migration
dotnet ef migrations add Identity --context IdentityDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api -o "Migrations/AppIdentityDbContext"

# Apply identity migration
dotnet ef database update --context IdentityDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

### Common Migration Operations

#### Create New Migration
```bash
dotnet ef migrations add MigrationName --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api -o "Migrations/AppApplicationDbContext"
```

#### Update Database
```bash
dotnet ef database update --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

#### Remove Last Migration
```bash
dotnet ef migrations remove --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

#### Reset Database
⚠️ **Warning**: This will drop all data in the database
```bash
dotnet ef database drop --context ApplicationDbContext --project ./BackEnd/3-Infrastructure/GoodHamburger.Database --startup-project ./BackEnd/4-Presentation/GoodHamburger.Api
```

## Entity Mappings

### Catalog Mappings
- **CategoryMap**: Menu categories (Burger, SideDish, Drink, etc.)
- **MenuItemMap**: Product catalog items
- **IngredientMap**: Recipe ingredients
- **MenuItemIngredientMap**: Many-to-many relationship between items and ingredients

### Ordering Mappings
- **OrderMap**: Customer orders and workflow states
- **OrderItemMap**: Individual items within orders
- **OrderDiscountMap**: Applied discounts (category combos, coupons)
- **OrderCouponMap**: Coupon usage tracking
- **OrderItemDetailMap**: Item modifications and extras

### Account Mappings
- **CustomerProfileMap**: Customer information and preferences
- **EmployeeProfileMap**: Staff profiles and roles

### Location Mappings
- **CountryMap**, **StateMap**, **CityMap**, **NeighborhoodMap**: Geographic data
- **StreetTypeMap**: Street type classifications

## Database Schema

### Key Tables
- **ord_orders**: Customer orders with status tracking
- **ord_order_items**: Individual order items with pricing
- **ord_order_discounts**: Applied discounts and promotions
- **ord_order_coupons**: Coupon usage tracking
- **cat_categories**: Menu categories and types
- **cat_menu_items**: Product catalog with pricing
- **customer_profiles**: Customer information and addresses
- **sales_coupons**: Promotional codes and rules

### Relationships
- Orders → OrderItems (1:N)
- Orders → OrderDiscounts (1:N)
- Orders → OrderCoupons (1:N)
- Categories → MenuItems (1:N)
- MenuItems → MenuItemIngredients (1:N)

## Development Notes

### Entity Framework Core Best Practices
- All entities inherit from `Entity` base class with GUID primary keys
- Use `IEntityTypeConfiguration<T>` for complex mappings
- Owned types for value objects (Address, Document, etc.)
- Proper cascade delete behavior configured
- Indexes for performance optimization

### Migration Guidelines
1. Always review generated migrations before applying
2. Use descriptive migration names
3. Test migrations on development database first
4. Keep migration history clean and sequential

### Troubleshooting
- **Shadow Property Warnings**: Usually harmless, indicate EF Core is detecting relationship changes
- **Migration Conflicts**: Clear migrations directory and recreate if needed
- **Connection Issues**: Verify PostgreSQL service is running and credentials are correct

## Dependencies
- **Microsoft.EntityFrameworkCore**: Core EF framework
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider
- **Microsoft.EntityFrameworkCore.Tools**: Migration tooling
