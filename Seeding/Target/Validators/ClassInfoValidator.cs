using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.EntityModel.Validators;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;

namespace JsonBridgeEF.Seeding.Target.Validators
{
    /// <summary>
    /// Domain Concept: Validator per <see cref="ClassInfo"/> che garantisce la correttezza semantica 
    /// e strutturale della classe target.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Invocato dai servizi di seeding prima della persistenza in database.</para>
    /// <para><b>Constraints:</b><br/>
    /// Verifica namespace, nome qualificato, descrizione e associazione a <see cref="ClassInfo.DbContextInfo"/>.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="EntityValidator{TSelf, TEntityProperty}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizza regex per la sintassi, reflection per controllare l’esistenza del tipo e controlli su descrizione.</para>
    /// </remarks>
    internal sealed partial class ClassInfoValidator 
        : EntityValidator<ClassInfo, ClassProperty>, 
          IValidateAndFix<ClassInfo>
    {
        private const int MaxDescriptionLength = 1000;
        private static readonly Regex IdentifierRegex = MyRegex();
        private static readonly Regex NamespaceRegex  = MyRegex1();

        /// <inheritdoc/>
        public void EnsureValid(ClassInfo model)
        {
            // 1) validazione base (figli, chiave, parent/child integrity)
            base.EnsureValid(model);

            // 2) controlli specifici di ClassInfo
            ValidateNamespace(model.Namespace);
            ValidateClassQualifiedName(model.ClassQualifiedName, model.Namespace, model.Name);
            ValidateDbContext(model.DbContextInfo);
            ValidateDescription(model.Description);
        }

        /// <inheritdoc/>
        public void Fix(ClassInfo model)
        {
            base.Fix(model);
            // Normalizza la descrizione se è null
            model.Description = FixDescription(model.Description);
        }

        private static void ValidateNamespace(string ns)
        {
            if (string.IsNullOrWhiteSpace(ns))
                throw new ValidationException("Il namespace non può essere nullo o vuoto.");
            if (!NamespaceRegex.IsMatch(ns))
                throw new ValidationException($"Il namespace '{ns}' non è un identificatore C# valido.");
        }

        private static void ValidateClassQualifiedName(
            string qualifiedName,
            string ns,
            string name)
        {
            var expected = $"{ns}.{name}";
            if (qualifiedName != expected)
                throw new ValidationException(
                    $"Il ClassQualifiedName '{qualifiedName}' non corrisponde a '{expected}'.");
            if (!IdentifierRegex.IsMatch(name))
                throw new ValidationException(
                    $"Il nome della classe '{name}' non è un identificatore C# valido.");
            _ = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(a => a.GetType(qualifiedName, throwOnError: false, ignoreCase: false))
                .FirstOrDefault(t => t != null)
                ?? throw new ValidationException(
                    $"Impossibile trovare il tipo '{qualifiedName}' nelle assembly caricate.");
        }

        private static void ValidateDbContext(DbContextInfo ctx)
        {
            if (ctx is null)
                throw new ValidationException("La classe deve essere associata a un DbContextInfo valido.");
        }

        /// <summary>
        /// Verifica che la descrizione non ecceda la lunghezza massima consentita.
        /// </summary>
        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > MaxDescriptionLength)
                throw new ValidationException(
                    $"La descrizione non può superare {MaxDescriptionLength} caratteri.");
        }

        /// <summary>
        /// Restituisce una descrizione non nulla, convertendo <c>null</c> in stringa vuota.
        /// </summary>
        private static string FixDescription(string? description)
            => description ?? string.Empty;

        [GeneratedRegex(@"^[A-Za-z_]\w*$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();

        [GeneratedRegex(@"^([A-Za-z_]\w*)(\.[A-Za-z_]\w*)*$", RegexOptions.Compiled)]
        private static partial Regex MyRegex1();
    }
}