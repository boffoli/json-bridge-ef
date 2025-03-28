using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Interfaces.Collections;
using JsonBridgeEF.Common.EfEntities.Managers;

namespace JsonBridgeEF.Common.EfEntities.Collections;

/// <summary>
/// Domain Collection: Collezione specializzata per entità <see cref="IKeyed"/>,
/// con supporto alla marcatura di un singolo elemento come chiave logica.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono derivare da <see cref="BaseEfKeyedEntity{TEntity}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Contenitore coerente di entità nominali persistite, dove solo una può essere marcata come chiave logica.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere inizializzata vuota e popolata con entità già costruite.
/// La marcatura viene gestita automaticamente in <see cref="Add"/> se <c>IsKey</c> è true,
/// oppure esplicitamente tramite <see cref="AddKey"/> o <see cref="MarkAsKey"/>.</para>
///
/// <para><b>Constraints:</b><br/>
/// - I nomi sono univoci (case-insensitive).<br/>
/// - Solo una entità può essere marcata come chiave logica (<c>IsKey = true</c>).</para>
///
/// <para><b>Relationships:</b><br/>
/// - Ogni entità implementa <see cref="IKeyed"/> e <see cref="INamed"/>.<br/>
/// - La chiave corrente è accessibile tramite <see cref="KeyEntity"/>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Il metodo <see cref="Add"/> riconosce automaticamente le entità con <c>IsKey = true</c>.<br/>
/// - Usare <see cref="AddKey"/> o <see cref="MarkAsKey"/> per assegnazioni esplicite.<br/>
/// - Usare <see cref="UnmarkAsKey"/> per rimuovere la chiave corrente.</para>
internal class EfKeyedEntityCollection<TEntity>
    : EfEntityCollection<TEntity>, IEfKeyedEntityCollection<TEntity>
    where TEntity : BaseEfKeyedEntity<TEntity>
{
    private readonly KeyedEntityCollectionManager<TEntity> _keyManager;

    /// <summary>
    /// Inizializza una nuova collezione con gestione automatica della chiave logica.
    /// </summary>
    public EfKeyedEntityCollection()
        : base()
    {
        _keyManager = new KeyedEntityCollectionManager<TEntity>(Entities);
    }

    /// <inheritdoc />
    public TEntity? KeyEntity => _keyManager.KeyEntity;

    /// <inheritdoc />
    public override void Add(TEntity entity)
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