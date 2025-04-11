using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;

namespace JsonBridgeEF.Seeding.Target.Model.ClassInfos
{
    /// <inheritdoc cref="IDomainMetadata"/>
    /// <summary>
    /// Partial class di <see cref="ClassInfo"/> responsabile dell'implementazione di <see cref="IDomainMetadata"/>.
    /// </summary>
    internal sealed partial class ClassInfo
    {
        /// <summary>
        /// Incapsula i metadati di dominio per l'entità, inclusi identificatore globale, descrizione, auditing e slug.
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

        // --- Metodi per metadati di dominio ---

        /// <inheritdoc />
        public void Touch() => _metadata.Touch();

        /// <inheritdoc />
        public void MarkDeleted() => _metadata.MarkDeleted();

        /// <inheritdoc />
        public void UnmarkDeleted() => _metadata.UnmarkDeleted();
    }
}