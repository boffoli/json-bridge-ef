using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Exceptions;
using JsonBridgeEF.Common.EfEntities.Interfaces.Collections;

namespace JsonBridgeEF.Common.EfEntities.Collections
{
    /// <summary>
    /// Concrete Implementation: Collezione generica per entità Entity Framework con supporto alla ricerca per nome,
    /// con vincolo di unicità case-insensitive e comparatore personalizzabile.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Tipo delle entità contenute, che devono derivare da <see cref="BaseEfEntity{TEntity}"/> e implementare <see cref="INamed"/>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Collezione riutilizzabile per entità nominali basate su EF Core, con controllo dell’unicità per nome
    /// e accesso efficiente agli elementi.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Può essere usata direttamente o come base per collezioni più specifiche (Owned, Keyed, ecc.).<br/>
    /// - Il comparatore predefinito è <see cref="NamedEntityComparer{TEntity}"/> se non specificato.</para>
    /// </remarks>
    internal class EfEntityCollection<TEntity> 
        : IEfEntityCollection<TEntity>
        where TEntity : BaseEfEntity<TEntity>
    {
        /// <summary>
        /// Contenitore interno con controllo di unicità.
        /// </summary>
        private protected readonly HashSet<TEntity> _entities;

        /// <summary>
        /// Inizializza la collezione con un comparatore opzionale.
        /// </summary>
        /// <param name="comparer">
        /// Comparatore per la gestione dell’unicità. Se null, viene usato <see cref="NamedEntityComparer{TEntity}.Instance"/>.
        /// </param>
        protected EfEntityCollection(IEqualityComparer<TEntity>? comparer = null)
        {
            _entities = new HashSet<TEntity>(comparer ?? NamedEntityComparer<TEntity>.Instance);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<TEntity> Entities => _entities;

        /// <inheritdoc />
        public TEntity? FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return _entities.FirstOrDefault(
                entity => entity.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            );
        }

        /// <inheritdoc />
        public virtual void Add(TEntity entity)
        {
            if (!_entities.Add(entity))
                throw new NamedEntityAlreadyExistsException(entity.Name, GetType().Name);
        }

        /// <summary>
        /// Restituisce un riepilogo dei nomi delle prime entità presenti nella collezione.
        /// </summary>
        public override string ToString()
        {
            const int previewLimit = 3;
            var names = _entities.Select(x => x.Name).Take(previewLimit).ToList();

            if (_entities.Count > previewLimit)
                names.Add("...");

            return string.Join(", ", names);
        }
    }
}