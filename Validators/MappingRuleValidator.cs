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
            ValidateJsonFieldId(model.JsonFieldId);
            ValidateTargetPropertyId(model.TargetPropertyId);
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

        // ======================== METODI PRIVATI PER JsonFieldId ========================
        private static void ValidateJsonFieldId(int jsonFieldId)
        {
            if (jsonFieldId <= 0)
                throw new ValidationException("The JsonFieldId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(JsonField).FullName!))
                throw new ValidationException($"The referenced JsonField (ID: {jsonFieldId}) does not exist.");
        }

        // ======================== METODI PRIVATI PER TargetPropertyId ========================
        private static void ValidateTargetPropertyId(int targetPropertyId)
        {
            if (targetPropertyId <= 0)
                throw new ValidationException("The TargetPropertyId must be a positive number.");

            if (!TargetModelInspector.TypeExists(typeof(TargetProperty).FullName!))
                throw new ValidationException($"The referenced TargetProperty (ID: {targetPropertyId}) does not exist.");
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