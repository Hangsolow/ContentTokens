# Getting Started with ContentTokens

This guide will help you get started with ContentTokens, a dynamic placeholder addon for Optimizely CMS v12.

## What is ContentTokens?

ContentTokens allows editors to define reusable, multilingual content tokens that can be used throughout your Optimizely CMS site. Simply write `{{TokenName}}` in any text field, and the addon will automatically replace it with the configured value.

## Prerequisites

Before you begin, ensure you have:

- .NET 8.0 SDK or later
- Optimizely CMS 12.x or later
- Visual Studio 2022, VS Code, or Rider (recommended)
- Basic knowledge of ASP.NET Core and Optimizely CMS

## Quick Start

### 1. Installation

Install the ContentTokens NuGet package:

```bash
dotnet add package ContentTokens
```

Or via Package Manager Console:

```powershell
Install-Package ContentTokens
```

### 2. Configuration

Add the ContentTokens middleware and Blazor Server support to your `Program.cs`:

```csharp
using ContentTokens.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor Server support for the admin interface
builder.Services.AddServerSideBlazor();

// ... other service registrations

var app = builder.Build();

// Add after UseRouting() and UseAuthorization()
app.UseRouting();
app.UseAuthorization();

// Add ContentTokens middleware
app.UseContentTokens();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub(); // Required for Blazor admin interface
});

app.Run();
```

### 3. Create Your First Token

After installation, the ContentTokens admin interface will be available in your Optimizely CMS admin menu.

**Via Blazor Admin UI:**
1. Log in to Optimizely CMS
2. Navigate to **Admin** menu
3. Click **Content Tokens**
4. Click "Add Token"
5. Enter:
   - **Name**: `CompanyName` (alphanumeric only)
   - **Value**: `Acme Corporation`
   - **Description**: `Company name used throughout the site`
6. Click "Save"

The Blazor interface provides real-time validation and immediate feedback on your actions.

**Via REST API:**
```bash
curl -X POST https://your-site.com/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CompanyName",
    "value": "Acme Corporation",
    "description": "Company name used throughout the site"
  }'
```

### 4. Use Tokens in Your Content

In any Optimizely CMS content block, page, or property, use the token:

```html
<h1>Welcome to {{CompanyName}}!</h1>
<p>Contact us at {{SupportEmail}} for assistance.</p>
```

When the page is rendered, tokens are automatically replaced:

```html
<h1>Welcome to Acme Corporation!</h1>
<p>Contact us at support@acme.com for assistance.</p>
```

## Key Concepts

### Token Syntax

Tokens use double curly braces: `{{TokenName}}`

- Token names are alphanumeric (letters and numbers only)
- Token names are case-sensitive
- Spaces are not allowed in token names

**Valid tokens:**
- `{{CompanyName}}`
- `{{SupportEmail}}`
- `{{PhoneNumber2024}}`

**Invalid tokens:**
- `{{Company Name}}` (contains space)
- `{{Support-Email}}` (contains hyphen)
- `{{Phone#}}` (contains special character)

### Multilingual Support

ContentTokens supports language-specific values:

1. Create a token without a language code for the default value
2. Create additional tokens with the same name but different language codes

Example:
```json
{
  "name": "WelcomeMessage",
  "value": "Welcome to our site!",
  "languageCode": "en"
}

{
  "name": "WelcomeMessage",
  "value": "Välkommen till vår webbplats!",
  "languageCode": "sv"
}

{
  "name": "WelcomeMessage",
  "value": "Willkommen auf unserer Website!",
  "languageCode": "de"
}
```

The addon automatically selects the correct value based on the current content language.

### Fallback Mechanism

If a token is not found:
1. First, the addon looks for a language-specific token
2. If not found, it falls back to a language-neutral token
3. If still not found, the original `{{TokenName}}` remains unchanged

## Common Use Cases

### Company Information

Store frequently used company details:
- `{{CompanyName}}`
- `{{CompanyAddress}}`
- `{{CompanyPhone}}`
- `{{CompanyEmail}}`

### Legal Text

Keep legal text consistent across pages:
- `{{CopyrightNotice}}`
- `{{PrivacyPolicy}}`
- `{{TermsOfService}}`

### Marketing Campaigns

Update campaign text site-wide:
- `{{CurrentPromotion}}`
- `{{SpecialOffer}}`
- `{{CallToAction}}`

### Contact Information

Centralize contact details:
- `{{SupportEmail}}`
- `{{SalesPhone}}`
- `{{CustomerServiceHours}}`

## Next Steps

- [Installation Guide](installation.md) - Detailed installation instructions
- [Configuration Guide](configuration.md) - Advanced configuration options
- [API Reference](api-reference.md) - Complete REST API documentation
- [Advanced Usage](advanced-usage.md) - More usage examples and patterns
- [Troubleshooting](troubleshooting.md) - Common issues and solutions

## Need Help?

- Check the [Troubleshooting Guide](troubleshooting.md)
- Visit the [GitHub Repository](https://github.com/Hangsolow/ContentTokens)
- Open an issue for bug reports or feature requests
