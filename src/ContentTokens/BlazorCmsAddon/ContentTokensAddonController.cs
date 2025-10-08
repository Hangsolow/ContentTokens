using EPiServer.Framework.Web.Resources;
using EPiServer.Shell;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentTokens.BlazorCmsAddon
{
    /// <summary>
    /// Controller for the Content Tokens Blazor addon in the CMS admin interface.
    /// This controller serves the Blazor component within the CMS UI.
    /// </summary>
    [Authorize(Policy = "CmsAdmins,CmsEditors,WebAdmins")]
    public class ContentTokensAddonController : Controller
    {
        /// <summary>
        /// Main view for the Content Tokens addon.
        /// </summary>
        /// <returns>The view containing the Blazor component</returns>
        [Route("/contenttokens")]
        public IActionResult Index()
        {
            return View("/BlazorCmsAddon/Views/Index.cshtml");
        }
    }
}
