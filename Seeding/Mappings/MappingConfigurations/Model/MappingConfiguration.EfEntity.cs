using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model
{
    /// <summary>
    /// Domain Class: Partial contenente le proprietà tecniche per EF.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Implementa IEfEntity per fornire l'ID tecnico usato per la persistenza.</para>
    /// <para>Creation Strategy: Popolato automaticamente da EF Core.</para>
    /// <para>Constraints: L'ID è readonly per il dominio, EF lo imposta via reflection.</para>
    /// <para>Usage Notes: Include i link chiave esterna verso JsonSchema e TargetDbContextInfo.</para>
    /// </remarks>
    internal partial class MappingConfiguration
    {
        /// <inheritdoc />
        [Key]
        public int Id { get; private set; }

        /// <summary>
        /// Foreign key referring to the source JSON schema.
        /// </summary>
        [Required]
        public int JsonSchemaId { get; private set; }

        /// <summary>
        /// Foreign key referring to the EF DbContext.
        /// </summary>
        [Required]
        public int DbContextInfoId { get; private set; }
    }
}
