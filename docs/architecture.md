# Architecture

This document provides an in-depth look at the ContentTokens architecture, explaining how the addon works and the core concepts that power the system.

## Overview

ContentTokens is an Optimizely CMS addon that provides a dynamic token replacement system for content editors. It allows editors to define reusable content snippets (tokens) that can be used throughout the site by simply writing `{{TokenName}}` in any text field.

## Architecture

### Components

The addon consists of several key components:

```
src/ContentTokens/
├── Models/
│   └── ContentToken.cs          # Data model for tokens
├── Services/
│   └── ContentTokenService.cs   # Service for managing tokens
├── Controllers/
│   └── ContentTokensController.cs  # REST API endpoints
├── Extensions/
│   └── ContentTokensApplicationBuilderExtensions.cs  # Middleware extensions
├── BlazorCmsAddon/
│   ├── Components/
│   │   └── ContentTokensManager.razor  # Blazor admin component
│   ├── ContentTokensAddonController.cs  # MVC controller for Blazor UI
│   ├── ContentTokensMenuProvider.cs     # Admin menu registration
│   └── Views/
│       └── Index.cshtml          # Blazor host view
└── ClientResources/
    └── Scripts/
        ├── ContentTokensTinyMcePlugin.js  # TinyMCE integration
        └── ContentTokensAutocomplete.js   # Dojo autocomplete widget
```

### Data Storage

Tokens are stored in Optimizely's Dynamic Data Store (DDS), which provides:
- Automatic schema management
- LINQ query support
- Built-in versioning
- No database schema changes required

The `ContentToken` model includes:
- `Id`: Unique identifier
- `Name`: Token name (e.g., "CompanyName")
- `Value`: The actual content to replace
- `LanguageCode`: Optional language-specific value
- `Description`: Documentation for editors
- `Created`/`Modified`: Timestamps

### Token Replacement

Token replacement happens through:

1. **Service Layer Replacement**
   - Developers use `IContentTokenService.ReplaceTokens()` to replace tokens
   - Uses regex pattern `\{\{(\w+)\}\}` to find tokens
   - Replaces tokens with their configured values
   - Respects current content language
   - Useful for any content type (HTML, emails, PDFs, etc.)

### Multilingual Support

The addon supports multiple languages through:

1. **Language-Specific Tokens**
   - Set `LanguageCode` property (e.g., "en", "sv", "de")
   - Used when content is in that specific language

2. **Fallback Mechanism**
   - If no language-specific token exists, falls back to language-neutral token
   - Language-neutral tokens have null/empty `LanguageCode`

3. **Priority Order**
   - Language-specific token (if exists)
   - Language-neutral token (fallback)
   - Original placeholder (if no token found)

### Admin Interface

The admin interface is built with Blazor Server and provides:
- Modern, responsive UI following Optimizely Axiom design guidelines
- List view of all tokens in a clean table format
- Full CRUD functionality (Create, Read, Update, Delete)
- Real-time validation (alphanumeric token names only)
- Success/error messaging
- Language filtering support
- Integrated directly into the CMS admin menu at `/contenttokens`

The Blazor implementation offers better performance and maintainability compared to legacy JavaScript frameworks.

### REST API

The API provides full CRUD operations:

```
GET    /api/contenttokens              # List all tokens
GET    /api/contenttokens/{name}       # Get specific token
POST   /api/contenttokens              # Create/Update token
DELETE /api/contenttokens/{id}         # Delete token
POST   /api/contenttokens/preview      # Preview token replacement
```

## Design Decisions

### Why Dynamic Data Store?

Pros:
- No database migrations needed
- Automatic schema management
- Built into Optimizely CMS
- Simple LINQ queries
- Version control built-in

Cons:
- Not as performant as direct SQL
- Limited query capabilities
- Mitigated by caching in production

### Why HTTP Middleware?

Pros:
- Automatic replacement without code changes
- Works with any content source
- Consistent across the site
- Easy to enable/disable

Cons:
- Processes all HTML responses
- Slight performance overhead
- Mitigated by regex optimization

### Why Regex Pattern?

The pattern `\{\{(\w+)\}\}` provides:
- Clear, distinctive syntax
- Low collision risk with content
- Easy to type for editors
- Familiar to developers (Mustache-like)
- Performant with compiled regex

Alternative patterns considered:
- `{TokenName}` - Too common, conflicts with CSS
- `[TokenName]` - Conflicts with links
- `<TokenName>` - Conflicts with HTML
- `##TokenName##` - Less intuitive

## Performance Considerations

### Optimization Strategies

1. **Compiled Regex**
   - Pattern is compiled once at startup
   - Significant performance improvement

2. **In-Memory Caching** (Future Enhancement)
   - Cache tokens in memory
   - Invalidate on changes
   - Reduce DDS queries

3. **Selective Processing**
   - Only processes HTML responses
   - Skips API responses, images, etc.
   - Checks content type first

4. **Efficient String Replacement**
   - Single pass through content
   - Uses `Regex.Replace` with callback

### Scalability

For high-traffic sites, consider:
- Implementing distributed caching (Redis)
- Pre-processing content at publish time
- Using output caching
- Monitoring middleware performance

## Security Considerations

### Authorization

- API requires authentication in production
- Uses Optimizely CMS roles (CmsAdmins, CmsEditors)
- `[AllowAnonymous]` only for examples

### Input Validation

- Token names: Alphanumeric only (enforced by regex)
- No HTML in token values (editor responsibility)
- XSS protection through standard ASP.NET Core

### Best Practices

1. Don't store sensitive data in tokens
2. Use appropriate authorization
3. Sanitize token values if needed
4. Monitor token creation/deletion

## Extensibility Points

### Custom Token Providers

Implement `IContentTokenService` to add:
- Database-backed tokens
- API-sourced tokens
- Cached tokens
- Composite providers

### Custom Middleware

### Custom Token Processing

Extend `IContentTokenService` for:
- Custom processing logic
- Additional content types
- Performance optimizations
- Logging/monitoring

### Custom Admin UI

The Blazor admin interface can be extended or customized for:
- Bulk operations
- Import/export functionality
- Advanced token validation
- Usage analytics
- Custom styling to match your brand
- Additional fields or metadata

## Integration Guide

### Basic Setup

1. Install NuGet package
2. Add middleware to pipeline
3. Access admin gadget in CMS

### Advanced Scenarios

**Email Templates**:
```csharp
var emailTemplate = "Thank you, {{CustomerName}}!";
var processed = tokenService.ReplaceTokens(emailTemplate);
```

**PDF Generation**:
```csharp
var pdfContent = GetPdfTemplate();
var processed = tokenService.ReplaceTokens(pdfContent);
GeneratePdf(processed);
```

**Custom Processing**:
```csharp
// Create custom token processing logic
public class CustomTokenService : IContentTokenService
{
    public string ReplaceTokens(string content, string languageCode = null)
    {
        // Custom processing logic
        content = PreProcess(content);
        // ... token replacement
        return PostProcess(content);
    }
}
```

## Testing Strategy

### Unit Tests

Test each component in isolation:
- Token service operations
- Regex pattern matching
- Language fallback logic
- API endpoints

### Integration Tests

Test end-to-end scenarios:
- Token replacement in HTML
- API CRUD operations
- Middleware integration
- Language switching

### Manual Testing

Verify in real Optimizely CMS:
- Admin gadget functionality
- Token replacement in pages
- Multilingual content
- Performance under load

## Future Enhancements

Potential improvements:
1. **Caching**: Add distributed caching support
2. **Bulk Operations**: Import/export tokens
3. **Version History**: Track token changes
4. **Usage Analytics**: See where tokens are used
5. **Token Validation**: Ensure referenced tokens exist
6. **Token Categories**: Organize tokens by type
7. **Token Permissions**: Fine-grained access control
8. **Token Preview**: Live preview in editor
9. **Token Suggestions**: Auto-complete in editor
10. **Audit Trail**: Log all token changes

## Troubleshooting

### Tokens Not Replaced

1. Check middleware is registered: `app.UseContentTokens()`
2. Verify token exists: `GET /api/contenttokens`
3. Check content type is HTML
4. Verify token name matches exactly (case-sensitive)

### Performance Issues

1. Enable output caching
2. Reduce number of tokens
3. Optimize regex pattern
4. Consider pre-processing

### Admin Interface Not Visible

1. Check Blazor Server is configured: `builder.Services.AddServerSideBlazor()` and `endpoints.MapBlazorHub()`
2. Verify middleware is registered: `app.UseContentTokens()`
3. Check the menu provider is loaded
4. Clear browser cache
5. Verify user has CMS admin permissions

## Support & Contributing

For issues and feature requests, visit:
https://github.com/Hangsolow/ContentTokens

Contributions welcome via pull requests!
