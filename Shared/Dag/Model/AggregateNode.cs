using System.Collections.ObjectModel;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Helpers;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Validators;
using JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <inheritdoc cref="IAggregateNode{TSelf, TValue}"/>
    /// <summary>
    /// Domain Class: Nodo aggregato che contiene altri nodi nel grafo.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto con nome e validatore opzionale.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessun ciclo, nessun duplicato, coerenza semantica con i figli.</para>
    /// <para><b>Relationships:</b><br/>
    /// Contiene figli sia di tipo aggregato che foglia; la validazione di aggiunta è delegata a <see cref="AggregateNodeValidator{TSelf, TValue}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Sovrascrivere gli hook semantici nelle sottoclassi per personalizzare il flusso.</para>
    /// </remarks>
    internal abstract class AggregateNode<TSelf, TValue> 
        : Node<TSelf>, IAggregateNode<TSelf, TValue>
        where TSelf  : AggregateNode<TSelf, TValue>, IAggregateNode<TSelf, TValue>
        where TValue: class, IValueNode<TValue, TSelf>
    {
        private readonly List<TSelf>  _selfChildren  = new();
        private readonly List<TValue> _valueChildren = new();

        private readonly IReadOnlyCollection<TSelf>  _selfChildrenView;
        private readonly IReadOnlyCollection<TValue> _valueChildrenView;

        /// <summary>
        /// Costruttore protetto: inizializza nome, validatore e le collezioni in sola lettura.
        /// </summary>
        /// <param name="name">Il nome del nodo aggregato.</param>
        /// <param name="validator">Validatore opzionale per il tipo concreto <typeparamref name="TSelf"/>.</param>
        protected AggregateNode(
            string name,
            IValidateAndFix<TSelf>? validator
        ) : base(name, validator)
        {
            _selfChildrenView  = new ReadOnlyCollection<TSelf>(_selfChildren);
            _valueChildrenView = new ReadOnlyCollection<TValue>(_valueChildren);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<TSelf> SelfChildren  => _selfChildrenView;

        /// <inheritdoc />
        public IReadOnlyCollection<TValue> ValueChildren => _valueChildrenView;

        /// <inheritdoc />
        public void AddChild(TSelf child)
        {
            HookedExecutionFlow
                .For<TSelf>()
                .WithOnStartFlowHook(OnBeforeAddChildFlow)
                .WithInitialValidation(c => AggregateNodeRelationGuard
                    .EnsureCanAddChild<TSelf, TValue>((TSelf)this, c))
                .WithOnPreActionHook(OnBeforeChildAdded)
                .WithAction(c =>
                {
                    if (!_selfChildren.Contains(c))
                        _selfChildren.Add(c);
                })
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
                .WithInitialValidation(c => AggregateNodeRelationGuard
                    .EnsureCanAddChild((TSelf)this, c))
                .WithOnPreActionHook(OnBeforeChildAdded)
                .WithAction(c =>
                {
                    if (!_valueChildren.Contains(c))
                        _valueChildren.Add(c);
                })
                .WithOnPostActionHook(OnAfterChildAdded)
                .SkipFinalValidation()
                .WithOnCompleteFlowHook(OnAfterAddChildFlow)
                .Execute(child);
        }

        // Hook virtuali per estendere il flusso semantico

        protected virtual void OnBeforeAddChildFlow(TSelf child) { }

        protected virtual void OnBeforeAddChildFlow(TValue child) { }

        protected virtual void OnBeforeChildAdded(TSelf child) { }

        protected virtual void OnBeforeChildAdded(TValue child) { }

        protected virtual void OnAfterChildAdded(TSelf child) { }

        protected virtual void OnAfterChildAdded(TValue child) { }

        protected virtual void OnAfterAddChildFlow(TSelf child) { }

        protected virtual void OnAfterAddChildFlow(TValue child) { }
    }
}