using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configures the <see cref="TargetDbContextDef"/> entity for Entity Framework.
    /// Defines the primary key, required properties, and relationships.
    /// </summary>
    internal class TargetDbContextDefConfig : IEntityTypeConfiguration<TargetDbContextDef>
    {
        public void Configure(EntityTypeBuilder<TargetDbContextDef> builder)
        {
            // Define primary key
            builder.HasKey(e => e.Id);

            // Required properties
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Namespace)
                   .IsRequired()
                   .HasMaxLength(255);

            // 1:N Relationship with TargetPropertyDef
            builder.HasMany(d => d.TargetProperties)
                   .WithOne(p => p.TargetDbContextDef)
                   .HasForeignKey(p => p.TargetDbContextDefId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}