/**
 * ContentTokens Dojo Autocomplete Widget
 * 
 * This Dojo widget provides autocomplete functionality for Content Tokens in plain string fields
 * within Optimizely CMS 12. When an editor types '{{' in a text field, a dropdown appears
 * showing available tokens fetched from the Content Tokens API.
 * 
 * Features:
 * - Autocomplete dropdown triggered by typing '{{'
 * - Fetches tokens from /api/contenttokens endpoint
 * - Inserts {{TokenName}} at cursor position
 * - Supports multiple tokens in a single field
 * - Visual highlighting of tokens with CSS
 * - Keyboard navigation (arrow keys, Enter, Escape)
 * 
 * Usage:
 * This widget is automatically applied to string properties via ContentTokenStringEditorDescriptor.cs
 * 
 * @module ContentTokens/ContentTokensAutocomplete
 */
define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/on",
    "dojo/dom-construct",
    "dojo/dom-class",
    "dojo/dom-style",
    "dojo/dom-geometry",
    "dojo/request/xhr",
    "dojo/keys",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/form/TextBox",
    "dijit/form/Textarea",
    "epi/shell/widget/_ValueRequiredMixin"
], function (
    declare,
    lang,
    on,
    domConstruct,
    domClass,
    domStyle,
    domGeom,
    xhr,
    keys,
    _Widget,
    _TemplatedMixin,
    TextBox,
    Textarea,
    _ValueRequiredMixin
) {
    return declare([_Widget, _TemplatedMixin, _ValueRequiredMixin], {
        
        // Template for the widget
        templateString: '<div class="dijitInline epiContentTokensWrapper">' +
                       '<div data-dojo-attach-point="inputNode" class="epiContentTokensInput"></div>' +
                       '<div data-dojo-attach-point="dropdownNode" class="epiContentTokensDropdown" style="display:none;"></div>' +
                       '</div>',
        
        // Widget properties
        value: "",
        intermediateChanges: false,
        multiline: false,
        
        // Internal state
        _tokens: [],
        _selectedIndex: -1,
        _autocompleteActive: false,
        _inputWidget: null,
        
        /**
         * Widget initialization
         */
        postCreate: function () {
            this.inherited(arguments);
            
            // Create the appropriate input widget (TextBox or Textarea)
            var InputWidgetType = this.multiline ? Textarea : TextBox;
            this._inputWidget = new InputWidgetType({
                value: this.value || "",
                intermediateChanges: true,
                style: "width: 100%;"
            });
            
            // Place the input widget
            this._inputWidget.placeAt(this.inputNode);
            this._inputWidget.startup();
            
            // Set up event handlers
            this._setupEventHandlers();
            
            // Load available tokens from API
            this._loadTokens();
        },
        
        /**
         * Set up event handlers for the input field
         */
        _setupEventHandlers: function () {
            var self = this;
            
            // Handle input changes
            this.own(
                on(this._inputWidget, "change", lang.hitch(this, function (newValue) {
                    this._setValue(newValue);
                }))
            );
            
            // Handle keyup for autocomplete trigger
            this.own(
                on(this._inputWidget.domNode, "keyup", lang.hitch(this, function (evt) {
                    this._handleKeyUp(evt);
                }))
            );
            
            // Handle keydown for navigation
            this.own(
                on(this._inputWidget.domNode, "keydown", lang.hitch(this, function (evt) {
                    if (this._autocompleteActive) {
                        this._handleKeyDown(evt);
                    }
                }))
            );
            
            // Close dropdown when clicking outside
            this.own(
                on(document, "click", lang.hitch(this, function (evt) {
                    if (!this.domNode.contains(evt.target)) {
                        this._hideDropdown();
                    }
                }))
            );
        },
        
        /**
         * Load available tokens from the API
         */
        _loadTokens: function () {
            var self = this;
            
            xhr("/api/contenttokens", {
                handleAs: "json",
                headers: {
                    "Content-Type": "application/json"
                }
            }).then(
                function (data) {
                    self._tokens = data || [];
                    console.log("Loaded " + self._tokens.length + " content tokens");
                },
                function (error) {
                    console.error("Failed to load content tokens:", error);
                    self._tokens = [];
                }
            );
        },
        
        /**
         * Handle keyup events to trigger autocomplete
         */
        _handleKeyUp: function (evt) {
            var inputElement = this._inputWidget.textbox || this._inputWidget.domNode;
            var value = inputElement.value;
            var cursorPos = inputElement.selectionStart;
            
            // Check if user typed '{{' before cursor
            if (cursorPos >= 2) {
                var textBeforeCursor = value.substring(0, cursorPos);
                var lastTwoChars = textBeforeCursor.slice(-2);
                
                if (lastTwoChars === "{{") {
                    this._showAutocomplete(cursorPos);
                    return;
                }
            }
            
            // Hide dropdown if not in token context
            if (this._autocompleteActive) {
                var textBeforeCursor = value.substring(0, cursorPos);
                var lastOpenBrace = textBeforeCursor.lastIndexOf("{{");
                
                if (lastOpenBrace === -1) {
                    this._hideDropdown();
                }
            }
        },
        
        /**
         * Handle keydown events for dropdown navigation
         */
        _handleKeyDown: function (evt) {
            if (!this._autocompleteActive) {
                return;
            }
            
            switch (evt.keyCode) {
                case keys.DOWN_ARROW:
                    evt.preventDefault();
                    this._selectNext();
                    break;
                    
                case keys.UP_ARROW:
                    evt.preventDefault();
                    this._selectPrevious();
                    break;
                    
                case keys.ENTER:
                    evt.preventDefault();
                    if (this._selectedIndex >= 0) {
                        this._insertToken(this._tokens[this._selectedIndex]);
                    }
                    break;
                    
                case keys.ESCAPE:
                    evt.preventDefault();
                    this._hideDropdown();
                    break;
            }
        },
        
        /**
         * Show autocomplete dropdown with available tokens
         */
        _showAutocomplete: function (cursorPos) {
            if (this._tokens.length === 0) {
                return;
            }
            
            // Clear existing dropdown content
            domConstruct.empty(this.dropdownNode);
            
            // Create dropdown items
            this._tokens.forEach(lang.hitch(this, function (token, index) {
                var itemNode = domConstruct.create("div", {
                    className: "epiContentTokensDropdownItem",
                    innerHTML: '<strong>' + token.name + '</strong>' +
                              (token.description ? '<br><span class="epiContentTokensDescription">' + 
                              token.description + '</span>' : '')
                }, this.dropdownNode);
                
                // Handle item click
                this.own(
                    on(itemNode, "click", lang.hitch(this, function (evt) {
                        evt.stopPropagation();
                        this._insertToken(token);
                    }))
                );
                
                // Handle item hover
                this.own(
                    on(itemNode, "mouseenter", lang.hitch(this, function () {
                        this._selectIndex(index);
                    }))
                );
            }));
            
            // Position and show dropdown
            this._positionDropdown();
            domStyle.set(this.dropdownNode, "display", "block");
            this._autocompleteActive = true;
            this._selectedIndex = 0;
            this._updateSelection();
        },
        
        /**
         * Position the dropdown relative to the cursor
         */
        _positionDropdown: function () {
            var inputPos = domGeom.position(this._inputWidget.domNode, true);
            
            domStyle.set(this.dropdownNode, {
                "top": (inputPos.h) + "px",
                "left": "0px",
                "width": inputPos.w + "px"
            });
        },
        
        /**
         * Hide the autocomplete dropdown
         */
        _hideDropdown: function () {
            domStyle.set(this.dropdownNode, "display", "none");
            this._autocompleteActive = false;
            this._selectedIndex = -1;
        },
        
        /**
         * Select the next item in the dropdown
         */
        _selectNext: function () {
            if (this._selectedIndex < this._tokens.length - 1) {
                this._selectedIndex++;
                this._updateSelection();
            }
        },
        
        /**
         * Select the previous item in the dropdown
         */
        _selectPrevious: function () {
            if (this._selectedIndex > 0) {
                this._selectedIndex--;
                this._updateSelection();
            }
        },
        
        /**
         * Select a specific index
         */
        _selectIndex: function (index) {
            this._selectedIndex = index;
            this._updateSelection();
        },
        
        /**
         * Update the visual selection in the dropdown
         */
        _updateSelection: function () {
            var items = this.dropdownNode.children;
            
            for (var i = 0; i < items.length; i++) {
                if (i === this._selectedIndex) {
                    domClass.add(items[i], "epiContentTokensDropdownItemSelected");
                } else {
                    domClass.remove(items[i], "epiContentTokensDropdownItemSelected");
                }
            }
        },
        
        /**
         * Insert the selected token at the cursor position
         */
        _insertToken: function (token) {
            var inputElement = this._inputWidget.textbox || this._inputWidget.domNode;
            var value = inputElement.value;
            var cursorPos = inputElement.selectionStart;
            
            // Find the position of '{{' before cursor
            var textBeforeCursor = value.substring(0, cursorPos);
            var lastOpenBrace = textBeforeCursor.lastIndexOf("{{");
            
            if (lastOpenBrace !== -1) {
                // Insert the token name and closing braces
                var tokenText = token.name + "}}";
                var newValue = value.substring(0, lastOpenBrace + 2) + 
                              tokenText + 
                              value.substring(cursorPos);
                
                // Update the input value
                this._inputWidget.set("value", newValue);
                this._setValue(newValue);
                
                // Set cursor position after the inserted token
                var newCursorPos = lastOpenBrace + 2 + tokenText.length;
                setTimeout(function () {
                    inputElement.setSelectionRange(newCursorPos, newCursorPos);
                    inputElement.focus();
                }, 0);
            }
            
            // Hide the dropdown
            this._hideDropdown();
        },
        
        /**
         * Set the widget value and trigger change event
         */
        _setValue: function (newValue) {
            this.value = newValue;
            this.onChange(newValue);
        },
        
        /**
         * Get the current widget value
         */
        _getValueAttr: function () {
            return this.value;
        },
        
        /**
         * Set the widget value programmatically
         */
        _setValueAttr: function (value) {
            this.value = value;
            if (this._inputWidget) {
                this._inputWidget.set("value", value);
            }
        },
        
        /**
         * Called when the value changes
         */
        onChange: function (value) {
            // Override in instances
        },
        
        /**
         * Widget cleanup
         */
        destroy: function () {
            if (this._inputWidget) {
                this._inputWidget.destroy();
            }
            this.inherited(arguments);
        }
    });
});
