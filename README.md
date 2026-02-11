# 📘 E‑Commerce Web Application(EWA) – Case Study

_A Comprehensive Functional & Architectural Overview_

## 📌 Introduction

This case study describes the design and functionality of an e‑commerce platform similar to **bidorbuy.com**, enabling users to **bid on items** or **directly purchase** them. The platform supports three user types:

*   **Buyers** – External users who browse, bid, purchase items.
*   **Sellers** – External users who set up shops, list items, manage auctions.
*   **Administrators** – Internal users who manage seller status, payments, and transactions.

The system includes complete workflows for **registration**, **bidding**, **buying**, **payment**, **shipping**, **inventory updates**, and **administrative oversight**. [\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## 🏠 1. Home Page Features

The home page acts as a discovery layer:

*   Displays **Top 5 selling items** with images.
*   Category-based browsing for all items.
*   Item details accessible after clicking item/category.
*   **Login required** to transact (buy/bid).
*   Search functionalities:
    *   Basic search (items/sellers)
    *   Advanced search (category, price, condition, seller location, bid format) [\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## 🛒 2. Buyer Features

After login, buyers access dedicated features:

### 🔍 Item Browsing

*   Category-wise item listings (image, description, bids, price, shipping, seller details).
*   Advanced search by multiple criteria.

### 🏷️ Bidding

*   View full auction details: current highest bid, bidder, start/end date/time, etc.
*   Place new bids and confirm.
*   Highest bidder becomes winner if reserve/min threshold is met.
*   “**My Current Bids**” section shows active/expired bids.

### 🛍️ Direct Purchase (Buy Now)

*   Buyers can purchase items directly, cancelling all bids.
*   Payment options available post-purchase.

### 📊 Price Comparison

Select multiple items and compare:

*   Seller pricing
*   Shipping cost
*   Item metadata

### 🧾 Transaction History

View all past bids won and direct purchases.

### 👀 Watch List

Buyers can watch items for quick monitoring via “My Watch List”.  
citeturn1search1

* * *

## 🏪 3. Seller Features

Sellers access complete shop management:

### 🛒 Shop Setup

*   Requires registration + payment of rent (monthly/quarterly/yearly).
*   Admin activates seller account after payment.

### 📦 Item Management

*   Add/edit items with details: name, description, category, quantity, price, condition, shipping, threshold price.
*   Inventory auto‑update on dispatch; item auto‑remove when quantity hits zero.

### 🔨 Auction Setup

*   Configure bid timings, threshold price, shipping cost, etc.

### 📈 Status Tracking

View sold/unsold items, bids, selling price, payment status, buyer details, dispatch information.

### 🚚 Dispatch & Inventory Updates

*   Shows items ready for dispatch.
*   Automatically decrements inventory on dispatch.

### 🧾 Seller Transactions

Complete list of past sales and payment methods used.  
[\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## 🔧 4. Administrator Features

Admins manage operational oversight:

### 🎛️ Seller Account Management

*   Activate/deactivate sellers based on rent payment status.

### 📊 Transaction Reporting

*   View all transactions between date ranges.

### ⚠️ Rent Defaulters

*   View sellers who defaulted on rent payments.  
    [\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## 💳 5. Payment Options

The platform supports multiple payment modes:

*   **PayPal**
*   **Credit Card** (via payment gateway)
*   **Demand Draft (DD)**
*   **Cheque Payment**

Sellers dispatch items only after DD/cheque is encashed.  
[\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## ⚙️ 6. Special System Considerations

The system design must ensure:

*   High maintainability
*   Scalability
*   Zero downtime
*   24×7 availability
*   Support for high traffic
*   Seamless integration with internal systems  
    [\[Case-Study...pplication | Word\]](https://cognizantonline-my.sharepoint.com/personal/2371402_cognizant_com/_layouts/15/Doc.aspx?sourcedoc=%7B22B12024-B9B3-46FF-889B-83E9AF81F708%7D&file=Case-Study-Ecommerce-Web-Application.doc&action=default&mobileredirect=true)

* * *

## 📂 7. Architecture Documentation Included

The following architecture documents are generated in this repository:

*   Microservice Architecture Overview
*   Deployment Diagram
*   Communication (Sequence) Diagrams
*   Class Diagram
*   Component Diagram
*   Entity–Relationship (ER) Diagram

Each is written in Markdown with Mermaid diagrams for GitHub rendering.

* * *

## 🧱 8. Tech Stack (Recommended)

While not specified in the uploaded case study, the architecture aligns well with:

*   **Backend**: Java/Spring Boot, .NET Core, Node.js
*   **Frontend**: React, Angular, or Vue
*   **Database**: PostgreSQL / MongoDB per microservice
*   **Containerization**: Docker + Kubernetes
*   **Messaging/Eventing**: Kafka or RabbitMQ
*   **Payment Integration**: PayPal API, Stripe
*   **Search**: Elasticsearch or OpenSearch

* * *

## 🚀 9. How to Use This Repository

*   Browse the `/docs` folder for architecture assets.
*   Open `.md` files directly in GitHub to view Mermaid diagrams.
*   Use this documentation as a reference for:
    *   System Design Interviews
    *   Architecture Blueprinting
    *   Academic Submissions
    *   Solutioning for real-world e‑commerce systems

## 📂 Project Folder Structure - Navigation

- ** E‑Commerce Web Application(EWA) -cs** #Root repository
- **Code** → `ewa_ecommerce_webapp_cs/src/` #Application source code
- **Services** → `ewa_ecommerce_webapp_cs/src/Services/` #Microservices (Policy, Claims, Payments, Members)
- **Web** → `ewa_ecommerce_webapp_cs/src/Web/` #Web portal (ASP.NET MVC/Blazor)
- **Gateways** → `ewa_ecommerce_webapp_cs/src/Gateways/` #API Gateway
- **Shared** → `ewa_ecommerce_webapp_cs/src/Shared/` #Shared domain, application, infrastructure
- **Tests** → `ewa_ecommerce_webapp_cs/tests/` #Unit, Integration, E2E tests
- **Tools** → `ewa_ecommerce_webapp_cs/tools/` #Scripts, infrastructure helpers
- **Docs** → `ewa_ecommerce_webapp_cs/docs/` #Architecture, ADRs, diagrams, specs
- **Architecture** → `ewa_ecommerce_webapp_cs/docs/architecture/` #standards, templates, checklists, decision log
- **Data** → `ewa_ecommerce_webapp_cs/docs/data/`
- **Diagrams** → `ewa_ecommerce_webapp_cs/ocs/diagrams/`
- **Governance** → `ewa_ecommerce_webapp_cs/governance/`
- **Quality & NFRs** → `ewa_ecommerce_webapp_cs/docs/quality/`
- **Security** → `ewa_ecommerce_webapp_cs/docs/security/`
- **Specs** → `ewa_ecommerce_webapp_cs/docs/Specs/`
- **Testing** → `ewa_ecommerce_webapp_cs/docs/testing/`
- **Use-Cases** → `ewa_ecommerce_webapp_cs/docs/use-cases/`
- **ADRs** → `ewa_ecommerce_webapp_cs/docs/adr/`
- **ReadMe** → `ewa_ecommerce_webapp_cs/README.md` #Project Information
- **Core Case Study** → `ewa_ecommerce_webapp_cs/case_study_2_imic.md` #Case Study Details
- **Contributors** → `ewa_ecommerce_webapp_cs/CONTRIBUTING.md` #Project Contributors/Team
- **Api Versons** → `ewa_ecommerce_webapp_cs/VERSION`


## 🛠️ Tech Stack (Example)
- **Frontend**: Angular / HTML / CSS  
- **Backend**: Asp.Net
- **Database**: MySQL  
- **Tools**: Postman, Git, VS Code  

---

## 📸 Screenshots  
(Add images in `docs/screenshots/`)

---

## 🧪 Testing
- Test cases for backend & frontend are included under /tests.

---
## 🙌 Author  
- Manish Kumar Dubey 

