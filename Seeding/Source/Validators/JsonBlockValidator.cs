using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonBlock"/> that ensures correctness of JSON block definitions.
    /// </summary>
    internal class JsonBlockValidator : IValidateAndFix<JsonBlock>
    {
        private readonly JsonFieldValidator _fieldValidator = new();

        /// <inheritdoc />
        public void EnsureValid(JsonBlock model)
        {
            ValidateName(model.Name);
            ValidateSlug(model.Slug);
            ValidateOwner(model.Owner);
            ValidateUniqueId(model.UniqueId);
            ValidateFields(model);
            ValidateKeyField(model);
            ValidateHierarchy(model);
        }

        /// <inheritdoc />
        public void Fix(JsonBlock model)
        {
            foreach (var field in model.Entities)
                _fieldValidator.Fix(field);
        }

        // ======================== NAME ========================
        private static void ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name cannot be null or empty.");
        }

        // ======================== SLUG ========================
        private static void ValidateSlug(string? slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ValidationException("The Slug cannot be null or empty.");
        }

        // ======================== OWNER ========================
        private static void ValidateOwner(JsonSchema? owner)
        {
            if (owner == null)
                throw new ValidationException("The JsonBlock must have a valid owner (JsonSchema).");
        }

        // ======================== UNIQUE ID ========================
        private static void ValidateUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty)
                throw new ValidationException("The UniqueId must be a valid, non-empty GUID.");
        }

        // ======================== FIELDS ========================
        private void ValidateFields(JsonBlock block)
        {
            var nameSet = new HashSet<string>();

            foreach (var field in block.Entities)
            {
                // Valida il campo singolarmente
                _fieldValidator.EnsureValid(field);

                // Verifica duplicati
                if (!nameSet.Add(field.Name))
                    throw new ValidationException($"Duplicate field name '{field.Name}' found in block '{block.Name}'.");
            }
        }

        // ======================== KEY FIELD ========================
        private static void ValidateKeyField(JsonBlock block)
        {
            var key = block.KeyEntity;

            if (key == null)
                return;

            if (!block.Entities.Contains(key))
                throw new ValidationException($"Key field '{key.Name}' does not belong to block '{block.Name}'.");
        }

        // ======================== HIERARCHY ========================
        private static void ValidateHierarchy(JsonBlock block)
        {
            foreach (var parent in block.Parents)
            {
                if (!parent.Children.Contains(block))
                {
                    throw new ValidationException(
                        $"Hierarchy inconsistency: parent block '{parent.Name}' does not recognize '{block.Name}' as a child.");
                }
            }

            foreach (var child in block.Children)
            {
                if (!child.Parents.Contains(block))
                {
                    throw new ValidationException(
                        $"Hierarchy inconsistency: child block '{child.Name}' does not recognize '{block.Name}' as a parent.");
                }
            }

            if (block.Parents.Contains(block) || block.Children.Contains(block))
            {
                throw new ValidationException($"A block cannot be its own parent or child: '{block.Name}'.");
            }
        }
    }
}