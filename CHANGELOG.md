# Changelog

All notable changes to the ContentTokens project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-06

### Added
- Initial release of ContentTokens addon for Optimizely CMS v12
- Token storage model with Dynamic Data Store support
- Multilingual token support with language fallback
- HTTP middleware for automatic token replacement in HTML responses
- REST API for token management (CRUD operations)
- Admin gadget for managing tokens in Optimizely CMS interface
- Token preview endpoint for testing replacements
- Example ASP.NET Core project demonstrating usage
- Comprehensive documentation (README, ARCHITECTURE, CONTRIBUTING)
- MIT License

### Features
- Define reusable content tokens with `{{TokenName}}` syntax
- Create, read, update, and delete tokens via REST API
- Automatic token replacement in rendered HTML content
- Language-specific token values with fallback to neutral tokens
- Admin UI gadget built with Dojo toolkit
- Support for token descriptions and metadata
- Timestamp tracking for token creation and modification

### Technical Details
- Built for .NET 8.0
- Compatible with Optimizely CMS 12.x
- Uses Dynamic Data Store for token persistence
- Compiled regex for performance
- ASP.NET Core middleware for token processing
- RESTful API with JSON responses

## [Unreleased]

### Planned Features
- Distributed caching support (Redis)
- Bulk import/export of tokens
- Token usage analytics
- Token version history
- Token categories/grouping
- Fine-grained permission system
- Auto-complete for token names in editor
- Validation warnings for missing tokens
- Token preview in page editor
- Unit and integration test suite

---

## Version History

- **1.0.0** - Initial release with core functionality
