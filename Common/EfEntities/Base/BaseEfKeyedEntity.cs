using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Entità persistente che può essere designata come chiave logica all’interno di una collezione,
/// con supporto completo per Entity Framework Core.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell'entità derivata (self-reference).</typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Questa classe rappresenta un'entità marcabile come "chiave logica", ovvero l’elemento primario di riferimento in un insieme.
/// La marcatura è gestita dalla collezione dell’owner, non direttamente dall’entità.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creata tramite factory o costruttore protetto, fornendo un nome.
/// Il costruttore vuoto è riservato a Entity Framework Core.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il flag <c>IsKey</c> deve essere modificato esclusivamente tramite metodi espliciti per garantire integrità del dominio.<br/>
/// - L’identità logica è basata sul nome, ma la selezione come chiave è opzionale e mutabile internamente.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Implementa <see cref="IKeyed"/> per indicare il supporto alla marcatura come entità chiave.<br/>
/// - Non assume ownership, ma può essere combinata con entità owned.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Utilizzare quando è necessario evidenziare un elemento "principale" all'interno di una collezione.<br/>
/// - La gestione dell’unicità e dello stato <c>IsKey</c> dovrebbe essere responsabilità della collezione chiamante.</para>
/// </remarks>
public abstract class BaseEfKeyedEntity<TSelf>
    : BaseEfEntity<TSelf>, IKeyed
    where TSelf : BaseEfKeyedEntity<TSelf>
{
    // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹

#pragma warning disable S1133
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfKeyedEntity() : base()
    {
        // ⚠️ EF Core materialization only
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    // 🔹 COSTRUTTORE PROTETTO 🔹

    /// <summary>
    /// Domain Constructor: Inizializza l'entità con nome e validatore opzionale.
    /// </summary>
    /// <param name="name">Nome dell’entità.</param>
    /// <param name="validator">Validatore opzionale per le regole di dominio.</param>
    protected BaseEfKeyedEntity(string name, IValidateAndFix<TSelf>? validator)
        : base(name, validator)
    {
    }

    // 🔹 IMPLEMENTAZIONE IKeyed 🔹

    /// <inheritdoc />
    public bool IsKey { get; private set; }

    /// <inheritdoc />
    public void MarkAsKey()
    {
        IsKey = true;
    }

    /// <inheritdoc />
    public void UnmarkAsKey()
    {
        IsKey = false;
    }
}