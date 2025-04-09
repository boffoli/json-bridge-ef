using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <inheritdoc cref="IValueNode{TSelf, TAggregate}"/>
    /// <summary>
    /// Domain Class: Nodo terminale associato a un aggregato.
    /// </summary>
    internal abstract class ValueNode<TSelf, TAggregate>(string name, TAggregate parent)
        : Node<TAggregate, TSelf>(name), IValueNode<TSelf, TAggregate>
        where TSelf : ValueNode<TSelf, TAggregate>, IValueNode<TSelf, TAggregate>
        where TAggregate : class, IAggregateNode<TAggregate, TSelf>
    {
        private readonly TAggregate _parent = parent ?? throw new ArgumentNullException(nameof(parent));

        /// <inheritdoc />
        [Required]
        public TAggregate Parent => _parent;
    }
}