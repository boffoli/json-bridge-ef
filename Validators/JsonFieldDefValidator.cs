using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="JsonFieldDef"/> that ensures correctness of JSON field definitions.
    /// </summary>
    internal partial class JsonFieldDefValidator : IValidateAndFix<JsonFieldDef>
    {
        private static readonly Regex FieldPathPattern = MyRegex();

        /// <inheritdoc />
        public void EnsureValid(JsonFieldDef model)
        {
            ValidateId(model.Id);
            ValidateJsonSchemaDefId(model.JsonSchemaDefId);
            ValidateSourceFieldPath(model.SourceFieldPath);
            ValidateDescription(model.Description);
        }

        /// <inheritdoc />
        public void Fix(JsonFieldDef model)
        {
            model.Description = FixDescription(model.Description);
        }

        // ======================== METODI PRIVATI PER ID ========================
        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        // ======================== METODI PRIVATI PER JSON SCHEMA DEF ID ========================
        private static void ValidateJsonSchemaDefId(int jsonSchemaDefId)
        {
            if (jsonSchemaDefId <= 0)
                throw new ValidationException("The JsonSchemaDefId must be a positive number.");
        }

        // ======================== METODI PRIVATI PER SOURCE FIELD PATH ========================
        private static void ValidateSourceFieldPath(string sourceFieldPath)
        {
            if (string.IsNullOrWhiteSpace(sourceFieldPath))
                throw new ValidationException("The SourceFieldPath cannot be empty.");

            if (!FieldPathPattern.IsMatch(sourceFieldPath))
                throw new ValidationException("The SourceFieldPath contains invalid characters.");
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

        // ======================== METODI PRIVATI PER ISKEY ========================


        [GeneratedRegex(@"^[a-zA-Z0-9_.]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}