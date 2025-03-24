using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Campo JSON che rappresenta una singola proprietà di un oggetto JSON,
/// collegato a un blocco (<see cref="JsonBlock"/>). Può fungere da chiave logica.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creato tramite il metodo statico <see cref="Create"/>.  
/// I costruttori sono privati o riservati a EF Core.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item><see cref="SourceFieldPath"/> è obbligatorio e non può essere vuoto.</item>
///   <item>Ogni campo può appartenere a un solo blocco e non può essere riassegnato.</item>
/// </list>
/// </para>
///
/// <para><b>Relationships:</b>
/// <list type="bullet">
///   <item>Relazione molti-a-uno con <see cref="JsonBlock"/>.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Il factory method <see cref="Create"/> inizializza il campo e lo registra nel blocco di destinazione.
/// EF Core gestisce automaticamente la foreign key tramite la proprietà <see cref="JsonBlock"/>.
/// </para>
public sealed class JsonField : BaseEfEntity<JsonField>
{
    // 🔹 COSTRUTTORI 🔹

    /// <summary>
    /// Costruttore richiesto da Entity Framework Core.
    /// Non deve essere rimosso.
    /// </summary>
    private JsonField() : base() { }

    /// <summary>
    /// Costruttore privato usato dalla factory <see cref="Create"/>.
    /// Inizializza solo il validatore di base.
    /// </summary>
    private JsonField(IValidateAndFix<JsonField>? validator)
        : base(validator) { }

    // 🔹 FACTORY 🔹

    /// <summary>
    /// Crea un nuovo campo JSON e lo collega al blocco specificato.
    /// </summary>
    /// <param name="block">Blocco JSON a cui appartiene. Non può essere null.</param>
    /// <param name="sourceFieldPath">Percorso logico del campo (es. "nome", "indirizzo.via").</param>
    /// <param name="validator">Validatore opzionale da applicare al campo.</param>
    /// <returns>Nuova istanza del campo JSON, già registrata nel blocco.</returns>
    public static JsonField Create(JsonBlock block, string sourceFieldPath, IValidateAndFix<JsonField>? validator = null)
    {
        ArgumentNullException.ThrowIfNull(block);

        if (string.IsNullOrWhiteSpace(sourceFieldPath))
            throw new ArgumentException("Il percorso del campo non può essere vuoto.", nameof(sourceFieldPath));

        var field = new JsonField(validator)
        {
            SourceFieldPath = sourceFieldPath,
            JsonBlock = block
        };

        block.AddField(field); // Tenta la registrazione: se fallisce, solleva eccezione e annulla la creazione
        return field;
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <inheritdoc/>
    protected override bool HasSlug => false;

    // 🔹 RELAZIONI EF 🔹

    [Required]
    internal int JsonBlockId { get; }

    /// <summary>
    /// Blocco JSON a cui questo campo appartiene.
    /// Impostato una sola volta dalla factory.
    /// </summary>
    [Required]
    public JsonBlock JsonBlock { get; private init; } = null!;

    // 🔹 PROPRIETÀ 🔹

    /// <summary>
    /// Percorso logico del campo all’interno del JSON.
    /// Es.: "nome", "indirizzo.via", "contatti.telefono".
    /// </summary>
    [Required, MaxLength(500)]
    public string SourceFieldPath { get; private init; } = string.Empty;

    /// <summary>
    /// Indica se il campo è marcato come chiave logica per l’importazione.
    /// Può essere impostato solo dal blocco proprietario.
    /// </summary>
    public bool IsKey { get; internal set; }

    // 🔹 VALIDAZIONE 🔹

    protected override void OnBeforeValidate() { }

    protected override void OnAfterValidate() { }
}