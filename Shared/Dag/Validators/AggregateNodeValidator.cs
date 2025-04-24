using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Exceptions;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Model;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore per nodi aggregati (<see cref="AggregateNode{TSelf,TValue}"/>).
/// </summary>
/// <remarks>
/// <para><b>Creation Strategy:</b><br/>
/// Usato come validatore base per le entità aggregate. Istanza unica per tipo generico.</para>
/// <para><b>Constraints:</b><br/>
/// - Nome non nullo o vuoto (ereditato da <see cref="NodeValidator"/>).<br/>
/// - Nessun figlio nullo.<br/>
/// - Chiavi figlio univoche (gestibile nei validatori derivati).</para>
/// <para><b>Relationships:</b><br/>
/// Estende <see cref="NodeValidator"/> e collabora con <see cref="ValueNodeValidator{TSelf,TAggregate}"/>.</para>
/// <para><b>Usage Notes:</b><br/>
/// Può essere esteso da validatori specifici come <c>EntityValidator</c> o <c>JsonEntityValidator</c>.</para>
/// </remarks>
internal class AggregateNodeValidator<TSelf, TValue> : NodeValidator, IValidateAndFix<IAggregateNode<TSelf, TValue>>
    where TSelf : class, IAggregateNode<TSelf, TValue>
    where TValue : class, IValueNode<TValue, TSelf>
{
    /// <inheritdoc />
    public void EnsureValid(IAggregateNode<TSelf, TValue> aggregateNode)
    {
        base.EnsureValid(aggregateNode);

        // Verifica: nessun figlio nullo nella lista SelfChildren
        foreach (var child in aggregateNode.SelfChildren)
        {
            if (child is null)
                throw AggregateNodeError.NullChild(aggregateNode.GetType().Name);
        }
    }

    /// <inheritdoc />
    public void Fix(IAggregateNode<TSelf, TValue> aggregateNode)
    {
        base.Fix(aggregateNode);
        // Nessuna correzione automatica per nodi aggregati
    }
}