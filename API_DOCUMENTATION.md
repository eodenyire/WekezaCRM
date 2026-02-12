# Wekeza CRM API Documentation

## Base URL

```
Development: https://localhost:5001/api
Production: https://api.wekeza.com/api
```

## Authentication

The API uses JWT (JSON Web Token) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer {your_jwt_token}
```

## Response Format

### Success Response
```json
{
  "data": { ... },
  "message": "Success",
  "statusCode": 200
}
```

### Error Response
```json
{
  "error": "Error message",
  "statusCode": 400
}
```

## Endpoints

### Health Check

#### GET /api/health
Check API health status.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-12T10:00:00Z",
  "version": "1.0.0",
  "service": "Wekeza CRM API"
}
```

---

## Customers

### GET /api/customers
Get all customers.

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+254712345678",
    "dateOfBirth": "1990-01-15",
    "address": "123 Main St",
    "city": "Nairobi",
    "country": "Kenya",
    "segment": "Retail",
    "kycStatus": "Verified",
    "customerReference": "CUS-20260212100000-abc123",
    "creditScore": 750.00,
    "lifetimeValue": 50000.00,
    "riskScore": 25,
    "isActive": true
  }
]
```

### GET /api/customers/{id}
Get customer by ID.

**Parameters:**
- `id` (path, required): Customer GUID

**Response:** Single customer object

### GET /api/customers/email/{email}
Get customer by email address.

**Parameters:**
- `email` (path, required): Customer email

**Response:** Single customer object

### GET /api/customers/segment/{segment}
Get customers by segment.

**Parameters:**
- `segment` (path, required): Customer segment (Retail, SME, Corporate, HighNetWorth)

**Response:** Array of customer objects

### POST /api/customers
Create a new customer.

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+254712345678",
  "dateOfBirth": "1990-01-15",
  "address": "123 Main St",
  "city": "Nairobi",
  "country": "Kenya",
  "segment": "Retail"
}
```

**Response:** Created customer object with 201 status

### PUT /api/customers/{id}
Update an existing customer.

**Parameters:**
- `id` (path, required): Customer GUID

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+254712345678",
  "address": "456 Oak Ave",
  "city": "Nairobi",
  "country": "Kenya",
  "segment": "SME"
}
```

**Response:** Updated customer object

### DELETE /api/customers/{id}
Delete a customer.

**Parameters:**
- `id` (path, required): Customer GUID

**Response:** 204 No Content

---

## Cases

### GET /api/cases
Get all cases.

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "caseNumber": "CASE-20260212100000-xyz789",
    "title": "Account Balance Discrepancy",
    "description": "Customer reports incorrect balance on savings account",
    "status": "Open",
    "priority": "High",
    "category": "Account Issue",
    "subCategory": "Balance",
    "createdAt": "2026-02-12T10:00:00Z",
    "createdBy": "System"
  }
]
```

### GET /api/cases/{id}
Get case by ID.

**Parameters:**
- `id` (path, required): Case GUID

**Response:** Single case object

### GET /api/cases/customer/{customerId}
Get all cases for a customer.

**Parameters:**
- `customerId` (path, required): Customer GUID

**Response:** Array of case objects

### POST /api/cases
Create a new case.

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Account Balance Discrepancy",
  "description": "Customer reports incorrect balance on savings account",
  "priority": "High",
  "category": "Account Issue",
  "subCategory": "Balance"
}
```

**Response:** Created case object with 201 status

### PUT /api/cases/{id}/status
Update case status.

**Parameters:**
- `id` (path, required): Case GUID

**Request Body:**
```json
{
  "status": "Resolved",
  "resolution": "Balance corrected after investigation. System glitch identified and fixed."
}
```

**Response:** Updated case object

### DELETE /api/cases/{id}
Delete a case.

**Parameters:**
- `id` (path, required): Case GUID

**Response:** 204 No Content

---

## Interactions

### GET /api/interactions
Get all interactions.

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "channel": "CallCenter",
    "subject": "Balance Inquiry",
    "description": "Customer called to check account balance",
    "interactionDate": "2026-02-12T10:00:00Z",
    "durationMinutes": 5
  }
]
```

### GET /api/interactions/{id}
Get interaction by ID.

**Parameters:**
- `id` (path, required): Interaction GUID

**Response:** Single interaction object

### GET /api/interactions/customer/{customerId}
Get all interactions for a customer.

**Parameters:**
- `customerId` (path, required): Customer GUID

**Response:** Array of interaction objects

### POST /api/interactions
Create a new interaction.

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "channel": "CallCenter",
  "subject": "Balance Inquiry",
  "description": "Customer called to check account balance",
  "interactionDate": "2026-02-12T10:00:00Z",
  "durationMinutes": 5
}
```

**Response:** Created interaction object with 201 status

### DELETE /api/interactions/{id}
Delete an interaction.

**Parameters:**
- `id` (path, required): Interaction GUID

**Response:** 204 No Content

---

## Phase 2: AI & Automation APIs

### Next Best Actions

#### GET /api/nextbestactions
Get all next best actions.

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "actionType": "ProductRecommendation",
    "title": "Recommend Premium Savings Account",
    "description": "Customer profile indicates readiness for premium account upgrade",
    "confidenceScore": 0.85,
    "recommendedProduct": "Premium Savings Account",
    "recommendedDate": "2026-02-12T10:00:00Z",
    "isCompleted": false
  }
]
```

#### POST /api/nextbestactions/generate/{customerId}
Generate AI-powered recommendations for a customer.

**Parameters:**
- `customerId` (path, required): Customer GUID

**Response:** Array of generated next best action recommendations

#### PUT /api/nextbestactions/{id}/complete
Mark a next best action as completed.

**Request Body:**
```json
{
  "outcome": "Customer accepted premium account upgrade"
}
```

**Response:** Updated action object

---

### Sentiment Analysis

#### POST /api/sentimentanalysis/analyze
Analyze sentiment of text (interaction or case notes).

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "text": "I am very happy with the service provided",
  "interactionId": "optional-interaction-id",
  "caseId": "optional-case-id"
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sentimentType": "Positive",
  "sentimentScore": 0.8,
  "textAnalyzed": "I am very happy with the service provided",
  "analyzedDate": "2026-02-12T10:00:00Z",
  "keyPhrases": "happy, service, provided"
}
```

#### GET /api/sentimentanalysis/customer/{customerId}
Get sentiment analysis history for a customer.

**Parameters:**
- `customerId` (path, required): Customer GUID

**Response:** Array of sentiment analysis objects

---

### Workflows

#### GET /api/workflows/definitions
Get all workflow definitions.

**Response:** Array of workflow definition objects

#### POST /api/workflows/definitions
Create a new workflow definition.

**Request Body:**
```json
{
  "name": "Case Escalation Workflow",
  "description": "Automatically escalate cases after 48 hours",
  "triggerType": "CaseAgeExceeded",
  "triggerConditions": "{\"hours\": 48}",
  "actions": "[{\"type\": \"Escalate\"}, {\"type\": \"Notify\"}]",
  "isActive": true,
  "executionOrder": 1
}
```

**Response:** Created workflow definition

#### POST /api/workflows/instances/trigger
Trigger a workflow instance.

**Request Body:**
```json
{
  "workflowDefinitionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerId": "optional-customer-id",
  "caseId": "optional-case-id",
  "context": "optional-context-json"
}
```

**Response:** Created workflow instance

---

### Notifications

#### GET /api/notifications/user/{userId}/unread
Get unread notifications for a user.

**Parameters:**
- `userId` (path, required): User GUID

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "type": "Alert",
    "title": "High Priority Case Assigned",
    "message": "You have been assigned a high priority case",
    "isRead": false,
    "actionUrl": "/cases/123",
    "createdAt": "2026-02-12T10:00:00Z"
  }
]
```

#### POST /api/notifications
Create a notification.

**Request Body:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "type": "Info",
  "title": "New Customer Inquiry",
  "message": "A customer has submitted a new inquiry",
  "actionUrl": "/inquiries/456"
}
```

**Response:** Created notification object

#### PUT /api/notifications/{id}/read
Mark a notification as read.

**Parameters:**
- `id` (path, required): Notification GUID

**Response:** 204 No Content

---

### Analytics

#### GET /api/analytics/dashboard
Get comprehensive analytics dashboard.

**Query Parameters:**
- `startDate` (optional): Start date for analytics period
- `endDate` (optional): End date for analytics period

**Response:**
```json
{
  "customerAnalytics": {
    "totalCustomers": 1250,
    "activeCustomers": 1100,
    "newCustomersThisMonth": 45,
    "customersBySegment": {
      "Retail": 850,
      "SME": 250,
      "Corporate": 100,
      "HighNetWorth": 50
    },
    "customersByKYCStatus": {
      "Verified": 1000,
      "Pending": 150,
      "InProgress": 100
    }
  },
  "caseAnalytics": {
    "totalCases": 320,
    "openCases": 45,
    "resolvedCases": 265,
    "averageResolutionTimeHours": 18.5,
    "casesByPriority": {
      "Low": 120,
      "Medium": 150,
      "High": 40,
      "Critical": 10
    }
  },
  "interactionAnalytics": {
    "totalInteractions": 5420,
    "interactionsThisMonth": 450,
    "interactionsByChannel": {
      "Branch": 2100,
      "CallCenter": 1800,
      "Email": 950,
      "WhatsApp": 570
    },
    "averageInteractionDurationMinutes": 12.3
  },
  "generatedAt": "2026-02-12T10:00:00Z"
}
```

#### GET /api/analytics/customers
Get customer-specific analytics.

**Response:** Customer analytics object

#### GET /api/analytics/cases
Get case-specific analytics.

**Response:** Case analytics object

#### GET /api/analytics/interactions
Get interaction-specific analytics.

**Response:** Interaction analytics object

---

## Enumerations

### Customer Segments
- `Retail` - Individual customers
- `SME` - Small and Medium Enterprises
- `Corporate` - Large corporate clients
- `HighNetWorth` - Premium banking customers

### KYC Status
- `Pending` - KYC not started
- `InProgress` - KYC verification in progress
- `Verified` - KYC verified and approved
- `Rejected` - KYC rejected
- `Expired` - KYC documents expired

### Case Status
- `Open` - Newly created case
- `InProgress` - Being worked on
- `PendingCustomer` - Waiting for customer response
- `Resolved` - Issue resolved
- `Closed` - Case closed
- `Escalated` - Escalated to higher authority

### Case Priority
- `Low` - Standard issues (SLA: 5 business days)
- `Medium` - Important issues (SLA: 2 business days)
- `High` - Urgent issues (SLA: 24 hours)
- `Critical` - Business-critical (SLA: 4 hours)

### Interaction Channels
- `Branch` - In-person at bank branch
- `CallCenter` - Phone call
- `Email` - Email communication
- `SMS` - Text message
- `WhatsApp` - WhatsApp message
- `MobileApp` - Mobile banking app
- `Web` - Web portal
- `ATM` - ATM transaction

### Action Types (Phase 2)
- `ProductRecommendation` - Recommend a new product
- `FollowUpCall` - Schedule follow-up call
- `SendEmail` - Send email communication
- `SendSMS` - Send SMS message
- `ScheduleMeeting` - Schedule meeting with customer
- `UpgradeAccount` - Suggest account upgrade
- `CrossSell` - Cross-sell opportunity
- `RetentionOffer` - Retention offer for at-risk customer
- `RiskReview` - Review customer risk profile

### Sentiment Types (Phase 2)
- `Positive` - Positive customer sentiment
- `Neutral` - Neutral sentiment
- `Negative` - Negative sentiment
- `VeryNegative` - Very negative sentiment requiring attention

### Notification Types (Phase 2)
- `Info` - Informational notification
- `Warning` - Warning notification
- `Alert` - Alert requiring attention
- `Success` - Success notification
- `Error` - Error notification

### Workflow Status (Phase 2)
- `Draft` - Workflow in draft state
- `Active` - Active and running
- `Paused` - Temporarily paused
- `Completed` - Successfully completed
- `Failed` - Failed with errors
- `Cancelled` - Manually cancelled

---

## Error Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful deletion) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing or invalid token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## Rate Limiting

The API implements rate limiting to prevent abuse:

- **Unauthenticated requests:** 100 requests per hour
- **Authenticated requests:** 1000 requests per hour

Rate limit headers:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 2026-02-12T11:00:00Z
```

---

## Pagination

Endpoints that return lists support pagination:

**Query Parameters:**
- `page` (default: 1) - Page number
- `pageSize` (default: 50, max: 100) - Items per page

**Response Headers:**
```
X-Total-Count: 500
X-Page-Number: 1
X-Page-Size: 50
Link: <https://api.wekeza.com/api/customers?page=2>; rel="next"
```

---

## Example Usage

### Create Customer and Log Interaction

#### 1. Create Customer
```bash
curl -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@example.com",
    "phoneNumber": "+254723456789",
    "segment": "Retail"
  }'
```

**Response:**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  ...
}
```

#### 2. Log Interaction
```bash
curl -X POST https://localhost:5001/api/interactions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "channel": "Branch",
    "subject": "Account Opening",
    "description": "Customer opened a new savings account",
    "durationMinutes": 30
  }'
```

---

## Webhooks (Future Feature)

Coming soon: Real-time notifications for:
- New customer registration
- Case status changes
- High-priority case creation
- Customer segment changes

---

## SDK Support

Official SDKs coming soon for:
- JavaScript/TypeScript
- Python
- C#
- Java

---

## Support

For API support:
- **Email:** dev@wekeza.com
- **Documentation:** https://docs.wekeza.com
- **Status Page:** https://status.wekeza.com
- **GitHub Issues:** https://github.com/eodenyire/WekezaCRM/issues
