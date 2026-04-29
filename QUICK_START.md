# Quick Start Guide

## Prerequisites
- Node.js 16+ (for frontend)
- .NET 10 SDK (for backend)

## Frontend Setup

```bash
cd pvs-frontend

# Install dependencies
npm install

# Configure environment (optional, defaults to localhost:5000/api)
echo "VITE_API_URL=http://localhost:5000/api" > .env.local

# Start development server
npm run dev
# Frontend: http://localhost:5173
```

## Backend Setup

```bash
cd backend/PVS.Api

# Build the project
dotnet build

# Run the API
dotnet run
# Backend API: http://localhost:5000
# Swagger/OpenAPI: http://localhost:5000/openapi/v1.json
```

## Test the Communication

### 1. Open Frontend
- Navigate to http://localhost:5173
- You'll be redirected to /login

### 2. Login
- Email: `user@example.com`
- Password: `password123`
- Click Login

### 3. View Properties
- After login, you'll see a list of sample properties
- Try:
  - View property details
  - Delete a property
  - Navigate pages

### 4. API Testing (Optional)

Using curl or Postman:

**Get Health Status:**
```bash
curl http://localhost:5000/api/health
```

**Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'
```

**Get Properties (with token):**
```bash
curl http://localhost:5000/api/properties \
  -H "Authorization: Bearer <YOUR_TOKEN_HERE>"
```

## Project Structure

```
pvs/
├── backend/
│   └── PVS.Api/
│       ├── Modules/
│       │   ├── Auth/          (Login/Register)
│       │   ├── Properties/    (CRUD operations)
│       │   ├── Clients/
│       │   ├── Appointments/
│       │   └── Offers/
│       ├── Models/            (Data models)
│       ├── Common/            (Shared utilities)
│       ├── Program.cs         (Startup configuration)
│       └── appsettings.json   (Configuration)
│
└── pvs-frontend/
    ├── src/
    │   ├── api/
    │   │   └── Client.js      (Axios HTTP client)
    │   ├── modules/
    │   │   ├── auth/          (Login component)
    │   │   ├── properties/    (Properties components)
    │   │   └── shared/
    │   ├── App.jsx            (Main router)
    │   └── main.jsx           (Entry point)
    ├── .env.local             (Environment variables)
    └── package.json           (Dependencies)
```

## Common Tasks

### Add New API Endpoint

**Backend (e.g., Clients/ClientsController.cs):**
```csharp
[HttpGet]
public IActionResult GetAll([FromQuery] int page = 1)
{
    // Your logic
    return Ok(data);
}
```

**Frontend (add to src/api/Client.js):**
```javascript
export const clientsAPI = {
  getAll: (page = 1) => 
    apiClient.get('/clients', { params: { page } }),
};
```

**Use in Component:**
```jsx
import { clientsAPI } from '../../api/Client';

const response = await clientsAPI.getAll(1);
```

### Handle Authentication

The API client automatically:
- Adds JWT token to all requests
- Removes token on 401 response
- Redirects to login on auth failure

No manual token handling needed!

### Add Protected Route

```jsx
<Route path="/admin" element={<AdminPanel />} />
// Manually check token if needed:
// if (!localStorage.getItem('authToken')) navigate('/login');
```

## Debugging Tips

1. **Check Backend Connection:**
   ```bash
   curl http://localhost:5000/api/health
   ```

2. **Check Token Storage:**
   - Open DevTools → Application → Local Storage
   - Look for `authToken` key

3. **Monitor API Calls:**
   - Open DevTools → Network tab
   - Look for requests to `localhost:5000/api/`

4. **Check CORS Issues:**
   - Look for red errors in browser console
   - Verify backend CORS policy includes frontend URL

5. **View API Response:**
   - DevTools → Network → Click request → Response tab

## Next Steps

1. **Connect to Database:**
   - Replace mock data in controllers with Entity Framework queries
   - Update DbContext with models

2. **Implement Authentication:**
   - Add password hashing (BCrypt.Net-Next)
   - Save users to database

3. **Add More Features:**
   - Create Clients, Appointments, Offers components
   - Follow the same pattern as Properties

4. **Improve Security:**
   - Add refresh token mechanism
   - Implement rate limiting
   - Add input validation

5. **Production Deployment:**
   - Use HTTPS
   - Set proper JWT secret
   - Configure CORS for production domain
   - Build frontend: `npm run build`
