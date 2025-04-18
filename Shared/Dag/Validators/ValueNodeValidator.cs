using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Model;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore generico per i nodi foglia <see cref="ValueNode{TSelf,TAggregate}"/>.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Istanza utilizzata internamente nei validatori di dominio specifici per validare nodi foglia in strutture a grafo.</para>
/// <para>Constraints: Il nodo deve avere un riferimento valido al proprio aggregato (<c>Parent</c> diverso da <c>null</c>).</para>
/// <para>Relationships: Estende <see cref="NodeValidator"/> per riutilizzare la validazione di base sui nodi, e implementa <see cref="IValidateAndFix{TModel}"/>.</para>
/// <para>Usage Notes: Non usato direttamente dall’utente. Deve essere invocato tramite validatori specifici come quelli per proprietà.</para>
/// </remarks>
internal class ValueNodeValidator<TSelf, TAggregate>
    : NodeValidator, IValidateAndFix<IValueNode<TSelf, TAggregate>>
    where TSelf : class, IValueNode<TSelf, TAggregate>
    where TAggregate : class, IAggregateNode<TAggregate, TSelf>
{
    /// <inheritdoc />
    public void EnsureValid(IValueNode<TSelf, TAggregate> valueNode)
    {
        base.EnsureValid(valueNode);

        if (valueNode.Parent == null)
            throw new ValidationException($"La proprietà 'Parent' non può essere null nel nodo '{valueNode.Name}'.");
    }

    /// <inheritdoc />
    public void Fix(IValueNode<TSelf, TAggregate> valueNode)
    {
        base.Fix(valueNode);
        // Nessuna correzione automatica per il parent
    }
}