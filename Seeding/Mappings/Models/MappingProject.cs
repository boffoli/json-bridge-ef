using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Validators;
using JsonBridgeEF.Common;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mappings.Models
{
    /// <summary>
    /// Rappresenta un progetto di mapping che raggruppa un insieme di mapping rule
    /// e condivide uno schema JSON comune.
    /// </summary>
    internal class MappingProject : ModelBase<MappingProject>
    {
        /// <summary>
        /// Costruttore richiesto da Entity Framework.
        /// </summary>
        internal MappingProject() : base(null) { }

        /// <summary>
        /// Costruttore che accetta un validatore opzionale.
        /// </summary>
        /// <param name="validator">Il validatore per questa istanza.</param>
        public MappingProject(IValidateAndFix<MappingProject>? validator)
            : base(validator) { }

        /// <summary>
        /// Identificatore univoco del progetto di mapping.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome del progetto di mapping.
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Riferimento allo schema JSON comune per tutte le rule del progetto.
        /// </summary>
        [Required]
        public int JsonSchemaDefId { get; set; }

        /// <summary>
        /// Chiave esterna al contesto target di database.
        /// </summary>
        [Required]
        public int TargetDbContextDefId { get; set; } // 🔹 **Aggiunto campo FK**

        /// <summary>
        /// Navigation property allo schema JSON.
        /// </summary>
        [ForeignKey(nameof(JsonSchemaDefId))]
        public virtual JsonSchemaDef JsonSchemaDef { get; set; } = null!;

        /// <summary>
        /// Collezione di mapping rule associate a questo progetto.
        /// </summary>
        public virtual ICollection<MappingRule> MappingRules { get; set; } = [];

        /// <summary>
        /// Proprietà di navigazione al contesto target.
        /// </summary>
        [ForeignKey(nameof(TargetDbContextDefId))] // 🔹 **Associazione alla FK**
        public virtual TargetDbContextDef TargetDbContextDef { get; set; } = null!;
    }
}