using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Helpers;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Model;
using JsonBridgeEF.Seeding.Source.Interfaces;


namespace JsonBridgeEF.Seeding.Mapping.MappingRules.Services
{
    /// <summary>
    /// Servizio per il seeding delle regole di mapping tra campi JSON e proprietà del database class.
    /// </summary>
    internal class MappingRuleSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding interattivo delle regole di mapping tra i campi JSON e le proprietà del database class.
        /// </summary>
        /// <param name="jsonProperties">Lista delle definizioni di campi JSON.</param>
        /// <param name="classProperties">Lista delle proprietà class.</param>
        /// <param name="mappingConfiguration">Il progetto di mapping di riferimento.</param>
        /// <returns>Lista delle regole di mapping salvate.</returns>
        public async Task<List<MappingRule>> SeedAsync(
            List<IJsonProperty> jsonProperties,
            List<IClassProperty> classProperties,
            MappingConfiguration mappingConfiguration)
        {
            ArgumentNullException.ThrowIfNull(jsonProperties);
            jsonProperties.ForEach(f => f.EnsureValid());
            ArgumentNullException.ThrowIfNull(classProperties);
            classProperties.ForEach(p => p.EnsureValid());
            mappingConfiguration.EnsureValid();

            Console.WriteLine("🔄 Avvio del popolamento interattivo delle regole di mapping...");
            var insertedRules = new List<MappingRule>();
            foreach (var classProp in classProperties)
            {
                var newRule = MappingRuleConsole.PromptMappingRuleForProperty(jsonProperties, classProp, mappingConfiguration);
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