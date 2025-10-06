using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System.Globalization;

namespace ContentTokens.Models
{
    /// <summary>
    /// Represents a reusable content token that can be used in text fields across the site.
    /// </summary>
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ContentToken : IDynamicData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the token.
        /// </summary>
        public Identity Id { get; set; } = Identity.NewIdentity;

        /// <summary>
        /// Gets or sets the token name (e.g., "CompanyName", "SupportEmail").
        /// This is what will be written as {{TokenName}} in content.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token value/content.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the language code for this token (e.g., "en", "sv", "de").
        /// Null or empty means it applies to all languages.
        /// </summary>
        public string? LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the description of what this token represents.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets when this token was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets when this token was last modified.
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
