using System.Reflection;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Common
{
    /// <summary>
    /// Classe base per i servizi di gestione dei database.
    /// Fornisce funzionalità comuni come la gestione dei repository, il cambio database e la pulizia dei dati.
    /// </summary>
    internal abstract class BaseDbService
    {
        #region 🔹 Campi privati e costruttore

        private IUnitOfWork _unitOfWork;
        protected readonly DbContextInspector _dbContextInspector;

        /// <summary>
        /// Inizializza il servizio con un'istanza di UnitOfWork.
        /// </summary>
        /// <param name="unitOfWork">L'istanza di UnitOfWork da utilizzare.</param>
        /// <exception cref="ArgumentNullException">Se <paramref name="unitOfWork"/> è null.</exception>
        protected BaseDbService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _dbContextInspector = new DbContextInspector(_unitOfWork.DbContext);
        }

        #endregion

        #region 🔹 Accesso ai repository

        /// <summary>
        /// Ottiene un repository per un'entità specifica.
        /// </summary>
        /// <typeparam name="TEntity">Il tipo di entità per il repository.</typeparam>
        /// <returns>Un'istanza di <see cref="IRepository{TEntity}"/>.</returns>
        protected IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _unitOfWork.Repository<TEntity>();
        }

        #endregion

        #region 🔹 Salvataggio delle modifiche

        /// <summary>
        /// Garantisce che l'entità sia tracciata nel DbContext (idempotente).
        /// </summary>
        protected void EnsureTracked(object entity)
        {
            var entry = _unitOfWork.DbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _unitOfWork.DbContext.Attach(entity);
            }
        }

        /// <summary>
        /// Salva le modifiche al database e gestisce eventuali errori.
        /// </summary>
        /// <returns>Un'operazione asincrona completata quando il salvataggio è avvenuto.</returns>
        protected async Task SaveChangesAsync()
        {
            try
            {
                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine("✅ Changes saved successfully.");
            }
            catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
            {
                Console.WriteLine("❌ Errore durante SaveChanges:");
                Console.WriteLine($"   Messaggio: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        #endregion

        #region 🔹 Pulizia dei dati per la classe derivata

        /// <summary>
        /// Cancella tutti gli elementi della tabella associata alla classe derivata.
        /// </summary>
        /// <typeparam name="TEntity">Il tipo di entità di cui eliminare i record.</typeparam>
        /// <returns>Un'operazione asincrona completata quando la pulizia è avvenuta.</returns>
        public async Task ClearAll<TEntity>() where TEntity : class
        {
            Console.WriteLine($"⚠️ Inizio eliminazione dei dati per {typeof(TEntity).Name}...");

            var dbContext = _unitOfWork.DbContext;
            var entityType = dbContext.Model.FindEntityType(typeof(TEntity));

            if (entityType == null)
            {
                Console.WriteLine($"❌ Errore: Nessuna tabella trovata per l'entità {typeof(TEntity).Name}");
                return;
            }

            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
            {
                Console.WriteLine($"❌ Errore: Il nome della tabella per {typeof(TEntity).Name} non è valido.");
                return;
            }

            Console.WriteLine($"🗑 Cancellazione della tabella: {tableName}");

            /// Disabilita temporaneamente le verifiche sulle chiavi esterne
            await dbContext.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

            // Esegue la cancellazione dei record in modo sicuro
            var commandText = $"DELETE FROM \"{tableName}\";";
            await dbContext.Database.ExecuteSqlRawAsync(commandText);

            // Riattiva le verifiche sulle chiavi esterne
            await dbContext.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;"); await SaveChangesAsync();
            Console.WriteLine($"✅ Tutti i dati per {typeof(TEntity).Name} sono stati eliminati con successo.");
        }

        #endregion

        #region 🔹 Cambio database

        /// <summary>
        /// Cambia dinamicamente il database associato creando una nuova istanza del DbContext corrispondente.
        /// </summary>
        /// <param name="fullyQualifiedName">Il nome completamente qualificato della classe DbContext.</param>
        /// <returns>Un'operazione asincrona completata quando il cambio è avvenuto.</returns>
        /// <exception cref="ArgumentException">Se <paramref name="fullyQualifiedName"/> è vuoto o nullo.</exception>
        /// <exception cref="InvalidOperationException">Se non è possibile trovare il tipo corrispondente.</exception>
        public async Task ChangeDatabaseAsync(string fullyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedName))
                throw new ArgumentException("Il nome completamente qualificato del DbContext non può essere vuoto.", nameof(fullyQualifiedName));

            Console.WriteLine($"🔄 Cambio database in corso: {fullyQualifiedName}...");

            // Chiude e rilascia il vecchio contesto
            await _unitOfWork.SaveChangesAsync();
            _unitOfWork.Dispose();

            // Determina il tipo di DbContext dal nome completamente qualificato
            Type? dbContextType = ExtractDbContextType(fullyQualifiedName);
            if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
            {
                Console.WriteLine($"❌ Errore: Il tipo '{fullyQualifiedName}' non è un DbContext valido.");
                Console.WriteLine("   Tipi disponibili:");
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(DbContext).IsAssignableFrom(t)))
                {
                    Console.WriteLine($"   - {type.FullName}");
                }
                throw new InvalidOperationException($"❌ Impossibile trovare il tipo '{fullyQualifiedName}' o non è un DbContext valido.");
            }

            // Crea dinamicamente il nuovo DbContext
            var dbContext = (DbContext?)Activator.CreateInstance(dbContextType)
                ?? throw new InvalidOperationException($"❌ Errore: impossibile creare un'istanza di '{fullyQualifiedName}'.");

            // Sostituisce UnitOfWork con il nuovo DbContext
            _unitOfWork = new UnitOfWork(dbContext);
            Console.WriteLine($"✅ Database cambiato con successo: {fullyQualifiedName}");
        }

        /// <summary>
        /// Estrae il tipo di DbContext dal nome completamente qualificato.
        /// </summary>
        /// <param name="fullyQualifiedName">Il nome completamente qualificato della classe DbContext.</param>
        /// <returns>Il tipo di DbContext corrispondente o null se non trovato.</returns>
        private static Type? ExtractDbContextType(string fullyQualifiedName)
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.FullName?.Equals(fullyQualifiedName, StringComparison.OrdinalIgnoreCase) == true);
        }

        #endregion
    }
}