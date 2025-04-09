namespace JsonBridgeEF.Shared.EfPersistance.Interfaces
{
    /// <summary>
    /// EF Interface: Definisce la chiave primaria tecnica usata da Entity Framework.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// La chiave tecnica è utilizzata esclusivamente a livello di persistenza per identificare in modo univoco
    /// un'entità all'interno della tabella del database.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Questa proprietà non fa parte del modello di dominio, ma è necessaria per il mapping con EF.<br/>
    /// - Poiché verrà utilizzata esclusivamente da EF, il setter non è esposto nel contratto pubblico; EF Core
    ///   può impostarla tramite meccanismi interni o reflection.
    /// </para>
    /// </remarks>
    internal interface IEfEntity
    {
        /// <summary>
        /// Chiave primaria tecnica, univoca all'interno della tabella del database.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Utilizzata da EF per il mapping e per le operazioni di join.<br/>
        /// <b>Access:</b> Sola lettura nel dominio; EF Core potrà impostarla internamente.
        /// </remarks>
        int Id { get; }
    }
}