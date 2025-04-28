namespace JsonBridgeEF.Seeding.Source.Exceptions
{
    using JsonBridgeEF.Shared.Dag.Exceptions;
    using JsonBridgeEF.Shared.EntityModel.Exceptions;

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione dei blocchi JSON.</para>
    /// <para><b>Usage:</b><br/>
    /// Viene utilizzata come base per eccezioni semantiche come blocchi mancanti o promozioni fallite.</para>
    /// </summary>
    public abstract class JsonEntityException : EntityException
    {
        protected JsonEntityException(string message) : base(message) { }

        // Costruttore per gestire eccezioni interne
        protected JsonEntityException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione generica per errori semantici generici legati ai blocchi JSON.</para>
    /// <para><b>Usage:</b><br/>
    /// Usata come fallback interno da factory statiche su <see cref="JsonEntityException"/>.</para>
    /// </summary>
    internal sealed class JsonEntityGenericException : JsonEntityException
    {
        public JsonEntityGenericException(string message) : base(message) { }
        public JsonEntityGenericException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando si tenta di aggiungere un blocco con nome già esistente all’interno dello schema.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da <c>JsonSchemaJsonEntityCollection</c> quando si viola il vincolo di unicità del nome.</para>
    /// </summary>
    internal sealed class JsonEntityAlreadyExistsException : JsonEntityException
    {
        public string JsonEntityName { get; }

        private JsonEntityAlreadyExistsException(string message, string jsonEntityName)
            : base(message)
        {
            JsonEntityName = jsonEntityName;
        }

        public static JsonEntityAlreadyExistsException AlreadyExists(string jsonEntityName) =>
            new($"❌ Esiste già un blocco con il nome '{jsonEntityName}.'", jsonEntityName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando un blocco specificato non è stato trovato all’interno dello schema JSON.</para>
    /// </summary>
    internal sealed class JsonEntityNotFoundException : JsonEntityException
    {
        public string JsonEntityName { get; }
        public string SchemaName { get; }

        private JsonEntityNotFoundException(string message, string jsonEntityName, string schemaName)
            : base(message)
        {
            JsonEntityName = jsonEntityName;
            SchemaName = schemaName;
        }

        public static JsonEntityNotFoundException NotFound(string jsonEntityName, string schemaName) =>
            new($"❌ Blocco '{jsonEntityName}' non trovato nello schema '{schemaName}.'", jsonEntityName, schemaName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando fallisce il tentativo di promuovere un blocco JSON come indipendente.</para>
    /// </summary>
    internal sealed class JsonEntityPromotionException : JsonEntityException
    {
        public string JsonEntityName { get; }
        public string KeyFieldName { get; }

        private JsonEntityPromotionException(string message, string jsonEntityName, string keyFieldName)
            : base(message)
        {
            JsonEntityName = jsonEntityName;
            KeyFieldName = keyFieldName;
        }

        public static JsonEntityPromotionException PromotionFailed(string jsonEntityName, string keyFieldName, string reason) =>
            new($"❌ Impossibile promuovere il blocco '{jsonEntityName}' come indipendente con chiave '{keyFieldName}': {reason}", jsonEntityName, keyFieldName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la descrizione di un'entità JSON supera la lunghezza massima consentita.</para>
    /// </summary>
    internal sealed class JsonEntityDescriptionTooLongException : JsonEntityException
    {
        public JsonEntityDescriptionTooLongException(string jsonEntityName, int maxLength)
            : base($"❌ La descrizione dell'entità '{jsonEntityName}' supera la lunghezza massima consentita di {maxLength} caratteri.") { }

        // Doppio costruttore con inner exception
        public JsonEntityDescriptionTooLongException(string jsonEntityName, int maxLength, Exception inner)
            : base($"❌ La descrizione dell'entità '{jsonEntityName}' supera la lunghezza massima consentita di {maxLength} caratteri.", inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per istanziare errori di validazione generici sui blocchi JSON.</para>
    /// </summary>
    internal static class JsonEntityError
    {
        // ==== Factory specifiche per JsonEntity ====
         // Aggiungiamo un nuovo metodo per la validazione di una JsonEntity.
        public static JsonEntityException InvalidJsonEntity() =>
            new JsonEntityGenericException($"❌ La entità JSON fornita non è valida (null o vuota).");

        public static JsonEntityException DescriptionTooLong(string jsonEntityName, int maxLength) =>
            new JsonEntityDescriptionTooLongException(jsonEntityName, maxLength);

        public static JsonEntityException InvalidSchemaReference() =>
            new JsonEntityGenericException("Il blocco JSON deve avere un riferimento valido allo schema.");

        public static JsonEntityException KeyPromotionFailed(string propertyName) =>
            new JsonEntityGenericException($"Nessuna proprietà trovata con il nome '{propertyName}' o oggetto già identificabile.");

        public static JsonEntityException InvalidTypeProperty(string entityName, string propertyName, string type) =>
            new JsonEntityGenericException($"❌ La proprietà '{propertyName}' nel blocco '{entityName}' ha un tipo non supportato: '{type}'.");

        public static JsonEntityException JsonBlockNotFound(string jsonEntityName) =>
            new JsonEntityGenericException($"❌ Il blocco JSON '{jsonEntityName}' non è stato trovato nel documento.");

        public static JsonEntityException MissingTypeForProperty(string propertyName, string jsonEntityName) =>
            new JsonEntityGenericException($"❌ Il campo '{propertyName}' nel blocco '{jsonEntityName}' non specifica il tipo.");

        public static JsonEntityException MissingSchemaReference(string? jsonEntityName = null) =>
            new JsonEntityGenericException(
                $"Il blocco JSON{(jsonEntityName is not null ? $" '{jsonEntityName}'" : string.Empty)} deve avere un riferimento valido allo schema.");

        // ==== Composizione: Factory da EntityError ====

        /// <inheritdoc cref="EntityError.DuplicateKey"/>
        public static EntityException DuplicateKey(string entityName, string keyName) =>
            EntityError.DuplicateKey(entityName, keyName);

        /// <inheritdoc cref="EntityError.MissingKey"/>
        public static EntityException MissingKey(string entityName) =>
            EntityError.MissingKey(entityName);

        /// <inheritdoc cref="EntityError.NullParentCollection"/>
        public static EntityException NullParentCollection(string entityName) =>
            EntityError.NullParentCollection(entityName);

        /// <inheritdoc cref="EntityError.DuplicateParents"/>
        public static EntityException DuplicateParents(string entityName) =>
            EntityError.DuplicateParents(entityName);

        /// <inheritdoc cref="EntityError.SelfReference"/>
        public static EntityException SelfReference(string entityName) =>
            EntityError.SelfReference(entityName);

        // ==== Composizione: Factory da AggregateNodeError ====

        /// <inheritdoc cref="AggregateNodeError.CycleDetected"/>
        public static AggregateNodeException CycleDetected(string parentName, string childName) =>
            EntityError.CycleDetected(parentName, childName);

        /// <inheritdoc cref="AggregateNodeError.NullChild"/>
        public static AggregateNodeException NullChild(string parentName) =>
            EntityError.NullChild(parentName);

        /// <inheritdoc cref="AggregateNodeError.DuplicateChildKey"/>
        public static AggregateNodeException DuplicateChildKey(string parentName, string duplicatedKey) =>
            EntityError.DuplicateChildKey(parentName, duplicatedKey);

        /// <inheritdoc cref="NodeError.InvalidName"/>
        public static NodeException InvalidName(string nodeType) =>
            EntityError.InvalidName(nodeType);

        /// <inheritdoc cref="NodeError.MissingId"/>
        public static NodeException MissingId(string nodeType) =>
            EntityError.MissingId(nodeType);
    }
}