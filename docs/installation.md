# Installation Guide

This guide provides detailed instructions for installing ContentTokens in your Optimizely CMS project.

## System Requirements

### Minimum Requirements

- **.NET Version**: .NET 8.0 or later
- **Optimizely CMS**: Version 12.x or later
- **ASP.NET Core**: 8.0 or later

### Recommended

- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
- **Database**: SQL Server 2016 or later (for Optimizely CMS)
- **Browser**: Modern browser for admin interface (Chrome, Firefox, Edge, Safari)

## Installation Methods

### Method 1: NuGet Package Manager (Recommended)

#### Using .NET CLI

```bash
cd /path/to/your/project
dotnet add package ContentTokens
```

#### Using Package Manager Console (Visual Studio)

```powershell
Install-Package ContentTokens
```

#### Using Visual Studio UI

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages..."
3. Click "Browse"
4. Search for "ContentTokens"
5. Click "Install"

### Method 2: Manual Installation

1. Download the latest release from [GitHub Releases](https://github.com/Hangsolow/ContentTokens/releases)
2. Extract the NuGet package (.nupkg file)
3. Add the DLL reference to your project

### Method 3: Build from Source

For development or customization:

```bash
# Clone the repository
git clone https://github.com/Hangsolow/ContentTokens.git
cd ContentTokens

# Build the project
dotnet build src/ContentTokens/ContentTokens.csproj -c Release

# Add project reference to your solution
dotnet add reference /path/to/ContentTokens/src/ContentTokens/ContentTokens.csproj
```

## Post-Installation Configuration

### 1. Register the Middleware and Blazor Support

Add ContentTokens middleware and Blazor Server support to your ASP.NET Core pipeline.

**For Optimizely CMS 12 (.NET 6/8):**

Edit your `Program.cs`:

```csharp
using ContentTokens.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services (Optimizely CMS services, etc.)
builder.Services.AddCms();

// Add Blazor Server support for the admin interface
builder.Services.AddServerSideBlazor();

// ... other services

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Optimizely CMS middleware
app.UseAuthentication();
app.UseAuthorization();

// *** Add ContentTokens middleware here ***
app.UseContentTokens();

app.UseEndpoints(endpoints =>
{
    endpoints.MapContent();
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub(); // Required for Blazor admin interface
});

app.Run();
```

**Important:** 
- Place `app.UseContentTokens()` after `UseRouting()`, `UseAuthentication()`, and `UseAuthorization()`
- But before `UseEndpoints()`
- Include `builder.Services.AddServerSideBlazor()` and `endpoints.MapBlazorHub()` for the Blazor admin interface

### 2. Verify Installation

After adding the middleware, verify the installation:

#### Check Admin Interface

1. Build and run your project
2. Log in to Optimizely CMS admin panel
3. Navigate to the **Admin** menu
4. Look for **Content Tokens**

If the menu item appears and opens the Blazor interface, the installation was successful!

#### Check REST API

The REST API should be available at `/api/contenttokens`:

```bash
curl http://localhost:5000/api/contenttokens
```

Expected response: `[]` (empty array if no tokens exist yet)

#### Check Dependencies

Verify all dependencies are installed:

```bash
dotnet list package
```

You should see ContentTokens in the list.

## Configuration Options

### Default Configuration

ContentTokens works out of the box with sensible defaults. No additional configuration is required for basic usage.

### Advanced Configuration (Optional)

For advanced scenarios, you can customize the behavior:

#### Custom Service Registration

If you need to replace the default token service:

```csharp
using ContentTokens.Services;

builder.Services.AddSingleton<IContentTokenService, YourCustomTokenService>();
```

#### Middleware Options

The middleware can be extended with custom behavior:

```csharp
// In your Startup or Program.cs
app.Use(async (context, next) =>
{
    // Custom pre-processing
    await next();
    // Custom post-processing
});

app.UseContentTokens();
```

## Troubleshooting Installation

### Issue: Package Not Found

**Error:** `Package 'ContentTokens' not found`

**Solution:**
1. Ensure you have the correct NuGet source configured
2. Check your internet connection
3. Try clearing NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```

### Issue: Admin Interface Not Appearing

**Possible Causes:**
1. Blazor Server not configured
2. Middleware not registered correctly
3. Menu provider not loaded

**Solutions:**
1. Verify `builder.Services.AddServerSideBlazor()` is called before building the app
2. Verify `endpoints.MapBlazorHub()` is included in the endpoint configuration
3. Verify `app.UseContentTokens()` is called in the correct order
4. Check that the ContentTokens assembly is loaded
5. Clear browser cache
6. Restart the application

### Issue: Build Errors

**Error:** `Could not find ContentTokens.dll`

**Solution:**
1. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```

### Issue: Runtime Errors

**Error:** `Unable to resolve service for type 'IContentTokenService'`

**Solution:**
The ContentTokens initialization module should register this automatically. Ensure:
1. The addon DLL is in the bin folder
2. Optimizely's module system is properly configured
3. The application has restarted after installation

## Upgrading

### Upgrading from a Previous Version

1. Update the NuGet package:
   ```bash
   dotnet add package ContentTokens --version [new-version]
   ```

2. Review [CHANGELOG.md](../CHANGELOG.md) for breaking changes

3. Test your application thoroughly

4. Update any custom implementations if needed

## Uninstalling

To remove ContentTokens:

1. Remove the Blazor Server services and middleware registration from `Program.cs`:
   ```csharp
   // Remove these lines:
   // builder.Services.AddServerSideBlazor();
   // app.UseContentTokens();
   // endpoints.MapBlazorHub();
   ```

2. Remove the NuGet package:
   ```bash
   dotnet remove package ContentTokens
   ```

3. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   ```

**Note:** Token data is stored in Optimizely's Dynamic Data Store and will remain in the database. To remove token data, manually delete entries from the DDS or use the REST API.

## Next Steps

- [Configuration Guide](configuration.md) - Advanced configuration
- [Getting Started Guide](getting-started.md) - Create your first token
- [API Reference](api-reference.md) - Explore the REST API

## Support

For installation issues:
- Check the [Troubleshooting Guide](troubleshooting.md)
- Search [existing issues](https://github.com/Hangsolow/ContentTokens/issues)
- Create a [new issue](https://github.com/Hangsolow/ContentTokens/issues/new) with details
