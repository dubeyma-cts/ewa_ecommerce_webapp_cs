
# Architecture Decision Record (ADR) — Architecture Characteristics & Cross‑Cutting Decisions

**ADR ID:** ADR-001  
**Status:** Proposed  
**Date:** 12 Feb 2026  
**Decision Owners:** Bhatia, Uttam Kumar (Manager – Projects)

This ADR consolidates the architecture‑specific decisions extracted from the requirement documents, guardrails, RTM+STRIDE, and C4 views. All unusual characters and internal citation markers have been removed for clean readability.

---

## 1. Identity, Authentication & Authorization
**Decision:** Adopt risk‑tiered MFA. MFA is mandatory for Admin & Seller roles, and optional for Buyer. Role‑based access control is enforced at the API Gateway/BFF and service layers. Admin exports remain restricted by RBAC.

**Rationale:** Ensures strong protection for privileged operations while maintaining low friction for buyers.

**Consequences:** Requires scope/policy governance and monitoring.

---

## 2. Data Protection, Auditability & Retention
**Decision:** Enforce TLS in transit and encryption at rest (SSE‑KMS). Store secrets in KMS. Maintain immutable audit logs with correlation IDs and at least 12 months of retention. No logging of PII or secrets.

**Rationale:** Ensures confidentiality, integrity, and regulatory compliance.

**Consequences:** Requires audit schema management and storage lifecycle governance.

---

## 3. Availability & Release Strategy
**Decision:** Use Blue/Green or canary deployments with automated smoke tests. Configure health probes (app ~5s, DB ~15s). Plan zonal failover with regional failover as secondary. Target uptime ≥99.9% with zero planned downtime for core flows.

**Consequences:** Enables safer releases and fast rollback.

---

## 4. Performance, Caching & Concurrency
**Decision:** Set p95 latency targets (≤3s UI, ≤500ms API). Use CDN for static assets and cache public GET APIs. Plan for ~10,000 concurrent sessions. Avoid high‑frequency polling; bidding page refreshes every ~5s.

---

## 5. Auction Integrity, Ordering & Idempotency
**Decision:** Enforce minimum bid increment = max(5%, ₹10). Buy‑Now cancels all active bids atomically after successful payment. Implement idempotent and ACID‑compliant writes with exponential backoff for transient failures.

---

## 6. Inventory, Stock & Catalog
**Decision:** Auto‑remove items at quantity zero. Default low‑stock threshold ≤5 (configurable). Support bulk CSV/UI updates. Refresh homepage Top‑5 daily. Use pagination, not infinite scroll.

---

## 7. Seller Lifecycle, Rent & Admin Controls
**Decision:** Activate sellers only after rent payment. Provide 7‑day grace period. Accept PayPal/Cards/Cheque/DD. Enable weekly reminders with three escalation levels.

---

## 8. Observability & Operability
**Decision:** Collect logs, metrics, traces with correlation IDs. Alerts must be tied to SLOs (p95 latency, error rate, uptime). Require runbooks and 100% tracing for checkout, bidding, and payment flows.

---

## 9. Scalability & Capacity Management
**Decision:** Autoscale when CPU ≥70% for 3 minutes or memory ≥75%. Add workers if queue depth >100. Scale‑in only when CPU <40% and stable. Design for ~10k concurrent sessions.

---

## 10. Release Engineering & Contract Testing
**Decision:** Contract tests are blocking for merges and deployments. APIs must be versioned using semantic versioning. Canary/Blue‑Green deployments should include automatic rollback.

---

## 11. Accessibility & UX
**Decision:** Enforce WCAG 2.1 AA accessibility. Use inline validation, proper color contrast, and fully keyboard‑navigable paths for registration, bidding, and purchase flows.

---

## 12. Traceability & Enforcement Points
All controls align with the architecture shown in the C4 views: identity at IdP, policy enforcement at API Gateway and BFF, data protection via KMS and encrypted data stores, audit in immutable log store, and observability pipeline for metrics, logs, and traces.

---

## 13. Open Questions (Require Stakeholder Input)
1. Should Buyer MFA remain optional or use step‑up MFA for high‑value purchases?
2. Should audit retention extend beyond 12 months (e.g., 24–36 months)?
3. Should active‑active multi‑region rollout be targeted for v2 or introduced earlier for bidding/orders?
4. What TTL/ETag strategy should be used for CDN/edge caching of GET APIs?

---

## 14. Exceptions
Any exception to this ADR requires a time‑bound waiver with impact analysis and remediation plan.

---

