using System.Collections.ObjectModel;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Validators;

namespace JsonBridgeEF.Shared.Dag.Model
{
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
        public void AddChild(TValue child)
        {
            // STEP 1: Hook pre-validazione semantica (override opzionale)
            OnBeforeValidateAdd(child);

            // STEP 2: Validazione logica e strutturale tramite il validatore centralizzato
            AggregateNodeValidator.EnsureCanAddChild<TSelf, TValue>((TSelf)this, child);

            // STEP 3: Hook post-validazione semantica (override opzionale)
            OnAfterValidateAdd(child);

            // STEP 4: Hook pre-inserimento
            OnBeforeChildAdded(child);

            // STEP 5: Inserimento nella lista interna
            _valueChildren.Add(child);

            // STEP 6: Hook post-inserimento
            OnAfterChildAdded(child);
        }

        /// <inheritdoc />
        public void AddChild(TSelf child)
        {
            // STEP 1: Hook pre-validazione semantica (override opzionale)
            OnBeforeValidated(child);

            // STEP 2: Validazione logica e strutturale tramite il validatore centralizzato
            AggregateNodeValidator.EnsureCanAddChild<TSelf, TValue>((TSelf)this, child);

            // STEP 3: Hook post-validazione semantica (override opzionale)
            OnAfterValidated(child);

            // STEP 4: Hook pre-inserimento
            OnBeforeExecution(child);

            // STEP 5: Aggiunta se mancante (presenza già validata ma evitata per idempotenza)
            if (!_selfChildren.Contains(child))
                _selfChildren.Add(child);

            // STEP 6: Hook post-inserimento
            OnAfterExecution(child);
        }

        /// <summary>
        /// Metodo invocato automaticamente prima della validazione semantica.
        /// Utile per arricchire la logica prima della convalida.
        /// </summary>
        protected virtual void OnBeforeValidated(TSelf child) { }

        /// <summary>
        /// Metodo invocato automaticamente prima della validazione semantica.
        /// Utile per arricchire la logica prima della convalida.
        /// </summary>
        protected virtual void OnBeforeValidateAdd(TValue child) { }

        /// <summary>
        /// Metodo invocato automaticamente dopo la validazione semantica.
        /// Utile per operazioni derivate dalla convalida.
        /// </summary>
        protected virtual void OnAfterValidated(TSelf child) { }

        /// <summary>
        /// Metodo invocato automaticamente dopo la validazione semantica.
        /// Utile per operazioni derivate dalla convalida.
        /// </summary>
        protected virtual void OnAfterValidateAdd(TValue child) { }

        /// <summary>
        /// Metodo invocato automaticamente prima dell'aggiunta di un nodo foglia.
        /// Utilizzato per sincronizzazioni o logica personalizzata.
        /// </summary>
        /// <param name="child">Nodo foglia in fase di inserimento.</param>
        protected virtual void OnBeforeChildAdded(TValue child) { }

        /// <summary>
        /// Metodo invocato automaticamente prima dell'aggiunta di un nodo figlio aggregato.
        /// Utilizzato per sincronizzazioni o logica personalizzata.
        /// </summary>
        /// <param name="child">Nodo figlio in fase di inserimento.</param>
        protected virtual void OnBeforeExecution(TSelf child) { }

        /// <summary>
        /// Metodo invocato automaticamente al termine dell'aggiunta di un nodo figlio aggregato.
        /// Utilizzato per sincronizzare relazioni inverse (es. AddParent).
        /// </summary>
        /// <param name="child">Nodo figlio appena aggiunto.</param>
        protected virtual void OnAfterExecution(TSelf child) { }

        /// <summary>
        /// Metodo invocato automaticamente al termine dell'aggiunta di un nodo foglia.
        /// Utile per aggiornamenti, notifiche o azioni post-inserimento.
        /// </summary>
        /// <param name="child">Nodo foglia appena inserito.</param>
        protected virtual void OnAfterChildAdded(TValue child) { }
    }
}
