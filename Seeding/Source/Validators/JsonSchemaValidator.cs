using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.Exceptions;
using JsonBridgeEF.Seeding.Source.Helpers;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;

namespace JsonBridgeEF.Seeding.Source.Validators
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
    internal sealed class JsonSchemaValidator : IValidateAndFix<JsonSchema>
    {
        private readonly IValidateAndFix<JsonEntity> _jsonEntityValidator;
        private const int MaxDescriptionLength = 1000;

        /// <summary>
        /// Costruttore con injection opzionale del validatore dei blocchi JSON.
        /// </summary>
        /// <param name="jsonEntityValidator">
        /// Validatore per <see cref="JsonEntity"/>. 
        /// Se <c>null</c>, viene usato <see cref="JsonEntityValidator"/>.
        /// </param>
        public JsonSchemaValidator(IValidateAndFix<JsonEntity>? jsonEntityValidator = null)
        {
            _jsonEntityValidator = jsonEntityValidator ?? new JsonEntityValidator();
        }

        /// <inheritdoc />
        public void EnsureValid(JsonSchema schema)
        {
            ValidateName(schema.Name);
            ValidateDescription(schema.Description);
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
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name of the schema cannot be null or empty.");
        }

        // ======================== DESCRIPTION ========================

        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > MaxDescriptionLength)
                throw new ValidationException(
                    $"The schema description cannot exceed {MaxDescriptionLength} characters.");
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
                    throw new JsonEntityAlreadyExistsException(jsonEntity.Name);

                JsonSchemaHelper.EnsureJsonEntityBelongsToSchema(schema, jsonEntity);
            }
        }
    }
}