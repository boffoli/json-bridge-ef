namespace JsonBridgeEF.Common.EfEntities.Interfaces.With;

/// <summary>
/// Domain Interface: Rappresenta un contenitore generico di entità del dominio, esponendo una collezione in sola lettura.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, generalmente appartenenti al modello persistente o al grafo dell’aggregate.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Interfaccia di base per modelli che aggregano o gestiscono una collezione di entità del dominio.
/// Non impone vincoli di tipo sulle entità, ma fornisce un punto di estensione comune per interfacce più specifiche.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// È pensata per essere estesa da interfacce specializzate come <c>IWithNamedEntities</c>, <c>IWithOwnedEntities</c>,
/// o <c>IWithKeyedEntities</c>, ognuna delle quali aggiunge vincoli e semantica propri.</para>
///
/// <para><b>Constraints:</b><br/>
/// Nessun vincolo specifico oltre al tipo generico <c>TEntity</c>.</para>
///
/// <para><b>Relationships:</b><br/>
/// Espone una collezione immutabile di entità accessibili dal contenitore o aggregate root che implementa questa interfaccia.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// È utile per uniformare l’accesso a insiemi di entità in contesti generici o riflessivi.<br/>
/// Può essere usata come base per pattern di aggregazione, validazione e tracciamento.</para>
/// </remarks>
public interface IWithEntities<out TEntity>
{
    /// <summary>
    /// Collezione delle entità gestite dal contenitore.
    /// </summary>
    IReadOnlyCollection<TEntity> Entities { get; }
}