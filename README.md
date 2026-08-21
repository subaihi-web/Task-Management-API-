# Task Management API

A complete ASP.NET Core Web API implementation of the Task Management assignment.

## Assignment coverage

The project implements:

- User signup and login.
- BCrypt password hashing.
- JWT Bearer authentication.
- JWT claims: `UserId`, `Username`, `Email`.
- User-specific task isolation.
- Task CRUD operations.
- Task filtering by title and status.
- `FromDate <= ToDate` validation.
- Relational foreign keys from `Tasks` to `Users` and `Lookups`.
- Unique username and email constraints.
- Entity Framework Core Code First model configuration.
- EF Core seeded lookup values.
- DTOs for request parameters and response objects.
- Swagger/OpenAPI documentation with JWT authorization support.
- Folder structure separating controllers, DTOs, data, models, services, settings, and extensions.

The requirements are based directly on the supplied assignment PDF. In particular, the required endpoints and lookup values are implemented as specified there.

## Technology

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQLite relational database
- JWT Bearer authentication
- BCrypt password hashing
- Swagger / OpenAPI

SQLite is used so the evaluator can run the assignment without installing SQL Server. The schema still uses relational tables and foreign keys as required.

## Project structure

```text
TaskManagementApi/
├── TaskManagement.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── TasksController.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── AuthResponseDto.cs
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── SignupRequestDto.cs
│   │   │   └── SignupResponseDto.cs
│   │   └── Tasks/
│   │       ├── TaskCreateDto.cs
│   │       ├── TaskCriteriaDto.cs
│   │       ├── TaskResponseDto.cs
│   │       └── TaskUpdateDto.cs
│   ├── Extensions/
│   │   └── ClaimsPrincipalExtensions.cs
│   ├── Models/
│   │   ├── Lookup.cs
│   │   ├── Task.cs
│   │   └── User.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── IAuthService.cs
│   │   ├── ITaskService.cs
│   │   ├── JwtTokenService.cs
│   │   └── TaskService.cs
│   ├── Settings/
│   │   └── JwtSettings.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── TaskManagement.Api.csproj
├── docs/
│   └── api-examples.md
├── .gitignore
└── README.md
```

## Database model

### Users

| Column | Type | Notes |
|---|---|---|
| Id | long | Primary key |
| Username | string | Required, unique |
| Email | string | Required, unique |
| HashedPassword | string | Password is stored as a BCrypt hash |

### Lookups

| Column | Type | Notes |
|---|---|---|
| Id | long | Primary key |
| MajorCode | int | Category code |
| MinorCode | int | Type code within category |
| Name | string | Lookup name |

### Tasks

| Column | Type | Notes |
|---|---|---|
| Id | long | Primary key |
| Title | string | Required |
| Description | string? | Nullable |
| FromDate | datetime | Start date |
| ToDate | datetime | Due date |
| StatusId | long | FK to `Lookups.Id` |
| UserId | long | FK to `Users.Id` |

## Seeded lookup data

The application seeds the required values through EF Core model configuration:

| Id | MajorCode | MinorCode | Name |
|---:|---:|---:|---|
| 1 | 1 | 0 | Task Status |
| 2 | 1 | 1 | Initiated |
| 3 | 1 | 2 | In Progress |
| 4 | 1 | 3 | Completed |
| 5 | 1 | 4 | Cancelled |

Only IDs 2-5 are accepted as task statuses. ID 1 represents the task-status category itself.

## API endpoints

### Authentication

#### `POST /api/auth/Signup`

Creates a user.

Example request:

```json
{
  "username": "wasan",
  "email": "wasan@example.com",
  "password": "Password123!"
}
```

Duplicate email and duplicate username are rejected with HTTP 400.

#### `POST /api/auth/Login`

Authenticates a user and returns a JWT.

Example request:

```json
{
  "email": "wasan@example.com",
  "password": "Password123!"
}
```

The JWT contains the required claims:

```text
UserId
Username
Email
```

### Tasks

All task endpoints require:

```http
Authorization: Bearer <JWT>
```

#### `GET /api/tasks/GetByCriteria`

Returns only the logged-in user's tasks.

Optional query parameters:

```text
?Title=report
?StatusId=3
?Title=report&StatusId=3
```

#### `GET /api/tasks/GetById/{id}`

Returns a task only when it belongs to the logged-in user.

- 200: owned task returned.
- 404: task does not exist.
- 400: task exists but belongs to another user.

#### `POST /api/tasks/Add`

Creates a task for the logged-in user. The `UserId` is intentionally taken from the JWT rather than the request body so a user cannot create a task for another account.

Example:

```json
{
  "title": "Prepare assignment",
  "description": "Complete the Task Management API.",
  "fromDate": "2026-08-21T09:00:00",
  "toDate": "2026-08-25T18:00:00",
  "statusId": 2
}
```

The API validates that `FromDate` is less than or equal to `ToDate` and that `StatusId` is one of the seeded task-status lookups.

#### `PUT /api/tasks/Update`

Updates an existing owned task.

Example:

```json
{
  "id": 1,
  "title": "Prepare final assignment",
  "description": "Finish documentation and testing.",
  "fromDate": "2026-08-21T09:00:00",
  "toDate": "2026-08-26T18:00:00",
  "statusId": 3
}
```

#### `DELETE /api/tasks/Delete/{id}`

Deletes an owned task.

- 204: deleted successfully.
- 404: task does not exist.
- 400: task belongs to another user.

## How to run

### Prerequisites

Install:

1. .NET 8 SDK.
2. Git.

Verify:

```bash
dotnet --version
git --version
```

### Restore and run

From the repository root:

```bash
dotnet restore TaskManagement.Api/TaskManagement.Api.csproj
dotnet run --project TaskManagement.Api/TaskManagement.Api.csproj
```

The application automatically creates the SQLite database and applies the EF Core model when it starts.

Swagger is available at:

```text
http://localhost:5180/swagger
```

or the HTTPS launch URL shown by the application.

### Important JWT note

The JWT key committed in `appsettings.json` is a development-only sample key. Do not use it in production. For a real deployment, override `Jwt:Key` with a strong secret through environment variables or another secret store.

Example environment variable on Windows PowerShell:

```powershell
$env:Jwt__Key="your-long-random-production-secret"
```

Example on Linux/macOS:

```bash
export Jwt__Key="your-long-random-production-secret"
```

## Recommended test flow

1. Call `Signup`.
2. Call `Login` and copy the returned token.
3. Click **Authorize** in Swagger.
4. Enter `Bearer <token>`.
5. Add a task with `statusId: 2`.
6. Get all tasks.
7. Filter by title.
8. Filter by status.
9. Get the task by ID.
10. Update the task.
11. Delete the task.
12. Create a second user and verify that the first user's task cannot be accessed or modified by the second user.
13. Test duplicate email and username.
14. Test `FromDate > ToDate`.
15. Test an invalid `StatusId`.

## Ownership/security design

The task owner is never accepted from the client. For every protected endpoint, the service obtains the user ID from the authenticated JWT and applies it to database queries or checks ownership before modification.

This is important because filtering task data only on the client side would not provide security. The ownership restriction is enforced server-side.

## Assignment-to-code mapping

| Assignment requirement | Implementation |
|---|---|
| User model | `Models/User.cs` |
| Lookup model | `Models/Lookup.cs` |
| Task model | `Models/Task.cs` |
| Unique username | EF unique index + signup validation |
| Unique email | EF unique index + signup validation |
| Hashed password | BCrypt in `AuthService` |
| JWT bearer | `Program.cs` + `JwtTokenService.cs` |
| UserId/Username/Email claims | `JwtTokenService.cs` |
| Signup | `AuthController` / `AuthService` |
| Login | `AuthController` / `AuthService` |
| GetByCriteria | `TasksController` / `TaskService` |
| GetById | `TasksController` / `TaskService` |
| Add | `TasksController` / `TaskService` |
| Update | `TasksController` / `TaskService` |
| Delete | `TasksController` / `TaskService` |
| Date validation | `TaskCreateDto` and `TaskUpdateDto` |
| Foreign keys | `AppDbContext` |
| Lookup seed | `AppDbContext.OnModelCreating` |
| DTOs | `DTOs` folder |
| Folder structure | Controllers / DTOs / Data / Models / Services / Settings / Extensions |

## GitHub submission

Create a **public** GitHub repository, then run these commands from the project root:

```bash
git init
git add .
git commit -m "Initial Task Management API implementation"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/task-management-api.git
git push -u origin main
```

Replace `YOUR_USERNAME` with your GitHub username and create the repository on GitHub before running `git push`.

Do not commit real passwords, production JWT secrets, API keys, or database credentials.


## Angular Frontend

The repository now includes a simple Angular frontend under `task-management-ui/`. It provides two user-facing pages: Login/Sign Up and My Tasks. The frontend consumes the existing ASP.NET Core API, stores the JWT after login, attaches it to authenticated requests, and provides task search, status filtering, add, edit and delete operations.

### Run the frontend

```bash
cd task-management-ui
npm install
npm start
```

Open `http://localhost:4200`. Make sure the API is running at `http://localhost:5180`.
