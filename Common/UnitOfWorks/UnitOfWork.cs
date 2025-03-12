using JsonBridgeEF.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Common.UnitOfWorks
{
    /// <summary>
    /// Implementazione di UnitOfWork per la gestione dei repository e della persistenza nel database.
    /// </summary>
    /// <remarks>
    /// Inizializza una nuova istanza di UnitOfWork con il DbContext specificato.
    /// </remarks>
    internal class UnitOfWork(DbContext dbContext) : IUnitOfWork
    {
        private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private readonly Dictionary<Type, object> _repositories = [];
        private bool _disposed = false;

        /// <inheritdoc />
        public DbContext DbContext => _dbContext;

        /// <inheritdoc />
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out object? repositoryInstance))
            {
                repositoryInstance = new Repository<TEntity>(_dbContext);
                _repositories[type] = repositoryInstance;
            }

            return (IRepository<TEntity>)repositoryInstance;
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Rilascia le risorse non gestite e, opzionalmente, le risorse gestite.
        /// Nota: Se il DbContext viene gestito da DI, non va disposto qui.
        /// </summary>
        /// <param name="disposing">Indica se vengono rilasciate anche le risorse gestite.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Previene la chiamata al finalizer
        }

        /// <summary>
        /// Finalizer per garantire il rilascio delle risorse non gestite.
        /// </summary>
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}