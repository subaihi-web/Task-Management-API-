# API Test Examples

Base URL: `http://localhost:5180`

## 1. Signup

```http
POST /api/auth/Signup
Content-Type: application/json
```

```json
{
  "username": "user1",
  "email": "user1@example.com",
  "password": "Password123!"
}
```

Expected: `201 Created`

## 2. Login

```http
POST /api/auth/Login
Content-Type: application/json
```

```json
{
  "email": "user1@example.com",
  "password": "Password123!"
}
```

Expected: `200 OK` with a JWT token.

## 3. Add task

```http
POST /api/tasks/Add
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

```json
{
  "title": "Learn ASP.NET Core",
  "description": "Complete the assignment.",
  "fromDate": "2026-08-21T09:00:00",
  "toDate": "2026-08-25T18:00:00",
  "statusId": 2
}
```

Expected: `201 Created`

## 4. Get all tasks

```http
GET /api/tasks/GetByCriteria
Authorization: Bearer <TOKEN>
```

Expected: `200 OK`

## 5. Filter

```http
GET /api/tasks/GetByCriteria?Title=ASP.NET&StatusId=2
Authorization: Bearer <TOKEN>
```

## 6. Get by ID

```http
GET /api/tasks/GetById/1
Authorization: Bearer <TOKEN>
```

## 7. Update

```http
PUT /api/tasks/Update
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

```json
{
  "id": 1,
  "title": "Learn ASP.NET Core - Updated",
  "description": "Finish the assignment and documentation.",
  "fromDate": "2026-08-21T09:00:00",
  "toDate": "2026-08-26T18:00:00",
  "statusId": 3
}
```

## 8. Delete

```http
DELETE /api/tasks/Delete/1
Authorization: Bearer <TOKEN>
```

Expected: `204 No Content`
