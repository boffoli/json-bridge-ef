namespace JsonBridgeEF.Seeding.Source.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione dei JSON Schema.</para>
    /// </summary>
    public abstract class JsonSchemaException : Exception
    {
        protected JsonSchemaException(string message) : base(message) { }

        protected JsonSchemaException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione generica per fallback semantici sugli schemi JSON.</para>
    /// <para><b>Usage:</b><br/>
    /// Usata internamente da <see cref="JsonSchemaError"/> come default.</para>
    /// </summary>
    internal sealed class JsonSchemaGenericException(string message) : JsonSchemaException(message) { }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory centralizzata per generare eccezioni relative agli schemi JSON.</para>
    /// </summary>
    internal static class JsonSchemaError
    {
         public static JsonSchemaException InvalidSchemaReference() =>
            new JsonSchemaGenericException("Lo schema JSON fornito non è valido (null o vuoto).");

        // Invalid Name con tipo
        public static JsonSchemaException InvalidName(string schemaType) =>
            new JsonSchemaGenericException($"❌ Il nome dello schema '{schemaType}' non può essere nullo o vuoto.");

        // Invalid FilePath
        public static JsonSchemaException InvalidFilePath() =>
            new JsonSchemaGenericException("Il percorso del file è nullo o vuoto.");

        // Invalid Json Content
        public static JsonSchemaException InvalidJsonContent(string? message = null, Exception? inner = null) =>
            JsonSchemaInvalidContentException.Invalid(message, inner);

        // File Not Found
        public static JsonSchemaException FileNotFound(string filePath) =>
            JsonSchemaFileNotFoundException.NotFound(filePath);

        // Name Already Exists
        public static JsonSchemaException NameAlreadyExists(string schemaName) =>
            JsonSchemaNameAlreadyExistsException.AlreadyExists(schemaName);

        // Content Already Exists
        public static JsonSchemaException ContentAlreadyExists(string existingSchemaName) =>
            JsonSchemaContentAlreadyExistsException.AlreadyExists(existingSchemaName);

        // Description Too Long con nome schema
        public static JsonSchemaException DescriptionTooLong(string schemaName, int maxLength) =>
            JsonSchemaDescriptionTooLongException.Exceeding(schemaName, maxLength);
    }

    /// <summary>
    /// Eccezione sollevata quando il nome dello schema JSON è già presente nel sistema.
    /// </summary>
    internal sealed class JsonSchemaNameAlreadyExistsException : JsonSchemaException
    {
        public string SchemaName { get; }

        private JsonSchemaNameAlreadyExistsException(string schemaName)
            : base($"❌ Lo schema '{schemaName}' esiste già nel database.")
        {
            SchemaName = schemaName;
        }

        public static JsonSchemaNameAlreadyExistsException AlreadyExists(string schemaName) =>
            new(schemaName);
    }

    /// <summary>
    /// Eccezione sollevata quando il contenuto dello schema JSON è già presente nel sistema.
    /// </summary>
    internal sealed class JsonSchemaContentAlreadyExistsException : JsonSchemaException
    {
        public string ExistingSchemaName { get; }

        private JsonSchemaContentAlreadyExistsException(string existingSchemaName)
            : base($"❌ Uno schema identico ('{existingSchemaName}') esiste già nel database.")
        {
            ExistingSchemaName = existingSchemaName;
        }

        public static JsonSchemaContentAlreadyExistsException AlreadyExists(string existingSchemaName) =>
            new(existingSchemaName);
    }

    /// <summary>
    /// Eccezione sollevata quando il file JSON di input non viene trovato nel file system.
    /// </summary>
    internal sealed class JsonSchemaFileNotFoundException : JsonSchemaException
    {
        public string FilePath { get; }

        private JsonSchemaFileNotFoundException(string filePath)
            : base($"❌ File JSON non trovato: {filePath}")
        {
            FilePath = filePath;
        }

        public static JsonSchemaFileNotFoundException NotFound(string filePath) =>
            new(filePath);
    }

    /// <summary>
    /// Eccezione sollevata quando il contenuto JSON non è valido o è nullo.
    /// </summary>
    internal sealed class JsonSchemaInvalidContentException : JsonSchemaException
    {
        private JsonSchemaInvalidContentException(string message, Exception? innerException)
            : base(message, innerException ?? new Exception()) { }

        public static JsonSchemaInvalidContentException Invalid(string? message = null, Exception? inner = null) =>
            new(message ?? "❌ Il contenuto JSON non può essere nullo o non valido.", inner);
    }

    /// <summary>
    /// Eccezione sollevata quando la descrizione di uno schema supera la lunghezza massima consentita.
    /// </summary>
    internal sealed class JsonSchemaDescriptionTooLongException : JsonSchemaException
    {
        private JsonSchemaDescriptionTooLongException(string schemaName, int maxLength)
            : base($"❌ La descrizione dello schema '{schemaName}' non può superare {maxLength} caratteri.") { }

        public static JsonSchemaDescriptionTooLongException Exceeding(string schemaName, int maxLength) =>
            new(schemaName, maxLength);
    }
}