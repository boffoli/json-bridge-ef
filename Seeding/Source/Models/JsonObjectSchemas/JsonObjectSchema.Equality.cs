namespace JsonBridgeEF.Seeding.Source.Model.JsonObjectSchemas
{
    /// <summary>
    /// Partial class di <see cref="JsonObjectSchema"/> contenente gli override per l'uguaglianza semantica basata sullo schema associato.
    /// </summary>
    internal sealed partial class JsonObjectSchema
    {
        // === Equality Overrides ===

        /// <inheritdoc />
        /// <summary>
        /// Determina se questo oggetto è logicamente equivalente a un altro oggetto dello stesso tipo.
        /// Due oggetti sono considerati equivalenti se condividono lo stesso schema associato.
        /// </summary>
        /// <param name="other">L'altra istanza di <see cref="JsonObjectSchema"/> da confrontare.</param>
        /// <returns><c>true</c> se gli oggetti hanno lo stesso schema; altrimenti, <c>false</c>.</returns>
        protected override bool EqualsByValue(JsonObjectSchema other)
        {
            // Due oggetti sono considerati logicamente equivalenti se hanno lo stesso schema associato.
            return Equals(Schema, other.Schema);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash code basato sullo schema associato per garantire coerenza con <see cref="EqualsByValue(JsonObjectSchema)"/>.
        /// </summary>
        /// <returns>L'hash code calcolato in base allo schema.</returns>
        protected override int GetValueHashCode()
        {
            // Combina l’hash dello schema per coerenza con EqualsByValue.
            return Schema?.GetHashCode() ?? 0;
        }
    }
}