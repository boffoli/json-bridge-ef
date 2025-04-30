using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Mappings.Interfaces;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model
{
    /// <summary>
    /// Domain Class: Configurazione del mapping dal JSON al DbContext di destinazione.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta la configurazione completa per collegare uno schema JSON a un contesto EF e generare i class models.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Viene creata tramite dependency injection o tramite costruttore pubblico che accetta nome e descrizione.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Richiede riferimenti validi a <see cref="JsonSchema"/> e <see cref="DbContextInfo"/>.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Implementa <see cref="IMappingConfiguration"/>, <see cref="IEfEntity"/> e <see cref="IDomainMetadata"/> via partial.
    /// Contiene una collezione di <see cref="IMappingRule"/>.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata dal MappingEngine per orchestrare la generazione delle classi EF e la migrazione dati.</para>
    /// </remarks>
    internal sealed partial class MappingConfiguration : IMappingConfiguration
    {
        #region JSON Schema Link

        /// <summary>
        /// Riferimento di navigazione allo schema JSON di origine.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Permette di accedere alla struttura e ai metadati dello schema.<br/>
        /// <b>Access:</b> Sola lettura tramite EF Core.
        /// </remarks>
        [ForeignKey(nameof(JsonSchemaId))]
        public JsonSchema JsonSchema { get; private set; } = null!;

        #endregion

        #region DbContext Link

        /// <summary>
        /// Riferimento di navigazione al DbContext di destinazione.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Permette di accedere alla configurazione delle entità.<br/>
        /// <b>Access:</b> Sola lettura tramite EF Core.
        /// </remarks>
        [ForeignKey(nameof(DbContextInfo))]
        public DbContextInfo DbContextInfo { get; private set; } = null!;

        #endregion

        #region Naming Overrides

        /// <summary>
        /// Namespace C# da usare per le classi generate.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Definisce lo spazio dei nomi del progetto generato.<br/>
        /// <b>Access:</b> Configurabile in fase di setup.
        /// </remarks>
        [Required, MaxLength(200)]
        public string Namespace { get; private set; } = string.Empty;

        /// <summary>
        /// Nome della classe DbContext da generare.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Specifica il nome del DbContext EF.<br/>
        /// <b>Access:</b> Configurabile in fase di setup.
        /// </remarks>
        [Required, MaxLength(100)]
        public string DbContextName { get; private set; } = string.Empty;

        #endregion

        #region Mapping Rules Collection

        /// <summary>
        /// Collezione di regole di mapping tra proprietà JSON e proprietà delle classi entity.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Definisce tutte le associazioni tra JSON e classi target.<br/>
        /// <b>Access:</b> Lettura e gestione interna.
        /// </remarks>
        public ICollection<IMappingRule> MappingRules { get; internal set; } = [];

        #endregion
    }
}