# API Documentation - Discussion & Comments

## ?? Overview

Các API endpoints ?? qu?n lý Discussion (Th?o lu?n) cho m?i bài h?c.

## ?? Authentication

T?t c? endpoints c?n JWT token trong header:
```
Authorization: Bearer {token}
```

Exception: `GET` endpoints cho phép truy c?p anonymous.

---

## ?? API Endpoints

### Base URL
```
api/lessons/{lessonId}/discussions
```

---

## 1?? GET - L?y t?t c? discussions c?a m?t lesson

### Request
```http
GET /api/lessons/{lessonId}/discussions
```

### Parameters
- `lessonId` (path, required): Lesson ID (UUID)

### Response - Success (200)
```json
{
  "success": true,
  "status_code": 200,
  "message": "Success",
  "data": [
    {
      "discussionId": "550e8400-e29b-41d4-a716-446655440000",
      "lessonId": "660e8400-e29b-41d4-a716-446655440001",
      "creatorId": "770e8400-e29b-41d4-a716-446655440002",
      "creatorName": "John Doe",
      "title": "How to use async/await?",
      "content": "I'm struggling with async/await in C#...",
      "isPinned": false,
      "isResolved": false,
      "viewCount": 25,
      "commentCount": 3,
      "createdAt": "2025-04-01T10:30:00Z",
      "updatedAt": "2025-04-01T10:30:00Z"
    }
  ]
}
```

### Response - Not Found (404)
```json
{
  "success": false,
  "status_code": 404,
  "message": "Lesson not found",
  "error_code": "NOT_FOUND"
}
```

---

## 2?? GET - L?y chi ti?t m?t discussion

### Request
```http
GET /api/lessons/{lessonId}/discussions/{discussionId}
```

### Parameters
- `lessonId` (path, required): Lesson ID
- `discussionId` (path, required): Discussion ID

### Response - Success (200)
```json
{
  "success": true,
  "status_code": 200,
  "message": "Success",
  "data": {
    "discussionId": "550e8400-e29b-41d4-a716-446655440000",
    "lessonId": "660e8400-e29b-41d4-a716-446655440001",
    "lessonTitle": "Understanding Async/Await",
    "creatorId": "770e8400-e29b-41d4-a716-446655440002",
    "creatorName": "John Doe",
    "creatorEmail": "john@example.com",
    "title": "How to use async/await?",
    "content": "I'm struggling with async/await in C#...",
    "isPinned": false,
    "isResolved": false,
    "viewCount": 26,
    "commentCount": 3,
    "createdAt": "2025-04-01T10:30:00Z",
    "updatedAt": "2025-04-01T10:30:00Z"
  }
}
```

**Note**: View count s? t? ??ng t?ng lên 1 khi g?i endpoint này.

---

## 3?? POST - T?o discussion m?i

### Request
```http
POST /api/lessons/{lessonId}/discussions
Authorization: Bearer {token}
Content-Type: application/json
```

### Parameters
- `lessonId` (path, required): Lesson ID

### Body
```json
{
  "title": "How to use async/await?",
  "content": "I'm struggling with async/await in C#. Can someone explain?"
}
```

### Response - Success (201)
```json
{
  "success": true,
  "status_code": 201,
  "message": "Resource created successfully",
  "data": {
    "discussionId": "550e8400-e29b-41d4-a716-446655440000",
    "lessonId": "660e8400-e29b-41d4-a716-446655440001",
    "creatorId": "770e8400-e29b-41d4-a716-446655440002",
    "creatorName": "John Doe",
    "title": "How to use async/await?",
    "content": "I'm struggling with async/await in C#. Can someone explain?",
    "isPinned": false,
    "isResolved": false,
    "viewCount": 0,
    "commentCount": 0,
    "createdAt": "2025-04-01T10:30:00Z",
    "updatedAt": "2025-04-01T10:30:00Z"
  }
}
```

### Response - Error (400)
```json
{
  "success": false,
  "status_code": 400,
  "message": "Bad request",
  "error_code": "BAD_REQUEST"
}
```

### Response - Unauthorized (401)
```json
{
  "success": false,
  "status_code": 401,
  "message": "User not authenticated",
  "error_code": "INVALID_USER"
}
```

### Response - Not Found (404)
```json
{
  "success": false,
  "status_code": 404,
  "message": "Lesson not found",
  "error_code": "NOT_FOUND"
}
```

---

## 4?? PUT - C?p nh?t discussion

### Request
```http
PUT /api/lessons/{lessonId}/discussions/{discussionId}
Authorization: Bearer {token}
Content-Type: application/json
```

### Parameters
- `lessonId` (path, required): Lesson ID
- `discussionId` (path, required): Discussion ID

### Body (T?t c? fields optional)
```json
{
  "title": "How to use async/await in C#?",
  "content": "Updated content...",
  "isPinned": true,
  "isResolved": false
}
```

### Response - Success (200)
```json
{
  "success": true,
  "status_code": 200,
  "message": "Success",
  "data": {
    "discussionId": "550e8400-e29b-41d4-a716-446655440000",
    "lessonId": "660e8400-e29b-41d4-a716-446655440001",
    "creatorId": "770e8400-e29b-41d4-a716-446655440002",
    "creatorName": "John Doe",
    "title": "How to use async/await in C#?",
    "content": "Updated content...",
    "isPinned": true,
    "isResolved": false,
    "viewCount": 26,
    "commentCount": 3,
    "createdAt": "2025-04-01T10:30:00Z",
    "updatedAt": "2025-04-01T11:00:00Z"
  }
}
```

### Response - Forbidden (403)
```json
{
  "success": false,
  "status_code": 403,
  "message": "You don't have permission to update this discussion",
  "error_code": "PERMISSION_DENIED"
}
```

**Note**: Ch? creator ho?c admin có th? update discussion.

---

## 5?? DELETE - Xóa discussion

### Request
```http
DELETE /api/lessons/{lessonId}/discussions/{discussionId}
Authorization: Bearer {token}
```

### Parameters
- `lessonId` (path, required): Lesson ID
- `discussionId` (path, required): Discussion ID

### Response - Success (200)
```json
{
  "success": true,
  "status_code": 200,
  "message": "Discussion deleted successfully"
}
```

### Response - Forbidden (403)
```json
{
  "success": false,
  "status_code": 403,
  "message": "You don't have permission to delete this discussion",
  "error_code": "PERMISSION_DENIED"
}
```

### Response - Not Found (404)
```json
{
  "success": false,
  "status_code": 404,
  "message": "Discussion not found",
  "error_code": "NOT_FOUND"
}
```

**Note**: Ch? creator ho?c admin có th? delete discussion.

---

## ?? Common Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## ?? Testing Examples

### Create Discussion
```bash
curl -X POST http://localhost:5000/api/lessons/660e8400-e29b-41d4-a716-446655440001/discussions \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "How to use async/await?",
    "content": "I need help with async/await"
  }'
```

### Get All Discussions
```bash
curl -X GET http://localhost:5000/api/lessons/660e8400-e29b-41d4-a716-446655440001/discussions
```

### Get Discussion Detail
```bash
curl -X GET http://localhost:5000/api/lessons/660e8400-e29b-41d4-a716-446655440001/discussions/550e8400-e29b-41d4-a716-446655440000
```

### Update Discussion
```bash
curl -X PUT http://localhost:5000/api/lessons/660e8400-e29b-41d4-a716-446655440001/discussions/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Updated title",
    "isPinned": true
  }'
```

### Delete Discussion
```bash
curl -X DELETE http://localhost:5000/api/lessons/660e8400-e29b-41d4-a716-446655440001/discussions/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer {token}"
```

---

## ?? Permission Rules

| Action | Creator | Admin | Others |
|--------|---------|-------|--------|
| View (GET) | ? | ? | ? |
| Create (POST) | N/A | ? | ?* |
| Update (PUT) | ? | ? | ? |
| Delete (DELETE) | ? | ? | ? |

*Only authenticated users can create discussions.

---

## ?? Notes

1. **View Count**: T? ??ng t?ng m?i khi g?i GET detail endpoint
2. **IsPinned**: Ch? admin ho?c creator có th? pin discussion
3. **IsResolved**: Ch? admin ho?c creator có th? mark discussion as resolved
4. **Comment Count**: T? ??ng tính t? s? comments liên quan
5. **Cascade Delete**: Khi xóa discussion, t?t c? comments s? b? xóa theo

---

## ?? Next Steps

- Comment API (Create, Update, Delete comments)
- CommentLike API (Like/Unlike comments)
- Pagination cho GET discussions
- Search & Filter discussions
