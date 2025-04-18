using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Entità owned e marcabile come chiave logica, con supporto Entity Framework Core.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell'entità (self-reference).</typeparam>
/// <typeparam name="TOwner">Tipo dell'aggregate root che possiede questa entità.</typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Entità di dominio che appartiene a un aggregate root e può essere identificata come "chiave logica"
/// tra le entità possedute, utile in configurazioni, mapping, alias.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Istanzia tramite costruttore protetto, specificando nome e owner. L'inserimento nell'owner è automatico.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nome deve essere univoco all’interno dell’owner.<br/>
/// - Solo una entità per owner può essere marcata come <c>IsKey = true</c>.</para>
///
/// <para><b>Relationships:</b><br/>
/// - L’owner deve implementare <see cref="IOwner{TSelf}"/>.<br/>
/// - La marcatura come chiave è gestita tramite <see cref="MarkAsKey"/>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - È responsabilità dell’owner assicurarsi che <c>IsKey</c> sia unico.<br/>
/// - Supporta <see cref="IKeyed"/> per compatibilità con lookup logici.</para>
/// </remarks>
public abstract class BaseEfKeyedOwnedEntity<TSelf, TOwner>
    : BaseEfOwnedEntity<TSelf, TOwner>, IKeyed
    where TSelf : BaseEfKeyedOwnedEntity<TSelf, TOwner>
    where TOwner : IOwner<TSelf>
{
    // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹
#pragma warning disable S1133
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfKeyedOwnedEntity() : base()
    {
        // EF-only
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    // 🔹 COSTRUTTORE PROTETTO CON OWNER 🔹

    /// <summary>
    /// Costruttore protetto con nome e owner.
    /// </summary>
    /// <param name="name">Nome descrittivo.</param>
    /// <param name="owner">Entità owner (aggregate root).</param>
    /// <param name="validator">Validatore opzionale.</param>
    protected BaseEfKeyedOwnedEntity(string name, TOwner owner, IValidateAndFix<TSelf>? validator)
        : base(name, owner, validator)
    {
    }

    // 🔹 IMPLEMENTAZIONE IKeyed 🔹

    /// <inheritdoc />
    public bool IsKey { get; private set; }

    /// <inheritdoc />
    public void MarkAsKey() => IsKey = true;

    /// <inheritdoc />
    public void UnmarkAsKey() => IsKey = false;
}