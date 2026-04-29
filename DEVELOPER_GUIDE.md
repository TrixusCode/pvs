# Developer Guide - Extending the PVS System

## How to Add New Features

This guide shows how to extend the PVS system with new API endpoints and components.

## Example: Adding Clients Module

### Step 1: Define Backend Model

Create/Edit `backend/PVS.Api/Models/Client.cs`:

```csharp
namespace PVS.Api.Models;

public class Client
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

### Step 2: Create Backend Controller

Create `backend/PVS.Api/Modules/Clients/ClientsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;

namespace PVS.Api.Modules.Clients;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    // Mock data
    private static readonly List<Client> Clients = new()
    {
        new Client
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "555-0100",
            Address = "123 Main St",
            City = "New York",
            State = "NY",
            ZipCode = "10001",
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        }
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var total = Clients.Count;
        var items = Clients.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Client>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found" });

        return Ok(new ApiResponse<Client>
        {
            Success = true,
            Data = client
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = new Client
        {
            Id = Clients.Max(c => c.Id) + 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            UserId = 1, // TODO: Get from authenticated user
            CreatedAt = DateTime.UtcNow
        };

        Clients.Add(client);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateClientRequest request)
    {
        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound();

        if (!string.IsNullOrEmpty(request.FirstName)) client.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName)) client.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Email)) client.Email = request.Email;
        if (!string.IsNullOrEmpty(request.Phone)) client.Phone = request.Phone;
        client.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Client>
        {
            Success = true,
            Data = client
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound();

        Clients.Remove(client);
        return Ok(new ApiResponse { Success = true });
    }
}

public class CreateClientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class UpdateClientRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
```

### Step 3: Add API Client Methods

Edit `pvs-frontend/src/api/Client.js`:

```javascript
// Find the existing clientsAPI section and update it:
export const clientsAPI = {
  getAll: (page = 1, pageSize = 10) =>
    apiClient.get('/clients', { params: { page, pageSize } }),
  
  getById: (id) => apiClient.get(`/clients/${id}`),
  
  create: (data) => apiClient.post('/clients', data),
  
  update: (id, data) => apiClient.put(`/clients/${id}`, data),
  
  delete: (id) => apiClient.delete(`/clients/${id}`),
};
```

### Step 4: Create Frontend Component

Create `pvs-frontend/src/modules/clients/Clients.jsx`:

```jsx
import { useState, useEffect } from 'react';
import { clientsAPI } from '../../api/Client';
import './Clients.css';

export default function Clients() {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({});

  useEffect(() => {
    fetchClients();
  }, []);

  const fetchClients = async () => {
    try {
      const response = await clientsAPI.getAll(1, 10);
      setClients(response.data.data || response.data);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load clients');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      const newClient = await clientsAPI.create(formData);
      setClients([...clients, newClient.data]);
      setFormData({});
      setShowForm(false);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create client');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this client?')) return;
    try {
      await clientsAPI.delete(id);
      setClients(clients.filter(c => c.id !== id));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete');
    }
  };

  if (loading) return <div className="container">Loading clients...</div>;

  return (
    <div className="container">
      <h1>Clients</h1>
      
      {error && <div className="error">{error}</div>}
      
      <button onClick={() => setShowForm(!showForm)}>
        {showForm ? 'Cancel' : 'Add Client'}
      </button>

      {showForm && (
        <form onSubmit={handleCreate} className="client-form">
          <input
            placeholder="First Name"
            value={formData.firstName || ''}
            onChange={(e) => setFormData({...formData, firstName: e.target.value})}
            required
          />
          <input
            placeholder="Last Name"
            value={formData.lastName || ''}
            onChange={(e) => setFormData({...formData, lastName: e.target.value})}
            required
          />
          <input
            type="email"
            placeholder="Email"
            value={formData.email || ''}
            onChange={(e) => setFormData({...formData, email: e.target.value})}
            required
          />
          <input
            placeholder="Phone"
            value={formData.phone || ''}
            onChange={(e) => setFormData({...formData, phone: e.target.value})}
          />
          <button type="submit">Save Client</button>
        </form>
      )}

      <table className="clients-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Phone</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {clients.map(client => (
            <tr key={client.id}>
              <td>{client.firstName} {client.lastName}</td>
              <td>{client.email}</td>
              <td>{client.phone}</td>
              <td>
                <button onClick={() => {/* TODO: edit */}}>Edit</button>
                <button onClick={() => handleDelete(client.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

### Step 5: Add Route in App.jsx

```jsx
import Clients from './modules/clients/Clients';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/properties" element={<Properties />} />
        <Route path="/clients" element={<Clients />} />  {/* Add this */}
        <Route path="/" element={<Navigate to="/properties" />} />
      </Routes>
    </Router>
  );
}
```

## Extending Without Creating from Scratch

### Quick Template for New CRUD

**Backend Controller Template:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemController : ControllerBase
{
    private static List<Item> Items = new();

    [HttpGet]
    public IActionResult GetAll(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        return Ok(new PaginatedResponse<Item>
        {
            Data = Items.Skip(skip).Take(pageSize).ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = Items.Count
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public IActionResult Create(CreateItemRequest request)
    {
        var item = new Item { /* map from request */ };
        Items.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateItemRequest request)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        // Update properties
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        Items.Remove(item);
        return Ok();
    }
}
```

**Frontend Component Template:**
```jsx
import { useEffect, useState } from 'react';
import { itemsAPI } from '../../api/Client';

export default function Items() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    itemsAPI.getAll(1, 10)
      .then(res => setItems(res.data))
      .catch(err => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <h1>Items</h1>
      {items.map(item => (
        <div key={item.id}>{item.name}</div>
      ))}
    </div>
  );
}
```

## Database Integration

### Setup Entity Framework (when ready)

1. **Create DbContext:**
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Client> Clients => Set<Client>();
}
```

2. **Configure in Program.cs:**
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));
```

3. **Create Migration:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Use in Controller:**
```csharp
private readonly AppDbContext _context;

public ClientsController(AppDbContext context)
{
    _context = context;
}

[HttpGet]
public IActionResult GetAll()
{
    var items = _context.Clients.ToList();
    return Ok(items);
}
```

## Common Patterns

### Handle Pagination
```javascript
const [page, setPage] = useState(1);
const [pageSize] = useState(10);

useEffect(() => {
  clientsAPI.getAll(page, pageSize)
    .then(res => {
      setItems(res.data.data);
      setTotalPages(res.data.totalPages);
    });
}, [page]);
```

### Handle Loading States
```jsx
const [loading, setLoading] = useState(false);
const [error, setError] = useState('');

const handleAction = async () => {
  setLoading(true);
  setError('');
  try {
    await clientsAPI.create(data);
    // Success
  } catch (err) {
    setError(err.response?.data?.message);
  } finally {
    setLoading(false);
  }
};
```

### Form Handling
```jsx
const [formData, setFormData] = useState({
  firstName: '',
  lastName: '',
  email: ''
});

const handleSubmit = async (e) => {
  e.preventDefault();
  await clientsAPI.create(formData);
  setFormData({ firstName: '', lastName: '', email: '' });
};
```

## Checklist for New Feature

- [ ] Created backend model in `Models/`
- [ ] Created backend controller in `Modules/`
- [ ] Added API methods in frontend `Client.js`
- [ ] Created frontend component in `modules/`
- [ ] Added CSS styling
- [ ] Added route in `App.jsx`
- [ ] Tested login flow
- [ ] Tested CRUD operations
- [ ] Handled loading/error states
- [ ] Tested pagination (if applicable)

---

Follow these patterns and you can extend the system indefinitely!
