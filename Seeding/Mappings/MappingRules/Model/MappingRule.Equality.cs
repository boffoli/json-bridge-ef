using JsonBridgeEF.Seeding.Mappings.MappingRules.Interfaces;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Model
{
    /// <summary>
    /// Domain Class: Partial per l'override dell'uguaglianza semantica di MappingRule.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Due regole sono equivalenti se condividono gli stessi ID di mapping.</para>
    /// <para>Creation Strategy: Equals e GetHashCode implementano IEquatable&lt;MappingRule&gt;.</para>
    /// <para>Constraints: Confronto basato unicamente sugli ID.</para>
    /// <para>Usage Notes: Utilizzato per identificare duplicati e test di coerenza.</para>
    /// </remarks>
    internal sealed partial class MappingRule
    {
        public override bool Equals(object? obj) => obj is MappingRule other && Equals(other);

        /// <summary>
        /// Determines semantic equality based on mapping identifiers.
        /// </summary>
        public bool Equals(IMappingRule? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return MappingConfigurationId == other.MappingConfigurationId
                && JsonEntityId == other.JsonEntityId
                && JsonPropertyId == other.JsonPropertyId
                && ClassInfoId == other.ClassInfoId
                && ClassPropertyId == other.ClassPropertyId;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(MappingConfigurationId);
            hash.Add(JsonEntityId);
            hash.Add(JsonPropertyId);
            hash.Add(ClassInfoId);
            hash.Add(ClassPropertyId);
            return hash.ToHashCode();
        }
    }
}