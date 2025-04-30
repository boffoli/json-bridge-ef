using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Services
{
    /// <summary>
    /// Servizio per il seeding dei progetti di mapping nel database.
    /// </summary>
    internal class MappingConfiguratorSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding di un progetto di mapping passato come parametro.
        /// </summary>
        /// <param name="mappingConfiguration">Il progetto di mapping da salvare nel database.</param>
        /// <returns>Il progetto di mapping creato e salvato nel database.</returns>
        /// <exception cref="ArgumentNullException">Sollevata se il progetto di mapping è nullo.</exception>
        /// <exception cref="InvalidOperationException">Sollevata se il progetto non viene trovato dopo l'inserimento.</exception>
        public async Task<MappingConfiguration> SeedAsync(MappingConfiguration mappingConfiguration)
        {
            mappingConfiguration.EnsureValid();

            GetRepository<MappingConfiguration>().Add(mappingConfiguration);
            await SaveChangesAsync();

            return mappingConfiguration;
        }
    }
}