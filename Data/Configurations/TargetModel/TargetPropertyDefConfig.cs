using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Data.Configurations.TargetModel
{
    /// <summary>
    /// Configures the <see cref="TargetProperty"/> entity for Entity Framework.
    /// Defines the primary key, required properties, unique constraints,
    /// and the relationship with <see cref="TargetDbContextInfo"/>.
    /// </summary>
    internal class TargetPropertyConfig : IEntityTypeConfiguration<TargetProperty>
    {
        public void Configure(EntityTypeBuilder<TargetProperty> builder)
        {
            // 🔹 Definisce la chiave primaria
            builder.HasKey(e => e.Id);

            // 🔹 Proprietà obbligatorie con vincoli
            builder.Property(e => e.Namespace)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.RootClass)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.Path)
                   .HasMaxLength(500);

            // 🔹 Vincolo di unicità su Namespace, RootClass e Name
            builder.HasIndex(e => new { e.Namespace, e.RootClass, e.Name })
                   .IsUnique();

            // 🔹 Relazione con TargetDbContextInfo (Restrict per evitare eliminazioni accidentali)
            builder.HasOne(e => e.TargetDbContextInfo)
                   .WithMany(d => d.TargetProperties)
                   .HasForeignKey(e => e.TargetDbContextInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}