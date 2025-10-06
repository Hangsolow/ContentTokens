# ContentTokens.CmsExample

This is a full Optimizely CMS 12 example project demonstrating the ContentTokens addon in a real CMS environment.

## Prerequisites

Before running this example, ensure you have:

- **.NET 8.0 SDK** or later
- **SQL Server** or **SQL Server LocalDB**
  - SQL Server 2016 or later
  - LocalDB v13.0 or later (comes with Visual Studio)
- **Visual Studio 2022** or **VS Code** (optional but recommended)

## Database Setup

The example is configured to use LocalDB by default. The connection string is in `appsettings.json`:

```json
"ConnectionStrings": {
  "EPiServerDB": "Server=(localdb)\\mssqllocaldb;Database=ContentTokens.CmsExample;Integrated Security=true;MultipleActiveResultSets=true"
}
```

### Using SQL Server Instead of LocalDB

If you prefer to use a full SQL Server instance, update the connection string:

```json
"ConnectionStrings": {
  "EPiServerDB": "Server=YOUR_SERVER;Database=ContentTokens.CmsExample;Integrated Security=true;MultipleActiveResultSets=true"
}
```

Or with SQL authentication:

```json
"ConnectionStrings": {
  "EPiServerDB": "Server=YOUR_SERVER;Database=ContentTokens.CmsExample;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true"
}
```

## Running the Example

### First Time Setup

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Database initialization:**
   - On first run, Optimizely CMS will automatically create the database
   - This may take a few minutes
   - Watch the console for initialization progress

4. **Access the CMS:**
   - Open your browser to: `https://localhost:5000` (or the port shown in console)
   - You'll be redirected to the admin interface
   - Create an admin user when prompted

### Using ContentTokens in the CMS

#### 1. Create Tokens

1. Log in to the CMS
2. Go to the Dashboard
3. Find the **Content Tokens** gadget
4. Click **Add Token** and create a few tokens:
   - Name: `CompanyName`, Value: `Acme Corporation`
   - Name: `SupportEmail`, Value: `support@acme.com`
   - Name: `PhoneNumber`, Value: `+1-555-0123`

#### 2. Use Tokens in Rich Text Fields

The CmsExample includes a TinyMCE plugin for easy token insertion:

**Method 1: Autocomplete (Recommended)**
1. Edit a page (StartPage or StandardPage)
2. Click in a Rich Text field (like MainBody)
3. Type `{{` (two opening curly braces)
4. A dropdown appears with available tokens
5. Select a token and press Enter

**Method 2: Toolbar Button**
1. Click in a Rich Text field
2. Click the **Token** button in the toolbar (bookmark icon)
3. Select a token from the dialog
4. Click **Insert**

**Method 3: Manual Entry**
- Simply type `{{TokenName}}` anywhere in the content

#### 3. View Token Replacement

Tokens are visually highlighted in the editor with a blue background. When you publish and view the page on the frontend, the tokens will be replaced with their actual values.

See the [TinyMCE Plugin Documentation](../../docs/tinymce-plugin.md) for advanced features and customization.

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Database initialization:**
   - On first run, Optimizely CMS will automatically create the database
   - This may take a few minutes
   - Watch the console for initialization progress

4. **Access the site:**
   - Navigate to `http://localhost:5000` (or the URL shown in console)
   - You'll see the CMS setup wizard on first run

5. **Complete CMS setup:**
   - Create an admin user account
   - The site will initialize with a sample site structure

### Logging In

After setup:
- Navigate to `http://localhost:5000/episerver/cms`
- Log in with the admin credentials you created
- You'll see the ContentTokens gadget in the CMS dashboard

## Using ContentTokens in CMS

### Via Admin Gadget

1. Log into the CMS admin interface
2. Go to the Dashboard
3. Find the **Content Tokens** gadget
4. Click **Add Token** to create tokens
5. Example tokens to create:
   - `CompanyName` → "Acme Corporation"
   - `SupportEmail` → "support@acme.com"
   - `PhoneNumber` → "+1-555-0123"

### Via REST API

While the CMS is running, use the API:

```bash
# Create a token
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CompanyName",
    "value": "Acme Corporation",
    "description": "Company name"
  }'

# List all tokens
curl http://localhost:5000/api/contenttokens

# Preview token replacement
curl -X POST http://localhost:5000/api/contenttokens/preview \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Welcome to {{CompanyName}}!",
    "languageCode": "en"
  }'
```

### In Content

1. Create or edit a page in the CMS
2. In any text field (title, body, etc.), use tokens:
   ```
   Welcome to {{CompanyName}}!
   Contact us at {{SupportEmail}} or call {{PhoneNumber}}.
   ```

3. When the page is viewed on the site, tokens are automatically replaced:
   ```
   Welcome to Acme Corporation!
   Contact us at support@acme.com or call +1-555-0123.
   ```

## Sample Content Types

This example includes two page types:

### Start Page
- **Heading** - Supports tokens
- **Main Body** - Rich text editor with token support
- **Main Content Area** - For blocks and widgets

### Standard Page
- **Title** - Supports tokens
- **Main Body** - Rich text editor with token support
- **Main Content Area** - For blocks and widgets

## Project Structure

```
ContentTokens.CmsExample/
├── Models/
│   ├── Pages/
│   │   ├── StartPage.cs        # Home page type
│   │   └── StandardPage.cs     # Standard content page type
│   └── Blocks/                 # (Add blocks here)
├── Views/
│   ├── Shared/                 # Shared views
│   └── _ViewImports.cshtml     # View imports
├── Business/
│   └── Initialization/         # Custom initialization
├── wwwroot/                    # Static files
├── Program.cs                  # Application entry point
└── appsettings.json            # Configuration

```

## Features Demonstrated

### 1. Token Replacement in Pages
Edit page content and use `{{TokenName}}` syntax. Tokens are replaced automatically when pages are rendered.

### 2. Multilingual Tokens
Create language-specific tokens:
```bash
# English
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{"name":"WelcomeMessage","value":"Welcome!","languageCode":"en"}'

# Swedish
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{"name":"WelcomeMessage","value":"Välkommen!","languageCode":"sv"}'
```

### 3. Admin Gadget
Manage all tokens from the CMS dashboard without coding.

### 4. REST API Integration
Full programmatic access to token management.

## Troubleshooting

### Database Connection Issues

**Error: "Cannot open database"**
- Ensure SQL Server/LocalDB is installed and running
- Check connection string in `appsettings.json`
- For LocalDB, run: `sqllocaldb start mssqllocaldb`

**Error: "Login failed"**
- Verify SQL Server authentication settings
- Check user permissions on the database
- Try using integrated security (Windows Authentication)

### CMS Initialization Issues

**Error: "Module initialization failed"**
- Delete the database and let CMS recreate it
- Check that all NuGet packages restored correctly
- Ensure .NET 8.0 SDK is installed

### Token Not Replacing

**Tokens appear as `{{TokenName}}` instead of values**
1. Verify token exists: `curl http://localhost:5000/api/contenttokens`
2. Check middleware is registered in `Program.cs`
3. Ensure `UseContentTokens()` is after `UseAuthorization()`
4. Clear browser cache and refresh

### Admin Gadget Not Visible

**ContentTokens gadget doesn't appear in dashboard**
1. Ensure you're logged in as admin
2. Check that ContentTokens addon DLL is in bin folder
3. Restart the application
4. Clear browser cache

## Development Tips

### Adding New Page Types

1. Create a new class in `Models/Pages/`
2. Inherit from `PageData`
3. Add `[ContentType]` attribute
4. Add properties with `[Display]` attributes
5. Restart the app - CMS will detect the new type

### Adding Blocks

1. Create classes in `Models/Blocks/`
2. Inherit from `BlockData`
3. Use `[ContentType]` and `[Display]` attributes
4. Create corresponding views in `Views/Shared/Blocks/`

### Customizing Token Behavior

You can extend the token service in `Business/Initialization/`:

```csharp
using ContentTokens.Services;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace ContentTokens.CmsExample.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomTokenInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // Register custom token service if needed
            // context.Services.AddSingleton<IContentTokenService, CustomTokenService>();
        }

        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }
    }
}
```

## Differences from Simple Example

The simple `ContentTokens.Example` project:
- ✅ Minimal setup, no database required
- ✅ Quick demonstration of token replacement
- ❌ No CMS admin interface
- ❌ No content editing
- ❌ API only for token management

This CMS example:
- ✅ Full Optimizely CMS experience
- ✅ Admin interface with gadget
- ✅ Content editing with token support
- ✅ Real-world CMS integration
- ❌ Requires SQL Server database
- ❌ More complex setup

## Related Documentation

- [Main ContentTokens README](../../README.md)
- [Getting Started Guide](../../docs/getting-started.md)
- [Configuration Guide](../../docs/configuration.md)
- [API Reference](../../docs/api-reference.md)
- [Optimizely CMS Documentation](https://docs.developers.optimizely.com/)

## Support

For issues specific to:
- **ContentTokens addon**: [GitHub Issues](https://github.com/Hangsolow/ContentTokens/issues)
- **Optimizely CMS**: [Optimizely Developer Community](https://world.optimizely.com/)

## Next Steps

1. Explore the CMS admin interface
2. Create sample pages and add tokens
3. Try multilingual tokens with different language versions
4. Customize page types and blocks
5. Integrate with your own Optimizely CMS projects
