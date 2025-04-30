namespace JsonBridgeEF.Seeding.Target.DbContextInfos.Model
{
    /// <summary>
    /// Partial class di <see cref="DbContextInfo"/> che contiene gli override per il confronto logico e il calcolo dell'hash.
    /// </summary>
    internal partial class DbContextInfo
    {
        #region Equality Overrides

        /// <inheritdoc />
        /// <summary>
        /// Determina se questa istanza di <see cref="DbContextInfo"/> è logicamente equivalente a un'altra.
        /// Due istanze sono considerate equivalenti se hanno lo stesso nome (case-insensitive)
        /// e lo stesso <see cref="ClassQualifiedName"/> (case-sensitive).
        /// </summary>
        /// <param name="obj">L'oggetto da confrontare con l'istanza corrente.</param>
        /// <returns><c>true</c> se i due oggetti sono equivalenti; altrimenti, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not DbContextInfo other)
                return false;

            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ClassQualifiedName, other.ClassQualifiedName, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash code dell'oggetto basandosi sul nome e sul nome completo della classe.
        /// </summary>
        /// <returns>Il valore dell'hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(Name),
                ClassQualifiedName?.GetHashCode() ?? 0);
        }

        #endregion
    }
}