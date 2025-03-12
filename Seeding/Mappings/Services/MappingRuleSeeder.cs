using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mappings.Helpers;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mappings.Services
{
    /// <summary>
    /// Servizio per il seeding delle regole di mapping tra campi JSON e proprietà del database target.
    /// </summary>
    internal class MappingRuleSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding interattivo delle regole di mapping tra i campi JSON e le proprietà del database target.
        /// </summary>
        /// <param name="jsonFieldDefs">Lista delle definizioni di campi JSON.</param>
        /// <param name="targetPropertyDefs">Lista delle proprietà target.</param>
        /// <param name="mappingProject">Il progetto di mapping di riferimento.</param>
        /// <returns>Lista delle regole di mapping salvate.</returns>
        public async Task<List<MappingRule>> SeedAsync(
            List<JsonFieldDef> jsonFieldDefs,
            List<TargetPropertyDef> targetPropertyDefs,
            MappingProject mappingProject)
        {
            ArgumentNullException.ThrowIfNull(jsonFieldDefs);
            jsonFieldDefs.ForEach(f => f.EnsureValid());
            ArgumentNullException.ThrowIfNull(targetPropertyDefs);
            targetPropertyDefs.ForEach(p => p.EnsureValid());
            mappingProject.EnsureValid();

            Console.WriteLine("🔄 Avvio del popolamento interattivo delle regole di mapping...");
            var insertedRules = new List<MappingRule>();
            foreach (var targetProp in targetPropertyDefs)
            {
                var newRule = MappingRuleConsole.PromptMappingRuleForProperty(jsonFieldDefs, targetProp, mappingProject);
                if (newRule == null)
                    continue;
                newRule.TryValidateAndFix();
                GetRepository<MappingRule>().Add(newRule);
                await SaveChangesAsync(); insertedRules.Add(newRule);
            }
            return insertedRules;
        }
    }
}