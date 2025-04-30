using JsonBridgeEF.Shared.DomainMetadata.Interfaces;
using JsonBridgeEF.Shared.DomainMetadata.Model;

namespace JsonBridgeEF.Seeding.Target.ClassProperties.Model
{
    /// <inheritdoc cref="IDomainMetadata"/>
    /// <summary>
    /// Partial class di <see cref="ClassProperty"/> che implementa i metadati di dominio tramite <see cref="DomainMetadata"/>.
    /// </summary>
    internal sealed partial class ClassProperty
    {
        /// <summary>
        /// Campo che incapsula i metadati di dominio tramite il value object <see cref="DomainMetadata"/>.
        /// </summary>
        private readonly DomainMetadata _metadata = new(name, description);
        
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
        public void MarkDeleted() => _metadata.MarkDeleted();

        /// <inheritdoc />
        public void UnmarkDeleted() => _metadata.UnmarkDeleted();

        /// <inheritdoc />
        public void Touch() => _metadata.Touch();
    }
}