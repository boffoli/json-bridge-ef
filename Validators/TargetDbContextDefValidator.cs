using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Helpers;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="TargetDbContextDef"/> ensuring the validity of database context definitions.
    /// </summary>
    internal partial class TargetDbContextDefValidator : IValidateAndFix<TargetDbContextDef>
    {
        private static readonly Regex NamespacePattern = MyRegex();

        /// <inheritdoc />
        public void EnsureValid(TargetDbContextDef model)
        {
            ValidateId(model.Id);
            ValidateName(model.Name);
            ValidateNamespace(model.Namespace);
            ValidateDescription(model.Description);
        }

        /// <inheritdoc />
        public void Fix(TargetDbContextDef model)
        {
            model.Description = FixDescription(model.Description);
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
        }

        // ======================== METODI PRIVATI PER NAMESPACE ========================
        private static void ValidateNamespace(string namespaceValue)
        {
            if (string.IsNullOrWhiteSpace(namespaceValue))
                throw new ValidationException("The Namespace cannot be empty.");

            if (!NamespacePattern.IsMatch(namespaceValue))
                throw new ValidationException("The Namespace contains invalid characters.");

            if (!TargetModelInspector.NamespaceExists(namespaceValue))
                throw new ValidationException($"The Namespace '{namespaceValue}' does not exist.");
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

        [GeneratedRegex(@"^[a-zA-Z0-9_.]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}