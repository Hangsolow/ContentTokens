using ContentTokens.Models;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ContentTokens.Services
{
    /// <summary>
    /// Service for managing and replacing content tokens.
    /// </summary>
    public interface IContentTokenService
    {
        /// <summary>
        /// Gets all tokens, optionally filtered by language.
        /// </summary>
        IEnumerable<ContentToken> GetAllTokens(string? languageCode = null);

        /// <summary>
        /// Gets a specific token by name and language.
        /// </summary>
        ContentToken? GetToken(string name, string? languageCode = null);

        /// <summary>
        /// Saves a token (creates or updates).
        /// </summary>
        void SaveToken(ContentToken token);

        /// <summary>
        /// Deletes a token by ID.
        /// </summary>
        void DeleteToken(Guid id);

        /// <summary>
        /// Replaces all tokens in the given text with their values.
        /// </summary>
        string ReplaceTokens(string text, string? languageCode = null);
    }

    /// <summary>
    /// Implementation of the content token service.
    /// </summary>
    public class ContentTokenService : IContentTokenService
    {
        private static readonly Regex TokenRegex = new Regex(@"\{\{(\w+)\}\}", RegexOptions.Compiled);
        private readonly DynamicDataStore _store;

        public ContentTokenService()
        {
            _store = DynamicDataStoreFactory.Instance.GetStore(typeof(ContentToken)) 
                ?? DynamicDataStoreFactory.Instance.CreateStore(typeof(ContentToken));
        }

        public IEnumerable<ContentToken> GetAllTokens(string? languageCode = null)
        {
            var allTokens = _store.Items<ContentToken>();

            if (string.IsNullOrEmpty(languageCode))
            {
                return allTokens.OrderBy(t => t.Name).ThenBy(t => t.LanguageCode);
            }

            // Return language-specific tokens and fallback to language-neutral tokens
            return allTokens
                .Where(t => string.IsNullOrEmpty(t.LanguageCode) || t.LanguageCode == languageCode)
                .OrderBy(t => t.Name)
                .ThenByDescending(t => t.LanguageCode); // Language-specific tokens first
        }

        public ContentToken? GetToken(string name, string? languageCode = null)
        {
            var tokens = _store.Items<ContentToken>()
                .Where(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(languageCode))
            {
                // Try to get language-specific token first
                var languageSpecificToken = tokens.FirstOrDefault(t => t.LanguageCode == languageCode);
                if (languageSpecificToken != null)
                    return languageSpecificToken;
            }

            // Fallback to language-neutral token
            return tokens.FirstOrDefault(t => string.IsNullOrEmpty(t.LanguageCode));
        }

        public void SaveToken(ContentToken token)
        {
            var now = DateTime.UtcNow;

            if (token.Id == null || token.Id == Identity.NewIdentity)
            {
                token.Created = now;
                token.Modified = now;
                _store.Save(token);
            }
            else
            {
                token.Modified = now;
                _store.Save(token);
            }
        }

        public void DeleteToken(Guid id)
        {
            var identity = Identity.Create(id);
            _store.Delete(identity);
        }

        public string ReplaceTokens(string text, string? languageCode = null)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return TokenRegex.Replace(text, match =>
            {
                var tokenName = match.Groups[1].Value;
                var token = GetToken(tokenName, languageCode);
                return token?.Value ?? match.Value; // If token not found, keep original {{TokenName}}
            });
        }
    }
}
