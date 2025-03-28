using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// Domain Class: Rappresenta la definizione di uno schema JSON come aggregate root,
/// contenente una collezione di blocchi strutturati (<see cref="JsonBlock"/>).
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Uno schema JSON composto da uno o più blocchi (<see cref="JsonBlock"/>),
/// ciascuno identificabile per nome e dipendente logicamente dallo schema padre.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere istanziato tramite il metodo statico <see cref="Create"/> con nome e contenuto JSON validi.
/// Il costruttore vuoto è riservato a Entity Framework Core.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nome è obbligatorio e univoco all’interno del contesto di utilizzo.<br/>
/// - Il contenuto JSON deve essere fornito e salvato come testo persistente.<br/>
/// - I blocchi aggiunti devono essere univoci per nome e legati allo schema.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Aggregate root di <see cref="JsonBlock"/> (relazione uno-a-molti).<br/>
/// - Le entità figlie sono gestite tramite <see cref="BaseEfEntityWithOwnedEntities{TSelf, TOwned}"/>.<br/>
/// - Le operazioni CRUD sui blocchi devono passare attraverso <see cref="AddEntity(JsonBlock)"/>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usare <see cref="Create"/> per creare lo schema in modo sicuro.<br/>
/// - Accedere ai blocchi tramite <see cref="Entities"/> o <see cref="OwnedEntities"/>.<br/>
/// - L’aggregate è progettato per la serializzazione e validazione semantica dei dati JSON.</para>
/// </remarks>
public sealed class JsonSchema : BaseEfEntityWithOwnedEntities<JsonSchema, JsonBlock>
{
    // 🔹 COSTRUTTORI 🔹

    /// <summary>
    /// Infrastructure constructor riservato a Entity Framework Core.
    /// </summary>
#pragma warning disable S1133
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    private JsonSchema() : base() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    /// <summary>
    /// Domain constructor privato, usato dalla factory per creare lo schema inizializzato.
    /// </summary>
    private JsonSchema(string name, string jsonContent, IValidateAndFix<JsonSchema>? validator)
        : base(name, validator)
    {
        JsonSchemaContent = jsonContent;
    }

    // 🔹 FACTORY METHOD 🔹

    /// <summary>
    /// Factory method per creare un nuovo schema JSON validato.
    /// </summary>
    /// <param name="name">Nome identificativo dello schema.</param>
    /// <param name="jsonContent">Contenuto JSON da salvare.</param>
    /// <param name="validator">Validatore opzionale per la business logic.</param>
    /// <returns>Nuova istanza di <see cref="JsonSchema"/>.</returns>
    /// <exception cref="ArgumentException">Se nome o contenuto sono vuoti.</exception>
    public static JsonSchema Create(string name, string jsonContent, IValidateAndFix<JsonSchema>? validator = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Il nome dello schema non può essere vuoto.", nameof(name));

        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new ArgumentException("Il contenuto JSON dello schema non può essere vuoto.", nameof(jsonContent));

        return new JsonSchema(name, jsonContent, validator);
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <inheritdoc/>
    protected override bool HasSlug => true;

    // 🔹 PROPRIETÀ PERSISTENTI 🔹

    /// <summary>
    /// Contenuto JSON associato a questo schema (salvato come TEXT).
    /// </summary>
    [Required]
    [Column(TypeName = "TEXT")]
    public string JsonSchemaContent { get; private set; } = string.Empty;

    // 🔹 DOMAIN PROPERTIES 🔹

    /// <summary>
    /// Domain Property: Restituisce i blocchi che risultano "indipendenti",
    /// secondo la logica domain-specific (ad esempio, senza genitore o con chiave definita).
    /// </summary>
    public IEnumerable<JsonBlock> IndependentBlocks
        => OwnedEntities.Where(b => b.IsIndependent());

    /// <summary>
    /// Domain Property: Restituisce i blocchi considerati "dipendenti",
    /// secondo la medesima logica (ad esempio, con genitore definito o senza chiave).
    /// </summary>
    public IEnumerable<JsonBlock> DependentBlocks
        => OwnedEntities.Where(b => !b.IsIndependent());

    // 🔹 VALIDAZIONE 🔹

    /// <inheritdoc/>
    protected override void OnBeforeValidate() { }

    /// <inheritdoc/>
    protected override void OnAfterValidate() { }

    // 🔹 TO STRING 🔹

    /// <inheritdoc/>
    public override string ToString()
        => $"{Name} (Blocks: {Entities.Count})";

}