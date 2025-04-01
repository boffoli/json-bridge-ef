using System.Collections.ObjectModel;

namespace JsonBridgeEF.SharedModel.Seeding.JsonModel
{
    /// <inheritdoc />
    /// <summary>
    /// Domain Class: Schema JSON chiuso, associato a oggetti <see cref="JsonObjectSchema"/> e proprietà <see cref="JsonProperty"/>.
    /// </summary>
    internal sealed class JsonSchema : IJsonSchema<JsonObjectSchema, JsonProperty>
    {
        private readonly List<JsonObjectSchema> _objectSchemas = [];

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="JsonSchema"/> con nome e contenuto JSON.
        /// </summary>
        /// <param name="name">Nome univoco dello schema.</param>
        /// <param name="jsonSchemaContent">Contenuto JSON originale.</param>
        /// <exception cref="ArgumentException">Se <paramref name="name"/> è nullo o vuoto.</exception>
        /// <exception cref="ArgumentNullException">Se <paramref name="jsonSchemaContent"/> è <c>null</c>.</exception>
        public JsonSchema(string name, string jsonSchemaContent)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome dello schema non può essere nullo o vuoto.", nameof(name));

            Name = name;
            JsonSchemaContent = jsonSchemaContent ?? throw new ArgumentNullException(nameof(jsonSchemaContent));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string JsonSchemaContent { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<JsonObjectSchema> ObjectSchemas => new ReadOnlyCollection<JsonObjectSchema>(_objectSchemas);

        /// <inheritdoc />
        public IReadOnlyCollection<JsonObjectSchema> IdentObjectSchemas =>
            _objectSchemas.Where(o => o.IsIdentifiable()).ToList().AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyCollection<JsonObjectSchema> NonIdentObjectSchemas =>
            _objectSchemas.Where(o => !o.IsIdentifiable()).ToList().AsReadOnly();

        /// <summary>
        /// Aggiunge un oggetto JSON allo schema.
        /// </summary>
        /// <param name="objectSchema">Oggetto da aggiungere.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="objectSchema"/> è <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Se esiste già un oggetto con lo stesso nome.</exception>
        public void AddObjectSchema(JsonObjectSchema objectSchema)
        {
            ArgumentNullException.ThrowIfNull(objectSchema);

            if (_objectSchemas.Any(o => o.Name == objectSchema.Name))
                throw new InvalidOperationException($"Oggetto con nome duplicato: {objectSchema.Name}.");

            _objectSchemas.Add(objectSchema);
        }
    }
}