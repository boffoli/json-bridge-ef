namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Model
{
    /// <summary>
    /// Partial class di <see cref="JsonSchema"/> che contiene gli override per il confronto logico e il calcolo dell'hash.
    /// </summary>
    internal sealed partial class JsonSchema
    {
        #region Equality Overrides

        /// <inheritdoc />
        /// <summary>
        /// Determina se questo schema JSON è logicamente equivalente a un altro oggetto.
        /// Due schemi sono considerati equivalenti se hanno lo stesso nome (case-insensitive)
        /// e lo stesso contenuto JSON (case-sensitive).
        /// </summary>
        /// <param name="obj">L'oggetto da confrontare.</param>
        /// <returns><c>true</c> se equivalenti; altrimenti, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not JsonSchema other)
                return false;

            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(JsonSchemaContent, other.JsonSchemaContent, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash code dell'istanza basandosi sul nome e sul contenuto JSON.
        /// </summary>
        /// <returns>Il valore dell'hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(Name),
                JsonSchemaContent?.GetHashCode() ?? 0);
        }

        #endregion
    }
}