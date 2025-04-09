using JsonBridgeEF.Seeding.Source.Model.JsonObjectSchemas;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Seeding.Source.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Model.JsonProperties
{
    /// <inheritdoc cref="IJsonProperty{TSelf, TParent}"/>
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> contenente il costruttore e la logica principale.
    /// </summary>
    internal sealed partial class JsonProperty : EntityProperty<JsonProperty, JsonObjectSchema>,
                                                 IJsonProperty<JsonProperty, JsonObjectSchema>,
                                                 IDomainMetadata,
                                                 IEfEntity
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonProperty"/>.
        /// </summary>
        /// <param name="name">Nome della proprietà JSON.</param>
        /// <param name="parent">Oggetto JSON (schema) proprietario.</param>
        /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="parent"/> è <c>null</c>.</exception>
        public JsonProperty(string name, JsonObjectSchema parent, bool isKey = false)
            : base(name, parent, isKey)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent), "Il genitore non può essere nullo.");

            _metadata = new DomainMetadata(name);
        }

        /// <inheritdoc />
        /// <summary>
        /// Designa questa proprietà come chiave logica per l'oggetto JSON proprietario.
        /// </summary>
        public void MarkAsKey()
        {
            SetKeyStatus(true);
        }

        /// <inheritdoc />
        /// <summary>
        /// Rimuove la designazione di chiave logica da questa proprietà.
        /// </summary>
        public void UnmarkAsKey()
        {
            SetKeyStatus(false);
        }
    }
}