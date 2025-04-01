using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Mapping.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta una proprietà di una classe target C#, con possibilità di essere marcata come chiave logica.
/// Appartiene a una classe aggregata (TargetClass) e descrive un nome, path, classe e contesto EF.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creata tramite factory con owner valido.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nome deve essere univoco all’interno del <see cref="TargetClass"/>.<br/>
/// - Il contesto EF e l'owner sono obbligatori.<br/>
/// - Solo una proprietà per classe può essere marcata come chiave.
/// </para>
///
/// <para><b>Relationships:</b><br/>
/// - Owned da <see cref="TargetClass"/> (aggregate root).<br/>
/// - Relazione N:1 con <see cref="TargetDbContextInfo"/>.<br/>
/// - Relazione 1:N con <see cref="MappingRule"/>.
/// </para>
internal sealed class TargetProperty : BaseEfKeyedOwnedEntity<TargetProperty, TargetClass>
{
    // 🔹 Costruttore EF Core
    [Obsolete("Reserved for EF Core", error: false)]
    private TargetProperty() : base() { }

    // 🔹 Costruttore di dominio
    public TargetProperty(string name, TargetClass owner, TargetDbContextInfo contextInfo, IValidateAndFix<TargetProperty>? validator = null)
        : base(name, owner, validator)
    {
        TargetDbContextInfo = contextInfo;
    }

    // 🔹 Scalar Properties

    [Required, MaxLength(255)]
    public string Namespace { get; private set; } = string.Empty;

    [Required, MaxLength(255)]
    public string RootClass { get; private set; } = string.Empty;

    [MaxLength(500)]
    public string? Path { get; private set; }

    [Required]
    public int TargetDbContextInfoId { get; private set; }

    [ForeignKey(nameof(TargetDbContextInfoId))]
    public TargetDbContextInfo TargetDbContextInfo { get; private set; } = null!;

    // 🔹 Navigation

    public ICollection<MappingRule> MappingRules { get; private set; } = new List<MappingRule>();

    // 🔹 Computed Properties

    public string ClassQualifiedName => $"{Namespace}.{RootClass}";

    public string PropertyQualifiedName =>
        string.IsNullOrWhiteSpace(Path)
            ? $"{RootClass}.{Name}"
            : $"{RootClass}.{Path}.{Name}";

    public string FullyQualifiedPropertyName =>
        string.IsNullOrWhiteSpace(Path)
            ? $"{ClassQualifiedName}.{Name}"
            : $"{ClassQualifiedName}.{Path}.{Name}";

    public string PropertyPathName =>
        string.IsNullOrWhiteSpace(Path) ? Name : $"{Path}.{Name}";
}