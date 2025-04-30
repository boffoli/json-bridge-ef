namespace JsonBridgeEF.Shared.EfPersistance.Exceptions
{
    /// <summary>
    /// Domain Concept: Eccezione base per anomalie semantiche relative a <see cref="IEfEntity"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Viene sollevata da validatori o da componenti di persistenza quando la chiave tecnica non è valida.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Estende <see cref="InvalidOperationException"/> per indicare errori di configurazione o stato.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare la factory <see cref="EfEntityError"/> per creare eccezioni specifiche.</para>
    /// </remarks>
    public abstract class EfEntityException : InvalidOperationException
    {
        /// <summary>
        /// Crea una nuova eccezione di tipo <see cref="EfEntityException"/>.
        /// </summary>
        protected EfEntityException(string message) : base(message) { }

        /// <summary>
        /// Crea una nuova eccezione di tipo <see cref="EfEntityException"/>, con eccezione interna.
        /// </summary>
        protected EfEntityException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando la chiave tecnica (Id) non è stata impostata (vale zero).
    /// </summary>
    internal sealed class EfEntityMissingIdException : EfEntityException
    {
        /// <summary>
        /// Crea una nuova eccezione per Id mancante.
        /// </summary>
        /// <param name="entityName">Nome dell'entità EF.</param>
        public EfEntityMissingIdException(string entityName)
            : base($"❌ L'entità EF '{entityName}' non ha un valore valido per la chiave primaria tecnica (Id).")
        { }
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando la chiave tecnica (Id) è negativa.
    /// </summary>
    internal sealed class EfEntityInvalidIdException : EfEntityException
    {
        /// <summary>
        /// Crea una nuova eccezione per Id negativo.
        /// </summary>
        /// <param name="entityName">Nome dell'entità EF.</param>
        /// <param name="idValue">Valore non valido di Id.</param>
        public EfEntityInvalidIdException(string entityName, int idValue)
            : base($"❌ L'entità EF '{entityName}' ha un Id non valido ({idValue}); deve essere ≥ 0.")
        { }
    }

    /// <summary>
    /// Domain Concept: Factory centralizzata per istanziare eccezioni semantiche legate a <see cref="IEfEntity"/>.
    /// </summary>
    internal static class EfEntityError
    {
        /// <summary>
        /// Id mancante (zero).
        /// </summary>
        public static EfEntityException MissingId(string entityName) =>
            new EfEntityMissingIdException(entityName);

        /// <summary>
        /// Id non valido (negativo).
        /// </summary>
        public static EfEntityException InvalidId(string entityName, int idValue) =>
            new EfEntityInvalidIdException(entityName, idValue);

        /// <summary>
        /// Nome del nodo non valido (delegato a <see cref="JsonBridgeEF.Shared.Dag.Exceptions.NodeError.InvalidName(string)"/>).
        /// </summary>
        public static Exception InvalidEntityName(string entityName) =>
            JsonBridgeEF.Shared.Dag.Exceptions.NodeError.InvalidName(entityName);
    }
}