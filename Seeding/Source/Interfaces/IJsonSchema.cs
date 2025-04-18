namespace JsonBridgeEF.Seeding.Source.Interfaces
{
    /// <summary>
    /// Domain Interface: Aggregate root per la definizione di uno schema JSON.
    /// </summary>
    /// <typeparam name="TJsonEntity">Il tipo concreto degli oggetti definiti nello schema.</typeparam>
    /// <typeparam name="TProperty">Il tipo delle proprietà contenute negli oggetti.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta una struttura JSON composta da oggetti tipizzati (schema di oggetto),
    /// ciascuno con un proprio insieme di proprietà. Lo schema modella la struttura astratta
    /// di documenti JSON validi secondo una semantica condivisa.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere istanziato tramite factory o costruttore controllato, fornendo un nome
    /// univoco e un contenuto JSON coerente. Gli oggetti contenuti devono essere validi e
    /// strutturalmente completi.</para>
    /// 
    /// <para><b>Constraints:</b><br/>
    /// - Il nome deve essere non nullo, non vuoto e univoco all'interno del dominio applicativo.<br/>
    /// - Il contenuto JSON grezzo deve essere fornito e memorizzato integralmente.<br/>
    /// - Ogni oggetto contenuto deve avere un nome univoco.<br/>
    /// - Gli oggetti possono essere identificabili o non identificabili a seconda della presenza di una key property.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// - Contiene oggetti <see cref="IJsonEntity{TJsonEntity, TProperty}"/> come elementi strutturali.<br/>
    /// - Gli oggetti sono logicamente posseduti dallo schema.<br/>
    /// - Ereditando da <see cref="IJsonComponent"/>, lo schema integra le funzionalità di persistenza e metadati (come <c>UniqueId</c>, <c>Description</c>, <c>Slug</c> e <c>Trackable</c>) definite in <c>IEfPersistable</c>.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare <see cref="IdentJsonEntities"/> e <see cref="NonIdentJsonEntities"/> per distinguere semanticamente gli oggetti.<br/>
    /// - Il contenuto JSON originale è accessibile tramite <see cref="JsonSchemaContent"/>.<br/>
    /// - <see cref="JsonEntities"/> espone l'intera collezione, in sola lettura.</para>
    /// </remarks>
    public interface IJsonSchema<TJsonEntity, TProperty> : IJsonComponent
        where TJsonEntity : class, IJsonEntity<TJsonEntity, TProperty>
        where TProperty : class, IJsonProperty<TProperty, TJsonEntity>
    {
        /// <summary>
        /// Contenuto JSON grezzo associato allo schema.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Conserva il testo originale o derivato della struttura JSON di riferimento.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        string JsonSchemaContent { get; }

        /// <summary>
        /// Tutti gli oggetti JSON definiti all'interno dello schema.
        /// </summary>
        /// <remarks>
        /// <b>Semantics:</b> Include tutte le istanze di <typeparamref name="TJsonEntity"/> presenti nello schema.<br/>
        /// <b>Access:</b> Collezione in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TJsonEntity> JsonEntities { get; }

        /// <summary>
        /// Oggetti JSON identificabili, cioè dotati di una proprietà chiave logica.
        /// </summary>
        /// <remarks>
        /// <b>Semantics:</b> Filtra <see cref="JsonEntities"/> mantenendo solo quelli con chiave logica definita.<br/>
        /// <b>Access:</b> Vista calcolata in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TJsonEntity> IdentJsonEntities { get; }

        /// <summary>
        /// Oggetti JSON non identificabili, privi di chiave logica.
        /// </summary>
        /// <remarks>
        /// <b>Semantics:</b> Filtra <see cref="JsonEntities"/> mantenendo solo quelli senza key definita.<br/>
        /// <b>Access:</b> Vista calcolata in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TJsonEntity> NonIdentJsonEntities { get; }

        /// <summary>
        /// Aggiunge un oggetto JSON allo schema.
        /// </summary>
        /// <param name="jsonEntity">L'oggetto JSON da aggiungere allo schema.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> <paramref name="jsonEntity"/> non deve essere <c>null</c> e deve avere un nome univoco all'interno dello schema.</para>
        /// <para><b>Side Effects:</b> L'oggetto viene aggiunto alla collezione <see cref="JsonEntities"/>.</para>
        /// </remarks>
        void AddJsonEntity(TJsonEntity jsonEntity);
    }
}