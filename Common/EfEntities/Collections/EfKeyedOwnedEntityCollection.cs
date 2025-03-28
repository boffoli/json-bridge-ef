using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Exceptions;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.EfEntities.Interfaces.Collections;
using JsonBridgeEF.Common.EfEntities.Managers;

namespace JsonBridgeEF.Common.EfEntities.Collections
;
/// <summary>
/// Domain Collection: Collezione specializzata per entità che sono sia <see cref="IKeyed"/> che <see cref="IOwned{TOwner}"/>,
/// con garanzia di unicità per nome, appartenenza all’owner e gestione di una singola chiave logica.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono derivare da <see cref="BaseEfKeyedOwnedEntity{TEntity, TOwner}"/>.
/// </typeparam>
/// <typeparam name="TOwner">
/// Tipo dell’entità proprietaria (aggregate root), che implementa <see cref="IOwner{TEntity}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Collezione coerente di entità possedute che espongono sia un nome univoco sia una chiave logica opzionale.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere istanziata passando il riferimento all’owner.<br/>
/// La marcatura come chiave viene gestita automaticamente in <see cref="Add"/> o esplicitamente tramite <see cref="AddKey"/>.</para>
///
/// <para><b>Constraints:</b><br/>
/// - I nomi sono univoci e case-insensitive all’interno della collezione.<br/>
/// - Le entità devono appartenere all’owner specificato.<br/>
/// - Solo una entità può essere marcata come chiave logica (<c>IsKey = true</c>).</para>
///
/// <para><b>Relationships:</b><br/>
/// - Le entità implementano <see cref="INamed"/>, <see cref="IKeyed"/> e <see cref="IOwned{TOwner}"/>.<br/>
/// - La proprietà <see cref="KeyEntity"/> restituisce la chiave logica attuale.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Il metodo <see cref="Add"/> riconosce entità già marcate come chiave.<br/>
/// - Usare <see cref="AddKey"/> o <see cref="MarkAsKey"/> per forzature o assegnazioni esplicite.<br/>
/// - Usare <see cref="UnmarkAsKey"/> per rimuovere la chiave corrente.</para>
internal class EfKeyedOwnedEntityCollection<TEntity, TOwner> : EfOwnedEntityCollection<TEntity, TOwner>,
    IEfKeyedEntityCollection<TEntity>,
    IEfOwnedEntityCollection<TEntity, TOwner>
    where TEntity : BaseEfKeyedOwnedEntity<TEntity, TOwner>
    where TOwner : class, IOwner<TEntity>
{
    private readonly KeyedEntityCollectionManager<TEntity> _keyManager;

    /// <summary>
    /// Costruttore che inizializza la collezione per un owner specifico.
    /// </summary>
    /// <param name="owner">Owner delle entità contenute.</param>
    public EfKeyedOwnedEntityCollection(TOwner owner)
        : base(owner)
    {
        _keyManager = new KeyedEntityCollectionManager<TEntity>(Entities);
    }

    /// <inheritdoc />
    public TEntity? KeyEntity => _keyManager.KeyEntity;

    /// <inheritdoc />
    public new void Add(TEntity entity)
        => _keyManager.Add(entity, base.Add);

    /// <inheritdoc />
    public void AddKey(TEntity entity, bool force = false)
        => _keyManager.AddKey(entity, force, base.Add);

    /// <inheritdoc />
    public void MarkAsKey(string name, bool force = false)
        => _keyManager.MarkAsKey(name, force, FindByName);

    /// <inheritdoc />
    public bool UnmarkAsKey()
        => _keyManager.UnmarkAsKey();
}