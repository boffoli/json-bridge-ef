using System.Collections.ObjectModel;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.Exceptions;
using JsonBridgeEF.Seeding.Source.Interfaces;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Model.JsonSchemas
{
    /// <inheritdoc cref="IJsonSchema{TJsonEntity, TProperty}"/>
    /// <summary>
    /// Partial class di <see cref="JsonSchema"/> che implementa le proprietà e il costruttore principali dello schema JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta uno schema JSON, contenente la definizione di uno o più oggetti (<see cref="JsonEntity"/>) e delle loro proprietà (<see cref="JsonProperty"/>).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Creazione tramite costruttore esplicito con nome, contenuto JSON originale, descrizione opzionale e validatore facoltativo.</para>
    /// <para><b>Invariants:</b><br/>
    /// - <see cref="Name"/> non può essere nullo o vuoto.<br/>
    /// - <see cref="JsonSchemaContent"/> non può essere <c>null</c>.<br/>
    /// - Descrizione può essere <c>null</c> o vuota.</para>
    /// <para><b>Relationships:</b><br/>
    /// Mantiene una collezione di <see cref="JsonEntities"/>, che rappresentano i singoli oggetti definiti dallo schema.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Per aggiungere un nuovo entity schema utilizzare <see cref="AddJsonEntity"/>.<br/>
    /// - Ogni aggiunta aggiorna automaticamente i metadati di dominio tramite <see cref="Touch"/>.</para>
    /// </remarks>
    internal sealed partial class JsonSchema
        : IJsonSchema<JsonEntity, JsonProperty>,
          IDomainMetadata,
          IEfEntity
    {
        private readonly List<JsonEntity> _jsonEntities = [];
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
            string? description,
            IValidateAndFix<JsonSchema>? validator)
        {
            Name = name;
            JsonSchemaContent = jsonSchemaContent;
            _metadata = new DomainMetadata(name, description);
            _validator = validator;

            _validator?.EnsureValid(this); // Validazione delegata
        }

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
                throw JsonSchemaError.InvalidName();

            _jsonEntities.Add(jsonEntity);
            this.Touch();
        }
    }
}
