# Development Plan: Good Hamburger Frontend

This document provides a step-by-step execution plan for the Gemini Agent to build the Blazor WebAssembly frontend for the Good Hamburger project.

## 🎯 Goal
Build a professional, responsive Blazor WASM application with Customer and Admin sections, strictly following the OpenAPI specification and business rules.

---

## 🛠 Phase 1: Infrastructure & Setup

### 1.1 Project Initialization
- Create a Blazor WebAssembly project net10.0.
- Install NuGet packages: `MudBlazor`, `Fluxor`, `Microsoft.AspNetCore.Components.WebAssembly.Authentication`.
- Register MudBlazor services and add CSS/JS references in `index.html`.

### 1.2 API Client Generation
- Use NSwag to generate `GoodHamburgerClient.cs` from `http://localhost:5000/swagger/v1/swagger.json`.
- Ensure all DTOs and methods match the provided OpenAPI JSON.

### 1.3 Authentication (JWT)
- Implement `AuthService` for Login/Logout/Refresh.
- Create a `DelegatingHandler` to intercept HTTP calls and add the `Authorization: Bearer` header.
- Create a custom `AuthenticationStateProvider` to manage User Claims and Roles (Admin, Manager, Employee, Customer).

---

## 🍔 Phase 2: Customer Section (Public/Authenticated)

### 2.1 Menu & Catalog
- **Page**: `Menu.razor`
- **Logic**: Fetch active categories and available items.
- **UI**: MudTabs for categories; MudCards for items.

### 2.2 Shopping Cart (Fluxor)
- **State**: Store list of selected items.
- **Business Rule Validation**: 
    - **CRITICAL**: Only 1 Burger, 1 Side, 1 Drink allowed per order.
    - If user adds a second item of the same category, show a MudSnackbar error: *"Each order can contain only one [Category]."*
- **Discount Preview**: Calculate if the current selection qualifies for 10% (B+S), 15% (B+D), or 20% (B+S+D) combos.

### 2.3 Checkout & My Orders
- **Checkout**: Form for `Address` and `CouponCode`.
- **My Orders**: List view of customer's previous orders with status tracking.

---

## ⚙️ Phase 3: Admin & Management Section

### 3.1 Order Monitor (KDS)
- **View**: A board showing orders by status.
- **Interaction**: Buttons to advance status (e.g., "Confirm Payment" -> "Start Preparation" -> "Ready").

### 3.2 Catalog Management
- **CRUD Categories**: Manage name, description, and display order.
- **CRUD MenuItems**: Edit prices, descriptions, and associate ingredients.
- **CRUD Ingredients**: Inventory management.

### 3.3 Coupons & Marketing
- List and create Coupons.
- Implement "Cancel" button to deactivate a coupon (using `POST /api/Coupons/{id}/cancel`).

---

## 🧪 Phase 4: UX & Refinement

### 4.1 Global Error Handling
- Handle 401 (Unauthorized), 403 (Forbidden), and 400 (Bad Request) globally.
- Show `Error.Message` from the API in a MudDialog or Snackbar.

### 4.2 Form Validation
- Use `EditContext` and DataAnnotations for all forms.
- Ensure `Sku` and `Slug` fields are validated.

### 4.3 Loading States
- Implement `MudProgressCircular` for all asynchronous API operations.
