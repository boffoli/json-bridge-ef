using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Helpers;
using JsonBridgeEF.Seeding.Source.Exceptions;

namespace JsonBridgeEF.Seeding.Source.Services
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Servizio per l’estrazione e la persistenza dei campi di un blocco JSON all’interno di uno schema.
    /// I <see cref="JsonProperty"/> vengono generati a partire dalla struttura del JSON associato.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// I campi vengono generati tramite <see cref="JsonPropertyExtractor"/> e tracciati tramite EF Core.
    /// Questo seeder non genera blocchi, ma opera sui blocchi già tracciati all'interno dello schema.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Inizializza il seeder dei campi JSON con il contesto di unità di lavoro corrente.
    /// </remarks>
    /// <param name="unitOfWork">Unità di lavoro attiva per accedere al contesto e ai repository.</param>
    internal sealed class JsonPropertySeeder : BaseDbService
    {
        public JsonPropertySeeder(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Estrae e salva i campi di un blocco specifico appartenente a uno schema.
        /// </summary>
        /// <param name="schema">Schema a cui appartiene il blocco.</param>
        /// <param name="jsonEntity">Blocco da cui generare i campi.</param>
        /// <returns>Lista dei campi generati e salvati.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Lo schema e il blocco devono essere non null e coerenti tra loro.</para>
        /// <para><b>Postconditions:</b> I campi vengono estratti e persistiti tramite il repository EF.</para>
        /// <para><b>Side Effects:</b> Salvataggio su database dei <see cref="JsonProperty"/>.</para>
        /// </remarks>
        internal async Task<List<JsonProperty>> SeedAsync(JsonSchema schema, JsonEntity jsonEntity)
        {
            if (schema == null)
                throw JsonSchemaError.InvalidSchemaReference();

            if (jsonEntity is null)
                throw JsonEntityError.InvalidJsonEntity();

            JsonSchemaHelper.EnsureJsonEntityBelongsToSchema(schema, jsonEntity);

            Console.WriteLine($"📂 Generazione campi per blocco '{jsonEntity.Name}' nello schema '{schema.Name}'");

            // 🧠 Estrazione dei campi dal contenuto dello schema
            JsonPropertyExtractor.ExtractJsonProperties(schema, jsonEntity);

            // 💾 Validazione e aggiunta dei campi al repository
            var repository = GetRepository<JsonProperty>();
            foreach (var property in jsonEntity.ValueChildren)
            {
                repository.Add(property);
            }

            await SaveChangesAsync();
            return jsonEntity.ValueChildren.ToList();
        }

        /// <summary>
        /// Estrae e salva i campi per una lista di blocchi appartenenti a uno schema.
        /// </summary>
        /// <param name="schema">Schema di riferimento.</param>
        /// <param name="jsonEntities">Lista di blocchi da processare.</param>
        /// <returns>Lista completa dei campi generati per tutti i blocchi.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> La lista di entità deve essere non nulla.</para>
        /// <para><b>Postconditions:</b> Tutti i campi validi vengono salvati nel contesto.</para>
        /// <para><b>Side Effects:</b> Persistenza multipla di <see cref="JsonProperty"/> nel repository.</para>
        /// </remarks>
        internal async Task<List<JsonProperty>> SeedAsync(JsonSchema schema, List<JsonEntity> jsonEntities)
        {
            if (jsonEntities == null || !jsonEntities.Any())
                return new List<JsonProperty>();

            var allFields = new List<JsonProperty>();

            foreach (var jsonEntity in jsonEntities)
            {
                var properties = await SeedAsync(schema, jsonEntity);
                allFields.AddRange(properties);
            }

            return allFields;
        }
    }
}