using JsonBridgeEF.Shared.Dag.Helpers;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Shared.Navigation.Helpers
{
    /// <summary>
    /// Domain Concept: Gestore per la gestione topologica dei riferimenti parentali tra nodi aggregati in un DAG.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Inizializzato tramite costruttore con il nodo da gestire.</para>
    /// <para>Constraints: Il nodo deve implementare sia <see cref="IAggregateNode{TNode, TValue}"/> che <see cref="IParentNavigableNode{TNode}"/>.</para>
    /// <para>Relationships: Collabora con il validatore <see cref="AggregateNodeRelationGuard"/> per garantire l’integrità strutturale.</para>
    /// <para>Usage Notes: Da utilizzare in contesti che richiedono risalita e sincronizzazione bidirezionale dei riferimenti parentali.</para>
    /// </remarks>
    /// <remarks>
    /// Costruisce una nuova istanza di ParentNavigationManager per il nodo specificato.
    /// </remarks>
    /// <param name="node">Il nodo da gestire.</param>
    internal sealed class ParentNavigationManager<TNode, TValue>(TNode node)
        where TNode : class, IAggregateNode<TNode, TValue>, IParentNavigableNode<TNode>
        where TValue : class, IValueNode<TValue, TNode>
    {
        private readonly TNode _node = node ?? throw new ArgumentNullException(nameof(node));

        /// <inheritdoc cref="IParentNavigableNode{TNode}.Parents"/>
        public IReadOnlyCollection<TNode> Parents => _node.Parents;

        /// <inheritdoc cref="IParentNavigableNode{TNode}.IsRoot"/>
        public bool IsRoot => _node.IsRoot;

        /// <summary>
        /// Callback da eseguire all'inizio del flusso <c>AddParent</c>.
        /// </summary>
        /// <remarks>
        /// Utilizza questo delegato per eseguire logica personalizzata prima dell'inizio del flusso di esecuzione.
        /// Ad esempio, potresti implementare il logging, la preparazione del contesto o altre operazioni preliminari.
        /// </remarks>
        public Action<TNode> OnBeforeAddParentFlow { get; set; } = _ => { };

        /// <summary>
        /// Callback da eseguire appena prima dell'aggiunta del genitore.
        /// </summary>
        /// <remarks>
        /// Usa questo delegato per eseguire operazioni di pre-condizione o per preparare lo stato prima di aggiungere il genitore.
        /// Questo è il punto in cui il nodo è stato validato e si sta per procedere con la mutazione dello stato.
        /// </remarks>
        public Action<TNode> OnBeforeParentAdded { get; set; } = _ => { };

        /// <summary>
        /// Callback da eseguire subito dopo l’aggiunta del genitore.
        /// </summary>
        /// <remarks>
        /// Utilizza questo delegato per reagire all'avvenuta aggiunta del genitore.
        /// Ad esempio, potresti notificare altri componenti, aggiornare lo stato o eseguire ulteriori operazioni in risposta.
        /// </remarks>
        public Action<TNode> OnAfterParentAdded { get; set; } = _ => { };

        /// <summary>
        /// Callback da eseguire alla fine dell’intero flusso <c>AddParent</c>.
        /// </summary>
        /// <remarks>
        /// Questo delegato viene invocato come ultima operazione del flusso e può essere usato per operazioni di pulizia,
        /// logging finale o notifiche di completamento del processo.
        /// </remarks>
        public Action<TNode> OnAfterAddParentFlow { get; set; } = _ => { };

        /// <summary>
        /// Aggiunge un nodo genitore al nodo corrente, con validazione strutturale e sincronizzazione bidirezionale.
        /// </summary>
        /// <param name="parent">Nodo da aggiungere come genitore.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il nodo padre non deve violare la struttura DAG.</para>
        /// <para><b>Postconditions:</b> Il nodo viene aggiunto alla collezione di genitori del nodo corrente e viceversa.</para>
        /// <para><b>Side Effects:</b> Modifica lo stato dei riferimenti parentali e figli nei nodi coinvolti.</para>
        /// </remarks>
        public void AddParent(TNode parent)
        {
            HookedExecutionFlow
                .For<TNode>()
                .WithOnStartFlowHook(OnBeforeAddParentFlow) // Esegue la callback d'inizio flusso
                .WithInitialValidation(p => AggregateNodeRelationGuard.EnsureCanAddParent<TNode, TValue>(_node, p)) // Validazione semantica
                .WithOnPreActionHook(OnBeforeParentAdded) // Esegue la callback prima dell'aggiunta vera e propria
                .WithAction(p => EstablishBidirectionalRelation(p))
                .WithOnPostActionHook(OnAfterParentAdded) // Esegue la callback subito dopo l'aggiunta
                .SkipFinalValidation() // Salta validazioni finali extra
                .WithOnCompleteFlowHook(OnAfterAddParentFlow) // Esegue la callback di completamento del flusso
                .Execute(parent); // Avvia il flusso
        }

        /// <summary>
        /// Stabilisce la relazione bidirezionale padre-figlio tra il nodo corrente e il nodo genitore passato.
        /// </summary>
        /// <param name="parent">Il nodo da impostare come genitore.</param>
        private void EstablishBidirectionalRelation(TNode parent)
        {
            // Imposta la relazione padre->figlio
            parent.AddChild(_node);

            // Se il nodo corrente (figlio) non ha già registrato il genitore, lo aggiunge
            if (!_node.Parents.Contains(parent))
            {
                _node.AddParent(parent);
            }
        }
    }
}