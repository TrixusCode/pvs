# Frontend-Backend Communication - Visual Flow

## Complete End-to-End Communication Flow

```
┌──────────────────────────────────────────────────────────────────────┐
│                         USER BROWSER                                 │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │           React Application (port 5173)                  │    │
│  │  ┌──────────────────────────────────────────────────┐   │    │
│  │  │  Login Page                                      │   │    │
│  │  │  ├─ Email input: user@example.com               │   │    │
│  │  │  └─ Password input: ••••••                       │   │    │
│  │  │  Click LOGIN ──────────────────────┐            │   │    │
│  │  └──────────────────────────────────────────────────┘   │    │
│  │                                        │                 │    │
│  │                                        ▼                 │    │
│  │  ┌──────────────────────────────────────────────────┐   │    │
│  │  │  src/api/Client.js (Axios Instance)              │   │    │
│  │  │                                                  │   │    │
│  │  │  authAPI.login(email, password)                 │   │    │
│  │  │    ↓ (Request Interceptor)                      │   │    │
│  │  │  POST /api/auth/login                           │   │    │
│  │  │  Headers: {                                      │   │    │
│  │  │    'Content-Type': 'application/json',           │   │    │
│  │  │    'Authorization': 'Bearer <token>' (if exists) │   │    │
│  │  │  }                                               │   │    │
│  │  └──────────────────────────────────────────────────┘   │    │
│  │                                        │                 │    │
│  └────────────────────────────────────────┼─────────────────┘    │
│                                           │                      │
│                                           ▼                      │
│                    ┌─────────────────────────────────┐           │
│                    │  CORS Policy Check (Axios)      │           │
│                    │  - Origin: localhost:5173 ✓    │           │
│                    │  - Method: POST ✓              │           │
│                    │  - Headers: allowed ✓          │           │
│                    └─────────────────────────────────┘           │
│                                                                  │
└──────────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP
                              │
                    ┌─────────▼──────────┐
                    │  Network (Internet) │
                    └─────────┬──────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────────┐
│                    .NET 10 WEB API (port 5000)                      │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  Request Pipeline:                                         │    │
│  │  1. CORS Middleware validates request origin              │    │
│  │  2. Routing: /api/auth/login → AuthController            │    │
│  │  3. Authentication Middleware (JWT validation)            │    │
│  │  4. Controller Action executes                            │    │
│  └────────────────────────────────────────────────────────────┘    │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  AuthController.Login(LoginRequest request)               │    │
│  │                                                            │    │
│  │  1. Validate model state                                  │    │
│  │  2. Check email/password (TODO: query database)           │    │
│  │  3. Generate JWT Token with claims:                       │    │
│  │     - User ID: 1                                           │    │
│  │     - Email: user@example.com                              │    │
│  │     - Name: Demo User                                      │    │
│  │     - Role: Admin                                          │    │
│  │     - Expiration: 24 hours                                 │    │
│  │                                                            │    │
│  │  return Ok(new {                                           │    │
│  │    success: true,                                          │    │
│  │    message: "Login successful",                            │    │
│  │    data: "eyJhbGc..." (JWT Token)                          │    │
│  │  })                                                        │    │
│  └────────────────────────────────────────────────────────────┘    │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  Response: 200 OK                                          │    │
│  │  Body: JSON with JWT token                                 │    │
│  └────────────────────────────────────────────────────────────┘    │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP Response
                              │
┌──────────────────────────────────────────────────────────────────────┐
│                         USER BROWSER                                 │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  Login Component (Login.jsx)                              │    │
│  │                                                            │    │
│  │  response.data = {                                         │    │
│  │    success: true,                                          │    │
│  │    message: "Login successful",                            │    │
│  │    data: "eyJhbGc..." (JWT Token)                          │    │
│  │  }                                                         │    │
│  │                                                            │    │
│  │  // Store token                                            │    │
│  │  localStorage.setItem('authToken', response.data.data)    │    │
│  │                                                            │    │
│  │  // Redirect to protected page                            │    │
│  │  navigate('/properties')                                   │    │
│  │                                                            │    │
│  │  ✓ Login successful!                                       │    │
│  └────────────────────────────────────────────────────────────┘    │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  Properties Page (Properties.jsx)                         │    │
│  │                                                            │    │
│  │  useEffect(() => {                                         │    │
│  │    fetchProperties();  // Called on page load              │    │
│  │  }, [page])                                                │    │
│  │                                                            │    │
│  │  const fetchProperties = async () => {                    │    │
│  │    const response = await propertiesAPI.getAll(page, 10); │    │
│  │    setProperties(response.data.data);                      │    │
│  │  }                                                         │    │
│  │                                                            │    │
│  │  Display: [Grid of 2 sample properties]                    │    │
│  │  ✓ Properties loaded!                                      │    │
│  └────────────────────────────────────────────────────────────┘    │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

## Second Request: Properties (with Authentication)

```
┌────────────────────────────────────────────────────────────────┐
│  React Component needs Properties                              │
└────────────────────────────────────────────────────────────────┘
                          │
                          ▼
         ┌─────────────────────────────────┐
         │ propertiesAPI.getAll(1, 10)    │
         │                                 │
         │ (from src/api/Client.js)        │
         └─────────────────────────────────┘
                          │
                          ▼
         ┌─────────────────────────────────┐
         │ Request Interceptor:             │
         │ 1. Get token from localStorage   │
         │ 2. Add to Authorization header   │
         │                                 │
         │ Authorization: Bearer            │
         │   eyJhbGciOiJIUzI1NiIsInR5cCI... │
         └─────────────────────────────────┘
                          │
                          ▼
        ┌──────────────────────────────────┐
        │ HTTP Request                     │
        │ GET /api/properties?             │
        │     page=1&pageSize=10           │
        │ Headers: Authorization: Bearer.. │
        └──────────────────────────────────┘
                          │
      ════════════════════════════════════════════
                          │
                          ▼
        ┌──────────────────────────────────┐
        │ .NET Backend                     │
        │ 1. JWT Middleware validates token│
        │ 2. Extracts user claims          │
        │ 3. Routes to                     │
        │    PropertiesController.GetAll() │
        │ 4. Returns 10 properties per page│
        └──────────────────────────────────┘
                          │
                          ▼
        ┌──────────────────────────────────┐
        │ HTTP Response (200 OK)           │
        │ {                                │
        │   "data": [...],                 │
        │   "currentPage": 1,              │
        │   "pageSize": 10,                │
        │   "totalCount": 24,              │
        │   "totalPages": 3                │
        │ }                                │
        └──────────────────────────────────┘
                          │
      ════════════════════════════════════════════
                          │
                          ▼
         ┌─────────────────────────────────┐
         │ Response Interceptor             │
         │ (checks for errors)              │
         └─────────────────────────────────┘
                          │
                          ▼
         ┌─────────────────────────────────┐
         │ React Component receives data    │
         │ setProperties(data.data)         │
         │ Renders cards with properties    │
         │                                 │
         │ Display:                        │
         │ ┌─────────────────────────────┐ │
         │ │ Beautiful House             │ │
         │ │ 123 Main St                 │ │
         │ │ $450,000 - 3 bedrooms       │ │
         │ └─────────────────────────────┘ │
         │ ┌─────────────────────────────┐ │
         │ │ Cozy Apartment              │ │
         │ │ 456 Oak Ave                 │ │
         │ │ $250,000 - 1 bedroom        │ │
         │ └─────────────────────────────┘ │
         │                                 │
         │ ✓ Properties displayed!          │
         └─────────────────────────────────┘
```

## Key Components Working Together

```
┌─────────────────────────────────────────────────────────┐
│  API Client (src/api/Client.js)                         │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  // Axios instance with defaults                       │
│  const apiClient = axios.create({                       │
│    baseURL: 'http://localhost:5000/api',                │
│    headers: { 'Content-Type': 'application/json' }      │
│  });                                                    │
│                                                         │
│  // Auto-inject token                                   │
│  apiClient.interceptors.request.use(config => {        │
│    config.headers.Authorization =                       │
│      `Bearer ${localStorage.getItem('authToken')}`;     │
│    return config;                                        │
│  });                                                    │
│                                                         │
│  // Handle 401 errors                                   │
│  apiClient.interceptors.response.use(                   │
│    response => response,                                │
│    error => {                                           │
│      if (error.response?.status === 401) {             │
│        localStorage.removeItem('authToken');            │
│        window.location.href = '/login';                 │
│      }                                                  │
│      return Promise.reject(error);                      │
│    }                                                    │
│  );                                                     │
│                                                         │
│  // Exported modules                                    │
│  export const authAPI = { login, register, logout }    │
│  export const propertiesAPI = { getAll, getById, ... } │
│  export const clientsAPI = { ... }                      │
│  export const appointmentsAPI = { ... }                 │
│  export const offersAPI = { ... }                       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## Data Models

```
User Model (Backend)
├─ Id: int
├─ Email: string
├─ FirstName: string
├─ LastName: string
├─ PasswordHash: string
├─ Role: string (User/Admin)
├─ CreatedAt: DateTime
└─ UpdatedAt: DateTime?

Property Model (Backend)
├─ Id: int
├─ Title: string
├─ Description: string
├─ Address: string
├─ City: string
├─ State: string
├─ ZipCode: string
├─ Price: decimal
├─ Bedrooms: int
├─ Bathrooms: int
├─ SquareFeet: decimal
├─ UserId: int (foreign key)
├─ CreatedAt: DateTime
└─ UpdatedAt: DateTime?

API Response Format
├─ Success: bool
├─ Message: string
└─ Data: object (or list for paginated)

Paginated Response
├─ Data: array
├─ CurrentPage: int
├─ PageSize: int
├─ TotalCount: int
└─ TotalPages: int (calculated)
```

## Request/Response Examples

### POST Login Request
```
→ Browser sends:
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

← Backend responds:
200 OK
Content-Type: application/json

{
  "success": true,
  "message": "Login successful",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cC..."
}

✓ Frontend stores token:
localStorage.setItem('authToken', <token>)
```

### GET Properties Request (with Auth)
```
→ Browser sends:
GET /api/properties?page=1&pageSize=10
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

← Backend responds:
200 OK
Content-Type: application/json

{
  "data": [
    {
      "id": 1,
      "title": "Beautiful House in Downtown",
      "address": "123 Main St",
      "price": 450000,
      "bedrooms": 3,
      "bathrooms": 2,
      "squareFeet": 2500,
      "createdAt": "2024-03-30T10:00:00"
    },
    {
      "id": 2,
      "title": "Cozy Apartment",
      "address": "456 Oak Ave",
      "price": 250000,
      "bedrooms": 1,
      "bathrooms": 1,
      "squareFeet": 800,
      "createdAt": "2024-03-31T10:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}

✓ Frontend displays data:
properties.map(p => <PropertyCard {...p} />)
```

### DELETE Property Request (with Auth)
```
→ Browser sends:
DELETE /api/properties/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

← Backend responds:
200 OK
Content-Type: application/json

{
  "success": true,
  "message": "Property deleted successfully",
  "data": null
}

✓ Frontend updates UI:
setProperties(properties.filter(p => p.id !== 1))
```

---

This document shows how every piece fits together to create a seamless frontend-backend communication system!
