using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.EntityModel.Validators;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;

namespace JsonBridgeEF.Seeding.Target.Validators
{
    /// <summary>
    /// Domain Concept: Validatore concreto per <see cref="ClassProperty"/> che estende <see cref="EntityPropertyValidator{TSelf, TEntity}"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Estende direttamente la logica base tramite ereditarietà.</para>
    /// <para><b>Constraints:</b><br/>
    /// Verifica la correttezza del nome qualificato completo e della descrizione.</para>
    /// <para><b>Relationships:</b><br/>
    /// Specializzazione di <see cref="EntityPropertyValidator{TSelf, TEntity}"/> per <see cref="ClassProperty"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Controlla che <see cref="ClassProperty.FullyQualifiedPropertyName"/> corrisponda a '{parent.ClassQualifiedName}.{name}' 
    /// e che la descrizione sia entro i limiti.</para>
    /// </remarks>
    internal sealed class ClassPropertyValidator
        : EntityPropertyValidator<ClassProperty, ClassInfo>,
          IValidateAndFix<ClassProperty>
    {
        private const int MaxDescriptionLength = 500;

        /// <inheritdoc/>
        public void EnsureValid(ClassProperty property)
        {
            // 1) validazione base di nome e parent linkage
            base.EnsureValid(property);

            // 2) validazione specifica: fully‑qualified name
            ValidateFullyQualifiedPropertyName(
                property.FullyQualifiedPropertyName,
                property.Parent,
                property.Name);

            // 3) validazione descrizione
            ValidateDescription(property.Description);
        }

        /// <inheritdoc/>
        public void Fix(ClassProperty property)
        {
            // applica correzioni base (es. descrizione null -> string.Empty)
            base.Fix(property);

            // normalizza la descrizione se è null
            property.Description = FixDescription(property.Description);
        }

        /// <summary>
        /// Verifica che il fully‑qualified property name sia corretto.
        /// </summary>
        private static void ValidateFullyQualifiedPropertyName(
            string fqn,
            ClassInfo parent,
            string name)
        {
            if (string.IsNullOrWhiteSpace(fqn))
                throw new ValidationException("Il nome qualificato della proprietà non può essere nullo o vuoto.");

            var expected = $"{parent.ClassQualifiedName}.{name}";
            if (!string.Equals(fqn, expected, StringComparison.Ordinal))
                throw new ValidationException(
                    $"Il FullyQualifiedPropertyName '{fqn}' non corrisponde al namespace e nome: atteso '{expected}'.");
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
    }
}