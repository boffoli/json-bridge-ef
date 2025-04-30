using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Helpers;
using JsonBridgeEF.Seeding.Mappings.Interfaces;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Validators;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Validators
{
    /// <summary>
    /// Domain Class: Validator per garantire la validità delle configurazioni di mapping tra JSON e DbContext.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Valida MappingConfiguration assicurandosi che tutti i riferimenti e le regole siano coerenti e completi.</para>
    /// <para>Creation Strategy: Utilizzato tramite injection nei servizi di gestione MappingConfiguration.</para>
    /// <para>Constraints: Tutti gli ID devono essere positivi e le regole devono essere valide individualmente.</para>
    /// <para>Relationships: Collabora con MappingRuleValidator per la validazione delle singole regole.</para>
    /// <para>Usage Notes: Chiamato prima della persistenza o della generazione di codice.</para>
    /// </remarks>
    internal sealed class MappingConfigurationValidator : IValidateAndFix<MappingConfiguration>
    {
        /// <inheritdoc />
        public void EnsureValid(MappingConfiguration model)
        {
            ValidateId(model.Id);
            ValidateJsonSchemaId(model.JsonSchemaId);
            ValidateTargetDbContextInfoId(model.DbContextInfoId);
            ValidateMappingRules(model.MappingRules);
        }

        /// <inheritdoc />
        public void Fix(MappingConfiguration model)
        {
            model.MappingRules = FixMappingRules(model.MappingRules);
        }

        #region Validazioni private

        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        private static void ValidateJsonSchemaId(int jsonSchemaId)
        {
            if (jsonSchemaId <= 0)
                throw new ValidationException("The JsonSchemaId must be a positive number.");
        }

        private static void ValidateTargetDbContextInfoId(int targetDbContextInfoId)
        {
            if (targetDbContextInfoId <= 0)
                throw new ValidationException("The TargetDbContextInfoId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(TargetDbContextInfo).FullName!))
                throw new ValidationException($"The referenced TargetDbContextInfo (ID: {targetDbContextInfoId}) does not exist.");
        }

        private static void ValidateMappingRules(ICollection<IMappingRule> mappingRules)
        {
            if (mappingRules == null)
                throw new ValidationException("The MappingRules collection cannot be null.");

            foreach (var rule in mappingRules)
            {
                var validator = new MappingRuleValidator();
                validator.EnsureValid(rule);
            }
        }

        private static ICollection<IMappingRule> FixMappingRules(ICollection<IMappingRule> mappingRules)
        {
            if (mappingRules == null)
                return [];

            var validator = new MappingRuleValidator();
            foreach (var rule in mappingRules)
            {
                validator.Fix(rule);
            }

            return mappingRules;
        }

        #endregion
    }
}