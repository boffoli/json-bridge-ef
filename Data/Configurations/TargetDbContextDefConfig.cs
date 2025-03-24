using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configures the <see cref="TargetDbContextInfo"/> entity for Entity Framework.
    /// Defines the primary key, required properties, unique constraints, and relationships.
    /// </summary>
    internal class TargetDbContextInfoConfig : IEntityTypeConfiguration<TargetDbContextInfo>
    {
        public void Configure(EntityTypeBuilder<TargetDbContextInfo> builder)
        {
            // 🔹 Definisce la chiave primaria
            builder.HasKey(e => e.Id);

            // 🔹 Proprietà obbligatorie con vincoli
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Namespace)
                   .IsRequired()
                   .HasMaxLength(255);

            // 🔹 Unicità per Namespace (un DbContext non può avere lo stesso Namespace)
            builder.HasIndex(e => e.Namespace)
                   .IsUnique();

            // 🔹 1:N con TargetProperty (Restrict per evitare eliminazioni accidentali)
            builder.HasMany(d => d.TargetProperties)
                   .WithOne(p => p.TargetDbContextInfo)
                   .HasForeignKey(p => p.TargetDbContextInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}