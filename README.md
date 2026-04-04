# 🍕 PizzariaFalia

A full-stack ASP.NET Core 8 MVC web application for managing and ordering from a pizzeria menu. Built with a clean layered architecture, role-based access control, a cart-based ordering system, and a dedicated admin panel for full menu management.

---

## Table of Contents

- [Project Concept](#project-concept)
- [Features](#features)
- [Architecture](#architecture)
- [Project Layers](#project-layers)
- [Services](#services)
- [Validation](#validation)
- [Data Seeding](#data-seeding)
- [Design Decisions](#design-decisions)
- [Test Coverage](#test-coverage)
- [Setup Instructions](#setup-instructions)
- [Tech Stack](#tech-stack)

---

## Project Concept

PizzariaFalia is an online ordering platform for a pizzeria. Guests can browse the full menu or filter by category. Authenticated customers can add dishes to a cart and place orders. Administrators have a dedicated panel to manage the entire menu — creating, editing, and soft-deleting categories and dishes — as well as managing registered users.

The application is intentionally structured around a real-world separation of concerns, where business logic lives in a services layer rather than in controllers, making each layer independently testable and maintainable.

---

## Features

### Customer-Facing
- Browse the full menu or filter dishes by category (including subcategories)
- View detailed information for any dish (size, grams, price)
- Add dishes to a persistent cart (small or big size)
- Remove individual items from the cart
- Place an order, which transitions the cart from `Pending` to `Ordered`
- View order history and order details
- Cancel a placed order

### Authentication & Identity
- Register and log in via ASP.NET Core Identity
- Email confirmation required for sign-in
- Role-based access control (`Admin` role)
- Admins can promote or demote any user to/from the Admin role

### Admin Panel (`/Administration`)
- Dedicated area restricted to users in the `Admin` role
- Create, edit, and soft-delete dishes
- Create and manage menu categories and subcategories
- View a paginated list of all registered users
- View and edit individual user details (username, email, address, email confirmation status)

---

## Architecture

The solution follows a **multi-project, layered architecture**. Each concern is isolated in its own class library, with the ASP.NET Core web application acting only as the composition root and presentation layer.

```
PizzariaFalia.sln
│
├── Web/
│   └── PizzariaFalia           → ASP.NET Core MVC app (controllers, views, Program.cs)
│
├── Services/
│   └── PizzariaFalia.Services.Core   → Business logic + service interfaces
│
├── Data/
│   ├── PizzariaFalia.Data            → EF Core DbContext, migrations, DataSeeder
│   ├── PizzariaFalia.Data.Models     → Entity classes
│   └── PizzariaFalia.Data.Models.Enums → Enums (Status)
│
└── Shared/
    ├── PizzariaFalia.ViewModels      → DTOs / view models used across layers
    ├── PizzariaFalia.Common          → Shared constants (ValidationConstants)
    └── PizzariaFalia.Tests           → NUnit test project
```

Dependency flow: `Web → Services.Core → Data → Data.Models`. The web layer never accesses the database directly — it only ever calls service interfaces.

---

## Project Layers

### `PizzariaFalia` (Web)
The ASP.NET Core MVC host. Contains controllers, Razor views, and `Program.cs`. All service dependencies are injected via interfaces registered in `Program.cs`. The `Administration` area groups all admin controllers under a common route prefix and role guard.

### `PizzariaFalia.Services.Core`
Contains all business logic. Services are defined behind interfaces (`ICartService`, `IMenuService`, etc.) so the web layer depends on abstractions, not implementations. This layer is the primary target for unit testing.

### `PizzariaFalia.Data`
Contains the `ApplicationDbContext` (inheriting `IdentityDbContext<ApplicationUser>`), EF Core migrations, and the `DataSeeder`. This layer owns all database configuration including the self-referencing category relationship.

### `PizzariaFalia.Data.Models`
Plain C# entity classes with data annotation constraints. No business logic resides here.

### `PizzariaFalia.ViewModels`
Shared DTOs that flow between the service layer and the web layer. Form view models carry their own `[Required]` and `[StringLength]` annotations, keeping validation logic close to the data shape.

### `PizzariaFalia.Common`
Houses `ValidationConstants` — a single source of truth for all length constraints used by both entity models and view model annotations.

---

## Services

| Interface | Implementation | Responsibility |
|---|---|---|
| `IMenuService` | `MenuService` | Read-only menu queries: categories (with subcategory tree), dish index, dish details, category-filtered dishes |
| `ICartService` | `CartService` | Cart lifecycle: create cart, add/remove items, retrieve cart contents, place order |
| `IOrderService` | `OrderService` | Order management: list all orders, order details, order items, change order status |
| `IAdminMenuChangeService` | `AdminMenuChangeService` | Admin CRUD: create/edit/soft-delete categories and dishes |
| `IUserManagerService` | `UserManagerService` | User management: list users, paginated list, get/edit user details |

All services are registered as **scoped** dependencies in `Program.cs` and receive `ApplicationDbContext` via constructor injection.

---

## Validation

Validation is applied at two levels:

**Model-level (Data Annotations on ViewModels)**

| ViewModel | Rules |
|---|---|
| `DishFormViewModel` | `Name`: required, 3–50 chars; `Description`: 10–1000 chars; `PriceSmall`, `GramsSmall`, `CategoryId`: required |
| `CategoryFormViewModel` | `Name` and `DisplayName`: required, 3–50 chars |
| `UserEditViewModel` | `UserName`: required; `Email`: valid email format |

Length constants are defined once in `ValidationConstants` and referenced by both the entity models (`[MaxLength]`) and the view models (`[StringLength]`), so the two layers always stay in sync.

**Service-level (Guard Clauses)**

Services perform additional runtime checks beyond what `ModelState` covers:

- `CartService` throws `ArgumentNullException` / `ArgumentException` on null items or empty user IDs
- `UserManagerService.EditUserAsync` throws `InvalidDataException` if the target username or email is already taken by another user
- `AdminMenuChangeService` throws `ArgumentException` for missing entities and `InvalidOperationException` for failed mutations
- `MenuService.GetDishDetailsAsync` throws `InvalidDataException` if the dish does not exist or has been soft-deleted

---

## Data Seeding

The `DataSeeder` class runs automatically on every application startup (after migrations are applied). It is idempotent — each method checks whether data already exists before inserting anything.

Seed data is loaded from three JSON files located in `PizzariaFalia/SeedData/`:

| File | Content |
|---|---|
| `categories.json` | Root categories and subcategories with fixed IDs |
| `dishes.json` | Full menu of dishes linked to seeded categories |
| `users.json` | Default user accounts (email + hashed password + address) |

Categories and dishes use `SET IDENTITY_INSERT ON/OFF` within a transaction to preserve the exact IDs defined in the JSON, ensuring foreign keys across the seed files remain consistent. Users are created through `UserManager<ApplicationUser>` so that passwords are properly hashed and all Identity infrastructure is correctly initialised.

---

## Design Decisions

**Soft deletes over hard deletes**
Dishes and categories are never removed from the database. Setting `isDeleted = true` preserves historical order records that reference those entities, avoiding orphaned foreign keys and keeping a full audit trail.

**Pending order as cart**
Rather than maintaining a separate cart table, the cart is modelled as an `Order` with `Status = Pending`. Placing the order simply changes the status to `Ordered`. This eliminates the need for a parallel data structure and keeps the order lifecycle in one place.

**Self-referencing category hierarchy**
Categories support one level of nesting (parent → subcategory) via a self-referencing foreign key. `GetDishesIndexByCategoryAsync` resolves both the parent and all its children so that filtering by a root category returns dishes from all subcategories automatically.

**Culture pinned to `en-US`**
Decimal separators in currency inputs are fixed to `en-US` format application-wide via `CultureInfo.DefaultThreadCurrentCulture`. This prevents locale-specific parsing failures with `decimal` form fields across different server environments.

**Centralised validation constants**
`ValidationConstants` in the `Common` project is the single source of truth for all string length constraints. This ensures that the database schema (via `[MaxLength]`) and the form validation (via `[StringLength]`) always agree without duplication.

**Interface-driven service layer**
Every service is defined behind an interface. This allows controller unit testing with mocked services and makes it straightforward to swap implementations (e.g. replacing `AdminMenuChangeService` with a version that publishes events) without touching any consuming code.

---

## Test Coverage

Unit tests live in the `PizzariaFalia.Tests` project and use **NUnit 4** with an **EF Core InMemory** provider. Each test class gets a fresh, isolated in-memory database via `DbContextFactory.Create()`, so tests are fully independent and run without any external infrastructure.

**94 tests across 5 test classes:**

| Test Class | Tests | What is covered |
|---|---|---|
| `AdminMenuChangeServiceTests` | 16 | Create/edit/delete category and dish, error paths for missing entities |
| `CartServiceTests` | 22 | Cart creation, add/remove items (both view model overloads), place order, price/gram selection by size, error guards |
| `MenuServiceTests` | 18 | Category tree queries, deleted entity filtering, subcategory inclusion, dish details, invalid dish handling |
| `OrderServiceTests` | 16 | Status transitions, all status values, order item field mapping, price/gram selection by size |
| `UserManagerServiceTests` | 22 | Get/edit user, duplicate username/email detection, null address fallback, pagination logic, ordering |

**To run the tests:**

```bash
dotnet test PizzariaFalia.Tests/PizzariaFalia.Tests.csproj
```

**To run with coverage:**

```bash
dotnet test PizzariaFalia.Tests/PizzariaFalia.Tests.csproj \
  --collect:"XPlat Code Coverage"
```

---

## Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local instance, Docker, or SQL Server Express)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/PizzariaFalia.git
cd PizzariaFalia
```

### 2. Configure the connection string

Open `PizzariaFalia/appsettings.json` and update the `DefaultConnection` string to point to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=PizzariaFaliaDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

For local development with Windows Authentication you can use:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PizzariaFaliaDb;Trusted_Connection=True;"
```

### 3. Run the application

```bash
cd PizzariaFalia
dotnet run
```

On first startup the application will automatically:
1. Apply all EF Core migrations (`db.Database.MigrateAsync()`)
2. Create the `Admin` role if it does not exist
3. Seed categories, dishes, and users from `SeedData/*.json`

No manual `dotnet ef database update` step is required.

### 4. Access the application

Navigate to `https://localhost:5001` (or the port shown in the terminal output).

Log in with a seeded user account, then use the **Admin** role controls to promote yourself to admin and access the `/Administration` panel.

### 5. Running tests

```bash
dotnet test
```

Tests use an in-memory database and require no SQL Server connection.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | ASP.NET Core Identity |
| Testing | NUnit 4, EF Core InMemory |
| Language | C# 12 / .NET 8 |
