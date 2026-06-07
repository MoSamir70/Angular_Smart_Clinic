# Smart Clinic

A full-stack **clinic queue and appointment management** system. Patients join a doctor's waiting queue, doctors call the next patient in real time, and admins manage clinics and staff. The backend uses **Clean Architecture** with **CQRS (MediatR)**; the frontend is **Angular 17** with **SignalR** for live updates.

---

## What This Project Does

| Role | Features |
|------|----------|
| **Patient** (`/patient`) | Register, join a doctor's queue, see position and estimated wait, receive live "you're called" notifications |
| **Doctor** (`/doctor`) | View current queue, call next patient, complete visits |
| **Admin** (`/admin`) | Manage clinics and doctors |

**Core domain:** Clinics, Doctors, Patients, Appointments, Queue Tickets, Notifications.

**Real-time:** SignalR hub at `/hubs/queue` pushes queue updates, ticket calls, and position changes to connected clients.

**API:** REST at `http://localhost:5250/api` — Swagger UI at `http://localhost:5250/swagger` in Development.

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| API | ASP.NET Core 8, Swagger, SignalR |
| Application | MediatR, AutoMapper, FluentValidation |
| Data | Entity Framework Core 8, **SQLite** (local `smartclinic.db`) |
| Frontend | Angular 17, Angular Material, `@microsoft/signalr` |
| Optional (configured, dev-disabled) | Redis backplane, SQL Server, Hangfire |

---

## Project Structure

```
Smart Clinic/
├── SmartClinic.sln              # .NET solution (backend only)
├── src/
│   ├── SmartClinic.Domain/      # Entities, enums, domain logic
│   ├── SmartClinic.Application/ # CQRS commands/queries, DTOs, interfaces
│   ├── SmartClinic.Infrastructure/ # EF Core DbContext, repositories, services
│   └── SmartClinic.API/         # Controllers, SignalR hub, Program.cs
└── smart-clinic-frontend/       # Angular SPA
    └── src/app/
        ├── components/          # patient, doctor, admin dashboards
        ├── services/            # api.service, signalr.service
        └── models/              # TypeScript interfaces
```

### Structure assessment

**What is already good**

- Clear **Clean Architecture** split: Domain → Application → Infrastructure → API.
- **Feature-based CQRS** under `Application/Features/` (Queue, Appointments, etc.).
- Frontend separated with role-based routes and dedicated API/SignalR services.
- Solution builds successfully with .NET 8.

**What should be improved**

| Issue | Recommendation |
|-------|----------------|
| `Samrt Clinic 2/` duplicate backend + zip archives | Remove or move outside the repo; keep a single `src/` as source of truth |
| No root `.gitignore` | Add one covering `.vs/`, `bin/`, `obj/`, `*.db`, `node_modules/`, `dist/` |
| Frontend not referenced in `.sln` | Either add a solution folder for docs/scripts, or document the two-app layout explicitly (this README) |
| No `tests/` project | Add `SmartClinic.Tests` for API and application layer |
| Commented-out blocks in `Program.cs` and `.csproj` files | Clean up; use git history instead of large comment blocks |
| `appsettings.json` lists SQL Server + Redis but runtime uses SQLite + in-memory SignalR | Align config with actual dev setup or use `appsettings.Development.json` |
| No Docker / compose file | Optional: add `docker-compose.yml` for consistent local runs |
| Build artifacts in workspace | Ensure `.gitignore` excludes `bin/`, `obj/`, `.angular/` |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or newer, e.g. .NET 9 SDK)
- [Node.js](https://nodejs.org/) 18+ and npm

---

## How to Run

Run **backend** and **frontend** in two terminals.

### 1. Backend API

```powershell
cd "d:\Smart Clinic"
dotnet restore SmartClinic.sln
dotnet run --project src\SmartClinic.API\SmartClinic.API.csproj
```

- API: `http://localhost:5250`
- Swagger: `http://localhost:5250/swagger`
- SignalR: `http://localhost:5250/hubs/queue`
- SQLite database is created automatically as `smartclinic.db` in the API working directory.

### 2. Frontend

```powershell
cd "d:\Smart Clinic\smart-clinic-frontend"
npm install
npm start
```

- App: `http://localhost:4200`
- Default route redirects to **Patient** dashboard (`/patient`)

Also available: `/doctor`, `/admin`.

### 3. Verify

1. Open `http://localhost:5250/swagger` and confirm the API is up.
2. Open `http://localhost:4200` — the UI calls `http://localhost:5250/api` and connects to SignalR on port `5250`.
3. Use **Admin** to create a clinic and doctor, then **Patient** to register and join the queue.

---

## API Overview

| Controller | Purpose |
|------------|---------|
| `PatientsController` | CRUD, join queue, queue status |
| `DoctorsController` | CRUD, view queue, call next patient |
| `ClinicsController` | Clinic management |
| `AppointmentsController` | Book, cancel, reschedule |
| `QueueController` | Queue operations (join, complete, cancel) |

---

## Architecture (high level)

```mermaid
flowchart LR
  subgraph frontend [Angular Frontend]
    P[Patient Dashboard]
    D[Doctor Dashboard]
    A[Admin Dashboard]
  end

  subgraph api [SmartClinic.API]
    REST[REST Controllers]
    Hub[QueueHub SignalR]
  end

  subgraph app [SmartClinic.Application]
    CQRS[MediatR Commands/Queries]
  end

  subgraph infra [SmartClinic.Infrastructure]
    EF[EF Core + SQLite]
  end

  P --> REST
  D --> REST
  A --> REST
  P --> Hub
  D --> Hub
  REST --> CQRS
  Hub --> CQRS
  CQRS --> EF
```

---

## Configuration

| Setting | Location | Default (dev) |
|---------|----------|---------------|
| API URL | `smart-clinic-frontend/src/app/services/api.service.ts` | `http://localhost:5250/api` |
| SignalR URL | `smart-clinic-frontend/src/app/services/signalr.service.ts` | `http://localhost:5250/hubs/queue` |
| API port | `src/SmartClinic.API/Properties/launchSettings.json` | `5250` |
| CORS | `Program.cs` | Allows `http://localhost:4200` in Development |

---

## Build for production

```powershell
# Backend
dotnet publish src\SmartClinic.API\SmartClinic.API.csproj -c Release -o ./publish/api

# Frontend
cd smart-clinic-frontend
npm run build
# Output: smart-clinic-frontend/dist/smart-clinic-frontend/
```

Update frontend `baseUrl` and SignalR URL to your deployed API host before production builds.

---

## License

Not specified in the repository. Add a `LICENSE` file if you plan to open-source or distribute this project.
