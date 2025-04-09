using JsonBridgeEF.Shared.Domain.Interfaces;

namespace JsonBridgeEF.Shared.Domain.Model
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
    /// Viene creato al momento della costruzione; lo slug viene calcolato internamente a partire da una stringa di input,
    /// come ad esempio il nome dell'entità.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - UniqueId è univoco e immutabile.<br/>
    /// - CreatedAt e UpdatedAt riflettono rispettivamente il momento di creazione e l'ultima modifica.<br/>
    /// - IsDeleted indica lo stato di cancellazione soft dell'entità.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Questo value object viene utilizzato internamente per gestire i metadati di audit e identificazione.</para>
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
        /// Costruttore: Inizializza i metadati con il nome da cui derivare lo slug e la descrizione opzionale.
        /// </summary>
        /// <param name="name">Il nome dell'entità, usato per generare lo slug.</param>
        /// <param name="description">Una descrizione opzionale dell'entità.</param>
        /// <exception cref="ArgumentException">Se <paramref name="name"/> è nullo o vuoto.</exception>
        public DomainMetadata(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome non può essere nullo o vuoto.", nameof(name));

            UniqueId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            _updatedAt = CreatedAt;
            _isDeleted = false;
            Description = description;
            Slug = GenerateSlug(name); // calcolo interno
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
        /// <param name="input">La stringa da trasformare in slug.</param>
        /// <returns>Una stringa formattata in modo da essere usata come slug.</returns>
        private static string GenerateSlug(string input)
        {
            return input.Trim().Replace(" ", "-").ToLowerInvariant();
        }
    }
}