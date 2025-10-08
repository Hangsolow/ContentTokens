# ContentTokens Tests

This directory contains tests for the ContentTokens library.

## Test Projects

### ContentTokens.Tests

Unit and integration tests for the ContentTokens library.

## Running Tests

To run all tests:

```bash
dotnet test
```

To run tests with detailed output:

```bash
dotnet test --verbosity detailed
```

## Test Organization

### Controllers Tests

Unit tests for the ContentTokensController API endpoints using mocked dependencies.

- `ContentTokensControllerTests.cs` - Tests for all REST API endpoints
  - GetAll - Returns all tokens
  - Get - Returns specific token or NotFound
  - Save - Creates/updates tokens with validation
  - Delete - Removes tokens
  - Preview - Tests token replacement

All controller tests use Moq to mock the IContentTokenService dependency.

### Services Tests

Integration tests for ContentTokenService that require a running Optimizely CMS environment.

- `ContentTokenServiceTests.cs` - Tests for token CRUD operations and replacement logic

**Note**: Service tests are marked as `Skip` because they require the full Optimizely CMS infrastructure with DynamicDataStore support. These tests are useful for integration testing in a real CMS environment.

## Test Results

Current test status:
- **7 passing** - All controller unit tests
- **11 skipped** - Service integration tests (require CMS environment)

## Dependencies

- xUnit - Testing framework
- FluentAssertions - Fluent assertion library
- Moq - Mocking framework for unit tests
- bUnit - Blazor component testing library

## Contributing

When adding new features:

1. Add unit tests for controllers using mocked dependencies
2. Add integration tests for services (mark with Skip attribute if they require CMS infrastructure)
3. Ensure all tests follow the naming convention: `MethodName_Condition_ExpectedResult`
4. Use FluentAssertions for readable assertions
