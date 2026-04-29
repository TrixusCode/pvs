# Quick API Endpoints Reference

## Summary Table

### Authentication (No Auth Required for Login/Register)

| Method | Endpoint | Public | Purpose |
|--------|----------|--------|---------|
| POST | `/auth/login` | ✅ | Login with email/password |
| POST | `/auth/register` | ✅ | Register new user account |
| GET | `/auth/me` | ❌ | Get current user profile |
| POST | `/auth/refresh-token` | ❌ | Generate new JWT token |
| POST | `/auth/change-password` | ❌ | Update user password |
| GET | `/health` | ✅ | Health check |

### Properties Module (All Require Auth)

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/properties` | List all properties (paginated) |
| GET | `/properties/{id}` | Get single property |
| POST | `/properties` | Create new property |
| PUT | `/properties/{id}` | Update property |
| DELETE | `/properties/{id}` | Delete property |
| GET | `/properties/search` | Search properties by title/address |

### Clients Module (All Require Auth)

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/clients` | List all clients (paginated) |
| GET | `/clients/{id}` | Get single client |
| POST | `/clients` | Create new client |
| PUT | `/clients/{id}` | Update client |
| DELETE | `/clients/{id}` | Delete client |
| GET | `/clients/search` | Search clients by name/email |
| GET | `/clients/by-type/{type}` | Get clients by type (Buyer/Seller/Both) |

### Appointments Module (All Require Auth)

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/appointments` | List all appointments (paginated) |
| GET | `/appointments/{id}` | Get single appointment |
| POST | `/appointments` | Create new appointment |
| PUT | `/appointments/{id}` | Update appointment |
| DELETE | `/appointments/{id}` | Delete appointment |
| GET | `/appointments/property/{id}` | Get appointments for property |
| GET | `/appointments/client/{id}` | Get appointments for client |
| GET | `/appointments/by-status/{status}` | Get appointments by status |
| GET | `/appointments/by-date` | Get appointments in date range |

### Offers Module (All Require Auth)

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/offers` | List all offers (paginated) |
| GET | `/offers/{id}` | Get single offer |
| POST | `/offers` | Create new offer |
| PUT | `/offers/{id}` | Update offer |
| DELETE | `/offers/{id}` | Delete offer |
| POST | `/offers/{id}/accept` | Accept offer (changes status) |
| POST | `/offers/{id}/reject` | Reject offer (changes status) |
| POST | `/offers/{id}/withdraw` | Withdraw offer (changes status) |
| GET | `/offers/property/{id}` | Get offers for property |
| GET | `/offers/client/{id}` | Get offers from client |
| GET | `/offers/by-status/{status}` | Get offers by status |
| GET | `/offers/price-range` | Get offers in price range |

---

## Demo Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@example.com | admin123 |
| Agent | agent@example.com | agent123 |

---

## Frontend API Usage

### Import
```javascript
import { 
  authAPI, 
  propertiesAPI, 
  clientsAPI, 
  appointmentsAPI, 
  offersAPI 
} from './api/Client';
```

### Common Patterns

#### Login
```javascript
const response = await authAPI.login('email@example.com', 'password');
localStorage.setItem('authToken', response.data.data);
```

#### Get All (Paginated)
```javascript
const response = await propertiesAPI.getAll(page=1, pageSize=10);
const properties = response.data.data;
const totalPages = response.data.totalPages;
```

#### Get Single
```javascript
const response = await propertiesAPI.getById(1);
const property = response.data.data;
```

#### Create
```javascript
const response = await propertiesAPI.create({
  title: 'House',
  price: 500000,
  bedrooms: 3,
  address: '123 Main St',
  city: 'NYC',
  state: 'NY',
  zipCode: '10001'
});
```

#### Update
```javascript
const response = await propertiesAPI.update(1, {
  title: 'Updated Title',
  price: 510000
});
```

#### Delete
```javascript
await propertiesAPI.delete(1);
```

#### Search
```javascript
const response = await propertiesAPI.search('downtown');
```

#### Filter/Query
```javascript
// Get buyers only
const buyers = await clientsAPI.getByType('Buyer');

// Get scheduled appointments
const appointments = await appointmentsAPI.getByStatus('Scheduled');

// Get accepted offers
const acceptedOffers = await offersAPI.getByStatus('Accepted');

// Get offers in price range
const offers = await offersAPI.getByPriceRange(400000, 500000);
```

#### Action Endpoints
```javascript
// Accept offer
await offersAPI.accept(1);

// Reject offer
await offersAPI.reject(1);

// Withdraw offer
await offersAPI.withdraw(1);

// Refresh token
const newToken = await authAPI.refreshToken();

// Change password
await authAPI.changePassword('oldpass', 'newpass');

// Get current user
const user = await authAPI.me();
```

---

## Data Models

### User
```javascript
{
  id: number,
  email: string,
  firstName: string,
  lastName: string,
  role: string, // Admin, Agent
  createdAt: datetime,
  updatedAt: datetime?
}
```

### Property
```javascript
{
  id: number,
  title: string,
  description: string,
  address: string,
  city: string,
  state: string,
  zipCode: string,
  price: decimal,
  bedrooms: number,
  bathrooms: number,
  squareFeet: decimal,
  userId: number,
  createdAt: datetime,
  updatedAt: datetime?
}
```

### Client
```javascript
{
  id: number,
  firstName: string,
  lastName: string,
  email: string,
  phone: string,
  address: string,
  city: string,
  state: string,
  zipCode: string,
  clientType: string, // Buyer, Seller, Both
  status: string, // Active, Inactive, Prospect
  userId: number,
  createdAt: datetime,
  updatedAt: datetime?
}
```

### Appointment
```javascript
{
  id: number,
  propertyId: number,
  clientId: number,
  appointmentDate: datetime,
  time: string, // "10:00 AM"
  type: string, // Showing, Inspection, Closing
  status: string, // Scheduled, Completed, Cancelled, No-Show
  notes: string,
  durationMinutes: decimal?,
  feedbackFromClient: string?,
  userId: number,
  createdAt: datetime,
  updatedAt: datetime?
}
```

### Offer
```javascript
{
  id: number,
  propertyId: number,
  clientId: number,
  offeredPrice: decimal,
  status: string, // Pending, Accepted, Rejected, Withdrawn, Expired
  offerDate: datetime,
  expirationDate: datetime?,
  offerType: string, // Full Price, Contingent, As-Is
  downPaymentPercent: decimal?,
  closingDaysRequested: number?,
  contingencies: string?, // Home inspection, appraisal, financing
  agentNotes: string?,
  userId: number,
  createdAt: datetime,
  updatedAt: datetime?
}
```

---

## Error Handling

### Standard Error Response
```json
{
  "success": false,
  "message": "Error description",
  "data": null
}
```

### Common Errors
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Missing or invalid token
- `409 Conflict` - Email already registered
- `404 Not Found` - Resource not found
- `500 Server Error` - Backend error

---

## URL Reference

| Service | URL |
|---------|-----|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:5000/api |
| API Docs | http://localhost:5000/openapi/v1.json |
| Swagger UI | http://localhost:5000/swagger |

---

## Postman Collection

Save this as `PVS-API.postman_collection.json` to import into Postman:

```json
{
  "info": {
    "name": "PVS API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Auth",
      "item": [
        {
          "name": "Login",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/auth/login",
            "body": {
              "raw": "{\"email\":\"admin@example.com\",\"password\":\"admin123\"}"
            }
          }
        },
        {
          "name": "Register",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/auth/register",
            "body": {
              "raw": "{\"email\":\"test@example.com\",\"password\":\"password123\",\"firstName\":\"John\",\"lastName\":\"Doe\",\"role\":\"Agent\"}"
            }
          }
        }
      ]
    },
    {
      "name": "Properties",
      "item": [
        {
          "name": "Get All",
          "request": {
            "method": "GET",
            "url": "{{baseUrl}}/properties",
            "auth": {
              "type": "bearer",
              "bearer": [{"key": "token", "value": "{{token}}"}]
            }
          }
        },
        {
          "name": "Create",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/properties",
            "auth": {
              "type": "bearer",
              "bearer": [{"key": "token", "value": "{{token}}"}]
            }
          }
        }
      ]
    }
  ]
}
```

Import by:
1. Open Postman
2. Click Import → Upload Files
3. Add environment variable `baseUrl=http://localhost:5000/api`
4. Add environment variable `token=<JWT_TOKEN>`

---

## Testing Workflow

### 1. Login
```bash
POST /auth/login
Body: {"email":"admin@example.com","password":"admin123"}
Response: Get token
```

### 2. Test Properties
```bash
GET /properties
Header: Authorization: Bearer <TOKEN>
```

### 3. Create Property
```bash
POST /properties
Header: Authorization: Bearer <TOKEN>
Body: { title, price, bedrooms, address, etc }
```

### 4. Create Client
```bash
POST /clients
Header: Authorization: Bearer <TOKEN>
Body: { firstName, lastName, email, phone, clientType, etc }
```

### 5. Create Appointment
```bash
POST /appointments
Header: Authorization: Bearer <TOKEN>
Body: { propertyId, clientId, appointmentDate, time, type, etc }
```

### 6. Create Offer
```bash
POST /offers
Header: Authorization: Bearer <TOKEN>
Body: { propertyId, clientId, offeredPrice, offerType, etc }
```

### 7. Accept Offer
```bash
POST /offers/{id}/accept
Header: Authorization: Bearer <TOKEN>
```

---

## Quick Tips

- **All endpoints except login/register require authentication**
- **Token from login response should be stored and included in all subsequent requests**
- **Pagination defaults: page=1, pageSize=10**
- **Max pageSize is typically 100**
- **Dates should be in ISO 8601 format: 2026-04-29T10:00:00**
- **All currency is in decimal format (no currency symbol)**
- **Prices can be searched with price-range endpoint**
- **Status values are case-insensitive**

---

Last Updated: April 29, 2026
