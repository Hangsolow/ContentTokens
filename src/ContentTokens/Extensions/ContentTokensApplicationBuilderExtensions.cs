using ContentTokens.Filters;
using Microsoft.AspNetCore.Builder;

namespace ContentTokens.Extensions
{
    /// <summary>
    /// Extension methods for configuring ContentTokens middleware.
    /// </summary>
    public static class ContentTokensApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the ContentTokens middleware to the application pipeline.
        /// This middleware will replace {{TokenName}} placeholders in HTML responses.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseContentTokens(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ContentTokenReplacementMiddleware>();
        }
    }
}
