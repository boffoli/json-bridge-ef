namespace JsonBridgeEF.Seeding.Source.Model.JsonProperties
{
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> contenente gli override per l'uguaglianza semantica.
    /// </summary>
    internal sealed partial class JsonProperty
    {
        /// <inheritdoc />
        /// <summary>
        /// Confronto logico specifico per la proprietà JSON.
        /// </summary>
        /// <remarks>
        /// In questa implementazione non esistono attributi aggiuntivi rispetto al nome;
        /// l'identità semantica è interamente rappresentata dal confronto case-insensitive sul nome,
        /// già gestito dalla classe base <see cref="EntityProperty{TEntity, TParent}"/>.
        /// </remarks>
        /// <param name="other">L'altra istanza di <see cref="JsonProperty"/> da confrontare.</param>
        /// <returns><c>true</c> se le proprietà sono considerate logicamente equivalenti; altrimenti, <c>false</c>.</returns>
        protected override bool EqualsByValue(JsonProperty other)
        {
            return true; // Nessuna proprietà aggiuntiva da confrontare
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash specifico per la proprietà.
        /// </summary>
        /// <remarks>
        /// Nessun campo aggiuntivo contribuisce all'identità semantica, quindi il valore restituito è neutro.
        /// </remarks>
        /// <returns>Il valore dell'hash.</returns>
        protected override int GetValueHashCode()
        {
            return 0;
        }
    }
}