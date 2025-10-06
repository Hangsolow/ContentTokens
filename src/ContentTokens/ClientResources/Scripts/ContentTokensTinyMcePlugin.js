/**
 * ContentTokens TinyMCE Plugin for Optimizely CMS 12
 * 
 * This plugin enables editors to insert content token placeholders (e.g., {{TokenName}})
 * into Rich Text (XhtmlString) fields with autocomplete functionality.
 * 
 * Features:
 * - Autocomplete suggestions when typing {{
 * - Fetches available tokens from /api/contenttokens
 * - Highlights token placeholders with custom CSS class
 * - Easy token insertion into the editor
 */

(function () {
    'use strict';

    // Register the TinyMCE plugin
    tinymce.PluginManager.add('contenttokens', function (editor, url) {
        
        var autocompleter = null;
        var availableTokens = [];
        var isLoadingTokens = false;

        /**
         * Fetches available tokens from the API endpoint
         */
        function loadTokens() {
            if (isLoadingTokens) {
                return Promise.resolve(availableTokens);
            }

            isLoadingTokens = true;
            
            return fetch('/api/contenttokens')
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Failed to load tokens');
                    }
                    return response.json();
                })
                .then(function (tokens) {
                    availableTokens = tokens.map(function (token) {
                        return {
                            type: 'cardmenuitem',
                            value: token.Name,
                            label: token.Name,
                            meta: token.Description || 'No description',
                            text: '{{' + token.Name + '}}'
                        };
                    });
                    isLoadingTokens = false;
                    return availableTokens;
                })
                .catch(function (error) {
                    console.error('Error loading content tokens:', error);
                    isLoadingTokens = false;
                    return [];
                });
        }

        /**
         * Setup the autocompleter for {{ trigger
         */
        function setupAutocompleter() {
            editor.ui.registry.addAutocompleter('contenttokens', {
                ch: '{{',
                minChars: 0,
                columns: 1,
                fetch: function (pattern) {
                    return loadTokens().then(function (tokens) {
                        var matchedTokens = tokens.filter(function (token) {
                            return token.value.toLowerCase().indexOf(pattern.toLowerCase()) !== -1;
                        });
                        return matchedTokens;
                    });
                },
                onAction: function (autocompleteApi, rng, value) {
                    editor.selection.setRng(rng);
                    editor.insertContent(value + '}}');
                    autocompleteApi.hide();
                }
            });
        }

        /**
         * Adds a toolbar button for inserting tokens
         */
        function setupToolbarButton() {
            editor.ui.registry.addButton('contenttokens', {
                text: 'Token',
                icon: 'bookmark',
                tooltip: 'Insert Content Token',
                onAction: function () {
                    openTokenDialog();
                }
            });
        }

        /**
         * Opens a dialog to select and insert a token
         */
        function openTokenDialog() {
            loadTokens().then(function (tokens) {
                if (tokens.length === 0) {
                    editor.windowManager.alert('No content tokens available. Please create tokens first.');
                    return;
                }

                var dialogConfig = {
                    title: 'Insert Content Token',
                    body: {
                        type: 'panel',
                        items: [
                            {
                                type: 'selectbox',
                                name: 'token',
                                label: 'Select Token',
                                items: tokens.map(function (token) {
                                    return {
                                        text: token.value + ' - ' + token.meta,
                                        value: token.text
                                    };
                                })
                            }
                        ]
                    },
                    buttons: [
                        {
                            type: 'cancel',
                            text: 'Cancel'
                        },
                        {
                            type: 'submit',
                            text: 'Insert',
                            primary: true
                        }
                    ],
                    onSubmit: function (api) {
                        var data = api.getData();
                        if (data.token) {
                            editor.insertContent(data.token);
                        }
                        api.close();
                    }
                };

                editor.windowManager.open(dialogConfig);
            });
        }

        /**
         * Adds a menu item for inserting tokens
         */
        function setupMenuItem() {
            editor.ui.registry.addMenuItem('contenttokens', {
                text: 'Content Token',
                icon: 'bookmark',
                onAction: function () {
                    openTokenDialog();
                }
            });
        }

        /**
         * Applies custom styling to token placeholders in the editor
         */
        function highlightTokens() {
            var content = editor.getContent();
            var highlighted = content.replace(
                /\{\{(\w+)\}\}/g,
                '<span class="opt-content-token">{{$1}}</span>'
            );
            
            if (content !== highlighted) {
                editor.setContent(highlighted);
            }
        }

        /**
         * Removes highlighting before getting content (for clean output)
         */
        function removeHighlighting(e) {
            if (e && e.content) {
                e.content = e.content.replace(
                    /<span class="opt-content-token">({{.*?}})<\/span>/g,
                    '$1'
                );
            }
        }

        /**
         * Setup content processing
         */
        function setupContentProcessing() {
            // Highlight tokens when content is set
            editor.on('SetContent', function () {
                setTimeout(highlightTokens, 100);
            });

            // Remove highlighting before saving
            editor.on('GetContent', removeHighlighting);
            editor.on('BeforeSetContent', function (e) {
                if (e.content) {
                    // Ensure tokens are properly formatted
                    e.content = e.content.replace(
                        /<span class="opt-content-token">({{.*?}})<\/span>/g,
                        '$1'
                    );
                }
            });
        }

        /**
         * Add custom CSS for token highlighting
         */
        function setupStyles() {
            editor.on('init', function () {
                editor.dom.addStyle(
                    '.opt-content-token {' +
                    '  background-color: #e8f4fd;' +
                    '  border: 1px solid #4a90e2;' +
                    '  border-radius: 3px;' +
                    '  padding: 2px 4px;' +
                    '  font-family: "Courier New", monospace;' +
                    '  font-weight: bold;' +
                    '  color: #2d5f8d;' +
                    '  cursor: pointer;' +
                    '}'
                );
            });
        }

        // Initialize plugin features
        setupAutocompleter();
        setupToolbarButton();
        setupMenuItem();
        setupContentProcessing();
        setupStyles();

        // Load tokens on initialization
        loadTokens();

        // Return plugin metadata
        return {
            getMetadata: function () {
                return {
                    name: 'Content Tokens Plugin',
                    url: 'https://github.com/Hangsolow/ContentTokens'
                };
            }
        };
    });
})();
