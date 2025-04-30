using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Exceptions;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Validators
{
    /// <summary>
    /// Domain Concept: Validatore per nodi di valore (<see cref="IValueNode{TSelf,TAggregate}"/>).
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Usato come validatore per tutte le implementazioni di <see cref="IValueNode{TSelf,TAggregate}"/>.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Il parent non può essere nullo.</para>
    /// </remarks>
    internal class ValueNodeValidator<TSelf, TAggregate>
        : IValidateAndFix<IValueNode<TSelf, TAggregate>>
        where TSelf      : class, IValueNode<TSelf, TAggregate>
        where TAggregate : class, IAggregateNode<TAggregate, TSelf>
    {
        /// <inheritdoc/>
        public void EnsureValid(IValueNode<TSelf, TAggregate> node)
        {
            if (node.Parent is null)
                throw ValueNodeError.NullParent(node.Name);
        }

        /// <inheritdoc/>
        public void Fix(IValueNode<TSelf, TAggregate> node)
        {
            // Nessuna correzione automatica prevista
        }
    }
}