# System Specifications Document
## Wekeza CRM - Technical Architecture and Specifications

**Document Version:** 1.0  
**Date:** February 12, 2026  
**Status:** Approved  
**Owner:** Technical Architecture Team

---

## 1. System Overview

### 1.1 System Architecture
Wekeza CRM follows Clean Architecture principles with clear separation of concerns across four main layers:

```
┌─────────────────────────────────────────────────────────┐
│                     Presentation Layer                   │
│              (API Controllers, Middleware)               │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Application Layer                       │
│         (Business Logic, DTOs, Interfaces)              │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    Domain Layer                          │
│          (Entities, Value Objects, Enums)               │
└─────────────────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Infrastructure Layer                       │
│    (Data Access, External Services, Repositories)       │
└─────────────────────────────────────────────────────────┘
```

### 1.2 Technology Stack

#### Backend
- **Framework:** .NET 8.0 (LTS)
- **Language:** C# 12
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server 2019+
- **API Documentation:** Swagger/OpenAPI 3.0
- **Authentication:** JWT Bearer Tokens
- **Testing:** xUnit, Moq, FluentAssertions

#### Infrastructure
- **Cloud Provider:** Azure (or AWS/GCP compatible)
- **Container:** Docker
- **Orchestration:** Kubernetes
- **CI/CD:** GitHub Actions
- **Monitoring:** Application Insights
- **Logging:** Serilog

---

## 2. Database Design

### 2.1 Entity Relationship Diagram (ERD)

```
┌──────────────┐       ┌──────────────┐       ┌──────────────┐
│   Customer   │───┬───│   Account    │       │  Transaction │
│              │   │   │              │───────│              │
│  - Id        │   │   │  - Id        │       │  - Id        │
│  - FirstName │   │   │  - AccountNo │       │  - Amount    │
│  - LastName  │   │   │  - Balance   │       │  - Date      │
│  - Email     │   │   │  - Currency  │       └──────────────┘
│  - Segment   │   │   └──────────────┘
│  - KYCStatus │   │
└──────────────┘   │   ┌──────────────┐
       │           └───│  Interaction │
       │               │              │
       │               │  - Id        │
       │               │  - Channel   │
       │               │  - Date      │
       │               │  - Notes     │
       │               └──────────────┘
       │
       │               ┌──────────────┐
       └───────────────│     Case     │
                       │              │
                       │  - Id        │
                       │  - Title     │
                       │  - Status    │
                       │  - Priority  │
                       └──────────────┘
```

### 2.2 Core Tables

#### Customers Table
```sql
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NOT NULL,
    DateOfBirth DATE,
    Segment NVARCHAR(50) NOT NULL,
    KYCStatus NVARCHAR(50) NOT NULL,
    CreditScore INT,
    LifetimeValue DECIMAL(18,2),
    RiskProfile NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(100)
);

CREATE INDEX IX_Customers_Email ON Customers(Email);
CREATE INDEX IX_Customers_Segment ON Customers(Segment);
CREATE INDEX IX_Customers_KYCStatus ON Customers(KYCStatus);
```

#### Cases Table
```sql
CREATE TABLE Cases (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    CaseNumber NVARCHAR(50) NOT NULL UNIQUE,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50) NOT NULL,
    Priority NVARCHAR(50) NOT NULL,
    Category NVARCHAR(100),
    SubCategory NVARCHAR(100),
    AssignedTo UNIQUEIDENTIFIER,
    ResolvedAt DATETIME2,
    SLADeadline DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(100),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

CREATE INDEX IX_Cases_CustomerId ON Cases(CustomerId);
CREATE INDEX IX_Cases_Status ON Cases(Status);
CREATE INDEX IX_Cases_Priority ON Cases(Priority);
```

### 2.3 Phase 2 Tables

#### NextBestActions Table
```sql
CREATE TABLE NextBestActions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    ActionType NVARCHAR(100) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ConfidenceScore DECIMAL(5,4),
    RecommendedDate DATETIME2 NOT NULL,
    CompletedAt DATETIME2,
    IsCompleted BIT DEFAULT 0,
    Outcome NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
```

#### SentimentAnalysis Table
```sql
CREATE TABLE SentimentAnalysis (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER,
    CaseId UNIQUEIDENTIFIER,
    InteractionId UNIQUEIDENTIFIER,
    SentimentType NVARCHAR(50) NOT NULL,
    SentimentScore DECIMAL(5,4),
    TextAnalyzed NVARCHAR(MAX),
    KeyPhrases NVARCHAR(MAX),
    AnalyzedDate DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
```

### 2.4 Phase 3 Tables

#### WhatsAppMessages Table
```sql
CREATE TABLE WhatsAppMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER,
    PhoneNumber NVARCHAR(20) NOT NULL,
    MessageType NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Content NVARCHAR(MAX),
    MediaUrl NVARCHAR(500),
    WhatsAppMessageId NVARCHAR(100),
    IsInbound BIT NOT NULL,
    SentAt DATETIME2,
    DeliveredAt DATETIME2,
    ReadAt DATETIME2,
    CreatedAt DATETIME2 NOT NULL
);
```

---

## 3. API Specifications

### 3.1 API Architecture

**Base URL:** `https://api.wekeza.com/v1`  
**Authentication:** JWT Bearer Token  
**Content-Type:** `application/json`  
**Rate Limiting:** 1000 requests/minute per API key

### 3.2 API Endpoints Summary

#### Customer Management (7 endpoints)
```
GET    /api/customers              - List all customers
GET    /api/customers/{id}         - Get customer by ID
POST   /api/customers              - Create customer
PUT    /api/customers/{id}         - Update customer
DELETE /api/customers/{id}         - Delete customer
GET    /api/customers/{id}/360     - Get 360° view
GET    /api/customers/segment/{segment} - Get by segment
```

#### Case Management (6 endpoints)
```
GET    /api/cases                  - List all cases
GET    /api/cases/{id}             - Get case by ID
POST   /api/cases                  - Create case
PUT    /api/cases/{id}             - Update case
GET    /api/cases/customer/{customerId} - Get customer cases
POST   /api/cases/{id}/escalate    - Escalate case
```

#### Analytics (4 endpoints)
```
GET    /api/analytics/customers    - Customer analytics
GET    /api/analytics/cases        - Case analytics
GET    /api/analytics/interactions - Interaction analytics
GET    /api/analytics/dashboard    - Dashboard summary
```

### 3.3 Sample API Request/Response

#### Create Customer
**Request:**
```http
POST /api/customers
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+254712345678",
  "segment": "Retail",
  "kycStatus": "Pending"
}
```

**Response:**
```http
HTTP/1.1 201 Created
Content-Type: application/json

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+254712345678",
  "segment": "Retail",
  "kycStatus": "Pending",
  "isActive": true,
  "createdAt": "2026-02-12T15:30:00Z"
}
```

---

## 4. Security Architecture

### 4.1 Authentication Flow

```
User → Login Request → API → Validate Credentials → Generate JWT
                                                          │
User ← JWT Token ←──────────────────────────────────────┘
│
└→ Subsequent Requests (with JWT in Authorization header)
   │
   └→ API → Validate JWT → Process Request → Response
```

### 4.2 Security Layers

**Layer 1: Network Security**
- TLS 1.3 for all communications
- Firewall rules
- DDoS protection
- IP whitelisting for admin access

**Layer 2: Application Security**
- JWT-based authentication
- Role-based access control (RBAC)
- API rate limiting
- Input validation and sanitization
- SQL injection prevention (parameterized queries)

**Layer 3: Data Security**
- Encryption at rest (AES-256)
- Encryption in transit (TLS 1.3)
- PII data masking
- Audit logging for all data access

**Layer 4: Compliance**
- GDPR compliance
- PCI-DSS for payment data
- Local banking regulations
- Data residency requirements

### 4.3 Security Controls

```yaml
Authentication:
  - JWT tokens with 30-minute expiration
  - Refresh tokens for session management
  - Multi-factor authentication support
  
Authorization:
  - Role-based permissions
  - Resource-level access control
  - API endpoint authorization
  
Data Protection:
  - Field-level encryption for sensitive data
  - Automated PII detection and masking
  - Secure key management (Azure Key Vault)
  
Audit & Compliance:
  - All API calls logged
  - Data access audit trail
  - Compliance reporting dashboards
```

---

## 5. Integration Architecture

### 5.1 Integration Points

```
┌─────────────────┐
│  Core Banking   │←─────┐
│     System      │      │
└─────────────────┘      │
                         │
┌─────────────────┐      │     ┌─────────────────┐
│  SMS Gateway    │←─────┼─────│  Wekeza CRM     │
└─────────────────┘      │     │  (Central Hub)  │
                         │     └─────────────────┘
┌─────────────────┐      │              │
│    WhatsApp     │←─────┤              │
│  Business API   │      │              │
└─────────────────┘      │              │
                         │              │
┌─────────────────┐      │              │
│   Email SMTP    │←─────┘              │
└─────────────────┘                     │
                                        │
┌─────────────────┐                     │
│  USSD Gateway   │←────────────────────┘
└─────────────────┘
```

### 5.2 Integration Methods

**REST APIs:**
- Core banking system integration
- WhatsApp Business API
- SMS gateway
- Email service provider

**Webhooks:**
- WhatsApp message status updates
- Payment notifications
- Third-party event notifications

**Message Queue:**
- Asynchronous processing
- Event-driven architecture
- Reliable message delivery

---

## 6. Performance Specifications

### 6.1 Response Time Requirements

| Operation Type | Target Response Time | Maximum Response Time |
|---------------|---------------------|----------------------|
| API GET (single record) | < 100ms | < 200ms |
| API GET (list) | < 200ms | < 500ms |
| API POST/PUT | < 150ms | < 300ms |
| Search Query | < 500ms | < 1000ms |
| Report Generation | < 5s | < 10s |
| Dashboard Load | < 1s | < 2s |

### 6.2 Throughput Requirements

| Metric | Target | Peak Capacity |
|--------|--------|--------------|
| Concurrent Users | 1,000 | 2,000 |
| API Requests/minute | 10,000 | 20,000 |
| Database Queries/second | 500 | 1,000 |
| Messages/second (WhatsApp) | 100 | 200 |

### 6.3 Scalability Design

**Horizontal Scaling:**
- Stateless API servers
- Load balancer distribution
- Auto-scaling based on CPU/memory

**Database Scaling:**
- Read replicas for queries
- Connection pooling
- Query optimization
- Caching layer (Redis)

**Caching Strategy:**
- Application-level caching (in-memory)
- Distributed caching (Redis)
- CDN for static assets
- Cache invalidation policies

---

## 7. Deployment Architecture

### 7.1 Environment Structure

**Development Environment:**
- Purpose: Active development
- Database: Dev SQL Server
- Auto-deploy from dev branch

**Staging Environment:**
- Purpose: UAT and integration testing
- Database: Staging SQL Server (prod-like data)
- Manual deployment

**Production Environment:**
- Purpose: Live system
- Database: Prod SQL Server (with replication)
- Blue-green deployment

### 7.2 Infrastructure Components

```
┌───────────────────────────────────────────────────────┐
│               Azure Load Balancer                      │
└─────────────┬─────────────────────┬───────────────────┘
              │                     │
    ┌─────────▼──────────┐ ┌───────▼──────────┐
    │   API Instance 1   │ │  API Instance 2  │
    │   (Container)      │ │  (Container)     │
    └─────────┬──────────┘ └───────┬──────────┘
              │                     │
    ┌─────────▼─────────────────────▼──────────┐
    │         Azure SQL Database                │
    │         (with read replicas)              │
    └──────────────────────────────────────────┘
              │
    ┌─────────▼──────────┐
    │   Redis Cache      │
    │   (Distributed)    │
    └────────────────────┘
```

### 7.3 Disaster Recovery

**Backup Strategy:**
- Database: Daily full backups, hourly incremental
- Retention: 30 days
- Off-site replication: Azure Geo-Redundant Storage

**Recovery Objectives:**
- RTO (Recovery Time Objective): 4 hours
- RPO (Recovery Point Objective): 1 hour

**High Availability:**
- Active-passive database configuration
- Automatic failover
- Multi-region deployment

---

## 8. Monitoring and Observability

### 8.1 Application Monitoring

**Metrics Tracked:**
- API response times (p50, p95, p99)
- Error rates and exceptions
- CPU and memory usage
- Database query performance
- Cache hit rates
- Message queue depth

**Alerting Thresholds:**
- API response time > 500ms
- Error rate > 1%
- CPU usage > 80%
- Memory usage > 85%
- Database connection pool exhaustion

### 8.2 Logging Strategy

**Log Levels:**
- ERROR: System errors requiring attention
- WARN: Warnings and anomalies
- INFO: Important events and transactions
- DEBUG: Detailed diagnostic information

**Log Aggregation:**
- Centralized logging (ELK stack or Azure Log Analytics)
- Structured logging (JSON format)
- Log retention: 90 days
- Real-time log streaming

### 8.3 Health Checks

**Endpoints:**
- `/api/health` - Overall system health
- `/api/health/db` - Database connectivity
- `/api/health/cache` - Cache service
- `/api/health/queue` - Message queue

**Monitoring Frequency:**
- Health checks: Every 30 seconds
- Metric collection: Every 1 minute
- Alert evaluation: Every 5 minutes

---

## 9. Development Standards

### 9.1 Coding Standards

**C# Conventions:**
- PascalCase for public members
- camelCase for private fields
- Async/await for all I/O operations
- Dependency injection for all services

**Architecture Patterns:**
- Repository pattern for data access
- CQRS for complex operations
- Factory pattern for object creation
- Strategy pattern for algorithms

### 9.2 Testing Requirements

**Unit Tests:**
- Minimum 80% code coverage
- Test all business logic
- Use mocking for dependencies
- Follow AAA pattern (Arrange-Act-Assert)

**Integration Tests:**
- Test API endpoints
- Test database operations
- Test external integrations
- Use test containers

**Performance Tests:**
- Load testing before release
- API response time validation
- Database query performance
- Memory leak detection

---

## 10. Technical Specifications Summary

| Component | Specification |
|-----------|--------------|
| **Platform** | .NET 8.0 |
| **Database** | SQL Server 2019+ |
| **API** | RESTful, OpenAPI 3.0 |
| **Authentication** | JWT Bearer Tokens |
| **Caching** | Redis 6+ |
| **Containers** | Docker |
| **Orchestration** | Kubernetes |
| **Cloud** | Azure (Azure-compatible) |
| **CI/CD** | GitHub Actions |
| **Monitoring** | Application Insights |
| **Logging** | Serilog |

---

**Document Control**  
**Last Updated:** February 12, 2026  
**Version:** 1.0  
**Status:** Approved  
**Distribution:** Technical Team, DevOps, Security Team
