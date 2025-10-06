# ContentTokens Documentation

Welcome to the ContentTokens documentation. This addon for Optimizely CMS v12 provides dynamic placeholders - write `{{TokenName}}` in any text field and let editors define reusable, multilingual content tokens.

## Getting Started

- **[Getting Started Guide](getting-started.md)** - Step-by-step setup and basic usage
- **[Installation and Quick Start](../README.md#installation)** - Basic installation and setup

## Core Concepts

- **[Architecture](architecture.md)** - Understanding how ContentTokens works
- **[API Reference](api-reference.md)** - Complete API documentation
- **[TinyMCE Plugin](tinymce-plugin.md)** - Rich text editor integration with autocomplete
- **[Dojo Plugin](dojo-plugin.md)** - Plain text field integration with autocomplete

## Configuration and Customization

- **[Configuration](configuration.md)** - Advanced configuration options and patterns
- **[Advanced Usage](advanced-usage.md)** - Complex scenarios and advanced patterns
- **[TinyMCE Customization](tinymce-plugin.md#customization)** - Customize the editor plugin
- **[Dojo Customization](dojo-plugin.md#customization)** - Customize the string field autocomplete

## Development and Testing

- **[Testing](testing.md)** - Testing strategies, unit tests, and integration tests
- **[Troubleshooting](troubleshooting.md)** - Common issues and solutions

## Quick Navigation

### For New Users
1. Start with [Getting Started](getting-started.md)
2. Review [Architecture](architecture.md) to understand the concepts
3. Try [Basic Configuration](configuration.md#middleware-configuration)

### For Advanced Users
1. Explore [Advanced Usage](advanced-usage.md) patterns
2. Set up comprehensive [Testing](testing.md)
3. Optimize with [Configuration](configuration.md) options

### For Troubleshooting
1. Check [Troubleshooting](troubleshooting.md) for common issues
2. Review [API Reference](api-reference.md) for interface details
3. Enable debugging with [Configuration](configuration.md#logging-configuration)

## Key Features Overview

| Feature | Description | Documentation |
|---------|-------------|---------------|
| **Token Replacement** | Automatic `{{TokenName}}` replacement | [Architecture](architecture.md#token-replacement) |
| **Multilingual** | Language-specific token values | [Advanced Usage](advanced-usage.md#multilingual-examples) |
| **REST API** | Full CRUD API for tokens | [API Reference](api-reference.md) |
| **Admin Gadget** | CMS dashboard widget | [Getting Started](getting-started.md#create-your-first-token) |
| **TinyMCE Plugin** | Rich text editor integration | [TinyMCE Plugin](tinymce-plugin.md) |
| **Dojo Plugin** | Plain text field autocomplete | [Dojo Plugin](dojo-plugin.md) |
| **Autocomplete** | Type `{{` for token suggestions | [TinyMCE Plugin](tinymce-plugin.md#usage) / [Dojo Plugin](dojo-plugin.md#usage) |
| **Testing** | Comprehensive testing support | [Testing](testing.md) |

## What is ContentTokens?

ContentTokens allows editors to define reusable, multilingual content tokens that can be used throughout your Optimizely CMS site. Simply write `{{TokenName}}` in any text field, and the addon will automatically replace it with the configured value.

### Key Features

✅ **Easy to Use** - Simple `{{TokenName}}` syntax  
✅ **Multilingual** - Support for language-specific tokens  
✅ **Automatic Replacement** - Works seamlessly via service layer  
✅ **Admin UI** - Manage tokens via CMS dashboard gadget  
✅ **TinyMCE Integration** - Rich text editor plugin with autocomplete  
✅ **Visual Highlighting** - Token placeholders highlighted in editor  
✅ **REST API** - Programmatic access for integration  
✅ **Fallback Support** - Graceful handling of missing tokens

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
