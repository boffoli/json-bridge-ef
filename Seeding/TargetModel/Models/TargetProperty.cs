using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;
using JsonBridgeEF.Seeding.Mappings.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta una proprietà di destinazione all'interno del modello target.
/// Contiene informazioni su nome, percorso, classe e contesto EF di appartenenza.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Istanza creata tramite il metodo statico <see cref="Create"/> con validazione opzionale.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nome, namespace e classe radice sono obbligatori.<br/>
/// - Il contesto target associato è obbligatorio.<br/>
/// - La proprietà può essere marcata come chiave.
/// </para>
///
/// <para><b>Relationships:</b><br/>
/// - N:1 con <see cref="TargetDbContextInfo"/> (proprietà obbligatoria).<br/>
/// - 1:N con <see cref="MappingRule"/> (regole di mappatura che coinvolgono questa proprietà).
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Espone nomi qualificati utili per la generazione automatica di codice o query.
/// </para>
internal sealed class TargetProperty : BaseEfEntity<TargetProperty>
{
    #region 🔹 Slug

    /// <inheritdoc/>
    protected override bool HasSlug => false;

    #endregion

    #region 🔹 Constructors

    /// <summary>
    /// Costruttore richiesto da Entity Framework Core.
    /// </summary>
    private TargetProperty() : base(null) { }

    /// <summary>
    /// Costruttore privato usato internamente dalla factory.
    /// </summary>
    private TargetProperty(IValidateAndFix<TargetProperty>? validator) : base(validator) { }

    #endregion

    #region 🔹 Factory

    /// <summary>
    /// Crea una nuova istanza di <see cref="TargetProperty"/> inizializzata e validata.
    /// </summary>
    public static TargetProperty Create(
        TargetDbContextInfo contextInfo,
        string name,
        string @namespace,
        string rootClass,
        string? path,
        bool isKey,
        IValidateAndFix<TargetProperty>? validator = null)
    {
        var entity = new TargetProperty(validator)
        {
            TargetDbContextInfo = contextInfo,
            Name = name,
            Namespace = @namespace,
            RootClass = rootClass,
            Path = path,
            IsKey = isKey
        };

        return entity;
    }

    #endregion

    #region 🔹 Scalar Properties

    /// <summary>
    /// Namespace della classe target (es. "JsonBridgeEF.Models.TargetDb").
    /// </summary>
    [Required, MaxLength(255)]
    public string Namespace { get; private set; } = string.Empty;

    /// <summary>
    /// Nome della classe radice a cui appartiene la proprietà (es. "User").
    /// </summary>
    [Required, MaxLength(255)]
    public string RootClass { get; private set; } = string.Empty;

    /// <summary>
    /// Percorso annidato della proprietà (es. "ShippingAddress.City").
    /// Vuoto se la proprietà è diretta sulla root class.
    /// </summary>
    [MaxLength(500)]
    public string? Path { get; private set; }

    /// <summary>
    /// Indica se questa proprietà è una chiave (PK o identificatore logico).
    /// </summary>
    [Required]
    public bool IsKey { get; private set; }

    #endregion

    #region 🔹 Navigation Properties

    /// <summary>
    /// Foreign key verso il contesto di destinazione.
    /// </summary>
    [Required]
    public int TargetDbContextInfoId { get; private set; }

    /// <summary>
    /// Contesto EF target di appartenenza.
    /// </summary>
    [ForeignKey(nameof(TargetDbContextInfoId))]
    public TargetDbContextInfo TargetDbContextInfo { get; private set; } = null!;

    /// <summary>
    /// Regole di mappatura associate a questa proprietà.
    /// </summary>
    public ICollection<MappingRule> MappingRules { get; private set; } = new List<MappingRule>();

    #endregion

    #region 🔹 Computed Properties

    /// <summary>
    /// Nome completamente qualificato della classe (Namespace + RootClass).
    /// </summary>
    public string ClassQualifiedName => $"{Namespace}.{RootClass}";

    /// <summary>
    /// Percorso completo della proprietà (RootClass + Path + Name).
    /// </summary>
    public string PropertyQualifiedName =>
        string.IsNullOrWhiteSpace(Path)
            ? $"{RootClass}.{Name}"
            : $"{RootClass}.{Path}.{Name}";

    /// <summary>
    /// Percorso completo incluso il namespace (Namespace + Class + Path + Name).
    /// </summary>
    public string FullyQualifiedPropertyName =>
        string.IsNullOrWhiteSpace(Path)
            ? $"{ClassQualifiedName}.{Name}"
            : $"{ClassQualifiedName}.{Path}.{Name}";

    /// <summary>
    /// Solo path + nome (es. "ShippingAddress.City.Street").
    /// </summary>
    public string PropertyPathName =>
        string.IsNullOrWhiteSpace(Path) ? Name : $"{Path}.{Name}";

    #endregion

    #region 🔹 Lifecycle Hooks

    /// <inheritdoc/>
    protected override void OnBeforeValidate()
    {
        // Logica da eseguire prima della validazione, se necessaria
    }

    /// <inheritdoc/>
    protected override void OnAfterValidate()
    {
        // Logica da eseguire dopo la validazione, se necessaria
    }

    #endregion
}