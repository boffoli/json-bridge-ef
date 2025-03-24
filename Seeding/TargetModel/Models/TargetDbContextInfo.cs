using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.TargetModel.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Definisce il contesto di database di destinazione all’interno del quale vengono registrate entità e proprietà target.
/// Centralizza il namespace e le definizioni associate a un `DbContext` generato.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// L’istanza deve essere creata tramite il metodo statico <see cref="Create"/>.<br/>
/// I costruttori sono riservati a Entity Framework o interni alla factory.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
/// <item>Il namespace è obbligatorio.</item>
/// <item>Il namespace non può superare i 255 caratteri.</item>
/// </list>
/// </para>
///
/// <para><b>Relationships (EF Core):</b>
/// <list type="bullet">
/// <item>1:N con <see cref="TargetProperty"/> tramite la proprietà <c>TargetProperties</c>.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// L’entità non prevede l’uso di uno slug identificativo automatico.
/// </para>
/// </summary>
internal sealed class TargetDbContextInfo : BaseEfEntity<TargetDbContextInfo>
{
    // ----------------------------
    // 🔹 Constructors (EF + Factory)
    // ----------------------------

    /// <summary>
    /// Costruttore privato riservato a Entity Framework Core.
    /// </summary>
    private TargetDbContextInfo() : base(null) { }

    /// <summary>
    /// Costruttore privato usato internamente dalla factory.
    /// </summary>
    private TargetDbContextInfo(IValidateAndFix<TargetDbContextInfo>? validator) : base(validator) { }

    // ----------------------------
    // 🔹 Factory Method
    // ----------------------------

    /// <summary>
    /// Crea un'istanza di <see cref="TargetDbContextInfo"/> con il namespace specificato.
    /// </summary>
    /// <param name="namespace">Namespace del DbContext di destinazione.</param>
    /// <param name="validator">Validatore opzionale per applicare regole custom.</param>
    public static TargetDbContextInfo Create(string @namespace, IValidateAndFix<TargetDbContextInfo>? validator = null)
    {
        var instance = new TargetDbContextInfo(validator)
        {
            Namespace = @namespace
        };
        return instance;
    }

    // ----------------------------
    // 🔹 Properties
    // ----------------------------

    /// <inheritdoc/>
    protected override bool HasSlug => false;

    /// <summary>
    /// Namespace del DbContext di destinazione.
    /// Esempio: <c>"JsonBridgeEF.Data"</c>.
    /// </summary>
    [Required, MaxLength(255)]
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Collezione delle proprietà definite in questo contesto.
    /// </summary>
    public ICollection<TargetProperty> TargetProperties { get; private set; } = [];

    // ----------------------------
    // 🔹 Validation Hooks
    // ----------------------------

    /// <inheritdoc/>
    protected override void OnBeforeValidate() { }

    /// <inheritdoc/>
    protected override void OnAfterValidate() { }
}