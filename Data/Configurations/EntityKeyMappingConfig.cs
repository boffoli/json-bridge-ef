using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.ZZZ;

namespace JsonBridgeEF.Data.Configurations
{
    internal class EntityKeyMappingConfig : IEntityTypeConfiguration<EntityKeyMapping>
    {
        public void Configure(EntityTypeBuilder<EntityKeyMapping> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.JsonKey).IsRequired();
            builder.HasOne(e => e.TargetPropertyDef)
                   .WithMany()
                   .HasForeignKey(e => e.TargetPropertyDefId)
                   .IsRequired();
            builder.HasIndex(e => new { e.JsonKey, e.TargetPropertyDefId })
                   .IsUnique();
        }
    }
}