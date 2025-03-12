using System.Linq.Expressions;

namespace JsonBridgeEF.Common.Repositories
{
    /// <summary>
    /// Interfaccia generica per il repository delle entità.
    /// Supporta operazioni CRUD con caricamento automatico delle relazioni di navigazione.
    /// </summary>
    /// <typeparam name="TEntity">Il tipo dell'entità gestita dal repository.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Restituisce tutte le entità del tipo specificato, includendo automaticamente le proprietà di navigazione.
        /// </summary>
        Task<List<TEntity>> GetAllAsync();

        /// <summary>
        /// Restituisce tutte le entità filtrate in base a un'espressione lambda, includendo automaticamente le proprietà di navigazione.
        /// </summary>
        /// <param name="predicate">Espressione per filtrare le entità.</param>
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Restituisce un'entità in base all'ID, includendo automaticamente le proprietà di navigazione.
        /// </summary>
        /// <param name="id">L'ID dell'entità da recuperare.</param>
        Task<TEntity?> GetByIdAsync(int id);

        /// <summary>
        /// Restituisce un elenco di entità filtrate in base a un'espressione lambda, includendo automaticamente le proprietà di navigazione.
        /// </summary>
        /// <param name="predicate">Espressione per filtrare le entità.</param>
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Restituisce la prima entità che soddisfa un criterio specifico, includendo automaticamente le proprietà di navigazione.
        /// </summary>
        /// <param name="predicate">Espressione per filtrare le entità.</param>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Verifica se esiste almeno un'entità che soddisfa un criterio.
        /// </summary>
        /// <param name="predicate">Espressione per verificare l'esistenza di entità corrispondenti.</param>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Aggiunge un'entità al repository.
        /// </summary>
        /// <param name="entity">L'entità da aggiungere.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Aggiunge un insieme di entità al repository.
        /// </summary>
        /// <param name="entities">Le entità da aggiungere.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Aggiorna un'entità esistente nel repository.
        /// </summary>
        /// <param name="entity">L'entità da aggiornare.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Rimuove un'entità dal repository.
        /// </summary>
        /// <param name="entity">L'entità da rimuovere.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Rimuove un insieme di entità dal repository.
        /// </summary>
        /// <param name="entities">Le entità da rimuovere.</param>
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}