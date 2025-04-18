using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Shared.Dag.Interfaces
{
    /// <summary>
    /// Domain Interface: Nodo identificabile in una struttura a grafo.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta l'unità base astratta per ogni componente del grafo, identificato da un nome univoco.</para>
    /// <para><b>Invariants:</b><br/>
    /// - Il nome (<c>Name</c>) è obbligatorio, non nullo, non vuoto e immutabile dopo la costruzione.</para>
    /// </remarks>
    public interface INode
    {
        /// <summary>
        /// Nome univoco del nodo, assegnato in fase di costruzione e non modificabile.
        /// </summary>
        [Required]
        string Name { get; }
    }

    /// <summary>
    /// Domain Interface: Nodo aggregato in un grafo.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto dell'aggregato.</typeparam>
    /// <typeparam name="TValue">Il tipo dei nodi foglia contenuti.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo composito che può contenere altri nodi aggregati e nodi foglia, formando una struttura gerarchica aciclica.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Il nodo aggregato viene creato con un nome valido ed unico. I figli vengono aggiunti esplicitamente tramite i metodi <c>AddChild</c>.</para>
    /// <para><b>Invariants:</b><br/>
    /// - Le collezioni <c>SelfChildren</c> e <c>ValueChildren</c> devono essere non <c>null</c> e consistenti (nessun duplicato, nessun riferimento al nodo stesso).<br/>
    /// - L'aggiunta dei figli deve garantire che la struttura rimanga aciclica.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega nodi aggregati e nodi foglia, in una navigazione strettamente discendente.</para>
    /// </remarks>
    public interface IAggregateNode<TSelf, TValue> : INode
        where TSelf  : class, IAggregateNode<TSelf, TValue>
        where TValue: class, IValueNode<TValue, TSelf>
    {
        /// <summary>
        /// Collezione in sola lettura dei nodi aggregati figli.
        /// </summary>
        IReadOnlyCollection<TSelf> SelfChildren { get; }

        /// <summary>
        /// Collezione in sola lettura dei nodi foglia figli.
        /// </summary>
        IReadOnlyCollection<TValue> ValueChildren { get; }

        /// <summary>
        /// Aggiunge un nodo aggregato figlio alla struttura.
        /// </summary>
        /// <param name="child">Nodo aggregato da aggiungere.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b><br/>
        /// - Il nodo <paramref name="child"/> non può essere <c>null</c>.<br/>
        /// - Non può essere lo stesso nodo (auto‑riferimento non ammesso).<br/>
        /// - L'aggiunta non deve creare cicli nella struttura.</para>
        /// <para><b>Postconditions:</b><br/>
        /// - Il nodo viene aggiunto a <c>SelfChildren</c> se supera le validazioni.</para>
        /// <para><b>Side Effects:</b><br/>
        /// - Potrebbero essere lanciate eccezioni in caso di violazioni dei vincoli.</para>
        /// </remarks>
        void AddChild(TSelf child);

        /// <summary>
        /// Aggiunge un nodo foglia figlio alla struttura.
        /// </summary>
        /// <param name="child">Nodo foglia da aggiungere.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b><br/>
        /// - Il nodo <paramref name="child"/> non può essere <c>null</c>.<br/>
        /// - Deve essere costruito con <c>this</c> come nodo aggregato parent.</para>
        /// <para><b>Postconditions:</b><br/>
        /// - Il nodo viene aggiunto a <c>ValueChildren</c> se supera le validazioni.</para>
        /// <para><b>Side Effects:</b><br/>
        /// - Potrebbero essere lanciate eccezioni in caso di violazioni dei vincoli di associazione.</para>
        /// </remarks>
        void AddChild(TValue child);
    }

    /// <summary>
    /// Domain Interface: Nodo terminale (foglia) in un grafo.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto del nodo foglia.</typeparam>
    /// <typeparam name="TAggregate">Il tipo del nodo aggregato proprietario.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo terminale che non può contenere altri nodi ed è associato in modo immutabile a un aggregato.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Il nodo foglia viene creato con un nome valido ed è associato a un nodo aggregato specifico, tramite un riferimento assegnato in fase di costruzione.</para>
    /// <para><b>Invariants:</b><br/>
    /// - Il riferimento a <c>Parent</c> è obbligatorio, non nullo e non modificabile dopo l'assegnazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Fa parte della struttura aggregata definita dal nodo <typeparamref name="TAggregate"/>.</para>
    /// </remarks>
    public interface IValueNode<TSelf, TAggregate> : INode
        where TSelf     : class, IValueNode<TSelf, TAggregate>
        where TAggregate: class, IAggregateNode<TAggregate, TSelf>
    {
        /// <summary>
        /// Nodo aggregato proprietario di questo nodo foglia.
        /// </summary>
        [Required]
        TAggregate Parent { get; }
    }
}