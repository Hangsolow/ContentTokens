define([
    "dojo/_base/declare",
    "epi/_Module",
    "epi/dependency",
    "epi/routes"
], function (
    declare,
    _Module,
    dependency,
    routes
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            var viewSettingsStore = dependency.resolve("epi.storeregistry").get("epi.cms.viewsettings");

            // Register the gadget
            viewSettingsStore.registerGadget("ContentTokens/Scripts/ContentTokensGadget", {
                title: "Content Tokens",
                description: "Manage reusable content tokens for use across the site",
                plugInAreas: ["/episerver/cms/dashboard"]
            });
        }
    });
});
