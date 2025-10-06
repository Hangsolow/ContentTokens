# Copilot Instructions for ContentTokens

## Project Overview

ContentTokens is an Optimizely CMS addon that enables editors to define reusable, multilingual content tokens that can be used throughout the site. Tokens use the syntax `{{TokenName}}` and are automatically replaced at runtime with their configured values.

### Key Features
- Multilingual support with language-specific token values
- REST API for programmatic token management
- Admin gadget for easy token management in Optimizely CMS
- Automatic token replacement via service layer
- TinyMCE and Dojo plugins for token autocomplete

## Technology Stack

- **.NET 8.0** - Primary framework
- **ASP.NET Core** - Web framework
- **Optimizely CMS 12.x** - Content Management System
- **Dojo Toolkit** - Admin UI (Optimizely standard)
- **TinyMCE** - Rich text editor integration
- **Dynamic Data Store (DDS)** - Data persistence

## Architecture

### Core Components

1. **Models** (`src/ContentTokens/Models/`)
   - `ContentToken.cs` - Data model for tokens with properties: Id, Name, Value, LanguageCode, Description, Created, Modified

2. **Services** (`src/ContentTokens/Services/`)
   - `IContentTokenService` - Interface for token operations
   - `ContentTokenService` - Implementation with token replacement logic using regex pattern `\{\{(\w+)\}\}`

3. **Controllers** (`src/ContentTokens/Controllers/`)
   - `ContentTokensController` - REST API endpoints for CRUD operations

4. **Client Resources** (`src/ContentTokens/ClientResources/`)
   - `ContentTokensGadget.js` - Dojo-based admin UI widget
   - TinyMCE and Dojo plugins for editor integration

### Token Syntax

- **Valid**: `{{CompanyName}}`, `{{SupportEmail}}`, `{{PhoneNumber2024}}`
- **Invalid**: `{{Company Name}}` (space), `{{Support-Email}}` (hyphen), `{{Phone#}}` (special char)
- Token names must be alphanumeric only (letters and numbers)
- Token names are case-sensitive

### Data Storage

Tokens are stored in Optimizely's Dynamic Data Store (DDS):
- No database migrations needed
- Automatic schema management
- LINQ query support
- Built-in versioning

## Coding Standards

### C# Conventions

Follow Microsoft's C# Coding Conventions:

```csharp
/// <summary>
/// Gets a token by name and optional language code.
/// </summary>
/// <param name="name">The token name</param>
/// <param name="languageCode">Optional language code (e.g., "en", "sv")</param>
/// <returns>The token if found, otherwise null</returns>
public ContentToken? GetToken(string name, string? languageCode = null)
{
    // Implementation
}
```

**Key Points:**
- Use XML documentation comments for public APIs
- Use nullable reference types appropriately
- Keep methods focused and small
- Use meaningful variable and method names
- Prefer LINQ where appropriate

### JavaScript/Dojo Conventions

For admin UI and plugins:

```javascript
/**
 * Loads all tokens from the API
 */
loadTokens: function () {
    request.get("/api/contenttokens", {
        handleAs: "json"
    }).then(function (tokens) {
        // Handle tokens
    });
}
```

**Key Points:**
- Use semicolons
- Use double quotes for strings
- Follow Dojo module pattern
- Add JSDoc comments for functions

### File Organization

```
src/ContentTokens/
├── Models/              # Data models
├── Services/            # Business logic
├── Controllers/         # REST API endpoints
├── Extensions/          # Extension methods
├── Gadgets/             # Admin gadget registration
├── ClientResources/     # JavaScript/CSS
│   └── Scripts/         # Dojo widgets and plugins
├── EditorDescriptors/   # Editor customizations
└── Infrastructure/      # Supporting code
```

## Common Patterns

### Token Replacement

```csharp
// Service layer replacement
var text = "Welcome to {{CompanyName}}!";
var result = tokenService.ReplaceTokens(text, languageCode: "en");
```

### Creating Tokens

```csharp
var token = new ContentToken
{
    Name = "CompanyName",
    Value = "Acme Corporation",
    LanguageCode = "en", // Optional
    Description = "Company name used throughout the site"
};
tokenService.SaveToken(token);
```

### Multilingual Tokens

```csharp
// English version
tokenService.SaveToken(new ContentToken 
{ 
    Name = "WelcomeMessage", 
    Value = "Welcome!", 
    LanguageCode = "en" 
});

// Swedish version
tokenService.SaveToken(new ContentToken 
{ 
    Name = "WelcomeMessage", 
    Value = "Välkommen!", 
    LanguageCode = "sv" 
});
```

### API Usage

```bash
# Get all tokens
GET /api/contenttokens?languageCode=en

# Get specific token
GET /api/contenttokens/CompanyName?languageCode=en

# Create/Update token
POST /api/contenttokens
Content-Type: application/json
{
  "name": "CompanyName",
  "value": "Acme Corporation",
  "languageCode": "en",
  "description": "Company name"
}

# Delete token
DELETE /api/contenttokens/{id}

# Preview replacement
POST /api/contenttokens/preview
{
  "text": "Welcome to {{CompanyName}}!",
  "languageCode": "en"
}
```

## Testing Guidelines

### Unit Tests

When adding unit tests (if test project exists):

```csharp
[Fact]
public void ReplaceTokens_WithValidToken_ReplacesCorrectly()
{
    // Arrange
    var service = new ContentTokenService();
    service.SaveToken(new ContentToken
    {
        Name = "CompanyName",
        Value = "Acme Corporation"
    });
    var text = "Welcome to {{CompanyName}}!";

    // Act
    var result = service.ReplaceTokens(text);

    // Assert
    result.Should().Be("Welcome to Acme Corporation!");
}
```

**Key Points:**
- Use Arrange-Act-Assert pattern
- Use meaningful test names
- Test edge cases (missing tokens, multiple tokens, language fallback)
- Use FluentAssertions for assertions

### Integration Tests

Test end-to-end scenarios:
- Token replacement in HTTP responses
- API CRUD operations
- Multilingual token resolution
- Middleware integration

## Build and Run

### Building

```bash
# Build the solution
dotnet build

# Build specific project
dotnet build src/ContentTokens/ContentTokens.csproj
```

### Running Examples

```bash
# Simple example (no database required)
cd tests/ContentTokens.Example
dotnet run

# Full CMS example (requires SQL Server)
cd tests/ContentTokens.CmsExample
dotnet run
```

## Common Tasks

### Adding a New Feature

1. Update the appropriate service/controller
2. Add tests if test infrastructure exists
3. Update documentation in `/docs`
4. Update CHANGELOG.md
5. Follow existing code patterns

### Modifying the Admin UI

1. Edit `ClientResources/Scripts/ContentTokensGadget.js`
2. Follow Dojo patterns
3. Test in Optimizely CMS admin interface
4. Maintain consistent styling with Optimizely

### Adding New API Endpoints

1. Add method to `ContentTokensController`
2. Follow REST conventions
3. Add XML documentation
4. Return appropriate HTTP status codes
5. Update API documentation in `/docs/api-reference.md`

## Documentation

All documentation is in the `/docs` folder:

- **getting-started.md** - Quick start guide
- **installation.md** - Installation instructions
- **configuration.md** - Configuration options
- **api-reference.md** - Complete API documentation
- **advanced-usage.md** - Usage examples
- **testing.md** - Testing strategies
- **architecture.md** - Architecture details
- **troubleshooting.md** - Common issues

**When adding features**, update relevant documentation files.

## Important Constraints

### Token Name Rules
- Must be alphanumeric only (no spaces, hyphens, underscores, or special characters)
- Case-sensitive
- Must match regex pattern: `\w+` (word characters)

### Performance Considerations
- Regex pattern is compiled for performance
- Consider caching for high-traffic sites
- Middleware processes only HTML responses

### Security
- Don't store sensitive data in tokens
- API should require authentication in production
- Token values are rendered as-is (no automatic HTML encoding)
- Use appropriate authorization for token management

## Example Workflows

### Adding Multilingual Support to a Token

```csharp
// 1. Create base token (fallback)
var baseToken = new ContentToken { Name = "Greeting", Value = "Hello" };
tokenService.SaveToken(baseToken);

// 2. Add language-specific versions
var enToken = new ContentToken 
{ 
    Name = "Greeting", 
    Value = "Hello", 
    LanguageCode = "en" 
};
tokenService.SaveToken(enToken);

var svToken = new ContentToken 
{ 
    Name = "Greeting", 
    Value = "Hej", 
    LanguageCode = "sv" 
};
tokenService.SaveToken(svToken);

// 3. Use in content
var text = "{{Greeting}}, welcome!";
var englishResult = tokenService.ReplaceTokens(text, "en"); // "Hello, welcome!"
var swedishResult = tokenService.ReplaceTokens(text, "sv"); // "Hej, welcome!"
```

### Bulk Token Import

```csharp
public async Task ImportTokensAsync(string jsonFilePath)
{
    var json = await File.ReadAllTextAsync(jsonFilePath);
    var tokens = JsonSerializer.Deserialize<ContentToken[]>(json);
    
    foreach (var token in tokens)
    {
        _tokenService.SaveToken(token);
    }
}
```

## Troubleshooting

### Tokens Not Replaced
- Verify token exists in DDS
- Check token name matches exactly (case-sensitive)
- Ensure middleware is registered: `app.UseContentTokens()`
- Verify content type is HTML

### Admin Gadget Not Visible
- Check `module.config` is embedded
- Verify module initialization
- Clear browser cache
- Check CMS permissions

### Performance Issues
- Enable output caching
- Consider pre-processing at publish time
- Optimize number of tokens
- Monitor middleware performance

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for detailed guidelines.

**Quick Checklist:**
- Follow existing code style
- Add tests for new features
- Update documentation
- Keep commits focused and well-described
- Ensure no build warnings

## Resources

- **Repository**: https://github.com/Hangsolow/ContentTokens
- **Optimizely Docs**: https://docs.developers.optimizely.com/
- **Blog Posts**: 
  - [Creating an Optimizely addon - Getting Started](https://world.optimizely.com/blogs/mark-stott/dates/2024/8/creating-an-optimizely-addon---getting-started/)
  - [Adding an Editor Interface Gadget](https://world.optimizely.com/blogs/mark-stott/dates/2024/8/creating-an-optimizely-cms-addon---adding-an-editor-interface-gadget/)
