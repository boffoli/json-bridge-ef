using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models
{
    /// <summary>
    /// Normalizza il percorso dei campi all’interno del JSON,
    /// garantendo che ogni percorso sia definito una sola volta per schema.
    /// </summary>
    internal class JsonFieldDef : ModelBase<JsonFieldDef>
    {
        /// <summary>
        /// Costruttore richiesto da Entity Framework Core.
        /// </summary>
        internal JsonFieldDef() : base(null) { }

        /// <summary>
        /// Costruttore pubblico che accetta un validatore opzionale.
        /// </summary>
        /// <param name="validator">Il validatore per questa istanza.</param>
        public JsonFieldDef(IValidateAndFix<JsonFieldDef>? validator)
            : base(validator) { }

        /// <summary>
        /// Identificatore univoco del campo JSON.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Chiave esterna allo schema JSON a cui appartiene il campo.
        /// </summary>
        [Required]
        public int JsonSchemaDefId { get; set; }

        /// <summary>
        /// Il percorso univoco del campo nel JSON all’interno di uno specifico schema.
        /// Esempio: "utente.nome" oppure "utente.indirizzo".
        /// </summary>
        [Required]
        public string SourceFieldPath { get; set; } = string.Empty;

        /// <summary>
        /// Note o descrizioni aggiuntive relative al campo
        /// (ad es. formati, restrizioni, ecc.).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indica se il campo è considerato chiave per il mapping.
        /// </summary>
        public bool IsKey { get; set; }
    }
}