using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonField"/> that ensures correctness of JSON field definitions.
    /// </summary>
    internal class JsonFieldValidator : IValidateAndFix<JsonField>
    {
        /// <inheritdoc />
        public void EnsureValid(JsonField model)
        {
            ValidateName(model.Name);
            ValidateDescription(model.Description);
            ValidateOwner(model);
            ValidateUniqueId(model.UniqueId);
        }

        /// <inheritdoc />
        public void Fix(JsonField model)
        {
            model.Description = FixDescription(model.Description);
        }

        // ======================== NAME ========================
        private static void ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name cannot be null or empty.");
        }

        // ======================== DESCRIPTION ========================
        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > 500)
                throw new ValidationException("The Description cannot exceed 500 characters.");
        }

        private static string FixDescription(string? description)
        {
            return description ?? string.Empty;
        }

        // ======================== OWNER ========================
        private static void ValidateOwner(JsonField model)
        {
            if (model.Owner == null)
                throw new ValidationException("The JsonField must have a valid owner (JsonBlock).");
        }

        // ======================== UNIQUE ID ========================
        private static void ValidateUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty)
                throw new ValidationException("The UniqueId must be a valid, non-empty GUID.");
        }
    }
}