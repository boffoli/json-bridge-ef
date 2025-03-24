using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonSchema"/> to ensure schema definitions are valid.
    /// </summary>
    internal partial class JsonSchemaValidator : IValidateAndFix<JsonSchema>
    {
        private static readonly Regex SchemaIdentifierPattern = MyRegex();

        /// <inheritdoc />
        public void EnsureValid(JsonSchema model)
        {
            ValidateId(model.Id);
            ValidateName(model.Name);
            ValidateJsonSchemaIdentifier(model.JsonSchemaIdentifier);
            ValidateDescription(model.Description);
            ValidateJsonFields(model.JsonFields);
        }

        /// <inheritdoc />
        public void Fix(JsonSchema model)
        {
            model.Name = FixName(model.Name);
            model.Description = FixDescription(model.Description);
            model.JsonFields = FixJsonFields(model.JsonFields);
        }

        // ======================== METODI PRIVATI PER ID ========================
        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        // ======================== METODI PRIVATI PER NAME ========================
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name cannot be empty.");

            if (name.Length > 200)
                throw new ValidationException("The Name cannot exceed 200 characters.");
        }

        private static string FixName(string? name)
        {
            return name ?? string.Empty;
        }

        // ======================== METODI PRIVATI PER JSON SCHEMA IDENTIFIER ========================
        private static void ValidateJsonSchemaIdentifier(string jsonSchemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(jsonSchemaIdentifier))
                throw new ValidationException("The JsonSchemaIdentifier cannot be empty.");

            if (!SchemaIdentifierPattern.IsMatch(jsonSchemaIdentifier))
                throw new ValidationException("The JsonSchemaIdentifier contains invalid characters.");

            if (jsonSchemaIdentifier.Length > 100)
                throw new ValidationException("The JsonSchemaIdentifier cannot exceed 100 characters.");
        }

        // ======================== METODI PRIVATI PER DESCRIPTION ========================
        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > 500)
                throw new ValidationException("The Description cannot exceed 500 characters.");
        }

        private static string FixDescription(string? description)
        {
            return description ?? string.Empty;
        }

        // ======================== METODI PRIVATI PER JSON FIELD DEFS ========================
        private static void ValidateJsonFields(ICollection<JsonField> jsonFields)
        {
            if (jsonFields == null)
                throw new ValidationException("The JsonFields collection cannot be null.");

            foreach (var field in jsonFields)
            {
                var validator = new JsonFieldValidator();
                validator.EnsureValid(field);
            }
        }

        private static ICollection<JsonField> FixJsonFields(ICollection<JsonField> jsonFields)
        {
            if (jsonFields == null)
                return [];

            var validator = new JsonFieldValidator();
            foreach (var field in jsonFields)
            {
                validator.Fix(field);
            }

            return jsonFields;
        }
        [GeneratedRegex(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}