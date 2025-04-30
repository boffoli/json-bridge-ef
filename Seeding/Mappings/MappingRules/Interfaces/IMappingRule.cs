using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Interfaces
{
    /// <summary>
    /// Domain Interface: Contratto per una regola di mapping tra proprietà JSON e proprietà target.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta l'associazione univoca tra una proprietà di un'entità JSON e una proprietà di una classe target.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Le istanze vengono create e gestite da Entity Framework Core come entità dipendenti di <c>MappingConfiguration</c>.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Ogni regola deve riferirsi a una configurazione esistente e a entità e proprietà valide lato sorgente e target.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IEfEntity"/>, <see cref="IDomainMetadata"/> per supportare identificazione tecnica, audit e descrizione,
    /// e implementa <see cref="IEquatable{IMappingRule}"/> per garantire confronto semantico.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata dal MappingEngine durante il popolamento dei modelli target a partire dal JSON sorgente.</para>
    /// </remarks>
    internal interface IMappingRule : IEfEntity, IDomainMetadata, IEquatable<IMappingRule>
    {
        /// <summary>
        /// Identificativo tecnico della configurazione di mapping di appartenenza.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Ricollega la regola al suo MappingConfiguration.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int MappingConfigurationId { get; }

        /// <summary>
        /// Identificativo tecnico dell'entità JSON di origine.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Determina quale entità JSON fornisce il valore da trasformare.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int JsonEntityId { get; }

        /// <summary>
        /// Identificativo tecnico della proprietà JSON di origine.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Specifica la proprietà di partenza per il mapping.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int JsonPropertyId { get; }

        /// <summary>
        /// Identificativo tecnico della classe target.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Indica la classe C# generata che riceverà il dato.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int ClassInfoId { get; }

        /// <summary>
        /// Identificativo tecnico della proprietà della classe target.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Specifica il campo finale di destinazione nella classe target.<br/>
        /// <b>Access:</b> Sola lettura.
        /// </remarks>
        int ClassPropertyId { get; }

        /// <summary>
        /// Formula JavaScript di trasformazione o validazione applicata al valore sorgente.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Permette di trasformare dinamicamente il dato prima di assegnarlo.<br/>
        /// <b>Access:</b> Sola lettura; deve contenere una funzione valida.
        /// </remarks>
        string JsFormula { get; }
    }
}