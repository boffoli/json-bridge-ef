using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Exceptions; // Per InvalidSlugException
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonSchema"/> to ensure schema definitions are valid.
    /// </summary>
    internal class JsonSchemaValidator : IValidateAndFix<JsonSchema>
    {
        private readonly JsonBlockValidator _blockValidator = new();

        /// <inheritdoc />
        public void EnsureValid(JsonSchema model)
        {
            ValidateName(model.Name);
            ValidateSlug(model.Slug);
            ValidateJsonContent(model.JsonSchemaContent);
            ValidateUniqueId(model.UniqueId);
            ValidateBlocks(model);
        }

        /// <inheritdoc />
        public void Fix(JsonSchema model)
        {
            foreach (var block in model.Entities)
                _blockValidator.Fix(block);
        }

        // ======================== NAME ========================
        private static void ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name of the schema cannot be null or empty.");
        }

        // ======================== SLUG ========================
        private static void ValidateSlug(string? slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new InvalidSlugException(slug);
        }

        // ======================== JSON CONTENT ========================
        private static void ValidateJsonContent(string? content)
        {
            JsonSchemaHelper.EnsureJsonContentIsValid(content);
        }

        // ======================== UNIQUE ID ========================
        private static void ValidateUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty)
                throw new ValidationException("The UniqueId must be a valid, non-empty GUID.");
        }

        // ======================== BLOCKS ========================
        private void ValidateBlocks(JsonSchema schema)
        {
            var nameSet = new HashSet<string>();

            foreach (var block in schema.Entities)
            {
                // Validazione del singolo blocco
                _blockValidator.EnsureValid(block);

                // Verifica duplicati
                if (!nameSet.Add(block.Name))
                    throw new BlockAlreadyExistsException(block.Name);

                // Verifica coerenza del blocco con lo schema corrente
                JsonSchemaHelper.EnsureBlockBelongsToSchema(schema, block);
            }
        }
    }
}