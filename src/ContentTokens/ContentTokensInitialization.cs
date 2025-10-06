using ContentTokens.Services;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;

namespace ContentTokens
{
    /// <summary>
    /// Module for initializing the ContentTokens addon.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentTokensInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            // Register the content token service
            context.Services.AddSingleton<IContentTokenService, ContentTokenService>();
        }

        public void Initialize(InitializationEngine context)
        {
            // Any initialization logic can go here
        }

        public void Uninitialize(InitializationEngine context)
        {
            // Cleanup if needed
        }
    }
}
