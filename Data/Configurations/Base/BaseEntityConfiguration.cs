using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JsonBridgeEF.Data.Configurations.Base
{
    /// <summary>
    /// Domain Concept: Classe base astratta che centralizza la configurazione dell'entità, includendo la creazione delle shadow foreign key.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Utilizza il Template Method pattern, in cui il metodo <c>Configure</c> invoca prima <c>CreateShadowForeignKeys</c> e poi la configurazione specifica dell'entità e delle relazioni.</para>
    /// <para>Usage Notes: Le classi derivate devono implementare <c>ConfigureEntity</c> e <c>ConfigureRelationships</c> per definire la configurazione specifica, mentre <c>CreateShadowForeignKeys</c> può essere sovrascritto per creare le shadow FK necessarie.</para>
    /// </remarks>
    /// <typeparam name="T">Il tipo di entità da configurare.</typeparam>
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
        /// <summary>
        /// Template method che configura l'entità: prima crea le shadow FK, poi configura l'entità e infine le relazioni.
        /// </summary>
        /// <param name="builder">Istanza dell'EntityTypeBuilder da configurare.</param>
        public void Configure(EntityTypeBuilder<T> builder)
        {
            // Invoca per primo la creazione delle shadow foreign key
            CreateShadowForeignKeys(builder);
            
            // Configurazione specifica dell'entità (chiavi primarie, proprietà, ecc.)
            ConfigureEntity(builder);
            
            // Configura le relazioni (le foreign key possono essere riutilizzate qui)
            ConfigureRelationships(builder);
        }
        
        /// <summary>
        /// Crea le proprietà shadow per le foreign key.
        /// </summary>
        /// <param name="builder">Istanza dell'EntityTypeBuilder da configurare.</param>
        /// <remarks>
        /// <para>Il metodo ha un'implementazione di default vuota; le classi derivate possono sovrascriverlo se devono creare shadow FK specifiche.</para>
        /// </remarks>
        protected virtual void CreateShadowForeignKeys(EntityTypeBuilder<T> builder)
        {
            // Implementazione di default: non fare nulla.
        }
        
        /// <summary>
        /// Configura il mapping specifico dell'entità (proprietà, chiavi primarie, tabelle, ecc.).
        /// </summary>
        /// <param name="builder">Istanza dell'EntityTypeBuilder da configurare.</param>
        protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
        
        /// <summary>
        /// Configura le relazioni dell'entità, compresa l'associazione con le foreign key (che potrebbero essere shadow).
        /// </summary>
        /// <param name="builder">Istanza dell'EntityTypeBuilder da configurare.</param>
        protected abstract void ConfigureRelationships(EntityTypeBuilder<T> builder);
    }
}