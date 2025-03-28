using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Managers;

/// <summary>
/// Domain Manager: Componente di supporto interno per la gestione delle relazioni gerarchiche
/// padre-figlio in entità che implementano <see cref="IHierarchical{TSelf}"/>.
/// </summary>
/// <typeparam name="TSelf">
/// Tipo concreto dell’entità gerarchica (self-reference), che implementa <see cref="IHierarchical{TSelf}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Responsabile del mantenimento coerente delle relazioni ricorsive tra entità genitrici e figlie
/// in strutture gerarchiche complesse, come alberi o DAG (grafo aciclico diretto).</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Da usare tramite composizione nelle entità che implementano <see cref="IHierarchical{TSelf}"/>.
/// Deve essere inizializzato con il nodo corrente che funge da riferimento centrale.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nodo corrente non può essere null.<br/>
/// - Le relazioni sono simmetriche e coerenti: se A è parent di B, allora B è child di A.<br/>
/// - Le aggiunte sono idempotenti (non creano duplicati).</para>
///
/// <para><b>Relationships:</b><br/>
/// - Le collezioni <see cref="Parents"/> e <see cref="Children"/> sono sincronizzate automaticamente.<br/>
/// - Le chiamate a <see cref="AddParent"/> e <see cref="AddChild"/> mantengono la coerenza reciproca.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usato all’interno di entità come <c>BaseEfHierarchicalEntity</c> per implementare comportamenti ricorsivi.<br/>
/// - Non gestisce rimozione o verifica di cicli: questi vincoli devono essere imposti dal dominio applicativo.</para>
/// </remarks>
/// <remarks>
/// Inizializza il manager per il nodo specificato.
/// </remarks>
/// <param name="node">Il nodo corrente (entità gerarchica da gestire).</param>
/// <exception cref="ArgumentNullException">Se <paramref name="node"/> è null.</exception>
internal sealed class HierarchicalEntityManager<TSelf>(TSelf node)
    where TSelf : class, IHierarchical<TSelf>
{
    // 🔹 PRIVATE FIELDS 🔹

    private readonly TSelf _node = node ?? throw new ArgumentNullException(nameof(node));
    private readonly HashSet<TSelf> _parents = [];
    private readonly HashSet<TSelf> _children = [];

    // 🔹 PUBLIC PROPERTIES 🔹

    /// <summary>
    /// Collezione delle entità genitrici del nodo corrente.
    /// </summary>
    public IReadOnlyCollection<TSelf> Parents => _parents;

    /// <summary>
    /// Collezione delle entità figlie del nodo corrente.
    /// </summary>
    public IReadOnlyCollection<TSelf> Children => _children;

    // 🔹 PUBLIC METHODS 🔹

    /// <summary>
    /// Aggiunge un’entità come figlio, aggiornando anche i riferimenti inversi.
    /// </summary>
    /// <param name="child">L’entità da aggiungere come figlia.</param>
    public void AddChild(TSelf child)
    {
        if (_children.Add(child))
            child.AddParent(_node);
    }

    /// <summary>
    /// Aggiunge un’entità come genitore, aggiornando anche i riferimenti inversi.
    /// </summary>
    /// <param name="parent">L’entità da aggiungere come genitore.</param>
    public void AddParent(TSelf parent)
    {
        if (_parents.Add(parent))
            parent.AddChild(_node);
    }
}