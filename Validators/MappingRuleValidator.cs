using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Config;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Helpers;
using JsonBridgeEF.Validators;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Validators
{
    /// <summary>
    /// Validator for <see cref="MappingRule"/> ensuring the validity of mapping rules.
    /// </summary>
    internal class MappingRuleValidator : IValidateAndFix<MappingRule>
    {
        /// <inheritdoc />
        public void EnsureValid(MappingRule model)
        {
            ValidateId(model.Id);
            ValidateMappingProjectId(model.MappingProjectId);
            ValidateJsonFieldDefId(model.JsonFieldDefId);
            ValidateTargetPropertyDefId(model.TargetPropertyDefId);
            ValidateJsFormula(model.JsFormula);
        }

        /// <inheritdoc />
        public void Fix(MappingRule model)
        {
            model.JsFormula = FixJsFormula(model.JsFormula);
        }

        // ======================== METODI PRIVATI PER ID ========================
        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        // ======================== METODI PRIVATI PER MappingProjectId ========================
        private static void ValidateMappingProjectId(int mappingProjectId)
        {
            if (mappingProjectId <= 0)
                throw new ValidationException("The MappingProjectId must be a positive number.");
        }

        // ======================== METODI PRIVATI PER JsonFieldDefId ========================
        private static void ValidateJsonFieldDefId(int jsonFieldDefId)
        {
            if (jsonFieldDefId <= 0)
                throw new ValidationException("The JsonFieldDefId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(JsonFieldDef).FullName!))
                throw new ValidationException($"The referenced JsonFieldDef (ID: {jsonFieldDefId}) does not exist.");
        }

        // ======================== METODI PRIVATI PER TargetPropertyDefId ========================
        private static void ValidateTargetPropertyDefId(int targetPropertyDefId)
        {
            if (targetPropertyDefId <= 0)
                throw new ValidationException("The TargetPropertyDefId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(TargetPropertyDef).FullName!))
                throw new ValidationException($"The referenced TargetPropertyDef (ID: {targetPropertyDefId}) does not exist.");
        }

        // ======================== METODI PRIVATI PER JsFormula ========================
        private static void ValidateJsFormula(string jsFormula)
        {
            if (string.IsNullOrWhiteSpace(jsFormula))
                throw new ValidationException("The JsFormula cannot be empty.");
        }

        private static string FixJsFormula(string? jsFormula)
        {
            return string.IsNullOrWhiteSpace(jsFormula) ? AppSettings.Get("Defaults:JsFormula") : jsFormula;
        }
    }
}