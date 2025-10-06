using EPiServer.Shell;
using Microsoft.AspNetCore.Identity;

namespace ContentTokens.CmsExample
{
    /// <summary>
    /// Extends the built-in ApplicationUser for Optimizely CMS
    /// </summary>
    [UIDescriptorRegistration]
    public class ApplicationUser : IdentityUser
    {
    }
}
