using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Facade.Dtos
{
    /// <summary>
    /// Domain Concept: DTO di ritorno dell’operazione di seeding JSON.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: istanziato dal facade dopo aver orchestrato i tre seeders.</para>
    /// <para>Constraints: contiene solo interfacce, non tipi concreti.</para>
    /// <para>Relationships: aggrega <see cref="IJsonSchema"/>, <see cref="IJsonEntity"/>, <see cref="IJsonProperty"/>.</para>
    /// <para>Usage Notes: viene restituito al client come risultato unico.</para>
    /// </remarks>
    public sealed class JsonSeedingResultDto
    {
        /// <summary>Lo schema JSON creato o aggiornato.</summary>
        public IJsonSchema Schema { get; }

        /// <summary>Lista dei blocchi (entità) generati.</summary>
        public IReadOnlyList<IJsonEntity> Entities { get; }

        /// <summary>Lista dei campi (proprietà) generati.</summary>
        public IReadOnlyList<IJsonProperty> Properties { get; }

        /// <summary>
        /// Inizializza un nuovo DTO di risultato seeding.
        /// </summary>
        public JsonSeedingResultDto(
            IJsonSchema schema,
            IReadOnlyList<IJsonEntity> entities,
            IReadOnlyList<IJsonProperty> properties)
        {
            Schema     = schema;
            Entities   = entities;
            Properties = properties;
        }
    }
}