# Agent Instructions - E-Commerce Web Application (Microservices Architecture)

**Project:** BidOrBuy E-Commerce Platform  
**Architecture:** Microservices with .NET 8.0 / C# and Azure Cloud Services  
**Last Updated:** February 18, 2026

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Requirements](#system-requirements)
3. [Architecture Principles](#architecture-principles)
4. [Development Guidelines](#development-guidelines)
5. [Microservices Component Plan](#microservices-component-plan)
6. [Technical Stack](#technical-stack)
7. [Security Requirements](#security-requirements)
8. [Testing Strategy](#testing-strategy)
9. [Deployment & DevOps](#deployment--devops)
10. [Team Collaboration](#team-collaboration)

---

## Project Overview

### Business Context
BidOrBuy is an e-commerce platform enabling:
- **Buyers** to purchase items through bidding or direct purchase
- **Sellers** to setup shops and sell products via auction or fixed-price
- **Administrators** to manage the platform, sellers, and transactions

### Core Business Capabilities
1. **User Management** - Registration, authentication, profile management for Buyers, Sellers, and Admins
2. **Catalog Management** - 13+ product categories with search and filtering
3. **Auction System** - Time-bound bidding with automatic winner selection
4. **Direct Purchase** - Buy-now functionality bypassing auction
5. **Shop Management** - Seller storefronts with inventory management
6. **Payment Processing** - Multiple payment options (PayPal, Credit Card, Cheque, DD, COD)
7. **Order Fulfillment** - Dispatch tracking and delivery management
8. **Admin Controls** - Seller activation, transaction monitoring, rent payment tracking

### Non-Functional Requirements (Critical)
- **Availability:** 99.9% uptime, 24/7 operation, zero planned downtime
- **Performance:** p95 latency ≤3s UI pages, ≤500ms APIs, handle 10k concurrent sessions
- **Scalability:** Auto-scaling, handle heavy traffic load
- **Security:** TLS 1.2+, encryption at rest, MFA for Admin/Seller, RBAC
- **Maintainability:** Clean architecture, SOLID principles, comprehensive documentation
- **Auditability:** Immutable audit logs with 12-month retention

---

## System Requirements

### Technical Constraints
- **.NET Version:** .NET 8.0 or higher
- **Language:** C# 12
- **Cloud Provider:** Microsoft Azure
- **Database:** Azure SQL Database + Azure Cosmos DB (for high-throughput scenarios)
- **Container Orchestration:** Azure Kubernetes Service (AKS)
- **API Style:** RESTful APIs with OpenAPI 3.0 specification
- **Authentication:** Azure AD B2C with OAuth 2.0/OIDC
- **Messaging:** Azure Service Bus for inter-service communication

---

## Architecture Principles

### Microservices Design Principles
1. **Domain-Driven Design (DDD):** Each microservice represents a bounded context
2. **Single Responsibility:** One service per business capability
3. **Database per Service:** Each microservice owns its data store
4. **API Gateway Pattern:** Single entry point for all client requests
5. **Saga Pattern:** Distributed transactions with compensating actions
6. **Event-Driven Communication:** Asynchronous messaging for loose coupling
7. **Circuit Breaker:** Resilience patterns for failure handling
8. **CQRS:** Separate read/write models where appropriate (Catalog, Orders)

### Code Quality Standards
- **SOLID Principles:** Mandatory for all code
- **Clean Architecture:** Separation of concerns (Domain, Application, Infrastructure, Presentation)
- **Dependency Injection:** Use built-in .NET DI container
- **Async/Await:** All I/O operations must be asynchronous
- **Error Handling:** Global exception handling with structured logging
- **Code Coverage:** Minimum 80% unit test coverage per service
- **Code Reviews:** All PRs require 2 approvals minimum

---

## Development Guidelines

### Naming Conventions
- **Namespaces:** `EWA.{ServiceName}.{Layer}` (e.g., `EWA.Catalog.Domain`)
- **Projects:** `{ServiceName}.{Layer}` (e.g., `Catalog.API`, `Catalog.Domain`)
- **Classes:** PascalCase with descriptive names
- **Interfaces:** Prefix with `I` (e.g., `IProductRepository`)
- **Methods:** PascalCase with verb-noun pattern (e.g., `GetProductById`)
- **Variables:** camelCase for local, PascalCase for properties
- **Constants:** ALL_CAPS with underscores

### Git Workflow
- **Branch Strategy:** GitFlow (main, develop, feature/*, release/*, hotfix/*)
- **Feature Branches:** `feature/{ticket-id}-{short-description}`
- **Commit Messages:** `[{ServiceName}] {Type}: {Description}` (e.g., `[Catalog] feat: Add product search`)
- **Types:** feat, fix, refactor, test, docs, chore, perf
- **Pull Requests:** Must pass all CI checks, contract tests, and code reviews

### Logging & Monitoring
- **Structured Logging:** Use Serilog with JSON formatting
- **Correlation IDs:** Track requests across services
- **Log Levels:** ERROR (user-impacting), WARN (degraded), INFO (business events), DEBUG (dev only)
- **Metrics:** Application Insights for telemetry
- **Distributed Tracing:** 100% sampling for checkout, bidding, payment flows
- **Alerts:** Tied to SLOs (p95 latency, error rate <1%, uptime ≥99.9%)

### Error Handling
- **Global Exception Middleware:** Standardized error responses
- **Problem Details (RFC 7807):** Use for API error responses
- **Retry Logic:** Exponential backoff (100ms-2s×3) for transient failures
- **Circuit Breaker:** Polly library with appropriate thresholds
- **User-Facing Messages:** Generic errors, detailed logs in backend

---

## Microservices Component Plan

### 1. **Identity & Access Management Service** (`Identity.API`)
**Responsibility:** User authentication, authorization, and profile management

**Key Features:**
- User registration (Buyer, Seller) with email verification
- Login with JWT token generation
- Multi-Factor Authentication (MFA) for Admin and Seller roles
- Account lockout after 5 failed attempts
- Password reset via email with secure token
- CAPTCHA integration on registration
- Secret question/answer for recovery
- Role-Based Access Control (RBAC)
- User profile CRUD operations
- Integration with Azure AD B2C

**Domain Entities:**
- `User`, `UserProfile`, `Role`, `Permission`, `AuthToken`, `LoginAttempt`, `MfaToken`

**Database:** Azure SQL Database (relational user data)

**External Integrations:**
- Azure AD B2C for identity provider
- SendGrid/Azure Communication Services for email
- reCAPTCHA for bot protection

**API Endpoints:**
```
POST   /api/auth/register/buyer
POST   /api/auth/register/seller
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/refresh-token
POST   /api/auth/forgot-password
POST   /api/auth/reset-password
POST   /api/auth/verify-email
POST   /api/auth/mfa/enable
POST   /api/auth/mfa/verify
GET    /api/users/{id}
PUT    /api/users/{id}
GET    /api/users/{id}/profile
PUT    /api/users/{id}/profile
```

**Technical Considerations:**
- Use IdentityServer or Azure AD B2C for OAuth 2.0/OIDC
- Store password hashes with bcrypt/Argon2
- Implement rate limiting on login endpoint
- Audit all authentication events

---

### 2. **Catalog Service** (`Catalog.API`)
**Responsibility:** Product catalog, categories, search, and item details

**Key Features:**
- Manage 13 product categories (Antiques, Art, Baby Products, Books, Cameras, Mobile Phones, DVDs, Toys, Computers, Watches, Jewelry, Electronics, Home Appliances)
- Product CRUD operations (Seller-owned)
- Product search with filters (category, price, condition, location, bid format)
- Advanced search with multiple criteria
- Top-5 selling items calculation (daily refresh)
- Product image upload and management
- Product condition tracking (New/Used)
- Pagination support
- Low-stock alerts (≤5 default)
- Auto-remove items at quantity zero

**Domain Entities:**
- `Product`, `Category`, `ProductImage`, `ProductAttribute`, `Inventory`, `SellerShop`

**Database:** 
- Azure Cosmos DB (for high-read throughput, search performance)
- Azure Blob Storage (for product images)

**API Endpoints:**
```
GET    /api/categories
```

... (file continues)
