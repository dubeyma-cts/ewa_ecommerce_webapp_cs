````markdown
# Agent Instructions - E-Commerce Web Application (Microservices Architecture)

**Project:** BidOrBuy E-Commerce Platform  
**Architecture:** Microservices with .NET 8.0 / C# and Azure Cloud Services  
**Last Updated:** February 23, 2026

---

> ## ⚠️ CURRENT MODE: PROOF OF CONCEPT (POC)
>
> **All code generated under these instructions is for DEMO and POC purposes only.**
> Hardcoded reference data, in-memory test data, and simplified implementations are intentional and expected.
> Do NOT generate production patterns (real databases, cloud services, secrets management, etc.) until the mode is explicitly changed to Production.
>
> See **[Section 0 — POC Mode & Code Generation Rules](#0-poc-mode--code-generation-rules)** before writing any code.

---

## Table of Contents
0. [POC Mode & Code Generation Rules](#0-poc-mode--code-generation-rules)
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

## 0. POC Mode & Code Generation Rules

> ### 🚨 READ THIS BEFORE GENERATING ANY CODE
>
> These instructions are currently operating in **Proof of Concept (POC) / Demo mode**.
> Anyone — human or AI agent — generating code from this document **must follow the rules in this section**.
> When the project transitions to production, this section will be updated and these rules will be superseded.

---

### 0.1 What POC Mode Means

This codebase is a **structured demonstration** of the BidOrBuy microservices architecture.
The goal is to show working, runnable features for demo and learning purposes — **not** to build a production-grade system.

The following table defines what is expected in POC mode versus what will be required in production:

| Concern | POC Mode (NOW) | Production Mode (FUTURE) |
|---|---|---|
| **Data storage** | Hardcoded in-memory lists | Azure SQL / Cosmos DB |
| **Authentication** | Hardcoded users, demo tokens | Azure AD B2C, JWT with signing keys |
| **Secrets** | None required | Azure Key Vault |
| **Messaging** | Not required | Azure Service Bus |
| **Configuration** | `appsettings.json` | Azure App Config + Key Vault references |
| **Logging** | Console / minimal | Serilog + Application Insights |
| **Error handling** | Basic HTTP status codes | RFC 7807 Problem Details + global middleware |
| **Tests** | Manual / Bruno / curl | xUnit + integration tests (80%+ coverage) |
| **Deployment** | `dotnet run` locally | AKS + CI/CD pipelines |
| **CORS** | `AllowAll` policy | Restricted origins |

---

### 0.2 Code Generation Rules for POC Mode

When generating code **in POC mode**, the agent **MUST**:

1. **Use hardcoded in-memory data** — no database, no ORM, no migrations.
   - Reference/test data is defined directly in the service class or controller as `static readonly` lists or dictionaries.
   - Clearly comment the data as `// Hardcoded demo data — POC only, not for production`.

2. **Keep implementations minimal** — only the code required to make the demo work.
   - No Saga orchestration, no event publishing, no circuit breakers in POC mode.
   - No elaborate layering: a single controller with inline logic is acceptable for a POC endpoint.

3. **Harden nothing** — no password hashing, no token signing, no MFA, no rate limiting.
   - POC tokens are plain strings (e.g. `"demo-token-{guid}"`).

4. **Only touch the relevant microservice** — do not modify other services unless the feature explicitly spans them.

5. **Use session or simple in-memory state** for the web app — no distributed cache, no cookie encryption, no auth middleware.

6. **Add a POC disclaimer comment** at the top of every new file created in POC mode:
   ```csharp
   // ============================================================
   // POC / DEMO CODE — NOT FOR PRODUCTION USE
   // Data is hardcoded. See AGENT_INSTRUCTIONS.md §0 for rules.
   // ============================================================
   ```

7. **Do not generate** the following in POC mode (raise a warning if asked):
   - Real database connection strings or EF Core `DbContext`
   - Azure SDK integrations (Service Bus, Key Vault, Blob Storage, etc.)
   - Real JWT signing/validation logic
   - Docker / Kubernetes manifests
   - CI/CD pipeline files

---

### 0.3 Informing Users

**Any agent generating code under these instructions must prefix its response with the following notice** when producing POC code:

> 📋 **POC Mode Active** — Code generated below follows the Proof of Concept rules in `AGENT_INSTRUCTIONS.md §0`.
> All data is hardcoded for demo purposes. This is **not production-ready code**.

---

### 0.4 Transitioning Out of POC Mode

When the team decides to move to production implementation:

1. Update the banner at the top of this file and this section heading to `PRODUCTION MODE`.
2. Remove the POC disclaimer from the TOC.
3. Replace this section with a link to the production architecture ADRs.
4. All previously generated POC files must be reviewed, replaced, or removed before any production deployment.

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
GET    /api/categories/{id}/products
GET    /api/products/{id}
GET    /api/products/search?q={query}&category={cat}&minPrice={min}&maxPrice={max}
GET    /api/products/top-selling?limit=5
POST   /api/products (Seller only)
PUT    /api/products/{id} (Seller only)
DELETE /api/products/{id} (Seller only)
POST   /api/products/{id}/images
GET    /api/products/seller/{sellerId}
PATCH  /api/products/{id}/inventory
```

**Technical Considerations:**
- Implement read-through cache with Redis for popular products
- Use Azure Cognitive Search for advanced search capabilities
- CDN for product image delivery
- Event publishing on product changes (for Auction, Order services)
- CQRS pattern with separate read/write models

---

### 3. **Auction & Bidding Service** (`Auction.API`)
**Responsibility:** Auction lifecycle, bid management, and winner selection

**Key Features:**
- Create auction for products with start/end date-time
- Place bid with validation (increment ≥ max(5%, ₹10))
- Real-time current highest bid tracking
- Minimum threshold price enforcement
- Automatic winner selection on auction end
- Bid cancellation when Buy Now executed
- Bid status tracking (Active/Expired)
- Watch list functionality
- Bid history and current bids for buyer
- Auction expiry handling (with/without winner)
- No automatic proxy bidding (MVP)

**Domain Entities:**
- `Auction`, `Bid`, `BidIncrement`, `WatchListItem`, `AuctionResult`

**Database:** Azure SQL Database (ACID transactions for bids)

**Real-time:** Azure SignalR Service for live bid updates

**API Endpoints:**
```
POST   /api/auctions (Seller only)
GET    /api/auctions/{id}
POST   /api/auctions/{id}/bids
GET    /api/auctions/{id}/bids
GET    /api/auctions/{id}/current-highest-bid
PUT    /api/auctions/{id}/cancel (when Buy Now)
GET    /api/users/{userId}/bids/current
GET    /api/users/{userId}/watchlist
POST   /api/users/{userId}/watchlist
DELETE /api/users/{userId}/watchlist/{productId}
GET    /api/auctions/ending-soon?hours=24
```

**Background Jobs:**
- Auction end processor (every minute)
- Winner notification
- Bid expiry handler

**Technical Considerations:**
- Optimistic concurrency for bid placement
- Idempotency tokens for bid operations
- SignalR for real-time bid updates
- Event-driven: Publish auction events to Service Bus
- Saga orchestration for Buy Now canceling bids

---

### 4. **Order Management Service** (`Order.API`)
**Responsibility:** Order creation, payment tracking, and fulfillment

**Key Features:**
- Create order from auction win or Buy Now
- Multiple payment method support (PayPal, Credit Card, Cheque, DD, COD)
- Payment status tracking
- Shipping address management (editable before payment only)
- Order status workflow (Created → Paid → Dispatched → Delivered)
- Order history for buyers and sellers
- Price comparison for direct buy
- Idempotent order creation
- Automatic inventory decrement on dispatch

**Domain Entities:**
- `Order`, `OrderItem`, `PaymentInfo`, `ShippingAddress`, `OrderStatus`, `Transaction`

**Database:** Azure SQL Database

**API Endpoints:**
```
POST   /api/orders
GET    /api/orders/{id}
PUT    /api/orders/{id}/payment
PUT    /api/orders/{id}/shipping-address (before payment only)
GET    /api/orders/buyer/{buyerId}
GET    /api/orders/seller/{sellerId}
PATCH  /api/orders/{id}/status
GET    /api/orders/compare-prices?productIds={ids}
GET    /api/transactions/history?startDate={start}&endDate={end}
```

**Saga Orchestration:**
- **Buy Now Saga:** Cancel bids → Create order → Reserve inventory → Process payment
- **Dispatch Saga:** Update order status → Decrement inventory → Send notification

**Technical Considerations:**
- Event sourcing for order state changes
- Saga pattern with compensating transactions
- Integration with Payment Gateway Service
- Integration with Notification Service
- Emit domain events for order lifecycle

---

### 5. **Payment Gateway Service** (`Payment.API`)
**Responsibility:** Payment processing, gateway integration, and transaction security

**Key Features:**
- PayPal integration
- Credit card processing via Stripe/Azure Payment Gateway
- Cheque and DD payment tracking
- Cash on Delivery (COD) support
- PCI-DSS compliance
- Payment retry logic
- Refund processing
- Payment verification
- Webhook handling for payment providers

**Domain Entities:**
- `Payment`, `PaymentMethod`, `PaymentTransaction`, `RefundRequest`, `PaymentWebhook`

**Database:** Azure SQL Database (encrypted payment records)

**API Endpoints:**
```
POST   /api/payments/initiate
POST   /api/payments/{id}/confirm
GET    /api/payments/{id}/status
POST   /api/payments/{id}/refund
POST   /api/webhooks/paypal
POST   /api/webhooks/stripe
GET    /api/payments/order/{orderId}
```

**Technical Considerations:**
- Never store raw card details (use tokenization)
- Encrypt payment data at rest (SSE-KMS)
- TLS 1.2+ for all communications
- Idempotency for payment operations
- Exponential backoff for gateway retries
- Audit all payment transactions
- Compliance: PCI-DSS Level 1

---

### 6. **Shop Management Service** (`Shop.API`)
**Responsibility:** Seller shop setup, inventory, and product management

**Key Features:**
- Shop creation and configuration
- Shop profile (name, description, payment options)
- Product inventory management
- Bulk stock updates (CSV upload)
- Low-stock alerts
- Item dispatch tracking
- Quantity auto-decrement on dispatch
- Auto-removal at zero quantity
- Item statistics and analytics
- Shop item listing with status

**Domain Entities:**
- `Shop`, `ShopConfiguration`, `InventoryItem`, `StockAlert`, `DispatchRecord`

**Database:** Azure SQL Database

**Storage:** Azure Blob Storage (for CSV uploads)

**API Endpoints:**
```
POST   /api/shops (Seller only)
GET    /api/shops/{id}
PUT    /api/shops/{id}
GET    /api/shops/{id}/inventory
POST   /api/shops/{id}/inventory/bulk-update
PATCH  /api/shops/{id}/inventory/{productId}
GET    /api/shops/{id}/items/status
POST   /api/shops/{id}/items/dispatch
GET    /api/shops/{id}/alerts/low-stock
GET    /api/shops/seller/{sellerId}
```

**Background Jobs:**
- Low-stock alert checker (daily)
- Zero-quantity item removal (hourly)
- Dispatch reminder notifications

**Technical Considerations:**
- Integration with Catalog Service for product sync
- Event publishing on inventory changes
- CSV validation for bulk updates
- Optimistic concurrency for inventory updates

---

### 7. **Seller Account Service** (`SellerAccount.API`)
**Responsibility:** Seller registration, activation, and rent management

**Key Features:**
- Seller registration with extended profile
- Rent payment plans (monthly, quarterly, yearly)
- Payment tracking for rent
- Account activation workflow (pending → active → suspended)
- 7-day grace period for late payments
- Rent reminder notifications (weekly, 3-level escalation)
- Seller status management by admin
- Rent default tracking
- Integration with Payment Gateway for rent

**Domain Entities:**
- `SellerAccount`, `RentPayment`, `PaymentPlan`, `AccountStatus`, `RentReminder`

**Database:** Azure SQL Database

**API Endpoints:**
```
POST   /api/seller-accounts
GET    /api/seller-accounts/{id}
PUT    /api/seller-accounts/{id}
POST   /api/seller-accounts/{id}/rent-payment
GET    /api/seller-accounts/{id}/rent-history
PATCH  /api/seller-accounts/{id}/status (Admin only)
GET    /api/seller-accounts/defaults (Admin only)
GET    /api/seller-accounts/pending-activation (Admin only)
```

**Background Jobs:**
- Rent due reminder scheduler
- Grace period expiry checker
- Escalation notification sender

**Technical Considerations:**
- State machine for account status transitions
- Event publishing on status changes
- Integration with Notification Service
- Admin approval workflow

---

### 8. **Notification Service** (`Notification.API`)
**Responsibility:** Multi-channel notifications (email, SMS, in-app)

**Key Features:**
- Email notifications (registration, bid updates, order confirmations, etc.)
- SMS notifications (optional, for high-value transactions)
- In-app notifications
- Notification templates management
- Notification preferences per user
- Delivery tracking and retry logic
- Notification history

**Domain Entities:**
- `Notification`, `NotificationTemplate`, `DeliveryStatus`, `UserPreference`

**Database:** Azure Cosmos DB (high-write throughput)

**External Services:**
- SendGrid or Azure Communication Services (email)
- Twilio or Azure Communication Services (SMS)

**Notification Events:**
- User registration confirmation
- Seller account activation
- Bid placed/outbid notification
- Auction won/lost
- Order created/paid/dispatched/delivered
- Low stock alerts
- Rent payment reminders
- Password reset

**API Endpoints:**
```
POST   /api/notifications/send
GET    /api/notifications/user/{userId}
PUT    /api/notifications/{id}/read
GET    /api/notifications/preferences/{userId}
PUT    /api/notifications/preferences/{userId}
```

**Technical Considerations:**
- Message queue consumer (Azure Service Bus)
- Async processing with retry logic
- Template engine (e.g., Handlebars)
- Rate limiting per channel
- Unsubscribe functionality

---

### 9. **Admin & Reporting Service** (`Admin.API`)
**Responsibility:** Platform administration, analytics, and reporting

**Key Features:**
- Seller account activation/deactivation
- Transaction history viewing (date range, filters)
- Rent default monitoring
- Platform analytics dashboard
- User management (view, suspend)
- Audit log access
- System health monitoring
- Revenue reports
- Category performance analytics

**Domain Entities:**
- `AdminUser`, `AuditLog`, `Report`, `PlatformMetrics`, `TransactionSummary`

**Database:** 
- Azure SQL Database (operational data)
- Azure Synapse Analytics (data warehouse for reporting)

**API Endpoints:**
```
POST   /api/admin/sellers/{id}/activate
POST   /api/admin/sellers/{id}/deactivate
GET    /api/admin/transactions?startDate={start}&endDate={end}
GET    /api/admin/sellers/defaults
GET    /api/admin/reports/revenue?period={period}
GET    /api/admin/reports/category-performance
GET    /api/admin/users?status={status}&role={role}
GET    /api/admin/audit-logs?userId={id}&action={action}
GET    /api/admin/metrics/platform
```

**Technical Considerations:**
- Read-only replicas for reporting queries
- ETL pipelines to data warehouse
- Role-based access (Admin only)
- Audit all admin actions
- Data aggregation for performance

---

### 10. **API Gateway** (`Gateway.API`)
**Responsibility:** Single entry point, routing, rate limiting, and cross-cutting concerns

**Key Features:**
- Request routing to backend services
- Authentication/Authorization (JWT validation)
- Rate limiting per user/IP
- Request/response logging
- CORS configuration
- API versioning
- Response caching
- Request transformation
- Circuit breaker for backend services
- Global error handling

**Technology:** Azure API Management (APIM) or Ocelot

**Configuration:**
- Routes to all microservices
- Authentication policies
- Rate limiting policies (e.g., 100 req/min per user)
- Caching policies for GET endpoints
- Retry and timeout policies

**Cross-Cutting Concerns:**
- Correlation ID injection
- Request tracing
- Security headers (CSP, HSTS, X-Frame-Options)
- Swagger/OpenAPI aggregation

---

### 11. **Search & Analytics Service** (`Search.API`)
**Responsibility:** Full-text search, filtering, and search analytics

**Key Features:**
- Product full-text search
- Seller search
- Advanced search with multiple filters
- Search suggestions/autocomplete
- Search history per user
- Popular search terms analytics
- Search result ranking
- Faceted search (category, price range, condition, location)

**Technology:** Azure Cognitive Search

**Database:** Azure Cosmos DB (search history)

**API Endpoints:**
```
GET    /api/search/products?q={query}&filters={filters}
GET    /api/search/sellers?q={query}
GET    /api/search/suggestions?q={query}
GET    /api/search/history/{userId}
GET    /api/search/trending
```

**Technical Considerations:**
- Index products from Catalog Service
- Real-time index updates via events
- Search result caching
- Synonym support
- Stemming and fuzzy matching

---

### 12. **Shared Libraries & Infrastructure**

#### **Shared.Common** (Class Library)
- Base entities and value objects
- Common DTOs and view models
- Extension methods
- Constants and enumerations
- Validation attributes

#### **Shared.Messaging** (Class Library)
- Event definitions (IEvent, IEventHandler)
- Message broker abstraction (Azure Service Bus)
- Event bus implementation
- Retry and dead-letter handling

#### **Shared.Observability** (Class Library)
- Logging configuration (Serilog)
- Correlation ID middleware
- Application Insights integration
- Health check endpoints
- Metrics collection

#### **Shared.Security** (Class Library)
- JWT token generation/validation
- Encryption/decryption utilities
- Password hashing
- Rate limiting middleware
- CORS configuration

#### **Shared.Testing** (Class Library)
- Test fixtures
- Mock factories
- Integration test base classes
- Contract testing utilities

---

## Technical Stack

### Backend Services
- **.NET 8.0** - Runtime and SDK
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 8** - ORM for SQL databases
- **Dapper** - Micro-ORM for high-performance queries
- **Azure Cosmos DB SDK** - For NoSQL operations
- **MassTransit** - Message broker abstraction
- **Polly** - Resilience and transient fault handling
- **FluentValidation** - Input validation
- **AutoMapper** - Object-to-object mapping
- **MediatR** - CQRS and mediator pattern
- **Swagger/OpenAPI** - API documentation

### Data Stores
- **Azure SQL Database** - Relational data (users, orders, transactions)
- **Azure Cosmos DB** - High-throughput reads (catalog, notifications)
- **Azure Cache for Redis** - Distributed caching
- **Azure Blob Storage** - File storage (images, CSV files)

### Messaging & Events
- **Azure Service Bus** - Inter-service messaging
- **Azure SignalR Service** - Real-time communication (bid updates)
- **Azure Event Grid** - Event routing

### Security & Identity
- **Azure AD B2C** - Identity provider
- **IdentityServer** - OAuth 2.0/OIDC (if not using AD B2C)
- **Azure Key Vault** - Secrets and certificate management

### Observability
- **Azure Application Insights** - APM and telemetry
- **Serilog** - Structured logging
- **Azure Log Analytics** - Log aggregation
- **Azure Monitor** - Metrics and alerting

### DevOps & Infrastructure
- **Azure Kubernetes Service (AKS)** - Container orchestration
- **Docker** - Containerization
- **Helm** - Kubernetes package management
- **Azure DevOps / GitHub Actions** - CI/CD pipelines
- **Terraform** - Infrastructure as Code
- **Azure Container Registry (ACR)** - Container image registry

### Frontend (Web)
- **Blazor Server / Blazor WebAssembly** - .NET-based SPA
- **SignalR Client** - Real-time updates
- **Bootstrap 5** - UI framework

### Testing
- **xUnit** - Unit testing framework
- **Moq** - Mocking library
- **FluentAssertions** - Assertion library
- **Testcontainers** - Integration testing with containers
- **SpecFlow** - BDD testing
- **k6 / Azure Load Testing** - Performance testing
- **Pact** - Contract testing

---

## Security Requirements

### Authentication & Authorization
1. **JWT Tokens:** Use short-lived access tokens (15 min) and refresh tokens (7 days)
2. **MFA:** Mandatory for Admin and Seller roles; TOTP-based (Google Authenticator)
3. **Password Policy:** Minimum 12 characters, complexity requirements, no common passwords
4. **Account Lockout:** 5 failed attempts → 15-minute lockout
5. **RBAC:** Role-based access control with permissions (Admin, Seller, Buyer)
6. **Session Management:** Secure session handling, logout invalidates tokens

### Data Protection
1. **TLS 1.2+:** Enforce HTTPS for all communications
2. **Encryption at Rest:** Azure SQL TDE, SSE-KMS for blob storage
3. **PII Masking:** No PII in logs; mask sensitive data in responses
4. **Data Classification:** Categorize data (Public, Internal, Confidential, Restricted)
5. **Secure Secrets:** Store in Azure Key Vault, rotate periodically

### Input Validation & Output Encoding
1. **Validation:** Server-side validation for all inputs (FluentValidation)
2. **Sanitization:** HTML encoding, SQL parameterization
3. **CSRF Protection:** Anti-forgery tokens for state-changing operations
4. **SQL Injection:** Use parameterized queries or ORM
5. **XSS Protection:** Content Security Policy, output encoding

### API Security
1. **Rate Limiting:** 100 requests/min per user, 1000/min per IP
2. **CORS:** Whitelist trusted origins
3. **API Versioning:** Support multiple versions, deprecation strategy
4. **Input Size Limits:** Max payload 10MB, max query params 50
5. **Security Headers:** HSTS, X-Content-Type-Options, X-Frame-Options

### Audit & Compliance
1. **Audit Logging:** Immutable logs for all critical operations
2. **Correlation IDs:** Track requests across services
3. **Retention:** 12-month minimum for audit logs
4. **PCI-DSS:** Compliance for payment data handling
5. **GDPR:** User data export and deletion capabilities

---

## Testing Strategy

### Unit Testing (80% coverage minimum)
- Test domain logic in isolation
- Mock external dependencies
- Fast execution (<5 sec per service)
- Run on every commit in CI pipeline

**Tools:** xUnit, Moq, FluentAssertions

### Integration Testing
- Test API endpoints end-to-end
- Use Testcontainers for databases
- Test service integrations
- Run in dedicated test environment

**Tools:** xUnit, Testcontainers, WebApplicationFactory

### Contract Testing
- Define contracts between services
- Consumer-driven contract tests
- Blocking for merges/deploys
- Versioned contracts

**Tools:** Pact, Pact Broker

### Performance Testing
- Load testing: 10k concurrent users
- Spike testing: 3x normal load
- Soak testing: 24-hour sustained load
- SLO validation: p95 latency ≤3s

**Tools:** k6, Azure Load Testing, JMeter

### Security Testing
- OWASP Top 10 vulnerability scanning
- Penetration testing (quarterly)
- Dependency scanning (automated)
- Static code analysis (SonarQube)

**Tools:** OWASP ZAP, SonarQube, Snyk, Dependabot

### E2E Testing
- Critical user journeys (registration → purchase)
- Cross-browser testing (Chrome, Firefox, Edge, Safari)
- Mobile responsiveness

**Tools:** Playwright, Selenium

### Accessibility Testing
- WCAG 2.1 AA compliance
- Keyboard navigation
- Screen reader compatibility
- Color contrast validation

**Tools:** Axe, Lighthouse

---

## Deployment & DevOps

### CI/CD Pipeline
1. **Build Stage:** Compile, restore packages, build Docker images
2. **Test Stage:** Unit tests, integration tests, contract tests
3. **Analysis Stage:** Code quality (SonarQube), security scan
4. **Publish Stage:** Push images to ACR
5. **Deploy Stage:** Deploy to environments (Dev → QA → Staging → Prod)
6. **Smoke Test Stage:** Automated smoke tests after deployment
7. **Performance Stage:** Load tests (weekly in staging)

### Deployment Strategy
- **Blue/Green Deployment:** Zero-downtime releases
- **Canary Releases:** 5% traffic → 25% → 50% → 100%
- **Automated Rollback:** On health check failures or error rate spike
- **Feature Flags:** LaunchDarkly or Azure App Configuration

### Infrastructure as Code
- **Terraform:** Provision Azure resources
- **Helm Charts:** Deploy microservices to AKS
- **GitOps:** Declarative infrastructure in Git

### Environments
1. **Development:** Developer local (Docker Compose) + shared Dev environment
2. **QA:** Integration testing environment
3. **Staging:** Production-like environment for final validation
4. **Production:** Multi-region deployment (primary + DR)

### Monitoring & Alerting
- **Health Checks:** Liveness and readiness probes (5s interval)
- **SLI/SLO Dashboards:** Real-time monitoring
- **Alerts:** Slack/Teams integration for critical alerts
- **Runbooks:** Linked to every alert for troubleshooting
- **Chaos Engineering:** Monthly chaos tests (kill pods, network latency)

---

## Team Collaboration

### Communication
- **Daily Standups:** 15-minute sync per team
- **Sprint Planning:** Bi-weekly sprints (2 weeks)
- **Retrospectives:** End of each sprint
- **Code Reviews:** Within 24 hours, 2 approvals required
- **Documentation:** Confluence/Wiki for architecture decisions

### Ownership
- Each microservice has a dedicated team (2-4 developers)
- Service owner responsible for on-call rotation
- Shared libraries owned by platform team

### Onboarding
1. Read this document thoroughly
2. Setup local development environment (Docker Desktop, .NET 8 SDK)
3. Clone repository and review project structure
4. Complete "Hello World" microservice tutorial
5. Pair programming with senior developer for first task

### Development Workflow
1. Pick task from backlog (Azure DevOps / Jira)
2. Create feature branch from `develop`
3. Implement changes following guidelines
4. Write tests (unit + integration)
5. Commit with descriptive message
6. Push and create Pull Request
7. Address code review feedback
8. Merge after CI passes and approvals received
9. Delete feature branch

### Copilot Usage
- Use GitHub Copilot for code generation
- Review all generated code for quality and security
- Customize suggestions to follow team conventions
- Use Copilot Chat for architecture questions referencing this document

---

## Development Checklist (Per Service)

### Before Starting
- [ ] Understand the business domain and requirements
- [ ] Review this document and relevant ADRs
- [ ] Understand service boundaries and dependencies
- [ ] Review API contract with dependent services

### During Development
- [ ] Follow Clean Architecture structure
- [ ] Implement SOLID principles
- [ ] Use dependency injection
- [ ] Add comprehensive logging with correlation IDs
- [ ] Implement health check endpoints
- [ ] Add OpenAPI/Swagger documentation
- [ ] Write unit tests (80% coverage)
- [ ] Write integration tests
- [ ] Define and publish domain events
- [ ] Handle errors gracefully with retry logic
- [ ] Validate all inputs
- [ ] Add security headers and authentication

### Before Pull Request
- [ ] All tests pass locally
- [ ] Code follows naming conventions
- [ ] No hardcoded secrets or configurations
- [ ] Add/update README for service
- [ ] Update OpenAPI spec if API changed
- [ ] Run code quality analyzer (SonarLint)
- [ ] Check for security vulnerabilities
- [ ] Performance tested (if applicable)

### After Deployment
- [ ] Verify health checks are green
- [ ] Monitor metrics and logs
- [ ] Validate SLO compliance
- [ ] Update runbooks if needed

---

## Service Communication Matrix

| Service | Depends On | Communicates Via |
|---------|-----------|------------------|
| Identity.API | - | - |
| Catalog.API | Identity.API | HTTP (Auth) + Events |
| Auction.API | Identity.API, Catalog.API | HTTP + Events |
| Order.API | Identity.API, Catalog.API, Auction.API, Payment.API | HTTP + Saga (Events) |
| Payment.API | Identity.API | HTTP + Webhooks |
| Shop.API | Identity.API, Catalog.API | HTTP + Events |
| SellerAccount.API | Identity.API, Payment.API | HTTP + Events |
| Notification.API | (All services) | Events (Consumer) |
| Admin.API | Identity.API, SellerAccount.API, Order.API | HTTP |
| Search.API | Catalog.API, Shop.API | Events (Index sync) |
| Gateway.API | (All services) | HTTP Proxy |

---

## API Gateway Route Configuration

```
GET    /api/auth/*          → Identity.API
GET    /api/users/*         → Identity.API
POST   /api/auth/*          → Identity.API

GET    /api/categories/*    → Catalog.API
GET    /api/products/*      → Catalog.API
POST   /api/products/*      → Catalog.API (Seller)

GET    /api/auctions/*      → Auction.API
POST   /api/auctions/*      → Auction.API

GET    /api/orders/*        → Order.API
POST   /api/orders/*        → Order.API

POST   /api/payments/*      → Payment.API
GET    /api/payments/*      → Payment.API

GET    /api/shops/*         → Shop.API
POST   /api/shops/*         → Shop.API

GET    /api/seller-accounts/* → SellerAccount.API
POST   /api/seller-accounts/* → SellerAccount.API

GET    /api/notifications/* → Notification.API
POST   /api/notifications/* → Notification.API

GET    /api/admin/*         → Admin.API
POST   /api/admin/*         → Admin.API (Admin only)

GET    /api/search/*        → Search.API
```

---

## Event-Driven Architecture

### Event Publishing (Service Bus Topics)

| Event | Published By | Subscribed By |
|-------|-------------|---------------|
| `UserRegistered` | Identity.API | Notification.API |
| `ProductCreated` | Catalog.API | Search.API, Auction.API |
| `ProductUpdated` | Catalog.API | Search.API |
| `InventoryChanged` | Shop.API | Catalog.API, Notification.API |
| `AuctionStarted` | Auction.API | Notification.API |
| `BidPlaced` | Auction.API | Notification.API |
| `AuctionEnded` | Auction.API | Order.API, Notification.API |
| `OrderCreated` | Order.API | Notification.API, Catalog.API |
| `PaymentCompleted` | Payment.API | Order.API, Notification.API |
| `OrderDispatched` | Order.API | Shop.API, Notification.API |
| `SellerActivated` | SellerAccount.API | Shop.API, Notification.API |
| `RentDue` | SellerAccount.API | Notification.API |

---

## Database Schema Guidelines

### Naming Conventions
- **Tables:** PascalCase, plural (e.g., `Users`, `Products`, `Orders`)
- **Columns:** PascalCase (e.g., `FirstName`, `CreatedAt`)
- **Primary Keys:** `Id` (int/guid)
- **Foreign Keys:** `{Entity}Id` (e.g., `UserId`, `ProductId`)
- **Indexes:** `IX_{TableName}_{ColumnName}`
- **Constraints:** `CK_{TableName}_{ColumnName}` (check), `FK_{TableName}_{ForeignTableName}` (foreign key)

### Audit Columns (All tables)
- `CreatedAt` (datetime2) - When record was created
- `CreatedBy` (nvarchar) - User who created
- `UpdatedAt` (datetime2) - Last update timestamp
- `UpdatedBy` (nvarchar) - User who last updated
- `IsDeleted` (bit) - Soft delete flag
- `DeletedAt` (datetime2) - Deletion timestamp
- `RowVersion` (rowversion) - Optimistic concurrency

### Performance Optimization
- Index frequently queried columns
- Avoid N+1 queries (use eager loading)
- Partition large tables (Orders, Audit Logs)
- Archive old data (>1 year to cold storage)

---

## Sample Project Structure (Per Service)

```
Catalog.API/
├── Controllers/           # API endpoints
├── Middleware/           # Custom middleware
├── Program.cs            # Startup and DI configuration
├── appsettings.json      # Configuration
├── Dockerfile            # Container definition

Catalog.Application/      # Application layer
├── Commands/            # Write operations (CQRS)
├── Queries/             # Read operations (CQRS)
├── DTOs/                # Data transfer objects
├── Validators/          # FluentValidation rules
├── Interfaces/          # Application interfaces
├── Services/            # Application services
├── EventHandlers/       # Domain event handlers
├── Mappings/            # AutoMapper profiles

Catalog.Domain/           # Domain layer (core business logic)
├── Entities/            # Domain entities
├── ValueObjects/        # Value objects
├── Aggregates/          # Aggregate roots
├── Events/              # Domain events
├── Interfaces/          # Repository interfaces
├── Specifications/      # Specification pattern
├── Exceptions/          # Domain exceptions

Catalog.Infrastructure/   # Infrastructure layer
├── Persistence/         # EF Core DbContext, Repositories
│   ├── Configurations/  # Entity type configurations
│   ├── Migrations/      # EF Core migrations
│   └── Repositories/    # Repository implementations
├── Messaging/           # Service Bus integration
├── Caching/             # Redis cache implementation
├── Services/            # External service clients
└── Logging/             # Logging configuration

Catalog.Tests/            # Test projects
├── Unit/                # Unit tests
├── Integration/         # Integration tests
├── Contract/            # Contract tests
└── Fixtures/            # Test fixtures and helpers
```

---

## Configuration Management

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;",
    "RedisConnection": "..."
  },
  "AzureServiceBus": {
    "ConnectionString": "...",
    "QueueName": "..."
  },
  "Authentication": {
    "Authority": "...",
    "Audience": "..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ApplicationInsights": {
    "InstrumentationKey": "..."
  }
}
```

### Environment Variables (Kubernetes Secrets)
- Database connection strings
- Service Bus connection strings
- Azure Key Vault URL
- Application Insights key
- OAuth client secrets

---

## Performance Targets

| Metric | Target | Measurement |
|--------|--------|-------------|
| API Response Time (p95) | ≤500ms | Application Insights |
| Page Load Time (p95) | ≤3s | Browser timing |
| Database Query Time (p95) | ≤200ms | SQL Insights |
| Availability | ≥99.9% | Uptime monitoring |
| Error Rate | <1% | Application Insights |
| Concurrent Users | 10,000 | Load testing |
| Throughput | 1,000 req/sec | Load testing |
| Cache Hit Ratio | ≥80% | Redis metrics |

---

## Glossary

- **ADR:** Architecture Decision Record
- **APIM:** Azure API Management
- **BRD:** Business Requirements Document
- **CQRS:** Command Query Responsibility Segregation
- **DDD:** Domain-Driven Design
- **DTO:** Data Transfer Object
- **EDA:** Event-Driven Architecture
- **GDPR:** General Data Protection Regulation
- **JWT:** JSON Web Token
- **MFA:** Multi-Factor Authentication
- **OWASP:** Open Web Application Security Project
- **PCI-DSS:** Payment Card Industry Data Security Standard
- **RBAC:** Role-Based Access Control
- **SOLID:** Single responsibility, Open-closed, Liskov substitution, Interface segregation, Dependency inversion
- **SLI:** Service Level Indicator
- **SLO:** Service Level Objective
- **TDE:** Transparent Data Encryption
- **WCAG:** Web Content Accessibility Guidelines

---

## References

- [Case Study Document](./case_study_3_ewa.md)
- [Architectural Guardrails](./ewa_architectural_guardrails.md)
- [Architecture Decision Records](./docs/adr/)
- [API Documentation](./docs/api/)
- [Runbooks](./docs/runbooks/)

 - [Case Study Document](./case_study_3_ewa.md)
 - [Architectural Guardrails](./docs/architecture/ArchitectureRequirementAnalysisDesign/Architectural_Guardrails.md)
 - [Architecture Decision Records](./docs/adr/)
 - [Docs Root](./docs/README.md)

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-02-18 | AI Agent | Initial version with complete microservices plan |
| 1.1 | 2026-02-23 | AI Agent | Added Section 0 — POC Mode & Code Generation Rules; updated TOC and header banner |

---

**END OF AGENT INSTRUCTIONS**

> **Note to Developers:** This document is the single source of truth for developing the BidOrBuy E-Commerce platform. Always refer to this document when implementing features. If you encounter conflicts or ambiguities, raise them with the architecture team immediately. Use GitHub Copilot effectively by referencing specific sections of this document in your prompts.
````
