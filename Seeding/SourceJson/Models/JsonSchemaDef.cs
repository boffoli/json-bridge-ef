using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models
{
    /// <summary>
    /// Represents a JSON schema definition.
    /// Centralizes the definition of the JSON type used in the mapping process.
    /// </summary>
    internal class JsonSchemaDef : ModelBase<JsonSchemaDef>
    {
        /// <summary>
        /// Constructor required by Entity Framework Core.
        /// </summary>
        public JsonSchemaDef() : base(null) { }

        /// <summary>
        /// Public constructor that accepts an optional validator.
        /// </summary>
        /// <param name="validator">The validator for this instance.</param>
        public JsonSchemaDef(IValidateAndFix<JsonSchemaDef>? validator)
            : base(validator) { }

        /// <summary>
        /// Unique identifier of the JSON schema.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Display name of the JSON schema.
        /// Example: "User Data V1".
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Technical identifier defining the JSON schema.
        /// Example: "UserDataV1".
        /// </summary>
        [Required]
        public string JsonSchemaIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// Additional description or notes about the JSON schema.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Collection of field definitions for this JSON schema.
        /// 1:N relationship with <see cref="JsonFieldDef"/>.
        /// </summary>
        public virtual ICollection<JsonFieldDef> JsonFieldDefs { get; set; } = [];

        /// <summary>
        /// Collection of independent blocks associated with this JSON schema.
        /// 1:N relationship with <see cref="JsonIndepBlockInfo"/>.
        /// </summary>
        public virtual ICollection<JsonIndepBlockInfo> JsonIndepBlockInfos { get; set; } = [];
    }
}