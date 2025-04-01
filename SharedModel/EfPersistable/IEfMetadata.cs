namespace JsonBridgeEF.SharedModel.EfPersistable
{
    /// <summary>
    /// Domain Interface: Rappresenta un'entità persistente che aggrega identità, descrizione, slug e metadati temporali.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Interfaccia centrale per entità gestite da Entity Framework che espongono metadati semantici e di persistenza.
    /// </para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Da implementare direttamente nelle classi entità che richiedono tracciamento e identificazione.
    /// </para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere implementata da tutte le entità persistenti.<br/>
    /// - L'identificativo deve essere univoco e stabile nel tempo.
    /// </para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Estende <see cref="IIdentifiable"/>, <see cref="IDescribable"/>, <see cref="ISluggable"/>, <see cref="ITrackable"/>.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Usare per uniformare l'accesso ai metadati nelle entità applicative.
    /// </para>
    /// </remarks>
    public interface IEfPersistable : IIdentifiable, IDescribable, ISluggable, ITrackable
    {
    }

    /// <summary>
    /// Domain Interface: Rappresenta un identificativo univoco globale.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Ogni entità deve poter essere identificata globalmente tramite un GUID.
    /// </para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// L'identificativo viene assegnato alla creazione dell'entità.
    /// </para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere unico e immutabile.
    /// </para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Comunemente utilizzato come chiave primaria o identificatore tecnico.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Può essere utilizzato per correlazione, tracciamento o integrazione.
    /// </para>
    /// </remarks>
    public interface IIdentifiable
    {
        /// <summary>
        /// Identificativo univoco globale assegnato all'entità al momento della creazione.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Garantisce l'identità tecnica dell'entità.<br/>
        /// <b>Access:</b> Sola lettura
        /// </remarks>
        Guid UniqueId { get; }
    }

    /// <summary>
    /// Domain Interface: Aggiunge una descrizione testuale opzionale all'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// La descrizione fornisce un contesto utile all'utente o all'interfaccia.
    /// </para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Assegnabile manualmente dopo la creazione.
    /// </para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Facoltativa, non vincolata da lunghezza o formato.
    /// </para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Nessuna relazione diretta; utile in visualizzazioni o documentazione.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Non deve essere usata come campo chiave o identificativo.
    /// </para>
    /// </remarks>
    public interface IDescribable
    {
        /// <summary>
        /// Testo libero usato per fornire una descrizione leggibile dell'entità.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Supporta l’esperienza utente e la documentazione.<br/>
        /// <b>Access:</b> Lettura e scrittura
        /// </remarks>
        string? Description { get; set; }
    }

    /// <summary>
    /// Domain Interface: Fornisce uno slug generato automaticamente dal nome dell'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Lo slug è un alias leggibile da macchina e adatto per URL.
    /// </para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Derivato automaticamente dal nome o da altra proprietà primaria.
    /// </para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere univoco per il contesto applicativo.<br/>
    /// - Formattato in minuscolo, privo di spazi e caratteri speciali.
    /// </para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Può essere usato come chiave alternativa o route URL.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Non deve essere modificato dopo la pubblicazione dell'entità.
    /// </para>
    /// </remarks>
    public interface ISluggable
    {
        /// <summary>
        /// Identificatore leggibile da URL, derivato in modo deterministico dal nome.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Supporta routing, SEO e alias intelligibili.<br/>
        /// <b>Access:</b> Sola lettura
        /// </remarks>
        string Slug { get; }
    }

    /// <summary>
    /// Domain Interface: Traccia metadati di creazione, aggiornamento e cancellazione logica.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Fornisce tracciamento automatico del ciclo di vita dell'entità.
    /// </para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// I valori vengono gestiti automaticamente all'interno del contesto EF.
    /// </para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - I timestamp devono riflettere eventi reali.<br/>
    /// - IsDeleted non elimina realmente l'entità dal database.
    /// </para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Utilizzabile insieme a meccanismi di audit o soft delete.
    /// </para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare il metodo <see cref="Touch"/> per aggiornare <see cref="UpdatedAt"/>.
    /// </para>
    /// </remarks>
    public interface ITrackable
    {
        /// <summary>
        /// Data e ora di creazione dell'entità (in UTC).
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Tracciamento storico.<br/>
        /// <b>Access:</b> Sola lettura
        /// </remarks>
        DateTime CreatedAt { get; }

        /// <summary>
        /// Data e ora dell’ultima modifica (in UTC).
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Verifica di aggiornamenti.<br/>
        /// <b>Access:</b> Sola lettura
        /// </remarks>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// Indica se l’entità è marcata come cancellata logicamente.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Supporta cancellazione soft senza rimuovere l’entità.<br/>
        /// <b>Access:</b> Lettura e scrittura
        /// </remarks>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Aggiorna <see cref="UpdatedAt"/> con la data e ora correnti.
        /// </summary>
        /// <remarks>
        /// <b>Preconditions:</b> Nessuna.<br/>
        /// <b>Postconditions:</b> <c>UpdatedAt</c> sarà aggiornato.<br/>
        /// <b>Side Effects:</b> Nessuno.
        /// </remarks>
        void Touch();
    }
}