using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;

namespace JsonBridgeEF.Seeding.Source.Model.JsonProperties
{
    /// <inheritdoc cref="IDomainMetadata"/>
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> responsabile dell'implementazione dei metadati di dominio (<see cref="IDomainMetadata"/>).
    /// </summary>
    internal sealed partial class JsonProperty
    {
        /// <summary>
        /// Campo che incapsula i metadati di dominio tramite il value object <see cref="DomainMetadata"/>.
        /// </summary>
        private readonly DomainMetadata _metadata;

        // --- Proprietà di metadati di dominio ---

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

        // --- Metodi di metadati di dominio ---

        /// <inheritdoc />
        public void MarkDeleted() => _metadata.MarkDeleted();

        /// <inheritdoc />
        public void UnmarkDeleted() => _metadata.UnmarkDeleted();

        /// <inheritdoc />
        public void Touch() => _metadata.Touch();
    }
}