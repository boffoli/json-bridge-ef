using System.Text.RegularExpressions;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Target.DbContextInfos.Exceptions;
using JsonBridgeEF.Seeding.Target.DbContextInfos.Model;

namespace JsonBridgeEF.Seeding.Target.DbContextInfos.Validators
{
    /// <summary>
    /// Domain Concept: Validatore concreto per <see cref="DbContextInfo"/> che garantisce la correttezza semantica dei metadati di un DbContext EF.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Invocato dai servizi di seeding prima della configurazione dinamica del DbContext.</para>
    /// <para><b>Constraints:</b><br/>
    /// Verifica <see cref="DbContextInfo.Name"/>, <see cref="DbContextInfo.Namespace"/>, <see cref="DbContextInfo.ClassQualifiedName"/>,
    /// e lunghezza della <see cref="IDomainMetadata.Description"/>.</para>
    /// <para><b>Relationships:</b><br/>
    /// Implementa <see cref="IValidateAndFix{DbContextInfo}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizza regex per le convenzioni sui nomi e assicura la coerenza tra namespace e classQualifiedName.</para>
    /// </remarks>
    internal sealed class DbContextInfoValidator : IValidateAndFix<DbContextInfo>
    {
        private const int MaxDescriptionLength = 1000;
        private static readonly Regex IdentifierRegex = new Regex(@"^[A-Za-z_]\w*$", RegexOptions.Compiled);
        private static readonly Regex NamespaceRegex = new Regex(@"^([A-Za-z_]\w*)(\.[A-Za-z_]\w*)*$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public void EnsureValid(DbContextInfo info)
        {
            ValidateName(info.Name);
            ValidateNamespace(info.Namespace);
            ValidateClassQualifiedName(info.ClassQualifiedName, info.Namespace, info.Name);
            ValidateDescription(info.Description);
        }

        /// <inheritdoc/>
        public void Fix(DbContextInfo info)
        {
            // Normalizza la descrizione se è nulla o troppo lunga
            info.Description = FixDescription(info.Description);
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw DbContextInfoError.InvalidDbContextName(name);
            if (!IdentifierRegex.IsMatch(name))
                throw DbContextInfoError.InvalidDbContextName(name);
        }

        private static void ValidateNamespace(string ns)
        {
            if (string.IsNullOrWhiteSpace(ns))
                throw DbContextInfoError.InvalidDbContextNamespace(ns);
            if (!NamespaceRegex.IsMatch(ns))
                throw DbContextInfoError.InvalidDbContextNamespace(ns);
        }

        private static void ValidateClassQualifiedName(string qualifiedName, string ns, string name)
        {
            var expected = $"{ns}.{name}";
            if (!string.Equals(qualifiedName, expected, StringComparison.Ordinal))
                throw DbContextInfoError.InvalidDbContextClassQualifiedName(expected, qualifiedName);
        }

        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > MaxDescriptionLength)
                throw DbContextInfoError.DescriptionTooLong(MaxDescriptionLength);
        }

        private static string FixDescription(string? description)
        {
            if (description is null)
            {
                return string.Empty;
            }

            if (description.Length <= MaxDescriptionLength)
            {
                return description;
            }

            return description.Substring(0, MaxDescriptionLength);
        }
    }
}