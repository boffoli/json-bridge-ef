using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.With;

/// <summary>
/// Domain Interface: Rappresenta un contenitore che possiede entità "owned" in una relazione molti-a-uno,
/// implementando esplicitamente i contratti di <see cref="IOwner{TEntity}"/> e <see cref="IWithEntities{TEntity}"/>.
/// </summary>
/// <typeparam name="TOwner">
/// Tipo del contenitore stesso, che implementa <c>IWithOwnedEntities</c> e funge da proprietario delle entità.
/// </typeparam>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono implementare <see cref="IOwned{TOwner}"/> e mantenere un riferimento al proprietario.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Modella una relazione di *strong ownership* tra un aggregate root e le sue entità figlie.
/// Ogni entità "owned" è parte integrante del ciclo di vita del contenitore e non può esistere autonomamente.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Il contenitore implementa questa interfaccia per abilitare la gestione strutturata delle entità figlie.
/// L’inserimento avviene tramite costruttori o factory method che garantiscono l’assegnazione corretta dell’owner.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Ogni <c>TEntity</c> deve avere un riferimento forte e non nullo al proprio <c>TOwner</c>.<br/>
/// - Solo l’owner può aggiungere, mantenere o validare la coerenza delle entità possedute.<br/>
/// - Le entità non devono essere condivise tra owner diversi.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Estende <see cref="IOwner{TEntity}"/> per supportare i contratti di ownership generici.<br/>
/// - Estende <see cref="IWithEntities{TEntity}"/> per accedere al contenuto in sola lettura.<br/>
/// - Ogni <c>TEntity</c> implementa <see cref="IOwned{TOwner}"/> per referenziare il contenitore.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Utilizzare in contesti DDD per esprimere contenimento strutturale, configurazioni, parametri o sezioni.<br/>
/// - Il contenitore è responsabile del ciclo di vita e della coerenza logica delle sue entità figlie.</para>
/// </remarks>
public interface IWithOwnedEntities<TOwner, TEntity> 
    : IOwner<TEntity>, IWithEntities<TEntity>
    where TOwner : IWithOwnedEntities<TOwner, TEntity>
    where TEntity : IOwned<TOwner>
{
}