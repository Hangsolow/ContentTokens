# Blazor CMS Admin Addon

The ContentTokens addon now includes a modern Blazor-based admin interface for managing content tokens within the Optimizely CMS admin area.

## Features

- **Modern UI**: Built with Blazor Server and follows Optimizely Axiom design guidelines
- **Full CRUD Operations**: Create, read, update, and delete content tokens
- **Validation**: Client-side validation for token names (alphanumeric only)
- **Multilingual Support**: Optional language code for language-specific tokens
- **User-Friendly**: Clean, intuitive interface with success/error messaging

## Accessing the Addon

After installation, the ContentTokens admin interface is available in the Optimizely CMS admin menu:

1. Log in to your Optimizely CMS admin interface
2. Navigate to **Admin** menu
3. Click on **Content Tokens**

The addon will appear at `/contenttokens` in your CMS admin area.

## Using the Interface

### Adding a Token

1. Click the **Add Token** button
2. Fill in the required fields:
   - **Token Name**: Alphanumeric only (e.g., `CompanyName`, `Year2024`)
   - **Token Value**: The actual value to replace the token with
   - **Language Code** (optional): e.g., `en`, `sv`, `de`
   - **Description** (optional): What this token represents
3. Click **Save**

### Editing a Token

1. Find the token in the list
2. Click the **Edit** button
3. Modify the fields as needed
4. Click **Save**

### Deleting a Token

1. Find the token in the list
2. Click the **Delete** button
3. Confirm the deletion

## Token Naming Rules

Token names must be **alphanumeric only**:
- ✅ `CompanyName`
- ✅ `SupportEmail`
- ✅ `Year2024`
- ❌ `Company Name` (contains space)
- ❌ `Support-Email` (contains hyphen)
- ❌ `current_year` (contains underscore)

## Technical Details

### Architecture

The Blazor addon consists of:

1. **ContentTokensManager.razor** - Main Blazor component for token management
2. **ContentTokensAddonController.cs** - MVC controller serving the Blazor view
3. **ContentTokensMenuProvider.cs** - Registers the menu item in CMS admin
4. **Index.cshtml** - Host view for the Blazor component

### Integration

The addon integrates with the existing ContentTokens API (`/api/contenttokens`) and uses the same `IContentTokenService` for all operations.

### Blazor Server Configuration

The addon uses Blazor Server mode. Ensure your `Program.cs` includes:

```csharp
// Add Blazor Server support
builder.Services.AddServerSideBlazor();

// ... other configuration ...

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    // ... other endpoints ...
});
```

### Authorization

The controller is configured to require admin access. By default, it uses:

```csharp
[Authorize(Policy = "CmsAdmins,CmsEditors,WebAdmins")]
```

Adjust the policy as needed for your security requirements.

## Design

The UI follows Optimizely's Axiom design system:
- Clean, modern interface
- Consistent with CMS UI patterns
- Responsive design
- Accessible components
- Clear success/error messaging

## Troubleshooting

### Addon Not Appearing in Menu

1. Ensure Blazor Server is configured in `Program.cs`
2. Check that the ContentTokens package is properly installed
3. Verify the user has appropriate permissions

### Token Operations Failing

1. Check the browser console for JavaScript errors
2. Verify the API at `/api/contenttokens` is accessible
3. Ensure the DynamicDataStore is properly configured

## Migrating from JavaScript Gadget

The old JavaScript/Dojo-based gadget has been removed. The new Blazor addon provides:

- Better performance
- Modern UI
- Easier maintenance
- Better integration with the CMS
- Same functionality with improved UX

No data migration is required - all existing tokens continue to work.
