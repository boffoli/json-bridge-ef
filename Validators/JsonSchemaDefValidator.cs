using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonSchemaDef"/> to ensure schema definitions are valid.
    /// </summary>
    internal partial class JsonSchemaDefValidator : IValidateAndFix<JsonSchemaDef>
    {
        private static readonly Regex SchemaIdentifierPattern = MyRegex();

        /// <inheritdoc />
        public void EnsureValid(JsonSchemaDef model)
        {
            ValidateId(model.Id);
            ValidateName(model.Name);
            ValidateJsonSchemaIdentifier(model.JsonSchemaIdentifier);
            ValidateDescription(model.Description);
            ValidateJsonFieldDefs(model.JsonFieldDefs);
        }

        /// <inheritdoc />
        public void Fix(JsonSchemaDef model)
        {
            model.Name = FixName(model.Name);
            model.Description = FixDescription(model.Description);
            model.JsonFieldDefs = FixJsonFieldDefs(model.JsonFieldDefs);
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
        private static void ValidateJsonFieldDefs(ICollection<JsonFieldDef> jsonFieldDefs)
        {
            if (jsonFieldDefs == null)
                throw new ValidationException("The JsonFieldDefs collection cannot be null.");

            foreach (var fieldDef in jsonFieldDefs)
            {
                var validator = new JsonFieldDefValidator();
                validator.EnsureValid(fieldDef);
            }
        }

        private static ICollection<JsonFieldDef> FixJsonFieldDefs(ICollection<JsonFieldDef> jsonFieldDefs)
        {
            if (jsonFieldDefs == null)
                return [];

            var validator = new JsonFieldDefValidator();
            foreach (var fieldDef in jsonFieldDefs)
            {
                validator.Fix(fieldDef);
            }

            return jsonFieldDefs;
        }
        [GeneratedRegex(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}