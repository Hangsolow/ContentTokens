using ContentTokens.Models;
using ContentTokens.Services;
using EPiServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentTokens.Controllers
{
    /// <summary>
    /// REST API controller for managing content tokens.
    /// </summary>
    [Authorize(Roles = "CmsAdmins,CmsEditors,WebAdmins")]
    [Route("api/contenttokens")]
    [ApiController]
    public class ContentTokensController : ControllerBase
    {
        private readonly IContentTokenService _tokenService;

        public ContentTokensController(IContentTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Gets all tokens, optionally filtered by language.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? languageCode = null)
        {
            var tokens = _tokenService.GetAllTokens(languageCode);
            return Ok(tokens);
        }

        /// <summary>
        /// Gets a specific token by name and language.
        /// </summary>
        [HttpGet("{name}")]
        public IActionResult Get(string name, [FromQuery] string? languageCode = null)
        {
            var token = _tokenService.GetToken(name, languageCode);
            if (token == null)
                return NotFound();

            return Ok(token);
        }

        /// <summary>
        /// Creates or updates a token.
        /// </summary>
        [HttpPost]
        public IActionResult Save([FromBody] ContentToken token)
        {
            if (string.IsNullOrWhiteSpace(token.Name))
                return BadRequest(new { error = "Token name is required" });

            _tokenService.SaveToken(token);
            return Ok(token);
        }

        /// <summary>
        /// Deletes a token by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _tokenService.DeleteToken(id);
            return NoContent();
        }

        /// <summary>
        /// Tests token replacement in a given text.
        /// </summary>
        [HttpPost("preview")]
        public IActionResult Preview([FromBody] PreviewRequest request)
        {
            var result = _tokenService.ReplaceTokens(request.Text, request.LanguageCode);
            return Ok(new { original = request.Text, replaced = result });
        }
    }

    public class PreviewRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? LanguageCode { get; set; }
    }
}
