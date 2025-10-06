# Testing

This document provides comprehensive testing strategies and examples for ContentTokens implementations.

## Testing Strategy Overview

Testing ContentTokens involves several layers:

1. **Unit Tests**: Test individual components (token service, converters)
2. **Integration Tests**: Test the complete token replacement pipeline
3. **API Tests**: Verify REST API endpoints
4. **Middleware Tests**: Test token replacement in HTTP responses

## Test Project Setup

### Basic Test Project Structure

```
tests/
├── ContentTokens.Tests/
│   ├── Unit/
│   │   ├── Services/
│   │   │   └── ContentTokenServiceTests.cs
│   │   ├── Filters/
│   │   │   └── ContentTokenReplacementMiddlewareTests.cs
│   │   └── Controllers/
│   │       └── ContentTokensControllerTests.cs
│   ├── Integration/
│   │   ├── TokenReplacementTests.cs
│   │   └── ApiIntegrationTests.cs
│   └── TestData/
│       ├── SampleTokens.json
│       └── TestContent.html
└── ContentTokens.Example/    # Example/demo project
```

### Dependencies

Add these NuGet packages to your test project:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\ContentTokens\ContentTokens.csproj" />
  </ItemGroup>
</Project>
```

## Unit Testing

### Testing the Token Service

```csharp
using ContentTokens.Models;
using ContentTokens.Services;
using FluentAssertions;
using Xunit;

public class ContentTokenServiceTests
{
    [Fact]
    public void SaveToken_WithValidToken_StoresSuccessfully()
    {
        // Arrange
        var service = new ContentTokenService();
        var token = new ContentToken
        {
            Name = "TestToken",
            Value = "Test Value",
            Description = "Test token for unit testing"
        };

        // Act
        service.SaveToken(token);
        var retrieved = service.GetToken("TestToken");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("TestToken");
        retrieved.Value.Should().Be("Test Value");
    }

    [Fact]
    public void GetToken_WithNonExistentToken_ReturnsNull()
    {
        // Arrange
        var service = new ContentTokenService();

        // Act
        var result = service.GetToken("NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ReplaceTokens_WithValidToken_ReplacesCorrectly()
    {
        // Arrange
        var service = new ContentTokenService();
        service.SaveToken(new ContentToken
        {
            Name = "CompanyName",
            Value = "Acme Corporation"
        });

        var text = "Welcome to {{CompanyName}}!";

        // Act
        var result = service.ReplaceTokens(text);

        // Assert
        result.Should().Be("Welcome to Acme Corporation!");
    }

    [Fact]
    public void ReplaceTokens_WithMissingToken_LeavesPlaceholder()
    {
        // Arrange
        var service = new ContentTokenService();
        var text = "Welcome to {{NonExistent}}!";

        // Act
        var result = service.ReplaceTokens(text);

        // Assert
        result.Should().Be("Welcome to {{NonExistent}}!");
    }

    [Fact]
    public void ReplaceTokens_WithMultipleTokens_ReplacesAll()
    {
        // Arrange
        var service = new ContentTokenService();
        service.SaveToken(new ContentToken { Name = "Name", Value = "Acme" });
        service.SaveToken(new ContentToken { Name = "Email", Value = "info@acme.com" });

        var text = "{{Name}} at {{Email}}";

        // Act
        var result = service.ReplaceTokens(text);

        // Assert
        result.Should().Be("Acme at info@acme.com");
    }

    [Theory]
    [InlineData("en", "Welcome!")]
    [InlineData("sv", "Välkommen!")]
    [InlineData("de", "Willkommen!")]
    public void GetToken_WithLanguageSpecificToken_ReturnsCorrectValue(
        string languageCode, 
        string expectedValue)
    {
        // Arrange
        var service = new ContentTokenService();
        service.SaveToken(new ContentToken 
        { 
            Name = "Greeting", 
            Value = "Welcome!", 
            LanguageCode = "en" 
        });
        service.SaveToken(new ContentToken 
        { 
            Name = "Greeting", 
            Value = "Välkommen!", 
            LanguageCode = "sv" 
        });
        service.SaveToken(new ContentToken 
        { 
            Name = "Greeting", 
            Value = "Willkommen!", 
            LanguageCode = "de" 
        });

        // Act
        var token = service.GetToken("Greeting", languageCode);

        // Assert
        token.Should().NotBeNull();
        token!.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void GetToken_WithLanguageFallback_ReturnsNeutralToken()
    {
        // Arrange
        var service = new ContentTokenService();
        service.SaveToken(new ContentToken 
        { 
            Name = "CompanyName", 
            Value = "Acme Corporation",
            LanguageCode = null  // Language-neutral
        });

        // Act
        var token = service.GetToken("CompanyName", "fr"); // French not defined

        // Assert
        token.Should().NotBeNull();
        token!.Value.Should().Be("Acme Corporation");
    }

    [Fact]
    public void DeleteToken_WithExistingToken_RemovesSuccessfully()
    {
        // Arrange
        var service = new ContentTokenService();
        var token = new ContentToken { Name = "TempToken", Value = "Temporary" };
        service.SaveToken(token);
        var tokenId = token.Id;

        // Act
        service.DeleteToken(Guid.Parse(tokenId.ToString()));
        var retrieved = service.GetToken("TempToken");

        // Assert
        retrieved.Should().BeNull();
    }
}
```

### Testing the Controller

```csharp
using ContentTokens.Controllers;
using ContentTokens.Models;
using ContentTokens.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

public class ContentTokensControllerTests
{
    private readonly IContentTokenService _tokenService;
    private readonly ContentTokensController _controller;

    public ContentTokensControllerTests()
    {
        _tokenService = Substitute.For<IContentTokenService>();
        _controller = new ContentTokensController(_tokenService);
    }

    [Fact]
    public void GetAll_ReturnsAllTokens()
    {
        // Arrange
        var tokens = new List<ContentToken>
        {
            new ContentToken { Name = "Token1", Value = "Value1" },
            new ContentToken { Name = "Token2", Value = "Value2" }
        };
        _tokenService.GetAllTokens(Arg.Any<string?>()).Returns(tokens);

        // Act
        var result = _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(tokens);
    }

    [Fact]
    public void Get_WithExistingToken_ReturnsToken()
    {
        // Arrange
        var token = new ContentToken { Name = "Test", Value = "TestValue" };
        _tokenService.GetToken("Test", Arg.Any<string?>()).Returns(token);

        // Act
        var result = _controller.Get("Test");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(token);
    }

    [Fact]
    public void Get_WithNonExistentToken_ReturnsNotFound()
    {
        // Arrange
        _tokenService.GetToken("NonExistent", Arg.Any<string?>()).Returns((ContentToken?)null);

        // Act
        var result = _controller.Get("NonExistent");

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Save_WithValidToken_ReturnsOk()
    {
        // Arrange
        var token = new ContentToken { Name = "NewToken", Value = "NewValue" };

        // Act
        var result = _controller.Save(token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _tokenService.Received(1).SaveToken(token);
    }

    [Fact]
    public void Save_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var token = new ContentToken { Name = "", Value = "Value" };

        // Act
        var result = _controller.Save(token);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Delete_CallsServiceDelete()
    {
        // Arrange
        var tokenId = Guid.NewGuid();

        // Act
        var result = _controller.Delete(tokenId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _tokenService.Received(1).DeleteToken(tokenId);
    }

    [Fact]
    public void Preview_WithValidRequest_ReturnsPreview()
    {
        // Arrange
        var request = new PreviewRequest 
        { 
            Text = "Hello {{Name}}", 
            LanguageCode = "en" 
        };
        _tokenService.ReplaceTokens(request.Text, request.LanguageCode)
            .Returns("Hello World");

        // Act
        var result = _controller.Preview(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
```

## Integration Testing

### Testing Token Replacement in HTTP Responses

```csharp
using ContentTokens.Extensions;
using ContentTokens.Models;
using ContentTokens.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class TokenReplacementIntegrationTests
{
    [Fact]
    public async Task Middleware_ReplacesTokensInHtmlResponse()
    {
        // Arrange
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IContentTokenService, ContentTokenService>();
                services.AddControllers();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseContentTokens();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test", async context =>
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync("<h1>{{Title}}</h1>");
                    });
                });
            });

        var server = new TestServer(builder);
        var client = server.CreateClient();

        // Create token
        var tokenService = server.Services.GetRequiredService<IContentTokenService>();
        tokenService.SaveToken(new ContentToken
        {
            Name = "Title",
            Value = "Welcome"
        });

        // Act
        var response = await client.GetAsync("/test");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("Welcome");
        content.Should().NotContain("{{Title}}");
    }

    [Fact]
    public async Task Middleware_PreservesTokensWhenNotDefined()
    {
        // Arrange
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IContentTokenService, ContentTokenService>();
            })
            .Configure(app =>
            {
                app.UseContentTokens();
                app.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("{{UndefinedToken}}");
                });
            });

        var server = new TestServer(builder);
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("{{UndefinedToken}}");
    }
}
```

### Testing the REST API

```csharp
using System.Net;
using System.Net.Http.Json;
using ContentTokens.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTokens_ReturnsEmptyArrayInitially()
    {
        // Act
        var response = await _client.GetAsync("/api/contenttokens");
        var tokens = await response.Content.ReadFromJsonAsync<List<ContentToken>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        tokens.Should().NotBeNull();
        tokens.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateToken_Success()
    {
        // Arrange
        var token = new ContentToken
        {
            Name = "ApiTest",
            Value = "Test Value"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contenttokens", token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdToken = await response.Content.ReadFromJsonAsync<ContentToken>();
        createdToken.Should().NotBeNull();
        createdToken!.Name.Should().Be("ApiTest");
    }

    [Fact]
    public async Task GetToken_AfterCreation_ReturnsToken()
    {
        // Arrange
        var token = new ContentToken { Name = "GetTest", Value = "Value" };
        await _client.PostAsJsonAsync("/api/contenttokens", token);

        // Act
        var response = await _client.GetAsync("/api/contenttokens/GetTest");
        var retrieved = await response.Content.ReadFromJsonAsync<ContentToken>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("GetTest");
    }

    [Fact]
    public async Task DeleteToken_RemovesToken()
    {
        // Arrange
        var token = new ContentToken { Name = "DeleteTest", Value = "Value" };
        var createResponse = await _client.PostAsJsonAsync("/api/contenttokens", token);
        var created = await createResponse.Content.ReadFromJsonAsync<ContentToken>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/contenttokens/{created!.Id}");
        var getResponse = await _client.GetAsync("/api/contenttokens/DeleteTest");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PreviewEndpoint_ReplacesTokens()
    {
        // Arrange
        var token = new ContentToken { Name = "PreviewTest", Value = "Replaced" };
        await _client.PostAsJsonAsync("/api/contenttokens", token);

        var request = new { Text = "{{PreviewTest}}", LanguageCode = (string?)null };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contenttokens/preview", request);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().ContainKey("replaced");
        result!["replaced"].Should().Contain("Replaced");
    }
}
```

## Manual Testing

### Testing with the Example Project

1. **Start the example application:**
   ```bash
   cd tests/ContentTokens.Example
   dotnet run
   ```

2. **Create test tokens:**
   ```bash
   curl -X POST http://localhost:5065/api/contenttokens \
     -H "Content-Type: application/json" \
     -d '{"name":"TestToken","value":"Test Value"}'
   ```

3. **View the homepage:**
   - Navigate to http://localhost:5065
   - Verify tokens are replaced

4. **Test the API:**
   ```bash
   # List all tokens
   curl http://localhost:5065/api/contenttokens
   
   # Get specific token
   curl http://localhost:5065/api/contenttokens/TestToken
   
   # Preview replacement
   curl -X POST http://localhost:5065/api/contenttokens/preview \
     -H "Content-Type: application/json" \
     -d '{"text":"Hello {{TestToken}}"}'
   ```

## Performance Testing

### Benchmark Token Replacement

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ContentTokens.Models;
using ContentTokens.Services;

[MemoryDiagnoser]
public class TokenReplacementBenchmarks
{
    private IContentTokenService _service;
    private string _textWithTokens;

    [GlobalSetup]
    public void Setup()
    {
        _service = new ContentTokenService();
        _service.SaveToken(new ContentToken { Name = "Token1", Value = "Value1" });
        _service.SaveToken(new ContentToken { Name = "Token2", Value = "Value2" });
        _textWithTokens = "{{Token1}} and {{Token2}}";
    }

    [Benchmark]
    public string ReplaceTokens()
    {
        return _service.ReplaceTokens(_textWithTokens);
    }

    [Benchmark]
    public string ReplaceTokens_LargeText()
    {
        var largeText = string.Join(" ", Enumerable.Repeat("{{Token1}} {{Token2}}", 1000));
        return _service.ReplaceTokens(largeText);
    }
}

// Run with: dotnet run -c Release --project Benchmarks
```

## Best Practices

### 1. Isolate Tests

Each test should be independent and not rely on the state from other tests.

### 2. Use Test Data Builders

Create reusable test data builders:

```csharp
public class TokenBuilder
{
    private string _name = "DefaultToken";
    private string _value = "DefaultValue";
    private string? _languageCode = null;

    public TokenBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TokenBuilder WithValue(string value)
    {
        _value = value;
        return this;
    }

    public TokenBuilder WithLanguage(string languageCode)
    {
        _languageCode = languageCode;
        return this;
    }

    public ContentToken Build()
    {
        return new ContentToken
        {
            Name = _name,
            Value = _value,
            LanguageCode = _languageCode
        };
    }
}

// Usage:
var token = new TokenBuilder()
    .WithName("CompanyName")
    .WithValue("Acme Corp")
    .WithLanguage("en")
    .Build();
```

### 3. Test Edge Cases

Always test:
- Null/empty inputs
- Special characters in token names/values
- Very long token values
- Concurrent access
- Language fallbacks

### 4. Use Meaningful Test Names

```csharp
// Good
[Fact]
public void ReplaceTokens_WithMultipleTokens_ReplacesAllOccurrences()

// Bad
[Fact]
public void Test1()
```

## Continuous Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

## Troubleshooting Tests

### Test Failing: "Token not found"

**Cause:** Token not saved before retrieval  
**Solution:** Ensure SaveToken is called before GetToken

### Test Failing: "Middleware not replacing tokens"

**Cause:** Middleware not registered correctly  
**Solution:** Verify `app.UseContentTokens()` is in the pipeline

### Test Timing Out

**Cause:** Long-running operations  
**Solution:** Use async/await and set appropriate timeouts

## Next Steps

- [Configuration](configuration.md) - Configure testing environment
- [Troubleshooting](troubleshooting.md) - Debug test failures
- [API Reference](api-reference.md) - Understand the API for testing
