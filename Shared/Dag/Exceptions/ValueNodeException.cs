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
        /// <summary>Crea una nuova eccezione di tipo <see cref="ValueNodeException"/>.</summary>
        protected ValueNodeException(string message) : base(message) { }

        /// <summary>Crea una nuova eccezione di tipo <see cref="ValueNodeException"/>, con eccezione interna.</summary>
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
    /// Factory per eccezioni semantiche specifiche dei nodi valore (foglia), che usa il factory di <see cref="NodeError"/> in composizione.</para>
    /// </summary>
    internal static class ValueNodeError
    {
        /// <summary>Valore nullo o invalido.</summary>
        public static ValueNodeException InvalidValue(string nodeName) =>
            ValueNodeInvalidValueException.InvalidValue(nodeName);

        /// <summary>Valore nullo o invalido con eccezione interna.</summary>
        public static ValueNodeException InvalidValue(string nodeName, Exception inner) =>
            ValueNodeInvalidValueException.InvalidValue(nodeName, inner);

        /// <summary>Chiave duplicata.</summary>
        public static ValueNodeException DuplicateKey(string nodeName, string key) =>
            ValueNodeDuplicateKeyException.DuplicateKey(nodeName, key);

        /// <summary>Chiave duplicata con eccezione interna.</summary>
        public static ValueNodeException DuplicateKey(string nodeName, string key, Exception inner) =>
            ValueNodeDuplicateKeyException.DuplicateKey(nodeName, key, inner);

        /// <summary>Parent nullo.</summary>
        public static ValueNodeException NullParent(string nodeName) =>
            ValueNodeNullParentException.NullParent(nodeName);

        /// <summary>Parent nullo con eccezione interna.</summary>
        public static ValueNodeException NullParent(string nodeName, Exception inner) =>
            ValueNodeNullParentException.NullParent(nodeName, inner);
    }
}