using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mapping.Models
{
    /// <summary>
    /// Represents a mapping project that links a source JSON schema to a target database context.
    /// This is the top-level entity that groups mapping rules.
    /// </summary>
    internal class MappingProject : BaseModel<MappingProject>
    {
        /// <summary>
        /// Constructor required by Entity Framework Core.
        /// </summary>
        internal MappingProject() : base(null) { }

        /// <summary>
        /// Constructor that accepts an optional validator.
        /// </summary>
        public MappingProject(IValidateAndFix<MappingProject>? validator)
            : base(validator) { }

        /// <summary>
        /// Determines whether this entity should generate a slug.
        /// </summary>
        protected sealed override bool HasSlug => true;

        /// <summary>
        /// Foreign key linking this mapping project to a JSON schema.
        /// </summary>
        [Required]
        public int JsonSchemaId { get; set; }

        /// <summary>
        /// Navigation property to the JSON schema being mapped.
        /// </summary>
        [ForeignKey(nameof(JsonSchemaId))]
        public virtual JsonSchema JsonSchema { get; set; } = null!;

        /// <summary>
        /// Foreign key linking this mapping project to the target database context.
        /// </summary>
        [Required]
        public int TargetDbContextInfoId { get; set; }

        /// <summary>
        /// Navigation property to the target database context.
        /// </summary>
        [ForeignKey(nameof(TargetDbContextInfoId))]
        public virtual TargetDbContextInfo TargetDbContextInfo { get; set; } = null!;

        /// <summary>
        /// Collection of mapping rules associated with this project.
        /// </summary>
        public virtual ICollection<MappingRule> MappingRules { get; set; } = new List<MappingRule>();
    }
}