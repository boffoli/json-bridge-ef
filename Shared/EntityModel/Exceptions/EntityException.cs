using JsonBridgeEF.Shared.Dag.Exceptions;

namespace JsonBridgeEF.Shared.EntityModel.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutte le anomalie semantiche legate alle entità di dominio generiche.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni concrete relative a vincoli violati come duplicati, mancanze, nullità o autoreferenze.</para>
    /// </summary>
    public abstract class EntityException : AggregateNodeException
    {
        // Aggiunto il costruttore con il messaggio e l'eccezione interna
        protected EntityException(string message) : base(message) { }

        protected EntityException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando un'entità ha già una chiave definita e si tenta di ridefinirla.</para>
    /// </summary>
    internal sealed class EntityDuplicateKeyException(string entityName, string keyName) : EntityException($"❌ L'entità '{entityName}' ha già una proprietà chiave: '{keyName}'.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando un'entità identificabile non presenta una chiave.</para>
    /// </summary>
    internal sealed class EntityMissingKeyException(string entityName) : EntityException($"❌ L'entità '{entityName}' è marcata come identificabile ma non espone una proprietà chiave.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la collezione di genitori è nulla.</para>
    /// </summary>
    internal sealed class EntityNullParentCollectionException(string entityName) : EntityException($"❌ L'entità '{entityName}' ha una collezione di genitori nulla.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando ci sono duplicati nella collezione di genitori.</para>
    /// </summary>
    internal sealed class EntityDuplicateParentsException(string entityName) : EntityException($"❌ L'entità '{entityName}' contiene genitori duplicati nella collezione.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando un'entità si riferisce a sé stessa come genitore o figlio.</para>
    /// </summary>
    internal sealed class EntitySelfReferenceException(string entityName) : EntityException($"❌ L'entità '{entityName}' non può essere suo proprio genitore o figlio.")
    {
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory centralizzata per istanziare eccezioni semantiche legate ad entità generiche.</para>
    /// </summary>
    internal static class EntityError
    {
        /// <summary>
        /// Factory per chiave duplicata.
        /// </summary>
        public static EntityException DuplicateKey(string entityName, string keyName) =>
            new EntityDuplicateKeyException(entityName, keyName);

        /// <summary>
        /// Factory per chiave mancante.
        /// </summary>
        public static EntityException MissingKey(string entityName) =>
            new EntityMissingKeyException(entityName);

        /// <summary>
        /// Factory per collezione di genitori nulla.
        /// </summary>
        public static EntityException NullParentCollection(string entityName) =>
            new EntityNullParentCollectionException(entityName);

        /// <summary>
        /// Factory per genitori duplicati.
        /// </summary>
        public static EntityException DuplicateParents(string entityName) =>
            new EntityDuplicateParentsException(entityName);

        /// <summary>
        /// Factory per self-reference.
        /// </summary>
        public static EntityException SelfReference(string entityName) =>
            new EntitySelfReferenceException(entityName);
    }
}