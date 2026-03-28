using ContentTokens.Services;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace ContentTokens.Extensions
{
    /// <summary>
    /// Extension methods for registering ContentTokens services.
    /// </summary>
    public static class ContentTokensServiceCollectionExtensions
    {
        /// <summary>
        /// Registers ContentTokens services and the protected shell module with Optimizely CMS.
        /// Call this from <c>ConfigureServices</c> or <c>Program.cs</c> alongside <c>services.AddCms()</c>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddContentTokens(this IServiceCollection services)
        {
            services.AddSingleton<IContentTokenService, ContentTokenService>();

            services.Configure<ProtectedModuleOptions>(o =>
            {
                if (!o.Items.Any(i => i.Name.Equals("ContentTokens", StringComparison.OrdinalIgnoreCase)))
                {
                    o.Items.Add(new ModuleDetails { Name = "ContentTokens" });
                }
            });

            return services;
        }
    }
}
