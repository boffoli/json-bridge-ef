using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Helpers;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Validators
{
    /// <summary>
    /// Validator for the <see cref="TargetProperty"/> class.
    /// Ensures the validity of a target property definition and attempts to correct it when possible.
    /// </summary>
    internal class TargetPropertyValidator : IValidateAndFix<TargetProperty>
    {
        private static readonly Regex NamespacePattern = MyRegex();

        /// <inheritdoc />
        public void EnsureValid(TargetProperty model)
        {
            ValidateId(model.Id);
            ValidateTargetDbContextInfoId(model.TargetDbContextInfoId);
            ValidateNamespace(model.Namespace);
            ValidateRootClass(model.Namespace, model.RootClass);
            ValidatePath(model.Namespace, model.RootClass, model.Path);
            ValidateName(model.Namespace, model.RootClass, model.Path, model.Name);
        }

        /// <inheritdoc />
        public void Fix(TargetProperty model)
        {
            model.Path = FixPath(model.Path);
        }

        // ======================== METODI PRIVATI PER ID ========================
        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        // ======================== METODI PRIVATI PER TargetDbContextInfoId ========================
        private static void ValidateTargetDbContextInfoId(int targetDbContextInfoId)
        {
            if (targetDbContextInfoId <= 0)
                throw new ValidationException("The TargetDbContextInfoId must be a positive number.");
        }

        // ======================== METODI PRIVATI PER Namespace ========================
        private static void ValidateNamespace(string namespaceValue)
        {
            if (string.IsNullOrWhiteSpace(namespaceValue))
                throw new ValidationException("The Namespace cannot be empty.");

            if (!NamespacePattern.IsMatch(namespaceValue))
                throw new ValidationException("The Namespace contains invalid characters.");

            if (!TargetModelInspector.NamespaceExists(namespaceValue))
                throw new ValidationException($"The Namespace '{namespaceValue}' does not exist.");
        }

        // ======================== METODI PRIVATI PER RootClass ========================
        private static void ValidateRootClass(string namespaceValue, string rootClass)
        {
            if (string.IsNullOrWhiteSpace(rootClass))
                throw new ValidationException("The RootClass cannot be empty.");

            // Componi il nome completo del tipo
            string fullTypeName = $"{namespaceValue}.{rootClass}";

            // Verifica se il tipo esiste
            Type? type = TargetModelInspector.GetTypeByName(fullTypeName) ?? throw new ValidationException($"The RootClass '{rootClass}' in namespace '{namespaceValue}' does not exist.");

            // Verifica se il tipo contiene almeno una proprietà con [Key]
            if (!TargetModelInspector.HasKeyAttribute(type))
                throw new ValidationException($"The RootClass '{rootClass}' does not contain a property marked with [Key].");
        }

        // ======================== METODI PRIVATI PER Path ========================
        private static void ValidatePath(string namespaceValue, string rootClass, string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            Type? rootType = TargetModelInspector.GetTypeByName($"{namespaceValue}.{rootClass}") ?? throw new ValidationException($"Root class '{rootClass}' in namespace '{namespaceValue}' not found.");
            _ = TargetModelInspector.GetFinalTypeFromPath(rootType, path) ?? throw new ValidationException($"Invalid path '{path}' starting from '{rootClass}'.");
        }

        private static string FixPath(string? path)
        {
            return path ?? string.Empty;
        }

        // ======================== METODI PRIVATI PER Name ========================
        private static void ValidateName(string namespaceValue, string rootClass, string? path, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name cannot be empty.");

            Type? rootType = TargetModelInspector.GetTypeByName($"{namespaceValue}.{rootClass}") ?? throw new ValidationException($"Root class '{rootClass}' not found in namespace '{namespaceValue}'.");
            Type? resolvedType = TargetModelInspector.GetFinalTypeFromPath(rootType, path) ?? throw new ValidationException($"Invalid path '{path}' for root class '{rootClass}'.");
            if (!TargetModelInspector.PropertyExistsInType(resolvedType, name))
                throw new ValidationException($"The property '{name}' does not exist in '{resolvedType.Name}'.");
        }

        [GeneratedRegex(@"^[a-zA-Z0-9_.]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}