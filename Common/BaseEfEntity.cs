using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Common;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Classe base astratta per tutte le entità persistite tramite Entity Framework Core.
/// Rappresenta un'entità con identità, tracciamento temporale, validazione e supporto opzionale allo slug.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Le classi derivate devono essere create tramite factory method statici definiti localmente.
/// Il costruttore senza parametri è riservato alla materializzazione di EF Core.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Deve derivare da <c>BaseEfEntity&lt;TEntity&gt;</c>.</item>
///   <item>La proprietà <c>Name</c> è obbligatoria e lo slug viene generato automaticamente se <c>HasSlug</c> è true.</item>
///   <item>I metodi <c>OnBeforeValidate()</c> e <c>OnAfterValidate()</c> devono essere implementati dalle classi figlie.</item>
/// </list>
/// </para>
///
/// <para><b>Relationships (EF Core):</b>
/// <list type="bullet">
///   <item>È sufficiente assegnare la proprietà di navigazione (<c>MyEntity = ...</c>) per permettere a EF di impostare la relativa foreign key (<c>MyEntityId</c>).</item>
///   <item>Le proprietà che rappresentano foreign key dovrebbero essere <c>private</c> o <c>internal</c> per evitare incoerenze nel dominio.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Pensata per entità aggregate o componenti persistenti nel dominio. Incorpora un sistema di validazione e correzione opzionale.
/// Le classi derivate dovrebbero essere il più possibile coerenti e autovalidanti.
/// </para>
/// </summary>
/// <typeparam name="TEntity">Il tipo concreto dell'entità che eredita da questa base.</typeparam>
/// <remarks>
/// Costruttore protetto con validatore opzionale per i tipi derivati.
/// </remarks>
/// <param name="validator">Validatore opzionale per il ciclo di vita di validazione e correzione.</param>
public abstract partial class BaseEfEntity<TEntity>(IValidateAndFix<TEntity>? validator = null) where TEntity : BaseEfEntity<TEntity>
{
    private readonly IValidateAndFix<TEntity>? _validator = validator;
    private string _name = string.Empty;

    // 🔹 IDENTIFICATORI 🔹

    /// <summary>
    /// Chiave primaria dell'entità, generata automaticamente dal database.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identificatore univoco globale (UUID) per garantire la tracciabilità distribuita.
    /// Generato automaticamente alla creazione.
    /// </summary>
    [Required]
    public Guid UniqueId { get; set; } = Guid.NewGuid();

    // 🔹 METADATI TEMPORALI 🔹

    /// <summary>
    /// Timestamp UTC della creazione dell'entità.
    /// Impostato automaticamente alla creazione.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp UTC dell'ultima modifica dell'entità.
    /// Aggiornato con <see cref="Touch"/> o tramite salvataggio applicativo.
    /// </summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica se l'entità è stata eliminata logicamente (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // 🔹 DATI PRINCIPALI 🔹

    /// <summary>
    /// Nome descrittivo dell'entità.
    /// </summary>
    [Required, MaxLength(255)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            if (HasSlug)
                Slug = GenerateSlug(_name);
        }
    }

    /// <summary>
    /// Slug generato automaticamente dal nome dell'entità, se abilitato.
    /// </summary>
    [MaxLength(255)]
    public virtual string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Determina se lo slug deve essere generato automaticamente dal nome.
    /// Deve essere implementato nella classe derivata.
    /// </summary>
    protected abstract bool HasSlug { get; }

    /// <summary>
    /// Campo testuale opzionale per note, annotazioni o descrizioni aggiuntive.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    // 🔹 METODI DI GESTIONE 🔹

    /// <summary>
    /// Aggiorna il valore di <c>UpdatedAt</c> al tempo UTC corrente.
    /// </summary>
    public void Touch() => UpdatedAt = DateTime.UtcNow;

    /// <summary>
    /// Metodo chiamato prima della validazione.
    /// Deve essere implementato nelle classi derivate, anche se vuoto.
    /// </summary>
    protected abstract void OnBeforeValidate();

    /// <summary>
    /// Metodo chiamato dopo la validazione.
    /// Deve essere implementato nelle classi derivate, anche se vuoto.
    /// </summary>
    protected abstract void OnAfterValidate();

    /// <summary>
    /// Verifica che l'entità sia valida tramite il validatore configurato.
    /// </summary>
    public void EnsureValid()
    {
        OnBeforeValidate();
        _validator?.EnsureValid((TEntity)this);
        OnAfterValidate();
    }

    /// <summary>
    /// Tenta di correggere l'entità utilizzando il validatore.
    /// </summary>
    /// <exception cref="InvalidOperationException">Sollevata se non è disponibile alcun validatore.</exception>
    public void Fix()
    {
        if (_validator is null)
            throw new InvalidOperationException("Nessun validatore disponibile per correggere l'entità.");

        _validator.Fix((TEntity)this);
        Touch();
    }

    /// <summary>
    /// Tenta di validare l'entità; se fallisce, prova a correggerla e la valida nuovamente.
    /// </summary>
    public void TryValidateAndFix()
    {
        try
        {
            EnsureValid();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Validazione fallita: {ex.Message}. Tentativo di correzione...");
            Fix();
            EnsureValid();
        }
    }

    // 🔹 UTILITÀ 🔹

    /// <summary>
    /// Genera uno slug normalizzato da una stringa di input (minuscolo, ASCII-safe, separato da trattini).
    /// </summary>
    /// <param name="input">Testo di partenza da cui generare lo slug.</param>
    /// <returns>Slug generato.</returns>
    protected static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        normalized = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        normalized = MyRegex().Replace(normalized, "-");
        normalized = MyRegex1().Replace(normalized, string.Empty);
        normalized = MyRegex2().Replace(normalized, "-").Trim('-');

        return normalized;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex MyRegex1();
    [GeneratedRegex(@"-+")]
    private static partial Regex MyRegex2();
}