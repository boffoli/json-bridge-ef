using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.TargetModel.Models
{
    /// <summary>
    /// Represents the definition of a property within the target model.
    /// This class normalizes the data previously handled by TargetPropertyInfo.
    /// </summary>
    internal class TargetPropertyDef : ModelBase<TargetPropertyDef>
    {
        /// <summary>
        /// Constructor required by Entity Framework.
        /// </summary>
        internal TargetPropertyDef() : base(null) { }

        /// <summary>
        /// Constructor that accepts an optional validator.
        /// </summary>
        /// <param name="validator">The validator for this instance.</param>
        public TargetPropertyDef(IValidateAndFix<TargetPropertyDef>? validator)
            : base(validator) { }

        /// <summary>
        /// Unique identifier for the target property definition.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the target database context definition.
        /// </summary>
        [Required]
        public int TargetDbContextDefId { get; set; }

        /// <summary>
        /// Navigation property to the target database context definition.
        /// </summary>
        [ForeignKey(nameof(TargetDbContextDefId))]
        public virtual TargetDbContextDef TargetDbContextDef { get; set; } = null!;

        /// <summary>
        /// The namespace of the target model.
        /// Example: "JsonBridgeEF.Models.TargetDb".
        /// </summary>
        [Required]
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// The name of the main target class.
        /// Example: "User".
        /// </summary>
        [Required]
        public string RootClass { get; set; } = string.Empty;

        /// <summary>
        /// The path of nested properties, if applicable.
        /// Example: "ShippingAddress.City". Can be empty if not applicable.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// The final property name to which data is mapped.
        /// Example: "Street".
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Derived property: Concatenation of Namespace and RootClass.
        /// Example: "JsonBridgeEF.Models.TargetDb.User".
        /// </summary>
        public string ClassQualifiedName => $"{Namespace}.{RootClass}";

        /// <summary>
        /// Derived property: If Path is empty, returns "RootClass.Name"; otherwise, "RootClass.Path.Name".
        /// Example: "User.ShippingAddress.City.Street".
        /// </summary>
        public string PropertyQualifiedName => string.IsNullOrWhiteSpace(Path)
            ? $"{RootClass}.{Name}"
            : $"{RootClass}.{Path}.{Name}";

        /// <summary>
        /// Derived property: If Path is empty, returns "ClassQualifiedName.Name"; otherwise, "ClassQualifiedName.Path.Name".
        /// Example: "JsonBridgeEF.Models.TargetDb.User.ShippingAddress.City.Street".
        /// </summary>
        public string FullyQualifiedPropertyName => string.IsNullOrWhiteSpace(Path)
            ? $"{ClassQualifiedName}.{Name}"
            : $"{ClassQualifiedName}.{Path}.{Name}";

        /// <summary>
        /// Derived property: If Path is empty, returns only the Name; otherwise, concatenates Path and Name.
        /// Example: "ShippingAddress.City.Street".
        /// </summary>
        public string PropertyPathName => string.IsNullOrWhiteSpace(Path) ? Name : $"{Path}.{Name}";
    
    }
}