using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace ContentTokens.EditorDescriptors
{
    /// <summary>
    /// Editor descriptor that applies the ContentTokens Dojo autocomplete widget
    /// to string and LongString properties in Optimizely CMS 12.
    /// 
    /// This descriptor:
    /// - Registers the Dojo-based autocomplete widget for plain text fields
    /// - Enables token insertion with '{{' trigger in edit mode
    /// - Provides autocomplete suggestions from the Content Tokens API
    /// - Works with both single-line (string) and multi-line (LongString) properties
    /// 
    /// Usage:
    /// Apply to specific properties using [UIHint("ContentTokenString")] attribute
    /// or let it auto-apply to all string properties by modifying the EditorDescriptorInitialization.
    /// 
    /// Example property decoration:
    /// [UIHint("ContentTokenString")]
    /// public virtual string Tagline { get; set; }
    /// </summary>
    [EditorDescriptorRegistration(
        TargetType = typeof(string),
        UIHint = "ContentTokenString",
        EditorDescriptorBehavior = EditorDescriptorBehavior.PlaceLast)]
    public class ContentTokenStringEditorDescriptor : EditorDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the ContentTokenStringEditorDescriptor class.
        /// </summary>
        public ContentTokenStringEditorDescriptor()
        {
            // Set the client-side widget type to our custom Dojo module
            // This path is relative to the module's ClientResources/Scripts folder
            ClientEditingClass = "contentTokens/ContentTokensAutocomplete";
        }

        /// <summary>
        /// Configures the editor descriptor with custom settings.
        /// </summary>
        /// <param name="metadata">Metadata for the property being edited</param>
        /// <param name="attributes">Attributes from the property</param>
        public override void ModifyMetadata(
            ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            // Determine if this is a multi-line field (LongString)
            var isMultiline = metadata.ModelType == typeof(string) && 
                             metadata.PropertyName != null && 
                             (metadata.PropertyName.Contains("Description") || 
                              metadata.PropertyName.Contains("Text") ||
                              metadata.PropertyName.Contains("Content"));

            // Add editor settings
            metadata.EditorConfiguration["multiline"] = isMultiline;
            
            // Optional: Add custom CSS class for styling
            metadata.EditorConfiguration["cssClass"] = "epiContentTokensEditor";
            
            // Optional: Configure token API endpoint (default is /api/contenttokens)
            // metadata.EditorConfiguration["tokenApiEndpoint"] = "/api/contenttokens";
        }
    }

    /// <summary>
    /// Editor descriptor for explicitly multi-line text fields with Content Token support.
    /// Use [UIHint("ContentTokenLongString")] for TextArea fields that should have token autocomplete.
    /// </summary>
    [EditorDescriptorRegistration(
        TargetType = typeof(string),
        UIHint = "ContentTokenLongString",
        EditorDescriptorBehavior = EditorDescriptorBehavior.PlaceLast)]
    public class ContentTokenLongStringEditorDescriptor : EditorDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the ContentTokenLongStringEditorDescriptor class.
        /// </summary>
        public ContentTokenLongStringEditorDescriptor()
        {
            // Use the same Dojo widget as ContentTokenStringEditorDescriptor
            ClientEditingClass = "contentTokens/ContentTokensAutocomplete";
        }

        /// <summary>
        /// Configures the editor descriptor for multi-line text input.
        /// </summary>
        /// <param name="metadata">Metadata for the property being edited</param>
        /// <param name="attributes">Attributes from the property</param>
        public override void ModifyMetadata(
            ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            // Force multi-line mode
            metadata.EditorConfiguration["multiline"] = true;
            
            // Optional: Add custom CSS class for styling
            metadata.EditorConfiguration["cssClass"] = "epiContentTokensEditor epiContentTokensEditorMultiline";
        }
    }
}
