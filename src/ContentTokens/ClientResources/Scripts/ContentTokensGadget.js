define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-construct",
    "dojo/dom-class",
    "dojo/request",
    "dijit/_WidgetBase",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "epi/shell/widget/_ModelBindingMixin"
],
function (
    declare,
    lang,
    array,
    on,
    domConstruct,
    domClass,
    request,
    _WidgetBase,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    _ModelBindingMixin
) {
    return declare([_WidgetBase, _TemplatedMixin, _WidgetsInTemplateMixin], {
        
        templateString: '<div class="epi-gadget">' +
            '<div class="epi-gadgetInner">' +
                '<div class="epi-gadgetHeader">' +
                    '<h2 class="epi-gadgetTitle">Content Tokens</h2>' +
                '</div>' +
                '<div class="epi-gadgetContent">' +
                    '<div style="margin-bottom: 15px;">' +
                        '<button data-dojo-attach-point="addButton" type="button" class="dijitButton">Add Token</button>' +
                    '</div>' +
                    '<div data-dojo-attach-point="tokenList" class="token-list"></div>' +
                    '<div data-dojo-attach-point="editorPanel" class="token-editor" style="display:none; margin-top: 20px; padding: 15px; border: 1px solid #ccc; background: #f9f9f9;">' +
                        '<h3 data-dojo-attach-point="editorTitle">Add/Edit Token</h3>' +
                        '<div style="margin-bottom: 10px;">' +
                            '<label>Token Name (e.g., CompanyName):</label><br/>' +
                            '<input data-dojo-attach-point="nameInput" type="text" style="width: 100%; padding: 5px;" />' +
                        '</div>' +
                        '<div style="margin-bottom: 10px;">' +
                            '<label>Value:</label><br/>' +
                            '<textarea data-dojo-attach-point="valueInput" rows="3" style="width: 100%; padding: 5px;"></textarea>' +
                        '</div>' +
                        '<div style="margin-bottom: 10px;">' +
                            '<label>Language Code (e.g., en, sv - leave empty for all languages):</label><br/>' +
                            '<input data-dojo-attach-point="languageInput" type="text" style="width: 100%; padding: 5px;" />' +
                        '</div>' +
                        '<div style="margin-bottom: 10px;">' +
                            '<label>Description:</label><br/>' +
                            '<input data-dojo-attach-point="descriptionInput" type="text" style="width: 100%; padding: 5px;" />' +
                        '</div>' +
                        '<div>' +
                            '<button data-dojo-attach-point="saveButton" type="button" class="dijitButton dijitButtonPrimary">Save</button>' +
                            '<button data-dojo-attach-point="cancelButton" type="button" class="dijitButton">Cancel</button>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +
        '</div>',

        currentToken: null,

        postCreate: function () {
            this.inherited(arguments);
            this.loadTokens();
            
            on(this.addButton, "click", lang.hitch(this, this.showEditor));
            on(this.saveButton, "click", lang.hitch(this, this.saveToken));
            on(this.cancelButton, "click", lang.hitch(this, this.hideEditor));
        },

        loadTokens: function () {
            var self = this;
            request.get("/api/contenttokens", {
                handleAs: "json"
            }).then(function (tokens) {
                self.renderTokenList(tokens);
            }, function (error) {
                console.error("Error loading tokens:", error);
            });
        },

        renderTokenList: function (tokens) {
            domConstruct.empty(this.tokenList);

            if (tokens.length === 0) {
                domConstruct.create("p", {
                    innerHTML: "No tokens defined yet. Click 'Add Token' to create one.",
                    style: "color: #666; font-style: italic;"
                }, this.tokenList);
                return;
            }

            var table = domConstruct.create("table", {
                style: "width: 100%; border-collapse: collapse;",
                class: "epi-default"
            }, this.tokenList);

            var thead = domConstruct.create("thead", {}, table);
            var headerRow = domConstruct.create("tr", {}, thead);
            
            ["Name", "Value", "Language", "Description", "Actions"].forEach(function (header) {
                domConstruct.create("th", {
                    innerHTML: header,
                    style: "text-align: left; padding: 8px; border-bottom: 2px solid #ccc;"
                }, headerRow);
            });

            var tbody = domConstruct.create("tbody", {}, table);

            array.forEach(tokens, function (token) {
                var row = domConstruct.create("tr", {
                    style: "border-bottom: 1px solid #eee;"
                }, tbody);

                domConstruct.create("td", {
                    innerHTML: "{{" + token.Name + "}}",
                    style: "padding: 8px; font-family: monospace; font-weight: bold;"
                }, row);

                domConstruct.create("td", {
                    innerHTML: token.Value || "",
                    style: "padding: 8px;"
                }, row);

                domConstruct.create("td", {
                    innerHTML: token.LanguageCode || "(all)",
                    style: "padding: 8px;"
                }, row);

                domConstruct.create("td", {
                    innerHTML: token.Description || "",
                    style: "padding: 8px;"
                }, row);

                var actionsCell = domConstruct.create("td", {
                    style: "padding: 8px;"
                }, row);

                var editBtn = domConstruct.create("button", {
                    innerHTML: "Edit",
                    class: "dijitButton",
                    style: "margin-right: 5px;"
                }, actionsCell);

                on(editBtn, "click", lang.hitch(this, function () {
                    this.editToken(token);
                }));

                var deleteBtn = domConstruct.create("button", {
                    innerHTML: "Delete",
                    class: "dijitButton"
                }, actionsCell);

                on(deleteBtn, "click", lang.hitch(this, function () {
                    if (confirm("Are you sure you want to delete the token '" + token.Name + "'?")) {
                        this.deleteToken(token.Id.toString());
                    }
                }));
            }, this);
        },

        showEditor: function () {
            this.currentToken = null;
            this.editorTitle.innerHTML = "Add Token";
            this.nameInput.value = "";
            this.valueInput.value = "";
            this.languageInput.value = "";
            this.descriptionInput.value = "";
            this.editorPanel.style.display = "block";
        },

        hideEditor: function () {
            this.editorPanel.style.display = "none";
            this.currentToken = null;
        },

        editToken: function (token) {
            this.currentToken = token;
            this.editorTitle.innerHTML = "Edit Token";
            this.nameInput.value = token.Name;
            this.valueInput.value = token.Value;
            this.languageInput.value = token.LanguageCode || "";
            this.descriptionInput.value = token.Description || "";
            this.editorPanel.style.display = "block";
        },

        saveToken: function () {
            var tokenData = {
                Name: this.nameInput.value.trim(),
                Value: this.valueInput.value,
                LanguageCode: this.languageInput.value.trim() || null,
                Description: this.descriptionInput.value.trim()
            };

            if (!tokenData.Name) {
                alert("Token name is required!");
                return;
            }

            if (this.currentToken) {
                tokenData.Id = this.currentToken.Id;
                tokenData.Created = this.currentToken.Created;
            }

            var self = this;
            request.post("/api/contenttokens", {
                data: JSON.stringify(tokenData),
                headers: {
                    "Content-Type": "application/json"
                },
                handleAs: "json"
            }).then(function () {
                self.hideEditor();
                self.loadTokens();
            }, function (error) {
                console.error("Error saving token:", error);
                alert("Error saving token. Check console for details.");
            });
        },

        deleteToken: function (tokenId) {
            var self = this;
            request.del("/api/contenttokens/" + tokenId, {
                handleAs: "json"
            }).then(function () {
                self.loadTokens();
            }, function (error) {
                console.error("Error deleting token:", error);
                alert("Error deleting token. Check console for details.");
            });
        }
    });
});
