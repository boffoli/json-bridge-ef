using JsonBridgeEF.Shared.DomainMetadata.Model;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model
{
    /// <summary>
    /// Domain Class: Partial per gestire metadata domain (IDomainMetadata) tramite DomainMetadata VO.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Agregga audit, slug, descrizione e cancellazione logica come owned type.</para>
    /// <para>Creation Strategy: Iniettato nel costruttore principale, ricostruito da EF in OnModelCreating.</para>
    /// <para>Constraints: Non mappato direttamente, ma come owned type di EF Core.</para>
    /// <para>Usage Notes: Fornisce implementazione di IDomainMetadata per la classe MappingConfiguration.</para>
    /// </remarks>
    internal partial class MappingConfiguration
    {
        private readonly DomainMetadata _metadata;

        /// <summary>
        /// Costruttore EF Core per la partial metadata.
        /// </summary>
        private MappingConfiguration()
        {
            _metadata = new DomainMetadata(string.Empty, null);
        }

        /// <summary>
        /// Costruttore pubblico per inizializzare nome e descrizione del progetto di mapping.
        /// </summary>
        public MappingConfiguration(string name, string? description)
        {
            _metadata = new DomainMetadata(name, description);
        }

        public Guid UniqueId => _metadata.UniqueId;

        public string? Description
        {
            get => _metadata.Description;
            set => _metadata.Description = value;
        }

        public string Slug => _metadata.Slug;

        public DateTime CreatedAt => _metadata.CreatedAt;

        public DateTime UpdatedAt => _metadata.UpdatedAt;

        public bool IsDeleted => _metadata.IsDeleted;

        public void Touch() => _metadata.Touch();

        public void MarkDeleted() => _metadata.MarkDeleted();

        public void UnmarkDeleted() => _metadata.UnmarkDeleted();
    }
}
