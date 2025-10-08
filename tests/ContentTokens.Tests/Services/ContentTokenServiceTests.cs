using ContentTokens.Models;
using ContentTokens.Services;
using FluentAssertions;
using Xunit;

namespace ContentTokens.Tests.Services
{
    /// <summary>
    /// Integration tests for ContentTokenService.
    /// These tests require a running Optimizely CMS environment with DynamicDataStore support.
    /// Note: These tests are skipped in CI/CD pipelines as they require the full CMS infrastructure.
    /// </summary>
    public class ContentTokenServiceTests : IDisposable
    {
        private readonly ContentTokenService _service;

        public ContentTokenServiceTests()
        {
            // Note: This will fail if DynamicDataStoreFactory is not initialized
            // These are integration tests that require a real Optimizely environment
            _service = new ContentTokenService();
        }

        public void Dispose()
        {
            // Clean up any tokens created during tests
            try
            {
                var tokens = _service.GetAllTokens().ToList();
                foreach (var token in tokens)
                {
                    if (token.Id != null)
                    {
                        _service.DeleteToken(token.Id.ExternalId);
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors in test environment
            }
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void SaveToken_WithValidToken_StoresSuccessfully()
        {
            // Arrange
            var token = new ContentToken
            {
                Name = "TestToken",
                Value = "Test Value",
                Description = "Test token for unit testing"
            };

            // Act
            _service.SaveToken(token);
            var retrieved = _service.GetToken("TestToken");

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("TestToken");
            retrieved.Value.Should().Be("Test Value");
            retrieved.Description.Should().Be("Test token for unit testing");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void GetToken_WithNonExistentToken_ReturnsNull()
        {
            // Arrange & Act
            var result = _service.GetToken("NonExistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void GetAllTokens_ReturnsAllStoredTokens()
        {
            // Arrange
            var token1 = new ContentToken { Name = "Token1", Value = "Value1" };
            var token2 = new ContentToken { Name = "Token2", Value = "Value2" };
            _service.SaveToken(token1);
            _service.SaveToken(token2);

            // Act
            var tokens = _service.GetAllTokens().ToList();

            // Assert
            tokens.Should().HaveCountGreaterOrEqualTo(2);
            tokens.Should().Contain(t => t.Name == "Token1");
            tokens.Should().Contain(t => t.Name == "Token2");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void SaveToken_WithExistingToken_UpdatesToken()
        {
            // Arrange
            var token = new ContentToken { Name = "UpdateTest", Value = "Original Value" };
            _service.SaveToken(token);
            var originalId = token.Id;

            // Act
            token.Value = "Updated Value";
            _service.SaveToken(token);
            var retrieved = _service.GetToken("UpdateTest");

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Value.Should().Be("Updated Value");
            retrieved.Id.Should().Be(originalId);
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void DeleteToken_WithExistingToken_RemovesSuccessfully()
        {
            // Arrange
            var token = new ContentToken { Name = "TempToken", Value = "Temporary" };
            _service.SaveToken(token);
            var tokenId = token.Id.ExternalId;

            // Act
            _service.DeleteToken(tokenId);
            var retrieved = _service.GetToken("TempToken");

            // Assert
            retrieved.Should().BeNull();
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void ReplaceTokens_WithExistingToken_ReplacesSuccessfully()
        {
            // Arrange
            var token = new ContentToken { Name = "CompanyName", Value = "Acme Corporation" };
            _service.SaveToken(token);
            var text = "Welcome to {{CompanyName}}!";

            // Act
            var result = _service.ReplaceTokens(text);

            // Assert
            result.Should().Be("Welcome to Acme Corporation!");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void ReplaceTokens_WithNonExistentToken_LeavesTokenIntact()
        {
            // Arrange
            var text = "Hello {{NonExistent}}!";

            // Act
            var result = _service.ReplaceTokens(text);

            // Assert
            result.Should().Be("Hello {{NonExistent}}!");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void ReplaceTokens_WithMultipleTokens_ReplacesAllTokens()
        {
            // Arrange
            var token1 = new ContentToken { Name = "Company", Value = "Acme Corp" };
            var token2 = new ContentToken { Name = "Year", Value = "2024" };
            _service.SaveToken(token1);
            _service.SaveToken(token2);
            var text = "Copyright {{Year}} {{Company}}";

            // Act
            var result = _service.ReplaceTokens(text);

            // Assert
            result.Should().Be("Copyright 2024 Acme Corp");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void GetToken_WithLanguageCode_ReturnsLanguageSpecificToken()
        {
            // Arrange
            var tokenEn = new ContentToken { Name = "Greeting", Value = "Hello", LanguageCode = "en" };
            var tokenSv = new ContentToken { Name = "Greeting", Value = "Hej", LanguageCode = "sv" };
            _service.SaveToken(tokenEn);
            _service.SaveToken(tokenSv);

            // Act
            var resultEn = _service.GetToken("Greeting", "en");
            var resultSv = _service.GetToken("Greeting", "sv");

            // Assert
            resultEn.Should().NotBeNull();
            resultEn!.Value.Should().Be("Hello");
            resultSv.Should().NotBeNull();
            resultSv!.Value.Should().Be("Hej");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void GetToken_WithLanguageCode_FallsBackToNeutralToken()
        {
            // Arrange
            var neutralToken = new ContentToken { Name = "Fallback", Value = "Neutral Value" };
            _service.SaveToken(neutralToken);

            // Act
            var result = _service.GetToken("Fallback", "de");

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be("Neutral Value");
        }

        [Fact(Skip = "Integration test - requires Optimizely CMS environment with DynamicDataStore")]
        public void SaveToken_WithAlphanumericName_Succeeds()
        {
            // Arrange
            var token = new ContentToken { Name = "Token123", Value = "Value" };

            // Act
            _service.SaveToken(token);
            var retrieved = _service.GetToken("Token123");

            // Assert
            retrieved.Should().NotBeNull();
        }
    }
}
