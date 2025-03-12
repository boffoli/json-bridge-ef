using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.Mappings.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configurazione per <see cref="MappingProject"/> in Entity Framework.
    /// Definisce le relazioni 1-1 verso JsonSchemaDef e TargetDbContextDef (senza cascade)
    /// e la relazione 1-N verso MappingRule (con cascade).
    /// </summary>
    internal class MappingProjectConfig : IEntityTypeConfiguration<MappingProject>
    {
        public void Configure(EntityTypeBuilder<MappingProject> builder)
        {
            // 1️⃣ Definisce la chiave primaria
            builder.HasKey(e => e.Id);

            // 2️⃣ Proprietà obbligatorie
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            // 3️⃣ Relazione 1-1 con JsonSchemaDef (senza cascade)
            builder.HasOne(e => e.JsonSchemaDef)
                   .WithOne() // navigazione inversa non implementata
                   .HasForeignKey<MappingProject>(e => e.JsonSchemaDefId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 4️⃣ Relazione 1-1 con TargetDbContextDef (senza cascade)
            builder.HasOne(e => e.TargetDbContextDef)
                   .WithOne() // navigazione inversa non implementata
                   .HasForeignKey<MappingProject>(e => e.TargetDbContextDefId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 5️⃣ Relazione 1-N con MappingRule (con cascade)
            builder.HasMany(e => e.MappingRules)
                   .WithOne(r => r.MappingProject)
                   .HasForeignKey(r => r.MappingProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // 6️⃣ Indice univoco sul nome del progetto
            builder.HasIndex(e => e.Name)
                   .IsUnique();
        }
    }
}