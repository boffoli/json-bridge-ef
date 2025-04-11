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
        /// Per l'uguaglianza, si richiede che:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       I nomi (campo <c>Name</c>, verificato in maniera case-insensitive) siano uguali.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Il flag <c>IsKey</c> sia identico.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Il riferimento al <c>Parent</c> sia uguale (tramite il confronto definito sul nodo stesso).
        ///     </description>
        ///   </item>
        /// </list>
        /// Poiché il confronto per il nome è solitamente già effettuato a livello di classe base,
        /// questo override effettua comunque il controllo esplicito per sicurezza.
        /// </summary>
        /// <param name="other">L'altra istanza di <see cref="JsonProperty"/> da confrontare.</param>
        /// <returns><c>true</c> se le proprietà sono considerate logicamente equivalenti; altrimenti, <c>false</c>.</returns>
        protected sealed override bool EqualsByValue(JsonProperty other)
        {
            // Controllo esplicito per Name, IsKey e Parent.
            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase)
                   && this.IsKey == other.IsKey
                   && Equals(this.Parent, other.Parent);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash specifico per la proprietà JSON.
        /// Combina, per coerenza con EqualsByValue:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       Il campo <c>Name</c> normalizzato (trim e lowercase).
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Il valore del flag <c>IsKey</c>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       L'hash del <c>Parent</c>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </summary>
        /// <returns>Il valore dell'hash.</returns>
        protected sealed override int GetValueHashCode()
        {
            var hash = new HashCode();
            // Aggiunge il nome normalizzato
            hash.Add(this.Name?.Trim().ToLowerInvariant() ?? string.Empty);
            // Aggiunge lo stato di chiave
            hash.Add(this.IsKey);
            // Aggiunge l'hash del Parent (o 0 se null)
            hash.Add(this.Parent);
            return hash.ToHashCode();
        }
    }
}