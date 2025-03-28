namespace JsonBridgeEF.Common.EfEntities.Interfaces.Entities
{
    /// <summary>
    /// Domain Concept: Interfaccia che rappresenta un'entità in grado di contenere e gestire elementi figli.
    /// </summary>
    /// <typeparam name="TEntity">Tipo degli elementi figli associati all'entità proprietaria.</typeparam>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere implementata da ogni entità che accetta il possesso logico di uno o più elementi figli,
    /// gestiti tramite una relazione uno-a-molti.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Ogni entity aggiunto deve essere valido e coerente con le regole di appartenenza definite
    /// nell’entità concreta che implementa l’interfaccia.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Rappresenta il lato "owner" di una relazione di contenimento. Gli oggetti figli dovrebbero implementare
    /// <see cref="IOwned{TOwner}"/> per completare la relazione bidirezionale.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Può essere estesa da interfacce più specifiche come <c>IContainer</c>, <c>IWithKeyedEntities</c>, ecc.
    /// Il metodo <see cref="AddEntity"/> deve garantire integrità e consistenza della struttura dell’aggregato.</para>
    /// </remarks>
    public interface IOwner<TEntity>
    {
        /// <summary>
        /// Aggiunge un elemento all’entità proprietaria, stabilendo la relazione logica.
        /// </summary>
        /// <param name="entity">Elemento da associare all’owner.</param>
        void AddEntity(TEntity entity);
    }
}