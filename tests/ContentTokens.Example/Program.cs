using ContentTokens.Extensions;
using ContentTokens.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IContentTokenService, ContentTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseAuthorization();

// Add ContentTokens middleware
app.UseContentTokens();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    // Example endpoint that returns HTML with tokens
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
<head>
    <title>ContentTokens Example</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; }
        h1 { color: #333; }
        .example { background: #f5f5f5; padding: 15px; margin: 10px 0; border-radius: 5px; }
        code { background: #e0e0e0; padding: 2px 5px; border-radius: 3px; }
    </style>
</head>
<body>
    <h1>ContentTokens Example</h1>
    <p>This page demonstrates the ContentTokens addon for Optimizely CMS.</p>
    
    <div class='example'>
        <h2>Token Usage Example</h2>
        <p>Company Name: <strong>{{CompanyName}}</strong></p>
        <p>Support Email: <strong>{{SupportEmail}}</strong></p>
        <p>Phone Number: <strong>{{PhoneNumber}}</strong></p>
        <p>Welcome Message: <strong>{{WelcomeMessage}}</strong></p>
    </div>
    
    <div class='example'>
        <h2>How It Works</h2>
        <p>The above tokens like <code>{{CompanyName}}</code> will be automatically replaced with their configured values.</p>
        <p>If you haven't configured any tokens yet, they will remain as-is.</p>
    </div>
    
    <div class='example'>
        <h2>Managing Tokens</h2>
        <p>Use the REST API to manage tokens:</p>
        <ul>
            <li>GET <code>/api/contenttokens</code> - List all tokens</li>
            <li>POST <code>/api/contenttokens</code> - Create/update a token</li>
            <li>DELETE <code>/api/contenttokens/{id}</code> - Delete a token</li>
        </ul>
    </div>
    
    <div class='example'>
        <h2>Try It Out</h2>
        <p>Create a token using curl:</p>
        <pre>curl -X POST http://localhost:5000/api/contenttokens \
  -H ""Content-Type: application/json"" \
  -d '{
    ""name"": ""CompanyName"",
    ""value"": ""Acme Corporation"",
    ""description"": ""Company name used throughout the site""
  }'</pre>
        <p>Then refresh this page to see the token replaced!</p>
    </div>
</body>
</html>");
    });
});

app.Run();
