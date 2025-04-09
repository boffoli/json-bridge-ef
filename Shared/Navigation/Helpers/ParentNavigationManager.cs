using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Validators;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Shared.Navigation.Helpers;

/// <summary>
/// Domain Concept: Gestore per la gestione topologica dei riferimenti parentali tra nodi aggregati in un DAG.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Inizializzato tramite costruttore con il nodo da gestire.</para>
/// <para>Constraints: Il nodo deve implementare sia <see cref="IAggregateNode{TNode, TValue}"/> che <see cref="IParentNavigableNode{TNode}"/>.</para>
/// <para>Relationships: Collabora con il validatore <see cref="AggregateNodeValidator"/> per garantire l’integrità strutturale.</para>
/// <para>Usage Notes: Da utilizzare in contesti che richiedono risalita e sincronizzazione bidirezionale dei riferimenti parentali.</para>
/// </remarks>
internal class ParentNavigationManager<TNode, TValue>(TNode node)
    where TNode : class, IAggregateNode<TNode, TValue>, IParentNavigableNode<TNode>
    where TValue : class, IValueNode<TValue, TNode>
{
    private readonly TNode _node = node ?? throw new ArgumentNullException(nameof(node));

    /// <inheritdoc cref="IParentNavigableNode{TNode}.Parents"/>
    public IReadOnlyCollection<TNode> Parents => _node.Parents;

    /// <inheritdoc cref="IParentNavigableNode{TNode}.IsRoot"/>
    public bool IsRoot => _node.IsRoot;

    /// <summary>
    /// Aggiunge un nodo genitore al nodo corrente, con validazione strutturale e sincronizzazione bidirezionale.
    /// </summary>
    /// <param name="parent">Nodo da aggiungere come genitore.</param>
    public void AddParent(TNode parent)
    {
        // STEP 1: Hook pre-validazione semantica (override opzionale)
        OnBeforeValidated(parent);

        // STEP 2: Validazione strutturale e semantica
        AggregateNodeValidator.EnsureCanAddParent<TNode, TValue>(_node, parent);

        // STEP 3: Hook post-validazione semantica (override opzionale)
        OnAfterValidated(parent);

        // STEP 4: Hook pre-inserimento (sincronizzazione, logica custom)
        OnBeforeExecution(parent);

        // STEP 5: Collegamento inverso, se necessario (AddChild interno è già idempotente)
        parent.AddChild(_node);

        // STEP 6: Aggiunta se mancante (presenza già validata ma evitata per idempotenza)
        if (!_node.Parents.Contains(parent))
            _node.AddParent(parent);

        // STEP 7: Hook post-inserimento (per eventuali azioni lato nodo)
        OnAfterExecution(parent);
    }

    /// <summary>
    /// Metodo invocato automaticamente prima della validazione semantica.
    /// </summary>
    /// <param name="parent">Nodo genitore da validare.</param>
    /// <remarks>
    /// <para><b>Preconditions:</b> Nessuna.</para>
    /// <para><b>Postconditions:</b> Possibilità di modificare o loggare prima della validazione.</para>
    /// <para><b>Side Effects:</b> Nessuno per default, ma può essere sovrascritto per logica custom.</para>
    /// </remarks>
    protected virtual void OnBeforeValidated(TNode parent) { }

    /// <summary>
    /// Metodo invocato automaticamente dopo la validazione semantica.
    /// </summary>
    /// <param name="parent">Nodo genitore validato.</param>
    /// <remarks>
    /// <para><b>Preconditions:</b> La validazione deve essere completata con successo.</para>
    /// <para><b>Postconditions:</b> Nessuna per default.</para>
    /// <para><b>Side Effects:</b> Nessuno per default, ma può essere sovrascritto.</para>
    /// </remarks>
    protected virtual void OnAfterValidated(TNode parent) { }

    /// <summary>
    /// Metodo invocato automaticamente prima dell'esecuzione dell'aggiunta.
    /// </summary>
    /// <param name="parent">Nodo genitore in fase di aggiunta.</param>
    /// <remarks>
    /// <para><b>Preconditions:</b> La validazione deve essere completata con successo.</para>
    /// <para><b>Postconditions:</b> Possibilità di preparare lo stato per l’aggiunta.</para>
    /// <para><b>Side Effects:</b> Nessuno per default, ma può essere sovrascritto per aggiunte custom.</para>
    /// </remarks>
    protected virtual void OnBeforeExecution(TNode parent) { }

    /// <summary>
    /// Metodo invocato automaticamente dopo l'esecuzione dell’aggiunta.
    /// </summary>
    /// <param name="parent">Nodo genitore appena aggiunto.</param>
    /// <remarks>
    /// <para><b>Preconditions:</b> L'aggiunta deve essere stata eseguita correttamente.</para>
    /// <para><b>Postconditions:</b> Possibilità di reagire all’aggiunta (eventi, log).</para>
    /// <para><b>Side Effects:</b> Nessuno per default, ma può essere sovrascritto per effetti derivati.</para>
    /// </remarks>
    protected virtual void OnAfterExecution(TNode parent) { }
}