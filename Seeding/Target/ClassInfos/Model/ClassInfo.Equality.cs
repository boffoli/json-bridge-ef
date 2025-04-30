namespace JsonBridgeEF.Seeding.Target.ClassInfos.Model
{
    /// <summary>
    /// Partial class di <see cref="ClassInfo"/> contenente gli override per il confronto logico e il calcolo dell'hash.
    /// </summary>
    internal sealed partial class ClassInfo
    {
        /// <inheritdoc />
        protected sealed override bool EqualsByValue(ClassInfo other)
        {
            // Due classi sono considerate logicamente uguali se appartengono allo stesso namespace
            // e hanno lo stesso nome qualificato completo (confronto case-insensitive).
            return string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ClassQualifiedName, other.ClassQualifiedName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        protected sealed override int GetValueHashCode()
        {
            // Combina l'hash del namespace e del nome qualificato in modo coerente con EqualsByValue.
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace),
                StringComparer.OrdinalIgnoreCase.GetHashCode(ClassQualifiedName));
        }
    }
}