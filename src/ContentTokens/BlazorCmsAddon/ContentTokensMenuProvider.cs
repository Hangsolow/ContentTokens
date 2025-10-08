using EPiServer.Shell.Navigation;

namespace ContentTokens.BlazorCmsAddon
{
    /// <summary>
    /// Menu provider that adds the Content Tokens item to the CMS admin menu.
    /// </summary>
    [MenuProvider]
    public class ContentTokensMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var menuItem = new UrlMenuItem(
                "Content Tokens",
                "/global/cms/admin/contenttokens",
                "/contenttokens")
            {
                SortIndex = 100,
                IsAvailable = (context) => true
            };

            return new[] { menuItem };
        }
    }
}
