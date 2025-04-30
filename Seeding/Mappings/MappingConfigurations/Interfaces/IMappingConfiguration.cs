using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Interfaces
{
    /// <summary>
    /// Domain Interface: Contratto per la configurazione del mapping da JSON a DbContext.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta tutte le impostazioni necessarie per collegare uno schema JSON di origine a un contesto EF target.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// L'interfaccia è implementata da una classe EF Core e gestita tramite migrazione dati e configurazione iniziale.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Richiede che <see cref="JsonSchema"/> e <see cref="DbContextInfo"/> siano validi e coerenti rispetto alla migrazione.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IEfEntity"/> e <see cref="IDomainMetadata"/> e implementa <see cref="IEquatable{IMappingConfiguration}"/> per supportare identificazione tecnica, audit e uguaglianza semantica.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata dal MappingEngine per generare classi EF e orchestrare la migrazione dei dati.</para>
    /// </remarks>
    internal interface IMappingConfiguration : IEfEntity, IDomainMetadata, IEquatable<IMappingConfiguration>
    {
        /// <summary>
        /// Identificativo tecnico dello schema JSON di origine.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Associa la configurazione a uno specifico schema JSON.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int JsonSchemaId { get; }

        /// <summary>
        /// Riferimento di navigazione allo schema JSON di origine.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Permette di accedere alla struttura e ai metadati dello schema.<br/>
        /// <b>Access:</b> Sola lettura tramite EF Core.
        /// </remarks>
        JsonSchema JsonSchema { get; }

        /// <summary>
        /// Identificativo tecnico del DbContext target.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Associa la configurazione a un DbContext di destinazione.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int DbContextInfoId { get; }

        /// <summary>
        /// Riferimento di navigazione al DbContext di destinazione.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Permette di accedere alle definizioni di tabelle e configurazioni globali.<br/>
        /// <b>Access:</b> Sola lettura tramite EF Core.
        /// </remarks>
        DbContextInfo DbContextInfo { get; }

        /// <summary>
        /// Namespace C# in cui generare le classi entity e il contesto EF.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Definisce lo spazio dei nomi delle classi generate.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        string Namespace { get; }

        /// <summary>
        /// Nome della classe del DbContext generato.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Specifica il nome della classe del DbContext.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        string DbContextName { get; }

        /// <summary>
        /// Collezione di regole di mapping tra proprietà JSON e proprietà delle classi entity.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Definisce tutte le associazioni configurate per il progetto di migrazione.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        ICollection<IMappingRule> MappingRules { get; }
    }
}