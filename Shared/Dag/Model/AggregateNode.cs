using System.Collections.ObjectModel;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Validators;
using JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

namespace JsonBridgeEF.Shared.Dag.Model;

/// <inheritdoc cref="IAggregateNode{TSelf, TValue}"/>
/// <summary>
/// Domain Class: Nodo aggregato che contiene altri nodi nel grafo.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Istanza tramite costruttore protetto con nome.</para>
/// <para>Constraints: Nessun ciclo, nessun duplicato, coerenza semantica con i figli.</para>
/// <para>Relationships: Contiene figli sia di tipo aggregato sia foglia; collabora con il validatore <see cref="AggregateNodeValidator"/>.</para>
/// <para>Usage Notes: Sovrascrivere gli hook semantici nelle sottoclassi per arricchire il comportamento.</para>
/// </remarks>
internal abstract class AggregateNode<TSelf, TValue>(string name)
    : Node<TSelf, TValue>(name), IAggregateNode<TSelf, TValue>
    where TSelf : AggregateNode<TSelf, TValue>, IAggregateNode<TSelf, TValue>
    where TValue : class, IValueNode<TValue, TSelf>
{
    private readonly List<TSelf> _selfChildren = [];
    private readonly List<TValue> _valueChildren = [];

    /// <inheritdoc />
    public IReadOnlyCollection<TSelf> SelfChildren => new ReadOnlyCollection<TSelf>(_selfChildren);

    /// <inheritdoc />
    public IReadOnlyCollection<TValue> ValueChildren => new ReadOnlyCollection<TValue>(_valueChildren);

    /// <inheritdoc />
    public void AddChild(TSelf child)
    {
        HookedExecutionFlow
            .For<TSelf>()
            .WithOnStartFlowHook(OnBeforeAddChildFlow)
            .WithInitialValidation(c => AggregateNodeValidator.EnsureCanAddChild<TSelf, TValue>((TSelf)this, c))
            .WithOnPreActionHook(OnBeforeChildAdded)
            .WithAction(c => { if (!_selfChildren.Contains(c)) _selfChildren.Add(c); })
            .WithOnPostActionHook(OnAfterChildAdded)
            .SkipFinalValidation()
            .WithOnCompleteFlowHook(OnAfterAddChildFlow)
            .Execute(child);
    }

    /// <inheritdoc />
    public void AddChild(TValue child)
    {
        HookedExecutionFlow
            .For<TValue>()
            .WithOnStartFlowHook(OnBeforeAddChildFlow)
            .WithInitialValidation(c => AggregateNodeValidator.EnsureCanAddChild((TSelf)this, c))
            .WithOnPreActionHook(OnBeforeChildAdded)
            .WithAction(c => _valueChildren.Add(c))
            .WithOnPostActionHook(OnAfterChildAdded)
            .SkipFinalValidation()
            .WithOnCompleteFlowHook(OnAfterAddChildFlow)
            .Execute(child);
    }

    protected virtual void OnBeforeAddChildFlow(TSelf child){}
    protected virtual void OnBeforeAddChildFlow(TValue child){}

    protected virtual void OnBeforeChildAdded(TSelf child){}
    protected virtual void OnBeforeChildAdded(TValue child){}

    protected virtual void OnAfterChildAdded(TSelf child){}
    protected virtual void OnAfterChildAdded(TValue child){}

    protected virtual void OnAfterAddChildFlow(TSelf child){}
    protected virtual void OnAfterAddChildFlow(TValue child){}
}
