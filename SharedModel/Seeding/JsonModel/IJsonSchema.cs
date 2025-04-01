namespace JsonBridgeEF.SharedModel.Seeding.JsonModel
{
    /// <summary>
    /// Domain Interface: Aggregate root per la definizione di uno schema JSON.
    /// </summary>
    /// <typeparam name="TObjectSchema">Il tipo concreto degli oggetti definiti nello schema.</typeparam>
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
    /// - Contiene oggetti <see cref="IJsonObjectSchema{TObjectSchema, TProperty}"/> come elementi strutturali.<br/>
    /// - Gli oggetti sono logicamente posseduti dallo schema.<br/>
    /// - Può essere serializzato, validato e analizzato come unità logica completa.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare <see cref="IdentObjectSchemas"/> e <see cref="NonIdentObjectSchemas"/> per distinguere semanticamente gli oggetti.<br/>
    /// - Il contenuto JSON originale è accessibile tramite <see cref="JsonSchemaContent"/>.<br/>
    /// - <see cref="ObjectSchemas"/> espone l'intera collezione, in sola lettura.</para>
    /// </remarks>
    public interface IJsonSchema<TObjectSchema, TProperty>
        where TObjectSchema : class, IJsonObjectSchema<TObjectSchema, TProperty>
        where TProperty : class, IJsonProperty<TProperty, TObjectSchema>
    {
        /// <summary>
        /// Nome descrittivo dello schema.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Identifica lo schema in modo univoco all'interno del dominio applicativo.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        string Name { get; }

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
        /// <b>Semantics:</b> Include tutte le istanze di <typeparamref name="TObjectSchema"/> presenti nello schema.<br/>
        /// <b>Access:</b> Collezione in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TObjectSchema> ObjectSchemas { get; }

        /// <summary>
        /// Oggetti JSON identificabili, cioè dotati di una proprietà chiave.
        /// </summary>
        /// <remarks>
        /// <b>Semantics:</b> Filtra <see cref="ObjectSchemas"/> mantenendo solo quelli con chiave logica definita.<br/>
        /// <b>Access:</b> Vista calcolata in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TObjectSchema> IdentObjectSchemas { get; }

        /// <summary>
        /// Oggetti JSON non identificabili, privi di chiave logica.
        /// </summary>
        /// <remarks>
        /// <b>Semantics:</b> Filtra <see cref="ObjectSchemas"/> mantenendo solo quelli senza key definita.<br/>
        /// <b>Access:</b> Vista calcolata in sola lettura.
        /// </remarks>
        IReadOnlyCollection<TObjectSchema> NonIdentObjectSchemas { get; }
    }
}