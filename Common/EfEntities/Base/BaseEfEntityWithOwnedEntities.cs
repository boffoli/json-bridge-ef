using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Collections;
using JsonBridgeEF.Common.EfEntities.Interfaces.With;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Aggregate root persistente con gestione di una collezione di entità owned,
/// che implementa <see cref="IWithOwnedEntities{TSelf, TOwned}"/> per supportare la relazione uno-a-molti.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell'entità aggregante (self-reference).</typeparam>
/// <typeparam name="TOwned">
/// Tipo delle entità possedute, che devono derivare da <see cref="BaseEfOwnedEntity{TOwned, TSelf}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta un aggregate root con entità figlie strutturalmente dipendenti e collegate
/// tramite relazione uno-a-molti (owned). Il root è responsabile della gestione e coerenza delle entità figlie.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Le entità figlie vengono istanziate tramite factory o costruttore, passando l'owner come parametro.
/// La collezione viene inizializzata automaticamente e garantisce unicità e integrità dei riferimenti.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Le entità devono essere univoche per nome (case-insensitive).<br/>
/// - Ogni entità deve essere posseduta da questo aggregate (verificato internamente).</para>
///
/// <para><b>Relationships:</b><br/>
/// - Implementa <see cref="IWithOwnedEntities{TSelf, TOwned}"/>, che estende <see cref="IOwner{TOwned}"/> e <see cref="IWithEntities{TOwned}"/>.<br/>
/// - Contiene una <see cref="EfOwnedEntityCollection{TOwned, TSelf}"/> per la gestione delle entità figlie.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usare <see cref="AddEntity(TOwned)"/> per aggiungere nuove entità figlie in modo sicuro.<br/>
/// - Le entità sono accessibili tramite <see cref="Entities"/> o <see cref="OwnedEntities"/>.</para>
/// </remarks>
public abstract class BaseEfEntityWithOwnedEntities<TSelf, TOwned>
    : BaseEfEntity<TSelf>, IWithOwnedEntities<TSelf, TOwned>
    where TSelf : BaseEfEntityWithOwnedEntities<TSelf, TOwned>
    where TOwned : BaseEfOwnedEntity<TOwned, TSelf>
{
    private readonly EfOwnedEntityCollection<TOwned, TSelf> _collection;

    #region EF Core Constructor

    /// <summary>
    /// Constructor riservato a EF Core. Inizializza la collezione interna delle entità possedute.
    /// </summary>
#pragma warning disable S1133 // deprecated
    [Obsolete("Reserved for EF Core materialization only", error: false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS8618
    protected BaseEfEntityWithOwnedEntities()
    {
        _collection = new EfOwnedEntityCollection<TOwned, TSelf>((TSelf)this);
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    #endregion

    #region Domain Constructor

    /// <summary>
    /// Domain Constructor: Inizializza l'entità con un nome e un validatore opzionale,
    /// e prepara la collezione di entità owned.
    /// </summary>
    /// <param name="name">Nome obbligatorio per l'entità.</param>
    /// <param name="validator">Validatore opzionale per le regole di dominio.</param>
    protected BaseEfEntityWithOwnedEntities(string name, IValidateAndFix<TSelf>? validator = null)
        : base(name, validator)
    {
        _collection = new EfOwnedEntityCollection<TOwned, TSelf>((TSelf)this);
    }

    #endregion

    #region Owned Entities Collection

    /// <summary>
    /// Collezione in sola lettura delle entità owned appartenenti a questo aggregate.
    /// Alias semanticamente esplicito per <see cref="Entities"/>.
    /// </summary>
    public IReadOnlyCollection<TOwned> OwnedEntities => _collection.Entities;

    /// <inheritdoc cref="IWithEntities{TEntity}.Entities"/>
    public IReadOnlyCollection<TOwned> Entities => _collection.Entities;

    /// <inheritdoc cref="IWithEntities{TEntity}.AddEntity(TEntity)"/>
    public void AddEntity(TOwned entity)
    {
        _collection.Add(entity);
    }

    #endregion
}