# ContentTokens

💬 Dynamic placeholders for Optimizely CMS — write {{TokenName}} in any text field and let editors define reusable, multilingual content tokens.

## Overview

ContentTokens is an addon for Optimizely CMS v12 that enables editors to define reusable content tokens that can be used throughout the site. Simply write `{{TokenName}}` in any text field, and the addon will automatically replace it with the defined token value at runtime.

## Features

- 🌍 **Multilingual Support**: Define different token values for different languages
- ✏️ **Easy Management**: Intuitive admin gadget for creating and editing tokens
- 🔄 **Automatic Replacement**: Tokens are replaced automatically via service layer
- 🎯 **Simple Syntax**: Use `{{TokenName}}` anywhere in your content
- 📝 **REST API**: Full API support for programmatic token management
- 🎨 **TinyMCE Plugin**: Rich text editor integration with autocomplete for easy token insertion
- 🔤 **Dojo Plugin**: Plain text field autocomplete for string properties
- 💡 **Visual Highlighting**: Token placeholders are highlighted in the editor for easy identification

## Installation

### NuGet Package

```bash
Install-Package ContentTokens
```

Or via .NET CLI:

```bash
dotnet add package ContentTokens
```

### Configuration

1. Add the ContentTokens middleware to your `Startup.cs` or `Program.cs`:

```csharp
using ContentTokens.Extensions;

// In your Configure/WebApplication setup
app.UseContentTokens();
```

**Important**: Add the middleware after `UseRouting()` and `UseAuthentication()`, but before `UseEndpoints()`:

```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Add ContentTokens middleware here
app.UseContentTokens();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});
```

2. The addon will automatically register itself with Optimizely CMS through the initialization module.

## Usage

### Managing Tokens via Admin UI

1. Log in to the Optimizely CMS admin interface
2. Navigate to the Dashboard
3. Look for the "Content Tokens" gadget
4. Click "Add Token" to create a new token
5. Fill in the token details:
   - **Token Name**: The name used in content (e.g., `CompanyName`)
   - **Value**: The content that will replace the token
   - **Language Code**: Optional language code (e.g., `en`, `sv`, `de`) - leave empty for all languages
   - **Description**: Optional description of the token's purpose

### Using Tokens in Content

#### Method 1: Rich Text Editor (TinyMCE) with Autocomplete

The ContentTokens addon includes a TinyMCE plugin that makes inserting tokens easy:

1. Click inside a Rich Text (XhtmlString) field
2. Type `{{` (two opening curly braces)
3. A dropdown appears with available tokens and their descriptions
4. Use arrow keys to navigate or click to select
5. Press Enter to insert the token

Alternatively, click the **Token** button in the toolbar or use the **Insert** menu.

See [TinyMCE Plugin Documentation](docs/tinymce-plugin.md) for detailed information.

#### Method 2: Plain Text Fields with Autocomplete (Dojo Plugin)

For plain string properties, the Dojo autocomplete plugin provides similar functionality:

1. Click in a text field with token support (decorated with `[UIHint("ContentTokenString")]`)
2. Type `{{` (two opening curly braces)
3. A dropdown appears with available tokens
4. Navigate with arrow keys or mouse
5. Press Enter or click to insert the token

See [Dojo Plugin Documentation](docs/dojo-plugin.md) for detailed information.

#### Method 3: Manual Entry

Simply write the token name in double curly braces anywhere in your content:

```html
<p>Welcome to {{CompanyName}}!</p>
<p>Contact us at {{SupportEmail}} or call {{PhoneNumber}}</p>
```

At runtime, these will be replaced with the actual values:

```html
<p>Welcome to Acme Corporation!</p>
<p>Contact us at support@acme.com or call +1-555-0123</p>
```

### Multilingual Tokens

You can define language-specific values for tokens. The addon will automatically use the appropriate value based on the current content language:

- Token: `WelcomeMessage`
  - English (`en`): "Welcome to our site!"
  - Swedish (`sv`): "Välkommen till vår webbplats!"
  - German (`de`): "Willkommen auf unserer Website!"

If a language-specific token is not found, the addon falls back to the language-neutral token (one without a language code).

### Programmatic Access

You can also access tokens programmatically using the `IContentTokenService`:

```csharp
using ContentTokens.Services;

public class MyController : Controller
{
    private readonly IContentTokenService _tokenService;

    public MyController(IContentTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public IActionResult Index()
    {
        // Get a specific token
        var token = _tokenService.GetToken("CompanyName", "en");
        
        // Replace tokens in text
        var text = "Welcome to {{CompanyName}}!";
        var result = _tokenService.ReplaceTokens(text, "en");
        
        // Get all tokens
        var allTokens = _tokenService.GetAllTokens();
        
        return View();
    }
}
```

### REST API

The addon provides a REST API for managing tokens:

#### Get All Tokens
```http
GET /api/contenttokens?languageCode=en
```

#### Get a Specific Token
```http
GET /api/contenttokens/{name}?languageCode=en
```

#### Create or Update Token
```http
POST /api/contenttokens
Content-Type: application/json

{
  "name": "CompanyName",
  "value": "Acme Corporation",
  "languageCode": "en",
  "description": "The company name displayed across the site"
}
```

#### Delete Token
```http
DELETE /api/contenttokens/{id}
```

#### Preview Token Replacement
```http
POST /api/contenttokens/preview
Content-Type: application/json

{
  "text": "Welcome to {{CompanyName}}!",
  "languageCode": "en"
}
```

## Use Cases

- **Company Information**: Store company name, address, phone numbers, etc.
- **Legal Text**: Copyright notices, terms of service snippets
- **Social Media Links**: Twitter handles, Facebook URLs
- **Campaign Text**: Promotional messages that need to be updated frequently
- **Contact Information**: Support emails, sales contacts
- **Branding**: Slogans, taglines, brand-specific terminology

## Architecture

The addon consists of several key components:

- **ContentToken Model**: Stores token data in Optimizely's Dynamic Data Store
- **ContentTokenService**: Service for managing and replacing tokens
- **ContentTokensController**: REST API for token management
- **Admin Gadget**: Dojo-based UI widget for the Optimizely admin interface

## Example Projects

Two example projects are provided to demonstrate the addon:

### ContentTokens.Example (Simple)
**Location**: `tests/ContentTokens.Example/`

A lightweight ASP.NET Core application that demonstrates basic token replacement without requiring a database:
- ✅ Quick setup, no database required
- ✅ REST API for token management
- ✅ Live token replacement demonstration
- ✅ Ideal for understanding token functionality
- [View README](tests/ContentTokens.Example/README.md)

### ContentTokens.CmsExample (Full CMS)
**Location**: `tests/ContentTokens.CmsExample/`

A complete Optimizely CMS 12 implementation showing real-world integration:
- ✅ Full Optimizely CMS 12 setup
- ✅ Admin UI gadget for token management
- ✅ Sample content types (pages with token support)
- ✅ CMS content editing with tokens
- ❌ Requires SQL Server or LocalDB
- [View README](tests/ContentTokens.CmsExample/README.md)

**Which to use?**
- Use **ContentTokens.Example** for quick testing and API exploration
- Use **ContentTokens.CmsExample** for full CMS integration and real-world scenarios

## Requirements

- Optimizely CMS 12.x or higher
- .NET 8.0 or higher
- ASP.NET Core

## Documentation

Comprehensive documentation is available in the [docs](docs/) folder:

- **[Getting Started](docs/getting-started.md)** - Quick start guide
- **[Installation](docs/installation.md)** - Detailed installation instructions
- **[Configuration](docs/configuration.md)** - Advanced configuration options
- **[API Reference](docs/api-reference.md)** - Complete REST API documentation
- **[Advanced Usage](docs/advanced-usage.md)** - Usage examples and patterns
- **[Testing](docs/testing.md)** - Testing strategies and examples
- **[Architecture](docs/architecture.md)** - Architecture and design details
- **[Troubleshooting](docs/troubleshooting.md)** - Common issues and solutions

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/Hangsolow/ContentTokens).

## Acknowledgments

Created following the excellent blog posts by Mark Stott:
- [Creating an Optimizely addon - Getting Started](https://world.optimizely.com/blogs/mark-stott/dates/2024/8/creating-an-optimizely-addon---getting-started/)
- [Creating an Optimizely CMS addon - Adding an Editor Interface Gadget](https://world.optimizely.com/blogs/mark-stott/dates/2024/8/creating-an-optimizely-cms-addon---adding-an-editor-interface-gadget/)
