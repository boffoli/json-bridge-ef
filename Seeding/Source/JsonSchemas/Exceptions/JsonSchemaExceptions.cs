namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione dei JSON Schema.</para>
    /// </summary>
    public abstract class JsonSchemaException : Exception
    {
        /// <summary>
        /// Crea una nuova eccezione di tipo <see cref="JsonSchemaException"/>.
        /// </summary>
        protected JsonSchemaException(string message) 
            : base(message) 
        { }

        /// <summary>
        /// Crea una nuova eccezione di tipo <see cref="JsonSchemaException"/>, con eccezione interna.
        /// </summary>
        protected JsonSchemaException(string message, Exception innerException) 
            : base(message, innerException) 
        { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione generica per fallback semantici sugli schemi JSON.</para>
    /// <para><b>Usage:</b><br/>
    /// Usata internamente da <see cref="JsonSchemaError"/> come default.</para>
    /// </summary>
    internal sealed class JsonSchemaGenericException(string message)
        : JsonSchemaException(message)
    { }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando si tenta di operare su un’entità che non appartiene allo schema specificato.</para>
    /// </summary>
    /// <remarks>
    /// Crea una nuova eccezione di ownership mismatch.
    /// </remarks>
    internal sealed class EntityOwnershipMismatchException(string entityName, string schemaName) : JsonSchemaException($"❌ L'entità JSON '{entityName}' non appartiene allo schema '{schemaName}'.")
    {
        /// <summary>
        /// Nome dell’entità JSON che non appartiene allo schema.
        /// </summary>
        public string EntityName { get; } = entityName;

        /// <summary>
        /// Nome dello schema con cui c’è disallineamento.
        /// </summary>
        public string SchemaName { get; } = schemaName;
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory centralizzata per generare eccezioni relative agli schemi JSON.</para>
    /// </summary>
    internal static class JsonSchemaError
    {
        /// <summary>
        /// Schema nullo o vuoto.
        /// </summary>
        public static JsonSchemaException InvalidSchemaReference() =>
            new JsonSchemaGenericException("Lo schema JSON fornito non è valido (null o vuoto).");

        /// <summary>
        /// Nome dello schema non valido.
        /// </summary>
        public static JsonSchemaException InvalidName(string schemaType) =>
            new JsonSchemaGenericException($"❌ Il nome dello schema '{schemaType}' non può essere nullo o vuoto.");

        /// <summary>
        /// Percorso file JSON nullo o vuoto.
        /// </summary>
        public static JsonSchemaException InvalidFilePath() =>
            new JsonSchemaGenericException("Il percorso del file è nullo o vuoto.");

        /// <summary>
        /// Contenuto JSON non valido o malformato.
        /// </summary>
        public static JsonSchemaException InvalidJsonContent(string? message = null, Exception? inner = null) =>
            JsonSchemaInvalidContentException.Invalid(message, inner);

        /// <summary>
        /// File JSON di input non trovato.
        /// </summary>
        public static JsonSchemaException FileNotFound(string filePath) =>
            JsonSchemaFileNotFoundException.NotFound(filePath);

        /// <summary>
        /// Nome schema già presente nel database.
        /// </summary>
        public static JsonSchemaException NameAlreadyExists(string schemaName) =>
            JsonSchemaNameAlreadyExistsException.AlreadyExists(schemaName);

        /// <summary>
        /// Contenuto schema duplicato già presente nel database.
        /// </summary>
        public static JsonSchemaException ContentAlreadyExists(string existingSchemaName) =>
            JsonSchemaContentAlreadyExistsException.AlreadyExists(existingSchemaName);

        /// <summary>
        /// Descrizione dello schema supera la lunghezza massima consentita.
        /// </summary>
        public static JsonSchemaException DescriptionTooLong(string schemaName, int maxLength) =>
            JsonSchemaDescriptionTooLongException.Exceeding(schemaName, maxLength);

        /// <summary>
        /// Ownership mismatch tra entità JSON e schema.
        /// </summary>
        public static JsonSchemaException EntityOwnershipMismatch(string entityName, string schemaName) =>
            new EntityOwnershipMismatchException(entityName, schemaName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il nome dello schema JSON è già presente nel sistema.</para>
    /// </summary>
    internal sealed class JsonSchemaNameAlreadyExistsException : JsonSchemaException
    {
        /// <summary>Nome dello schema duplicato.</summary>
        public string SchemaName { get; }

        private JsonSchemaNameAlreadyExistsException(string schemaName)
            : base($"❌ Lo schema '{schemaName}' esiste già nel database.")
        {
            SchemaName = schemaName;
        }

        /// <summary>Factory per nome già esistente.</summary>
        public static JsonSchemaNameAlreadyExistsException AlreadyExists(string schemaName) =>
            new(schemaName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il contenuto dello schema JSON è già presente nel sistema.</para>
    /// </summary>
    internal sealed class JsonSchemaContentAlreadyExistsException : JsonSchemaException
    {
        /// <summary>Nome dello schema esistente con contenuto identico.</summary>
        public string ExistingSchemaName { get; }

        private JsonSchemaContentAlreadyExistsException(string existingSchemaName)
            : base($"❌ Uno schema identico ('{existingSchemaName}') esiste già nel database.")
        {
            ExistingSchemaName = existingSchemaName;
        }

        /// <summary>Factory per contenuto già esistente.</summary>
        public static JsonSchemaContentAlreadyExistsException AlreadyExists(string existingSchemaName) =>
            new(existingSchemaName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il file JSON di input non viene trovato nel file system.</para>
    /// </summary>
    internal sealed class JsonSchemaFileNotFoundException : JsonSchemaException
    {
        /// <summary>Percorso file non trovato.</summary>
        public string FilePath { get; }

        private JsonSchemaFileNotFoundException(string filePath)
            : base($"❌ File JSON non trovato: {filePath}")
        {
            FilePath = filePath;
        }

        /// <summary>Factory per file non trovato.</summary>
        public static JsonSchemaFileNotFoundException NotFound(string filePath) =>
            new(filePath);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il contenuto JSON non è valido o è nullo.</para>
    /// </summary>
    internal sealed class JsonSchemaInvalidContentException : JsonSchemaException
    {
        private JsonSchemaInvalidContentException(string message, Exception? inner)
            : base(message, inner ?? new Exception()) { }

        /// <summary>Factory per contenuto JSON invalido.</summary>
        public static JsonSchemaInvalidContentException Invalid(string? message = null, Exception? inner = null) =>
            new(message ?? "❌ Il contenuto JSON non può essere nullo o non valido.", inner);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la descrizione di uno schema supera la lunghezza massima consentita.</para>
    /// </summary>
    internal sealed class JsonSchemaDescriptionTooLongException : JsonSchemaException
    {
        private JsonSchemaDescriptionTooLongException(string schemaName, int maxLength)
            : base($"❌ La descrizione dello schema '{schemaName}' non può superare {maxLength} caratteri.")
        { }

        /// <summary>Factory per descrizione troppo lunga.</summary>
        public static JsonSchemaDescriptionTooLongException Exceeding(string schemaName, int maxLength) =>
            new(schemaName, maxLength);
    }
}