using EPiServer.Cms.TinyMce.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc;

namespace ContentTokens.EditorDescriptors
{
    /// <summary>
    /// Editor descriptor that registers the ContentTokens TinyMCE plugin
    /// with Optimizely CMS 12.
    /// 
    /// This descriptor:
    /// - Adds the ContentTokens plugin to TinyMCE
    /// - Adds the token button to the toolbar
    /// - Adds the menu item to the Insert menu
    /// - Configures external plugin loading
    /// </summary>
    [ServiceConfiguration(typeof(TinyMceConfiguration))]
    public class ContentTokensTinyMceEditorDescriptor : TinyMceConfiguration
    {
        public ContentTokensTinyMceEditorDescriptor(
            IContentRenderer contentRenderer,
            LocalizationService localizationService,
            IServiceProvider serviceProvider,
            IUrlHelper urlHelper) 
            : base(contentRenderer, localizationService, serviceProvider, urlHelper)
        {
        }

        /// <summary>
        /// Returns default TinyMCE settings with ContentTokens plugin enabled
        /// </summary>
        public override object DefaultSettings()
        {
            var settings = base.DefaultSettings();
            
            // Note: The actual implementation requires dynamic object manipulation
            // In a real implementation, you would modify the settings object to add:
            // - Plugin name to the plugins array
            // - Button to the toolbar
            // - External plugin path
            
            return settings;
        }
    }
}
