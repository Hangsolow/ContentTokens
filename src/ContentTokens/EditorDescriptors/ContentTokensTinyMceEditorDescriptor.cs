using EPiServer.Cms.TinyMce.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace ContentTokens.EditorDescriptors
{
    /// <summary>
    /// Initialization module that registers the ContentTokens TinyMCE plugin
    /// with Optimizely CMS 12.
    ///
    /// This module:
    /// - Adds the ContentTokens external plugin to TinyMCE's default configuration
    /// - Appends the token picker button to the default toolbar
    ///
    /// The plugin enables token insertion directly from rich text fields in edit mode.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentTokensTinyMceInitializer : IInitializableModule
    {
        /// <summary>
        /// Registers the ContentTokens TinyMCE plugin with the default configuration.
        /// </summary>
        /// <remarks>
        /// IInitializableModule instances are created by Optimizely via Activator.CreateInstance
        /// (no DI), so dependencies must be resolved from the InitializationEngine instead.
        /// </remarks>
        public void Initialize(InitializationEngine context)
        {
            var tinyMceConfiguration = context.Locate.Advanced.GetRequiredService<TinyMceConfiguration>();
            tinyMceConfiguration.Default()
                .AddExternalPlugin(
                    "contentTokensPlugin",
                    "/EPiServer/ContentTokens/ClientResources/Scripts/ContentTokensTinyMcePlugin.js")
                .AppendToolbar("contentTokensPlugin", 0);
        }

        /// <summary>
        /// Uninitializes the module. No cleanup needed for TinyMCE plugin registration.
        /// </summary>
        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
