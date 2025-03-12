using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JsonBridgeEF.Common.Repositories
{
    /// <summary>
    /// Implementazione generica del repository per la gestione delle entità nel database.
    /// Supporta operazioni CRUD con caricamento automatico delle proprietà di navigazione.
    /// </summary>
    /// <typeparam name="TEntity">Il tipo dell'entità gestita dal repository.</typeparam>
    internal class Repository<TEntity>(DbContext dbContext) : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
        private readonly DbContext _dbContext = dbContext; // Necessario per ottenere i metadati

        #region Lettura

        /// <inheritdoc />
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await ApplyAutoIncludes(_dbSet).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ApplyAutoIncludes(_dbSet.Where(predicate)).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await ApplyAutoIncludes(_dbSet).FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        /// <inheritdoc />
        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ApplyAutoIncludes(_dbSet.Where(predicate)).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ApplyAutoIncludes(_dbSet.Where(predicate)).FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        #endregion

        #region Scrittura

        /// <inheritdoc />
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <inheritdoc />
        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        /// <inheritdoc />
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc />
        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc />
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region Metodi Privati

        /// <summary>
        /// Applica dinamicamente le espressioni di Include() alle proprietà di navigazione basandosi su Reflection.
        /// </summary>
        /// <param name="query">L'oggetto IQueryable su cui applicare gli include.</param>
        /// <returns>La query con gli include applicati.</returns>
        private IQueryable<TEntity> ApplyAutoIncludes(IQueryable<TEntity> query)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
            if (entityType == null) return query;

            foreach (var navigation in entityType.GetNavigations())
            {
                query = query.Include(navigation.Name);
            }
            return query;
        }

        #endregion
    }
}