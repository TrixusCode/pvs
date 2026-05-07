# Task 4: Sequence Diagram

## Purpose of Sequence Diagrams

A Sequence Diagram is an interaction diagram that shows how objects interact with each other in a particular scenario over time. It focuses on the sequence of messages exchanged between objects to accomplish a specific task.

**Benefits in PVS Development:**
- Shows the detailed flow of interactions between system components
- Helps identify required methods and operations for each class
- Reveals potential timing and synchronization issues
- Serves as a blueprint for implementing the interaction logic
- Aids in understanding system behavior for complex use cases

## Sequence Diagram for "Make Appointment" (Successful Booking Scenario)

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    Buyer    │    │   Frontend  │    │   Backend   │    │  Database   │
│             │    │             │    │             │    │             │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │                  │
       │ 1. Click "Schedule│                  │                  │
       │    Viewing"       │                  │                  │
       │─────────────────►│                  │                  │
       │                  │                  │                  │
       │                  │ 2. GET /api/properties/{id}         │
       │                  │────────────────────────────────────►│
       │                  │                  │                  │
       │                  │                  │ 3. Validate token│
       │                  │                  │ & retrieve property│
       │                  │                  │◄──────────────────│
       │                  │◄────────────────────────────────────│
       │                  │                  │                  │
       │                  │ 4. Display calendar                │
       │◄─────────────────│                  │                  │
       │                  │                  │                  │
       │ 5. Select date/time│                  │                  │
       │─────────────────►│                  │                  │
       │                  │                  │                  │
       │                  │ 6. POST /api/appointments           │
       │                  │                  │                  │
       │                  │ {propertyId, date, time, notes}     │
       │                  │────────────────────────────────────►│
       │                  │                  │                  │
       │                  │                  │ 7. Validate request│
       │                  │                  │ & check availability│
       │                  │                  │─────────────────►│
       │                  │                  │◄──────────────────│
       │                  │                  │                  │
       │                  │                  │ 8. Create appointment│
       │                  │                  │─────────────────►│
       │                  │                  │◄──────────────────│
       │                  │◄────────────────────────────────────│
       │                  │                  │                  │
       │                  │ 9. POST /api/notifications/send     │
       │                  │                  │                  │
       │                  │ {appointmentId, type: "confirmation"}│
       │                  │────────────────────────────────────►│
       │                  │                  │                  │
       │                  │                  │ 10. Send emails   │
       │                  │                  │─────────────────►│
       │                  │                  │◄──────────────────│
       │                  │◄────────────────────────────────────│
       │                  │                  │                  │
       │ 11. Display success│                  │                  │
       │     message        │                  │                  │
       │◄─────────────────►│                  │                  │
       │                  │                  │                  │

Time ▼
```

## Alternative Flow: Time Conflict

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│    Buyer    │    │   Frontend  │    │   Backend   │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                  │
       │ Select date/time │                  │
       │─────────────────►│                  │
       │                  │ POST /api/appointments
       │                  │────────────────────────────────────►
       │                  │                  │
       │                  │                  │ Check availability
       │                  │                  │─────────────────►
       │                  │                  │◄──────────────────
       │                  │                  │ TIME CONFLICT!
       │                  │◄────────────────────────────────────
       │                  │                  │
       │                  │ Display "Time unavailable"         │
       │◄─────────────────│                  │
       │                  │                  │
       │                  │ GET /api/availability/{agentId}    │
       │                  │────────────────────────────────────►
       │                  │                  │
       │                  │                  │ Query available slots
       │                  │                  │─────────────────►
       │                  │                  │◄──────────────────
       │                  │◄────────────────────────────────────
       │                  │                  │
       │ Display alternative│                  │
       │ times             │                  │
       │◄─────────────────►│                  │
```

## Key Interactions Explained

1. **User Interface Interaction:** Buyer selects viewing option
2. **Data Retrieval:** Frontend fetches property details for context
3. **User Input:** Buyer provides appointment preferences
4. **Business Logic:** Backend validates availability and business rules
5. **Data Persistence:** Appointment record is created in database
6. **Notification:** Automated emails sent to confirm booking
7. **User Feedback:** Success/error messages displayed to user

This sequence diagram helps identify the required API endpoints, database operations, and integration points needed for the appointment booking feature.</content>
<parameter name="filePath">/home/trixuscode/Desktop/pvs/SEQUENCE_DIAGRAM.md