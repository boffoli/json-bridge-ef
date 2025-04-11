namespace JsonBridgeEF.Seeding.Target.Model.Properties
{
    /// <summary>
    /// Partial class di <see cref="ClassProperty"/> che implementa l’uguaglianza logica basata sul nome qualificato.
    /// </summary>
    internal sealed partial class ClassProperty
    {
        /// <inheritdoc />
        /// <summary>
        /// Confronta due proprietà in base al nome qualificato completo.
        /// </summary>
        /// <param name="other">Altra proprietà da confrontare.</param>
        /// <returns><c>true</c> se le proprietà sono logicamente equivalenti.</returns>
        protected sealed override bool EqualsByValue(ClassProperty other)
        {
            return string.Equals(
                FullyQualifiedPropertyName,
                other.FullyQualifiedPropertyName,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash code coerente con <see cref="EqualsByValue"/>.
        /// </summary>
        /// <returns>Hash code del nome qualificato.</returns>
        protected sealed override int GetValueHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(FullyQualifiedPropertyName);
        }
    }
}