using JsonBridgeEF.Shared.DomainMetadata.Interfaces;

namespace JsonBridgeEF.Shared.DomainMetadata.Model
{
    /// <summary>
    /// Domain Value Object: Incapsula tutti i metadati fondamentali di un'entità di dominio.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Unifica l’identità globale (GUID), la descrizione, lo slug e i metadati temporali (CreatedAt, UpdatedAt, IsDeleted)
    /// in un unico contratto definito dall’interfaccia <see cref="IDomainMetadata"/>.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Creato tramite costruttore pubblico, con o senza nome base per lo slug.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - UniqueId è sempre univoco.<br/>
    /// - Slug può essere vuoto.<br/>
    /// - Timestamps rispettano gli eventi di creazione/modifica.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Ideale per gestire audit, identificazione e cancellazione logica nel dominio.</para>
    /// </remarks>
    internal sealed class DomainMetadata : IDomainMetadata
    {
        /// <inheritdoc />
        public Guid UniqueId { get; }

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public string Slug { get; }

        /// <inheritdoc />
        public DateTime CreatedAt { get; }

        private DateTime _updatedAt;
        /// <inheritdoc />
        public DateTime UpdatedAt => _updatedAt;

        private bool _isDeleted;
        /// <inheritdoc />
        public bool IsDeleted => _isDeleted;

        /// <summary>
        /// Costruttore principale: inizializza metadati e genera slug.
        /// </summary>
        /// <param name="name">Nome per generare lo slug. Se vuoto, slug sarà vuoto.</param>
        /// <param name="description">Descrizione opzionale.</param>
        public DomainMetadata(string name, string? description)
        {
            UniqueId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            _updatedAt = CreatedAt;
            _isDeleted = false;
            Description = description;
            Slug = string.IsNullOrWhiteSpace(name) ? string.Empty : GenerateSlug(name);
        }

        /// <summary>
        /// Costruttore alternativo: inizializza senza nome, senza slug.
        /// </summary>
        /// <param name="description">Descrizione opzionale.</param>
        public DomainMetadata(string? description)
            : this(string.Empty, description) // chiama il principale passando name = ""
        {
        }

        /// <inheritdoc />
        public void Touch()
        {
            _updatedAt = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public void MarkDeleted()
        {
            _isDeleted = true;
            Touch();
        }

        /// <inheritdoc />
        public void UnmarkDeleted()
        {
            _isDeleted = false;
            Touch();
        }

        /// <summary>
        /// Calcola uno slug leggibile da URL a partire da una stringa di input.
        /// </summary>
        /// <param name="input">Stringa da trasformare in slug.</param>
        /// <returns>Slug URL-friendly.</returns>
        private static string GenerateSlug(string input)
        {
            return input.Trim().Replace(" ", "-").ToLowerInvariant();
        }
    }
}