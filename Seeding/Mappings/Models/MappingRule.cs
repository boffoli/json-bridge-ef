using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Validators;
using JsonBridgeEF.Common;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mappings.Models
{
    /// <summary>
    /// Defines the mapping rule between a JSON source field and a target .NET model property.
    /// The rule links a JSON field (via JsonFieldinition) to a target property defined in TargetProperty,
    /// with an optional transformation specified in JsFormula.
    /// This rule is associated with a MappingProject that defines the common JSON schema.
    /// </summary>
    internal class MappingRule : BaseModel<MappingRule>
    {
        /// <summary>
        /// Parameterless constructor required by Entity Framework.
        /// Ensures the entity can be instantiated without dependencies.
        /// </summary>
        internal MappingRule() : base(null) { }

        /// <summary>
        /// Constructor that accepts an optional validator.
        /// Allows validation logic to be injected at creation.
        /// </summary>
        /// <param name="validator">The validator for this instance.</param>
        public MappingRule(IValidateAndFix<MappingRule>? validator)
            : base(validator) { }

        /// <summary>
        /// Determines whether this entity should generate a slug automatically.
        /// </summary>
        protected override bool HasSlug => false;

        // -------------------------------------------------
        //   DATABASE PROPERTIES
        // -------------------------------------------------

        /// <summary>
        /// Foreign key to the MappingProject that groups the mapping rules.
        /// Ensures that each mapping rule belongs to exactly one MappingProject.
        /// </summary>
        [Required]
        public int MappingProjectId { get; set; }

        /// <summary>
        /// Foreign key to the JsonFieldinition that defines the JSON field path.
        /// Ensures that the rule is associated with a specific JSON field.
        /// </summary>
        [Required]
        public int JsonFieldId { get; set; }

        /// <summary>
        /// Foreign key to the TargetProperty that defines the target property.
        /// Ensures that each target property can be mapped multiple times within a project.
        /// </summary>
        [Required]
        public int TargetPropertyId { get; set; }

        /// <summary>
        /// JavaScript formula used to transform or validate the source value before mapping.
        /// Required, but defaults to a simple identity transformation.
        /// </summary>
        [Required, MaxLength(1000)]
        public string JsFormula { get; set; } = "function transform(value) { return value; }";

        // -------------------------------------------------
        //   NAVIGATION PROPERTIES (DATABASE RELATIONSHIPS)
        // -------------------------------------------------

        /// <summary>
        /// Navigation property to the MappingProject.
        /// Links this rule to its parent MappingProject.
        /// </summary>
        [ForeignKey(nameof(MappingProjectId))]
        public virtual MappingProject MappingProject { get; set; } = null!;

        /// <summary>
        /// Navigation property to the JSON field definition.
        /// Establishes a relationship between the rule and the JSON field.
        /// </summary>
        [ForeignKey(nameof(JsonFieldId))]
        public virtual JsonField JsonField { get; set; } = null!;

        /// <summary>
        /// Navigation property to the target property definition.
        /// Establishes a relationship between the rule and the target property.
        /// </summary>
        [ForeignKey(nameof(TargetPropertyId))]
        public virtual TargetProperty TargetProperty { get; set; } = null!;

        // -------------------------------------------------
        //   PROXY PROPERTIES (NOT MAPPED)
        // -------------------------------------------------

        /// <summary>
        /// Gets the fully qualified name of the target property, as defined in the TargetProperty.
        /// Example: "JsonBridgeEF.Models.TargetDb.User.ShippingAddress.City.Street".
        /// Not mapped to the database, used for reference in business logic.
        /// </summary>
        [NotMapped]
        public string FullyQualifiedTargetPropertyName => TargetProperty.FullyQualifiedPropertyName;

        /// <summary>
        /// Gets the class qualified name of the target entity.
        /// Example: "JsonBridgeEF.Models.TargetDb.User".
        /// Not mapped to the database, used for reference in business logic.
        /// </summary>
        [NotMapped]
        public string TargetClassQualifiedName => TargetProperty.ClassQualifiedName;
    }
}