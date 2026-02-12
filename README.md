# Wekeza CRM - Bank-Grade Customer Relationship Management System

![Version](https://img.shields.io/badge/version-3.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Tests](https://img.shields.io/badge/tests-74%20passing-brightgreen.svg)
![Coverage](https://img.shields.io/badge/coverage-comprehensive-brightgreen.svg)
![License](https://img.shields.io/badge/license-Proprietary-red.svg)

## Overview

Wekeza CRM is a modern, bank-grade Customer Relationship Management system built with .NET 8, following Clean Architecture and Domain-Driven Design principles. Designed specifically for financial institutions to manage customer relationships, interactions, and service delivery across multiple channels.

**✅ World-Class Testing:** 74 comprehensive tests with 100% pass rate across all layers.

## Features

### Customer Intelligence (360° View)
- ✅ Comprehensive customer profiles with KYC tracking
- ✅ Customer segmentation (Retail, SME, Corporate, High-Net-Worth)
- ✅ Credit scoring and risk profiling
- ✅ Lifetime value calculation
- ✅ Account linking and management

### Case Management
- ✅ Ticketing system for customer issues
- ✅ Priority-based case handling (Low, Medium, High, Critical)
- ✅ SLA tracking
- ✅ Case status workflow (Open, InProgress, Resolved, Closed, Escalated)
- ✅ Internal case notes and history

### Omni-Channel Engagement
- ✅ Multi-channel interaction tracking (Branch, Call Center, Email, SMS, WhatsApp, Mobile App, Web, ATM)
- ✅ Interaction history and timeline
- ✅ Customer communication log

### Sales & Marketing
- ✅ Campaign management
- ✅ Customer targeting by segment
- ✅ Campaign performance tracking

### Phase 2: AI & Automation Features ✨
- ✅ **AI-Powered Next Best Actions** - Smart recommendations for customer engagement
- ✅ **Customer Sentiment Analysis** - Analyze sentiment from interactions and cases
- ✅ **Workflow Automation Engine** - Define and execute automated workflows
- ✅ **Real-Time Notifications** - User notifications with read/unread tracking
- ✅ **Advanced Analytics Dashboards** - Comprehensive analytics for customers, cases, and interactions

### Phase 3: Communication & Reporting ✨
- ✅ **WhatsApp Business Integration** - Send/receive WhatsApp messages with delivery tracking
- ✅ **USSD Agent Banking** - Interactive USSD menus for mobile money operations
- ✅ **Advanced Reporting Engine** - Template-based reports with scheduling and export (PDF, Excel, CSV, JSON)

### Integration Ready
- ✅ RESTful API with OpenAPI/Swagger documentation
- ✅ JWT-based authentication
- ✅ CORS support for web/mobile integration
- ✅ Ready for core banking system integration

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

```
WekezaCRM/
├── src/
│   ├── Core/
│   │   ├── WekezaCRM.Domain/           # Domain entities, value objects, enums
│   │   └── WekezaCRM.Application/      # Business logic, DTOs, interfaces
│   ├── Infrastructure/
│   │   └── WekezaCRM.Infrastructure/   # Data access, EF Core, repositories
│   └── API/
│       └── WekezaCRM.API/             # REST API, controllers, middleware
├── tests/                              # Unit and integration tests
└── Document.md                         # CRM design benchmark study
```

### Technology Stack

- **.NET 8** - Latest LTS framework
- **Entity Framework Core 8** - ORM with SQL Server
- **ASP.NET Core Web API** - RESTful services
- **JWT Authentication** - Secure token-based auth
- **Swagger/OpenAPI** - API documentation
- **Clean Architecture** - Maintainable, testable code structure
- **Repository Pattern** - Data access abstraction

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019+ or SQL Server LocalDB
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/eodenyire/WekezaCRM.git
cd WekezaCRM
```

2. **Update database connection string**

Edit `src/API/WekezaCRM.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WekezaCRM;Trusted_Connection=true;"
  }
}
```

3. **Add EF Core tools (if not installed)**
```bash
dotnet tool install --global dotnet-ef
```

4. **Create database migration**
```bash
cd src/Infrastructure/WekezaCRM.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../../API/WekezaCRM.API
```

5. **Apply database migration**
```bash
dotnet ef database update --startup-project ../../API/WekezaCRM.API
```

6. **Build the solution**
```bash
cd ../../..
dotnet build
```

7. **Run the application**
```bash
cd src/API/WekezaCRM.API
dotnet run
```

8. **Access Swagger UI**
```
https://localhost:5001/swagger
```

## Testing

### World-Class Test Suite ✅

The Wekeza CRM includes comprehensive testing with **74 tests** across all layers:

| Test Category | Tests | Status |
|--------------|-------|--------|
| Domain Entity Tests | 41 | ✅ 100% Passing |
| Repository Tests | 13 | ✅ 100% Passing |
| API Integration Tests | 20 | ✅ 100% Passing |
| **TOTAL** | **74** | **✅ 100% Passing** |

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/WekezaCRM.Domain.Tests/WekezaCRM.Domain.Tests.csproj
dotnet test tests/WekezaCRM.Application.Tests/WekezaCRM.Application.Tests.csproj
dotnet test tests/WekezaCRM.API.Tests/WekezaCRM.API.Tests.csproj

# Run with detailed output
dotnet test --verbosity detailed
```

### Test Coverage

- ✅ All domain entities (Phase 1, 2, and 3)
- ✅ Repository CRUD operations
- ✅ Query methods and filtering
- ✅ API endpoint availability
- ✅ Entity validation and relationships
- ✅ Edge cases and null scenarios

**For detailed testing documentation, see [TESTING_GUIDE.md](TESTING_GUIDE.md)**

## API Endpoints

### Customers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customers` | Get all customers |
| GET | `/api/customers/{id}` | Get customer by ID |
| GET | `/api/customers/email/{email}` | Get customer by email |
| GET | `/api/customers/segment/{segment}` | Get customers by segment |
| POST | `/api/customers` | Create new customer |
| PUT | `/api/customers/{id}` | Update customer |
| DELETE | `/api/customers/{id}` | Delete customer |

### Cases

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cases` | Get all cases |
| GET | `/api/cases/{id}` | Get case by ID |
| GET | `/api/cases/customer/{customerId}` | Get cases by customer |
| POST | `/api/cases` | Create new case |
| PUT | `/api/cases/{id}/status` | Update case status |
| DELETE | `/api/cases/{id}` | Delete case |

### Next Best Actions (Phase 2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/nextbestactions` | Get all next best actions |
| GET | `/api/nextbestactions/{id}` | Get action by ID |
| GET | `/api/nextbestactions/customer/{customerId}` | Get actions for customer |
| GET | `/api/nextbestactions/customer/{customerId}/pending` | Get pending actions |
| POST | `/api/nextbestactions/generate/{customerId}` | Generate AI recommendations |
| PUT | `/api/nextbestactions/{id}/complete` | Complete an action |
| DELETE | `/api/nextbestactions/{id}` | Delete action |

### Sentiment Analysis (Phase 2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/sentimentanalysis` | Get all sentiment analyses |
| GET | `/api/sentimentanalysis/{id}` | Get analysis by ID |
| GET | `/api/sentimentanalysis/customer/{customerId}` | Get sentiment for customer |
| POST | `/api/sentimentanalysis/analyze` | Analyze sentiment of text |

### Workflows (Phase 2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/workflows/definitions` | Get all workflow definitions |
| GET | `/api/workflows/definitions/active` | Get active workflows |
| POST | `/api/workflows/definitions` | Create workflow definition |
| PUT | `/api/workflows/definitions/{id}` | Update workflow definition |
| GET | `/api/workflows/instances` | Get all workflow instances |
| POST | `/api/workflows/instances/trigger` | Trigger a workflow |
| PUT | `/api/workflows/instances/{id}/status` | Update instance status |

### Notifications (Phase 2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | Get all notifications |
| GET | `/api/notifications/user/{userId}` | Get user notifications |
| GET | `/api/notifications/user/{userId}/unread` | Get unread notifications |
| POST | `/api/notifications` | Create notification |
| PUT | `/api/notifications/{id}/read` | Mark as read |
| DELETE | `/api/notifications/{id}` | Delete notification |

### Analytics (Phase 2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/analytics/customers` | Get customer analytics |
| GET | `/api/analytics/cases` | Get case analytics |
| GET | `/api/analytics/interactions` | Get interaction analytics |
| GET | `/api/analytics/dashboard` | Get comprehensive dashboard |

## Domain Models

### Customer Segments
- **Retail** - Individual customers
- **SME** - Small and Medium Enterprises
- **Corporate** - Large corporate clients
- **HighNetWorth** - Premium banking customers

### Case Priorities
- **Low** - Standard issues
- **Medium** - Important issues
- **High** - Urgent issues requiring immediate attention
- **Critical** - Business-critical issues

### Case Status
- **Open** - Newly created case
- **InProgress** - Being worked on
- **PendingCustomer** - Waiting for customer response
- **Resolved** - Issue resolved
- **Closed** - Case closed
- **Escalated** - Escalated to higher authority

### Interaction Channels
- Branch
- Call Center
- Email
- SMS
- WhatsApp
- Mobile App
- Web
- ATM

## Configuration

### JWT Settings
```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "WekezaCRM",
    "Audience": "WekezaCRM",
    "ExpiryMinutes": 60
  }
}
```

### Database
The system uses SQL Server by default. Connection strings can be configured in `appsettings.json`.

## Security

- JWT-based authentication for API access
- Role-based authorization (ready for implementation)
- CORS policy configuration
- Secure password handling
- Audit trails for all operations

## Integration with Wekeza Core Banking

This CRM system is designed to integrate seamlessly with the [Wekeza Core Banking System](https://github.com/eodenyire/Wekeza). Key integration points:

1. **Customer Synchronization** - Sync customer data between CRM and core banking
2. **Account Data** - Real-time account balance and transaction history
3. **Transaction Events** - Trigger CRM interactions based on banking transactions
4. **KYC Updates** - Bidirectional KYC status synchronization
5. **Product Recommendations** - Use banking behavior for cross-sell opportunities

## Design Philosophy

Based on world-class CRM systems (Salesforce, Microsoft Dynamics, Zendesk), this system combines:

- **Salesforce Power** - Robust features and extensibility
- **Zendesk Experience** - Superior customer service capabilities  
- **HubSpot Usability** - Clean, intuitive interface
- **Africa-Optimized** - WhatsApp native, USSD integration ready, agent banking support

For more details, see [Document.md](Document.md) for the complete design benchmark study.

## Future Roadmap

### Phase 2 Enhancements ✅ COMPLETED
- [x] AI-powered next best action recommendations
- [x] Customer sentiment analysis
- [x] Automated workflow engine
- [x] Real-time notifications
- [x] Advanced analytics dashboards

### Phase 3 Features ✅ COMPLETED
- [x] WhatsApp Business API integration - Full messaging support with delivery tracking
- [x] USSD support for agent banking - Interactive menu system for mobile money
- [x] Advanced reporting engine - Template-based reports with scheduling and multiple formats
- [ ] Mobile CRM app (Flutter/React Native) - Planned for future release
- [ ] Desktop client (Electron) - Planned for future release

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## Support

For technical support or questions:
- Email: dev@wekeza.com
- Documentation: See `/docs` folder
- Issues: https://github.com/eodenyire/WekezaCRM/issues

---

Built with ❤️ for Wekeza Bank by the Engineering Team
