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
        /// <param name="targetDbContextInfo">L'istanza di TargetDbContextInfo da salvare.</param>
        internal async Task<TargetDbContextInfo> SeedAsync(TargetDbContextInfo targetDbContextInfo)
        {
            targetDbContextInfo.EnsureValid();

            Console.WriteLine($"📂 Creazione della definizione del contesto target: {targetDbContextInfo.Name}");

            GetRepository<TargetDbContextInfo>().Add(targetDbContextInfo);
            await SaveChangesAsync();
            return targetDbContextInfo;
        }
    }
}