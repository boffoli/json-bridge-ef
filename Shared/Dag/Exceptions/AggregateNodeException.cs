namespace JsonBridgeEF.Shared.Dag.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per anomalie semantiche nei nodi aggregati (es. cicli, figli nulli, chiavi duplicate).</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni concrete per gestire problemi nella struttura aggregata dei nodi.</para>
    /// </summary>
    public abstract class AggregateNodeException : NodeException
    {
        // Aggiunto costruttore che accetta un messaggio e un'eccezione interna
        protected AggregateNodeException(string message) : base(message) { }

        // Aggiunto costruttore che accetta un messaggio e un'eccezione interna
        protected AggregateNodeException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando viene rilevato un ciclo nella gerarchia dei nodi aggregati.</para>
    /// </summary>
    internal sealed class AggregateNodeCycleException : AggregateNodeException
    {
        public AggregateNodeCycleException(string parentName, string childName)
            : base($"❌ Ciclo rilevato tra i nodi '{parentName}' e '{childName}'.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando un nodo aggregato ha un figlio nullo.</para>
    /// </summary>
    internal sealed class AggregateNodeNullChildException : AggregateNodeException
    {
        public AggregateNodeNullChildException(string parentName)
            : base($"❌ Il nodo aggregato '{parentName}' contiene un riferimento nullo a un figlio.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando viene rilevata una chiave duplicata tra i figli del nodo aggregato.</para>
    /// </summary>
    internal sealed class AggregateNodeDuplicateChildKeyException : AggregateNodeException
    {
        public AggregateNodeDuplicateChildKeyException(string parentName, string duplicatedKey)
            : base($"❌ Il nodo '{parentName}' ha più figli con la chiave '{duplicatedKey}'.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per eccezioni semantiche specifiche dei nodi aggregati.<br/>
    /// Include anche i metodi di <see cref=\"NodeError\"/> tramite composizione.</para>
    /// </summary>
    internal static class AggregateNodeError
    {
        // ==== Specifici per AggregateNode ====

        /// <summary>
        /// Factory per ciclo diretto padre-figlio.
        /// </summary>
        public static AggregateNodeException CycleDetected(string parentName, string childName) =>
            new AggregateNodeCycleException(parentName, childName);

        /// <summary>
        /// Factory per figlio nullo nel nodo aggregato.
        /// </summary>
        public static AggregateNodeException NullChild(string parentName) =>
            new AggregateNodeNullChildException(parentName);

        /// <summary>
        /// Factory per chiavi duplicate tra figli.
        /// </summary>
        public static AggregateNodeException DuplicateChildKey(string parentName, string duplicatedKey) =>
            new AggregateNodeDuplicateChildKeyException(parentName, duplicatedKey);

        // ==== Composizione: metodi da NodeError ====

        /// <inheritdoc cref="NodeError.InvalidName"/>
        public static NodeException InvalidName(string nodeType) =>
            NodeError.InvalidName(nodeType);

        /// <inheritdoc cref="NodeError.MissingId"/>
        public static NodeException MissingId(string nodeType) =>
            NodeError.MissingId(nodeType);
    }
}