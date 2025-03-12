using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configura l'entità <see cref="JsonIndepBlockInfo"/> per Entity Framework.
    /// Definisce la struttura della tabella, la chiave primaria, i vincoli per le proprietà e la relazione 1:N con <see cref="JsonSchemaDef"/>
    /// utilizzando esclusivamente la foreign key.
    /// </summary>
    internal class JsonIndepBlockInfoConfig : IEntityTypeConfiguration<JsonIndepBlockInfo>
    {
        public void Configure(EntityTypeBuilder<JsonIndepBlockInfo> builder)
        {
            // 🔹 Definisce la chiave primaria
            builder.HasKey(b => b.Id);

            // 🔹 Proprietà obbligatorie con vincoli
            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(b => b.KeyField)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(b => b.ForeignKeyName)
                   .IsRequired()
                   .HasMaxLength(255);

            // 🔹 La proprietà della foreign key è obbligatoria
            builder.Property(b => b.JsonSchemaDefId)
                   .IsRequired();

            // 🔹 Configura la relazione 1:N con JsonSchemaDef utilizzando solo la foreign key.
            // La navigazione è unidirezionale: dal padre (JsonSchemaDef) verso il figlio (JsonIndepBlockInfo).
            builder.HasOne<JsonSchemaDef>()
                   .WithMany(s => s.JsonIndepBlockInfos)
                   .HasForeignKey(b => b.JsonSchemaDefId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}