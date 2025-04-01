using JsonBridgeEF.SharedModel.EntityModel;

namespace JsonBridgeEF.SharedModel.Seeding.JsonModel
{
    /// <inheritdoc cref="IJsonProperty{TSelf, TParent}"/>
    /// <summary>
    /// Domain Class: rappresenta una proprietà concreta in un oggetto JSON.
    /// </summary>
    internal sealed class JsonProperty 
        : EntityProperty<JsonProperty, JsonObjectSchema>, 
          IJsonProperty<JsonProperty, JsonObjectSchema>
    {
        // === Constructor ===

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="JsonProperty"/>.
        /// </summary>
        /// <param name="name">Nome della proprietà JSON.</param>
        /// <param name="parent">Oggetto JSON (schema) proprietario.</param>
        /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="parent"/> è <c>null</c>.</exception>
        public JsonProperty(string name, JsonObjectSchema parent, bool isKey = false)
            : base(name, parent, isKey)
        {
        }

        // === Key Management ===

        /// <inheritdoc />
        public void MarkAsKey()
        {
            SetKeyStatus(true);
        }

        /// <inheritdoc />
        public void UnmarkAsKey()
        {
            SetKeyStatus(false);
        }

        // === Equality Overrides ===

        /// <summary>
        /// Confronto logico specifico per proprietà JSON.
        /// </summary>
        /// <remarks>
        /// In questa implementazione non esistono attributi aggiuntivi rispetto al nome.
        /// L'identità semantica è interamente rappresentata dal confronto case-insensitive sul nome,
        /// già gestito a livello superiore.
        /// </remarks>
        protected override bool EqualsByValue(JsonProperty other)
        {
            return true; // Nessuna proprietà aggiuntiva da confrontare
        }

        /// <summary>
        /// Calcolo dell'hash specifico oltre il nome (nessun campo aggiuntivo).
        /// </summary>
        /// <remarks>
        /// Poiché nessun altro campo concorre all'identità semantica,
        /// il valore restituito è neutro.
        /// </remarks>
        protected override int GetValueHashCode()
        {
            return 0; // Nessuna proprietà aggiuntiva => hash non contribuito
        }
    }
}