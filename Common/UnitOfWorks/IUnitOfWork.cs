using JsonBridgeEF.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Common.UnitOfWorks
{
    /// <summary>
    /// Interfaccia di base per l'unità di lavoro (Unit of Work).
    /// Coordina la persistenza dei repository e garantisce l'atomicità delle operazioni.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Restituisce il contesto del database gestito da questa UnitOfWork.
        /// </summary>
        DbContext DbContext { get; }

        /// <summary>
        /// Restituisce un repository per il tipo specificato.
        /// </summary>
        /// <typeparam name="TEntity">Il tipo dell'entità per il repository.</typeparam>
        /// <returns>Un'istanza di <see cref="IRepository{TEntity}"/> per il tipo specificato.</returns>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        /// <summary>
        /// Salva tutte le modifiche apportate al database in un'unica transazione.
        /// </summary>
        /// <returns>Il numero di entità salvate nel database.</returns>
        Task<int> SaveChangesAsync();
    }
}