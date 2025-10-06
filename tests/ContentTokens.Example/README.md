# ContentTokens.Example

This is an example ASP.NET Core web application demonstrating how to use the ContentTokens addon.

## Running the Example

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the example project:
   ```bash
   cd tests/ContentTokens.Example
   dotnet run
   ```

3. Open your browser and navigate to `http://localhost:5000` (or the URL shown in the console)

## Creating Sample Tokens

Use the REST API to create tokens. Here are some examples:

### Create a Company Name token:
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CompanyName",
    "value": "Acme Corporation",
    "description": "Company name used throughout the site"
  }'
```

### Create a Support Email token:
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "SupportEmail",
    "value": "support@acme.com",
    "description": "Support email address"
  }'
```

### Create a Phone Number token:
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "PhoneNumber",
    "value": "+1-555-0123",
    "description": "Support phone number"
  }'
```

### Create a multilingual Welcome Message (English):
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Welcome to our website!",
    "languageCode": "en",
    "description": "Welcome message in English"
  }'
```

### Create a multilingual Welcome Message (Swedish):
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Välkommen till vår webbplats!",
    "languageCode": "sv",
    "description": "Welcome message in Swedish"
  }'
```

## Viewing Tokens

List all tokens:
```bash
curl http://localhost:5000/api/contenttokens
```

Get a specific token:
```bash
curl http://localhost:5000/api/contenttokens/CompanyName
```

## Testing Token Replacement

After creating tokens, refresh the homepage in your browser. You should see the `{{TokenName}}` placeholders replaced with the actual values you configured!

## What's Happening

1. The `Program.cs` configures the ContentTokens service
2. The homepage contains HTML with token placeholders like `{{CompanyName}}`
3. The service replaces token placeholders with their configured values
4. All token placeholders are replaced when content is processed
5. The final HTML is sent to the browser with actual values instead of placeholders

## Integration with Optimizely CMS

In a real Optimizely CMS installation:
- The initialization module would automatically register services
- The admin gadget would provide a UI for managing tokens
- Editors could use tokens in content blocks, pages, and properties
- The middleware would work the same way to replace tokens in rendered content
