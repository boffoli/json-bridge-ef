namespace JsonBridgeEF.Shared.Dag.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutte le anomalie semantiche legate a un nodo generico.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni concrete relative a vincoli violati come ID mancante o nome invalido.</para>
    /// </summary>
    public abstract class NodeException : InvalidOperationException
    {
        protected NodeException(string message) : base(message) { }
        protected NodeException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione per nome nullo o vuoto.</para>
    /// </summary>
    internal sealed class NodeInvalidNameException(string nodeType) : NodeException($"❌ Il nodo di tipo '{nodeType}' ha un nome nullo o vuoto.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory centralizzata per creare <see cref="NodeException"/> legate ai nodi generici.</para>
    /// </summary>
    internal static class NodeError
    {
        /// <summary>Factory per nome non valido.</summary>
        public static NodeException InvalidName(string nodeType)
            => new NodeInvalidNameException(nodeType);
    }
}