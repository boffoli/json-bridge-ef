using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Model;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Validators;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Validators
{
    /// <summary>
    /// Domain Concept: Validator per <see cref="JsonEntity"/> che garantisce la correttezza semantica del blocco JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Invocato dai servizi di seeding per validare il modello prima della persistenza.</para>
    /// <para><b>Constraints:</b><br/>
    /// Controlla il riferimento allo schema JSON e la validità della descrizione testuale.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="EntityValidator{JsonEntity, JsonProperty}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// La validazione di figli, chiave e gerarchia parentale è già gestita alla base.</para>
    /// </remarks>
    internal sealed class JsonEntityValidator
        : EntityValidator<JsonEntity, JsonProperty>,
          IValidateAndFix<JsonEntity>
    {
        private const int MaxDescriptionLength = 1000;

        /// <inheritdoc/>
        public void EnsureValid(JsonEntity model)
        {
            // validazione di base (figli, chiave, integrità parent‐child)
            base.EnsureValid(model);

            // validazione specifica: lo schema non può essere null
            ValidateSchema(model.Schema, model.Name);

            // validazione aggiuntiva: descrizione non troppo lunga
            ValidateDescription(model.Description, model.Name);
        }

        /// <inheritdoc/>
        public void Fix(JsonEntity model)
        {
            base.Fix(model);
            model.Description = FixDescription(model.Description);
        }

        /// <summary>
        /// Verifica che l'entità abbia uno schema JSON associato.
        /// </summary>
        private static void ValidateSchema(
            IJsonSchema<JsonEntity>? schema,
            string jsonEntityName)
        {
            if (schema is null)
                throw JsonEntityError.MissingSchemaReference(jsonEntityName);
        }

        /// <summary>
        /// Verifica che la descrizione non ecceda la lunghezza massima consentita.
        /// </summary>
        private static void ValidateDescription(string? description, string jsonEntityName)
        {
            if (description?.Length > MaxDescriptionLength)
                throw JsonEntityError.DescriptionTooLong(jsonEntityName, MaxDescriptionLength);
        }

        /// <summary>
        /// Normalizza la descrizione tornando string.Empty se era null.
        /// </summary>
        private static string FixDescription(string? description)
            => description ?? string.Empty;
    }
}