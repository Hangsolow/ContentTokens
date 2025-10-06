# ContentTokens for Optimizely CMS

Dynamic placeholders for Optimizely CMS — write `{{TokenName}}` in any text field and let editors define reusable, multilingual content tokens.

## Quick Start

### 1. Install

```bash
dotnet add package ContentTokens
```

### 2. Configure

Add to your `Startup.cs` or `Program.cs`:

```csharp
using ContentTokens.Extensions;

// After UseRouting() and UseAuthorization()
app.UseContentTokens();
```

### 3. Use

Write tokens in your content:
```html
<p>Welcome to {{CompanyName}}!</p>
<p>Contact us at {{SupportEmail}}</p>
```

Manage tokens in the Optimizely CMS admin dashboard gadget or via the REST API.

## Features

✅ Multilingual token support  
✅ Automatic HTML replacement  
✅ Admin UI gadget  
✅ REST API  
✅ Language fallback  

## Documentation

Full documentation: https://github.com/Hangsolow/ContentTokens

## License

MIT License - see LICENSE file for details
