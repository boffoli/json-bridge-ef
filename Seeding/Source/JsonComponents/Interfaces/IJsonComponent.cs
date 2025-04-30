using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonComponents.Interfaces
{
    /// <summary>
    /// Domain Interface: Componente JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Interfaccia marker per tutti gli elementi del modello JSON che rappresentano blocchi,
    /// campi o strutture, integrando i metadati di dominio e la chiave tecnica EF.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementata direttamente dalle classi concrete che modellano componenti del grafo JSON.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Nessun requisito addizionale; serve esclusivamente per categorizzare
    /// gli elementi del modello JSON.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Estende <see cref="IComponentModel"/> per fornire identificazione nominale.<br/>
    /// - Estende <see cref="IDomainMetadata"/> per includere GUID, descrizione, slug e audit.<br/>
    /// - Estende <see cref="IEfEntity"/> per esporre la chiave primaria tecnica utilizzata da EF.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata nelle interfacce generiche e nei servizi per trattare
    /// in modo omogeneo tutti gli elementi del modello JSON.</para>
    /// </remarks>
    public interface IJsonComponent : IComponentModel,
                                       IDomainMetadata,
                                       IEfEntity
    {
        // Marker comune per tutti i componenti JSON.
    }
}