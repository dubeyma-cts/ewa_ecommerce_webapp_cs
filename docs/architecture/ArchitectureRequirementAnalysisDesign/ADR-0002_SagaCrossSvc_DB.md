# ADR-000Y: Saga Orchestration for Cross-Service Consistency

**Status:** Proposed  
**Date:** 12 Feb 2026  
**Decision Owner:** Bhatia, Uttam Kumar (Manager – Projects)

---

## 1. Context

The ecommerce platform uses a microservice architecture where workflows span multiple services. Once Database per Service is adopted, 2PC is not an option, and microservices.io prescribes Sagas for multi-service consistency. 

## 2. Decision

### 2.1 Adopt Orchestration-Based Saga
Orders Service acts as Saga Orchestrator coordinating cross-service workflows.

### 2.2 Require Idempotent Commands
All participating services must expose idempotent command handlers.

### 2.3 All Multi-Service Workflows Implemented as Orchestrated Sagas

## 3. Consequences

### Positive
- Centralized visibility
- Easier debugging
- Clear compensation steps

### Negative
- Orchestrator becomes critical component
- Requires reliable messaging infrastructure

## 4. Alternatives Considered
- Choreography-Based Saga (rejected)
- 2PC (rejected)
- Synchronous API Composition (rejected)

## 5. Implementation Plan
1. Implement Saga Orchestrator in Orders Service.
2. Define command contracts.
3. Implement compensation handlers.
4. Add domain events.
5. Ensure idempotency.
6. Integrate Outbox Pattern.
7. Add tracing.

## 6. References
- Saga Pattern — microservices.io
