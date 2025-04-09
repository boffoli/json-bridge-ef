using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;

namespace JsonBridgeEF.Seeding.Target.Model.Properties
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
        private readonly IDomainMetadata _metadata;
        
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