using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Helpers;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Model;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Services
{
    /// <summary>
    /// Domain Concept: Servizio per il seeding degli schemi JSON nel database.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Iniettato tramite <see cref="IUnitOfWork"/>; costruisce e salva istanze <see cref="JsonSchema"/> concrete.</para>
    /// <para>Constraints: Richiede contenuto JSON valido, nome univoco e tracciamento tramite EF.</para>
    /// <para>Relationships: Utilizza <see cref="JsonSchemaHelper"/> per generazione e validazione, e <see cref="JsonSchemaValidator"/> per la convalida strutturale.</para>
    /// <para>Usage Notes: Va utilizzato all’avvio per registrare uno schema completo, da cui si potranno derivare entità e proprietà.</para>
    /// </remarks>
    internal sealed class JsonSchemaSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Crea uno schema a partire da un file JSON di esempio.
        /// </summary>
        /// <param name="schemaName">Nome logico dello schema da creare.</param>
        /// <param name="sampleJsonFilePath">Percorso del file JSON esemplificativo.</param>
        /// <param name="forceSave">Se true, consente duplicazione controllata.</param>
        /// <returns>Istanza concreta dello schema salvato.</returns>
        internal async Task<IJsonSchema<JsonEntity>> SeedFromSampleJsonFileAsync(
            string schemaName,
            string sampleJsonFilePath,
            bool forceSave = false)
        {
            JsonSchemaHelper.EnsureFileExists(sampleJsonFilePath);
            var sampleJsonContent = await JsonSchemaHelper.ReadJsonFileContentAsync(sampleJsonFilePath);
            return await SeedFromSampleJsonAsync(schemaName, sampleJsonContent, forceSave);
        }

        /// <summary>
        /// Crea uno schema da una stringa JSON di esempio.
        /// </summary>
        /// <param name="schemaName">Nome logico dello schema.</param>
        /// <param name="sampleJsonContent">Contenuto JSON di esempio.</param>
        /// <param name="forceSave">Se true, consente salvataggio anche in caso di duplicati.</param>
        /// <returns>Istanza salvata dello schema JSON.</returns>
        internal async Task<IJsonSchema<JsonEntity>> SeedFromSampleJsonAsync(
            string schemaName,
            string sampleJsonContent,
            bool forceSave = false)
        {
            var generatedSchema = JsonSchemaHelper.GenerateSchemaFromSample(sampleJsonContent);
            return await SeedFromSchemaAsync(schemaName, generatedSchema, forceSave);
        }

        /// <summary>
        /// Salva uno schema JSON preesistente da file.
        /// </summary>
        /// <param name="schemaName">Nome da assegnare allo schema.</param>
        /// <param name="jsonSchemaFilePath">Percorso del file contenente lo schema.</param>
        /// <param name="forceSave">Se true, consente duplicati.</param>
        /// <returns>Oggetto schema salvato.</returns>
        internal async Task<IJsonSchema<JsonEntity>> SeedFromSchemaFileAsync(
            string schemaName,
            string jsonSchemaFilePath,
            bool forceSave = false)
        {
            JsonSchemaHelper.EnsureFileExists(jsonSchemaFilePath);
            var jsonSchemaContent = await JsonSchemaHelper.ReadJsonFileContentAsync(jsonSchemaFilePath);
            return await SeedFromSchemaAsync(schemaName, jsonSchemaContent, forceSave);
        }

        /// <summary>
        /// Salva uno schema preesistente dato come stringa.
        /// </summary>
        /// <param name="schemaName">Nome da assegnare allo schema.</param>
        /// <param name="jsonSchemaContent">Contenuto JSON strutturato da salvare.</param>
        /// <param name="forceSave">Consente salvataggio anche se già presente.</param>
        /// <returns>Istanza tracciata dello schema.</returns>
        internal async Task<IJsonSchema<JsonEntity>> SeedFromSchemaAsync(
            string schemaName,
            string jsonSchemaContent,
            bool forceSave = false)
        {
            var repo = GetRepository<JsonSchema>();

            await JsonSchemaHelper.EnsureSchemaNameIsValidAsync(schemaName, repo);
            JsonSchemaHelper.EnsureJsonContentIsValid(jsonSchemaContent);
            await JsonSchemaHelper.EnsureSchemaContentIsUniqueAsync(jsonSchemaContent, repo, forceSave);

            var jsonSchema = new JsonSchema(
                name: schemaName,
                jsonSchemaContent: jsonSchemaContent,
                description: string.Empty,
                validator: new JsonSchemaValidator()
            );

            return await SaveSchemaAsync(jsonSchema);
        }

        /// <summary>
        /// Registra lo schema fornito nel contesto EF e lo salva.
        /// </summary>
        /// <param name="jsonSchema">Istanza dello schema da tracciare e salvare.</param>
        /// <returns>Oggetto schema persistito.</returns>
        private async Task<IJsonSchema<JsonEntity>> SaveSchemaAsync(JsonSchema jsonSchema)
        {
            GetRepository<JsonSchema>().Add(jsonSchema);
            await SaveChangesAsync();

            Console.WriteLine($"📂 Schema JSON salvato: {jsonSchema.Name}");
            return jsonSchema;
        }
    }
}