using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mappings.Models;

namespace JsonBridgeEF.Seeding.Mappings.Services
{
    /// <summary>
    /// Servizio per il seeding dei progetti di mapping nel database.
    /// </summary>
    internal class MappingProjectSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding di un progetto di mapping passato come parametro.
        /// </summary>
        /// <param name="mappingProject">Il progetto di mapping da salvare nel database.</param>
        /// <returns>Il progetto di mapping creato e salvato nel database.</returns>
        /// <exception cref="ArgumentNullException">Sollevata se il progetto di mapping è nullo.</exception>
        /// <exception cref="InvalidOperationException">Sollevata se il progetto non viene trovato dopo l'inserimento.</exception>
        public async Task<MappingProject> SeedAsync(MappingProject mappingProject)
        {
            mappingProject.EnsureValid();

            GetRepository<MappingProject>().Add(mappingProject);
            await SaveChangesAsync();

            return mappingProject;
        }
    }
}