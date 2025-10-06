# TinyMCE Plugin for ContentTokens

This document provides comprehensive information about the ContentTokens TinyMCE plugin for Optimizely CMS 12.

## Overview

The ContentTokens TinyMCE plugin enables content editors to insert dynamic token placeholders (e.g., `{{CompanyName}}`) directly into Rich Text (XhtmlString) fields with autocomplete functionality and visual highlighting.

## Features

### 🎯 Core Functionality

- **Autocomplete**: Type `{{` in the editor to trigger a dropdown with available tokens
- **Toolbar Button**: Click the "Token" button to insert tokens via dialog
- **Menu Integration**: Access tokens through the Insert menu
- **Visual Highlighting**: Token placeholders are highlighted with custom CSS styling
- **API Integration**: Fetches available tokens from `/api/contenttokens` endpoint
- **Clean Output**: Removes highlighting before saving content

### 🎨 Visual Features

Token placeholders are styled for easy identification:
- Blue background with border (`#e8f4fd` background, `#4a90e2` border)
- Monospace font for clarity
- Hover effects for interactivity
- Customizable via CSS

## Installation

### 1. Files Included

The plugin consists of the following files:

```
src/ContentTokens/
├── ClientResources/
│   ├── Scripts/
│   │   ├── ContentTokensTinyMcePlugin.js      # Main plugin file
│   │   └── TinyMceInitializer.js              # Initialization helper
│   └── Styles/
│       └── ContentTokens.css                   # Token styling
├── EditorDescriptors/
│   └── ContentTokensTinyMceEditorDescriptor.cs # C# registration
└── module.config                               # Module configuration
```

### 2. Automatic Registration

The plugin is automatically registered when the ContentTokens package is installed. The `module.config` file includes:

```xml
<clientResources>
  <add name="contenttokens-tinymce" path="Scripts/ContentTokensTinyMcePlugin.js" resourceType="Script" />
  <add name="contenttokens-styles" path="Styles/ContentTokens.css" resourceType="Style" />
</clientResources>
```

### 3. Manual TinyMCE Configuration (Optional)

If you need more control, you can manually configure TinyMCE in your CMS initialization:

```csharp
services.Configure<TinyMceConfiguration>(config =>
{
    // Add the ContentTokens plugin
    config.Default()
        .AddPlugin("contenttokens")
        .AddExternalPlugin("contenttokens", "/ClientResources/Scripts/ContentTokensTinyMcePlugin.js")
        .Toolbar(toolbar => toolbar.Append("contenttokens"));
});
```

## Usage

### For Content Editors

#### Method 1: Autocomplete (Recommended)

1. Click inside a Rich Text field
2. Type `{{` (two opening curly braces)
3. A dropdown appears with available tokens
4. Use arrow keys to navigate
5. Press Enter or click to insert the token

![Autocomplete Example](tinymce-autocomplete.png)

#### Method 2: Toolbar Button

1. Click inside a Rich Text field
2. Click the **Token** button in the toolbar (bookmark icon)
3. Select a token from the dropdown
4. Click **Insert**

#### Method 3: Insert Menu

1. Click inside a Rich Text field
2. Go to **Insert** menu
3. Select **Content Token**
4. Choose a token and click **Insert**

### Token Appearance

Inserted tokens appear as highlighted placeholders:

```
Welcome to {{CompanyName}}! Contact us at {{SupportEmail}}.
```

Visual appearance in editor:
- <span style="background:#e8f4fd;border:1px solid #4a90e2;padding:2px 6px;font-family:monospace;font-weight:bold;color:#2d5f8d;">{{CompanyName}}</span>
- <span style="background:#e8f4fd;border:1px solid #4a90e2;padding:2px 6px;font-family:monospace;font-weight:bold;color:#2d5f8d;">{{SupportEmail}}</span>

## API Integration

### Token Endpoint

The plugin fetches tokens from:

```
GET /api/contenttokens
```

Expected response format:

```json
[
  {
    "Name": "CompanyName",
    "Value": "Acme Corporation",
    "Description": "Company display name",
    "LanguageCode": null
  },
  {
    "Name": "SupportEmail",
    "Value": "support@acme.com",
    "Description": "Support contact email",
    "LanguageCode": "en"
  }
]
```

### Caching

Tokens are fetched once when the plugin initializes and cached in memory. To refresh:
- Reload the page
- Or modify the plugin to add a refresh button

## Customization

### Styling Token Placeholders

Edit `/ClientResources/Styles/ContentTokens.css`:

```css
/* Blue theme (default) */
.opt-content-token {
    background-color: #e8f4fd;
    border: 1px solid #4a90e2;
    color: #2d5f8d;
}

/* Yellow theme - uncomment to use */
.opt-content-token {
    background-color: #fff3cd;
    border: 1px solid #ffc107;
    color: #856404;
}

/* Green theme - uncomment to use */
.opt-content-token {
    background-color: #d4edda;
    border: 1px solid #28a745;
    color: #155724;
}
```

### Changing Autocomplete Trigger

Modify the `ch` property in `ContentTokensTinyMcePlugin.js`:

```javascript
editor.ui.registry.addAutocompleter('contenttokens', {
    ch: '{{',        // Change to '@' or '$$' if desired
    minChars: 0,     // Minimum characters before showing suggestions
    // ...
});
```

### Adding Custom Metadata

Extend the token display in autocomplete:

```javascript
availableTokens = tokens.map(function (token) {
    return {
        type: 'cardmenuitem',
        value: token.Name,
        label: token.Name,
        meta: token.Description + ' | Lang: ' + (token.LanguageCode || 'all'),
        text: '{{' + token.Name + '}}'
    };
});
```

### Toolbar Button Icon

Change the button icon in `ContentTokensTinyMcePlugin.js`:

```javascript
editor.ui.registry.addButton('contenttokens', {
    text: 'Token',
    icon: 'bookmark',    // Options: bookmark, plus, code-sample, etc.
    tooltip: 'Insert Content Token',
    // ...
});
```

Available TinyMCE icons:
- `bookmark`
- `code-sample`
- `template`
- `plus`
- `insert-character`

## Technical Details

### Plugin Architecture

```
ContentTokensTinyMcePlugin.js
├── loadTokens()              # Fetches tokens from API
├── setupAutocompleter()      # Configures {{ trigger
├── setupToolbarButton()      # Adds toolbar button
├── setupMenuItem()           # Adds menu item
├── openTokenDialog()         # Shows token selection dialog
├── highlightTokens()         # Applies CSS to placeholders
├── removeHighlighting()      # Cleans output before save
├── setupContentProcessing()  # Hooks into content events
└── setupStyles()             # Injects CSS into editor
```

### Event Hooks

The plugin hooks into these TinyMCE events:

- **init**: Inject custom CSS styles
- **SetContent**: Highlight tokens when content loads
- **GetContent**: Remove highlighting before saving
- **BeforeSetContent**: Clean content before insertion

### Browser Compatibility

The plugin uses vanilla JavaScript (ES5) and is compatible with:
- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Internet Explorer 11 (with polyfills)

## Troubleshooting

### Tokens Not Appearing in Autocomplete

**Problem**: Typing `{{` doesn't show suggestions

**Solutions**:
1. Check API endpoint: Open browser console, look for `/api/contenttokens` request
2. Verify tokens exist: Visit `/api/contenttokens` directly in browser
3. Check for JavaScript errors in browser console
4. Ensure plugin is loaded: Look for "ContentTokens TinyMCE plugin initialized" in console

### Token Button Missing from Toolbar

**Problem**: Toolbar doesn't show the Token button

**Solutions**:
1. Clear browser cache and reload
2. Check `module.config` includes plugin reference
3. Verify TinyMCE configuration in CMS
4. Check browser console for loading errors

### Tokens Not Highlighted

**Problem**: Tokens display as plain text without styling

**Solutions**:
1. Verify CSS file is loaded: Check Network tab in browser dev tools
2. Check CSS class name matches: `.opt-content-token`
3. Clear editor cache: Ctrl+F5 to hard reload
4. Verify `setupStyles()` is being called

### API Endpoint Returns 404

**Problem**: Cannot load tokens from API

**Solutions**:
1. Ensure ContentTokensController is registered
2. Check route configuration: `/api/contenttokens`
3. Verify authentication/authorization settings
4. Check ApplicationUrl in appsettings.json

### Tokens Not Replaced in Output

**Problem**: Frontend shows `{{TokenName}}` instead of actual values

**Solutions**:
1. This plugin only provides editor functionality
2. Token replacement happens server-side via `IContentTokenService.ReplaceTokens()`
3. See main documentation for implementing token replacement in views/templates

## Advanced Scenarios

### Language-Specific Tokens

To filter tokens by current content language:

```javascript
function loadTokens(languageCode) {
    var url = '/api/contenttokens';
    if (languageCode) {
        url += '?languageCode=' + languageCode;
    }
    
    return fetch(url)
        .then(function (response) {
            return response.json();
        });
}
```

### Token Validation

Add validation before insertion:

```javascript
onAction: function (autocompleteApi, rng, value) {
    // Validate token exists
    if (!isValidToken(value)) {
        alert('Invalid token selected');
        return;
    }
    
    editor.selection.setRng(rng);
    editor.insertContent(value + '}}');
    autocompleteApi.hide();
}
```

### Custom Token Format

Support alternative formats like `[TokenName]` or `{TokenName}`:

```javascript
// Change in multiple places
ch: '[',                           // Trigger character
text: '[' + token.Name + ']'       // Token format
```

Then update regex patterns accordingly.

### Real-Time Token Preview

Show token values in tooltip:

```javascript
editor.on('mouseover', function (e) {
    var target = e.target;
    if (target.classList.contains('opt-content-token')) {
        var tokenName = target.textContent.replace(/{{|}}/g, '');
        var token = findTokenByName(tokenName);
        if (token) {
            showTooltip(target, token.Value);
        }
    }
});
```

## Code Examples

### Complete Implementation Example

```html
<!-- In a page template -->
@model StandardPage

<div class="content">
    @Html.PropertyFor(m => m.MainBody)
</div>
```

### Server-Side Token Replacement

```csharp
public class StandardPageController : PageController<StandardPage>
{
    private readonly IContentTokenService _tokenService;
    
    public StandardPageController(IContentTokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public ActionResult Index(StandardPage currentPage)
    {
        // Get HTML content with tokens
        var html = currentPage.MainBody?.ToHtmlString() ?? "";
        
        // Replace tokens
        var replaced = _tokenService.ReplaceTokens(html);
        
        // Create view model with replaced content
        var model = new StandardPageViewModel
        {
            Content = replaced
        };
        
        return View(model);
    }
}
```

## Performance Considerations

### Token Caching

The plugin caches tokens in memory. For better performance:

```javascript
var tokenCache = {
    tokens: [],
    expiry: null,
    duration: 5 * 60 * 1000  // 5 minutes
};

function loadTokens() {
    if (tokenCache.tokens.length > 0 && 
        tokenCache.expiry > Date.now()) {
        return Promise.resolve(tokenCache.tokens);
    }
    
    return fetch('/api/contenttokens')
        .then(function (response) {
            return response.json();
        })
        .then(function (tokens) {
            tokenCache.tokens = tokens;
            tokenCache.expiry = Date.now() + tokenCache.duration;
            return tokens;
        });
}
```

### Lazy Loading

Load plugin only when needed:

```javascript
editor.on('focus', function () {
    if (!tokensLoaded) {
        loadTokens();
        tokensLoaded = true;
    }
});
```

## Security Considerations

### XSS Prevention

The plugin sanitizes all input:

```javascript
// Tokens are inserted as text, not HTML
editor.insertContent(tinymce.html.Entities.encodeRaw(data.token));
```

### API Authentication

In production, secure the API endpoint:

```csharp
[Authorize(Roles = "CmsEditors,WebAdmins")]
[Route("api/contenttokens")]
public class ContentTokensController : ControllerBase
{
    // ...
}
```

### Content Validation

Validate token format server-side:

```csharp
var tokenPattern = new Regex(@"^{{[\w]+}}$");
if (!tokenPattern.IsMatch(tokenText))
{
    throw new ArgumentException("Invalid token format");
}
```

## Version History

### v1.0.0
- Initial release
- Autocomplete functionality
- Toolbar button and menu item
- Token highlighting
- API integration

## Support and Contributing

For issues, suggestions, or contributions:
- GitHub: https://github.com/Hangsolow/ContentTokens
- Documentation: /docs/

## License

MIT License - see LICENSE file for details
