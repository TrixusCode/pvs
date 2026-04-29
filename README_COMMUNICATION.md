# PVS Project - Frontend-Backend Communication Setup

## 📋 Overview

This project demonstrates **complete frontend-backend communication** with:
- ✅ **React + Vite** frontend with TypeScript-ready setup
- ✅ **.NET 10** Web API with JWT authentication
- ✅ **Axios** HTTP client with interceptors
- ✅ **React Router** for navigation
- ✅ **CORS** properly configured
- ✅ **JWT token management** with localStorage
- ✅ Sample components and API endpoints

## 🚀 Quick Start

### System Requirements
- Node.js 16+ 
- .NET 10 SDK
- Git

### Backend Start
```bash
cd backend/PVS.Api
dotnet build
dotnet run
# Runs on http://localhost:5000
# API docs: http://localhost:5000/openapi/v1.json
```

### Frontend Start
```bash
cd pvs-frontend
npm install  # (already done)
npm run dev
# Runs on http://localhost:5173
```

### Test It
1. Go to http://localhost:5173
2. Login with any email/password
3. See properties list
4. Try deleting, paginating

## 📁 Project Structure

```
pvs/
├── backend/
│   └── PVS.Api/
│       ├── Modules/
│       │   ├── Auth/
│       │   │   └── AuthController.cs          ← Handle login/register
│       │   ├── Properties/
│       │   │   └── PropertiesController.cs    ← Handle CRUD
│       │   ├── Clients/
│       │   ├── Appointments/
│       │   └── Offers/
│       ├── Models/
│       │   ├── User.cs
│       │   ├── Property.cs
│       │   ├── Client.cs
│       │   ├── Appointment.cs
│       │   └── Offer.cs
│       ├── Common/
│       │   ├── ApiResponse.cs                 ← Response wrapper
│       │   ├── JwtSettings.cs
│       │   └── Roles.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Data/
│       │   ├── AppDbContext.cs                ← (Empty, ready to configure)
│       │   └── DbSeeder.cs                    ← (Empty, ready to seed data)
│       ├── Program.cs                         ← ✅ Configured with CORS & JWT
│       └── appsettings.json
│
├── pvs-frontend/
│   ├── src/
│   │   ├── api/
│   │   │   └── Client.js                      ← ✅ Axios client with interceptors
│   │   ├── modules/
│   │   │   ├── auth/
│   │   │   │   ├── Login.jsx                  ← ✅ Login page
│   │   │   │   └── Login.css
│   │   │   ├── properties/
│   │   │   │   ├── Properties.jsx             ← ✅ Properties list
│   │   │   │   └── Properties.css
│   │   │   └── shared/
│   │   ├── App.jsx                            ← ✅ Routing configured
│   │   ├── main.jsx
│   │   └── index.css
│   ├── .env.local                             ← ✅ API URL
│   ├── .env.example                           
│   ├── package.json                           ← ✅ Dependencies installed
│   └── vite.config.js
│
├── FRONTEND_BACKEND_COMMUNICATION.md          ← Detailed guide
├── COMMUNICATION_FLOW.md                      ← Visual flows
├── QUICK_START.md                             ← Quick reference
└── README.md                                  ← This file
```

## 🔌 How Communication Works

### 1. Simple Flow
```
User clicks Login
    ↓
Frontend calls authAPI.login(email, password)
    ↓
Axios sends POST to http://localhost:5000/api/auth/login
    ↓
Backend AuthController validates & returns JWT token
    ↓
Frontend stores token in localStorage
    ↓
Axios automatically adds token to all future requests
    ↓
Protected endpoints return data
```

### 2. API Layers

**Frontend (React Component)**
```jsx
const handleLogin = async () => {
  const response = await authAPI.login(email, password);
  localStorage.setItem('authToken', response.data.token);
  navigate('/properties');
};
```

**HTTP Layer (Axios Client)**
```javascript
// Automatic token injection
apiClient.interceptors.request.use(config => {
  config.headers.Authorization = 
    `Bearer ${localStorage.getItem('authToken')}`;
  return config;
});
```

**Network (HTTP Request)**
```
POST /api/auth/login
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Backend (.NET Controller)**
```csharp
[HttpPost("login")]
[AllowAnonymous]
public IActionResult Login(LoginRequest request)
{
    var user = /* validate credentials */;
    var token = GenerateJwtToken(user);
    return Ok(new { token });
}
```

## 📚 Available Endpoints

### Authentication
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/auth/login` | ❌ | Login with email/password |
| POST | `/api/auth/register` | ❌ | Register new account |
| GET | `/api/auth/me` | ✅ | Get current user |

### Properties
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/properties` | ✅ | Get all properties (paginated) |
| GET | `/api/properties/{id}` | ✅ | Get property by ID |
| POST | `/api/properties` | ✅ | Create new property |
| PUT | `/api/properties/{id}` | ✅ | Update property |
| DELETE | `/api/properties/{id}` | ✅ | Delete property |
| GET | `/api/properties/search?q=...` | ✅ | Search properties |

### Clients, Appointments, Offers
Similar CRUD endpoints (TODO: implement controllers)

## 🔐 Authentication Flow

### Token Generation
```csharp
// Backend generates JWT with claims
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Role, user.Role)
    }),
    Expires = DateTime.UtcNow.AddHours(24),
    SigningCredentials = new SigningCredentials(key, ...)
};
```

### Token Storage
```javascript
// Frontend stores in localStorage (dev only, use HttpOnly in production)
localStorage.setItem('authToken', token);
```

### Token Usage
```javascript
// Automatically included in all requests
Authorization: Bearer <token>
```

### Token Validation
```csharp
// Backend validates on protected endpoints
[Authorize]
public IActionResult GetProperties() { ... }
```

### Token Expiration
```javascript
// Automatic logout on 401 response
apiClient.interceptors.response.use(
    response => response,
    error => {
        if (error.response?.status === 401) {
            localStorage.removeItem('authToken');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);
```

## 📝 API Client Usage

### Import
```javascript
import { authAPI, propertiesAPI, clientsAPI } from '../../api/Client';
```

### Authentication
```javascript
// Login
const response = await authAPI.login('user@example.com', 'pass');
const token = response.data.token;

// Register
await authAPI.register('user@example.com', 'pass', 'John', 'Doe');

// Logout
await authAPI.logout();
```

### Properties
```javascript
// Get all (paginated)
const response = await propertiesAPI.getAll(page = 1, pageSize = 10);

// Get single
const property = await propertiesAPI.getById(1);

// Create
await propertiesAPI.create({
  title: 'House',
  price: 450000,
  bedrooms: 3,
  ...
});

// Update
await propertiesAPI.update(1, { title: 'Updated' });

// Delete
await propertiesAPI.delete(1);

// Search
const results = await propertiesAPI.search('downtown');
```

## 🛠 Development Guide

### Add New Endpoint

#### Backend
1. Create/edit controller in `Modules/*/`
2. Add HTTP method with `[HttpGet/Post/Put/Delete]`
3. Include `[Authorize]` if protected

```csharp
[HttpGet("search")]
[Authorize]
public IActionResult Search(string q)
{
    return Ok(data);
}
```

#### Frontend
1. Add method to `src/api/Client.js`

```javascript
export const propertiesAPI = {
    search: (q) => apiClient.get('/properties/search', { params: { q } })
};
```

2. Use in component

```jsx
const results = await propertiesAPI.search('downtown');
```

### Handle Errors
```javascript
try {
    const data = await propertiesAPI.getAll();
} catch (error) {
    // 401: Auto-logout + redirect
    // 400: Validation errors in error.response.data
    // 500: Server error
    console.error(error.response?.data?.message);
}
```

### Add Protected Page
```jsx
<Route path="/properties" element={<Properties />} />

// Component can check auth:
// if (!localStorage.getItem('authToken')) navigate('/login');
```

## 🔒 Security

✅ **Implemented:**
- JWT token-based auth
- Token automatically included in requests
- Automatic logout on 401
- CORS policy
- Token expiration (24 hours)

⚠️ **Production Improvements Needed:**
- Password hashing (BCrypt ready in dependencies)
- HTTPS only
- HttpOnly cookies instead of localStorage
- Refresh token rotation
- Rate limiting
- Input validation
- SQL injection prevention

## 🧪 Testing

### Manual Testing
```bash
# Backend health check
curl http://localhost:5000/api/health

# Login in postman/curl
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "user@example.com", 
  "password": "password123"
}

# Get properties (copy token from login response)
GET http://localhost:5000/api/properties
Authorization: Bearer <TOKEN>
```

### Frontend Testing
1. Open http://localhost:5173
2. Open DevTools → Network tab
3. Login and observe requests
4. Check Local Storage for authToken
5. Delete properties and see real-time updates

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "message": "Login successful",
  "data": { "token": "eyJ..." }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Invalid credentials",
  "data": null
}
```

### Paginated Response
```json
{
  "data": [...],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 24,
  "totalPages": 3
}
```

## 🐛 Troubleshooting

### "401 Unauthorized" on API calls
- ✓ Check token in localStorage (DevTools → Application)
- ✓ Verify token format in Authorization header
- ✓ Check token hasn't expired (24 hours)

### CORS errors
- ✓ Backend includes frontend URL in CORS policy
- ✓ Check request origin matches policy
- ✓ Ensure credentials are allowed

### Login not working
- ✓ Backend running on http://localhost:5000?
- ✓ Frontend .env.local points to correct API URL?
- ✓ Check backend console for errors

### Properties page blank
- ✓ Login successful?
- ✓ Token stored in localStorage?
- ✓ Check browser console for API errors
- ✓ Check Network tab for failed requests

## 📚 Documentation Files

- **[FRONTEND_BACKEND_COMMUNICATION.md](FRONTEND_BACKEND_COMMUNICATION.md)** - Comprehensive guide
- **[COMMUNICATION_FLOW.md](COMMUNICATION_FLOW.md)** - Visual diagrams
- **[QUICK_START.md](QUICK_START.md)** - Quick reference

## 🔗 Architecture Diagram

```
User Input
    ↓
Component (Login.jsx, Properties.jsx)
    ↓
API Client (src/api/Client.js)
    ├─ Request Interceptor (adds token)
    ├─ Response Interceptor (handles errors)
    └─ Axios instance (sends HTTP)
    ↓
HTTP Request (POST/GET/PUT/DELETE)
    ↓
CORS Middleware (validates origin)
    ↓
Backend Router (maps to controller)
    ↓
JWT Middleware (validates token)
    ↓
Controller (AuthController, PropertiesController)
    ├─ Validate input
    ├─ Business logic
    └─ Database access (TODO)
    ↓
HTTP Response (JSON)
    ↓
Response Interceptor (checks status)
    ↓
Component (updates state)
    ↓
User sees updated UI
```

## 🚢 Deployment Checklist

- [ ] Update `appsettings.json` with production JWT secret
- [ ] Set `VITE_API_URL` to production API URL
- [ ] Run `npm run build` to build frontend
- [ ] Deploy backend to Azure/AWS/Heroku
- [ ] Deploy frontend (Vercel/Netlify)
- [ ] Update CORS policy for production domains
- [ ] Enable HTTPS
- [ ] Set up environment variables
- [ ] Configure database connection
- [ ] Implement password hashing
- [ ] Set up logging/monitoring

## 📞 Support

For detailed information on any aspect:
1. Check the [FRONTEND_BACKEND_COMMUNICATION.md](FRONTEND_BACKEND_COMMUNICATION.md)
2. Review [COMMUNICATION_FLOW.md](COMMUNICATION_FLOW.md) for visual examples
3. Read [QUICK_START.md](QUICK_START.md) for quick answers

---

**Project Status:** ✅ Ready to use | 🚧 Database integration needed | 📋 Add more controllers

Last Updated: April 29, 2026
