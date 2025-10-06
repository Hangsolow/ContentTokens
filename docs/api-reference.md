# API Reference

Complete reference for the ContentTokens REST API.

## Base URL

```
/api/contenttokens
```

## Authentication

By default, the API requires authentication and authorization. Users must have one of these roles:
- `CmsAdmins`
- `CmsEditors`
- `WebAdmins`

For the example project, authentication is disabled with `[AllowAnonymous]`.

## Endpoints

### Get All Tokens

Retrieve all content tokens, optionally filtered by language.

**Endpoint:** `GET /api/contenttokens`

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| languageCode | string | No | Filter tokens by language code (e.g., "en", "sv", "de") |

**Request Example:**

```bash
# Get all tokens
curl http://localhost:5000/api/contenttokens

# Get tokens for a specific language
curl http://localhost:5000/api/contenttokens?languageCode=en
```

**Response:**

```json
[
  {
    "id": {},
    "name": "CompanyName",
    "value": "Acme Corporation",
    "languageCode": null,
    "description": "Company name used throughout the site",
    "created": "2025-01-06T10:00:00Z",
    "modified": "2025-01-06T10:00:00Z"
  },
  {
    "id": {},
    "name": "WelcomeMessage",
    "value": "Welcome to our website!",
    "languageCode": "en",
    "description": "Welcome message in English",
    "created": "2025-01-06T11:00:00Z",
    "modified": "2025-01-06T11:00:00Z"
  }
]
```

**Status Codes:**
- `200 OK` - Success
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions

---

### Get Token by Name

Retrieve a specific token by its name.

**Endpoint:** `GET /api/contenttokens/{name}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| name | string | Yes | Token name |

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| languageCode | string | No | Language code for language-specific token |

**Request Example:**

```bash
# Get token without language
curl http://localhost:5000/api/contenttokens/CompanyName

# Get language-specific token
curl http://localhost:5000/api/contenttokens/WelcomeMessage?languageCode=en
```

**Response:**

```json
{
  "id": {},
  "name": "CompanyName",
  "value": "Acme Corporation",
  "languageCode": null,
  "description": "Company name used throughout the site",
  "created": "2025-01-06T10:00:00Z",
  "modified": "2025-01-06T10:00:00Z"
}
```

**Status Codes:**
- `200 OK` - Token found
- `404 Not Found` - Token does not exist
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions

---

### Create or Update Token

Create a new token or update an existing one.

**Endpoint:** `POST /api/contenttokens`

**Request Headers:**
```
Content-Type: application/json
```

**Request Body:**

```json
{
  "name": "string (required)",
  "value": "string (required)",
  "languageCode": "string (optional)",
  "description": "string (optional)"
}
```

**Field Descriptions:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Token name (alphanumeric only) |
| value | string | Yes | Token value/content |
| languageCode | string | No | Language code (e.g., "en", "sv", "de"). Null for language-neutral |
| description | string | No | Description of the token's purpose |
| id | object | No | For updates, include the existing token's ID |

**Request Example:**

```bash
# Create new token
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CompanyName",
    "value": "Acme Corporation",
    "description": "Company name used throughout the site"
  }'

# Create language-specific token
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Welcome to our website!",
    "languageCode": "en",
    "description": "English welcome message"
  }'

# Update existing token
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "id": {"InternalId": "guid-here"},
    "name": "CompanyName",
    "value": "New Company Name",
    "description": "Updated company name"
  }'
```

**Response:**

```json
{
  "id": {},
  "name": "CompanyName",
  "value": "Acme Corporation",
  "languageCode": null,
  "description": "Company name used throughout the site",
  "created": "2025-01-06T10:00:00Z",
  "modified": "2025-01-06T10:00:00Z"
}
```

**Status Codes:**
- `200 OK` - Token created or updated successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions

---

### Delete Token

Delete a token by its ID.

**Endpoint:** `DELETE /api/contenttokens/{id}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | guid | Yes | Token ID |

**Request Example:**

```bash
curl -X DELETE http://localhost:5000/api/contenttokens/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

**Response:**

No content (empty response body)

**Status Codes:**
- `204 No Content` - Token deleted successfully
- `404 Not Found` - Token does not exist
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions

---

### Preview Token Replacement

Test token replacement without storing the result.

**Endpoint:** `POST /api/contenttokens/preview`

**Request Headers:**
```
Content-Type: application/json
```

**Request Body:**

```json
{
  "text": "string (required)",
  "languageCode": "string (optional)"
}
```

**Field Descriptions:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| text | string | Yes | Text containing tokens to replace |
| languageCode | string | No | Language code for language-specific replacement |

**Request Example:**

```bash
curl -X POST http://localhost:5000/api/contenttokens/preview \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Welcome to {{CompanyName}}! Contact us at {{SupportEmail}}.",
    "languageCode": "en"
  }'
```

**Response:**

```json
{
  "original": "Welcome to {{CompanyName}}! Contact us at {{SupportEmail}}.",
  "replaced": "Welcome to Acme Corporation! Contact us at support@acme.com."
}
```

**Status Codes:**
- `200 OK` - Preview generated successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions

---

## Data Models

### ContentToken

```csharp
{
  "id": Identity,           // Unique identifier
  "name": string,           // Token name (alphanumeric)
  "value": string,          // Token content/value
  "languageCode": string?,  // Language code (null for neutral)
  "description": string?,   // Token description
  "created": DateTime,      // Creation timestamp (UTC)
  "modified": DateTime      // Last modification timestamp (UTC)
}
```

### PreviewRequest

```csharp
{
  "text": string,          // Text with tokens
  "languageCode": string?  // Optional language code
}
```

### PreviewResponse

```csharp
{
  "original": string,   // Original text
  "replaced": string    // Text with tokens replaced
}
```

## Error Responses

### Error Format

```json
{
  "error": "Error message description"
}
```

### Common Errors

**400 Bad Request**
```json
{
  "error": "Token name is required"
}
```

**401 Unauthorized**
```json
{
  "error": "Authentication required"
}
```

**403 Forbidden**
```json
{
  "error": "Insufficient permissions to manage tokens"
}
```

**404 Not Found**
```json
{
  "error": "Token not found"
}
```

**500 Internal Server Error**
```json
{
  "error": "An unexpected error occurred"
}
```

## Code Examples

### C# Example

```csharp
using System.Net.Http;
using System.Text.Json;

public class ContentTokensApiClient
{
    private readonly HttpClient _httpClient;
    
    public ContentTokensApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IEnumerable<ContentToken>> GetAllTokensAsync(string? languageCode = null)
    {
        var url = "/api/contenttokens";
        if (!string.IsNullOrEmpty(languageCode))
            url += $"?languageCode={languageCode}";
            
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<ContentToken>>();
    }
    
    public async Task<ContentToken> CreateTokenAsync(ContentToken token)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/contenttokens", token);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<ContentToken>();
    }
    
    public async Task DeleteTokenAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/api/contenttokens/{id}");
        response.EnsureSuccessStatusCode();
    }
}
```

### JavaScript Example

```javascript
class ContentTokensAPI {
  constructor(baseUrl = '/api/contenttokens') {
    this.baseUrl = baseUrl;
  }
  
  async getAllTokens(languageCode = null) {
    const url = languageCode 
      ? `${this.baseUrl}?languageCode=${languageCode}`
      : this.baseUrl;
      
    const response = await fetch(url);
    return await response.json();
  }
  
  async createToken(token) {
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(token)
    });
    
    return await response.json();
  }
  
  async deleteToken(id) {
    await fetch(`${this.baseUrl}/${id}`, {
      method: 'DELETE'
    });
  }
  
  async previewReplacement(text, languageCode = null) {
    const response = await fetch(`${this.baseUrl}/preview`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ text, languageCode })
    });
    
    return await response.json();
  }
}
```

## Rate Limiting

Currently, no rate limiting is implemented. For production use, consider adding rate limiting middleware.

## Versioning

The current API version is `1.0`. Future versions will be indicated in the URL path (e.g., `/api/v2/contenttokens`).

## Next Steps

- [Configuration Guide](configuration.md) - Configure API behavior
- [Examples](examples.md) - More usage examples
- [Troubleshooting](troubleshooting.md) - API troubleshooting
