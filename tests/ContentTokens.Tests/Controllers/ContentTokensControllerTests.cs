using ContentTokens.Controllers;
using ContentTokens.Models;
using ContentTokens.Services;
using EPiServer.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ContentTokens.Tests.Controllers
{
    /// <summary>
    /// Unit tests for ContentTokensController.
    /// Tests all API endpoints for managing content tokens.
    /// </summary>
    public class ContentTokensControllerTests
    {
        private readonly Mock<IContentTokenService> _mockTokenService;
        private readonly ContentTokensController _controller;

        public ContentTokensControllerTests()
        {
            _mockTokenService = new Mock<IContentTokenService>();
            _controller = new ContentTokensController(_mockTokenService.Object);
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
            _mockTokenService.Setup(s => s.GetAllTokens(null)).Returns(tokens);

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
            _mockTokenService.Setup(s => s.GetToken("Test", It.IsAny<string?>())).Returns(token);

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
            _mockTokenService.Setup(s => s.GetToken("NonExistent", It.IsAny<string?>())).Returns((ContentToken?)null);

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
            _mockTokenService.Verify(s => s.SaveToken(token), Times.Once);
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
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().NotBeNull();
        }

        [Fact]
        public void Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var tokenId = Guid.NewGuid();

            // Act
            var result = _controller.Delete(tokenId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockTokenService.Verify(s => s.DeleteToken(tokenId), Times.Once);
        }

        [Fact]
        public void Preview_WithValidRequest_ReturnsReplacedText()
        {
            // Arrange
            var request = new PreviewRequest 
            { 
                Text = "Hello {{Name}}", 
                LanguageCode = "en" 
            };
            _mockTokenService.Setup(s => s.ReplaceTokens(request.Text, request.LanguageCode))
                .Returns("Hello World");

            // Act
            var result = _controller.Preview(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
        }
    }
}
