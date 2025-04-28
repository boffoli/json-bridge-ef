using System.Text.RegularExpressions;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.EntityModel.Validators;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Exceptions;

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

        /// <inheritdoc />
        /// <summary>
        /// Verifica che la classe sia valida. 
        /// Include la validazione di namespace, nome qualificato della classe, DbContext e descrizione.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// Corregge eventuali problemi nel modello. 
        /// Normalizza la descrizione se è null.
        /// </summary>
        public void Fix(ClassInfo model)
        {
            base.Fix(model);
            model.Description = FixDescription(model.Description);
        }

        /// <summary>
        /// Verifica che il namespace sia valido.
        /// </summary>
        /// <param name="ns">Il namespace da validare.</param>
        private static void ValidateNamespace(string ns)
        {
            if (string.IsNullOrWhiteSpace(ns))
                throw ClassInfoError.InvalidClassInfo();
            if (!NamespaceRegex.IsMatch(ns))
                throw ClassInfoError.InvalidClassQualifiedName(ns, ns);
        }

        /// <summary>
        /// Verifica che il nome qualificato della classe sia corretto.
        /// </summary>
        /// <param name="qualifiedName">Il nome qualificato da validare.</param>
        /// <param name="ns">Il namespace della classe.</param>
        /// <param name="name">Il nome della classe.</param>
        private static void ValidateClassQualifiedName(
            string qualifiedName,
            string ns,
            string name)
        {
            var expected = $"{ns}.{name}";
            if (qualifiedName != expected)
                throw ClassInfoError.InvalidClassQualifiedName(expected, qualifiedName);
            if (!IdentifierRegex.IsMatch(name))
                throw ClassInfoError.InvalidClassName(name);
            _ = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(a => a.GetType(qualifiedName, throwOnError: false, ignoreCase: false))
                .FirstOrDefault(t => t != null)
                ?? throw ClassInfoError.ClassNotFound(qualifiedName);
        }

        /// <summary>
        /// Verifica che il DbContext associato sia valido.
        /// </summary>
        /// <param name="ctx">Il DbContext da validare.</param>
        private static void ValidateDbContext(DbContextInfo ctx)
        {
            if (ctx is null)
                throw ClassInfoError.InvalidDbContext();
        }

        /// <summary>
        /// Verifica che la descrizione non ecceda la lunghezza massima consentita.
        /// </summary>
        /// <param name="description">La descrizione da validare.</param>
        private static void ValidateDescription(string? description)
        {
            if (description != null && description.Length > MaxDescriptionLength)
                throw ClassInfoError.DescriptionTooLong(description, MaxDescriptionLength);
        }

        /// <summary>
        /// Restituisce una descrizione non nulla, convertendo <c>null</c> in stringa vuota.
        /// </summary>
        /// <param name="description">La descrizione da normalizzare.</param>
        /// <returns>La descrizione normalizzata.</returns>
        private static string FixDescription(string? description)
            => description ?? string.Empty;

        /// <summary>
        /// Regex per validare identificatori C#.
        /// </summary>
        [GeneratedRegex(@"^[A-Za-z_]\w*$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();

        /// <summary>
        /// Regex per validare namespace C#.
        /// </summary>
        [GeneratedRegex(@"^([A-Za-z_]\w*)(\.[A-Za-z_]\w*)*$", RegexOptions.Compiled)]
        private static partial Regex MyRegex1();
    }
}