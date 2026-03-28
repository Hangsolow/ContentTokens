using ContentTokens.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ContentTokens.Extensions
{
    /// <summary>
    /// Extension methods for configuring ContentTokens in the ASP.NET Core pipeline.
    /// </summary>
    public static class ContentTokensApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the ContentTokens middleware to the pipeline. It intercepts HTML responses
        /// and replaces <c>{{TokenName}}</c> placeholders with their configured values.
        /// Place this after <c>UseRouting()</c> and <c>UseAuthentication()</c>.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseContentTokens(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ContentTokensMiddleware>();
        }
    }
}
