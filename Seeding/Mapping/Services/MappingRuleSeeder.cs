using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mapping.Helpers;
using JsonBridgeEF.Seeding.Mapping.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mapping.Services
{
    /// <summary>
    /// Servizio per il seeding delle regole di mapping tra campi JSON e proprietà del database target.
    /// </summary>
    internal class MappingRuleSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding interattivo delle regole di mapping tra i campi JSON e le proprietà del database target.
        /// </summary>
        /// <param name="jsonFields">Lista delle definizioni di campi JSON.</param>
        /// <param name="targetPropertys">Lista delle proprietà target.</param>
        /// <param name="mappingProject">Il progetto di mapping di riferimento.</param>
        /// <returns>Lista delle regole di mapping salvate.</returns>
        public async Task<List<MappingRule>> SeedAsync(
            List<JsonField> jsonFields,
            List<TargetProperty> targetPropertys,
            MappingProject mappingProject)
        {
            ArgumentNullException.ThrowIfNull(jsonFields);
            jsonFields.ForEach(f => f.EnsureValid());
            ArgumentNullException.ThrowIfNull(targetPropertys);
            targetPropertys.ForEach(p => p.EnsureValid());
            mappingProject.EnsureValid();

            Console.WriteLine("🔄 Avvio del popolamento interattivo delle regole di mapping...");
            var insertedRules = new List<MappingRule>();
            foreach (var targetProp in targetPropertys)
            {
                var newRule = MappingRuleConsole.PromptMappingRuleForProperty(jsonFields, targetProp, mappingProject);
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