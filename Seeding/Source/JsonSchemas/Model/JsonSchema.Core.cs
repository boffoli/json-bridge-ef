using System.Collections.ObjectModel;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Model;

namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Model
{
    /// <inheritdoc cref="IJsonSchema{JsonEntity}"/>
    /// <summary>
    /// Domain Concept: Implementazione concreta di uno schema JSON composto da blocchi strutturati (entità).
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Istanziato tramite costruttore esplicito, con nome, contenuto JSON e validatore opzionale.</para>
    /// <para>Constraints: Il nome e il contenuto JSON non possono essere nulli o vuoti. La descrizione è opzionale.</para>
    /// <para>Relationships: Contiene una lista di <see cref="JsonEntity"/>, che modellano gli oggetti JSON definiti nello schema.</para>
    /// <para>Usage Notes: Usare <see cref="AddJsonEntity"/> per registrare blocchi, i quali vengono tracciati internamente
    /// e restituiti tramite <see cref="JsonEntities"/>, <see cref="IdentJsonEntities"/> e <see cref="NonIdentJsonEntities"/>.</para>
    /// </remarks>
    internal sealed partial class JsonSchema : IJsonSchema<JsonEntity>
    {
        private readonly List<JsonEntity> _jsonEntities = new();
        private readonly IValidateAndFix<JsonSchema>? _validator;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonSchema"/>.
        /// </summary>
        /// <param name="name">Nome univoco dello schema.</param>
        /// <param name="jsonSchemaContent">Contenuto JSON originale.</param>
        /// <param name="description">Descrizione testuale opzionale.</param>
        /// <param name="validator">Validatore opzionale per lo schema.</param>
        public JsonSchema(
            string name,
            string jsonSchemaContent,
            string? description = null,
            IValidateAndFix<JsonSchema>? validator = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw JsonSchemaError.InvalidName(nameof(JsonSchema));
            if (string.IsNullOrWhiteSpace(jsonSchemaContent))
                throw JsonSchemaError.InvalidSchemaReference();

            Name = name;
            JsonSchemaContent = jsonSchemaContent;
            _metadata = new DomainMetadata(name, description);
            _validator = validator;

            _validator?.EnsureValid(this);
        }

        #region IJsonSchema<TJsonEntity> Members

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string JsonSchemaContent { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<JsonEntity> JsonEntities
            => new ReadOnlyCollection<JsonEntity>(_jsonEntities);

        /// <inheritdoc />
        public IReadOnlyCollection<JsonEntity> IdentJsonEntities
            => _jsonEntities
               .Where(e => e.IsIdentifiable())
               .ToList()
               .AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyCollection<JsonEntity> NonIdentJsonEntities
            => _jsonEntities
               .Where(e => !e.IsIdentifiable())
               .ToList()
               .AsReadOnly();

        /// <inheritdoc />
        public void AddJsonEntity(JsonEntity jsonEntity)
        {
            if (jsonEntity is null)
                throw JsonSchemaError.InvalidSchemaReference();

            _jsonEntities.Add(jsonEntity);
            Touch();
        }

        #endregion
    }
}