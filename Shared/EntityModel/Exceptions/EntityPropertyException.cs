using JsonBridgeEF.Shared.Dag.Exceptions;

namespace JsonBridgeEF.Shared.EntityModel.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per anomalie semantiche nelle proprietà delle entità all'interno di un grafo.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni specifiche per errori come valore nullo o duplicazione nelle proprietà.</para>
    /// </summary>
    public abstract class EntityPropertyException : ValueNodeException
    {
        // Costruttore con message
        protected EntityPropertyException(string message)
            : base(message) { }

        // Costruttore con message e inner exception
        protected EntityPropertyException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il valore della proprietà dell'entità è nullo o invalido.</para>
    /// </summary>
    internal sealed class EntityPropertyInvalidValueException : EntityPropertyException
    {
        private EntityPropertyInvalidValueException(string nodeName)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha un valore nullo o invalido.") { }

        // Doppio costruttore con inner exception
        private EntityPropertyInvalidValueException(string nodeName, Exception inner)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha un valore nullo o invalido.", inner) { }

        public static EntityPropertyInvalidValueException InvalidValue(string nodeName) =>
            new(nodeName);

        public static EntityPropertyInvalidValueException InvalidValue(string nodeName, Exception inner) =>
            new(nodeName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando viene rilevata una chiave duplicata nelle proprietà dell'entità.</para>
    /// </summary>
    internal sealed class EntityPropertyDuplicateKeyException : EntityPropertyException
    {
        private EntityPropertyDuplicateKeyException(string nodeName, string key)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha una chiave duplicata: '{key}'.") { }

        // Doppio costruttore con inner exception
        private EntityPropertyDuplicateKeyException(string nodeName, string key, Exception inner)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha una chiave duplicata: '{key}'.", inner) { }

        public static EntityPropertyDuplicateKeyException DuplicateKey(string nodeName, string key) =>
            new(nodeName, key);

        public static EntityPropertyDuplicateKeyException DuplicateKey(string nodeName, string key, Exception inner) =>
            new(nodeName, key, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la proprietà dell'entità ha un parent nullo.</para>
    /// </summary>
    internal sealed class EntityPropertyNullParentException : EntityPropertyException
    {
        private EntityPropertyNullParentException(string nodeName)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha un parent nullo.") { }

        // Doppio costruttore con inner exception
        private EntityPropertyNullParentException(string nodeName, Exception inner)
            : base($"❌ La proprietà dell'entità '{nodeName}' ha un parent nullo.", inner) { }

        public static EntityPropertyNullParentException NullParent(string nodeName) =>
            new(nodeName);

        public static EntityPropertyNullParentException NullParent(string nodeName, Exception inner) =>
            new(nodeName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per eccezioni semantiche specifiche delle proprietà delle entità, che usa il factory di ValueNodeError in composizione.</para>
    /// </summary>
    internal static class EntityPropertyError
    {
        // ==== Specifici per EntityProperty ====

        public static EntityPropertyException InvalidValue(string nodeName) =>
            EntityPropertyInvalidValueException.InvalidValue(nodeName);

        public static EntityPropertyException InvalidValue(string nodeName, Exception inner) =>
            EntityPropertyInvalidValueException.InvalidValue(nodeName, inner);

        public static EntityPropertyException DuplicateKey(string nodeName, string key) =>
            EntityPropertyDuplicateKeyException.DuplicateKey(nodeName, key);

        public static EntityPropertyException DuplicateKey(string nodeName, string key, Exception inner) =>
            EntityPropertyDuplicateKeyException.DuplicateKey(nodeName, key, inner);

        public static EntityPropertyException NullParent(string nodeName) =>
            EntityPropertyNullParentException.NullParent(nodeName);

        public static EntityPropertyException NullParent(string nodeName, Exception inner) =>
            EntityPropertyNullParentException.NullParent(nodeName, inner);

        // ==== Composizione: metodi da ValueNodeError ====

        public static ValueNodeException InvalidValueFromValueNode(string nodeName) =>
            ValueNodeError.InvalidValue(nodeName);

        public static ValueNodeException DuplicateKeyFromValueNode(string nodeName, string key) =>
            ValueNodeError.DuplicateKey(nodeName, key);

        public static ValueNodeException NullParentFromValueNode(string nodeName) =>
            ValueNodeError.NullParent(nodeName);

        public static NodeException InvalidName(string nodeType) =>
            ValueNodeError.InvalidName(nodeType);

        public static NodeException MissingId(string nodeType) =>
            ValueNodeError.MissingId(nodeType);
    }
}