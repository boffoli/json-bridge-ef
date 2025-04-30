using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonProperties.Model;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Model;
using JsonBridgeEF.Shared.EntityModel.Model;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Model
{
    /// <summary>
    /// Domain Concept: Oggetto JSON composto da proprietà strutturate, appartenente a uno schema JSON.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Creato tramite costruttore esplicito che accetta nome, descrizione e schema di appartenenza.</para>
    /// <para>Constraints: Il nome e lo schema associato non possono essere null o vuoti. L’entità può essere marcata come identificabile tramite una proprietà chiave.</para>
    /// <para>Relationships: Appartiene a uno <see cref="IJsonSchema{JsonEntity}"/>, contiene istanze di <see cref="JsonProperty"/>.</para>
    /// <para>Usage Notes: Dopo la creazione, l’entità viene automaticamente registrata nello schema. È possibile promuoverla a entità identificabile.</para>
    /// </remarks>
    internal sealed partial class JsonEntity
        : Entity<JsonEntity, JsonProperty>,
          IJsonEntity<JsonEntity, JsonProperty>
    {
        #region Constructor

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonEntity"/>.
        /// </summary>
        /// <param name="name">Il nome dell'entità JSON.</param>
        /// <param name="schema">Lo schema JSON proprietario, che implementa <see cref="IJsonSchema{JsonEntity}"/>.</param>
        /// <param name="description">Descrizione testuale dell'entità.</param>
        /// <param name="validator">Validatore opzionale per questa entità.</param>
        public JsonEntity(
            string name,
            IJsonSchema<JsonEntity> schema,
            string description,
            IValidateAndFix<JsonEntity>? validator = null)
            : base(name, validator)
        {
            // assegna il proprietario
            Schema = schema ?? throw JsonEntityError.InvalidSchemaReference();
            _metadata = new DomainMetadata(name, description);

            // registra questa entità nel suo schema
            schema.AddJsonEntity(this);
        }

        #endregion

        #region Navigation Property

        /// <inheritdoc />
        public IJsonSchema<JsonEntity> Schema { get; private set; }

        // implementazione esplicita per la versione non-generica
        IJsonSchema IJsonEntity.Schema => Schema;

        #endregion

        #region Identification Methods

        /// <inheritdoc />
        public void MakeIdentifiable(
            IJsonProperty<JsonProperty, JsonEntity> keyProperty,
            bool force = false)
        {
            if (force || !IsIdentifiable())
                keyProperty.MarkAsKey();
        }

        /// <inheritdoc />
        public void MakeIdentifiable(string propertyName, bool force = false)
        {
            foreach (var prop in Properties)
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                    && (force || !IsIdentifiable()))
                {
                    prop.MarkAsKey();
                    return;
                }
            }

            throw JsonEntityError.KeyPromotionFailed(propertyName);
        }

        /// <inheritdoc />
        public bool MakeNonIdentifiable()
        {
            bool removed = false;
            foreach (var prop in Properties)
            {
                if (prop.IsKey)
                {
                    prop.UnmarkAsKey();
                    removed = true;
                }
            }
            return removed;
        }

        #endregion

        #region Audit Hooks

        /// <inheritdoc />
        protected sealed override void OnAfterAddChildFlow(JsonEntity child)
            => Touch();

        /// <inheritdoc />
        protected sealed override void OnAfterAddChildFlow(JsonProperty child)
            => Touch();

        #endregion
    }
}