# ADR-000X: Data Architecture Strategy — Database per Service, CQRS View Stores, and Saga-Based Consistency

**Status:** Proposed  
**Date:** 12 Feb 2026  
**Decision Owner:** Bhatia, Uttam Kumar (Manager – Projects)

---

## 1. Context

The ecommerce platform is designed as a microservice architecture. Each domain service must own its data and evolve independently.

Microservices.io states that each microservice must own a private database, inaccessible directly from other services, to preserve loose coupling and independent deployability.

Since cross‑service workflows cannot rely on distributed ACID transactions, microservices.io specifies Sagas as the mechanism for ensuring consistency across service boundaries.

Cross‑service read scenarios require CQRS with dedicated view databases to support queries without breaking service encapsulation.

---

## 2. Decision

### 2.1 Adopt “Database per Service” as the persistence strategy
Each microservice will have its own logical database. Includes Catalog, Search, Bidding, Orders, Payments, Inventory, Seller, Notification, Audit.

### 2.2 Implement multi-service consistency using the Saga Pattern
Cross-service workflows (e.g., checkout, Buy‑Now) will use Sagas orchestrated by the Orders Service.

### 2.3 Use CQRS with View Databases for multi-service queries
Materialized view DBs will be created for Order History, Live Bidding Feed, Seller KPIs.

### 2.4 Allow Polyglot Persistence
Choose optimal storage technology per service.

---

## 3. Consequences

### Positive
- Loose coupling and independent deployment.
- High scalability.
- CQRS enables efficient multi-service reads.
- Sagas provide reliable consistency.

### Negative
- Eventual consistency must be accepted.
- More operational overhead.
- Requires event-driven coordination.

---

## 4. Alternatives Considered
- Shared Database (rejected)
- API Composition (rejected)
- Distributed Transactions / 2PC (rejected)

---

## 5. Implementation Plan
1. Create separate schemas/databases.
2. Introduce domain events.
3. Implement Saga orchestrator.
4. Build CQRS view stores.
5. Deploy immutable audit store.
6. Publish data ownership matrix.

---

## 6. References
- Database per Service — microservices.io
- Saga Pattern — microservices.io
- CQRS Pattern — microservices.io
