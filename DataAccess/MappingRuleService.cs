using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mapping.Models;

namespace JsonBridgeEF.DataAccess
{
    /// <summary>
    /// Servizio per la gestione di tutte le regole di mapping, senza vincoli su un singolo progetto.
    /// </summary>
    internal class MappingRuleService(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {

        #region 🔹 Operazioni CRUD sulle regole di mapping

        /// <summary>
        /// Recupera tutte le regole di mapping presenti nel database.
        /// </summary>
        /// <returns>Lista di tutte le regole di mapping.</returns>
        public async Task<List<MappingRule>> GetAllRulesAsync()
        {
            return await GetRepository<MappingRule>().GetAllAsync();
        }

        /// <summary>
        /// Recupera una regola di mapping in base al suo ID.
        /// </summary>
        /// <param name="id">ID della regola di mapping.</param>
        /// <returns>Istanza della regola di mapping se trovata, altrimenti null.</returns>
        public async Task<MappingRule?> GetMappingByIdAsync(int id)
        {
            return await GetRepository<MappingRule>().GetByIdAsync(id);
        }

        /// <summary>
        /// Aggiunge una nuova regola di mapping e la salva nel database.
        /// </summary>
        /// <param name="mapping">Regola di mapping da aggiungere.</param>
        public async Task AddMappingAsync(MappingRule mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping), "La regola di mapping non può essere null.");

            mapping.EnsureValid();
            GetRepository<MappingRule>().Add(mapping);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna una regola di mapping esistente.
        /// </summary>
        /// <param name="mapping">Regola di mapping da aggiornare.</param>
        public async Task UpdateMappingAsync(MappingRule mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping), "La regola di mapping non può essere null.");

            var existingRule = await GetMappingByIdAsync(mapping.Id)
                ?? throw new KeyNotFoundException($"Nessuna regola di mapping trovata con ID {mapping.Id}.");

            mapping.EnsureValid();
            existingRule.JsonFieldId = mapping.JsonFieldId;
            existingRule.TargetPropertyId = mapping.TargetPropertyId;
            existingRule.JsFormula = mapping.JsFormula;

            await SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una regola di mapping dal database.
        /// </summary>
        /// <param name="id">ID della regola di mapping da eliminare.</param>
        public async Task DeleteMappingAsync(int id)
        {
            var mapping = await GetMappingByIdAsync(id)
                ?? throw new KeyNotFoundException($"Nessuna regola di mapping trovata con ID {id}.");

            GetRepository<MappingRule>().Remove(mapping);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Cancella tutte le regole di mapping nel database.
        /// </summary>
        public async Task ClearAllRulesAsync()
        {
            var rulesToRemove = await GetAllRulesAsync();

            if (rulesToRemove.Count == 0)
            {
                Console.WriteLine("⚠️ Nessuna regola di mapping da eliminare.");
                return;
            }

            GetRepository<MappingRule>().RemoveRange(rulesToRemove);
            await SaveChangesAsync();
        }

        #endregion
    }
}