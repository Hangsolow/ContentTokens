using Microsoft.AspNetCore.Builder;

namespace ContentTokens.Extensions
{
    /// <summary>
    /// Extension methods for configuring ContentTokens.
    /// </summary>
    public static class ContentTokensApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds ContentTokens configuration to the application.
        /// This method is provided for future extensibility but currently has no implementation.
        /// Token replacement is handled automatically by the service layer.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseContentTokens(this IApplicationBuilder app)
        {
            // Reserved for future middleware or initialization logic
            return app;
        }
    }
}
