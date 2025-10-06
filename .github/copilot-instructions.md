# Copilot Instructions for ContentTokens

This repository contains ContentTokens, an Optimizely CMS 12 addon that provides dynamic placeholders for reusable content tokens.

## Project Overview

ContentTokens allows editors to define reusable content tokens (e.g., `{{CompanyName}}`, `{{SupportEmail}}`) that can be used throughout an Optimizely CMS site. The addon automatically replaces these tokens at runtime with their defined values, supporting multilingual content.

## Technology Stack

- **.NET 8.0**: Primary framework
- **ASP.NET Core**: Web framework
- **Optimizely CMS 12.x**: CMS platform
- **Dojo Toolkit**: JavaScript framework for admin UI widgets
- **TinyMCE**: Rich text editor with custom plugin
- **Dynamic Data Store**: Token storage (Optimizely's built-in storage)

## Repository Structure

```
src/ContentTokens/           # Main addon library
  Models/                    # ContentToken models
  Services/                  # ContentTokenService for token management
  Controllers/               # REST API controllers
  ClientResources/           # JavaScript/CSS for admin UI
  Extensions/                # Middleware and service extensions
tests/
  ContentTokens.Example/     # Simple ASP.NET Core example
  ContentTokens.CmsExample/  # Full CMS integration example
docs/                        # Comprehensive documentation
```

## Coding Standards

### C# Code

- **Follow Microsoft's C# Coding Conventions**
- Use **meaningful variable and method names**
- Add **XML documentation comments** for all public APIs
- Keep methods **focused and small**
- Use **LINQ** where appropriate
- Enable **nullable reference types** (already enabled in project)
- Use **async/await** for I/O operations

Example:
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

### JavaScript/Dojo Code

- Use **semicolons**
- Use **double quotes** for strings
- Follow **Dojo module pattern** for AMD modules
- Add **JSDoc comments** for functions
- Use **camelCase** for variables and function names

Example:
```javascript
/**
 * Loads all tokens from the API
 */
loadTokens: function () {
    // Implementation
}
```

## Token Naming Conventions

Tokens use the format `{{TokenName}}` where:
- Token names are **alphanumeric only** (letters and numbers)
- Token names are **case-sensitive**
- **No spaces, hyphens, underscores, or special characters**

Good examples:
- `{{CompanyName}}`
- `{{SupportEmail}}`
- `{{CurrentYear}}`
- `{{WelcomeMessageEN}}`

Bad examples:
- `{{company name}}` (contains space)
- `{{Support-Email}}` (contains hyphen)
- `{{current_year}}` (contains underscore)

## Key Components

### ContentToken Model
Stores token data with properties: Name, Value, LanguageCode, Description

### ContentTokenService (IContentTokenService)
- `GetToken(name, languageCode)` - Get a specific token
- `GetAllTokens(languageCode)` - Get all tokens
- `SaveToken(token)` - Create or update a token
- `DeleteToken(id)` - Delete a token
- `ReplaceTokens(text, languageCode)` - Replace tokens in text

### ContentTokensController
REST API endpoints at `/api/contenttokens`:
- GET all tokens
- GET specific token
- POST create/update token
- DELETE token
- POST preview replacement

### Middleware
`ContentTokensMiddleware` intercepts responses and replaces tokens using regex pattern `\{\{(\w+)\}\}`

## Building and Testing

### Build the Solution
```bash
dotnet build
```

### Run Example Projects
```bash
# Simple example (no database)
cd tests/ContentTokens.Example
dotnet run

# Full CMS example (requires SQL Server/LocalDB)
cd tests/ContentTokens.CmsExample
dotnet run
```

### Testing
- Write unit tests for new features
- Ensure existing tests pass
- Use meaningful test names following pattern: `MethodName_Condition_ExpectedResult`

## Documentation

### Structure
All documentation is in the `docs/` folder:
- `getting-started.md` - Quick start guide
- `installation.md` - Installation instructions
- `configuration.md` - Advanced configuration
- `api-reference.md` - REST API documentation
- `advanced-usage.md` - Usage examples
- `architecture.md` - Architecture details
- `testing.md` - Testing guide
- `troubleshooting.md` - Common issues

### When to Update Documentation
- When adding new features or APIs
- When changing existing behavior
- When adding new configuration options
- When fixing bugs that affect usage

## Common Patterns

### Adding a New Token Programmatically
```csharp
var token = new ContentToken
{
    Name = "TokenName",
    Value = "Token Value",
    LanguageCode = "en",
    Description = "Description of the token"
};
_tokenService.SaveToken(token);
```

### Replacing Tokens in Text
```csharp
var text = "Welcome to {{CompanyName}}!";
var result = _tokenService.ReplaceTokens(text, "en");
// Result: "Welcome to Acme Corporation!"
```

### Multilingual Support
Define the same token name with different language codes for multi-language support. The service will automatically select the appropriate value based on the requested language, falling back to the language-neutral token if not found.

## Pull Request Guidelines

Before submitting:
- [ ] Code builds without errors or warnings
- [ ] All tests pass
- [ ] Code follows style guidelines
- [ ] XML documentation is added for public APIs
- [ ] Documentation is updated if needed
- [ ] Commit messages are clear and descriptive

## Important Notes

- **Token Regex**: The regex pattern `\{\{(\w+)\}\}` is compiled for performance
- **Middleware Order**: ContentTokens middleware must be placed after `UseRouting()` and `UseAuthentication()` but before `UseEndpoints()`
- **Storage**: Uses Optimizely's Dynamic Data Store by default
- **Multilingual**: Language fallback is automatic (specific language → language-neutral)
- **No Breaking Changes**: Maintain backward compatibility for public APIs

## Resources

- [Optimizely CMS Documentation](https://docs.developers.optimizely.com/)
- [Dojo Toolkit Documentation](https://dojotoolkit.org/)
- [TinyMCE Documentation](https://www.tiny.cloud/docs/)
- [Microsoft C# Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

## Questions?

1. Check existing documentation in `docs/`
2. Review `CONTRIBUTING.md`
3. Search closed issues on GitHub
4. Open a new issue with your question
