using JsonBridgeEF.Shared.EntityModel.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Target.ClassComponents.Interfaces
{
    /// <summary>
    /// Domain Interface: Marker per i componenti del modello a classi.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Indica che l'elemento appartiene al modello orientato agli oggetti (OO).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementata direttamente dalle classi che rappresentano componenti strutturali del modello.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessuna logica; funge esclusivamente da marker per la categorizzazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende:
    /// <list type="bullet">
    ///   <item><see cref="IComponentModel"/> per supportare l’identificazione nominale nel grafo del modello a entità.</item>
    ///   <item><see cref="IDomainMetadata"/> per includere slug, descrizione e audit metadata.</item>
    ///   <item><see cref="IEfEntity"/> per esporre la chiave primaria tecnica utilizzata da EF.</item>
    /// </list></para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata per raggruppare logicamente tutte le entità o componenti del modello OO che condividono caratteristiche comuni di struttura e identificazione.</para>
    /// </remarks>
    public interface IClassComponent : IComponentModel,
                                       IDomainMetadata,
                                       IEfEntity
    {
        // Marker comune per tutti i componenti del modello OO.
    }
}