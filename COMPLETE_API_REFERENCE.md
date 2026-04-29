# Complete API Reference - All Endpoints

## Base URL
```
http://localhost:5000/api
```

## Authentication
All endpoints except `/auth/login` and `/auth/register` require Bearer token:
```
Authorization: Bearer <JWT_TOKEN>
```

---

## 🔑 Authentication Module (`/auth`)

### 1. Login
**Request:**
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "admin123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Demo Credentials:**
- Admin: `admin@example.com` / `admin123`
- Agent: `agent@example.com` / `agent123`

---

### 2. Register
**Request:**
```http
POST /auth/register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "password": "password123",
  "firstName": "John",
  "lastName": "Smith",
  "role": "Agent"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 3,
      "email": "newuser@example.com",
      "firstName": "John",
      "lastName": "Smith",
      "role": "Agent"
    }
  }
}
```

**Validation Rules:**
- Email required, must be unique
- Password minimum 6 characters
- First name and last name required
- Role: "Admin" or "Agent" (default: "Agent")

---

### 3. Get Current User
**Request:**
```http
GET /auth/me
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "admin@example.com",
    "firstName": "Admin",
    "lastName": "User",
    "role": "Admin",
    "createdAt": "2026-04-29T10:00:00"
  }
}
```

---

### 4. Refresh Token
**Request:**
```http
POST /auth/refresh-token
Authorization: Bearer <OLD_TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### 5. Change Password
**Request:**
```http
POST /auth/change-password
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "currentPassword": "admin123",
  "newPassword": "newpassword123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

---

## 🏠 Properties Module (`/properties`)

### 1. Get All Properties (Paginated)
**Request:**
```http
GET /properties?page=1&pageSize=10
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "title": "Beautiful House in Downtown",
      "description": "3 bedroom, 2 bathroom house with modern amenities",
      "address": "123 Main St",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "price": 450000,
      "bedrooms": 3,
      "bathrooms": 2,
      "squareFeet": 2500,
      "userId": 1,
      "createdAt": "2026-03-30T10:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 24,
  "totalPages": 3
}
```

---

### 2. Get Property by ID
**Request:**
```http
GET /properties/1
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "title": "Beautiful House in Downtown",
    ...
  }
}
```

---

### 3. Create Property
**Request:**
```http
POST /properties
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "title": "New Property",
  "description": "Modern apartment",
  "address": "789 Park Ave",
  "city": "Los Angeles",
  "state": "CA",
  "zipCode": "90001",
  "price": 550000,
  "bedrooms": 4,
  "bathrooms": 3,
  "squareFeet": 3200
}
```

**Response (201 Created):**
```json
{
  "id": 25,
  "title": "New Property",
  "price": 550000,
  ...
}
```

---

### 4. Update Property
**Request:**
```http
PUT /properties/1
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "title": "Updated Title",
  "price": 475000,
  "bedrooms": 3
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Property updated successfully",
  "data": { ... }
}
```

---

### 5. Delete Property
**Request:**
```http
DELETE /properties/1
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Property deleted successfully"
}
```

---

### 6. Search Properties
**Request:**
```http
GET /properties/search?q=downtown
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    { "id": 1, "title": "Beautiful House in Downtown", ... }
  ]
}
```

---

## 👥 Clients Module (`/clients`)

### 1. Get All Clients
**Request:**
```http
GET /clients?page=1&pageSize=10
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@example.com",
      "phone": "(555) 123-4567",
      "address": "123 Oak Street",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "clientType": "Buyer",
      "status": "Active",
      "userId": 1,
      "createdAt": "2026-03-30T10:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}
```

---

### 2. Get Client by ID
**Request:**
```http
GET /clients/1
Authorization: Bearer <TOKEN>
```

---

### 3. Create Client
**Request:**
```http
POST /clients
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phone": "(555) 987-6543",
  "address": "456 Elm St",
  "city": "Los Angeles",
  "state": "CA",
  "zipCode": "90001",
  "clientType": "Seller",
  "status": "Active"
}
```

**Options:**
- `clientType`: "Buyer", "Seller", "Both"
- `status`: "Active", "Inactive", "Prospect"

---

### 4. Update Client
**Request:**
```http
PUT /clients/1
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "phone": "(555) 555-5555",
  "status": "Inactive"
}
```

---

### 5. Delete Client
**Request:**
```http
DELETE /clients/1
Authorization: Bearer <TOKEN>
```

---

### 6. Search Clients
**Request:**
```http
GET /clients/search?q=John
Authorization: Bearer <TOKEN>
```

Searches: First name, last name, email

---

### 7. Get Clients by Type
**Request:**
```http
GET /clients/by-type/Buyer
Authorization: Bearer <TOKEN>
```

---

## 📅 Appointments Module (`/appointments`)

### 1. Get All Appointments
**Request:**
```http
GET /appointments?page=1&pageSize=10
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "propertyId": 1,
      "clientId": 1,
      "appointmentDate": "2026-05-02T10:00:00",
      "time": "10:00 AM",
      "type": "Showing",
      "status": "Scheduled",
      "notes": "Initial property showing",
      "durationMinutes": 60,
      "feedbackFromClient": null,
      "userId": 1,
      "createdAt": "2026-04-29T10:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}
```

---

### 2. Get Appointment by ID
**Request:**
```http
GET /appointments/1
Authorization: Bearer <TOKEN>
```

---

### 3. Create Appointment
**Request:**
```http
POST /appointments
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "propertyId": 1,
  "clientId": 1,
  "appointmentDate": "2026-05-05T14:00:00",
  "time": "2:00 PM",
  "type": "Inspection",
  "status": "Scheduled",
  "notes": "Complete home inspection",
  "durationMinutes": 120
}
```

**Options:**
- `type`: "Showing", "Inspection", "Closing"
- `status`: "Scheduled", "Completed", "Cancelled", "No-Show"

---

### 4. Update Appointment
**Request:**
```http
PUT /appointments/1
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "status": "Completed",
  "feedbackFromClient": "Client very satisfied with property"
}
```

---

### 5. Delete Appointment
**Request:**
```http
DELETE /appointments/1
Authorization: Bearer <TOKEN>
```

---

### 6. Get Appointments for Property
**Request:**
```http
GET /appointments/property/1
Authorization: Bearer <TOKEN>
```

---

### 7. Get Appointments for Client
**Request:**
```http
GET /appointments/client/1
Authorization: Bearer <TOKEN>
```

---

### 8. Get Appointments by Status
**Request:**
```http
GET /appointments/by-status/Scheduled
Authorization: Bearer <TOKEN>
```

---

### 9. Get Appointments by Date Range
**Request:**
```http
GET /appointments/by-date?startDate=2026-04-01&endDate=2026-05-31
Authorization: Bearer <TOKEN>
```

---

## 💰 Offers Module (`/offers`)

### 1. Get All Offers
**Request:**
```http
GET /offers?page=1&pageSize=10
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "propertyId": 1,
      "clientId": 1,
      "offeredPrice": 425000,
      "status": "Pending",
      "offerDate": "2026-04-27T10:00:00",
      "expirationDate": "2026-05-02T10:00:00",
      "offerType": "Contingent",
      "downPaymentPercent": 20,
      "closingDaysRequested": 30,
      "contingencies": "Home inspection, appraisal, financing",
      "agentNotes": "Good client, ready to move",
      "userId": 1,
      "createdAt": "2026-04-27T10:00:00"
    }
  ],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1
}
```

---

### 2. Get Offer by ID
**Request:**
```http
GET /offers/1
Authorization: Bearer <TOKEN>
```

---

### 3. Create Offer
**Request:**
```http
POST /offers
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "propertyId": 1,
  "clientId": 1,
  "offeredPrice": 440000,
  "expirationDate": "2026-05-05T10:00:00",
  "offerType": "Contingent",
  "downPaymentPercent": 20,
  "closingDaysRequested": 30,
  "contingencies": "Home inspection, financing",
  "agentNotes": "Strong offer from buyer"
}
```

**Options:**
- `status`: "Pending", "Accepted", "Rejected", "Withdrawn", "Expired"
- `offerType`: "Full Price", "Contingent", "As-Is"

---

### 4. Update Offer
**Request:**
```http
PUT /offers/1
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "offeredPrice": 430000,
  "downPaymentPercent": 25
}
```

---

### 5. Delete Offer
**Request:**
```http
DELETE /offers/1
Authorization: Bearer <TOKEN>
```

---

### 6. Accept Offer
**Request:**
```http
POST /offers/1/accept
Authorization: Bearer <TOKEN>
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Offer accepted successfully",
  "data": { "id": 1, "status": "Accepted", ... }
}
```

---

### 7. Reject Offer
**Request:**
```http
POST /offers/1/reject
Authorization: Bearer <TOKEN>
```

---

### 8. Withdraw Offer
**Request:**
```http
POST /offers/1/withdraw
Authorization: Bearer <TOKEN>
```

---

### 9. Get Offers for Property
**Request:**
```http
GET /offers/property/1
Authorization: Bearer <TOKEN>
```

---

### 10. Get Offers from Client
**Request:**
```http
GET /offers/client/1
Authorization: Bearer <TOKEN>
```

---

### 11. Get Offers by Status
**Request:**
```http
GET /offers/by-status/Accepted
Authorization: Bearer <TOKEN>
```

---

### 12. Get Offers by Price Range
**Request:**
```http
GET /offers/price-range?minPrice=400000&maxPrice=500000
Authorization: Bearer <TOKEN>
```

---

## ✅ Health Check

### Check Backend Status
**Request:**
```http
GET /health
```

**Response (200 OK):**
```json
{
  "status": "ok",
  "timestamp": "2026-04-29T10:00:00"
}
```

---

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation completed",
  "data": { ... }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "data": null
}
```

### Paginated Response
```json
{
  "data": [...],
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5
}
```

---

## 🔴 Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 500 | Server Error |

---

## 🧪 Testing with cURL

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "admin123"
  }'
```

### Get Properties (with token)
```bash
TOKEN="eyJhbGciOiJIUzI1NiIs..."

curl http://localhost:5000/api/properties \
  -H "Authorization: Bearer $TOKEN"
```

### Create Property
```bash
TOKEN="eyJhbGciOiJIUzI1NiIs..."

curl -X POST http://localhost:5000/api/properties \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "New House",
    "price": 500000,
    "bedrooms": 3,
    "address": "789 Main St",
    "city": "NYC",
    "state": "NY",
    "zipCode": "10001"
  }'
```

---

## 🚀 Frontend Usage Examples

### Login
```javascript
import { authAPI } from './api/Client';

const response = await authAPI.login('admin@example.com', 'admin123');
localStorage.setItem('authToken', response.data.data);
```

### Get All Properties
```javascript
import { propertiesAPI } from './api/Client';

const response = await propertiesAPI.getAll(1, 10);
console.log(response.data.data); // array of properties
console.log(response.data.totalPages); // pagination
```

### Create Property
```javascript
await propertiesAPI.create({
  title: 'New House',
  price: 450000,
  bedrooms: 3,
  bathrooms: 2,
  address: '123 Main St',
  city: 'New York',
  state: 'NY',
  zipCode: '10001'
});
```

### Get Offers for Property
```javascript
const offers = await offersAPI.getByPropertyId(1);
console.log(offers.data.data); // array of offers
```

### Accept an Offer
```javascript
await offersAPI.accept(5);
```

---

This is the complete API reference for the PVS system with all modules fully implemented!
