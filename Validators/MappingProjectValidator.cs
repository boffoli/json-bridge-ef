using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Helpers;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="MappingProject"/> to ensure validity of mapping project configurations.
    /// </summary>
    internal class MappingProjectValidator : IValidateAndFix<MappingProject>
    {
        /// <inheritdoc />
        public void EnsureValid(MappingProject model)
        {
            ValidateId(model.Id);
            ValidateName(model.Name);
            ValidateJsonSchemaDefId(model.JsonSchemaDefId);
            ValidateTargetDbContextDefId(model.TargetDbContextDefId);
            ValidateMappingRules(model.MappingRules);
        }

        /// <inheritdoc />
        public void Fix(MappingProject model)
        {
            model.MappingRules = FixMappingRules(model.MappingRules);
        }

        // ======================== METODI PRIVATI PER ID ========================
        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        // ======================== METODI PRIVATI PER NAME ========================
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("The Name cannot be empty.");
        }

        // ======================== METODI PRIVATI PER JsonSchemaDefId ========================
        private static void ValidateJsonSchemaDefId(int jsonSchemaDefId)
        {
            if (jsonSchemaDefId <= 0)
                throw new ValidationException("The JsonSchemaDefId must be a positive number.");
        }

        // ======================== METODI PRIVATI PER TargetDbContextDefId ========================
        private static void ValidateTargetDbContextDefId(int targetDbContextDefId)
        {
            if (targetDbContextDefId <= 0)
                throw new ValidationException("The TargetDbContextDefId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(TargetDbContextDef).FullName!))
                throw new ValidationException($"The referenced TargetDbContextDef (ID: {targetDbContextDefId}) does not exist.");
        }

        // ======================== METODI PRIVATI PER MappingRules ========================
        private static void ValidateMappingRules(ICollection<MappingRule> mappingRules)
        {
            if (mappingRules == null)
                throw new ValidationException("The MappingRules collection cannot be null.");

            foreach (var rule in mappingRules)
            {
                var validator = new MappingRuleValidator();
                validator.EnsureValid(rule);
            }
        }

        private static ICollection<MappingRule> FixMappingRules(ICollection<MappingRule> mappingRules)
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
    }
}