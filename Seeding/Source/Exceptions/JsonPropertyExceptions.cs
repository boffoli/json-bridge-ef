namespace JsonBridgeEF.Seeding.Source.Exceptions
{
    using JsonBridgeEF.Shared.EntityModel.Exceptions;

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutte le anomalie relative alle proprietà JSON all’interno dei blocchi.</para>
    /// <para><b>Usage:</b><br/>
    /// Estesa da eccezioni concrete sollevate in caso di conflitti, errori di chiave o assenza.</para>
    /// </summary>
    public abstract class JsonPropertyException : EntityPropertyException
    {
        public string FieldName { get; }
        public string? JsonEntityName { get; }

        // Costruttore che accetta un messaggio
        protected JsonPropertyException(string fieldName, string? jsonEntityName, string message)
            : base(message)  // Chiamata al costruttore della classe base (EntityPropertyException)
        {
            FieldName = fieldName;
            JsonEntityName = jsonEntityName;
        }

        // Costruttore con eccezione interna
        protected JsonPropertyException(string fieldName, string? jsonEntityName, string message, Exception inner)
            : base(message, inner)  // Chiamata al costruttore della classe base (EntityPropertyException) con inner exception
        {
            FieldName = fieldName;
            JsonEntityName = jsonEntityName;
        }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando una proprietà con lo stesso nome è già presente all’interno del blocco.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da metodi come <c>TryAddProperty</c> durante la creazione delle proprietà.</para>
    /// </summary>
    internal sealed class JsonPropertyAlreadyExistsException : JsonPropertyException
    {
        // Costruttore
        private JsonPropertyAlreadyExistsException(string fieldName, string? jsonEntityName)
            : base(fieldName, jsonEntityName, $"❌ La proprietà '{fieldName}' non può essere aggiunta perché esiste già nel blocco '{jsonEntityName ?? "?"}'.") { }

        // Costruttore con eccezione interna
        private JsonPropertyAlreadyExistsException(string fieldName, string? jsonEntityName, Exception inner)
            : base(fieldName, jsonEntityName, $"❌ La proprietà '{fieldName}' non può essere aggiunta perché esiste già nel blocco '{jsonEntityName ?? "?"}'.", inner) { }

        /// <summary>
        /// Factory per proprietà duplicata.
        /// </summary>
        public static JsonPropertyAlreadyExistsException AlreadyExists(string fieldName, string? jsonEntityName) =>
            new(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per proprietà duplicata con eccezione interna.
        /// </summary>
        public static JsonPropertyAlreadyExistsException AlreadyExists(string fieldName, string? jsonEntityName, Exception inner) =>
            new(fieldName, jsonEntityName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando una chiave è già presente nella proprietà e non è consentita la sovrascrizione.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da metodi come <c>MarkAsKey</c> quando si tenta di sovrascrivere una chiave esistente.</para>
    /// </summary>
    internal sealed class JsonPropertyKeyAlreadyPresentException : JsonPropertyException
    {
        // Costruttore
        private JsonPropertyKeyAlreadyPresentException(string fieldName, string? jsonEntityName)
            : base(fieldName, jsonEntityName, $"❌ Esiste già una proprietà chiave diversa nel blocco '{jsonEntityName ?? "?"}': '{fieldName}'.") { }

        // Costruttore con eccezione interna
        private JsonPropertyKeyAlreadyPresentException(string fieldName, string? jsonEntityName, Exception inner)
            : base(fieldName, jsonEntityName, $"❌ Esiste già una proprietà chiave diversa nel blocco '{jsonEntityName ?? "?"}': '{fieldName}'.", inner) { }

        /// <summary>
        /// Factory per chiave già presente.
        /// </summary>
        public static JsonPropertyKeyAlreadyPresentException KeyAlreadyPresent(string fieldName, string? jsonEntityName) =>
            new(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per chiave già presente con eccezione interna.
        /// </summary>
        public static JsonPropertyKeyAlreadyPresentException KeyAlreadyPresent(string fieldName, string? jsonEntityName, Exception inner) =>
            new(fieldName, jsonEntityName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la proprietà indicata non è stata trovata nel blocco.</para>
    /// <para><b>Usage:</b><br/>
    /// Sollevata da metodi come <c>SetKey</c> quando la proprietà specificata non è trovata nella collezione.</para>
    /// </summary>
    internal sealed class JsonPropertyNotFoundException : JsonPropertyException
    {
        // Costruttore
        private JsonPropertyNotFoundException(string fieldName, string? jsonEntityName)
            : base(fieldName, jsonEntityName, $"❌ La proprietà '{fieldName}' non è stata trovata nel blocco '{jsonEntityName ?? "?"}'.") { }

        // Costruttore con eccezione interna
        private JsonPropertyNotFoundException(string fieldName, string? jsonEntityName, Exception inner)
            : base(fieldName, jsonEntityName, $"❌ La proprietà '{fieldName}' non è stata trovata nel blocco '{jsonEntityName ?? "?"}'.", inner) { }

        /// <summary>
        /// Factory per proprietà non trovata.
        /// </summary>
        public static JsonPropertyNotFoundException NotFound(string fieldName, string? jsonEntityName) =>
            new(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per proprietà non trovata con eccezione interna.
        /// </summary>
        public static JsonPropertyNotFoundException NotFound(string fieldName, string? jsonEntityName, Exception inner) =>
            new(fieldName, jsonEntityName, inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la descrizione della proprietà supera la lunghezza massima consentita.</para>
    /// </summary>
    internal sealed class JsonPropertyDescriptionTooLongException : JsonPropertyException
    {
        public JsonPropertyDescriptionTooLongException(string fieldName, string? jsonEntityName, int maxLength)
            : base(fieldName, jsonEntityName, $"❌ La descrizione della proprietà '{fieldName}' nel blocco '{jsonEntityName ?? "?"}' supera la lunghezza massima consentita di {maxLength} caratteri.") { }

        // Doppio costruttore con inner exception
        public JsonPropertyDescriptionTooLongException(string fieldName, string? jsonEntityName, int maxLength, Exception inner)
            : base(fieldName, jsonEntityName, $"❌ La descrizione della proprietà '{fieldName}' nel blocco '{jsonEntityName ?? "?"}' supera la lunghezza massima consentita di {maxLength} caratteri.", inner) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per la generazione di errori legati a proprietà JSON nei blocchi.</para>
    /// </summary>
    internal static class JsonPropertyError
    {
        /// <summary>
        /// Factory per proprietà duplicata.
        /// </summary>
        public static JsonPropertyException AlreadyExists(string fieldName, string? jsonEntityName) =>
            JsonPropertyAlreadyExistsException.AlreadyExists(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per chiave già presente.
        /// </summary>
        public static JsonPropertyException KeyAlreadyPresent(string fieldName, string? jsonEntityName) =>
            JsonPropertyKeyAlreadyPresentException.KeyAlreadyPresent(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per proprietà non trovata.
        /// </summary>
        public static JsonPropertyException NotFound(string fieldName, string? jsonEntityName) =>
            JsonPropertyNotFoundException.NotFound(fieldName, jsonEntityName);

        /// <summary>
        /// Factory per descrizione troppo lunga.
        /// </summary>
        public static JsonPropertyException DescriptionTooLong(string fieldName, string? jsonEntityName, int maxLength) =>
            new JsonPropertyDescriptionTooLongException(fieldName, jsonEntityName, maxLength);

        // ==== Composizione: metodi da EntityPropertyError ====

        /// <summary>
        /// Factory per valore non valido.
        /// </summary>
        public static EntityPropertyException InvalidValue(string nodeName) =>
            EntityPropertyError.InvalidValue(nodeName);

        /// <summary>
        /// Factory per valore non valido con eccezione interna.
        /// </summary>
        public static EntityPropertyException InvalidValue(string nodeName, Exception inner) =>
            EntityPropertyError.InvalidValue(nodeName, inner);

        /// <summary>
        /// Factory per chiave duplicata nelle proprietà.
        /// </summary>
        public static EntityPropertyException DuplicateKey(string nodeName, string key) =>
            EntityPropertyError.DuplicateKey(nodeName, key);

        /// <summary>
        /// Factory per chiave duplicata nelle proprietà con eccezione interna.
        /// </summary>
        public static EntityPropertyException DuplicateKey(string nodeName, string key, Exception inner) =>
            EntityPropertyError.DuplicateKey(nodeName, key, inner);

        /// <summary>
        /// Factory per parent nullo nelle proprietà.
        /// </summary>
        public static EntityPropertyException NullParent(string nodeName) =>
            EntityPropertyError.NullParent(nodeName);

        /// <summary>
        /// Factory per parent nullo nelle proprietà con eccezione interna.
        /// </summary>
        public static EntityPropertyException NullParent(string nodeName, Exception inner) =>
            EntityPropertyError.NullParent(nodeName, inner);
    }
}