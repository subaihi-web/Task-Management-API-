# Task Management Angular Frontend

A simple Angular frontend for the Task Management ASP.NET Core Web API.

## Pages

1. **Login / Sign Up** - authenticate users and create new accounts.
2. **My Tasks** - view, search, filter, add, edit and delete tasks for the logged-in user.

## Backend URL

The frontend is configured to use:

`http://localhost:5180/api`

If the backend runs on another port, update `src/app/services/api.config.ts`.

## Requirements

- Node.js 18.19+ or a compatible modern Node.js version
- npm
- Angular CLI (installed locally through the project dependencies)
- The ASP.NET Core API running on `http://localhost:5180`

## Run

```bash
cd task-management-ui
npm install
npm start
```

Open `http://localhost:4200`.

## Authentication

After login, the JWT is stored in `localStorage`. An Angular HTTP interceptor adds the token as:

`Authorization: Bearer <token>`

for API requests. The route guard prevents access to the task page without a valid token.

## API operations used

- `POST /api/auth/Signup`
- `POST /api/auth/Login`
- `GET /api/tasks/GetByCriteria`
- `GET /api/tasks/GetById/{id}`
- `POST /api/tasks/Add`
- `PUT /api/tasks/Update`
- `DELETE /api/tasks/Delete/{id}`
