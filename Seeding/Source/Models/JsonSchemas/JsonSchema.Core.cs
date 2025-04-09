using System.Collections.ObjectModel;
using JsonBridgeEF.Seeding.Source.Interfaces;
using JsonBridgeEF.Seeding.Source.Model.JsonObjectSchemas;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Model.JsonSchemas
{
    /// <inheritdoc cref="IJsonSchema{TObjectSchema, TProperty}" />
    /// <summary>
    /// Partial class di <see cref="JsonSchema"/> che implementa le proprietà e il costruttore principali dello schema JSON.
    /// </summary>
    internal sealed partial class JsonSchema
    : IJsonSchema<JsonObjectSchema, JsonProperty>,
      IDomainMetadata,
      IEfEntity
    {
        private readonly List<JsonObjectSchema> _objectSchemas = [];

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonSchema"/> con nome e contenuto JSON.
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
            _metadata = new DomainMetadata(name);
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

        /// <inheritdoc />
        public void AddObjectSchema(JsonObjectSchema objectSchema)
        {
            ArgumentNullException.ThrowIfNull(objectSchema);

            if (_objectSchemas.Any(o => o.Name == objectSchema.Name))
                throw new InvalidOperationException($"Oggetto con nome duplicato: {objectSchema.Name}.");

            _objectSchemas.Add(objectSchema);
            this.Touch();
        }
    }
}