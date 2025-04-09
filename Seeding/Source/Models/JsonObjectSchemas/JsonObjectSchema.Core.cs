using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Navigation.Interfaces;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Seeding.Source.Interfaces;
using JsonBridgeEF.Shared.Navigation.Helpers;
using JsonBridgeEF.Shared.Domain.Model;

namespace JsonBridgeEF.Seeding.Source.Model.JsonObjectSchemas
{
    /// <inheritdoc cref="IJsonObjectSchema{TSelf, TJsonProperty}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta uno schema oggetto JSON composto da proprietà strutturate.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Costruito tramite costruttore esplicito con nome e schema JSON di appartenenza.</para>
    /// <para><b>Constraints:</b> Lo schema associato non può essere <c>null</c>.</para>
    /// <para><b>Relationships:</b> Ogni oggetto JSON è associato a uno schema tramite la proprietà <c>Schema</c>. 
    /// La relazione è unidirezionale: lo schema mantiene la collezione degli oggetti JSON, mentre l’oggetto JSON conosce solo il proprio schema.</para>
    /// </remarks>
    internal sealed partial class JsonObjectSchema
        : Entity<JsonObjectSchema, JsonProperty>,
          IJsonObjectSchema<JsonObjectSchema, JsonProperty>,
          IParentNavigableNode<JsonObjectSchema>,
          IDomainMetadata,
          IEfEntity
    {
        #region Costruttore

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonObjectSchema"/>.
        /// </summary>
        /// <param name="name">Il nome dell'oggetto JSON.</param>
        /// <param name="schema">Lo schema JSON a cui questo oggetto appartiene.</param>
        /// <exception cref="ArgumentNullException">Sollevata se <paramref name="schema"/> è <c>null</c>.</exception>
        public JsonObjectSchema(string name, IJsonSchema<JsonObjectSchema, JsonProperty> schema)
            : base(name)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            _metadata = new DomainMetadata(name);
            _parentManager = new ParentNavigationManager<JsonObjectSchema, JsonProperty>(this);

            // Relazione unidirezionale: registra questo oggetto nello schema
            schema.AddObjectSchema(this);
        }

        #endregion

        #region Proprietà

        /// <inheritdoc />
        public IJsonSchema<JsonObjectSchema, JsonProperty> Schema { get; }

        #endregion

        #region Identificazione

        /// <inheritdoc />
        public void MakeIdentifiable(IJsonProperty<JsonProperty, JsonObjectSchema> keyProperty, bool force = false)
        {
            if (force || !IsIdentifiable())
                keyProperty.MarkAsKey();
        }

        /// <inheritdoc />
        public void MakeIdentifiable(string propertyName, bool force = false)
        {
            foreach (var prop in Properties)
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase) &&
                    (force || !IsIdentifiable()))
                {
                    prop.MarkAsKey();
                    return;
                }
            }

            throw new InvalidOperationException($"Nessuna proprietà trovata con il nome '{propertyName}' o oggetto già identificabile.");
        }

        /// <inheritdoc />
        public bool MakeNonIdentifiable()
        {
            bool foundKey = false;
            foreach (var prop in Properties)
            {
                if (prop.IsKey)
                {
                    prop.UnmarkAsKey();
                    foundKey = true;
                }
            }
            return foundKey;
        }

        #endregion

        #region Validazione

        /// <summary>
        /// Esegue validazioni aggiuntive personalizzate su un'entità figlia di tipo <see cref="JsonObjectSchema"/>.
        /// </summary>
        protected override void AdditionalCustomValidateEntity(JsonObjectSchema child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        /// <summary>
        /// Esegue validazioni aggiuntive personalizzate su una proprietà figlia di tipo <see cref="JsonProperty"/>.
        /// </summary>
        protected override void AdditionalCustomValidateProperty(JsonProperty child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        #endregion
    }
}