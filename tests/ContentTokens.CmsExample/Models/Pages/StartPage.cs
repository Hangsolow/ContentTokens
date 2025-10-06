using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ContentTokens.CmsExample.Models.Pages
{
    /// <summary>
    /// Used for the start page of the site
    /// </summary>
    [ContentType(
        DisplayName = "Start Page",
        GUID = "19671657-B684-4D95-A61F-8DD4FE60D559",
        Description = "The start page for the site")]
    [ImageUrl("~/images/page-type-thumbnail-start.png")]
    public class StartPage : PageData
    {
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables. You can also use tokens like {{CompanyName}}.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        public virtual XhtmlString? MainBody { get; set; }

        [Display(
            Name = "Main content area",
            Description = "Content area for main content blocks",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual ContentArea? MainContentArea { get; set; }

        [Display(
            Name = "Heading",
            Description = "Page heading (supports tokens like {{CompanyName}})",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        [CultureSpecific]
        public virtual string? Heading { get; set; }
    }
}
