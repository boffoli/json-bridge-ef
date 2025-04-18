namespace JsonBridgeEF.Seeding.Source.Model.JsonEntities
{
    /// <summary>
    /// Partial class di <see cref="JsonEntity"/> contenente gli override per l'uguaglianza semantica
    /// basata sulla struttura interna: nome, figli aggregati, figli foglia, identificabilità e proprietà chiave.
    /// </summary>
    internal sealed partial class JsonEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Determina se questo oggetto è logicamente equivalente a un altro oggetto dello stesso tipo.
        /// Due oggetti sono considerati equivalenti se:
        /// <list type="bullet">
        ///   <item><description>I loro <c>Name</c> sono uguali (case-insensitive).</description></item>
        ///   <item><description>Le collezioni <c>SelfChildren</c> contengono lo stesso numero di elementi e,
        ///         elemento per elemento, sono uguali.</description></item>
        ///   <item><description>Le collezioni <c>ValueChildren</c> contengono lo stesso numero di elementi e,
        ///         elemento per elemento, sono uguali.</description></item>
        ///   <item><description>Hanno lo stesso stato di identificabilità (risultato di <c>IsIdentifiable()</c>).</description></item>
        ///   <item><description>Se esiste una proprietà chiave (ottenuta con <c>GetKeyProperty()</c>),
        ///         essa è uguale nell'altro oggetto; oppure nessuno dei due la possiede.</description></item>
        /// </list>
        /// </summary>
        /// <param name="other">L'altra istanza di <see cref="JsonEntity"/> da confrontare.</param>
        /// <returns><c>true</c> se i due oggetti sono logicamente equivalenti; altrimenti <c>false</c>.</returns>
        protected sealed override bool EqualsByValue(JsonEntity other)
        {
            // Confronta i nomi in modo case-insensitive.
            if (!string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Confronta la collezione SelfChildren.
            if (this.SelfChildren.Count != other.SelfChildren.Count)
            {
                return false;
            }
            var selfChildren1 = this.SelfChildren.ToList();
            var selfChildren2 = other.SelfChildren.ToList();
            for (int i = 0; i < selfChildren1.Count; i++)
            {
                if (!selfChildren1[i].Equals(selfChildren2[i]))
                {
                    return false;
                }
            }

            // Confronta la collezione ValueChildren.
            if (this.ValueChildren.Count != other.ValueChildren.Count)
            {
                return false;
            }
            var valueChildren1 = this.ValueChildren.ToList();
            var valueChildren2 = other.ValueChildren.ToList();
            for (int i = 0; i < valueChildren1.Count; i++)
            {
                if (!valueChildren1[i].Equals(valueChildren2[i]))
                {
                    return false;
                }
            }

            // Confronta lo stato di identificabilità.
            if (this.IsIdentifiable() != other.IsIdentifiable())
            {
                return false;
            }

            // Confronta la proprietà chiave:
            var key1 = this.GetKeyProperty();
            var key2 = other.GetKeyProperty();
            if ((key1 == null) ^ (key2 == null))  // XOR: se solo uno è null.
            {
                return false;
            }
            if (key1 != null && !key1.Equals(key2))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Calcola l'hash code in modo da essere coerente con <see cref="EqualsByValue(JsonEntity)"/>.
        /// Combina:
        /// <list type="bullet">
        ///   <item><description>Il <c>Name</c> normalizzato (trim e lowercase).</description></item>
        ///   <item><description>Tutti gli hash degli elementi in <c>SelfChildren</c> nell'ordine.</description></item>
        ///   <item><description>Tutti gli hash degli elementi in <c>ValueChildren</c> nell'ordine.</description></item>
        ///   <item><description>Il valore booleano di <c>IsIdentifiable()</c>.</description></item>
        ///   <item><description>L'hash della proprietà chiave (se esistente); altrimenti, 0.</description></item>
        /// </list>
        /// </summary>
        /// <returns>L'hash code calcolato in base alla struttura logica dell'oggetto.</returns>
        protected sealed override int GetValueHashCode()
        {
            var hash = new HashCode();

            // Aggiunge il nome normalizzato.
            hash.Add(this.Name?.Trim().ToLowerInvariant() ?? string.Empty);

            // Aggiunge gli hash di tutti i SelfChildren (nell'ordine).
            foreach (var child in this.SelfChildren)
            {
                hash.Add(child?.GetHashCode() ?? 0);
            }

            // Aggiunge gli hash di tutti i ValueChildren (nell'ordine).
            foreach (var child in this.ValueChildren)
            {
                hash.Add(child?.GetHashCode() ?? 0);
            }

            // Aggiunge lo stato di identificabilità.
            hash.Add(this.IsIdentifiable());

            // Aggiunge l'hash della proprietà chiave (se presente).
            var key = this.GetKeyProperty();
            hash.Add(key != null ? key.GetHashCode() : 0);

            return hash.ToHashCode();
        }
    }
}