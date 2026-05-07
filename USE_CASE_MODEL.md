# Task 3: Use Case Model

## Purpose of Use Case Models

A Use Case model is a behavioral model that describes the functional requirements of a system from the user's perspective. It shows what the system does (use cases) and who interacts with it (actors), without specifying how the functionality is implemented.

**Benefits in PVS Development:**
- Helps identify all system functionality from user viewpoints
- Serves as a communication tool between stakeholders and developers
- Forms the basis for system testing and validation
- Ensures the system meets user needs before implementation begins

## Actors in the PVS System

### Primary Actors:
1. **Property Owner/Seller** - Lists properties and manages sales
2. **Buyer** - Searches properties and makes offers
3. **Real Estate Agent** - Manages properties, clients, and appointments
4. **Branch Manager** - Oversees branch operations and staff

### Secondary Actors:
5. **System Administrator** - Manages system configuration
6. **Email System** - Sends notifications (automated)

## Use Case Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        PVS System                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────┐    ┌─────────────────┐    ┌─────────────┐     │
│  │  Property   │    │   Real Estate   │    │   Branch    │     │
│  │   Owner     │◄──►│     Agent       │◄──►│   Manager   │     │
│  │             │    │                 │    │             │     │
│  └─────────────┘    └─────────────────┘    └─────────────┘     │
│          │                     │                     │         │
│          │                     │                     │         │
│          ▼                     ▼                     ▼         │
│  ┌─────────────┐    ┌─────────────────┐    ┌─────────────┐     │
│  │   Manage    │    │   Manage        │    │   Manage    │     │
│  │ Properties  │    │   Properties    │    │   Branch    │     │
│  └─────────────┘    └─────────────────┘    └─────────────┘     │
│                                                                 │
│  ┌─────────────┐    ┌─────────────────┐                         │
│  │    Buyer    │────┤   Search &      │◄────────────────────────┘
│  │             │    │   View          │
│  └─────────────┘    │   Properties    │
│                     └─────────────────┘
│                              │
│                              ▼
│                     ┌─────────────────┐
│                     │   Make          │
│                     │   Appointment   │
│                     └─────────────────┘
│                              │
│                              ▼
│                     ┌─────────────────┐
│                     │   Submit        │
│                     │   Offer         │
│                     └─────────────────┘
│                                                                 │
│  ┌─────────────┐                                               │
│  │   Email     │◄──────────────────────────────────────────────┘
│  │   System    │    (Send notifications)
│  └─────────────┘                                               │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Scenarios for "Make Appointment" Use Case

### Scenario 1: Successful Appointment Booking
1. Buyer searches for properties and selects one to view
2. System displays property details and "Schedule Viewing" button
3. Buyer clicks button and selects preferred date/time
4. System checks agent availability and property availability
5. System confirms appointment and sends confirmation email
6. Appointment is recorded with "Scheduled" status

### Scenario 2: Conflicting Appointment Time
1. Buyer attempts to schedule viewing at 2:00 PM on Tuesday
2. System checks agent's calendar and finds conflict
3. System suggests alternative times (3:00 PM, 4:00 PM)
4. Buyer selects alternative time
5. System confirms appointment and sends confirmation

## Use Case Description: Make Appointment

**Use Case Name:** Make Appointment

**Participating Actors:** Buyer, Real Estate Agent, Email System

**Entry condition:** Buyer is logged in and viewing a property details page

**Flow of events:**
1. Buyer clicks "Schedule Viewing" button on property page
2. System displays calendar with available time slots
3. Buyer selects preferred date and time
4. System validates selected time against agent's availability
5. If time is available:
   - System creates appointment record
   - System sends confirmation email to buyer and agent
   - System updates property viewing history
6. If time is not available:
   - System displays alternative time suggestions
   - Return to step 3
7. Buyer receives confirmation with appointment details

**Exit condition:** Appointment is successfully scheduled and confirmed, or user cancels the process

**Special requirements:** Appointments must be scheduled at least 24 hours in advance. System must prevent double-booking. Email notifications must be sent within 5 minutes of booking.</content>
<parameter name="filePath">/home/trixuscode/Desktop/pvs/USE_CASE_MODEL.md