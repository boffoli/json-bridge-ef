namespace JsonBridgeEF.Common.EfEntities.Interfaces.Entities
{
    /// <summary>
    /// Domain Concept: Interfaccia che vincola un'entità al suo proprietario, stabilendo una relazione molti-a-uno.
    /// </summary>
    /// <typeparam name="TOwner">Tipo dell'entità proprietaria.</typeparam>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere implementata da entità che rappresentano elementi figli o componenti
    /// strutturalmente dipendenti da un owner. Il riferimento al proprietario viene impostato
    /// nel costruttore e popolato da EF Core durante la materializzazione.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Il riferimento a <c>Owner</c> non può essere null durante l’inizializzazione esplicita.
    /// Il valore <c>OwnerId</c> viene gestito da EF Core come chiave esterna ed è disponibile dopo il salvataggio.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Rappresenta il lato "owned" di una relazione con un owner. L’owner deve implementare
    /// <see cref="IOwner{TEntity}"/> per garantire la simmetria e permettere l’aggiunta strutturata
    /// di oggetti figli.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// È utile per mappare relazioni navigabili e controllate da EF Core. Le implementazioni concrete
    /// dovrebbero garantire che <c>Owner</c> e <c>OwnerId</c> siano coerenti per mantenere l'integrità
    /// del modello ad oggetti e del database.</para>
    /// </remarks>
    public interface IOwned<TOwner>
    {
        /// <summary>
        /// Proprietà che espone il proprietario dell'entità.
        /// </summary>
        TOwner Owner { get; }

        /// <summary>
        /// Identificativo del proprietario (chiave esterna per EF Core).
        /// </summary>
        int OwnerId { get; }
    }
}