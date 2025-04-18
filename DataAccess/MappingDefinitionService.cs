using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.DataAccess
{
    /// <summary>
    /// Servizio per la gestione delle definizioni di mapping, tra cui tipi JSON, campi JSON, entità target e contesti di database target.
    /// </summary>
    internal class MappingDefService(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        #region 🔹 Operazioni sui Tipi JSON

        /// <summary>
        /// Aggiunge una nuova definizione di tipo JSON al database.
        /// </summary>
        /// <param name="jsonSchema">Definizione del tipo JSON da aggiungere.</param>
        public async Task AddJsonSchemaAsync(JsonSchema jsonSchema)
        {
            if (string.IsNullOrWhiteSpace(jsonSchema.Name))
                throw new InvalidOperationException("Il nome del tipo JSON non può essere vuoto.");

            GetRepository<JsonSchema>().Add(jsonSchema);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di tipi JSON dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di tipi JSON.</returns>
        public async Task<List<JsonSchema>> GetJsonSchemaAsync() =>
            await GetRepository<JsonSchema>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di tipo JSON specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del tipo JSON se trovata, altrimenti null.</returns>
        public async Task<JsonSchema?> GetJsonSchemaByIdAsync(int id) =>
            await GetRepository<JsonSchema>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sui Campi JSON

        /// <summary>
        /// Aggiunge una nuova definizione di campo JSON al database.
        /// </summary>
        /// <param name="jsonField">Definizione del campo JSON da aggiungere.</param>
        public async Task AddJsonFieldAsync(JsonField jsonField)
        {
            GetRepository<JsonField>().Add(jsonField);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di campi JSON dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di campi JSON.</returns>
        public async Task<List<JsonField>> GetJsonFieldsAsync() =>
            await GetRepository<JsonField>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di campo JSON specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del campo JSON se trovata, altrimenti null.</returns>
        public async Task<JsonField?> GetJsonFieldByIdAsync(int id) =>
            await GetRepository<JsonField>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sulle Entità Target

        /// <summary>
        /// Aggiunge una nuova definizione di entità target al database.
        /// </summary>
        /// <param name="targetProperty">Definizione dell'entità target da aggiungere.</param>
        public async Task AddTargetPropertyAsync(TargetProperty targetProperty)
        {
            if (string.IsNullOrWhiteSpace(targetProperty.Namespace) || string.IsNullOrWhiteSpace(targetProperty.RootClass) ||
                string.IsNullOrWhiteSpace(targetProperty.Name))
            {
                throw new InvalidOperationException("I dati dell'entità target non possono essere vuoti.");
            }

            GetRepository<TargetProperty>().Add(targetProperty);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di entità target dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di entità target.</returns>
        public async Task<List<TargetProperty>> GetTargetPropertiesAsync() =>
            await GetRepository<TargetProperty>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di entità target specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione dell'entità target se trovata, altrimenti null.</returns>
        public async Task<TargetProperty?> GetTargetPropertyByIdAsync(int id) =>
            await GetRepository<TargetProperty>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sui Contesti di Database Target

        /// <summary>
        /// Aggiunge una nuova definizione di TargetDbContext al database.
        /// </summary>
        /// <param name="targetDbContext">Definizione del contesto target da aggiungere.</param>
        public async Task AddTargetDbContextInfoAsync(TargetDbContextInfo targetDbContext)
        {
            if (string.IsNullOrWhiteSpace(targetDbContext.Name) || string.IsNullOrWhiteSpace(targetDbContext.Namespace))
                throw new InvalidOperationException("Il nome e il namespace del TargetDbContext non possono essere vuoti.");

            GetRepository<TargetDbContextInfo>().Add(targetDbContext);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni dei contesti database target.
        /// </summary>
        /// <returns>Lista delle definizioni di TargetDbContext.</returns>
        public async Task<List<TargetDbContextInfo>> GetTargetDbContextInfoAsync() =>
            await GetRepository<TargetDbContextInfo>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di TargetDbContext specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del TargetDbContext se trovata, altrimenti null.</returns>
        public async Task<TargetDbContextInfo?> GetTargetDbContextInfoByIdAsync(int id) =>
            await GetRepository<TargetDbContextInfo>().GetByIdAsync(id);

        #endregion
    }
}