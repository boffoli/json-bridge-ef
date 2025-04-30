using System.Text.Json;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Model;

namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Helpers
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Helper statico per la gestione e validazione di schemi JSON nell’ambito del modello concettuale.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Tutti i metodi sono privi di stato e destinati a essere utilizzati in seeding, import o validazioni manuali.</para>
    /// <para><b>Constraints:</b><br/>
    /// Richiede repository compatibili con <see cref="IRepository{JsonSchema}"/>  
    /// e i marker <see cref="IJsonSchema"/>, <see cref="IJsonEntity"/>.</para>
    /// </summary>
    internal static class JsonSchemaHelper
    {
        // ————————————————————————————————
        // Nome e contenuto unico
        // ————————————————————————————————

        /// <summary>
        /// Verifica che <paramref name="schemaName"/> sia non vuoto e unico nel repository.
        /// </summary>
        public static async Task EnsureSchemaNameIsValidAsync(
            string schemaName,
            IRepository<JsonSchema> repository)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw JsonSchemaError.InvalidName(nameof(JsonSchema));

            if (await repository.ExistsAsync(s => s.Name == schemaName))
                throw JsonSchemaError.NameAlreadyExists(schemaName);
        }

        /// <summary>
        /// Verifica che <paramref name="jsonSchemaContent"/> sia unico, a meno di forza il salvataggio.
        /// </summary>
        public static async Task EnsureSchemaContentIsUniqueAsync(
            string jsonSchemaContent,
            IRepository<JsonSchema> repository,
            bool forceSave)
        {
            var existing = await repository.FirstOrDefaultAsync(s => s.JsonSchemaContent == jsonSchemaContent);
            if (existing is not null && !forceSave)
                throw JsonSchemaError.ContentAlreadyExists(existing.Name);
        }

        // ————————————————————————————————
        // File I/O e parsing JSON
        // ————————————————————————————————

        /// <summary>
        /// Verifica che il file esista sul filesystem.
        /// </summary>
        public static void EnsureFileExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw JsonSchemaError.InvalidFilePath();

            if (!File.Exists(filePath))
                throw JsonSchemaError.FileNotFound(filePath);
        }

        /// <summary>
        /// Legge il contenuto del file in modo asincrono.
        /// </summary>
        public static async Task<string> ReadJsonFileContentAsync(string filePath)
        {
            EnsureFileExists(filePath);
            return await File.ReadAllTextAsync(filePath);
        }

        /// <summary>
        /// Verifica che <paramref name="jsonContent"/> sia un JSON ben formato.
        /// </summary>
        public static void EnsureJsonContentIsValid(string? jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
                throw JsonSchemaError.InvalidJsonContent("Il contenuto JSON è nullo o vuoto.");

            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                throw JsonSchemaError.InvalidJsonContent("Il contenuto JSON non è valido.", ex);
            }
        }

        /// <summary>
        /// Genera uno schema JSON a partire da un esempio.
        /// </summary>
        public static string GenerateSchemaFromSample(string sampleJsonContent)
        {
            EnsureJsonContentIsValid(sampleJsonContent);
            var schema = NJsonSchema.JsonSchema.FromSampleJson(sampleJsonContent);
            return schema.ToJson();
        }

        // ————————————————————————————————
        // Ownership check
        // ————————————————————————————————

        /// <summary>
        /// Verifica che <paramref name="jsonEntity"/> appartenga realmente a <paramref name="schema"/>.
        /// </summary>
        public static void EnsureJsonEntityBelongsToSchema(
            IJsonSchema schema,
            IJsonEntity jsonEntity)
        {
            if (!ReferenceEquals(jsonEntity.Schema, schema))
                throw JsonSchemaError.EntityOwnershipMismatch(
                    jsonEntity.Name,
                    schema.Name);
        }
    }
}