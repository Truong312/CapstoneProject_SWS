# User Management API Documentation

## Base URL
```
/api/user
```

## Authentication
All endpoints require JWT Bearer token authentication.

## Authorization Roles
- **Role 1**: Admin (Full access)
- **Role 2**: Manager (Read access)
- **Role 3**: Warehouse Staff
- **Role 4**: Provider/Supplier

---

## Endpoints

### 1. Get Paginated Users
Get a paginated list of users with filtering and sorting options.

**Endpoint:** `GET /api/user`

**Authorization:** Admin (Role 1) or Manager (Role 2)

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageIndex | int | No | 1 | Page number (1-based) |
| pageSize | int | No | 10 | Number of items per page |
| search | string | No | null | Search keyword for email, fullname |
| roleFilter | int | No | null | Filter by role (1=Admin, 2=Manager, 3=Staff, 4=Provider) |
| sortBy | string | No | null | Field name to sort by |
| sortDesc | bool | No | false | Sort descending if true |

**Request Example:**
```http
GET /api/user?pageIndex=1&pageSize=10&roleFilter=2&search=john
Authorization: Bearer {your_token}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "responseCode": null,
  "message": "Users retrieved successfully",
  "data": {
    "total": 50,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "items": [
      {
        "userId": 1,
        "email": "john.doe@example.com",
        "fullName": "John Doe",
        "phone": "0123456789",
        "address": "123 Main St, City",
        "role": 2,
        "roleName": "Manager"
      },
      {
        "userId": 2,
        "email": "jane.smith@example.com",
        "fullName": "Jane Smith",
        "phone": "0987654321",
        "address": "456 Oak Ave, City",
        "role": 2,
        "roleName": "Manager"
      }
    ]
  }
}
```

**Response Error (400 Bad Request):**
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "responseCode": "ERROR",
  "message": "Error retrieving users: {error_message}",
  "data": null
}
```

---

### 2. Get User By ID
Retrieve detailed information about a specific user.

**Endpoint:** `GET /api/user/{id}`

**Authorization:** Admin (Role 1) or Manager (Role 2)

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | int | Yes | User ID |

**Request Example:**
```http
GET /api/user/5
Authorization: Bearer {your_token}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "responseCode": null,
  "message": "User retrieved successfully",
  "data": {
    "userId": 5,
    "email": "user@example.com",
    "fullName": "User Name",
    "phone": "0123456789",
    "address": "123 Street, City",
    "role": 3,
    "roleName": "Warehouse Staff"
  }
}
```

**Response Error (404 Not Found):**
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "responseCode": "NOT_FOUND",
  "message": "User not found",
  "data": null
}
```

---

### 3. Create User
Create a new user account.

**Endpoint:** `POST /api/user`

**Authorization:** Admin (Role 1) only

**Request Body:**
```json
{
  "email": "newuser@example.com",
  "password": "SecurePass123",
  "fullName": "New User",
  "phone": "0123456789",
  "address": "123 New Street, City",
  "role": 3
}
```

**Request Body Schema:**

| Field | Type | Required | Validation | Description |
|-------|------|----------|------------|-------------|
| email | string | Yes | Valid email format | User's email address |
| password | string | Yes | Min 6 characters | User's password |
| fullName | string | Yes | Not empty | User's full name |
| phone | string | No | - | User's phone number |
| address | string | No | - | User's address |
| role | int | Yes | 1-4 | User's role (1=Admin, 2=Manager, 3=Staff, 4=Provider) |

**Request Example:**
```http
POST /api/user
Authorization: Bearer {your_token}
Content-Type: application/json

{
  "email": "newuser@example.com",
  "password": "SecurePass123",
  "fullName": "New User",
  "phone": "0123456789",
  "address": "123 New Street, City",
  "role": 3
}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 201,
  "responseCode": null,
  "message": "User created successfully",
  "data": {
    "userId": 10,
    "email": "newuser@example.com",
    "fullName": "New User",
    "phone": "0123456789",
    "address": "123 New Street, City",
    "role": 3,
    "roleName": "Warehouse Staff"
  }
}
```

**Response Error (409 Conflict):**
```json
{
  "isSuccess": false,
  "statusCode": 409,
  "responseCode": "CONFLICT",
  "message": "Email already exists",
  "data": null
}
```

**Response Error (400 Bad Request - Validation):**
```json
{
  "errors": {
    "Email": ["The Email field is not a valid e-mail address."],
    "Password": ["The field Password must be a string with a minimum length of 6."],
    "FullName": ["The FullName field is required."]
  }
}
```

---

### 4. Update User
Update an existing user's information.

**Endpoint:** `PUT /api/user/{id}`

**Authorization:** Admin (Role 1) only

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | int | Yes | User ID to update |

**Request Body:**
```json
{
  "fullName": "Updated User Name",
  "phone": "0987654321",
  "address": "456 Updated Street, City",
  "role": 2
}
```

**Request Body Schema:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| fullName | string | Yes | User's full name |
| phone | string | No | User's phone number |
| address | string | No | User's address |
| role | int | Yes | User's role (1-4) |

**Note:** Email and password cannot be updated through this endpoint.

**Request Example:**
```http
PUT /api/user/5
Authorization: Bearer {your_token}
Content-Type: application/json

{
  "fullName": "Updated User Name",
  "phone": "0987654321",
  "address": "456 Updated Street, City",
  "role": 2
}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "responseCode": null,
  "message": "User updated successfully",
  "data": {
    "userId": 5,
    "email": "user@example.com",
    "fullName": "Updated User Name",
    "phone": "0987654321",
    "address": "456 Updated Street, City",
    "role": 2,
    "roleName": "Manager"
  }
}
```

**Response Error (404 Not Found):**
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "responseCode": "NOT_FOUND",
  "message": "User not found",
  "data": null
}
```

---

### 5. Delete User
Delete a user account.

**Endpoint:** `DELETE /api/user/{id}`

**Authorization:** Admin (Role 1) only

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | int | Yes | User ID to delete |

**Request Example:**
```http
DELETE /api/user/5
Authorization: Bearer {your_token}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "responseCode": null,
  "message": "User deleted successfully",
  "data": null
}
```

**Response Error (404 Not Found):**
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "responseCode": "NOT_FOUND",
  "message": "User not found",
  "data": null
}
```

---

### 6. Get Providers
Get a paginated list of providers/suppliers (users with Role = 4).

**Endpoint:** `GET /api/user/providers`

**Authorization:** All authenticated users

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageIndex | int | No | 1 | Page number (1-based) |
| pageSize | int | No | 10 | Number of items per page |

**Request Example:**
```http
GET /api/user/providers?pageIndex=1&pageSize=20
Authorization: Bearer {your_token}
```

**Response Success (200 OK):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "responseCode": null,
  "message": "Users retrieved successfully",
  "data": {
    "total": 15,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1,
    "items": [
      {
        "userId": 20,
        "email": "provider1@example.com",
        "fullName": "Provider Company 1",
        "phone": "0111222333",
        "address": "100 Business St, City",
        "role": 4,
        "roleName": "Provider/Supplier"
      },
      {
        "userId": 21,
        "email": "provider2@example.com",
        "fullName": "Provider Company 2",
        "phone": "0444555666",
        "address": "200 Commerce Ave, City",
        "role": 4,
        "roleName": "Provider/Supplier"
      }
    ]
  }
}
```

---

## Common Error Responses

### 401 Unauthorized
Token is missing or invalid.
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
User does not have required role/permission.
```json
{
  "message": "Forbidden"
}
```

### 500 Internal Server Error
Server-side error occurred.
```json
{
  "isSuccess": false,
  "statusCode": 500,
  "responseCode": "ERROR",
  "message": "Error processing request: {error_details}",
  "data": null
}
```

---

## Data Models

### UserDto
```typescript
{
  userId: number;
  email: string;
  fullName: string;
  phone: string | null;
  address: string | null;
  role: number;  // 1=Admin, 2=Manager, 3=Staff, 4=Provider
  roleName: string;
}
```

### CreateUserRequest
```typescript
{
  email: string;          // Required, valid email
  password: string;       // Required, min 6 chars
  fullName: string;       // Required
  phone?: string;         // Optional
  address?: string;       // Optional
  role: number;          // Required, 1-4
}
```

### UpdateUserRequest
```typescript
{
  fullName: string;       // Required
  phone?: string;         // Optional
  address?: string;       // Optional
  role: number;          // Required, 1-4
}
```

### UserPagedRequestDto
```typescript
{
  pageIndex?: number;     // Default: 1
  pageSize?: number;      // Default: 10
  search?: string;        // Optional
  roleFilter?: number;    // Optional, 1-4
  sortBy?: string;        // Optional
  sortDesc?: boolean;     // Default: false
}
```

---

## Role Reference

| Role ID | Role Name | Description |
|---------|-----------|-------------|
| 1 | Admin | Full system access |
| 2 | Manager | Manage warehouse operations |
| 3 | Warehouse Staff | Handle warehouse tasks |
| 4 | Provider/Supplier | External suppliers |

---

## Testing with cURL

### Get all users
```bash
curl -X GET "http://localhost:8080/api/user?pageIndex=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Get user by ID
```bash
curl -X GET "http://localhost:8080/api/user/5" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Create user
```bash
curl -X POST "http://localhost:8080/api/user" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePass123",
    "fullName": "New User",
    "phone": "0123456789",
    "address": "123 Street",
    "role": 3
  }'
```

### Update user
```bash
curl -X PUT "http://localhost:8080/api/user/5" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Updated Name",
    "phone": "0987654321",
    "address": "456 Street",
    "role": 2
  }'
```

### Delete user
```bash
curl -X DELETE "http://localhost:8080/api/user/5" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Get providers
```bash
curl -X GET "http://localhost:8080/api/user/providers?pageIndex=1&pageSize=20" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Notes

1. **Authentication**: All endpoints require a valid JWT token in the Authorization header.
2. **Password Security**: Passwords are automatically hashed using BCrypt before storage.
3. **Email Uniqueness**: Email addresses must be unique across all users.
4. **Role Validation**: Role values must be between 1 and 4.
5. **Pagination**: Page index starts from 1 (not 0).
6. **Soft Delete**: Consider implementing soft delete if you need to maintain user history.

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-01-18 | Initial API documentation |

