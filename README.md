# Travl Platform

## Overview

**Travl Platform** is an intercity ride-hailing and ride-sharing application designed to connect passengers with drivers traveling between cities. It offers affordable, convenient, and eco-friendly transportation solutions while adhering to scalable and modular architectural principles.

## Features

- **Passengers**:
  - Book rides based on origin, destination, and time.
  - Split costs with other passengers for shared rides.
  - Real-time tracking and secure payment options.

- **Drivers**:
  - List available routes and manage bookings.
  - Track earnings and approve passenger requests.

- **Admins**:
  - Oversee platform activity, resolve disputes, and manage user profiles.

## Clean Architecture

Travl Platform follows the **Clean Architecture** to ensure separation of concerns and maintainability. This design comprises the following layers:

### 1. API Layer
- Exposes endpoints for external communication.
- Includes ASP.NET Web API Controllers and SignalR hubs.

### 2. Application Layer
- Implements application-specific logic and orchestrates use cases.
- Contains interfaces that abstract infrastructure dependencies.

### 3. Core Layer
- Contains the core business rules and domain entities.
- Fully independent of external systems and frameworks.

### 4. Infrastructure Layer
- Implements interfaces defined in the application layer.
- Manages persistence, messaging, and external service integrations.

## System Architecture

This repository includes three diagrams, modeled using Structurizr, to provide a clear view of the system:

### 1. Context View (C1)
Highlights high-level interactions between the Travl Platform, its users (Passengers, Drivers, Admins), and external systems (RabbitMQ, Payment Gateway, Notification Service).

### 2. Container View (C2)
Illustrates the main components of the system:
- **Mobile App** (MAUI)
- **Web App** (React)
- **Backend API** (ASP.NET Core)
- **Read & Write Database** (PostgreSQL/SQL Server)

### 3. Component View (C3)
Focuses on the Backend API, breaking it down into:
- **API Layer**
- **Application Logic**
- **Core Logic**
- **Infrastructure**

These diagrams can be found in the `architecture/` folder:
- `C1_ContextView.png`
- `C2_ContainerView.png`
- `C3_ComponentView.png`

## Folder Structure

The project adheres to the following modular folder structure, designed around Clean Architecture principles and CQRS:

```
/Travl.Backend
│
├── /Core
│   ├── /Entities
│   ├── /DomainServices
│
├── /Application
│   ├── /Commands
│   │   ├── CreateBooking
│   │   │   ├── CreateBookingCommand.cs
│   │   │   ├── CreateBookingCommandHandler.cs
│   │   │   ├── CreateBookingValidator.cs
│   │
│   ├── /Queries
│   │   ├── GetAvailableRides
│   │   │   ├── GetAvailableRidesQuery.cs
│   │   │   ├── GetAvailableRidesQueryHandler.cs
│   │
│   ├── /Interfaces
│   ├── /DTOs
│
├── /Infrastructure
│   ├── /Persistence
│   │   ├── /Migrations
│   │   ├── RideRepository.cs
│   │   ├── UserRepository.cs
│   │
│   ├── /Messaging
│   │   ├── RabbitMQPublisher.cs
│   │   ├── RabbitMQConsumer.cs
│   │
│   ├── /ExternalServices
│   │   ├── PaymentGateway.cs
│   │   ├── NotificationService.cs
│
├── /API
│   ├── /Controllers
│   ├── /Filters
│
└── /Tests
    ├── /UnitTests
    ├── /IntegrationTests
```

## How to Run

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-username/travl-platform.git
   ```

2. **Navigate to the Backend Project**:
   ```bash
   cd Travl.Backend
   ```

3. **Setup Environment Variables**:
   - Database connection strings.
   - RabbitMQ credentials.
   - Payment gateway API keys.

4. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

5. **Start the Backend API**:
   ```bash
   dotnet run
   ```

## Contribution Guidelines

- Fork the repository.
- Create a feature branch.
- Commit your changes with clear and concise messages.
- Submit a pull request for review.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

### Acknowledgments
- **Structurizr**: For modeling the system's architecture.
- **Clean Architecture**: For inspiring the modular design approach.

