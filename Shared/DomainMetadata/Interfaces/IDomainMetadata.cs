namespace JsonBridgeEF.Shared.DomainMetadata.Interfaces
{
    using System;

    /// <summary>
    /// Domain Interface: Aggrega tutti i metadati fondamentali di un'entità di dominio.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un contratto completo per entità di dominio che espongono identità, audit, descrizione e slug.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato solitamente da value object interni alle entità persistenti.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere immutabile tranne per i campi di audit.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Composto da <see cref="IDomainIdentifiable"/>, <see cref="IDescribable"/>, <see cref="ISluggable"/>, <see cref="IAuditable"/>, <see cref="ISoftDeletable"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare come base per metadati interni, audit e sincronizzazione.</para>
    /// </remarks>
    public interface IDomainMetadata : IDomainIdentifiable, IDescribable, ISluggable, IAuditable, ISoftDeletable
    {
    }

    /// <summary>
    /// Domain Interface: Rappresenta un identificativo univoco globale.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Ogni entità deve poter essere identificata globalmente tramite un GUID.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// L'identificativo viene assegnato automaticamente alla creazione.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere unico e immutabile.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Parte dei metadati di dominio (<see cref="IDomainMetadata"/>).</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Usato per tracking, audit o integrazione con sistemi esterni.</para>
    /// </remarks>
    public interface IDomainIdentifiable
    {
        Guid UniqueId { get; }
    }

    /// <summary>
    /// Domain Interface: Aggiunge una descrizione testuale opzionale all'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Consente di associare una descrizione leggibile all'entità, utile per documentazione e interfacce utente.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Assegnata manualmente dopo la costruzione.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Facoltativa; nessun vincolo di formato.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Utilizzata da <see cref="IDomainMetadata"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Non usarla come identificativo o chiave di business.</para>
    /// </remarks>
    public interface IDescribable
    {
        string? Description { get; set; }
    }

    /// <summary>
    /// Domain Interface: Fornisce uno slug generato automaticamente dal nome dell'entità.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Lo slug è un identificativo URL-friendly derivato da una proprietà semantica dell'entità.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Calcolato automaticamente al momento della creazione da un nome o alias primario.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Deve essere univoco nel contesto applicativo e non modificabile dopo la generazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Parte di <see cref="IDomainMetadata"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Ottimale per URL, SEO e routing RESTful.</para>
    /// </remarks>
    public interface ISluggable
    {
        string Slug { get; }
    }

    /// <summary>
    /// Domain Interface: Rappresenta la capacità di un'entità di essere auditabile temporalmente.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Traccia la data di creazione e l'ultimo aggiornamento, utile per audit trail e sincronizzazione.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Timestamps generati al momento della costruzione e aggiornati tramite metodo esplicito.</para>
    /// <para><b>Constraints:</b><br/>
    /// - I valori devono riflettere eventi reali e non essere modificati manualmente.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Usare <see cref="Touch"/> per aggiornare il campo <see cref="UpdatedAt"/>.</para>
    /// </remarks>
    public interface IAuditable
    {
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
        void Touch();
    }

    /// <summary>
    /// Domain Interface: Estende la capacità di un'entità di supportare la cancellazione logica.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// La cancellazione logica consente di rendere inattiva un'entità senza rimuoverla dal sistema.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Il flag viene impostato esplicitamente tramite metodi.</para>
    /// <para><b>Constraints:</b><br/>
    /// - La gestione della cancellazione è separata dalla logica di auditing.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Chiamare <see cref="MarkDeleted"/> o <see cref="UnmarkDeleted"/> per modificare lo stato.</para>
    /// </remarks>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; }
        void MarkDeleted();
        void UnmarkDeleted();
    }
}
