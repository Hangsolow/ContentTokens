/**
 * ContentTokens TinyMCE Initializer
 * 
 * This script initializes the ContentTokens plugin for TinyMCE in Optimizely CMS 12.
 * It hooks into the TinyMCE setup to add the plugin and configure toolbar buttons.
 */

define([
    "dojo/_base/declare",
    "epi-cms/contentediting/command/_ChangeContentStatus",
    "epi/dependency"
], function (
    declare,
    _ChangeContentStatus,
    dependency
) {
    return declare(null, {
        initialize: function () {
            // Hook into TinyMCE initialization
            if (window.tinymce) {
                this._registerPlugin();
            } else {
                // Wait for TinyMCE to load
                var checkTinyMCE = setInterval(function () {
                    if (window.tinymce) {
                        clearInterval(checkTinyMCE);
                        this._registerPlugin();
                    }
                }.bind(this), 100);
            }
        },

        _registerPlugin: function () {
            // The plugin is registered via the external script file
            // This function can be used for additional configuration if needed
            console.log("ContentTokens TinyMCE plugin initialized");
        }
    });
});
