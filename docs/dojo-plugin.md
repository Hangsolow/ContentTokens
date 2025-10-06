# ContentTokens Dojo Autocomplete Plugin

## Overview

The ContentTokens Dojo Autocomplete plugin provides intelligent token insertion for plain string fields in Optimizely CMS 12. When editors type `{{` in a text field, an autocomplete dropdown appears showing available Content Tokens, making it easy to insert tokens without memorizing their names.

## Features

- ✅ **Autocomplete Dropdown**: Triggered by typing `{{` in any text field
- ✅ **API Integration**: Fetches tokens from `/api/contenttokens` endpoint
- ✅ **Keyboard Navigation**: Arrow keys, Enter, and Escape support
- ✅ **Multi-line Support**: Works with both TextBox and Textarea fields
- ✅ **Cursor Positioning**: Inserts tokens at the current cursor position
- ✅ **Multiple Tokens**: Supports multiple tokens in a single field
- ✅ **Visual Feedback**: Hover and selection highlighting
- ✅ **Dojo Integration**: Built using Optimizely's standard Dojo architecture

## Architecture

### Components

1. **ContentTokensAutocomplete.js** - Main Dojo widget (13KB)
   - Extends `dijit/_Widget`, `dijit/_TemplatedMixin`, `epi/shell/widget/_ValueRequiredMixin`
   - Handles autocomplete logic and UI
   - Manages keyboard navigation and token insertion

2. **ContentTokensAutocomplete.css** - Widget styling (3KB)
   - Dropdown appearance
   - Item hover/selection states
   - Responsive design

3. **ContentTokenStringEditorDescriptor.cs** - C# registration
   - Applies widget to string properties
   - Provides two UI hints: `ContentTokenString` and `ContentTokenLongString`

4. **module.config** - Module registration
   - Registers Dojo paths
   - Loads client resources

## Installation

### 1. Verify Files

Ensure these files exist in your project:

```
src/ContentTokens/
├── ClientResources/
│   ├── Scripts/
│   │   └── ContentTokensAutocomplete.js
│   └── Styles/
│       └── ContentTokensAutocomplete.css
├── EditorDescriptors/
│   └── ContentTokenStringEditorDescriptor.cs
└── module.config
```

### 2. Register the Module

The widget is automatically registered via `module.config`:

```xml
<clientResources>
  <add name="contenttokens-autocomplete" path="Scripts/ContentTokensAutocomplete.js" resourceType="Script" />
  <add name="contenttokens-autocomplete-styles" path="Styles/ContentTokensAutocomplete.css" resourceType="Style" />
</clientResources>

<dojo>
  <paths>
    <add name="contentTokens" path="Scripts" />
  </paths>
</dojo>
```

### 3. Build and Deploy

```bash
dotnet build
dotnet run
```

The widget will be available in edit mode after the CMS starts.

## Usage

### For Content Editors

1. **Open a page** in edit mode
2. **Click on a text field** that has token support
3. **Type `{{`** - an autocomplete dropdown appears
4. **Navigate** using:
   - Arrow keys (↑/↓) to move selection
   - Enter to insert selected token
   - Escape to close dropdown
   - Mouse click to select a token
5. **Continue typing** - the token `{{TokenName}}` is inserted at cursor position

### For Developers

#### Apply to Specific Properties

Use the `[UIHint]` attribute to enable token autocomplete on specific properties:

```csharp
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

[ContentType(DisplayName = "Standard Page", GUID = "...")]
public class StandardPage : PageData
{
    // Single-line text field with token autocomplete
    [UIHint("ContentTokenString")]
    [Display(Name = "Tagline", Order = 10)]
    public virtual string Tagline { get; set; }

    // Multi-line text field with token autocomplete
    [UIHint("ContentTokenLongString")]
    [Display(Name = "Description", Order = 20)]
    public virtual string Description { get; set; }

    // Regular string field (no autocomplete)
    [Display(Name = "Author", Order = 30)]
    public virtual string Author { get; set; }
}
```

#### Apply to All String Properties (Advanced)

To enable autocomplete on all string properties, modify the editor descriptor:

```csharp
[EditorDescriptorRegistration(
    TargetType = typeof(string),
    EditorDescriptorBehavior = EditorDescriptorBehavior.PlaceLast)]
public class ContentTokenStringEditorDescriptor : EditorDescriptor
{
    public ContentTokenStringEditorDescriptor()
    {
        ClientEditingClass = "contentTokens/ContentTokensAutocomplete";
    }
}
```

> **Note**: This applies to ALL string properties. Use with caution.

## API Integration

### Token Endpoint

The widget fetches tokens from `/api/contenttokens`:

**Request:**
```http
GET /api/contenttokens HTTP/1.1
Accept: application/json
```

**Response:**
```json
[
  {
    "name": "CompanyName",
    "description": "Company display name"
  },
  {
    "name": "SupportEmail",
    "description": "Primary support email address"
  }
]
```

### Custom Endpoint (Optional)

To use a different API endpoint, modify the widget or pass configuration:

```csharp
public override void ModifyMetadata(
    ExtendedMetadata metadata,
    IEnumerable<Attribute> attributes)
{
    base.ModifyMetadata(metadata, attributes);
    
    // Custom token endpoint
    metadata.EditorConfiguration["tokenApiEndpoint"] = "/api/custom/tokens";
}
```

## Customization

### CSS Styling

#### Change Dropdown Colors

Edit `ContentTokensAutocomplete.css`:

```css
.epiContentTokensDropdownItem:hover,
.epiContentTokensDropdownItemSelected {
    background-color: #e3f2fd; /* Light blue */
}
```

#### Apply Theme

Add a theme class to enable different color schemes:

```css
/* Add to your custom CSS */
.epiContentTokensWrapper.theme-dark .epiContentTokensDropdown {
    background-color: #2c2c2c;
    border-color: #444444;
}

.epiContentTokensWrapper.theme-dark .epiContentTokensDropdownItem {
    color: #ffffff;
}
```

### Widget Configuration

Extend the widget to add custom behavior:

```javascript
define([
    "contentTokens/ContentTokensAutocomplete",
    "dojo/_base/declare"
], function (ContentTokensAutocomplete, declare) {
    return declare([ContentTokensAutocomplete], {
        
        // Override token loading to add caching
        _loadTokens: function () {
            var cached = sessionStorage.getItem('contentTokens');
            if (cached) {
                this._tokens = JSON.parse(cached);
                return;
            }
            
            this.inherited(arguments);
        }
    });
});
```

## Examples

### Basic Usage

```csharp
[ContentType(DisplayName = "Article Page")]
public class ArticlePage : PageData
{
    [UIHint("ContentTokenString")]
    public virtual string Title { get; set; }
    
    [UIHint("ContentTokenLongString")]
    public virtual string Summary { get; set; }
}
```

### With Multiple Tokens

An editor can use multiple tokens in a single field:

```
Welcome to {{CompanyName}}! Contact us at {{SupportEmail}} or call {{PhoneNumber}}.
```

### With Regular Text

Tokens can be mixed with regular text:

```
Title: {{CompanyName}} - {{Department}}
```

## Troubleshooting

### Dropdown Not Appearing

**Symptom**: Typing `{{` doesn't show the dropdown.

**Solutions**:
1. Check browser console for JavaScript errors
2. Verify `module.config` registers the script correctly
3. Ensure `/api/contenttokens` endpoint is accessible
4. Clear browser cache and reload

### Tokens Not Loading

**Symptom**: Dropdown appears but is empty.

**Solutions**:
1. Test API endpoint directly: `GET /api/contenttokens`
2. Check network tab in browser dev tools
3. Verify authentication/authorization
4. Check server logs for errors

### Widget Not Applied to Properties

**Symptom**: Regular text box appears instead of token-enabled field.

**Solutions**:
1. Verify `[UIHint("ContentTokenString")]` attribute is present
2. Rebuild the project
3. Restart the CMS application
4. Clear browser cache

### Keyboard Navigation Not Working

**Symptom**: Arrow keys don't navigate the dropdown.

**Solutions**:
1. Ensure focus is on the input field
2. Check for JavaScript errors in console
3. Verify dropdown is actually open (`_autocompleteActive === true`)

## Performance Considerations

### Token Caching

The widget loads tokens once on initialization. For better performance with many tokens:

```javascript
// Add caching to localStorage
_loadTokens: function () {
    var cached = localStorage.getItem('contentTokens');
    var cacheTime = localStorage.getItem('contentTokensCacheTime');
    var now = new Date().getTime();
    
    // Cache for 5 minutes
    if (cached && cacheTime && (now - parseInt(cacheTime)) < 300000) {
        this._tokens = JSON.parse(cached);
        return;
    }
    
    // Load from API and cache
    xhr("/api/contenttokens", {
        handleAs: "json"
    }).then(function (data) {
        this._tokens = data || [];
        localStorage.setItem('contentTokens', JSON.stringify(data));
        localStorage.setItem('contentTokensCacheTime', now.toString());
    });
}
```

### Lazy Loading

For sites with many tokens, implement search/filter:

```javascript
_showAutocomplete: function (cursorPos) {
    var searchTerm = this._getSearchTerm();
    var filteredTokens = this._tokens.filter(function (token) {
        return token.name.toLowerCase().includes(searchTerm.toLowerCase());
    });
    
    // Display only filtered tokens
    this._renderDropdown(filteredTokens);
}
```

## Browser Support

- ✅ Chrome/Edge (latest)
- ✅ Firefox (latest)
- ✅ Safari (latest)
- ⚠️ Internet Explorer 11 (limited support, requires polyfills)

## Security

### XSS Protection

The widget uses `domConstruct.create()` with proper escaping:

```javascript
innerHTML: '<strong>' + token.name + '</strong>' +
          (token.description ? '<br><span>' + token.description + '</span>' : '')
```

For user-generated content, ensure proper sanitization on the server side.

### API Authentication

The widget respects Optimizely's authentication. Ensure `/api/contenttokens` requires authentication:

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContentTokensController : ControllerBase
{
    // ...
}
```

## Advanced Topics

### Custom Widget Extension

Create a custom widget that extends the base functionality:

```javascript
define([
    "contentTokens/ContentTokensAutocomplete",
    "dojo/_base/declare",
    "dojo/_base/lang"
], function (ContentTokensAutocomplete, declare, lang) {
    return declare([ContentTokensAutocomplete], {
        
        // Add token preview on hover
        _showTokenPreview: function (token) {
            // Custom preview logic
        },
        
        // Add token validation
        _validateToken: function (token) {
            return token.name && token.name.length > 0;
        }
    });
});
```

### Integration with Other Widgets

Combine with other Optimizely widgets:

```csharp
[EditorDescriptorRegistration(
    TargetType = typeof(string),
    UIHint = "ContentTokenStringWithCounter")]
public class ContentTokenStringWithCounterEditorDescriptor : EditorDescriptor
{
    public ContentTokenStringWithCounterEditorDescriptor()
    {
        ClientEditingClass = "contentTokens/ContentTokensAutocompleteWithCounter";
    }
}
```

## See Also

- [TinyMCE Plugin Guide](tinymce-plugin.md) - Token autocomplete for Rich Text fields
- [API Reference](api-reference.md) - REST API documentation
- [Architecture](architecture.md) - System architecture overview
- [Configuration](configuration.md) - Advanced configuration options

## Support

For issues or questions:
- Check the [Troubleshooting Guide](troubleshooting.md)
- Review browser console for errors
- Verify API endpoint accessibility
- Check Optimizely CMS logs

## License

MIT License - See LICENSE file for details
