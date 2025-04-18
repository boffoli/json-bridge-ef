using System;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.Interfaces;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Model.JsonEntities
{
    /// <inheritdoc cref="IJsonEntity{TSelf, TJsonProperty}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta uno schema oggetto JSON composto da proprietà strutturate.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Costruito tramite costruttore esplicito con nome e schema JSON di appartenenza.</para>
    /// <para><b>Constraints:</b> Lo schema associato non può essere <c>null</c>.</para>
    /// <para><b>Relationships:</b> Ogni oggetto JSON è associato a uno schema tramite la proprietà <c>Schema</c>. 
    /// La relazione è unidirezionale: lo schema mantiene la collezione degli oggetti JSON, mentre l’oggetto JSON conosce solo il proprio schema.</para>
    /// </remarks>
    internal sealed partial class JsonEntity
        : Entity<JsonEntity, JsonProperty>,
          IJsonEntity<JsonEntity, JsonProperty>,
          IDomainMetadata,
          IEfEntity
    {
        #region Costruttore

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="JsonEntity"/>.
        /// </summary>
        /// <param name="name">Il nome dell'oggetto JSON.</param>
        /// <param name="schema">Lo schema JSON a cui questo oggetto appartiene.</param>
        /// <param name="description">Descrizione testuale.</param>
        /// <param name="validator">Validatore opzionale da iniettare.</param>
        public JsonEntity(
            string name,
            JsonSchema schema,
            string description,
            IValidateAndFix<JsonEntity>? validator)
            : base(name, validator)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            _metadata = new DomainMetadata(name, description);

            // Relazione unidirezionale: registra questo oggetto nello schema
            schema.AddJsonEntity(this);
        }

        #endregion

        #region Proprietà

        /// <inheritdoc />
        public JsonSchema Schema { get; }

        #endregion

        #region Identificazione

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

            throw new InvalidOperationException(
                $"Nessuna proprietà trovata con il nome '{propertyName}' o oggetto già identificabile.");
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

        /// <inheritdoc />
        /// <summary>
        /// Metodo hook eseguito automaticamente al termine del flusso di aggiunta di un'entità figlia (<see cref="JsonEntity"/>).
        /// </summary>
        /// <param name="child">L'entità figlia appena aggiunta.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il flusso di aggiunta è stato completato con successo.</para>
        /// <para><b>Postconditions:</b> Aggiorna lo stato interno tramite <see cref="Touch"/>.</para>
        /// <para><b>Side Effects:</b> La proprietà <c>UpdatedAt</c> viene aggiornata se implementato.</para>
        /// </remarks>
        protected sealed override void OnAfterAddChildFlow(JsonEntity child)
        {
            this.Touch();
        }

        /// <inheritdoc />
        /// <summary>
        /// Metodo hook eseguito automaticamente al termine del flusso di aggiunta di una proprietà (<see cref="JsonProperty"/>).
        /// </summary>
        /// <param name="child">La proprietà appena aggiunta.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il flusso di aggiunta è stato completato con successo.</para>
        /// <para><b>Postconditions:</b> Aggiorna lo stato interno tramite <see cref="Touch"/>.</para>
        /// <para><b>Side Effects:</b> La proprietà <c>UpdatedAt</c> viene aggiornata se implementato.</para>
        /// </remarks>
        protected sealed override void OnAfterAddChildFlow(JsonProperty child)
        {
            this.Touch();
        }
    }
}