using JsonBridgeEF.SharedModel.EntityModel;

namespace JsonBridgeEF.SharedModel.Seeding.ClassModel
{
    /// <inheritdoc cref="IClassProperty{TSelf, TParent}"/>
    /// <summary>
    /// Implementazione concreta di <see cref="IClassProperty{TSelf, TParent}"/> per il modello orientato agli oggetti.
    /// </summary>
    internal class ClassProperty : EntityProperty<ClassProperty, ClassModel>, IClassProperty<ClassProperty, ClassModel>
    {
        private readonly string _fullyQualifiedPropertyName;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="ClassProperty"/>.
        /// </summary>
        /// <param name="name">Nome della proprietà.</param>
        /// <param name="parent">Classe proprietaria.</param>
        /// <param name="fullyQualifiedPropertyName">
        /// Nome qualificato completo della proprietà (es. <c>MyApp.Models.User.FirstName</c>).
        /// </param>
        /// <param name="isKey">Indica se questa proprietà è designata come chiave logica.</param>
        /// <exception cref="ArgumentNullException">
        /// Sollevata se <paramref name="fullyQualifiedPropertyName"/> è null o vuoto.
        /// </exception>
        public ClassProperty(string name, ClassModel parent, string fullyQualifiedPropertyName, bool isKey = false)
            : base(name, parent, isKey)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedPropertyName))
                throw new ArgumentNullException(nameof(fullyQualifiedPropertyName), "Il nome qualificato della proprietà non può essere nullo o vuoto.");

            _fullyQualifiedPropertyName = fullyQualifiedPropertyName;
        }

        /// <inheritdoc />
        public string FullyQualifiedPropertyName => _fullyQualifiedPropertyName;

        /// <summary>
        /// Confronta due proprietà in base al nome qualificato completo.
        /// </summary>
        /// <param name="other">Altra proprietà da confrontare.</param>
        /// <returns><c>true</c> se le proprietà sono logicamente equivalenti.</returns>
        protected override bool EqualsByValue(ClassProperty other)
        {
            // Confronto case-insensitive sul nome completamente qualificato
            return string.Equals(
                FullyQualifiedPropertyName,
                other.FullyQualifiedPropertyName,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Restituisce un hash code basato sul nome qualificato completo.
        /// </summary>
        /// <returns>Hash code coerente con <see cref="EqualsByValue"/>.</returns>
        protected override int GetValueHashCode()
        {
            // Hash case-insensitive del nome qualificato
            return StringComparer.OrdinalIgnoreCase.GetHashCode(FullyQualifiedPropertyName);
        }
    }
}