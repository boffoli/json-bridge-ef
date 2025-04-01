using JsonBridgeEF.SharedModel.DagModel;

namespace JsonBridgeEF.SharedModel.EntityModel
{
    /// <summary>
    /// Domain Interface: Componente base del modello a entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Componente astratto di base per entità e proprietà nel grafo del modello a entità.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato da entità (aggregato) e proprietà (foglia) che partecipano alla struttura a grafo.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessuno specifico, funge da radice comune.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="INode"/> per garantire l’identificazione nominale nel grafo.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utile come punto comune per funzioni generiche su elementi del modello.</para>
    /// </remarks>
    public interface IComponentModel : INode
    {
    }

    /// <summary>
    /// Domain Interface: Nodo tipizzato nel contesto del modello a entità.
    /// </summary>
    /// <typeparam name="TEntity">Tipo dell'entità aggregata.</typeparam>
    /// <typeparam name="TEntityProperty">Tipo della proprietà foglia.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Estende <see cref="INode"/> aggiungendo due parametri generici per definire il contesto del modello a entità (entità/proprietà).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Interfaccia marker, implementata implicitamente dai componenti concreti.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessuno diretto, dipende dai vincoli delle interfacce aggregate.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IComponentModel"/> e <see cref="INode{TEntity, TEntityProperty}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Implementato da componenti che necessitano di conoscere la tipizzazione degli aggregati e delle foglie.</para>
    /// </remarks>
    public interface IComponent<TEntity, TEntityProperty> : IComponentModel, INode<TEntity, TEntityProperty>
        where TEntity : class, IEntity<TEntity, TEntityProperty>
        where TEntityProperty : class, IEntityProperty<TEntityProperty, TEntity>
    {
    }

    /// <summary>
    /// Domain Interface: Rappresenta un'entità che possiede proprietà e partecipa a una struttura gerarchica.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto dell'entità.</typeparam>
    /// <typeparam name="TEntityProperty">Il tipo di proprietà contenute.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo aggregato che contiene proprietà e può essere connesso ad altre entità come parte di un grafo orientato.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere creato tramite factory o costruttore con nome valido. Le proprietà vanno aggiunte esplicitamente.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Le proprietà devono avere nomi univoci.<br/>
    /// - Solo una proprietà può essere contrassegnata come chiave.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TEntityProperty"/>.<br/>
    /// - Può essere connessa gerarchicamente ad altre entità.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Interfaccia base per strutture JSON o classi OO.<br/>
    /// La proprietà <c>Properties</c> è un alias semantico per <c>ValueChildren</c>.</para>
    /// </remarks>
    public interface IEntity<TSelf, TEntityProperty> :
        IComponent<TSelf, TEntityProperty>,
        IAggregateNode<TSelf, TEntityProperty>
        where TSelf : class, IEntity<TSelf, TEntityProperty>
        where TEntityProperty : class, IEntityProperty<TEntityProperty, TSelf>
    {
        /// <summary>
        /// Collezione delle proprietà associate all'entità.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Accesso semantico alle proprietà come foglie del grafo.<br/>
        /// <b>Access:</b> Sola lettura.<br/>
        /// <b>Note:</b> Alias per <see cref="IAggregateNode{TSelf,TEntityProperty}.ValueChildren"/>.
        /// </remarks>
        IReadOnlyCollection<TEntityProperty> Properties { get; }

        /// <summary>
        /// Verifica se l'entità ha una proprietà chiave designata.
        /// </summary>
        /// <returns><c>true</c> se è presente una proprietà chiave; <c>false</c> altrimenti.</returns>
        bool IsIdentifiable();

        /// <summary>
        /// Restituisce la proprietà attualmente designata come chiave, se esistente.
        /// </summary>
        /// <returns>La proprietà chiave oppure <c>null</c> se non impostata.</returns>
        TEntityProperty? GetKeyProperty();
    }

    /// <summary>
    /// Domain Interface: Rappresenta una proprietà che appartiene a un'entità.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto della proprietà.</typeparam>
    /// <typeparam name="TEntity">Il tipo dell'entità a cui appartiene.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo foglia che rappresenta una proprietà logica appartenente a un'entità aggregata.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Creato con nome valido, assegnato a una entità tramite <c>AddProperty</c>.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Deve appartenere a una singola entità.<br/>
    /// - Può essere marcata come chiave una sola volta all’interno dell’entità.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a un'entità che implementa <typeparamref name="TEntity"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Non può avere figli. Funziona come terminale semantico della struttura entità.</para>
    /// </remarks>
    public interface IEntityProperty<TSelf, TEntity> :
        IComponent<TEntity, TSelf>,
        IValueNode<TSelf, TEntity>
        where TSelf : class, IEntityProperty<TSelf, TEntity>
        where TEntity : class, IEntity<TEntity, TSelf>
    {
        /// <summary>
        /// Indica se questa proprietà è la chiave logica dell'entità a cui appartiene.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Determina l’identificabilità logica dell'entità.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        bool IsKey { get; }
    }
}
