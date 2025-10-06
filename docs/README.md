# ContentTokens Documentation

Welcome to the ContentTokens documentation! This addon provides dynamic placeholders for Optimizely CMS v12.

## Quick Navigation

### Getting Started
- **[Getting Started Guide](getting-started.md)** - Start here if you're new to ContentTokens
- **[Installation Guide](installation.md)** - Detailed installation instructions
- **[Configuration Guide](configuration.md)** - Configure ContentTokens for your needs

### Reference
- **[API Reference](api-reference.md)** - Complete REST API documentation
- **[Usage Examples](examples.md)** - Practical examples and patterns
- **[Troubleshooting](troubleshooting.md)** - Common issues and solutions

## What is ContentTokens?

ContentTokens allows editors to define reusable, multilingual content tokens that can be used throughout your Optimizely CMS site. Simply write `{{TokenName}}` in any text field, and the addon will automatically replace it with the configured value.

### Key Features

✅ **Easy to Use** - Simple `{{TokenName}}` syntax  
✅ **Multilingual** - Support for language-specific tokens  
✅ **Automatic Replacement** - Works seamlessly with HTML responses  
✅ **Admin UI** - Manage tokens via CMS dashboard gadget  
✅ **REST API** - Programmatic access for integration  
✅ **Fallback Support** - Graceful handling of missing tokens  

## Quick Start

### 1. Install

```bash
dotnet add package ContentTokens
```

### 2. Configure

```csharp
app.UseContentTokens();
```

### 3. Create Token

```bash
curl -X POST /api/contenttokens -H "Content-Type: application/json" -d '{
  "name": "CompanyName",
  "value": "Acme Corporation"
}'
```

### 4. Use in Content

```html
<h1>Welcome to {{CompanyName}}!</h1>
```

## Documentation Structure

### For New Users

1. Read the [Getting Started Guide](getting-started.md)
2. Follow the [Installation Guide](installation.md)
3. Review [Usage Examples](examples.md)

### For Developers

1. Review the [Configuration Guide](configuration.md)
2. Explore the [API Reference](api-reference.md)
3. Check [Usage Examples](examples.md) for integration patterns

### For Troubleshooting

1. Check the [Troubleshooting Guide](troubleshooting.md)
2. Search [GitHub Issues](https://github.com/Hangsolow/ContentTokens/issues)
3. Review the [Configuration Guide](configuration.md) for advanced setup

## Additional Resources

- **Main README**: [../README.md](../README.md)
- **Architecture Guide**: [../ARCHITECTURE.md](../ARCHITECTURE.md)
- **Contributing Guide**: [../CONTRIBUTING.md](../CONTRIBUTING.md)
- **Changelog**: [../CHANGELOG.md](../CHANGELOG.md)

## Common Use Cases

### Company Information
Store and reuse company details across your site:
```
{{CompanyName}}, {{CompanyAddress}}, {{CompanyPhone}}
```

### Legal Text
Keep legal text consistent:
```
{{CopyrightNotice}}, {{PrivacyPolicy}}, {{TermsOfService}}
```

### Marketing Campaigns
Update campaign text site-wide:
```
{{CurrentPromotion}}, {{SpecialOffer}}, {{CallToAction}}
```

### Contact Information
Centralize contact details:
```
{{SupportEmail}}, {{SalesPhone}}, {{CustomerServiceHours}}
```

## Support

- **Documentation**: You're reading it!
- **Issues**: [GitHub Issues](https://github.com/Hangsolow/ContentTokens/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Hangsolow/ContentTokens/discussions)

## Version

This documentation is for **ContentTokens v1.0.0**

## License

ContentTokens is licensed under the [MIT License](../LICENSE).

---

**Need help?** Start with the [Getting Started Guide](getting-started.md) or check the [Troubleshooting Guide](troubleshooting.md).
