using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Model;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Model
{
    /// <summary>
    /// Domain Class: Partial di <see cref="MappingRule"/> per la gestione dei metadati di dominio tramite <see cref="DomainMetadata"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Incapsula UniqueId, Description, Slug, CreatedAt, UpdatedAt e IsDeleted tramite un value object di dominio standardizzato.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Il value object <see cref="DomainMetadata"/> viene inizializzato nel costruttore di MappingRule oppure ricostruito da EF Core.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// I dati di audit (CreatedAt, UpdatedAt) e lo stato (IsDeleted) sono gestiti internamente e non devono essere modificati direttamente.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// MappingRule implementa <see cref="IDomainMetadata"/> delegando il comportamento a <see cref="DomainMetadata"/> come owned type.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// <list type="bullet">
    /// <item>Configurato come owned type nel metodo OnModelCreating di EF Core.</item>
    /// <item>Utilizzato per fornire tracciabilità e audit automatici all'entità MappingRule.</item>
    /// </list>
    /// </para>
    /// </remarks>
    internal sealed partial class MappingRule
    {
        private readonly DomainMetadata _metadata;

        /// <inheritdoc />
        public Guid UniqueId => _metadata.UniqueId;

        /// <inheritdoc />
        public string? Description
        {
            get => _metadata.Description;
            set => _metadata.Description = value;
        }

        /// <inheritdoc />
        public string Slug => _metadata.Slug;

        /// <inheritdoc />
        public DateTime CreatedAt => _metadata.CreatedAt;

        /// <inheritdoc />
        public DateTime UpdatedAt => _metadata.UpdatedAt;

        /// <inheritdoc />
        public bool IsDeleted => _metadata.IsDeleted;

        /// <inheritdoc />
        public void Touch() => _metadata.Touch();

        /// <inheritdoc />
        public void MarkDeleted() => _metadata.MarkDeleted();

        /// <inheritdoc />
        public void UnmarkDeleted() => _metadata.UnmarkDeleted();
    }
}