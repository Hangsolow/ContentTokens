# Troubleshooting Guide

This guide helps you diagnose and fix common issues with ContentTokens.

## Common Issues

### Issue: Tokens Not Being Replaced

**Symptoms:**
- Tokens like `{{TokenName}}` appear as-is in the rendered HTML
- No replacement occurs

**Possible Causes & Solutions:**

#### 1. Middleware Not Registered

**Check:** Is `app.UseContentTokens()` in your `Program.cs`?

```csharp
// Correct placement
app.UseRouting();
app.UseAuthorization();
app.UseContentTokens();  // ← Must be here
app.UseEndpoints(...);
```

**Solution:** Add the middleware in the correct order.

#### 2. Token Doesn't Exist

**Check:** Verify the token exists in the database.

```bash
curl http://localhost:5000/api/contenttokens
```

**Solution:** Create the token via the API or admin gadget.

#### 3. Wrong Content Type

The middleware only processes `text/html` by default.

**Check:** Response Content-Type header

**Solution:** Ensure your response has `Content-Type: text/html`

#### 4. Token Name Mismatch

Tokens are case-sensitive!

**Check:** 
- Token in database: `CompanyName`
- Token in content: `{{companyname}}` ❌
- Should be: `{{CompanyName}}` ✅

**Solution:** Use exact token name with correct casing.

#### 5. Invalid Token Syntax

**Check:**
- Valid: `{{TokenName}}`
- Invalid: `{{ TokenName }}` (spaces)
- Invalid: `{TokenName}` (single braces)
- Invalid: `{{Token Name}}` (space in name)

**Solution:** Use valid syntax with no spaces.

---

### Issue: Admin Gadget Not Visible

**Symptoms:**
- "Content Tokens" gadget doesn't appear in CMS dashboard

**Possible Causes & Solutions:**

#### 1. Module Not Loaded

**Check:** Look for errors in the application logs during startup.

**Solution:** 
- Verify the DLL is in the bin folder
- Restart the application
- Check Optimizely module initialization logs

#### 2. Permissions Issue

**Check:** User's CMS permissions

**Solution:** Ensure user has appropriate role:
- CmsAdmins
- CmsEditors
- WebAdmins

#### 3. Browser Cache

**Check:** Clear browser cache

**Solution:**
```
Ctrl+Shift+Delete (Windows)
Cmd+Shift+Delete (Mac)
```

Then reload the page.

#### 4. Module Configuration

**Check:** Verify `module.config` is embedded in assembly

**Solution:** Check `.csproj`:
```xml
<ItemGroup>
  <EmbeddedResource Include="module.config" />
</ItemGroup>
```

---

### Issue: API Returns 401 Unauthorized

**Symptoms:**
- API requests return 401 status code
- Cannot create or manage tokens via API

**Possible Causes & Solutions:**

#### 1. Authentication Required

**Check:** Controller has `[Authorize]` attribute

```csharp
[Authorize(Roles = "CmsAdmins,CmsEditors,WebAdmins")]
public class ContentTokensController : ControllerBase
```

**Solution:** 
- Log in to the application
- Include authentication cookie/header in API requests
- For testing, temporarily use `[AllowAnonymous]`

#### 2. Missing Authorization Middleware

**Check:** `app.UseAuthorization()` is registered

```csharp
app.UseAuthentication();
app.UseAuthorization();  // ← Required
```

**Solution:** Add authorization middleware before ContentTokens.

---

### Issue: API Returns 403 Forbidden

**Symptoms:**
- Authenticated but API returns 403
- User cannot manage tokens

**Possible Causes & Solutions:**

**Check:** User's role assignments

**Solution:** Assign user to one of these roles:
- CmsAdmins
- CmsEditors  
- WebAdmins

---

### Issue: Language-Specific Tokens Not Working

**Symptoms:**
- Language-specific tokens not being used
- Always gets default token

**Possible Causes & Solutions:**

#### 1. Language Code Mismatch

**Check:** Token language code vs. current culture

```bash
# List all tokens
curl http://localhost:5000/api/contenttokens

# Check what languages exist
```

**Solution:** Ensure language codes match exactly (e.g., "en" not "en-US")

#### 2. Culture Not Set

**Check:** Current culture in the application

```csharp
var culture = ContentLanguage.PreferredCulture;
Console.WriteLine($"Current culture: {culture?.Name}");
```

**Solution:** Ensure Optimizely CMS language routing is configured.

---

### Issue: Performance Problems

**Symptoms:**
- Slow page loads
- High CPU usage
- Memory issues

**Possible Causes & Solutions:**

#### 1. No Caching

**Check:** Token service has no caching layer

**Solution:** Implement caching (see [Configuration Guide](configuration.md))

```csharp
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IContentTokenService, CachedTokenService>();
```

#### 2. Too Many Tokens

**Check:** Number of tokens in database

```bash
curl http://localhost:5000/api/contenttokens | jq length
```

**Solution:** 
- Remove unused tokens
- Consider pagination
- Implement indexing

#### 3. Large Token Values

**Check:** Token value sizes

**Solution:** 
- Limit token value length
- Don't store large HTML blocks
- Use separate storage for large content

---

### Issue: Build Errors

#### Error: "Package ContentTokens not found"

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

#### Error: "Type 'IContentTokenService' could not be found"

**Check:** Using directive present

```csharp
using ContentTokens.Services;
```

**Solution:** Add the using statement or fully qualify the type.

#### Error: "Assembly load exception"

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

---

### Issue: Runtime Errors

#### Error: "Unable to resolve service for type 'IContentTokenService'"

**Symptoms:**
- Application crashes on startup
- Dependency injection error

**Check:** Service registration in initialization module

**Solution:** The ContentTokens initialization module should register this automatically. If not:

```csharp
builder.Services.AddSingleton<IContentTokenService, ContentTokenService>();
```

#### Error: "NullReferenceException in Token Service"

**Symptoms:**
- Service throws exception
- Token replacement fails

**Solution:**
- Check token service is registered
- Verify database connectivity
- Check application logs for details

#### Error: "ObjectDisposedException: Cannot access a closed Stream"

**Symptoms:**
- Error in processing
- Intermittent failures

**Solution:** This was fixed in version 1.0. Ensure you're using the latest version.

---

## Debugging Tips

### Enable Detailed Logging

Add to `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "ContentTokens": "Debug"
    }
  }
}
```

### Check Token Replacement

Test token replacement without the middleware:

```csharp
var tokenService = app.Services.GetRequiredService<IContentTokenService>();
var result = tokenService.ReplaceTokens("Test: {{TokenName}}");
Console.WriteLine(result);
```

### Verify Middleware Execution

Add logging to see if middleware is executing:

```csharp
app.Use(async (context, next) =>
{
    Console.WriteLine($"Before ContentTokens: {context.Request.Path}");
    await next();
    Console.WriteLine($"After ContentTokens: {context.Response.ContentType}");
});

app.UseContentTokens();
```

### Test API Directly

Use curl to test API endpoints:

```bash
# Get all tokens
curl -v http://localhost:5000/api/contenttokens

# Create token
curl -v -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","value":"TestValue"}'

# Preview replacement
curl -v -X POST http://localhost:5000/api/contenttokens/preview \
  -H "Content-Type: application/json" \
  -d '{"text":"Hello {{Test}}"}'
```

### Check Database

Tokens are stored in Optimizely's Dynamic Data Store:

```sql
-- For SQL Server
SELECT * FROM tblBigTable 
WHERE StoreName LIKE '%ContentToken%'
```

---

## Getting Help

### Before Opening an Issue

1. Check this troubleshooting guide
2. Review the [documentation](getting-started.md)
3. Search [existing issues](https://github.com/Hangsolow/ContentTokens/issues)
4. Try the example project

### When Opening an Issue

Include:

1. **Environment:**
   - .NET version
   - Optimizely CMS version
   - ContentTokens version
   - Operating system

2. **Reproduction Steps:**
   - What you did
   - What you expected
   - What actually happened

3. **Logs:**
   - Error messages
   - Stack traces
   - Relevant log output

4. **Code:**
   - Middleware configuration
   - Token creation code
   - Sample content with tokens

### Example Issue Report

```markdown
## Environment
- .NET: 8.0
- Optimizely CMS: 12.10
- ContentTokens: 1.0.0
- OS: Windows 11

## Issue
Tokens not being replaced in production

## Steps to Reproduce
1. Deploy to Azure App Service
2. Create token via API
3. View page with token
4. Token appears as {{TokenName}}

## Expected
Token should be replaced with value

## Actual
Token remains as {{TokenName}}

## Logs
[Include relevant logs]

## Code
[Include middleware configuration]
```

---

## FAQ

### Q: Can I use tokens in email templates?

**A:** Yes! Use the token service directly:

```csharp
var email = _tokenService.ReplaceTokens(emailTemplate, languageCode);
```

### Q: Are tokens case-sensitive?

**A:** Yes, `{{CompanyName}}` and `{{companyname}}` are different.

### Q: Can I use special characters in token names?

**A:** No, only alphanumeric characters (letters and numbers) are allowed.

### Q: How many tokens can I create?

**A:** There's no hard limit, but performance may degrade with thousands of tokens. Consider caching.

### Q: Can I use tokens in JSON/XML responses?

**A:** By default, only HTML responses are processed. You can extend the middleware for other content types.

### Q: Do tokens work with output caching?

**A:** Yes, but the tokens are replaced when the page is cached. Updates to token values won't appear until cache expires.

### Q: Can I use tokens in JavaScript?

**A:** Yes, but be careful with escaping:

```html
<script>
var message = "{{JavaScriptMessage}}";  // Ensure value is JS-safe
</script>
```

### Q: How do I backup tokens?

**A:** Use the API to export all tokens:

```bash
curl http://localhost:5000/api/contenttokens > tokens-backup.json
```

---

## Additional Resources

- [Getting Started](getting-started.md)
- [API Reference](api-reference.md)
- [Configuration Guide](configuration.md)
- [Advanced Usage](advanced-usage.md)
- [GitHub Issues](https://github.com/Hangsolow/ContentTokens/issues)
