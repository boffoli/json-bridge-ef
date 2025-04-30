using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Model;
using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Model;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Model
{
    /// <inheritdoc cref="IMappingRule"/>
    internal sealed partial class MappingRule
    {
        #region Constructor

        /// <summary>
        /// Costruttore completo: inizializza tutti gli attributi fondamentali e valida il modello.
        /// </summary>
        /// <param name="description">Descrizione della regola di mapping (usata anche per generare lo slug).</param>
        /// <param name="jsonEntityId">ID dell'entità JSON sorgente.</param>
        /// <param name="jsonPropertyId">ID della proprietà JSON sorgente.</param>
        /// <param name="classInfoId">ID della classe target.</param>
        /// <param name="classPropertyId">ID della proprietà target.</param>
        /// <param name="jsFormula">Formula JavaScript di trasformazione/validazione (facoltativa, default = identity).</param>
        /// <param name="validator">Validatore opzionale da applicare all'istanza dopo l'inizializzazione.</param>
        public MappingRule(
            string description,
            int jsonEntityId,
            int jsonPropertyId,
            int classInfoId,
            int classPropertyId,
            string? jsFormula = null,
            IValidateAndFix<MappingRule>? validator = null)
        {
            JsonEntityId = jsonEntityId;
            JsonPropertyId = jsonPropertyId;
            ClassInfoId = classInfoId;
            ClassPropertyId = classPropertyId;
            JsFormula = jsFormula ?? "function transform(value) { return value; }";

            _metadata = new DomainMetadata(description);

            validator?.EnsureValid(this);
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Navigazione alla configurazione principale.
        /// </summary>
        public MappingConfiguration MappingConfiguration { get; private set; } = null!;

        /// <summary>
        /// Navigazione all'entità JSON di origine.
        /// </summary>
        public JsonEntity JsonEntity { get; private set; } = null!;

        /// <summary>
        /// Navigazione alla proprietà JSON di origine.
        /// </summary>
        public JsonProperty JsonProperty { get; private set; } = null!;

        /// <summary>
        /// Navigazione alla classe target.
        /// </summary>
        public ClassInfo ClassInfo { get; private set; } = null!;

        /// <summary>
        /// Navigazione alla proprietà della classe target.
        /// </summary>
        public ClassProperty ClassProperty { get; private set; } = null!;

        #endregion

        #region Mapping Rule Specifics

        /// <summary>
        /// Formula JavaScript applicata alla trasformazione o validazione della proprietà.
        /// Se non specificato, si applica la funzione identità.
        /// </summary>
        [Required, MaxLength(1000)]
        public string JsFormula { get; internal set; }

        #endregion
    }
}