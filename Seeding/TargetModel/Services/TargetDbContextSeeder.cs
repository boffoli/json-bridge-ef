using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Services
{
    /// <summary>
    /// Servizio per il seeding della definizione del contesto target nel database.
    /// </summary>
    internal class TargetDbContextSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding della definizione del contesto target nel database.
        /// </summary>
        /// <param name="targetDbContextDef">L'istanza di TargetDbContextDef da salvare.</param>
        internal async Task<TargetDbContextDef> SeedAsync(TargetDbContextDef targetDbContextDef)
        {
            targetDbContextDef.EnsureValid();

            Console.WriteLine($"📂 Creazione della definizione del contesto target: {targetDbContextDef.Name}");

            GetRepository<TargetDbContextDef>().Add(targetDbContextDef);
            await SaveChangesAsync();
            return targetDbContextDef;
        }
    }
}