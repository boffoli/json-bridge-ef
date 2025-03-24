using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.TargetModel.Helpers;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Services
{
    /// <summary>
    /// Servizio per il seeding delle definizioni delle proprietà target nel database.
    /// </summary>
    internal class TargetPropertySeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding delle definizioni delle proprietà target.
        /// </summary>
        /// <param name="targetDbContextInfo">Il contesto target di riferimento.</param>
        /// <param name="targetNamespace">Il namespace contenente le entità target.</param>
        /// <param name="referenceEntityType">Un tipo di riferimento per determinare l'assembly corretto.</param>
        internal async Task<List<TargetProperty>> SeedAsync(
            TargetDbContextInfo targetDbContextInfo,
            string targetNamespace,
            Type referenceEntityType)
        {
            ArgumentNullException.ThrowIfNull(targetDbContextInfo);
            if (string.IsNullOrWhiteSpace(targetNamespace)) throw new ArgumentNullException(nameof(targetNamespace));
            ArgumentNullException.ThrowIfNull(referenceEntityType);

            Console.WriteLine($"📂 Creazione delle proprietà target per il contesto: {targetDbContextInfo.Name} nel namespace '{targetNamespace}'");

            // Genera le proprietà in base al namespace e all'entità di riferimento
            var definitions = TargetPropertyHelper.GenerateTargetProperties(targetDbContextInfo.Id, targetNamespace, referenceEntityType);

            foreach (var prop in definitions)
            {
                if (string.IsNullOrWhiteSpace(prop.Namespace) || string.IsNullOrWhiteSpace(prop.RootClass) || string.IsNullOrWhiteSpace(prop.Name))
                {
                    throw new InvalidOperationException("I dati dell'entità target non possono essere vuoti.");
                }
                prop.TryValidateAndFix();

                GetRepository<TargetProperty>().Add(prop);
                await SaveChangesAsync();
            }
            return definitions;
        }
    }
}