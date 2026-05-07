# Task 5: Class Diagram

## Analysis of Classes

The PVS system requires the following core classes based on the problem statement analysis:

### 1. User
**Justification:** Represents system users including administrators, managers, agents, and clients. Contains authentication information and role-based access control.

### 2. Branch
**Justification:** Represents physical branch offices. Each branch has a manager and contains multiple employees and properties.

### 3. Property
**Justification:** Core business entity representing real estate listings. Contains all property details, pricing, and status information.

### 4. Client
**Justification:** Represents buyers and sellers who interact with the system. Can be both buyers and sellers simultaneously.

### 5. Appointment
**Justification:** Represents scheduled property viewings between clients and agents. Tracks scheduling, status, and feedback.

### 6. Offer
**Justification:** Represents offers made on properties. Includes pricing, terms, and negotiation status.

### 7. Address
**Justification:** Reusable component for storing location information used by branches, properties, and clients.

## Class Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              PVS Class Diagram                                  │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────────┐    ┌─────────────────────┐    ┌─────────────────────┐ │
│  │       User          │    │      Branch         │    │      Address        │ │
│  ├─────────────────────┤    ├─────────────────────┤    ├─────────────────────┤ │
│  │ +Id: int            │    │ +Id: int            │    │ +Id: int            │ │
│  │ +Email: string      │    │ +Name: string       │    │ +City: string       │ │
│  │ +FirstName: string  │    │ +Description: string│    │ +State: string      │ │
│  │ +LastName: string   │    │ +Address: Address   │    │ +ZipCode: string    │ │
│  │ +PasswordHash: str  │    │ +Phone: string      │    │                     │ │
│  │ +Role: string       │    │ +Email: string      │    │ +GetFullAddress()   │ │
│  │ +CreatedAt: DateTime│    │ +ManagerName: str   │    └─────────────────────┘ │
│  │ +UpdatedAt: DateTime│    │ +Status: BranchStat │                             │
│  │                     │    │ +ManagerUserId: int │                             │
│  │ +Login()            │    │ +CreatedAt: DateTime│                             │
│  │ +ChangePassword()   │    │ +UpdatedAt: DateTime│                             │
│  │ +GetProfile()       │    └─────────────────────┘                             │
│  └─────────────────────┘              │                                        │
│              ▲                         │                                        │
│              │ 1                    1..*│                                        │
│              │                         ▼                                        │
│              │                ┌─────────────────────┐                            │
│              │                │      Property       │                            │
│              │                ├─────────────────────┤                            │
│              │                │ +Id: int            │                            │
│              │                │ +Title: string      │                            │
│              │                │ +PropertyType: enum │                            │
│              │                │ +Description: str   │                            │
│              │                │ +Address: Address   │                            │
│              │                │ +Price: decimal     │                            │
│              │                │ +Bedrooms: int      │                            │
│              │                │ +Bathrooms: int     │                            │
│              │                │ +SquareFeet: decimal│                            │
│              │                │ +UserId: int        │                            │
│              │                │ +CreatedAt: DateTime│                            │
│              │                │ +UpdatedAt: DateTime│                            │
│              │                │                     │                            │
│              │                │ +CalculatePriceSqFt()│                           │
│              │                │ +IsAvailable()       │                            │
│              │                └─────────────────────┘                            │
│              │                         │                                        │
│              │ 1                    1..*│                                        │
│              │                         ▼                                        │
│  ┌─────────────────────┐       ┌─────────────────────┐                            │
│  │       Client        │       │    Appointment      │                            │
│  ├─────────────────────┤       ├─────────────────────┤                            │
│  │ +Id: int            │       │ +Id: int            │                            │
│  │ +FirstName: string  │       │ +PropertyId: int    │                            │
│  │ +LastName: string   │       │ +ClientId: int      │                            │
│  │ +Email: string      │       │ +AppointmentDate: DT│                            │
│  │ +Phone: string      │       │ +Time: string       │                            │
│  │ +Address: Address   │       │ +Type: AppointmentTy│                            │
│  │ +ClientType: enum   │       │ +Status: Appointment│                            │
│  │ +Status: ClientStat │       │ +Notes: string      │                            │
│  │ +UserId: int        │       │ +DurationMinutes:dec│                            │
│  │ +CreatedAt: DateTime│       │ +FeedbackFromClient:│                            │
│  │ +UpdatedAt: DateTime│       │ +UserId: int        │                            │
│  │                     │       │ +CreatedAt: DateTime│                            │
│  │ +GetFullName()      │       │ +UpdatedAt: DateTime│                            │
│  │ +IsActive()         │       │                     │                            │
│  └─────────────────────┘       │ +Schedule()         │                            │
│              ▲                 │ +Cancel()           │                            │
│              │ 1               │ +Complete()         │                            │
│              │                 └─────────────────────┘                            │
│              │ 1..*                    │                                        │
│              │                         │                                        │
│              ▼                         ▼                                        │
│  ┌─────────────────────┐       ┌─────────────────────┐                            │
│  │       Offer         │       │                     │                            │
│  ├─────────────────────┤       │                     │                            │
│  │ +Id: int            │       │                     │                            │
│  │ +PropertyId: int    │       │                     │                            │
│  │ +ClientId: int      │       │                     │                            │
│  │ +OfferedPrice: dec  │       │                     │                            │
│  │ +Status: OfferStatus│       │                     │                            │
│  │ +OfferDate: DateTime│       │                     │                            │
│  │ +ExpirationDate: DT │       │                     │                            │
│  │ +OfferType: enum    │       │                     │                            │
│  │ +DownPayment%: dec  │       │                     │                            │
│  │ +ClosingDaysReq: int│       │                     │                            │
│  │ +Contingencies: enum│       │                     │                            │
│  │ +AgentNotes: string │       │                     │                            │
│  │ +UserId: int        │       │                     │                            │
│  │ +CreatedAt: DateTime│       │                     │                            │
│  │ +UpdatedAt: DateTime│       │                     │                            │
│  │                     │       │                     │                            │
│  │ +Accept()           │       │                     │                            │
│  │ +Reject()           │       │                     │                            │
│  │ +Withdraw()         │       │                     │                            │
│  └─────────────────────┘       │                     │                            │
│                                │                     │                            │
└────────────────────────────────┴─────────────────────┴────────────────────────────┘
```

## Relationships Explained

### Association Relationships:
- **User → Branch (1..*):** A user (manager) can manage multiple branches, but each branch has one manager
- **User → Property (1..*):** An agent can list multiple properties
- **User → Client (1..*):** An agent can have multiple clients
- **User → Appointment (1..*):** An agent can schedule multiple appointments
- **User → Offer (1..*):** An agent can handle multiple offers

### Composition Relationships:
- **Branch → Address (1..1):** Each branch has exactly one address
- **Property → Address (1..1):** Each property has exactly one address
- **Client → Address (0..1):** A client may or may not have an address

### Aggregation Relationships:
- **Property → Appointment (1..*):** A property can have multiple viewing appointments
- **Property → Offer (1..*):** A property can receive multiple offers
- **Client → Appointment (1..*):** A client can have multiple appointments
- **Client → Offer (1..*):** A client can make multiple offers

## Design Patterns Used

### Repository Pattern
Each entity has associated repository classes for data access abstraction.

### Service Layer Pattern
Business logic is encapsulated in service classes that coordinate between repositories and controllers.

### DTO Pattern
Data Transfer Objects are used for API communication to separate internal models from external contracts.

This class diagram provides a solid foundation for implementing the PVS system with proper separation of concerns and clear relationships between business entities.</content>
<parameter name="filePath">/home/trixuscode/Desktop/pvs/CLASS_DIAGRAM.md