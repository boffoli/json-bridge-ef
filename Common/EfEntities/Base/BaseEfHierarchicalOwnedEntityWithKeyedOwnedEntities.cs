using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Collections;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.EfEntities.Interfaces.With;
using JsonBridgeEF.Common.EfEntities.Managers;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Entità owned che funge da aggregate root locale per entità figlie owned e keyed,
/// e partecipa a una struttura gerarchica padre-figlio con entità dello stesso tipo.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell'entità corrente (self-reference).</typeparam>
/// <typeparam name="TOwner">Tipo dell'entità da cui questa è posseduta.</typeparam>
/// <typeparam name="TEntity">Tipo delle entità figlie owned e keyed aggregate.</typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Questa entità rappresenta un nodo interno di una struttura gerarchica, come un albero o DAG,
/// ed è anche un aggregate root per un insieme coerente di entità figlie owned e keyed.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere inizializzata tramite costruttore con nome, owner e validatore opzionale.
/// Le collezioni interne sono gestite in automatico.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Le entità <typeparamref name="TEntity"/> devono avere nomi univoci e appartenere a questa entità.<br/>
/// - Solo una può essere marcata come chiave logica.<br/>
/// - Le relazioni gerarchiche tra istanze <typeparamref name="TSelf"/> devono essere simmetriche e coerenti.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Questa entità è posseduta da <typeparamref name="TOwner"/>.<br/>
/// - Aggrega entità figlie di tipo <typeparamref name="TEntity"/>.<br/>
/// - Partecipa a una gerarchia ricorsiva con altre entità <typeparamref name="TSelf"/>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usare <see cref="AddEntity"/> per aggiungere entità figlie.<br/>
/// - Usare <see cref="MarkAsKey(TEntity)"/> o <see cref="MarkAsKey(string)"/> per designare la chiave logica.<br/>
/// - Usare <see cref="AddChild"/> e <see cref="AddParent"/> per relazioni gerarchiche.<br/>
/// - Le entità figlie keyed NON sono coinvolte nella gerarchia.</para>
/// </remarks>
public abstract class BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities<TSelf, TOwner, TEntity>
    : BaseEfOwnedEntity<TSelf, TOwner>,
      IWithKeyedEntities<TEntity>,
      IWithOwnedEntities<TSelf, TEntity>,
      IHierarchical<TSelf>
    where TSelf : BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities<TSelf, TOwner, TEntity>, IHierarchical<TSelf>
    where TOwner : IOwner<TSelf>
    where TEntity : BaseEfKeyedOwnedEntity<TEntity, TSelf>
{
    private protected readonly EfKeyedOwnedEntityCollection<TEntity, TSelf> _keyedCollection;
    private readonly HierarchicalEntityManager<TSelf> _hierarchy;

    #region EF Core Constructor

    /// <summary>
    /// Costruttore riservato a EF Core.
    /// </summary>
#pragma warning disable S1133 // Deprecated code should be removed
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities()
    {
        _keyedCollection = new EfKeyedOwnedEntityCollection<TEntity, TSelf>((TSelf)this);
        _hierarchy = new HierarchicalEntityManager<TSelf>((TSelf)this);
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    #endregion

    #region Domain Constructor

    /// <summary>
    /// Domain constructor: inizializza nome, owner e validatore.
    /// </summary>
    protected BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities(string name, TOwner owner, IValidateAndFix<TSelf>? validator)
        : base(name, owner, validator)
    {
        _keyedCollection = new EfKeyedOwnedEntityCollection<TEntity, TSelf>((TSelf)this);
        _hierarchy = new HierarchicalEntityManager<TSelf>((TSelf)this);
    }

    #endregion

    #region Owned + Keyed Entities

    /// <inheritdoc/>
    public IReadOnlyCollection<TEntity> Entities => _keyedCollection.Entities;

    /// <inheritdoc/>
    public TEntity? KeyEntity => _keyedCollection.KeyEntity;

    /// <inheritdoc/>
    public void AddEntity(TEntity entity) => _keyedCollection.Add(entity);

    /// <inheritdoc/>
    public TEntity? GetKeyEntity() => _keyedCollection.KeyEntity;

    #endregion

    #region Hierarchical

    /// <inheritdoc/>
    public IReadOnlyCollection<TSelf> Parents => _hierarchy.Parents;

    /// <inheritdoc/>
    public IReadOnlyCollection<TSelf> Children => _hierarchy.Children;

    /// <inheritdoc/>
    public void AddParent(TSelf parent) => _hierarchy.AddParent(parent);

    /// <inheritdoc/>
    public void AddChild(TSelf child) => _hierarchy.AddChild(child);

    #endregion
}