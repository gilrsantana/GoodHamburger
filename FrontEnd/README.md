# Good Hamburger - FrontEnd (Blazor WebAssembly)

This project is the frontend component of the "Good Hamburger" system, built using **Blazor WebAssembly**. It follows a **Spec-Driven Development** strategy, consuming the backend API defined by the OpenAPI specification.

## 🚀 Development Strategy: Spec-Driven

To ensure consistency between the Backend and Frontend, we utilize the OpenAPI (Swagger) definition to drive the client-side development.

### 1. Client Generation
We use **NSwag** or **Microsoft.Extensions.ApiDescription.Client** to automatically generate the Typed HTTP Client.
*   **Tool**: `NSwag.CodeGeneration.CSharp`
*   **Source**: `http://localhost:5000/swagger/v1/swagger.json`
*   **Outcome**: A generated `GoodHamburgerClient.cs` containing all DTOs and Service methods, reducing manual mapping errors and ensuring type safety.

### 2. State Management
*   **Fluxor**: Implemented for complex state management (Cart, User Session) to maintain a single source of truth and predictable state transitions.

### 3. UI Framework
*   **MudBlazor**: Utilized for a professional, responsive material design interface.

---

## 🔐 Authentication & Authorization

The application uses **JWT (JSON Web Token)** for security.
*   **Login**: Consumes `/api/Auth/login`.
*   **Persistence**: Tokens are securely stored in `localStorage`.
*   **Interceptor**: A custom `DelegatingHandler` automatically attaches the `Authorization: Bearer <token>` header to all outgoing requests.
*   **Role-Based Access**: The UI dynamically adjusts based on the claims (Admin, Manager, Employee, Customer).

---

## 📦 Project Structure

### 👤 User (Customer) Section
Public and Customer-facing area focused on ordering.
*   **Menu/Catalog**: Browse categories and menu items (Burger, Sides, Drinks).
*   **Product Detail**: View ingredients and nutritional info (Calories).
*   **Shopping Cart**: Logic to enforce the "Only one per category" rule.
*   **Checkout**: Address entry and Coupon application (`/api/Orders`).
*   **My Orders**: Order history and real-time status tracking.

### 🛠️ Admin & Management Section
Protected area for staff and administrators.
*   **Dashboard**: Overview of revenue and order counts.
*   **Order Management**: Monitor the preparation pipeline (Pending -> InPreparation -> Ready -> etc.).
*   **Catalog Management**: CRUD operations for:
    *   **Categories**: Create, update, and sort menu categories.
    *   **Menu Items**: Manage products, pricing, and associations.
    *   **Ingredients**: Manage the stock items used in products.
*   **Marketing**: Manage **Coupons** (Create, Activate, Deactivate).
*   **Staff Management**: Manage Employee profiles and roles.

---

## 🛠️ How to Generate/Update the API Client

Whenever the Backend API changes:
1.  Ensure the Backend project is running.
2.  In the FrontEnd project directory, run:
    ```bash
    dotnet nswag run
    ```
    *Or use the Visual Studio "Refresh Web Service Client" option if using the built-in OpenAPI service reference.*

## 🏃 Execution Instructions

1.  **Configure API Base Address**: Update `appsettings.json` to point to the correct Backend URL.
2.  **Run the project**:
    ```bash
    dotnet run
    ```
3.  **Access**: Navigate to `https://localhost:5001`.

## 📱 Functional Screen Map

### 1. Customer Area (Public/Authenticated)
- **Home/Menu**: 
    - Display categories as tabs (Burgers, Sides, Drinks).
    - Grid of `MenuItem` cards with Price and "Add" button.
- **Cart/Order Drawer**: 
    - **Logic**: Enforce "Max 1 per category". Show validation error if user tries to add a second Burger.
    - **Discount Preview**: Dynamically show if the "Combo 10/15/20%" is active based on cart contents.
- **Checkout**: Address form (Street, Number, Zip) and Coupon field.
- **Order Success**: Display generated Order ID and current status (Pending).

### 2. Admin Area (Auth Required: Management Role)
- **Order Monitor**: A "KDS" (Kitchen Display System) style view. Columns for `Pending`, `InPreparation`, `Ready`. Buttons to "Advance" order status.
- **Catalog Manager**: 
    - List view with toggle for `IsActive` or `IsAvailable`.
    - Form to edit `Price` and upload `ImageUrl`.
- **Coupon Dashboard**: List of active coupons with "Deactivate" button.
