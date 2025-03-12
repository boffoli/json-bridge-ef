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
        public async Task AddJsonSchemaDefAsync(JsonSchemaDef jsonSchema)
        {
            if (string.IsNullOrWhiteSpace(jsonSchema.Name) || string.IsNullOrWhiteSpace(jsonSchema.JsonSchemaIdentifier))
                throw new InvalidOperationException("Il nome e l'identificatore del tipo JSON non possono essere vuoti.");

            GetRepository<JsonSchemaDef>().Add(jsonSchema);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di tipi JSON dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di tipi JSON.</returns>
        public async Task<List<JsonSchemaDef>> GetJsonSchemaDefAsync() =>
            await GetRepository<JsonSchemaDef>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di tipo JSON specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del tipo JSON se trovata, altrimenti null.</returns>
        public async Task<JsonSchemaDef?> GetJsonSchemaDefByIdAsync(int id) =>
            await GetRepository<JsonSchemaDef>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sui Campi JSON

        /// <summary>
        /// Aggiunge una nuova definizione di campo JSON al database.
        /// </summary>
        /// <param name="jsonField">Definizione del campo JSON da aggiungere.</param>
        public async Task AddJsonFieldDefAsync(JsonFieldDef jsonField)
        {
            if (string.IsNullOrWhiteSpace(jsonField.SourceFieldPath))
                throw new InvalidOperationException("Il percorso del campo JSON non può essere vuoto.");

            GetRepository<JsonFieldDef>().Add(jsonField);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di campi JSON dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di campi JSON.</returns>
        public async Task<List<JsonFieldDef>> GetJsonFieldDefsAsync() =>
            await GetRepository<JsonFieldDef>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di campo JSON specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del campo JSON se trovata, altrimenti null.</returns>
        public async Task<JsonFieldDef?> GetJsonFieldDefByIdAsync(int id) =>
            await GetRepository<JsonFieldDef>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sulle Entità Target

        /// <summary>
        /// Aggiunge una nuova definizione di entità target al database.
        /// </summary>
        /// <param name="targetProperty">Definizione dell'entità target da aggiungere.</param>
        public async Task AddTargetPropertyDefAsync(TargetPropertyDef targetProperty)
        {
            if (string.IsNullOrWhiteSpace(targetProperty.Namespace) || string.IsNullOrWhiteSpace(targetProperty.RootClass) ||
                string.IsNullOrWhiteSpace(targetProperty.Name))
            {
                throw new InvalidOperationException("I dati dell'entità target non possono essere vuoti.");
            }

            GetRepository<TargetPropertyDef>().Add(targetProperty);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni di entità target dal database.
        /// </summary>
        /// <returns>Lista delle definizioni di entità target.</returns>
        public async Task<List<TargetPropertyDef>> GetTargetPropertyDefsAsync() =>
            await GetRepository<TargetPropertyDef>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di entità target specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione dell'entità target se trovata, altrimenti null.</returns>
        public async Task<TargetPropertyDef?> GetTargetPropertyDefByIdAsync(int id) =>
            await GetRepository<TargetPropertyDef>().GetByIdAsync(id);

        #endregion

        #region 🔹 Operazioni sui Contesti di Database Target

        /// <summary>
        /// Aggiunge una nuova definizione di TargetDbContext al database.
        /// </summary>
        /// <param name="targetDbContext">Definizione del contesto target da aggiungere.</param>
        public async Task AddTargetDbContextDefAsync(TargetDbContextDef targetDbContext)
        {
            if (string.IsNullOrWhiteSpace(targetDbContext.Name) || string.IsNullOrWhiteSpace(targetDbContext.Namespace))
                throw new InvalidOperationException("Il nome e il namespace del TargetDbContext non possono essere vuoti.");

            GetRepository<TargetDbContextDef>().Add(targetDbContext);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Recupera tutte le definizioni dei contesti database target.
        /// </summary>
        /// <returns>Lista delle definizioni di TargetDbContext.</returns>
        public async Task<List<TargetDbContextDef>> GetTargetDbContextDefAsync() =>
            await GetRepository<TargetDbContextDef>().GetAllAsync();

        /// <summary>
        /// Recupera una definizione di TargetDbContext specifica tramite identificatore.
        /// </summary>
        /// <param name="id">L'ID della definizione.</param>
        /// <returns>La definizione del TargetDbContext se trovata, altrimenti null.</returns>
        public async Task<TargetDbContextDef?> GetTargetDbContextDefByIdAsync(int id) =>
            await GetRepository<TargetDbContextDef>().GetByIdAsync(id);

        #endregion
    }
}