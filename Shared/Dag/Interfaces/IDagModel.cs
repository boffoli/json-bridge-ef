using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Shared.Dag.Interfaces
{
    /// <summary>
    /// Domain Interface: Marker per tutti i nodi di un grafo generico.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Qualsiasi entità che possa partecipare a una struttura a grafo (DAG o simile).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato da tutti i tipi di nodo del dominio che necessitano di un nome univoco.</para>
    /// <para><b>Constraints:</b><br/>
    /// Deve esporre un nome non nullo e immutabile.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Usato come tipo di input/ritorno per servizi e helper che lavorano su collezioni eterogenee di nodi.</para>
    /// </remarks>
    public interface INode
    {
        /// <summary>
        /// Nome univoco del nodo nel grafo.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Domain Interface: Contratto per un nodo aggregato in un grafo generico.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un nodo che può contenere sia figli dello stesso tipo (self-children)
    /// sia figli di valore (value-children).</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato da classi che estendono 
    /// <see cref="JsonBridgeEF.Shared.Dag.Model.AggregateNode{TSelf,TValue}"/>.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Le collezioni di figli non contengono null né duplicati.<br/>
    /// - Non sono ammessi cicli parent-child.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Estende <see cref="INode"/>.<br/>
    /// - I figli di tipo valore implementano 
    ///   <see cref="IValueNode{TValue,TSelf}"/>.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - L’aggiunta di figli avviene tramite i metodi 
    ///   <c>AddChild</c> e <c>AddChildValue</c>, che orchestrano hook pre- e post-azione.</para>
    /// </remarks>
    public interface IAggregateNode<TSelf, TValue> : INode
        where TSelf : class, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        /// <summary>
        /// Figli dello stesso tipo di nodo (self-children).
        /// </summary>
        IReadOnlyCollection<TSelf> SelfChildren { get; }

        /// <summary>
        /// Figli di tipo valore (value-children).
        /// </summary>
        IReadOnlyCollection<TValue> ValueChildren { get; }

        /// <summary>
        /// Aggiunge un figlio dello stesso tipo di nodo.
        /// </summary>
        /// <param name="child">Il nodo figlio da aggiungere.</param>
        void AddChild(TSelf child);

        /// <summary>
        /// Aggiunge un figlio di tipo valore.
        /// </summary>
        /// <param name="child">Il nodo di valore da aggiungere.</param>
        void AddChildValue(TValue child);
    }

    /// <summary>
    /// Domain Interface: Contratto per un nodo di valore (leaf) in un grafo aggregato.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un nodo senza figli, che appartiene a un nodo aggregato.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato da classi che estendono 
    /// <see cref="Model.ValueNode{TSelf,TAggregate}"/>.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Estende <see cref="INode"/>.<br/>
    /// - Ha un parent di tipo <see cref="IAggregateNode{TAggregate,TSelf}"/>.</para>
    /// </remarks>
    public interface IValueNode<TSelf, TAggregate> : INode
        where TSelf     : class, IValueNode<TSelf, TAggregate>
        where TAggregate: class, IAggregateNode<TAggregate, TSelf>
    {
        /// <summary>
        /// Il nodo aggregato proprietario di questo valore.
        /// </summary>
        TAggregate Parent { get; }
    }
}
