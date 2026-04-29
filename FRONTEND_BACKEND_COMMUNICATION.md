# Frontend-Backend Communication Guide for PVS Project

This guide explains how the frontend and backend communicate, with complete examples.

## Architecture Overview

The PVS project uses:
- **Frontend**: React + Vite + Axios
- **Backend**: .NET 10 Web API with JWT Authentication
- **Communication**: HTTP/REST with Bearer token authentication

```
┌─────────────────────────────────────────────────────────────┐
│                    React Frontend                           │
│              (http://localhost:5173)                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Components (Login, Properties)                       │  │
│  │         ↓                           ↓                 │  │
│  │    Uses authAPI                  Uses propertiesAPI  │  │
│  │    (Client.js)                   (Client.js)         │  │
│  └──────────────────────────────────────────────────────┘  │
│              ↓ (HTTP Requests via Axios)                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
             ┌──────────────────────────────┐
             │    CORS Policy Validates     │
             │    Request Headers           │
             └──────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              .NET 10 Web API Backend                         │
│           (http://localhost:5000/api)                       │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Controllers (Auth, Properties, Clients, etc)        │  │
│  │      ↓              ↓              ↓                 │  │
│  │  AuthController  PropertiesController  ...           │  │
│  │      ↓              ↓              ↓                 │  │
│  │   Login         Get/Create/        ...               │  │
│  │   Register      Update/Delete                        │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Frontend: API Client (`src/api/Client.js`)

The API Client is a centralized module that handles all HTTP communication.

### Key Features:

1. **Axios Instance Configuration**
   - Base URL: `http://localhost:5000/api`
   - Automatic JWT token injection
   - Error handling with automatic logout on 401

2. **Request Interceptor**: Automatically adds JWT token to authorization header
   ```javascript
   apiClient.interceptors.request.use((config) => {
     const token = localStorage.getItem('authToken');
     if (token) {
       config.headers.Authorization = `Bearer ${token}`;
     }
     return config;
   });
   ```

3. **Response Interceptor**: Handles errors and clears auth if needed
   ```javascript
   apiClient.interceptors.response.use(
     (response) => response,
     (error) => {
       if (error.response?.status === 401) {
         localStorage.removeItem('authToken');
         window.location.href = '/login';
       }
       return Promise.reject(error);
     }
   );
   ```

### Available Modules:

#### **authAPI** - Authentication
```javascript
// Login
const response = await authAPI.login('user@example.com', 'password');
localStorage.setItem('authToken', response.data.token);

// Register
await authAPI.register('user@example.com', 'password', 'John', 'Doe');

// Logout
await authAPI.logout(); // Clears token and redirects
```

#### **propertiesAPI** - Property Management
```javascript
// Get all properties (with pagination)
const response = await propertiesAPI.getAll(page = 1, pageSize = 10);
// Response: { data: [...], totalPages: X, currentPage: Y }

// Get single property
const property = await propertiesAPI.getById(1);

// Create property
await propertiesAPI.create({
  title: 'Beautiful House',
  description: 'Large 3-bedroom house',
  address: '123 Main St',
  city: 'New York',
  state: 'NY',
  zipCode: '10001',
  price: 450000,
  bedrooms: 3,
  bathrooms: 2,
  squareFeet: 2500
});

// Update property
await propertiesAPI.update(1, {
  title: 'Updated Title',
  price: 475000
});

// Delete property
await propertiesAPI.delete(1);

// Search properties
const results = await propertiesAPI.search('downtown');
```

#### **clientsAPI, appointmentsAPI, offersAPI**
Similar CRUD operations available for all resources.

## Frontend: Components

### Login Component (`src/modules/auth/Login.jsx`)

```jsx
import { authAPI } from '../../api/Client';

const handleSubmit = async (e) => {
  e.preventDefault();
  try {
    // 1. Send login request to backend
    const response = await authAPI.login(email, password);
    
    // 2. Store JWT token locally
    localStorage.setItem('authToken', response.data.token);
    
    // 3. Redirect to protected page
    navigate('/properties');
  } catch (err) {
    // Show error message
    setError(err.response?.data?.message || 'Login failed');
  }
};
```

**Flow:**
1. User enters email and password
2. Component calls `authAPI.login()`
3. Axios sends POST to `/api/auth/login`
4. Backend validates credentials and returns JWT token
5. Frontend stores token in localStorage
6. Future API calls include this token in the Authorization header

### Properties Component (`src/modules/properties/Properties.jsx`)

```jsx
import { propertiesAPI } from '../../api/Client';

const fetchProperties = async () => {
  try {
    // 1. API Client automatically adds Authorization header with JWT token
    const response = await propertiesAPI.getAll(page, 10);
    
    // 2. Extract paginated data
    setProperties(response.data.data);
    setTotalPages(response.data.totalPages);
  } catch (err) {
    // If 401, interceptor automatically clears token and redirects to login
    setError(err.response?.data?.message);
  }
};

const handleDelete = async (id) => {
  try {
    // Delete property
    await propertiesAPI.delete(id);
    // Update local state
    setProperties(properties.filter(p => p.id !== id));
  } catch (err) {
    setError('Failed to delete property');
  }
};
```

**Flow:**
1. Component loads and calls `fetchProperties()`
2. API Client injects JWT token: `Authorization: Bearer <token>`
3. Axios sends GET to `/api/properties?page=1&pageSize=10`
4. Backend validates token and returns paginated properties
5. Component updates UI with data

## Backend: API Controllers

### Auth Controller (`Modules/Auth/AuthController.cs`)

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validate email/password
        if (string.IsNullOrEmpty(request.Email)) 
            return Unauthorized();
        
        // TODO: Check database for user
        
        // Generate JWT token
        var token = GenerateJwtToken(user);
        
        return Ok(new { token });
    }
    
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // TODO: Hash password and save user
        // Generate token and return
    }
    
    private string GenerateJwtToken(User user)
    {
        // Creates JWT with claims:
        // - user ID
        // - email
        // - name
        // - role
        // Expires in 24 hours
    }
}
```

### Properties Controller (`Modules/Properties/PropertiesController.cs`)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires valid JWT token
public class PropertiesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Return paginated property list
        return Ok(new PaginatedResponse<Property>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] CreatePropertyRequest request)
    {
        // Create new property
        return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdatePropertyRequest request)
    {
        // Update property fields
        return Ok(property);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Delete property
        return Ok();
    }
}
```

## Complete Request-Response Examples

### Example 1: Login Flow

**Frontend Request:**
```javascript
const response = await authAPI.login('user@example.com', 'password123');
```

**HTTP Request sent by Axios:**
```
POST /api/auth/login HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Backend Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Frontend Code:**
```javascript
localStorage.setItem('authToken', response.data.data); // Store token
navigate('/properties'); // Redirect to protected page
```

### Example 2: Get Properties (with Authorization)

**Frontend Request:**
```javascript
const response = await propertiesAPI.getAll(1, 10);
```

**HTTP Request sent by Axios:**
```
GET /api/properties?page=1&pageSize=10 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Backend Processing:**
1. Extracts token from Authorization header
2. Validates JWT signature and expiration
3. Extracts user claims from token
4. Returns properties for that user

**Backend Response:**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Beautiful House in Downtown",
      "price": 450000,
      "bedrooms": 3,
      "address": "123 Main St",
      ...
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 24,
  "totalPages": 3
}
```

**Frontend Code:**
```javascript
setProperties(response.data.data);
setTotalPages(response.data.totalPages);
```

### Example 3: Create Property (POST with Authorization)

**Frontend Request:**
```javascript
await propertiesAPI.create({
  title: 'New Property',
  price: 350000,
  bedrooms: 2,
  address: '456 Oak Ave'
});
```

**HTTP Request:**
```
POST /api/properties HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "New Property",
  "price": 350000,
  "bedrooms": 2,
  "address": "456 Oak Ave",
  ...
}
```

**Backend Response (201 Created):**
```json
{
  "id": 25,
  "title": "New Property",
  "price": 350000,
  "bedrooms": 2,
  ...
}
```

## Running the Project

### Backend Setup:
```bash
cd backend/PVS.Api
dotnet build
dotnet run
# API runs on http://localhost:5000
```

### Frontend Setup:
```bash
cd pvs-frontend
npm install
npm run dev
# App runs on http://localhost:5173
```

### Configuration

**Backend** - Update JWT secret in `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-for-production"
  }
}
```

**Frontend** - Set API URL in `.env.local`:
```
VITE_API_URL=http://localhost:5000/api
```

## CORS Configuration

Backend allows requests from frontend:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## Error Handling

### Frontend:
All API errors are automatically handled:
- **401 (Unauthorized)**: Token cleared, user redirected to login
- **400/422 (Validation Errors)**: Error message displayed to user
- **500 (Server Error)**: Generic error message shown

### Backend:
All endpoints return consistent response format:
```json
{
  "success": true/false,
  "message": "descriptive message",
  "data": {}
}
```

## Security Best Practices

✅ **Implemented:**
- JWT token-based authentication
- Authorization header with Bearer token
- Automatic token injection via Axios interceptor
- CORS policy restricts to localhost
- Token expiration (24 hours)

🔄 **To Implement:**
- Password hashing (BCrypt.Net-Next already in dependencies)
- Refresh token mechanism
- HTTPS in production
- Secure cookie storage (HttpOnly)
- Rate limiting
- Input validation and sanitization

## Troubleshooting

**Issue: 401 Unauthorized on API calls**
- ✓ Check token is stored in localStorage
- ✓ Verify token format: `Authorization: Bearer <token>`
- ✓ Check token expiration (24 hours)

**Issue: CORS errors**
- ✓ Verify frontend URL in backend CORS policy
- ✓ Check Content-Type header is application/json

**Issue: Login not working**
- ✓ Check backend is running on http://localhost:5000
- ✓ Check frontend .env.local has correct VITE_API_URL

**Issue: Properties page shows nothing after login**
- ✓ Check token is persisted in localStorage
- ✓ Open browser DevTools → Application → Local Storage
- ✓ Verify authToken exists

---

This guide provides a complete overview of the frontend-backend communication architecture and implementation.
