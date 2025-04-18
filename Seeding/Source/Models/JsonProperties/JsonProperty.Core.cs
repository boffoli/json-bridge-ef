using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Seeding.Source.Interfaces;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Seeding.Source.Model.JsonProperties
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
    internal sealed partial class JsonProperty(
        string name,
        JsonEntity parent,
        bool isKey,
        string description,
        IValidateAndFix<JsonProperty>? validator) : EntityProperty<JsonProperty, JsonEntity>(name, parent, isKey, validator),
                                                 IJsonProperty<JsonProperty, JsonEntity>,
                                                 IDomainMetadata,
                                                 IEfEntity
    {

        /// <inheritdoc />
        public void MarkAsKey() => SetKeyStatus(true);

        /// <inheritdoc />
        public void UnmarkAsKey() => SetKeyStatus(false);
    }
}