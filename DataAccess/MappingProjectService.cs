using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mappings.Models;

namespace JsonBridgeEF.DataAccess
{
    /// <summary>
    /// Servizio per la gestione dei progetti di mapping.
    /// </summary>
    internal class MappingProjectService(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Recupera tutti i progetti di mapping presenti nel database.
        /// </summary>
        public async Task<List<MappingProject>> GetAllProjectsAsync() =>
            await GetRepository<MappingProject>().GetAllAsync();

        /// <summary>
        /// Recupera un progetto di mapping in base al suo ID.
        /// </summary>
        public async Task<MappingProject?> GetProjectByIdAsync(int projectId) =>
            await GetRepository<MappingProject>().GetByIdAsync(projectId);

        /// <summary>
        /// Aggiunge un nuovo progetto di mapping al database.
        /// </summary>
        public async Task AddProjectAsync(MappingProject project)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
                throw new InvalidOperationException("Il nome del progetto non può essere vuoto.");

            if (project.JsonSchemaId <= 0)
                throw new InvalidOperationException("Il progetto deve essere associato a un tipo JSON valido.");

            GetRepository<MappingProject>().Add(project);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna un progetto di mapping esistente.
        /// </summary>
        public async Task UpdateProjectAsync(MappingProject project)
        {
            GetRepository<MappingProject>().Update(project);
            await SaveChangesAsync();
        }
    }
}