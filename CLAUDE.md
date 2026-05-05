# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Run (full stack via Aspire)
```bash
cd Aspire.partonair-v01/Aspire.partonair-v01.AppHost
dotnet run
# Aspire dashboard: https://localhost:17360
# Starts SQL Server, Redis, API, and frontend automatically
```

### Run API only
```bash
cd Aspire.partonair-v01/Aspire.partonair-v01.Server
dotnet run
# API: https://localhost:7540
# Scalar docs: https://localhost:7540/scalar
```

### Run frontend only
```bash
cd Aspire.partonair-v01/frontend.partonair-v01
npm install
npm run dev      # http://localhost:5173
npm run build
npm run lint
```

### Database migrations
```bash
cd Infrastructure.partonair_v01
dotnet ef migrations add <MigrationName> -o ORM/EFCore/Migrations
# Migrations are applied automatically on startup via WaitingMigrationIsReadyAsync()
```

The `ApplicationDbContextFactory` reads `ConnectionStrings:SqlServer_local` from `Infrastructure.partonair_v01/appsettings.Development.json` for design-time (CLI) usage. At runtime, Aspire injects the connection string via `partonairdb`.

## Architecture

**Partonair** is a professional networking platform (profiles, contact requests, evaluations) built with .NET Aspire, ASP.NET Core 10, and React 19/Vite.

### Layer structure

```
Presentation  →  Aspire.partonair-v01.Server/   (Controllers, JWT, middleware)
Application   →  BLL.partonair_v01/              (Services, MediatR handlers, Mappers)
Domain        →  Domain.partonair_v01/           (Entities, Contracts, Exceptions, Enums)
Infrastructure→  Infrastructure.partonair_v01/   (EF Core DbContext, Repositories, UoW)
Shared DTOs   →  ShareModels.partonair-v01/      (Create/View/Update DTOs)
Orchestration →  Aspire.partonair-v01.AppHost/   (Registers SQL Server, Redis, API, frontend)
Frontend      →  frontend.partonair-v01/         (React SPA, proxies /api/* to backend)
```

### Key design patterns

**CQRS via MediatR** — All write operations go through `Commands/` and reads through `Queries/` in `BLL.partonair_v01/MediatR/`. Controllers inject `IMediator` and never call services directly. Commands/Queries are C# `record` types.

**Repository + Unit of Work** — `IGenericRepository<T>` provides base CRUD; domain-specific repositories extend it. `IUnitOfWork` aggregates all repositories and commits transactions via `SaveChangesAsync()`. No `DbContext` is accessed outside `Infrastructure.partonair_v01`.

**Mapper pattern** — Extension methods on DTOs (`.ToEntity()`) and on entities (`.ToView()`). No external mapping library. Password hashing and timestamps are assigned during mapping in `BLL.partonair_v01/Mappers/`.

**DI organization** — Three extension methods registered in `DependencyInjectionManager.cs`:
- `AddPresentationAPILayer()` — controllers, CORS, Swagger/Scalar, JWT auth, exception handler
- `AddApplicationLayer()` — MediatR, services (User, Profile, Contact, Evaluation), BCrypt, UoW
- `AddInfrastructureLayer()` — repositories

`DbContext` and Redis are wired by Aspire helpers in `Program.cs`:
```csharp
builder.AddSqlServerDbContext<ApplicationDbContext>("partonairdb");
builder.AddRedisOutputCache("cache");
```

**Exception handling** — `CustomExceptionHandler` maps domain exceptions from `Domain.partonair_v01/Exceptions/` to RFC 9457 ProblemDetails. HTTP codes: 400 (bad input/cancellation), 404 (not found), 409 (conflict/concurrency), 503 (DB unavailable), 500 (unexpected).

### Domain entities

| Entity | Key relationships |
|---|---|
| `User` | 1:1 ProfileUser, 1:n Announcements, n:m Contacts (sender/receiver), n:m Evaluations (evaluator/evaluated) |
| `ProfileUser` | 1:1 User, 1:n Images |
| `Contact` | SenderId + ReceiverId → User; Status enum: Pending/Accepted/Refused/Blocked |
| `Evaluation` | EvaluatorId + EvaluatedId → User |
| `Announcement` | UserId → User |

Roles enum: `Visitor`, `Employee`, `Company`.

### Frontend

- React Router v7 for routing; Flowbite + TailwindCSS for UI.
- `src/services/api.js` — fetch-based API client with named exports per domain (userApi, profileApi, etc.).
- `src/components/contexts/` — React Context for modal/auth state.
- Vite proxies `/api/*` to backend (`process.env.services__server__https__0` or `https://localhost:7540`).

### Authentication

JWT Bearer with hardcoded dev secrets in `DependencyInjectionManager.cs` (issuer `"ton-issuer"`, key `"ta-cle-super-secrete-min-256bits"`). BCrypt used for password hashing. CORS is fully open for development.

**Note:** JWT secrets must be moved to environment variables or a secrets manager before any production deployment.
