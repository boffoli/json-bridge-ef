using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Interface: Rappresenta un’entità che espone un nome descrittivo univoco all’interno di un contesto.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Il nome è utilizzato come identificatore leggibile e potenzialmente anche come chiave logica per lookup o ordinamenti.</para>
/// <para><b>Usage Notes:</b><br/>
/// - Di norma viene implementato da tutte le entità di dominio persistenti.<br/>
/// - Combinabile con interfacce come <c>IKeyed</c> e <c>IOwned</c> per raffinare il comportamento.</para>
/// </remarks>
public interface INamed
{
    /// <summary>
    /// Nome descrittivo dell’entità.
    /// </summary>
    string Name { get; }
}

/// <summary>
/// Domain Concept: Classe base astratta per tutte le entità persistite tramite Entity Framework Core.
/// Rappresenta un'entità con identità, tracciamento temporale, validazione e supporto opzionale allo slug.
/// </summary>
/// <remarks>
/// <para><b>Creation Strategy:</b><br/>
/// Le classi derivate devono essere create tramite factory method statici. Il costruttore vuoto è riservato a EF Core.</para>
/// <para><b>Constraints:</b><br/>
/// Il nome è obbligatorio. Lo slug viene generato se <c>HasSlug</c> è <c>true</c>. Il validatore è opzionale.</para>
/// <para><b>Relationships:</b><br/>
/// Può essere estesa da qualsiasi entità persistente. Le relazioni EF sono gestite tramite proprietà di navigazione e chiavi esterne private.</para>
/// <para><b>Usage Notes:</b><br/>
/// Fornisce metodi di validazione e correzione. Le entità figlie devono implementare <c>OnBeforeValidate()</c> e <c>OnAfterValidate()</c>.</para>
/// </remarks>
public abstract partial class BaseEfEntity<TSelf> : INamed
    where TSelf : BaseEfEntity<TSelf>
{
    private readonly IValidateAndFix<TSelf>? _validator;
    private string _name;

    // 🔹 COSTRUTTORE 🔹

    /// <summary>
    /// Domain Concept: Costruttore riservato a Entity Framework Core per la materializzazione dell'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Non deve essere invocato direttamente nel dominio. Usato esclusivamente da EF.</para>
    /// <para><b>Constraints:</b> Il campo <c>_name</c> viene popolato da EF. Nessuna logica applicativa eseguita.</para>
    /// </remarks>
#pragma warning disable S1133 // Deprecated code should be removed
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfEntity()
    {
        // ⚠️ EF Core materialization only
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    /// <summary>
    /// Domain Concept: Costruttore protetto per inizializzare un'entità con nome e validatore opzionale.
    /// </summary>
    /// <param name="name">Nome obbligatorio dell'entità.</param>
    /// <param name="validator">Validatore opzionale per garantire la correttezza del dominio.</param>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Deve essere invocato da factory o costruttori protetti delle entità figlie.</para>
    /// <para><b>Constraints:</b> Il nome deve essere non nullo e non vuoto. Lo slug è generato se <c>HasSlug</c> è attivo.</para>
    /// </remarks>
    protected BaseEfEntity(string name, IValidateAndFix<TSelf>? validator)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _validator = validator;
        _name = name;

        if (HasSlug)
            Slug = GenerateSlug(_name);
    }

    // 🔹 IDENTIFICATORI 🔹

    /// <summary>
    /// Identificatore numerico primario generato automaticamente da EF Core.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificatore univoco globale assegnato al momento della creazione.
    /// </summary>
    public Guid UniqueId { get; set; } = Guid.NewGuid();

    // 🔹 METADATI TEMPORALI 🔹

    /// <summary>
    /// Timestamp di creazione.
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp dell'ultima modifica.
    /// </summary>
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Flag per la cancellazione logica.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // 🔹 DATI PRINCIPALI 🔹

    /// <summary>
    /// Nome descrittivo dell'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Constraints:</b> Obbligatorio, max 255 caratteri. Modificarlo rigenera lo slug se <c>HasSlug</c> è attivo.</para>
    /// </remarks>
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
    /// Slug derivato dal nome, usato per identificazioni SEO-friendly o confronti.
    /// </summary>
    /// <remarks>
    /// <para><b>Usage Notes:</b> Generato automaticamente se <c>HasSlug</c> è true. Non modificabile manualmente.</para>
    /// </remarks>
    [MaxLength(255)]
    public virtual string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Indica se la generazione automatica dello slug è attiva.
    /// </summary>
    protected abstract bool HasSlug { get; }

    /// <summary>
    /// Descrizione opzionale dell'entità.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    // 🔹 METODI DI GESTIONE 🔹

    /// <summary>
    /// Aggiorna il campo <see cref="UpdatedAt"/> al timestamp corrente.
    /// </summary>
    public void Touch() => UpdatedAt = DateTime.UtcNow;

    /// <summary>
    /// Metodo di hook chiamato prima della validazione.
    /// </summary>
    protected abstract void OnBeforeValidate();

    /// <summary>
    /// Metodo di hook chiamato dopo la validazione.
    /// </summary>
    protected abstract void OnAfterValidate();

    /// <summary>
    /// Esegue la validazione dell'entità. Solleva eccezione se non valida.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> L'entità deve essere inizializzata.</para>
    /// <para><b>Postconditions:</b> L'entità è valida oppure viene sollevata un'eccezione.</para>
    /// </remarks>
    public void EnsureValid()
    {
        OnBeforeValidate();
        _validator?.EnsureValid((TSelf)this);
        OnAfterValidate();
    }

    /// <summary>
    /// Applica correzioni automatiche all'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> Richiede che il validatore sia disponibile.</para>
    /// <para><b>Side Effects:</b> Modifica lo stato dell'entità e aggiorna <see cref="UpdatedAt"/>.</para>
    /// </remarks>
    public void Fix()
    {
        if (_validator is null)
            throw new InvalidOperationException("Nessun validatore disponibile per correggere l'entità.");

        _validator.Fix((TSelf)this);
        Touch();
    }

    /// <summary>
    /// Esegue la validazione con fallback automatico alla correzione se necessario.
    /// </summary>
    /// <remarks>
    /// <para><b>Usage Notes:</b> Utile in scenari interattivi o di importazione dati. Logga eventuali eccezioni su console.</para>
    /// </remarks>
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
    /// Genera uno slug da una stringa di input, normalizzando e filtrando caratteri non validi.
    /// </summary>
    /// <param name="input">Testo di partenza per la generazione dello slug.</param>
    /// <returns>Slug normalizzato, pronto per uso identificativo.</returns>
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