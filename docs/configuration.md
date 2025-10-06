# Configuration Guide

This guide covers advanced configuration options for ContentTokens.

## Basic Configuration

ContentTokens works with minimal configuration. After installing the NuGet package, simply add the middleware:

```csharp
app.UseContentTokens();
```

## Middleware Configuration

### Placement in Pipeline

The middleware placement is critical for proper functionality:

```csharp
app.UseRouting();              // 1. Routing must come first
app.UseAuthentication();       // 2. Authentication
app.UseAuthorization();        // 3. Authorization
app.UseContentTokens();        // 4. ContentTokens (after auth, before endpoints)
app.UseEndpoints(...);         // 5. Endpoints last
```

### Conditional Middleware

Enable token replacement only in specific environments:

```csharp
if (app.Environment.IsProduction() || app.Environment.IsStaging())
{
    app.UseContentTokens();
}
```

### Custom Content Type Processing

By default, ContentTokens processes `text/html` responses. To extend this:

```csharp
// Create a custom middleware wrapper
app.Use(async (context, next) =>
{
    // Process additional content types
    if (context.Response.ContentType?.Contains("text/xml") == true)
    {
        // Custom processing logic
    }
    await next();
});

app.UseContentTokens();
```

## Service Configuration

### Custom Token Service

Implement your own token storage or logic:

```csharp
using ContentTokens.Services;

public class CustomTokenService : IContentTokenService
{
    // Implement interface methods
    // E.g., use Redis, SQL, or external API
}

// Register in Program.cs
builder.Services.AddSingleton<IContentTokenService, CustomTokenService>();
```

### Service Lifetime

The default service is registered as Singleton. To change:

```csharp
// Scoped service (per-request)
builder.Services.AddScoped<IContentTokenService, ContentTokenService>();

// Transient service (per-injection)
builder.Services.AddTransient<IContentTokenService, ContentTokenService>();
```

## Storage Configuration

### Dynamic Data Store

ContentTokens uses Optimizely's Dynamic Data Store by default. No configuration needed.

### Custom Storage Backend

To use a different storage mechanism:

```csharp
public class SqlTokenService : IContentTokenService
{
    private readonly IDbConnection _connection;
    
    public SqlTokenService(IDbConnection connection)
    {
        _connection = connection;
    }
    
    public IEnumerable<ContentToken> GetAllTokens(string? languageCode = null)
    {
        // Custom SQL query
        return _connection.Query<ContentToken>(
            "SELECT * FROM ContentTokens WHERE LanguageCode = @lang",
            new { lang = languageCode }
        );
    }
    
    // Implement other methods...
}

// Register
builder.Services.AddSingleton<IDbConnection>(sp => 
    new SqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<IContentTokenService, SqlTokenService>();
```

## API Configuration

### Customize API Routes

The default API route is `/api/contenttokens`. To customize:

```csharp
[Route("api/v1/tokens")]  // Custom route
public class ContentTokensController : ControllerBase
{
    // Controller implementation
}
```

### Authorization Configuration

By default, the API allows anonymous access in the example project. For production:

```csharp
[Authorize(Roles = "CmsAdmins,CmsEditors,WebAdmins")]
[Route("api/contenttokens")]
[ApiController]
public class ContentTokensController : ControllerBase
{
    // Secure endpoints
}
```

Or configure policies:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TokenManagement", policy =>
        policy.RequireRole("CmsAdmins", "CmsEditors"));
});

// In controller
[Authorize(Policy = "TokenManagement")]
public class ContentTokensController : ControllerBase
{
    // ...
}
```

### CORS Configuration

If accessing the API from external sources:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ContentTokensAPI", builder =>
    {
        builder.WithOrigins("https://your-domain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// In pipeline
app.UseCors("ContentTokensAPI");
app.UseContentTokens();
```

## Token Pattern Configuration

### Custom Token Syntax

The default pattern is `{{TokenName}}`. To use a custom pattern:

```csharp
public class CustomTokenService : ContentTokenService
{
    private static readonly Regex CustomTokenRegex = 
        new Regex(@"\[\[(\w+)\]\]", RegexOptions.Compiled);
    
    public override string ReplaceTokens(string text, string? languageCode = null)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return CustomTokenRegex.Replace(text, match =>
        {
            var tokenName = match.Groups[1].Value;
            var token = GetToken(tokenName, languageCode);
            return token?.Value ?? match.Value;
        });
    }
}
```

This changes token syntax from `{{Name}}` to `[[Name]]`.

## Caching Configuration

### In-Memory Caching

Add caching to improve performance:

```csharp
public class CachedTokenService : IContentTokenService
{
    private readonly IContentTokenService _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

    public CachedTokenService(ContentTokenService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public IEnumerable<ContentToken> GetAllTokens(string? languageCode = null)
    {
        var cacheKey = $"tokens_{languageCode ?? "all"}";
        
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheExpiration;
            return _inner.GetAllTokens(languageCode);
        });
    }
    
    // Implement other methods with cache invalidation...
}

// Register
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ContentTokenService>();
builder.Services.AddSingleton<IContentTokenService, CachedTokenService>();
```

### Distributed Caching

For multi-server environments, use Redis or SQL Server cache:

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "ContentTokens:";
});

// Use IDistributedCache in your service
```

## Logging Configuration

### Enable Detailed Logging

```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddFilter("ContentTokens", LogLevel.Debug);
});
```

### Custom Logger

```csharp
public class ContentTokenService : IContentTokenService
{
    private readonly ILogger<ContentTokenService> _logger;
    
    public ContentTokenService(ILogger<ContentTokenService> logger)
    {
        _logger = logger;
    }
    
    public void SaveToken(ContentToken token)
    {
        _logger.LogInformation("Saving token: {TokenName}", token.Name);
        // Save logic
        _logger.LogInformation("Token saved successfully: {TokenId}", token.Id);
    }
}
```

## Performance Configuration

### Regex Optimization

The default regex is already compiled for performance:

```csharp
private static readonly Regex TokenRegex = 
    new Regex(@"\{\{(\w+)\}\}", RegexOptions.Compiled);
```

For even better performance with many tokens, consider:

```csharp
private static readonly Regex TokenRegex = 
    new Regex(@"\{\{(\w+)\}\}", 
        RegexOptions.Compiled | 
        RegexOptions.CultureInvariant);
```

### Response Buffering

For large pages, consider buffering:

```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.BufferBody = true;
    options.BufferBodyLengthLimit = 128 * 1024 * 1024; // 128MB
});
```

## Environment-Specific Configuration

### appsettings.json

```json
{
  "ContentTokens": {
    "Enabled": true,
    "CacheExpirationMinutes": 10,
    "MaxTokenLength": 1000,
    "AllowAnonymousAccess": false
  }
}
```

### Use Configuration in Code

```csharp
public class ContentTokenOptions
{
    public bool Enabled { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 10;
    public int MaxTokenLength { get; set; } = 1000;
    public bool AllowAnonymousAccess { get; set; } = false;
}

// In Program.cs
builder.Services.Configure<ContentTokenOptions>(
    builder.Configuration.GetSection("ContentTokens"));

// In your service
public class ContentTokenService
{
    private readonly ContentTokenOptions _options;
    
    public ContentTokenService(IOptions<ContentTokenOptions> options)
    {
        _options = options.Value;
    }
}
```

### Environment Variables

Override settings with environment variables:

```bash
export ContentTokens__Enabled=false
export ContentTokens__CacheExpirationMinutes=30
```

## Security Configuration

### Input Validation

Add validation for token values:

```csharp
public void SaveToken(ContentToken token)
{
    if (token.Name.Length > 100)
        throw new ArgumentException("Token name too long");
    
    if (token.Value.Length > _options.MaxTokenLength)
        throw new ArgumentException("Token value too long");
    
    // Save logic
}
```

### XSS Protection

Token values are rendered as-is. To HTML-encode:

```csharp
public string ReplaceTokens(string text, string? languageCode = null)
{
    return TokenRegex.Replace(text, match =>
    {
        var token = GetToken(match.Groups[1].Value, languageCode);
        return token != null 
            ? System.Net.WebUtility.HtmlEncode(token.Value)
            : match.Value;
    });
}
```

## Next Steps

- [API Reference](api-reference.md) - Complete API documentation
- [Examples](examples.md) - Configuration examples
- [Troubleshooting](troubleshooting.md) - Common configuration issues
