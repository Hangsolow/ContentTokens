using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ContentTokens.CmsExample.Models.Pages
{
    /// <summary>
    /// Used for standard pages on the site
    /// </summary>
    [ContentType(
        DisplayName = "Standard Page",
        GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4",
        Description = "A standard page with title and body content")]
    [ImageUrl("~/images/page-type-thumbnail-standard.png")]
    public class StandardPage : PageData
    {
        [Display(
            Name = "Title",
            Description = "The title of the page (supports tokens like {{CompanyName}})",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        public virtual string? Title { get; set; }

        [Display(
            Name = "Main body",
            Description = "The main body content (supports tokens like {{SupportEmail}}, {{PhoneNumber}})",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [CultureSpecific]
        public virtual XhtmlString? MainBody { get; set; }

        [Display(
            Name = "Main content area",
            Description = "Content area for additional blocks",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual ContentArea? MainContentArea { get; set; }
    }
}
