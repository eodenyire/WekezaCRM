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
