# Contributing to ContentTokens

Thank you for your interest in contributing to ContentTokens! This document provides guidelines and instructions for contributing.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or Rider
- Basic knowledge of Optimizely CMS
- Git

### Setting Up Development Environment

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/ContentTokens.git
   cd ContentTokens
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the example project:
   ```bash
   cd ContentTokens.Example
   dotnet run
   ```

## Development Workflow

### Branching Strategy

- `main` - Stable release branch
- `develop` - Development branch (if applicable)
- Feature branches: `feature/your-feature-name`
- Bug fixes: `fix/issue-description`

### Making Changes

1. Create a new branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes following the coding standards

3. Test your changes:
   ```bash
   dotnet build
   dotnet test  # If tests exist
   ```

4. Commit your changes:
   ```bash
   git add .
   git commit -m "Description of your changes"
   ```

5. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

6. Create a Pull Request

## Coding Standards

### C# Style Guide

- Follow Microsoft's C# Coding Conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and small
- Use LINQ where appropriate

Example:
```csharp
/// <summary>
/// Gets a token by name and optional language code.
/// </summary>
/// <param name="name">The token name</param>
/// <param name="languageCode">Optional language code</param>
/// <returns>The token if found, otherwise null</returns>
public ContentToken? GetToken(string name, string? languageCode = null)
{
    // Implementation
}
```

### JavaScript/Dojo Style Guide

- Use semicolons
- Use double quotes for strings
- Follow Dojo module pattern
- Add JSDoc comments for functions

Example:
```javascript
/**
 * Loads all tokens from the API
 */
loadTokens: function () {
    // Implementation
}
```

## Testing

### Unit Tests

- Write unit tests for new features
- Ensure existing tests pass
- Aim for good test coverage
- Use meaningful test names

Example:
```csharp
[Fact]
public void ReplaceTokens_ShouldReplaceSimpleToken()
{
    // Arrange
    var service = new ContentTokenService();
    var token = new ContentToken { Name = "Test", Value = "Value" };
    service.SaveToken(token);
    
    // Act
    var result = service.ReplaceTokens("Hello {{Test}}!");
    
    // Assert
    Assert.Equal("Hello Value!", result);
}
```

### Integration Tests

- Test end-to-end scenarios
- Use the example project for manual testing
- Verify token replacement in various contexts

## Pull Request Guidelines

### Before Submitting

- [ ] Code builds without errors or warnings
- [ ] All tests pass
- [ ] Code follows style guidelines
- [ ] Documentation is updated
- [ ] Commit messages are clear and descriptive

### PR Description Template

```markdown
## Description
Brief description of the changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## How Has This Been Tested?
Describe the tests you ran

## Checklist
- [ ] My code follows the style guidelines
- [ ] I have performed a self-review
- [ ] I have commented my code where needed
- [ ] I have updated the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix/feature works
```

## Documentation

### What to Document

- Public APIs and interfaces
- Configuration options
- Usage examples
- Architecture decisions

### Where to Document

- XML comments in code
- README.md for usage
- ARCHITECTURE.md for design decisions
- Inline comments for complex logic

## Reporting Issues

### Bug Reports

Include:
- Clear description of the bug
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment details (.NET version, Optimizely version)
- Screenshots if applicable

### Feature Requests

Include:
- Clear description of the feature
- Use case / motivation
- Proposed implementation (optional)
- Examples of similar features elsewhere

## Code Review Process

1. Maintainer reviews PR
2. Address feedback and comments
3. Make requested changes
4. Update PR with changes
5. Maintainer approves and merges

## Release Process

1. Update version in `.csproj`
2. Update CHANGELOG.md
3. Create release notes
4. Tag release
5. Build and publish NuGet package

## Community

### Communication Channels

- GitHub Issues - Bug reports and feature requests
- GitHub Discussions - General questions and discussions
- Pull Requests - Code contributions

### Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Help others learn and grow
- Follow the Golden Rule

## Recognition

Contributors will be recognized in:
- README.md contributors section
- Release notes
- Git commit history

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

If you have questions about contributing, please:
1. Check existing documentation
2. Search closed issues
3. Open a new issue with your question

Thank you for contributing to ContentTokens! 🎉
