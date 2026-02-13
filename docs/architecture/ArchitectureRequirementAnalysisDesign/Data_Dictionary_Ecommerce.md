
# DATA DICTIONARY (Clean GitHub Markdown)
**Version:** 1.0
**Generated:** 2026-02-12 17:55:23

This file contains **no special characters**, **no hidden Unicode**, **no citation markers**, and is fully **GitHub‑safe**.

---

## 0. Conventions
- PK = Primary Key (UUID)
- FK = Foreign Key (service-internal only)
- *_ref = Cross-service reference (validated by API/Saga)
- Timestamps = TIMESTAMPTZ (UTC)
- Monetary values = DECIMAL(19,4)
- Audit = immutable, append-only, >=12 months retention

---

## 1. Identity & Access Service

### Table: user
| Field | Type | Null | Description |
|-------|------|------|-------------|
| user_id (PK) | UUID | No | User identifier |
| email | VARCHAR(254) | No | Login email (unique) |
| display_handle | VARCHAR(50) | No | Public alias (non-PII) |
| mfa_enabled | BOOLEAN | No | MFA required for seller/admin |
| status | ENUM(ACTIVE, LOCKED, DISABLED) | No | User status |
| created_at | TIMESTAMPTZ | No | Created timestamp |
| updated_at | TIMESTAMPTZ | No | Last updated |

### Table: role
| Field | Type | Description |
|-------|------|-------------|
| role_id (PK) | UUID | Role ID |
| name | ENUM(BUYER, SELLER, ADMIN) | Role name |

### Table: user_role
| Field | Type | Description |
|-------|------|-------------|
| user_id | UUID | FK -> user |
| role_id | UUID | FK -> role |
| PK | (user_id, role_id) | Composite key |

---

## 2. Seller Management Service

### Table: seller
| Field | Type | Null | Description |
|--------|------|------|-------------|
| seller_id (PK) | UUID | No | Seller identifier |
| owner_user_ref | UUID | No | Cross-service user ref |
| status | ENUM(PENDING, ACTIVE, SUSPENDED) | No | Seller status |
| activated_at | TIMESTAMPTZ | Yes | Activation time |
| created_at | TIMESTAMPTZ | No |
| updated_at | TIMESTAMPTZ | No |

### Table: rent_agreement
| Field | Type | Null | Description |
|--------|------|------|-------------|
| agreement_id (PK) | UUID | No | Rent agreement |
| seller_id (FK) | UUID | No | FK -> seller |
| amount | DECIMAL(19,4) | No | Rent amount |
| period | ENUM(MONTHLY, QUARTERLY, ANNUAL) | No |
| grace_days | INT | No | Default 7 |
| start_date | DATE | No |

### Table: rent_payment
| Field | Type | Null | Description |
|--------|------|------|-------------|
| payment_id (PK) | UUID | No | Rent payment |
| agreement_id (FK) | UUID | No | FK -> rent_agreement |
| method | ENUM(PAYPAL, CARD, CHEQUE, DD) | No |
| amount | DECIMAL(19,4) | No |
| status | ENUM(RECEIVED, CLEARED, FAILED) | No |
| paid_at | TIMESTAMPTZ | No |

---

## 3. Catalog Service

### Table: product
| Field | Type | Null | Description |
|--------|------|------|-------------|
| product_id (PK) | UUID | No | Product ID |
| seller_ref | UUID | No | Cross-service seller ref |
| title | VARCHAR(160) | No |
| description | TEXT | Yes |
| buy_now_price | DECIMAL(19,4) | Yes |
| is_active | BOOLEAN | No |
| created_at | TIMESTAMPTZ | No |
| updated_at | TIMESTAMPTZ | No |

### Table: category
| Field | Type | Null | Description |
|--------|------|------|-------------|
| category_id (PK) | UUID | No |
| name | VARCHAR(120) | No |
| path | VARCHAR(512) | Yes | Category hierarchy |

### Table: product_category
| Field | Type | Description |
|--------|------|-------------|
| product_id (FK) | UUID | FK -> product |
| category_id (FK) | UUID | FK -> category |
| PK | (product_id, category_id) | Composite key |

### Table: media_reference
| Field | Type | Null | Description |
|--------|------|------|-------------|
| media_id (PK) | UUID | No |
| product_id (FK) | UUID | No |
| uri | TEXT | No |
| mime_type | VARCHAR(100) | No |
| created_at | TIMESTAMPTZ | No |

---

## 4. Inventory Service

### Table: inventory
| Field | Type | Null | Description |
|--------|------|------|-------------|
| product_ref (PK) | UUID | No | Cross-service product ref |
| qty_on_hand | INT | No |
| qty_reserved | INT | No |
| low_stock_threshold | INT | No |
| remove_at_zero | BOOLEAN | No |
| updated_at | TIMESTAMPTZ | No |

### Table: inventory_reservation
| Field | Type | Null | Description |
|--------|------|------|-------------|
| reservation_id (PK) | UUID | No |
| order_ref | UUID | No | Cross-service order ref |
| product_ref | UUID | No |
| qty | INT | No |
| status | ENUM(HELD, RELEASED, CONSUMED) | No |
| reserved_at | TIMESTAMPTZ | No |
| updated_at | TIMESTAMPTZ | No |

---

## 5. Bidding Service

### Table: auction
| Field | Type | Null | Description |
|--------|------|------|-------------|
| auction_id (PK) | UUID | No |
| product_ref | UUID | No | Cross-service product ref |
| start_at | TIMESTAMPTZ | No |
| end_at | TIMESTAMPTZ | No |
| start_price | DECIMAL(19,4) | No |
| min_inc_pct | DECIMAL(5,2) | No |
| min_inc_flat | DECIMAL(19,2) | No |
| current_high_bid | DECIMAL(19,4) | Yes |
| version | INT | No |
| status | ENUM(SCHEDULED, OPEN, CLOSED, CANCELLED) | No |

### Table: bid
| Field | Type | Null | Description |
|--------|------|------|-------------|
| bid_id (PK) | UUID | No |
| auction_id (FK) | UUID | No |
| buyer_user_ref | UUID | No |
| amount | DECIMAL(19,4) | No |
| placed_at | TIMESTAMPTZ | No |
| status | ENUM(ACCEPTED, REJECTED, OUTBID, CANCELLED) | No |
| reject_reason | VARCHAR(120) | Yes |

---

## 6. Orders Service

### Table: order
| Field | Type | Null | Description |
|--------|------|------|-------------|
| order_id (PK) | UUID | No |
| buyer_user_ref | UUID | No |
| source | ENUM(BUY_NOW, AUCTION_OUTCOME) | No |
| status | ENUM(PENDING, APPROVED, REJECTED, PAID, CANCELLED, FULFILLED) | No |
| total_amount | DECIMAL(19,4) | No |
| currency | CHAR(3) | No |
| correlation_id | UUID | No |
| version | INT | No |
| created_at | TIMESTAMPTZ | No |
| updated_at | TIMESTAMPTZ | No |

### Table: order_line
| Field | Type | Null | Description |
|--------|------|------|-------------|
| order_line_id (PK) | UUID | No |
| order_id (FK) | UUID | No |
| product_ref | UUID | No |
| quantity | INT | No |
| price_each | DECIMAL(19,4) | No |
| ref_auction_id | UUID | Yes |

### Table: address_snapshot
| Field | Type | Null | Description |
|--------|------|------|-------------|
| order_id (PK & FK) | UUID | No |
| full_name | VARCHAR(120) | No |
| line1 | VARCHAR(120) | No |
| line2 | VARCHAR(120) | Yes |
| line3 | VARCHAR(120) | Yes |
| city | VARCHAR(80) | No |
| state | VARCHAR(80) | Yes |
| postal_code | VARCHAR(20) | No |
| country | CHAR(2) | No |
| phone | VARCHAR(30) | Yes |

---

## 7. Payments Integration Service

### Table: payment_transaction
| Field | Type | Null | Description |
|--------|------|------|-------------|
| payment_id (PK) | UUID | No |
| order_ref | UUID | No |
| type | ENUM(AUTH, CAPTURE, REFUND) | No |
| amount | DECIMAL(19,4) | No |
| gateway_ref | VARCHAR(120) | Yes |
| status | ENUM(PENDING, SUCCESS, FAILED) | No |
| attempted_at | TIMESTAMPTZ | No |

---

## 8. Notification Service

### Table: notification_template
| Field | Type | Description |
|--------|------|-------------|
| template_id (PK) | UUID | Template ID |
| key | VARCHAR(80) | Unique key |
| channel | ENUM(EMAIL, SMS) | Delivery channel |
| subject | VARCHAR(160) | Email subject |
| body | TEXT | Message body |
| created_at | TIMESTAMPTZ | Timestamp |

### Table: notification
| Field | Type | Null | Description |
|--------|------|------|-------------|
| notification_id (PK) | UUID | No |
| to_user_ref | UUID | Yes |
| to_seller_ref | UUID | Yes |
| to_order_ref | UUID | Yes |
| channel | ENUM(EMAIL, SMS) | No |
| template_key | VARCHAR(80) | No |
| payload | JSONB | Yes |
| status | ENUM(QUEUED, SENT, FAILED) | No |
| sent_at | TIMESTAMPTZ | Yes |

---

## 9. Audit Service (Immutable)

### Table: audit_event
| Field | Type | Null | Description |
|--------|------|------|-------------|
| audit_id (PK) | UUID | No |
| actor_user_ref | UUID | Yes |
| resource_type | VARCHAR(40) | No |
| resource_id | UUID | No |
| action | VARCHAR(60) | No |
| correlation_id | UUID | Yes |
| created_at | TIMESTAMPTZ | No |
| payload | JSONB | Yes | Redacted, no PII |

---

## 10. Search Service (Read Model)

### Table: product_search_document
| Field | Type | Description |
|--------|------|-------------|
| product_id | UUID | Document key |
| title | TEXT | Searchable title |
| description | TEXT | Searchable description |
| categories | ARRAY(TEXT) | Category facets |
| seller_name | TEXT | Optional |
| price_range | TEXT | Bucketed price |
| is_active | BOOLEAN | Visibility |
| metrics | JSONB | Popularity/Ranking |

---

## 11. Transactional Outbox (Template)

| Field | Type | Description |
|--------|------|-------------|
| id (PK) | UUID | Outbox message ID |
| aggregate_id | UUID | Related aggregate |
| type | VARCHAR(120) | Event/Command type |
| payload | JSONB | Event payload |
| headers | JSONB | Metadata |
| created_at | TIMESTAMPTZ | Timestamp |
| status | ENUM(PENDING, SENT, FAILED) | Delivery state |

---

End of Document.
