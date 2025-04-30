using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonProperties.Model
{
    /// <inheritdoc cref="IJsonProperty{TSelf, TParent}"/>
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> contenente il costruttore e la logica principale.
    /// </summary>
    /// <remarks>
    /// Inizializza una nuova istanza della classe <see cref="JsonProperty"/>.
    /// </remarks>
    /// <param name="name">Nome della proprietà JSON.</param>
    /// <param name="parent">Oggetto JSON (schema) proprietario.</param>
    /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
    /// <param name="description">Descrizione della proprietà.</param>
    /// <param name="validator">Validatore opzionale da iniettare.</param>
    /// <inheritdoc cref="IJsonProperty{TSelf, TParent}"/>
    /// <remarks>
    /// Costruttore principale: accetta il parent come interfaccia, default jsFormula e validator opzionale.
    /// </remarks>
    /// <param name="name">Nome della proprietà JSON.</param>
    /// <param name="parent">Entità JSON proprietaria (interfaccia).</param>
    /// <param name="isKey">Se deve essere chiave logica.</param>
    /// <param name="description">Descrizione.</param>
    /// <param name="validator">Validatore opzionale.</param>
    internal sealed partial class JsonProperty(
        string name,
        IJsonEntity<JsonEntity, JsonProperty> parent,
        bool isKey,
        string description,
        IValidateAndFix<JsonProperty>? validator = null) : 
        EntityProperty<JsonProperty, JsonEntity>(name, parent, isKey, validator),
        IJsonProperty<JsonProperty, JsonEntity>
    {

        /// <inheritdoc />
        public void MarkAsKey() => SetKeyStatus(true);

        /// <inheritdoc />
        public void UnmarkAsKey() => SetKeyStatus(false);
    }
}