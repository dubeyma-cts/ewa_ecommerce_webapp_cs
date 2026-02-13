# ADR-000Z: Transactional Outbox Pattern for Reliable Event Publishing

**Status:** Proposed  
**Date:** 12 Feb 2026  
**Decision Owner:** Bhatia, Uttam Kumar (Manager – Projects)

---

## 1. Context

The ecommerce platform uses **Database per Service**. Services must persist domain changes **and** publish domain events/commands to a broker as part of Saga-driven workflows. Traditional two‑phase commit (2PC) between the database and the broker is discouraged and often unsupported in microservices; direct publish inside the business transaction is unreliable (crash windows lead to lost/phantom events). The **Transactional Outbox** pattern addresses this by persisting an event in an outbox **in the same transaction** as the aggregate change and later relaying it to the broker. citeturn10search40

Because Sagas require reliable step events to progress or compensate, Outbox becomes a foundational building block for our cross‑service consistency strategy. citeturn10search41

---

## 2. Decision

### 2.1 Adopt Transactional Outbox for all services that publish domain events
For any mutation that must emit an event, the service will:
1) Write the event to an **Outbox table** within the same DB transaction as the business update;  
2) Use a **relay** to publish pending outbox entries to the message broker;  
3) Mark entries as sent (or delete) after broker acknowledgement.  
This guarantees atomic persistence of data + event without 2PC. citeturn10search40

### 2.2 Delivery mechanism: Polling Publisher or CDC
Use **Polling Publisher** (simple SQL polling) by default; high‑throughput services may use **Change Data Capture (CDC)**. Both are recognized approaches for emitting outbox messages. citeturn10search44

### 2.3 Downstream idempotency
Consumers must be **idempotent** because Outbox is at‑least‑once delivery; deduplicate via message IDs or idempotency keys. citeturn10search44

---

## 3. Consequences

### Positive
- **Reliable event publishing** even across crashes/restarts. citeturn10search40  
- Eliminates need for **2PC** while keeping atomicity of data + event. citeturn10search40  
- Works with **any SQL database**; simple to operate with polling. citeturn10search44  
- Enables robust **Saga orchestration** by guaranteeing messages are eventually delivered. citeturn10search41

### Negative
- Additional operational components (outbox table, relay/CDC, monitoring). citeturn10search44  
- **At‑least‑once** semantics; requires consumer idempotency and possibly ordering controls per aggregate. citeturn10search44

---

## 4. Alternatives Considered

### ❌ Direct publish within the transaction
Rejected: cannot atomically coordinate DB and broker; risks lost/phantom events. citeturn10search40

### ❌ Two‑Phase Commit (2PC)
Rejected: not viable/undesirable in microservices due to coupling/availability; often unsupported. citeturn10search40

### ❌ Choreography without guaranteed delivery
Rejected: Saga steps depend on reliable delivery; lack of guarantees breaks consistency. citeturn10search41

---

## 5. Implementation Plan
1. Add **Outbox** table per publishing service; include fields: `id`, `aggregateId`, `type`, `payload`, `headers`, `createdAt`, `status`. citeturn10search40  
2. Persist outbox entries **in the same DB transaction** as domain writes. citeturn10search40  
3. Implement **Polling Publisher** (batch polling + publish + mark sent); consider **CDC** for higher scale. citeturn10search44  
4. Enforce **idempotent consumers** (dedup keys), and monitor outbox backlog/lag. citeturn10search44  
5. Establish **cleanup/TTL** strategy for processed outbox rows. citeturn10search44

---

## 6. References
- **Transactional Outbox — microservices.io** (problem/solution, atomic DB + outbox, relay) citeturn10search40  
- **Polling Publisher — microservices.io** (publishing mechanism, benefits/limitations) citeturn10search44  
- **Saga — microservices.io** (need for reliable step events in microservices) citeturn10search41  
