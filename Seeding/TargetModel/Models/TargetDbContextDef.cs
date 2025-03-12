using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.TargetModel.Models
{
    /// <summary>
    /// Defines the target database context in which target entities operate.
    /// Centralizes the target DbContext namespace and other relevant information.
    /// </summary>
    internal class TargetDbContextDef : ModelBase<TargetDbContextDef>
    {
        /// <summary>
        /// Constructor required by Entity Framework Core.
        /// </summary>
        internal TargetDbContextDef() : base(null) { }

        /// <summary>
        /// Public constructor that accepts an optional validator.
        /// </summary>
        /// <param name="validator">The validator for this instance.</param>
        public TargetDbContextDef(IValidateAndFix<TargetDbContextDef>? validator)
            : base(validator) { }

        /// <summary>
        /// Unique identifier of the target database context.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Display name of the target context.
        /// Example: "Target Database Context".
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Namespace of the target DbContext.
        /// Example: "JsonBridgeEF.Data".
        /// </summary>
        [Required]
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes or relevant information about the target context.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Collection of properties associated with this database context.
        /// Defines a 1:N relationship with <see cref="TargetPropertyDef"/>.
        /// </summary>
        public virtual ICollection<TargetPropertyDef> TargetProperties { get; set; } = [];
    }
}