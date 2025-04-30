using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonEntities.Validators;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Helpers;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Model;

namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Validators
{
    /// <summary>
    /// Domain Concept: Validatore concreto per <see cref="JsonSchema"/> che garantisce la correttezza semantica dello schema JSON e dei blocchi associati.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Istanza con injection opzionale del validatore di blocco (<see cref="JsonEntityValidator"/>).</para>
    /// <para><b>Constraints:</b><br/>
    /// Nome non nullo o vuoto, contenuto JSON valido, descrizione entro limiti, blocchi con nome univoco e ownership corretta.</para>
    /// <para><b>Relationships:</b><br/>
    /// Collabora con <see cref="JsonEntityValidator"/> per la validazione ricorsiva dei blocchi JSON.</para>
    /// <para><b>Usage Notes:</b>  
    /// Valido per ambienti di seeding o validazione dominio pre-persistenza.</para>
    /// </remarks>
    /// <remarks>
    /// Costruttore con injection opzionale del validatore dei blocchi JSON.
    /// </remarks>
    /// <param name="jsonEntityValidator">
    /// Validatore per <see cref="JsonEntity"/>. 
    /// Se <c>null</c>, viene usato <see cref="JsonEntityValidator"/>.
    /// </param>
    internal sealed class JsonSchemaValidator(IValidateAndFix<JsonEntity>? jsonEntityValidator = null) : IValidateAndFix<JsonSchema>
    {
        private readonly IValidateAndFix<JsonEntity> _jsonEntityValidator = jsonEntityValidator ?? new JsonEntityValidator();
        private const int MaxDescriptionLength = 1000;

        /// <inheritdoc />
        public void EnsureValid(JsonSchema schema)
        {
            ValidateName(schema.Name);
            ValidateDescription(schema.Description, schema.Name); // Passando il nome dello schema
            ValidateJsonContent(schema.JsonSchemaContent);
            ValidateJsonEntities(schema);
        }

        /// <inheritdoc />
        public void Fix(JsonSchema schema)
        {
            // correzione dei blocchi JSON
            foreach (var jsonEntity in schema.JsonEntities)
                _jsonEntityValidator.Fix(jsonEntity);

            // normalizzo la descrizione se è null
            schema.Description = FixDescription(schema.Description);
        }

        // ======================== NAME ========================

        private static void ValidateName(string? name)
        {
            // Aggiungiamo un controllo preventivo per evitare che venga passato un valore null o vuoto.
            if (string.IsNullOrWhiteSpace(name))
                throw JsonSchemaError.InvalidName("Schema name"); // Passiamo un messaggio significativo se il nome è null o vuoto
        }

        // ======================== DESCRIPTION ========================

        private static void ValidateDescription(string? description, string schemaName)
        {
            if (description != null && description.Length > MaxDescriptionLength)
                throw JsonSchemaError.DescriptionTooLong(schemaName, MaxDescriptionLength); // Passando il nome dello schema e la lunghezza massima
        }

        private static string FixDescription(string? description)
            => description ?? string.Empty;

        // ======================== JSON CONTENT ========================

        private static void ValidateJsonContent(string? content)
            => JsonSchemaHelper.EnsureJsonContentIsValid(content);

        // ======================== ENTITIES ========================

        private void ValidateJsonEntities(JsonSchema schema)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var jsonEntity in schema.JsonEntities)
            {
                _jsonEntityValidator.EnsureValid(jsonEntity);

                if (!seen.Add(jsonEntity.Name))
                    throw JsonEntityAlreadyExistsException.AlreadyExists(jsonEntity.Name);

                JsonSchemaHelper.EnsureJsonEntityBelongsToSchema(schema, jsonEntity);
            }
        }
    }
}