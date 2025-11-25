# Streamflix API

A .NET Web API for a streaming service, built with ASP.NET Core, Entity Framework Core, and PostgreSQL.

## Getting Started

Follow these steps to get the project running on your local machine after cloning the repository.

### Prerequisites

*   **Docker Desktop**: Required to run the database and API easily.
*   **Git**: To clone the repository.
*   *(Optional)* **.NET 10 SDK**: Only if you want to run the API outside of Docker.

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd streamflix
```

### 2. Run with Docker (Recommended)

This is the easiest way to run the application. It sets up both the API and the PostgreSQL database automatically.

```bash
docker-compose up --build
```

*   Wait for the logs to show `Application started. Press Ctrl+C to shut down.`
*   The database migrations will apply automatically on startup.
*   Seed data (Genres, Qualities, Warnings) will be inserted automatically.

### 3. Access the Database (pgAdmin)

This project includes **pgAdmin** as a web-based PostgreSQL management tool, running in Docker.

Once `docker-compose` is up:

*   Open: `http://localhost:8080`
*   Log in with (from `docker-compose.yml`):
    *   Email: `admin@example.com`
    *   Password: `admin`

#### Register the database server in pgAdmin (first time only per machine)

1. In pgAdmin, right-click **Servers** → **Register** → **Server…**
2. **General** tab:
    *   Name: `streamflix` (any name is fine)
3. **Connection** tab:
    *   Host name/address: `db`
    *   Port: `5432`
    *   Maintenance database: `streamflixdb`
    *   Username: `postgres`
    *   Password: `postgres`
4. Click **Save**.

You should now see the `streamflixdb` database under **Databases → streamflixdb → Schemas → public → Tables**.

### 3. Access the API

Once the application is running, you can access the Swagger UI to test the endpoints:

*   **Swagger UI**: [http://localhost:5001/swagger](http://localhost:5001/swagger)

---

## Development Guide

### Project Structure

*   **`src/Api`**: The ASP.NET Core Web API project.
    *   **`Controllers`**: Handles HTTP requests.
    *   **`DTOs`**: Data Transfer Objects (API contracts).
*   **`src/Infrastructure`**: Database and Domain logic.
    *   **`Entities`**: Database table definitions.
    *   **`Data`**: DbContext and Seeding logic.

### How to Add New Entities

If you want to extend the application (e.g., add an `Account` entity), follow these steps:

#### 1. Create the Entity Class
**Where:** `src/Infrastructure/Entities/Account.cs`
Define the class that represents your database table.
*   Add properties like `Id`, `Email`, `PasswordHash`, `Username`.
*   Add any relationships (e.g., `List<Content> Watchlist`).

#### 2. Register in DbContext
**Where:** `src/Infrastructure/Data/ApplicationDbContext.cs`
Tell Entity Framework that this new class should be a table in the database.
*   Add a property: `public DbSet<Account> Accounts { get; set; }`
*   (Optional) Configure specific rules in `OnModelCreating` (e.g., `entity.HasIndex(e => e.Email).IsUnique()`).

#### 3. Create a Database Migration
**Where:** Terminal
Generate the code that will actually update your PostgreSQL database schema.
```bash
dotnet ef migrations add AddAccountEntity --project Infrastructure --startup-project Api
```
*   This creates a new file in `Migrations` describing the changes.
*   *Note: Since auto-migration is set up in `Program.cs`, this will apply automatically next time you restart the app.*

#### 4. Create DTOs (Data Transfer Objects)
**Where:** `src/Api/DTOs/AccountDtos.cs`
Define what the API accepts and returns. **Never expose your Entity directly** (especially for Accounts with passwords!).
*   `CreateAccountDto`: Contains `Username`, `Email`, `Password`.
*   `AccountDto`: Contains `Id`, `Username`, `Email` (NO password).

#### 5. Create the Controller
**Where:** `src/Api/Controllers/AccountsController.cs`
Create the endpoints to manage accounts.
*   Inject `ApplicationDbContext`.
*   **POST**: Accept `CreateAccountDto` -> Create `Account` Entity -> Save to DB -> Return `AccountDto`.
*   **GET**: Fetch `Account` Entity -> Convert to `AccountDto` -> Return JSON.

#### 6. Test
**Where:** Swagger UI
*   Restart the application: `docker-compose restart api`
*   Go to Swagger and test your new `/accounts` endpoints.

---

## Troubleshooting

**"Connection Refused" when running in Docker**
*   Ensure `src/Api/appsettings.Development.json` has `"Host=db"` in the connection string, NOT `"Host=localhost"`. Docker containers talk to each other by service name.

**"Connection Refused" when running locally (`dotnet run`)**
*   If running outside Docker, you must change the connection string in `appsettings.Development.json` to `"Host=localhost"`.
