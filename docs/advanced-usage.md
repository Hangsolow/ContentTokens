# Advanced Usage

This guide provides practical examples and advanced patterns for using ContentTokens in various scenarios.

## Basic Usage

### Example 1: Simple Token Replacement

**Create tokens:**
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{"name": "CompanyName", "value": "Acme Corporation"}'

curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{"name": "Year", "value": "2025"}'
```

**Use in content:**
```html
<footer>
  <p>&copy; {{Year}} {{CompanyName}}. All rights reserved.</p>
</footer>
```

**Result:**
```html
<footer>
  <p>&copy; 2025 Acme Corporation. All rights reserved.</p>
</footer>
```

### Example 2: Contact Information

**Create contact tokens:**
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "SupportEmail",
    "value": "support@acme.com",
    "description": "Primary support email"
  }'

curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "SalesPhone",
    "value": "+1-555-0123",
    "description": "Sales department phone"
  }'
```

**Use in content:**
```html
<div class="contact-info">
  <p>Email us at <a href="mailto:{{SupportEmail}}">{{SupportEmail}}</a></p>
  <p>Call our sales team at <a href="tel:{{SalesPhone}}">{{SalesPhone}}</a></p>
</div>
```

## Multilingual Advanced Usage

### Example 3: Language-Specific Welcome Messages

**Create tokens for multiple languages:**
```bash
# English
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Welcome to our website!",
    "languageCode": "en"
  }'

# Swedish
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Välkommen till vår webbplats!",
    "languageCode": "sv"
  }'

# German
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "WelcomeMessage",
    "value": "Willkommen auf unserer Website!",
    "languageCode": "de"
  }'
```

**Use in content:**
```html
<h1>{{WelcomeMessage}}</h1>
```

**Result (depends on current language):**
- English: "Welcome to our website!"
- Swedish: "Välkommen till vår webbplats!"
- German: "Willkommen auf unserer Website!"

### Example 4: Fallback Behavior

**Create language-neutral fallback:**
```bash
# Fallback (no language code)
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TermsLink",
    "value": "/terms-and-conditions"
  }'

# English-specific
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TermsLink",
    "value": "/en/terms-and-conditions",
    "languageCode": "en"
  }'
```

**Behavior:**
- When viewing in English: uses `/en/terms-and-conditions`
- When viewing in French (no French token): falls back to `/terms-and-conditions`

## Advanced Advanced Usage

### Example 5: Dynamic Campaign Content

**Seasonal promotions:**
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CurrentPromotion",
    "value": "Summer Sale - 30% Off!",
    "description": "Current promotional message"
  }'

curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "PromoBanner",
    "value": "<div class=\"promo-banner\">🎉 Use code SUMMER30 for 30% off!</div>",
    "description": "Promotional banner HTML"
  }'
```

**Use in multiple pages:**
```html
<!-- Homepage -->
<h2>{{CurrentPromotion}}</h2>
{{PromoBanner}}

<!-- Product page -->
<aside>{{PromoBanner}}</aside>

<!-- Cart page -->
<div class="discount-notice">{{CurrentPromotion}}</div>
```

**Update promotion:** Simply update the token values, and all pages reflect the change instantly.

### Example 6: A/B Testing with Tokens

**Create variant tokens:**
```bash
# Control variant
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CTAButton",
    "value": "Buy Now"
  }'

# Test variant (update when testing)
# curl -X POST http://localhost:5000/api/contenttokens \
#   -H "Content-Type: application/json" \
#   -d '{
#     "name": "CTAButton",
#     "value": "Get Started Today"
#   }'
```

**Use in template:**
```html
<button class="cta-button">{{CTAButton}}</button>
```

### Example 7: Legal and Compliance Text

**Create compliance tokens:**
```bash
curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "CopyrightNotice",
    "value": "© 2025 Acme Corporation. All rights reserved.",
    "description": "Standard copyright notice"
  }'

curl -X POST http://localhost:5000/api/contenttokens \
  -H "Content-Type: application/json" \
  -d '{
    "name": "GDPRNotice",
    "value": "We use cookies to improve your experience. By using our site, you agree to our cookie policy.",
    "description": "GDPR cookie notice"
  }'
```

**Use site-wide:**
```html
<!-- Footer -->
<footer>
  <p>{{CopyrightNotice}}</p>
</footer>

<!-- Cookie banner -->
<div class="cookie-banner">{{GDPRNotice}}</div>
```

## Programmatic Usage

### Example 8: Using the Service in C# Code

```csharp
using ContentTokens.Services;

public class EmailService
{
    private readonly IContentTokenService _tokenService;
    
    public EmailService(IContentTokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public string BuildEmailContent(string template, string language = "en")
    {
        // Email template with tokens
        var emailTemplate = @"
            Dear Customer,
            
            {{WelcomeMessage}}
            
            Please contact us at {{SupportEmail}} if you have any questions.
            
            Best regards,
            {{CompanyName}} Team
        ";
        
        // Replace tokens
        return _tokenService.ReplaceTokens(emailTemplate, language);
    }
}
```

### Example 9: Custom Token Processing

```csharp
using ContentTokens.Services;

public class ReportGenerator
{
    private readonly IContentTokenService _tokenService;
    
    public ReportGenerator(IContentTokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public string GenerateReport()
    {
        var reportTemplate = @"
            # Monthly Report - {{ReportMonth}}
            
            ## Summary
            Company: {{CompanyName}}
            Period: {{ReportPeriod}}
            
            ## Key Metrics
            - Total Sales: {{TotalSales}}
            - New Customers: {{NewCustomers}}
            
            Contact: {{SupportEmail}}
        ";
        
        return _tokenService.ReplaceTokens(reportTemplate);
    }
}
```

### Example 10: Bulk Token Creation

```csharp
public async Task SeedDefaultTokensAsync()
{
    var defaultTokens = new[]
    {
        new ContentToken 
        { 
            Name = "CompanyName", 
            Value = "Acme Corporation",
            Description = "Company name"
        },
        new ContentToken 
        { 
            Name = "SupportEmail", 
            Value = "support@acme.com",
            Description = "Support email"
        },
        new ContentToken 
        { 
            Name = "Year", 
            Value = DateTime.Now.Year.ToString(),
            Description = "Current year"
        }
    };
    
    foreach (var token in defaultTokens)
    {
        _tokenService.SaveToken(token);
    }
}
```

## Integration Advanced Usage

### Example 11: Using with Optimizely Content

```csharp
using EPiServer.Core;
using ContentTokens.Services;

public class PageViewModel
{
    private readonly IContentTokenService _tokenService;
    
    public PageViewModel(IContentTokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    public string ProcessPageContent(PageData page)
    {
        var content = page.Property["MainBody"]?.ToString();
        if (string.IsNullOrEmpty(content))
            return content;
            
        // Get current language
        var language = page.LanguageBranch;
        
        // Replace tokens
        return _tokenService.ReplaceTokens(content, language);
    }
}
```

### Example 12: API Integration with JavaScript

```javascript
// Token management UI
class TokenManager {
  constructor() {
    this.apiUrl = '/api/contenttokens';
  }
  
  async loadTokens() {
    const response = await fetch(this.apiUrl);
    const tokens = await response.json();
    this.displayTokens(tokens);
  }
  
  async createToken(name, value, language = null) {
    const token = {
      name: name,
      value: value,
      languageCode: language
    };
    
    const response = await fetch(this.apiUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(token)
    });
    
    if (response.ok) {
      await this.loadTokens();
      this.showSuccess('Token created successfully');
    }
  }
  
  async deleteToken(id) {
    if (!confirm('Are you sure?')) return;
    
    await fetch(`${this.apiUrl}/${id}`, {
      method: 'DELETE'
    });
    
    await this.loadTokens();
  }
  
  displayTokens(tokens) {
    const container = document.getElementById('token-list');
    container.innerHTML = tokens.map(token => `
      <div class="token-item">
        <strong>{{${token.name}}}</strong>
        <span>${token.value}</span>
        <button onclick="tokenManager.deleteToken('${token.id}')">Delete</button>
      </div>
    `).join('');
  }
}

// Initialize
const tokenManager = new TokenManager();
tokenManager.loadTokens();
```

## Best Practices

### Token Naming Conventions

```bash
# Good examples
{{CompanyName}}
{{SupportEmail}}
{{CurrentYear}}
{{WelcomeMessageEN}}

# Bad examples
{{company name}}        # Contains space
{{Support-Email}}       # Contains hyphen
{{current_year}}        # Contains underscore
{{Welcome Message!}}    # Contains space and special char
```

### Organizing Tokens

```bash
# Group by category using prefixes
{{Contact_Email}}
{{Contact_Phone}}
{{Contact_Address}}

{{Legal_Copyright}}
{{Legal_Terms}}
{{Legal_Privacy}}

{{Promo_Banner}}
{{Promo_Discount}}
{{Promo_Code}}
```

### Version Control

Keep a JSON file of tokens for deployment:

```json
{
  "tokens": [
    {
      "name": "CompanyName",
      "value": "Acme Corporation",
      "description": "Company name"
    },
    {
      "name": "SupportEmail",
      "value": "support@acme.com",
      "description": "Support email"
    }
  ]
}
```

## Next Steps

- [API Reference](api-reference.md) - Complete API documentation
- [Configuration Guide](configuration.md) - Advanced configuration
- [Troubleshooting](troubleshooting.md) - Common issues and solutions
