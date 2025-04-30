using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Interfaces;

namespace JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model
{
    /// <summary>
    /// Domain Class: Partial contenente l'uguaglianza semantica di MappingConfiguration.
    /// </summary>
    /// <remarks>
    /// <para>Domain Concept: Due configurazioni sono equivalenti se hanno gli stessi
    /// JsonSchemaId, DbContextInfoId e identiche regole di mapping.</para>
    /// <para>Creation Strategy: EqualsByValue e GetValueHashCode vengono usati internamente da Equals/GetHashCode.</para>
    /// <para>Constraints: La comparazione delle regole avviene ordinando per Id.</para>
    /// <para>Usage Notes: Implementa IEquatable per ottimizzare i confronti.</para>
    /// </remarks>
    internal partial class MappingConfiguration
    {
        public override bool Equals(object? obj) => obj is MappingConfiguration other && Equals(other);

        /// <summary>
        /// Semantic equality based on schema, context, and mapping rules.
        /// </summary>
        public bool Equals(IMappingConfiguration? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (JsonSchemaId != other.JsonSchemaId) return false;
            if (DbContextInfoId != other.DbContextInfoId) return false;

            if (MappingRules.Count != other.MappingRules.Count) return false;
            var orderedThis  = MappingRules.OrderBy(r => r.Id);
            var orderedOther = other.MappingRules.OrderBy(r => r.Id);
            return orderedThis.SequenceEqual(orderedOther);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(JsonSchemaId);
            hash.Add(DbContextInfoId);
            foreach (var rule in MappingRules.OrderBy(r => r.Id))
                hash.Add(rule.GetHashCode());
            return hash.ToHashCode();
        }
    }
}