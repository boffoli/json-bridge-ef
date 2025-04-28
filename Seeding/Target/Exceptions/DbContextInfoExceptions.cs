namespace JsonBridgeEF.Seeding.Target.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione dei metadati del DbContext.</para>
    /// </summary>
    public abstract class DbContextInfoException : Exception
    {
        protected DbContextInfoException(string message) : base(message) { }

        protected DbContextInfoException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il nome del DbContext non è valido.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il nome del DbContext non può essere nullo o vuoto.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoInvalidNameException : DbContextInfoException
    {
        public DbContextInfoInvalidNameException(string name)
            : base($"❌ Il nome del DbContext '{name}' non è valido.") { }

        public static DbContextInfoInvalidNameException InvalidName(string name) =>
            new(name);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il namespace del DbContext non è valido.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il namespace del DbContext non può essere nullo o vuoto.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoInvalidNamespaceException : DbContextInfoException
    {
        public DbContextInfoInvalidNamespaceException(string ns)
            : base($"❌ Il namespace del DbContext '{ns}' non è valido.") { }

        public static DbContextInfoInvalidNamespaceException InvalidNamespace(string ns) =>
            new(ns);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il `ClassQualifiedName` del DbContext non è valido.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il ClassQualifiedName del DbContext deve corrispondere al formato previsto.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoInvalidClassQualifiedNameException : DbContextInfoException
    {
        public DbContextInfoInvalidClassQualifiedNameException(string expected, string actual)
            : base($"❌ Il ClassQualifiedName '{actual}' non corrisponde al valore atteso '{expected}'.") { }

        public static DbContextInfoInvalidClassQualifiedNameException InvalidClassQualifiedName(string expected, string actual) =>
            new(expected, actual);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la descrizione del DbContext supera la lunghezza massima consentita.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> La lunghezza della descrizione del DbContext deve essere inferiore al valore massimo consentito.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoDescriptionTooLongException : DbContextInfoException
    {
        public DbContextInfoDescriptionTooLongException(int maxLength)
            : base($"❌ La descrizione del DbContext non può superare {maxLength} caratteri.") { }

        public static DbContextInfoDescriptionTooLongException TooLong(int maxLength) =>
            new(maxLength);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando non è stato trovato un DbContext nel namespace specificato.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il DbContext non è stato trovato nel namespace fornito.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoNotFoundException : DbContextInfoException
    {
        public DbContextInfoNotFoundException(string ns)
            : base($"❌ Nessun DbContext trovato nel namespace '{ns}'.") { }

        public static DbContextInfoNotFoundException NotFound(string ns) =>
            new(ns);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando sono stati trovati più di un DbContext nel namespace specificato.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Più di un DbContext trovato nel namespace specificato.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoMultipleFoundException : DbContextInfoException
    {
        public DbContextInfoMultipleFoundException(string ns, string[] dbContexts)
            : base($"❌ Più di un DbContext trovato nel namespace '{ns}': {string.Join(", ", dbContexts)}") { }

        public static DbContextInfoMultipleFoundException MultipleFound(string ns, string[] dbContexts) =>
            new(ns, dbContexts);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando l'istanza del DbContext non può essere creata.</para>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il DbContext non può essere instanziato correttamente.</para>
    /// </remarks>
    /// </summary>
    internal sealed class DbContextInfoInstantiationFailedException : DbContextInfoException
    {
        public DbContextInfoInstantiationFailedException(string className)
            : base($"❌ Impossibile istanziare il DbContext '{className}'.") { }

        public static DbContextInfoInstantiationFailedException InstantiationFailed(string className) =>
            new(className);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per istanziare errori legati alla gestione dei metadati del DbContext.</para>
    /// <remarks>
    /// <para><b>Usage:</b><br/>
    /// Viene utilizzata per centralizzare la creazione di eccezioni specifiche relative al DbContext.</para>
    /// </remarks>
    /// </summary>
    internal static class DbContextInfoError
    {
        /// <summary>
        /// Factory per nome del DbContext non valido.
        /// </summary>
        public static DbContextInfoException InvalidDbContextName(string name) =>
            DbContextInfoInvalidNameException.InvalidName(name);

        /// <summary>
        /// Factory per namespace del DbContext non valido.
        /// </summary>
        public static DbContextInfoException InvalidDbContextNamespace(string ns) =>
            DbContextInfoInvalidNamespaceException.InvalidNamespace(ns);

        /// <summary>
        /// Factory per ClassQualifiedName del DbContext non valido.
        /// </summary>
        public static DbContextInfoException InvalidDbContextClassQualifiedName(string expected, string actual) =>
            DbContextInfoInvalidClassQualifiedNameException.InvalidClassQualifiedName(expected, actual);

        /// <summary>
        /// Factory per descrizione del DbContext troppo lunga.
        /// </summary>
        public static DbContextInfoException DescriptionTooLong(int maxLength) =>
            DbContextInfoDescriptionTooLongException.TooLong(maxLength);

        /// <summary>
        /// Factory per DbContext non trovato nel namespace.
        /// </summary>
        public static DbContextInfoException DbContextNotFound(string ns) =>
            DbContextInfoNotFoundException.NotFound(ns);

        /// <summary>
        /// Factory per più di un DbContext trovato nel namespace.
        /// </summary>
        public static DbContextInfoException MultipleDbContextsFound(string ns, string[] dbContexts) =>
            DbContextInfoMultipleFoundException.MultipleFound(ns, dbContexts);

        /// <summary>
        /// Factory per fallimento nell'instanziare il DbContext.
        /// </summary>
        public static DbContextInfoException DbContextInstantiationFailed(string className) =>
            DbContextInfoInstantiationFailedException.InstantiationFailed(className);
    }
}