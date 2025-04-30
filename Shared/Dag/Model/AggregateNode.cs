using System.Collections.ObjectModel;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Helpers;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <inheritdoc cref="IAggregateNode{TSelf, TValue}"/>
    /// <summary>
    /// Domain Class: Nodo aggregato che contiene altri nodi nel grafo.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un nodo in un grafo che può aggregare sia altri nodi dello stesso tipo
    /// (self-children) sia nodi di tipo valore (value-children).</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto con nome e validatore opzionale.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Le collezioni di figli non possono contenere duplicati né riferimenti null.<br/>
    /// - Non possono crearsi cicli di parent-child.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Eredita da <see cref="Node{TSelf}"/> per la gestione di nome, uguaglianza e hash code.<br/>
    /// - Implementa <see cref="IAggregateNode{TSelf, TValue}"/> per la navigazione e manipolazione dei figli.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - L’aggiunta di figli avviene tramite il flusso orchestrato di 
    ///   <see cref="HookedExecutionFlow"/>, con hook pre- e post-azione.<br/>
    /// - È possibile specializzare i metodi protetti 
    ///   (<c>OnBeforeAddChildFlow</c>, <c>OnBeforeChildAdded</c>, 
    ///   <c>OnAfterChildAdded</c>, <c>OnAfterAddChildFlow</c>) per aggiungere logica custom.</para>
    /// </remarks>
    internal abstract class AggregateNode<TSelf, TValue> 
        : Node<TSelf>, IAggregateNode<TSelf, TValue>
        where TSelf   : AggregateNode<TSelf, TValue>, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        private readonly List<TSelf>  _selfChildren  = [];
        private readonly List<TValue> _valueChildren = [];

        private readonly IReadOnlyCollection<TSelf>  _selfChildrenView;
        private readonly IReadOnlyCollection<TValue> _valueChildrenView;

        /// <summary>
        /// Costruttore protetto: inizializza nome, validatore e le collezioni in sola lettura.
        /// </summary>
        /// <param name="name">Il nome del nodo aggregato.</param>
        /// <param name="validator">Validatore opzionale per il tipo concreto <typeparamref name="TSelf"/>.</param>
        protected AggregateNode(string name, IValidateAndFix<TSelf>? validator = null)
            : base(name, validator)
        {
            _selfChildrenView  = new ReadOnlyCollection<TSelf>(_selfChildren);
            _valueChildrenView = new ReadOnlyCollection<TValue>(_valueChildren);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<TSelf> SelfChildren => _selfChildrenView;

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
        public void AddChildValue(TValue child)
        {
            HookedExecutionFlow
                .For<TValue>()
                .WithOnStartFlowHook(OnBeforeAddChildFlow)
                .WithInitialValidation(c => AggregateNodeRelationGuard
                    .EnsureCanAddChild<TSelf, TValue>((TSelf)this, c))
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

        // ================ Protected hooks for customization ================

        /// <summary>Hook invocato prima di iniziare il flusso di aggiunta di un figlio self.</summary>
        protected virtual void OnBeforeAddChildFlow(TSelf child) { }

        /// <summary>Hook invocato prima di iniziare il flusso di aggiunta di un figlio value.</summary>
        protected virtual void OnBeforeAddChildFlow(TValue child) { }

        /// <summary>Hook invocato prima dell’azione di aggiunta di un figlio self.</summary>
        protected virtual void OnBeforeChildAdded(TSelf child) { }

        /// <summary>Hook invocato prima dell’azione di aggiunta di un figlio value.</summary>
        protected virtual void OnBeforeChildAdded(TValue child) { }

        /// <summary>Hook invocato subito dopo l’azione di aggiunta di un figlio self.</summary>
        protected virtual void OnAfterChildAdded(TSelf child) { }

        /// <summary>Hook invocato subito dopo l’azione di aggiunta di un figlio value.</summary>
        protected virtual void OnAfterChildAdded(TValue child) { }

        /// <summary>Hook invocato al termine del flusso di aggiunta di un figlio self.</summary>
        protected virtual void OnAfterAddChildFlow(TSelf child) { }

        /// <summary>Hook invocato al termine del flusso di aggiunta di un figlio value.</summary>
        protected virtual void OnAfterAddChildFlow(TValue child) { }
    }
}