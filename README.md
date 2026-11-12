# CRUD ASP.NET Core + Angular

A Todo CRUD app with an ASP.NET Core Web API (EF Core + SQL Server, JWT auth) backend
and an Angular frontend (Angular Material + Tailwind CSS).

## Project structure

```
.
├── netCurdApi/          # ASP.NET Core Web API (backend)
└── AngularFrontend/     # Angular app (frontend)
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (project targets `net10.0`)
- [Node.js](https://nodejs.org/) `^20.19`, `^22.12`, or `>=24` (required by Angular 21)
- SQL Server Express (instance `SQLEXPRESS`)
- SQL Server Management Studio (SSMS) — optional, for browsing the database
- `dotnet-ef` tool (only if you need to run migrations manually):

  ```powershell
  dotnet tool install --global dotnet-ef
  ```

## Database connection

The backend reads the connection string from `netCurdApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AppBase;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- `Server=.\\SQLEXPRESS` — local SQL Server Express instance (use `.\SQLEXPRESS` or `YOUR-PC-NAME\SQLEXPRESS`).
- `Database=AppBase` — database name used by the app.
- `Trusted_Connection=True` — uses Windows Authentication.
- `TrustServerCertificate=True` — avoids the self-signed certificate error on local dev.

### Connect with SSMS

1. Open **SQL Server Management Studio**.
2. In **Connect to Server**:
   - **Server type:** Database Engine
   - **Server name:** `.\SQLEXPRESS` (or `YOUR-PC-NAME\SQLEXPRESS`)
   - **Authentication:** Windows Authentication
3. Click **Connect**, then expand **Databases → AppBase**.

> If you cannot connect, make sure the **SQL Server (SQLEXPRESS)** service is running
> (`services.msc`). Verify the machine name with `hostname` in a terminal.

### Apply migrations / create the database

The `AppBase` database and its tables are created from the migrations in
`netCurdApi/Migrations` (a single `InitialCreate` migration). From the `netCurdApi` folder run:

```powershell
dotnet ef database update
```

If you prefer to create the database manually in SSMS first, run:

```sql
CREATE DATABASE AppBase;
```

then run `dotnet ef database update` to create the tables and seed data.

### Seed data

`TodoItems` (`todo_01` … `todo_05`) plus three users:

| Username | Password    | Status   |
| -------- | ----------- | -------- |
| `admin`  | `1234`      | Active   |
| `alice`  | `Alice@123` | Active   |
| `bob`    | `Bob@123`   | Inactive (login returns 403) |

## Run the backend

```powershell
cd netCurdApi
dotnet restore
dotnet run
```

The API starts on:

- HTTP: `http://localhost:5044`
- HTTPS: `https://localhost:7215`

Swagger UI (Development only): `http://localhost:5044/swagger`

## Run the frontend

In a second terminal:

```powershell
cd AngularFrontend
npm install
npm start
```

Then open `http://localhost:4200/`.

The Angular dev server runs on port **4200** and calls the API at `http://localhost:5044`.
The API has a CORS policy for `http://localhost:4200`; if you change the port, update the
policy in `netCurdApi/Program.cs`.

### Frontend routes

| Route       | Page                  | Access                        |
| ----------- | --------------------- | ----------------------------- |
| `/main`     | Dashboard             | Public                        |
| `/login`    | Login                 | Public                        |
| `/register` | Register              | Public                        |
| `/todo`     | Todo list             | Requires login (`authGuard`)  |
| `` / `**`   | Redirects to `/main`  |                               |

JWT is stored in `localStorage` and attached to API requests by `auth.interceptor.ts`.

## Auth API

Base URL: `http://localhost:5044/api/auth`

### Register

`POST /api/auth/register`

```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "Passw0rd!"
}
```

### Login

`POST /api/auth/login`

```json
{
  "usernameOrEmail": "admin",
  "password": "1234"
}
```

Both return:

```json
{
  "token": "<jwt>",
  "expiresAt": "2026-01-01T00:00:00Z",
  "user": { "id": 1, "username": "admin", "email": "admin@example.com", "status": "A", "createdAt": "..." }
}
```

Send the token on protected requests:

```
Authorization: Bearer <jwt>
```

## Tasks API

Base URL: `http://localhost:5044/api/tasks`

| Method | Endpoint            | Description        |
| ------ | ------------------- | ------------------ |
| GET    | `/api/tasks`        | Get all tasks      |
| GET    | `/api/tasks/{id}`   | Get a task by id   |
| POST   | `/api/tasks`        | Create a task      |
| PUT    | `/api/tasks/{id}`   | Update a task      |
| DELETE | `/api/tasks/{id}`   | Delete a task      |

Example request body for POST/PUT:

```json
{
  "title": "New task",
  "description": "Task details",
  "isCompleted": false
}
```

## Troubleshooting

- **Cannot connect in SSMS** — the server name must match your machine. Use
  `.\SQLEXPRESS` or run `hostname` and use `HOSTNAME\SQLEXPRESS`. Confirm the
  `SQL Server (SQLEXPRESS)` service is running.
- **Login failed / certificate error** — ensure the connection string uses
  `Trusted_Connection=True` and `TrustServerCertificate=True`.
- **Database does not exist** — run `dotnet ef database update` from `netCurdApi`.
- **CORS error in the browser** — the API allows `http://localhost:4200`; if the Angular
  dev server uses another port, update the policy in `netCurdApi/Program.cs`.
- **`npm`/`npx` blocked by PowerShell** — the execution policy may block the `.ps1`
  shims; use `npm.cmd` / `npx.cmd` instead.



Test-Path "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe"
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"
dotnet ef migrations add InitialCreate
dotnet ef database update