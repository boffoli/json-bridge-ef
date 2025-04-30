using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Source.Facade.Dtos;
using JsonBridgeEF.Seeding.Source.Facade.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Services;
using JsonBridgeEF.Seeding.Source.JsonProperties.Services;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Services;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Model;

namespace JsonBridgeEF.Seeding.Source.Facade.Services
{
    /// <summary>
    /// Domain Concept: Facade che centralizza il seeding completo di schemi, entità e proprietà JSON.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: iniettato con <see cref="IUnitOfWork"/> e costruisce internamente i seeders.</para>
    /// <para>Constraints: delega tutta la logica dettagliata ai seeder specifici.</para>
    /// <para>Relationships: usa <c>JsonSchemaSeeder</c>, <c>JsonEntitySeeder</c> e <c>JsonPropertySeeder</c>.</para>
    /// <para>Usage Notes: espone un unico metodo <c>SeedAsync</c> per il client.</para>
    /// </remarks>
    /// <remarks>
    /// Crea un’istanza del facade per il seeding JSON.
    /// </remarks>
    internal sealed class JsonSeedingFacade : IJsonSeedingFacade
    {
        private readonly IUnitOfWork _unitOfWork;

        public JsonSeedingFacade(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task<JsonSeedingResultDto> SeedAsync(
            string schemaName,
            string sampleJsonFilePath,
            bool forceSave = false)
        {
            // 1) Seeding dello schema
            var schemaSeeder = new JsonSchemaSeeder(_unitOfWork);
            var schema = await schemaSeeder.SeedFromSampleJsonFileAsync(
                schemaName, sampleJsonFilePath, forceSave);

            // 2) Seeding dei blocchi (entità)
            var entitySeeder = new JsonEntitySeeder(_unitOfWork);
            var entities = await entitySeeder
                .SeedAsync<JsonEntity, JsonProperty>(schema);

            // 3) Seeding dei campi (proprietà)
            var propertySeeder = new JsonPropertySeeder(_unitOfWork);
            var properties = await propertySeeder
                .SeedAsync(schema, entities);

            // 4) Costruzione del DTO di risultato
            return new JsonSeedingResultDto(schema, entities, properties);
        }
    }
}