using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configures the <see cref="TargetPropertyDef"/> entity for Entity Framework.
    /// Defines the primary key, required properties, and constraints.
    /// The relationship with <see cref="TargetDbContextDef"/> is configured on the principal side.
    /// </summary>
    internal class TargetPropertyDefConfig : IEntityTypeConfiguration<TargetPropertyDef>
    {
        public void Configure(EntityTypeBuilder<TargetPropertyDef> builder)
        {
            // Define primary key
            builder.HasKey(e => e.Id);

            // Required properties
            builder.Property(e => e.Namespace)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.RootClass)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            // Note:
            // The relationship 1:N with TargetDbContextDef is configured exclusively in the TargetDbContextDefConfig.
        }
    }
}