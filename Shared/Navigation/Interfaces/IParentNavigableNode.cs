namespace JsonBridgeEF.Shared.Navigation.Interfaces
{
    /// <summary>
    /// Domain Interface: rappresenta un nodo che consente la risalita topologica
    /// tramite riferimenti espliciti ai propri genitori.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto del nodo stesso (ricorsivo).</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo di un grafo diretto aciclico (DAG) che può essere percorso verso l’alto tramite
    /// riferimenti espliciti ai nodi genitori. Utile per modellare ereditarietà,
    /// dipendenze gerarchiche o altre strutture ascendenti.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Collegato a zero o più nodi genitori di tipo <typeparamref name="TSelf"/>.<br/>
    /// - Non implica che il nodo sia anche un aggregato.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Applicare questa interfaccia solo quando è richiesta navigazione verso i genitori.<br/>
    /// - Non usata per i nodi foglia.</para>
    /// </remarks>
    public interface IParentNavigableNode<TSelf>
        where TSelf : class
    {
        /// <summary>
        /// Collezione dei nodi genitori collegati a questo nodo.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Espone i riferimenti ai nodi da cui questo nodo dipende o eredita.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        IReadOnlyCollection<TSelf> Parents { get; }

        /// <summary>
        /// Indica se il nodo è radice, ovvero non ha alcun genitore.
        /// </summary>
        /// <remarks>
        /// <b>Definition:</b> Il nodo è root se <c>Parents.Count == 0</c>.
        /// </remarks>
        bool IsRoot { get; }

        /// <summary>
        /// Aggiunge un riferimento a un nodo genitore.
        /// </summary>
        /// <param name="parent">Nodo da collegare come genitore.</param>
        /// <remarks>
        /// <b>Preconditions:</b> Il nodo <paramref name="parent"/> non può essere <c>null</c> o causare cicli.<br/>
        /// <b>Postconditions:</b> Il nodo viene aggiunto a <see cref="Parents"/> se valido.
        /// </remarks>
        void AddParent(TSelf parent);
    }
}