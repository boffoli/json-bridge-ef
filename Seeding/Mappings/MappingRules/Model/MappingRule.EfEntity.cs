using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Model
{
    /// <summary>
    /// Domain Class: Partial contenente solo gli identificatori tecnici e le chiavi esterne.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Implementa IEfEntity e definisce Id e FK primitive.</para>
    /// <para>Creation Strategy: Popolato da EF Core durante il materialization.</para>
    /// </remarks>
    internal sealed partial class MappingRule
    {
        /// <inheritdoc />
        [Key]
        public int Id { get; private set; }

        /// <summary>
        /// Identifier of the parent mapping configuration.
        /// </summary>
        public int MappingConfigurationId { get; private set; }

        /// <summary>
        /// Identifier of the source JSON entity.
        /// </summary>
        public int JsonEntityId { get; private set; }

        /// <summary>
        /// Identifier of the source JSON property.
        /// </summary>
        public int JsonPropertyId { get; private set; }

        /// <summary>
        /// Identifier of the target class model.
        /// </summary>
        public int ClassInfoId { get; private set; }

        /// <summary>
        /// Identifier of the target class property.
        /// </summary>
        public int ClassPropertyId { get; private set; }
    }
}