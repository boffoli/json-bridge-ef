using JsonBridgeEF.SharedModel.DagModel;
using JsonBridgeEF.SharedModel.EntityModel;
using JsonBridgeEF.SharedModel.Infrastructure;

namespace JsonBridgeEF.SharedModel.Seeding.JsonModel
{
    /// <inheritdoc cref="IJsonObjectSchema{TSelf, TJsonProperty}"/>
    /// <summary>
    /// Domain Class: Oggetto JSON concreto associato a uno schema JSON generico.
    /// </summary>
    internal sealed class JsonObjectSchema 
        : Entity<JsonObjectSchema, JsonProperty>, 
          IJsonObjectSchema<JsonObjectSchema, JsonProperty>, 
          IParentNavigableNode<JsonObjectSchema>
    {
        // === Fields ===

        private readonly IJsonSchema<JsonObjectSchema, JsonProperty> _schema;
        private readonly ParentNavigationManager<JsonObjectSchema, JsonProperty> _parentManager;

        // === Constructor ===

        /// <summary>
        /// Inizializza una nuova istanza del tipo <see cref="JsonObjectSchema"/>.
        /// </summary>
        /// <param name="name">Nome dell’oggetto JSON.</param>
        /// <param name="schema">Schema JSON di riferimento.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="schema"/> è <c>null</c>.</exception>
        public JsonObjectSchema(string name, IJsonSchema<JsonObjectSchema, JsonProperty> schema)
            : base(name)
        {
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
            _parentManager = new(this);
        }

        // === Properties ===

        /// <inheritdoc />
        public IJsonSchema<JsonObjectSchema, JsonProperty> Schema => _schema;

        /// <inheritdoc />
        public IReadOnlyCollection<JsonObjectSchema> Parents => _parentManager.Parents;

        /// <inheritdoc />
        public bool IsRoot => _parentManager.IsRoot;

        // === Parent Navigation ===

        /// <inheritdoc />
        public void AddParent(JsonObjectSchema parent)
        {
            _parentManager.AddParent(parent, bidirectional: true);
        }

        // === Identification ===

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

        // === Validation Hooks ===

        /// <inheritdoc />
        protected override void AdditionalCustomValidateEntity(JsonObjectSchema child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        /// <inheritdoc />
        protected override void AdditionalCustomValidateProperty(JsonProperty child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        // === Equality Overrides ===

        /// <inheritdoc />
        protected override bool EqualsByValue(JsonObjectSchema other)
        {
            // Due oggetti sono considerati logicamente equivalenti se hanno lo stesso schema associato.
            return Equals(Schema, other.Schema);
        }

        /// <inheritdoc />
        protected override int GetValueHashCode()
        {
            // Combina l’hash dello schema per coerenza con EqualsByValue.
            return Schema?.GetHashCode() ?? 0;
        }
    }
}