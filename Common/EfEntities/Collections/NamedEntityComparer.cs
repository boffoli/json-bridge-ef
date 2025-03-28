using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Collections
{
    /// <summary>
    /// Comparatore case-insensitive per entità che espongono la proprietà <see cref="INamedEntity.Name"/>.
    /// </summary>
    /// <typeparam name="TEntity">Tipo dell'entità con nome, che implementa <see cref="INamedEntity"/>.</typeparam>
    internal sealed class NamedEntityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : INamed
    {
        /// <summary>
        /// Istanza singleton del comparatore.
        /// </summary>
        public static readonly NamedEntityComparer<TEntity> Instance = new();

        /// <inheritdoc/>
        public bool Equals(TEntity? x, TEntity? y)
        {
            if (x is null || y is null)
                return false;

            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public int GetHashCode(TEntity obj)
        {
            return obj.Name.ToLowerInvariant().GetHashCode();
        }
    }
}