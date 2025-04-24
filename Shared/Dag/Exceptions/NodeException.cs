namespace JsonBridgeEF.Shared.Dag.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutte le anomalie semantiche legate a un nodo generico.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni concrete relative a vincoli violati come ID mancante o inconsistenza di nome.</para>
    /// </summary>
    public abstract class NodeException : InvalidOperationException
    {
        // Costruttore che accetta solo il messaggio
        protected NodeException(string message) : base(message) { }

        // Costruttore che accetta il messaggio e una eccezione interna
        protected NodeException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il nome del nodo è nullo o vuoto.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da <see cref="Validators.NodeValidator"/> o validatori derivati quando <c>Name</c> non è valorizzato correttamente.</para>
    /// <para><b>Note:</b><br/>
    /// Il parametro <c>nodeType</c> deve essere il tipo semantico del nodo (es. "JsonEntity", "JsonProperty"), non il valore di <c>Name</c>.</para>
    /// </summary>
    internal sealed class NodeInvalidNameException : NodeException
    {
        public NodeInvalidNameException(string nodeType)
            : base($"❌ Il nodo di tipo '{nodeType}' ha un nome nullo o vuoto.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando l'identificatore del nodo è nullo.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da validatori o factory che richiedono identificazione non nulla per operazioni topologiche.</para>
    /// <para><b>Note:</b><br/>
    /// Il parametro <c>nodeType</c> deve essere il tipo semantico del nodo (es. "JsonEntity", "JsonProperty").</para>
    /// </summary>
    internal sealed class NodeMissingIdException : NodeException
    {
        public NodeMissingIdException(string nodeType)
            : base($"❌ Il nodo di tipo '{nodeType}' ha un identificatore nullo.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory centralizzata per istanziare eccezioni semantiche relative ai nodi generici.</para>
    /// </summary>
    internal static class NodeError
    {
        /// <summary>
        /// Factory per nome non valido.
        /// </summary>
        /// <param name="nodeType">Tipo semantico del nodo (es. "JsonEntity").</param>
        public static NodeException InvalidName(string nodeType) =>
            new NodeInvalidNameException(nodeType);

        /// <summary>
        /// Factory per ID mancante.
        /// </summary>
        /// <param name="nodeType">Tipo semantico del nodo (es. "JsonEntity").</param>
        public static NodeException MissingId(string nodeType) =>
            new NodeMissingIdException(nodeType);
    }
}