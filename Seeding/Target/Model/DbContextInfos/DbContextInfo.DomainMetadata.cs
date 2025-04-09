using JsonBridgeEF.Shared.Domain.Interfaces;

namespace JsonBridgeEF.Seeding.Target.Model.DbContextInfos
{
    /// <inheritdoc cref="IDomainMetadata"/>
    /// <summary>
    /// Partial class di <see cref="DbContextInfo"/> responsabile dell'implementazione di <see cref="IDomainMetadata"/>.
    /// </summary>
    internal partial class DbContextInfo
    {
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
        public void Touch() => _metadata.Touch();

        /// <inheritdoc />
        public void MarkDeleted() => _metadata.MarkDeleted();

        /// <inheritdoc />
        public void UnmarkDeleted() => _metadata.UnmarkDeleted();
    }
}