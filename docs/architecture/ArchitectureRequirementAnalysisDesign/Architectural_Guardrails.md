
# Architectural Guardrails — Ecommerce Web Application

**Generated on:** 12 Feb 2026

These guardrails translate the **BRD**, **Functional & Non‑Functional Use‑Cases**, and the confirmed **RTM & STRIDE** mappings into **prescriptive, testable architecture constraints**. They are written to be:

- **Actionable** (clear _must/should_ statements),  
- **Traceable** (each rule maps to use‑cases/RTM rows), and  
- **Verifiable** (each rule has a verification method / SLO / control).  

> **Sources**: Functional Use‑Cases (UC1–UC21), NFR Use‑Cases (UC‑NFR1–UC‑NFR8), Confirmed Clarifications, and RTM + STRIDE mappings. 

---

## 1. Identity, Authentication & Authorization

1. **MFA is mandatory for Admin & Seller roles; optional for Buyer at launch.** _Enforce at IdP and app layers._ **Must not** allow admin/seller sessions without MFA. **RBAC** gates admin exports and seller status actions. **Verification:** negative login tests per role + audit. 
2. **Account lockout after 5 failed attempts**; **email‑based reset** as primary; CAPTCHA required on registration to reduce abuse. **Verification:** auth policy tests; CAPTCHA present on UC4; reset e‑mail delivery. 
3. **Expose no PII in bidder surfaces** (show bidder handle only); **limit buyer info before payment** on seller views. **Verification:** UI/API masking assertions in UC6/UC15. 

---

## 2. Data Protection & Auditability

4. **Encrypt in transit (TLS 1.2+) and at rest (SSE‑KMS)** for all user/transaction data. **Do not** log secrets/PII; store secrets in KMS. **Verification:** config inspection, cipher suites, key policies, log sampling. 
5. **Maintain immutable audit logs** for critical events (login, bid, buy, seller activation, stock changes) with **≥12‑month retention** and **correlation IDs** for tracing. **Verification:** audit trail reconstruction of bid→buy; retention jobs. 

---

## 3. Availability, Resilience & Disaster Recovery

6. **Target uptime ≥99.9%** for core endpoints with **zero planned downtime** for search, item details, bid, buy, payment. **Verification:** SLI/SLO dashboards. 
7. **Health probes:** App **every 5s**, DB replication **every 15s**; **zonal failover** primary and **regional failover** secondary. **Verification:** chaos tests; failover runbooks. 
8. **Blue/Green (or canary) releases** with automated smoke tests **before** traffic shift. **Verification:** pipeline evidence + smoke suite pass criteria. 

---

## 4. Performance & Caching

9. **p95 latency SLOs:** **≤3s** for UI pages and **≤500ms** for APIs at normal load; **≤5s** for peak pages. **Verification:** load/spike tests. 
10. **Use CDN for static assets** and **cache public GET APIs** (non‑personalized); define TTLs and validation. **Verification:** cache headers and hit ratio. 
11. **Avoid high‑frequency client polling**; the Current Bids page **auto‑refreshes every 5s** and avoids competitor counts to reduce load. **Verification:** telemetry of polling cadence/QPS. 

---

## 5. Auction Integrity & Ordering

12. **Bid increment rule** is **max(5%, ₹10)** validated server‑side; reject under‑min bids; no automatic proxy bidding in MVP. **Verification:** boundary unit/API tests. 
13. **Buy Now cancels all active bids atomically** upon successful payment; address can be edited **only before** payment. **Verification:** transactional test around UC7; post‑payment edit blocked. 
14. **Idempotency + ACID for order/bid writes** with **exponential backoff (100ms–2s×3)** on transient failures; user sees generic retry (no rollback details). **Verification:** fault injection; idempotent handlers. 

---

## 6. Inventory, Stock & Catalog

15. **Auto‑remove items at quantity zero**; default **low‑stock alert at ≤5** (configurable); support **bulk stock updates** (CSV/UI). **Verification:** inventory jobs + UI behavior. 
16. **Homepage shows Top‑5** selling items with **daily refresh**, no personalization in v1; use **pagination** (no infinite scroll). **Verification:** daily job + UI checks. 

---

## 7. Seller Lifecycle & Admin Controls

17. **Seller activation** only after rent payment; **notify seller** on status change; **single‑admin** approval OK. **Verification:** event notifications + admin audit. 
18. **Rent payment** methods include PayPal/Cards/Cheque/DD; **7‑day grace**; **no auto‑debit**, send reminders/escalations (weekly, 3‑level). **Verification:** scheduler & comms logs. 

---

## 8. Observability & Operability

19. **Logs, Metrics, Traces** are mandatory with correlation IDs; **alerts** must be tied to SLOs (**p95 latency**, **error<1%**, **uptime≥99.9%**). **Verification:** dashboards, alert policies. 
20. **Trace sampling 100%** for checkout, bidding, payment; **runbooks** required for every alert. **Verification:** span coverage and runbook links in alerts. 

---

## 9. Scalability & Capacity

21. **Autoscaling thresholds**: scale‑out at **CPU≥70% for 3 minutes** and **Mem≥75%**; **queue depth>100** adds workers; **safe scale‑in** only when **CPU<40%** and no spikes. **Verification:** policy tests and load rehearsal. 
22. **Design for 10k concurrent sessions** during peak. **Verification:** peak load test scenario in CI performance stage. 

---

## 10. Release Engineering & Contract Testing

23. **Contract tests are blocking** for merges/deploys; **version APIs** (semver); **canary/blue‑green** with automatic rollback on degradation. **Verification:** CI evidence, contract break detection, rollback logs. 

---

## 11. Accessibility & UX

24. **WCAG 2.1 AA across buyer & seller surfaces**; enforce contrast for themes; **inline field‑level validation**; keyboard‑only paths must complete registration, bid, buy. **Verification:** automated Axe + manual audits. 

---

## 12. Compliance Checklist (Quick Map)

| Area | Guardrail Snapshot | Verification | Source |
|---|---|---|---|
| Identity | MFA (Admin/Seller), 5‑attempt lockout, CAPTCHA on signup | Auth policy tests; CAPTCHA visible |  |
| Data | TLS + SSE‑KMS; PII‑safe logs; 12‑month audit | Config & log sampling; retention job |  |
| Availability | 99.9% uptime; health 5s/15s; zonal→regional failover | Chaos/recovery drills |  |
| Performance | p95 ≤3s UI; CDN + API GET cache | Load tests; cache headers |  |
| Bidding | max(5%, ₹10) increment; no proxy | API negative tests |  |
| Orders | Buy‑Now cancels bids on pay; idempotent writes | E2E + fault injection |  |
| Inventory | Auto‑remove at zero; low‑stock ≤5 | Job + UI checks |  |
| Seller/Admin | Notify status change; single‑admin; rent grace 7d | Event + scheduler logs |  |
| Observability | SLO‑based alerts; 100% tracing for critical flows | Dashboards + span coverage |  |
| Scalability | CPU≥70%/3m, Mem≥75%, Q>100; safe scale‑in | Autoscaler policy tests |  |
| Release | Contract tests blocking; semver; canary/blue‑green | CI evidence; rollback logs |  |
| Accessibility | WCAG AA; inline validation; keyboard paths | Axe + manual audits |  |

---

## 13. Governance & Exceptions

- **Change control**: Any exception to these guardrails requires a **written ADR** referencing the affected UC/RTM row and an **expiry date** for the exception. 
- **Audit readiness**: RTM rows must link to tests (functional, security, performance) before a release gate is passed. 

---

### Appendix — Traceability Pointers
- **Functional RTM & STRIDE (merged)** — see consolidated register for linked verification approaches. 
- **Use‑Case Specs (Functional)** — actors, flows, validations for UC1–UC21. 
- **Use‑Case Specs (NFR)** — UC‑NFR1–UC‑NFR8 with sequences and acceptance criteria. 
