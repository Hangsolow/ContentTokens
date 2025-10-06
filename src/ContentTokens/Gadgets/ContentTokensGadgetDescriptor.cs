using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace ContentTokens.Gadgets
{
    /// <summary>
    /// Descriptor for the Content Tokens gadget.
    /// </summary>
    [ServiceConfiguration(typeof(IModuleDescriptor))]
    public class ContentTokensGadgetDescriptor : IModuleDescriptor
    {
        public string Name => "ContentTokens";

        public IEnumerable<string> Dependencies => new[] { "Shell", "CMS" };

        public string ModuleName => "ContentTokens";
    }

    /// <summary>
    /// Initialization module for registering the gadget.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Shell.UI.InitializationModule))]
    public class ContentTokensGadgetInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Gadget registration happens through module.config
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
