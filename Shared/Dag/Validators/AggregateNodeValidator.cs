using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Model;
using JsonBridgeEF.Shared.Dag.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore per nodi aggregati (<see cref="AggregateNode{TSelf,TValue}"/>).
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Usato come validatore base per le entità aggregate.</para>
/// <para>Constraints: Nessun figlio duplicato.</para>
/// <para>Relationships: Deriva da <see cref="NodeValidator"/>.</para>
/// <para>Usage Notes: Può essere esteso da validatori di entità come <c>EntityValidator</c>.</para>
/// </remarks>
internal class AggregateNodeValidator<TSelf, TValue> : NodeValidator, IValidateAndFix<IAggregateNode<TSelf, TValue>>
    where TSelf : class, IAggregateNode<TSelf, TValue>
    where TValue : class, IValueNode<TValue, TSelf>
{
    /// <inheritdoc />
    public void EnsureValid(IAggregateNode<TSelf, TValue> aggregateNode)
    {
        base.EnsureValid(aggregateNode);
        // Validazione su SelfChildren coerente con nodo aggregato
        foreach (var child in aggregateNode.SelfChildren)
        {
            if (child == null)
                throw new ValidationException($"Il nodo '{aggregateNode.Name}' contiene un riferimento nullo tra i SelfChildren.");
        }
    }

    /// <inheritdoc />
    public void Fix(IAggregateNode<TSelf, TValue> aggregateNode)
    {
        base.Fix(aggregateNode);
        // Nessuna correzione automatica per nodi aggregati
    }
}
