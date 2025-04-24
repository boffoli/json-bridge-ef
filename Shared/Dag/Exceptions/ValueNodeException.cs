namespace JsonBridgeEF.Shared.Dag.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per anomalie semantiche nei nodi valore (foglia) all'interno della struttura a grafo.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni specifiche per errori nei nodi foglia come valore nullo o duplicazione.</para>
    /// </summary>
    public abstract class ValueNodeException : NodeException
    {
        // Costruttore che accetta solo il messaggio di errore
        protected ValueNodeException(string message) : base(message) { }

        // Costruttore che accetta il messaggio di errore e una eventuale eccezione interna
        protected ValueNodeException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il valore del nodo foglia è nullo o invalido.</para>
    /// </summary>
    internal sealed class ValueNodeInvalidValueException : ValueNodeException
    {
        private ValueNodeInvalidValueException(string nodeName)
            : base($"❌ Il nodo valore '{nodeName}' ha un valore nullo o invalido.") { }

        private ValueNodeInvalidValueException(string nodeName, Exception inner)
            : base($"❌ Il nodo valore '{nodeName}' ha un valore nullo o invalido.", inner) { }

        public static ValueNodeInvalidValueException InvalidValue(string nodeName) =>
            new(nodeName);

        public static ValueNodeInvalidValueException InvalidValue(string nodeName, Exception inner) =>
            new(nodeName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando viene rilevata una chiave duplicata nei nodi foglia.</para>
    /// </summary>
    internal sealed class ValueNodeDuplicateKeyException : ValueNodeException
    {
        private ValueNodeDuplicateKeyException(string nodeName, string key)
            : base($"❌ Il nodo valore '{nodeName}' ha una chiave duplicata: '{key}'.") { }

        private ValueNodeDuplicateKeyException(string nodeName, string key, Exception inner)
            : base($"❌ Il nodo valore '{nodeName}' ha una chiave duplicata: '{key}'.", inner) { }

        public static ValueNodeDuplicateKeyException DuplicateKey(string nodeName, string key) =>
            new(nodeName, key);

        public static ValueNodeDuplicateKeyException DuplicateKey(string nodeName, string key, Exception inner) =>
            new(nodeName, key, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il nodo valore ha un parent nullo.</para>
    /// </summary>
    internal sealed class ValueNodeNullParentException : ValueNodeException
    {
        private ValueNodeNullParentException(string nodeName)
            : base($"❌ Il nodo valore '{nodeName}' ha un parent nullo.") { }

        private ValueNodeNullParentException(string nodeName, Exception inner)
            : base($"❌ Il nodo valore '{nodeName}' ha un parent nullo.", inner) { }

        public static ValueNodeNullParentException NullParent(string nodeName) =>
            new(nodeName);

        public static ValueNodeNullParentException NullParent(string nodeName, Exception inner) =>
            new(nodeName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per eccezioni semantiche specifiche dei nodi valore (foglia), che usa il factory di NodeError in composizione.</para>
    /// </summary>
    internal static class ValueNodeError
    {
        // ==== Specifici per ValueNode ====

        public static ValueNodeException InvalidValue(string nodeName) =>
            ValueNodeInvalidValueException.InvalidValue(nodeName);

        public static ValueNodeException InvalidValue(string nodeName, Exception inner) =>
            ValueNodeInvalidValueException.InvalidValue(nodeName, inner);

        public static ValueNodeException DuplicateKey(string nodeName, string key) =>
            ValueNodeDuplicateKeyException.DuplicateKey(nodeName, key);

        public static ValueNodeException DuplicateKey(string nodeName, string key, Exception inner) =>
            ValueNodeDuplicateKeyException.DuplicateKey(nodeName, key, inner);

        public static ValueNodeException NullParent(string nodeName) =>
            ValueNodeNullParentException.NullParent(nodeName);

        public static ValueNodeException NullParent(string nodeName, Exception inner) =>
            ValueNodeNullParentException.NullParent(nodeName, inner);

        // ==== Composizione: metodi da NodeError ====

        public static NodeException InvalidName(string nodeType) =>
            NodeError.InvalidName(nodeType);

        public static NodeException MissingId(string nodeType) =>
            NodeError.MissingId(nodeType);
    }
}