# EvuEase API (Backend)

ASP.NET Core Web API for EvuEase / EvalEase.

## Specifications

| Item | Value |
|------|--------|
| Framework | ASP.NET Core |
| Runtime / SDK | **.NET 10** |
| Language | C# |
| ORM | Entity Framework Core 10 (SQL Server provider) |
| Auth | JWT Bearer |
| API docs | Scalar / OpenAPI (`/scalar` in Development) |
| Default HTTPS URL | `https://localhost:7252` |
| Default HTTP URL | `http://localhost:5218` |
| Database | Microsoft SQL Server |
| Default catalog | `evalease` |

### Solution structure

```
EvuEase-API/
├── EvuEase.Api/                 # Web host, controllers, appsettings
├── EvuEase.Application/         # Services, DTOs, interfaces
├── EvuEase.Domain/              # Entities
├── EvuEase.Infrastructure/      # EF Core, repositories, storage, migrations
└── EvuEase.slnx
```

---

## Required software / packages

| Software | Required? | Purpose |
|----------|-----------|---------|
| **.NET 10 SDK** | Yes | Build and run the API (`dotnet`) |
| **SQL Server** | Yes | Application database (`evalease`) |
| **SSMS** or **Azure Data Studio** | Recommended | Create DB / run migration scripts |
| **Docker Desktop** | Optional | Run SQL Server in a container (especially Mac/Linux) |
| **Visual Studio 2022** or **VS Code + C# Dev Kit** | Optional | IDE / debugging |
| **Git** | Optional | Clone the repository |

NuGet packages (EF Core, JWT, OpenAPI/Scalar, etc.) are restored with `dotnet restore` — you do not install them manually.

## Prerequisites by OS

### Windows

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Install one of:
   - [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) + [SSMS](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms), or
   - **SQL Server LocalDB** (often included with Visual Studio / Build Tools), or
   - Docker Desktop + SQL Server container (see below).
3. Optional: [Visual Studio 2022](https://visualstudio.microsoft.com/) with ASP.NET workload, or VS Code + C# Dev Kit.

Verify:

```powershell
dotnet --version
```

### macOS

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Intel or Apple Silicon).
2. Install SQL Server via **Docker** (recommended on Mac):

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_strong_Password123" \
  -p 1433:1433 --name evuease-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

3. Optional: [Azure Data Studio](https://azure.microsoft.com/products/data-studio/) or `sqlcmd` to manage the database.

Verify:

```bash
dotnet --version
docker ps
```

### Linux

1. Install [.NET 10 SDK](https://learn.microsoft.com/dotnet/core/install/linux) for your distribution.
2. Install SQL Server via **Docker** (recommended), or [SQL Server on Linux](https://learn.microsoft.com/sql/linux/sql-server-linux-overview) if supported on your distro.
3. Optional: Azure Data Studio or `sqlcmd`.

Example (Ubuntu/Debian-style after SDK install):

```bash
dotnet --version
```

Docker SQL Server:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_strong_Password123" \
  -p 1433:1433 --name evuease-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## Database setup

The API uses **SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`). Create a database named `evalease` (or match the name in your connection string).

### Option A — Windows LocalDB (default `appsettings.json`)

Default connection string:

```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=evalease;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Create the database (PowerShell / `sqlcmd`):

```powershell
sqllocaldb start MSSQLLocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF DB_ID('evalease') IS NULL CREATE DATABASE evalease;"
```

Or create `evalease` in **SSMS** → Connect to `(localdb)\MSSQLLocalDB` → New Database.

### Option B — SQL Server Express / full SQL Server (Windows)

Example connection string:

```
Server=localhost\SQLEXPRESS;Database=evalease;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Or with SQL authentication:

```
Server=localhost,1433;Database=evalease;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;MultipleActiveResultSets=true
```

### Option C — Docker SQL Server (Windows / macOS / Linux)

1. Start the container (see Prerequisites).
2. Create the database:

```bash
docker exec -it evuease-sql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "Your_strong_Password123" -C \
  -Q "IF DB_ID('evalease') IS NULL CREATE DATABASE evalease;"
```

If `sqlcmd` path differs inside the image, use Azure Data Studio / any SQL client against `localhost,1433`.

3. Set connection string in `EvuEase.Api/appsettings.json` or `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=evalease;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Apply schema (tables and constraints)

SQL scripts live under:

`EvuEase.Infrastructure/Migrations/`

Recommended approach for a fresh SQL Server database:

1. Create empty database `evalease`.
2. Run migration scripts in chronological order from `EvuEase.Infrastructure/Migrations/` (`.sql` files such as soft-delete columns, credit request tables, course PK, evaluation audit, supporting document columns, etc.).
3. Alternatively, if you already have a `.bacpac` (for example `evalease_script.bacpac` in this repo), import it with SSMS or Azure Data Studio into `evalease`.

MySQL note: `phpmyadmin_create_all_tables.sql` is a MySQL/MariaDB schema dump for reference. The running API is wired to **SQL Server**, not MySQL, unless you change the EF provider and connection string.

### Configure JWT and CORS

Edit `EvuEase.Api/appsettings.json` (and Development overrides as needed):

- `Jwt:SecretKey` — at least 32 characters for HS256
- `Cors:AllowedOrigins` — must include the Angular origin (`http://localhost:4200`)

---

## Setup and run

### 1. Restore and build

**Windows (PowerShell):**

```powershell
cd EvuEase-API
dotnet restore
dotnet build
```

**macOS / Linux:**

```bash
cd EvuEase-API
dotnet restore
dotnet build
```

### 2. Run the API

HTTPS profile (matches frontend `environment.ts`):

```bash
cd EvuEase.Api
dotnet run --launch-profile https
```

Or:

```bash
dotnet run --project EvuEase.Api --launch-profile https
```

Then open:

- API base: `https://localhost:7252`
- Scalar docs: `https://localhost:7252/scalar`

On first HTTPS run, trust the development certificate:

**Windows / macOS:**

```bash
dotnet dev-certs https --trust
```

**Linux:** certificate trust varies by distro; you may use HTTP (`http://localhost:5218`) for local testing and point the frontend `apiUrl` accordingly, or follow [.NET HTTPS Linux guidance](https://learn.microsoft.com/aspnet/core/security/enforcing-ssl).

### 3. File storage folders

The API stores uploaded files under the Api project content root, for example:

- `App_Data/credit-request-documents/`
- `App_Data/curriculum-supporting-documents/`

These are created automatically on first upload.

OCR for curriculum PDF parsing uses `EvuEase.Api/tessdata/eng.traineddata` (copied to output on build).

---

## Common commands

| Command | Purpose |
|---------|---------|
| `dotnet restore` | Restore NuGet packages |
| `dotnet build` | Build the solution |
| `dotnet run --project EvuEase.Api --launch-profile https` | Run API with HTTPS |
| `dotnet run --project EvuEase.Api --launch-profile http` | Run API with HTTP only |

---

## Troubleshooting

| Issue | What to try |
|-------|-------------|
| Build fails with file locked (`EvuEase.Api.dll`) | Stop the running API / Visual Studio debug session, then rebuild |
| Cannot connect to database | Confirm SQL Server/LocalDB/Docker is running; verify `DefaultConnection` |
| Frontend CORS errors | Add `http://localhost:4200` to `Cors:AllowedOrigins` |
| HTTPS / certificate errors in browser | Run `dotnet dev-certs https --trust` |
| 500 on audit trail / supporting docs | Ensure related SQL migrations were applied to `evalease` |

---

## Related

- Frontend setup: [../EvuEase-Web/README.md](../EvuEase-Web/README.md)
- Repository overview: [../README.md](../README.md)
