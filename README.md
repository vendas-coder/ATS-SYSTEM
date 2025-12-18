# ATS-SYSTEM

A **production-ready Applicant Tracking System (ATS) backend** built with **ASP.NET Core (.NET 8)**, designed to manage the full hiring lifecycle — from application intake to decision — with **auditability, analytics, role-based access control, and workflow enforcement**.

This project demonstrates **clean architecture**, **domain-driven design principles**, and **enterprise-grade backend features** suitable for real-world HR and recruitment platforms.

---

## Key Features

### Core Hiring Workflow

* Job creation and management
* Candidate management
* Job applications with **strict status transition rules**
* Full application lifecycle tracking

### Application Notes & Timeline

* Recruiter notes per application stage
* Immutable **application timeline**
* Unified view of status changes, notes, and events

### Status Management

* Enforced application status transitions
* Ability to **revert application status with a required reason**
* Centralized business rules via domain policies

### Audit Trail & Event Sourcing

* Application audit logs (who, what, when, why)
* Event sourcing for analytics and compliance
* Designed for reporting, compliance, and debugging

### Hiring Analytics

* Hiring funnel analytics (per stage)
* Application conversion metrics
* Analytics-ready DTOs and APIs

### Multi‑Recruiter Ownership

* Multiple recruiters can be assigned to a single application
* Ownership stored at the domain level
* Authorization enforced at API level

### Security & Access Control

* Role-Based Access Control (RBAC)

  * Recruiter vs Admin
* Policy-based authorization
* Assignment-based access enforcement

### Automated Notifications

* Email triggers on application status changes
* Pluggable email service abstraction
* Stub email provider included for development/testing

### Error Handling & Hardening

* Centralized exception middleware
* Consistent API error responses
* Clean separation of concerns

---

## Architecture

The system follows **Clean Architecture** with clear boundaries:

```
ATS-SYSTEM
│
├── ATS.API            → Web API (Controllers, Auth, Middleware)
├── ATS.Application    → Business logic, Services, DTOs, Interfaces
├── ATS.Domain         → Core domain entities & rules
├── ATS.Infrastructure → EF Core, Repositories, Migrations
└── ATS.Workers        → Background processing (future-ready)
```

### Design Principles

* Dependency inversion
* Domain-driven modeling
* Testability & extensibility
* Infrastructure isolated from business logic

---

## Technology Stack

* **.NET 8 / ASP.NET Core Web API**
* **Entity Framework Core**
* **PostgreSQL** (via Npgsql)
* **Policy-based Authorization**
* **Clean Architecture**
* **GitHub + CI-ready structure**

---

## Database & Migrations

* EF Core migrations included
* Supports:

  * Applications
  * Candidates
  * Jobs
  * Application Notes
  * Audit Logs
  * Events
  * Recruiters & Assignments

Run migrations:

```bash
dotnet ef database update --project server/ATS.Infrastructure --startup-project server/ATS.API
```

---

## Running the Project

### Prerequisites

* .NET SDK 8.0+
* PostgreSQL

### Start API

```bash
cd server/ATS.API
dotnet run
```

Swagger UI:

```
http://localhost:5052/swagger
```

---

## API Capabilities Overview

* `/api/jobs`
* `/api/candidates`
* `/api/applications`
* `/api/applications/{id}/notes`
* `/api/applications/{id}/timeline`
* `/api/applications/{id}/revert`
* `/api/analytics/hiring-funnel`

---

## Testing Strategy (Recommended)

* Unit tests for domain rules
* Integration tests for API endpoints
* Mock repositories & email services

---

## Project Status

**Backend Completion:** ~90–95%

Implemented:

* Full workflow
* Security
* Analytics
* Audit trail
* Notifications

Remaining (Optional Enhancements):

* Background workers for async email
* Rate limiting
* Caching
* Frontend client

---

## Use Cases

* Recruitment platforms
* HR management systems
* Enterprise hiring pipelines
* Portfolio-grade backend project

---

## Author

**Vendas Coder**
Backend / Systems Engineer

---

## License

This project is open-source and available under the **MIT License**.

---

⭐ If you find this project useful, consider starring the repository.
