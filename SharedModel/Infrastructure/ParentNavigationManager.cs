using JsonBridgeEF.SharedModel.DagModel;

namespace JsonBridgeEF.SharedModel.Infrastructure
{
    /// <summary>
    /// Gestore per i servizi di risalita topologica su nodi che implementano
    /// sia <see cref="IParentNavigableNode{TNode}"/> sia <see cref="IAggregateNode{TNode, TValue}"/>.
    /// </summary>
    /// <typeparam name="TNode">Tipo concreto del nodo stesso (autocollegato ai propri genitori).</typeparam>
    /// <typeparam name="TValue">Tipo dei nodi foglia (non utilizzati per la risalita).</typeparam>
    /// <remarks>
    /// Inizializza una nuova istanza del gestore per il nodo specificato.
    /// </remarks>
    /// <param name="node">Nodo navigabile da gestire.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="node"/> è <c>null</c>.</exception>
    internal sealed class ParentNavigationManager<TNode, TValue>(TNode node)
        where TNode : class, IAggregateNode<TNode, TValue>, IParentNavigableNode<TNode>
        where TValue : class, IValueNode<TValue, TNode>
    {
        private readonly TNode _node = node ?? throw new ArgumentNullException(nameof(node));

        /// <summary>
        /// Restituisce i genitori attualmente associati al nodo.
        /// </summary>
        public IReadOnlyCollection<TNode> Parents => _node.Parents;

        /// <summary>
        /// Indica se il nodo è radice, ovvero se non ha genitori.
        /// </summary>
        public bool IsRoot => _node.IsRoot;

        /// <summary>
        /// Aggiunge un nodo genitore, verificando duplicati, cicli e, se richiesto, anche la relazione inversa.
        /// </summary>
        /// <param name="parent">Nodo da aggiungere come genitore.</param>
        /// <param name="bidirectional">
        /// Se <c>true</c>, il nodo corrente verrà aggiunto ai figli del genitore, e <paramref name="parent"/> sarà
        /// aggiunto solo se questa operazione ha successo.
        /// </param>
        /// <exception cref="ArgumentNullException">Se <paramref name="parent"/> è <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Se il nodo è già presente, è sé stesso o crea una dipendenza circolare.
        /// </exception>
        public void AddParent(TNode parent, bool bidirectional)
        {
            ArgumentNullException.ThrowIfNull(parent);

            // Il nodo non può essere suo stesso genitore
            if (_node.Equals(parent))
                throw new InvalidOperationException("Un nodo non può essere genitore di sé stesso.");

            // Verifica duplicato
            if (_node.Parents.Contains(parent))
                throw new InvalidOperationException("Il nodo specificato è già presente tra i genitori.");

            // Verifica che non si crei un ciclo
            if (CycleCheck.ContainsPath(parent, _node))
                throw new InvalidOperationException("L'aggiunta del nodo genitore creerebbe un ciclo nel grafo.");

            // Se bidirezionale, aggiunge il nodo corrente ai figli del genitore
            if (bidirectional)
            {
                parent.AddChild(_node);
            }

            _node.AddParent(parent);
        }

        /// <summary>
        /// Classe ausiliaria interna per la rilevazione di cicli.
        /// </summary>
        private static class CycleCheck
        {
            /// <summary>
            /// Verifica se esiste un percorso dal nodo <paramref name="start"/> al nodo <paramref name="target"/>.
            /// </summary>
            public static bool ContainsPath(TNode start, TNode target)
            {
                return Visit(start, target);
            }

            private static bool Visit(TNode current, TNode target)
            {
                foreach (var child in current.SelfChildren)
                {
                    if (child.Equals(target) || Visit(child, target))
                        return true;
                }

                return false;
            }
        }
    }
}