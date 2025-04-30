using JsonBridgeEF.Seeding.Source.JsonComponents.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces
{
    /// <summary>
    /// Domain Interface: Marker comune per tutti gli schemi JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta genericamente un insieme strutturato di entità JSON, senza dipendere dalla tipizzazione delle proprietà.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato da schemi JSON concreti, che possono essere specializzati tramite parametri generici.</para>
    /// 
    /// <para><b>Constraints:</b><br/>
    /// Nessun requisito aggiuntivo rispetto al ruolo di marker.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IJsonComponent"/> per integrare ciclo di vita e metadati comuni.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// Utile per raccolte polimorfiche di schemi JSON a runtime.</para>
    /// </remarks>
    public interface IJsonSchema : IJsonComponent
    {
        /// <summary>
        /// Conserva il testo originale o derivato dello schema JSON.
        /// </summary>
        string JsonSchemaContent { get; }
    }

    /// <summary>
    /// Domain Interface: Aggregate root per la definizione di uno schema JSON tipizzato.
    /// </summary>
    /// <typeparam name="TJsonEntity">
    /// Il tipo concreto degli oggetti definiti nello schema.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Modella uno schema JSON come insieme di entità JSON omogenee.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// - Contiene <see cref="IJsonEntity"/> come elementi.
    /// - Estende <see cref="IJsonSchema"/> per ereditare metadati comuni.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// - Utilizzare <see cref="IdentJsonEntities"/> e <see cref="NonIdentJsonEntities"/> per filtri semantici.
    /// - Chiamare <see cref="AddJsonEntity(IJsonEntity)"/> per registrare nuove entità.</para>
    /// </remarks>
    public interface IJsonSchema<TJsonEntity> : IJsonSchema
        where TJsonEntity : class, IJsonEntity
    {
        /// <summary>
        /// Collezione completa delle entità JSON definite.
        /// </summary>
        IReadOnlyCollection<TJsonEntity> JsonEntities { get; }

        /// <summary>
        /// Vista delle entità JSON identificabili (con chiave logica).
        /// </summary>
        IReadOnlyCollection<TJsonEntity> IdentJsonEntities { get; }

        /// <summary>
        /// Vista delle entità JSON non identificabili (senza chiave logica).
        /// </summary>
        IReadOnlyCollection<TJsonEntity> NonIdentJsonEntities { get; }

        /// <summary>
        /// Registra una nuova entità JSON all'interno dello schema.
        /// </summary>
        /// <param name="jsonEntity">Istanza di <typeparamref name="TJsonEntity"/> da aggiungere.</param>
        void AddJsonEntity(TJsonEntity jsonEntity);
    }
}
