using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Exceptions;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Collections;

/// <summary>
/// Domain Collection: Collezione specializzata per entità <see cref="IOwned{TOwner}"/> che garantisce
/// coerenza tra entità e owner e unicità per nome all’interno della collezione.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono derivare da <see cref="BaseEfOwnedEntity{TEntity, TOwner}"/>.
/// </typeparam>
/// <typeparam name="TOwner">
/// Tipo dell'entità proprietaria (aggregate root) che implementa <see cref="IOwner{TEntity}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Collezione specializzata per entità owned che fanno parte di un aggregate.
/// Impone vincoli forti di appartenenza e unicità basata sul nome.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere istanziata passando il riferimento all’owner, che rappresenta il contesto di appartenenza.
/// Le entità vengono registrate tramite il metodo <see cref="Add"/>.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Ogni entità deve essere associata allo stesso owner della collezione.<br/>
/// - I nomi sono univoci e case-insensitive all’interno della collezione.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Ogni <c>TEntity</c> implementa <see cref="INamed"/> e <see cref="IOwned{TOwner}"/>.<br/>
/// - L’owner implementa <see cref="IOwner{TEntity}"/> per registrare le entità.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - L’unicità è gestita tramite <see cref="NamedEntityComparer{TEntity}"/>.<br/>
/// - Le violazioni di coerenza sono gestite tramite eccezioni custom come <see cref="EntityOwnershipMismatchException"/>.</para>
/// </remarks>
internal class EfOwnedEntityCollection<TEntity, TOwner>(TOwner owner)
    : EfEntityCollection<TEntity>() // ✅ NamedEntityComparer è già il default
    where TEntity : BaseEfOwnedEntity<TEntity, TOwner>
    where TOwner : class, IOwner<TEntity>
{
    private readonly TOwner _owner = owner ?? throw new ArgumentNullException(nameof(owner));

    /// <summary>
    /// Aggiunge un’entità alla collezione, verificando l’associazione all’owner e la presenza di nomi duplicati.
    /// </summary>
    /// <param name="entity">Entità da aggiungere.</param>
    /// <exception cref="EntityOwnershipMismatchException">
    /// Se l’entità non è associata all’owner specificato.
    /// </exception>
    /// <exception cref="NamedEntityAlreadyExistsException">
    /// Se esiste già un’entità con lo stesso nome nella collezione.
    /// </exception>
    public new void Add(TEntity entity)
    {
        EnsureEntityBelongsToOwner(entity);
        base.Add(entity);
    }

    /// <summary>
    /// Verifica che l’entità fornita appartenga all’owner della collezione.
    /// </summary>
    /// <param name="entity">Entità da verificare.</param>
    /// <exception cref="EntityOwnershipMismatchException">
    /// Se l’entità è associata a un owner differente.
    /// </exception>
    private void EnsureEntityBelongsToOwner(TEntity entity)
    {
        if (!ReferenceEquals(entity.Owner, _owner))
            throw new EntityOwnershipMismatchException(entity.Name, _owner.ToString() ?? "Unknown");
    }
}