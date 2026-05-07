# Task 1: Requirements Elicitation

## Stakeholders in the PVS System

### 1. Property Owners/Sellers
**Justification:** They are the primary users who list their properties for sale through the system. The system must capture their property details, contact information, and manage the sale process from their perspective.

### 2. Real Estate Agents/Representatives
**Justification:** They are responsible for managing property listings, scheduling appointments, and facilitating the buying/selling process. They need access to all property and client information.

### 3. Branch Managers
**Justification:** They oversee the operations of their branch, manage staff, and have authority to add/modify employee and branch details. They need higher-level access to system data.

### 4. Buyers/Clients
**Justification:** They browse available properties, make offers, and schedule viewing appointments. The system must provide them with search capabilities and secure access to relevant information.

## Methods for Eliciting System Requirements

### 1. Interviews with Stakeholders
**Why:** Direct interaction with property owners, agents, managers, and buyers allows for gathering detailed insights into their workflows, pain points, and specific needs. This method is effective for understanding the human aspects of the system.

### 2. Document Analysis
**Why:** Reviewing existing real estate processes, current paperwork, and any legacy systems helps identify required data fields, business rules, and compliance requirements. This ensures the new system aligns with established practices.

## Additional Information Needed

### 1. User Authentication and Authorization Levels
**Why:** The problem statement mentions different user types (CEO, managers, representatives, clients) but doesn't specify access controls. Need to clarify what each role can view/modify.

### 2. Property Status Workflow
**Why:** The status field has multiple states (for sale, under offer, etc.) but the transitions between states aren't defined. Need to understand the business rules for status changes.

### 3. Appointment Scheduling Constraints
**Why:** Need to know if there are restrictions on appointment times, duration, or how far in advance appointments can be booked.

### 4. Offer Management Process
**Why:** The problem mentions offers can be accepted/rejected, but need details on the negotiation process, counter-offers, and how offers expire.

### 5. Reporting Requirements
**Why:** The system should provide insights to management, but specific reports (sales performance, branch statistics, etc.) aren't mentioned.</content>
<parameter name="filePath">/home/trixuscode/Desktop/pvs/REQUIREMENTS_ELICITATION.md