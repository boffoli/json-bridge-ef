using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Model;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Validators
{
    /// <summary>
    /// Domain Class: Validator per garantire la validità di una regola di mapping tra una proprietà JSON e una proprietà target.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Valida i dati di una singola MappingRule assicurandone la coerenza e la completezza.</para>
    /// <para>Creation Strategy: Utilizzato automaticamente durante la validazione di MappingConfiguration.</para>
    /// <para>Constraints: Gli ID devono essere positivi e la formula JavaScript deve essere presente.</para>
    /// <para>Relationships: Utilizzato in MappingConfigurationValidator.</para>
    /// <para>Usage Notes: Chiamare EnsureValid prima del salvataggio o della generazione di codice.</para>
    /// </remarks>
    internal sealed class MappingRuleValidator : IValidateAndFix<MappingRule>
    {
        /// <inheritdoc />
        public void EnsureValid(MappingRule model)
        {
            ValidateId(model.Id);
            ValidateMappingConfigurationId(model.MappingConfigurationId);
            ValidateJsonEntityId(model.JsonEntityId);
            ValidateJsonPropertyId(model.JsonPropertyId);
            ValidateTargetClassId(model.ClassInfoId);
            ValidateTargetPropertyId(model.ClassPropertyId);
            ValidateJsFormula(model.JsFormula);
        }

        /// <inheritdoc />
        public void Fix(MappingRule model)
        {
            model.JsFormula = FixJsFormula(model.JsFormula);
        }

        #region Validazioni private

        private static void ValidateId(int id)
        {
            if (id < 0)
                throw new ValidationException("The Id cannot be negative.");
        }

        private static void ValidateMappingConfigurationId(int mappingConfigurationId)
        {
            if (mappingConfigurationId <= 0)
                throw new ValidationException("The MappingConfigurationId must be a positive number.");
        }

        private static void ValidateJsonEntityId(int jsonEntityId)
        {
            if (jsonEntityId <= 0)
                throw new ValidationException("The JsonEntityId must be a positive number.");
        }

        private static void ValidateJsonPropertyId(int jsonPropertyId)
        {
            if (jsonPropertyId <= 0)
                throw new ValidationException("The JsonPropertyId must be a positive number.");
        }

        private static void ValidateTargetClassId(int targetClassId)
        {
            if (targetClassId <= 0)
                throw new ValidationException("The TargetClassId must be a positive number.");
        }

        private static void ValidateTargetPropertyId(int targetPropertyId)
        {
            if (targetPropertyId <= 0)
                throw new ValidationException("The TargetPropertyId must be a positive number.");
        }

        private static void ValidateJsFormula(string jsFormula)
        {
            if (string.IsNullOrWhiteSpace(jsFormula))
                throw new ValidationException("The JsFormula cannot be null or empty.");
        }

        private static string FixJsFormula(string jsFormula)
        {
            if (string.IsNullOrWhiteSpace(jsFormula))
                return "function transform(value) { return value; }";

            return jsFormula.Trim();
        }

        #endregion
    }
}
