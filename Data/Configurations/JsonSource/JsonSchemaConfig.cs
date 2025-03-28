using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations.JsonSource
{
    /// <summary>
    /// Configura l'entità <see cref="JsonSchema"/> per Entity Framework.
    /// Definisce vincoli, relazioni e comportamenti di persistenza.
    /// </summary>
    internal class JsonSchemaConfig : IEntityTypeConfiguration<JsonSchema>
    {
        public void Configure(EntityTypeBuilder<JsonSchema> builder)
        {
            // 🔹 Chiave primaria
            builder.HasKey(e => e.Id);

            // 🔹 Proprietà obbligatorie
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.JsonSchemaContent)
                   .IsRequired()
                   .HasColumnType("TEXT");

            // 🔹 Indice univoco sul nome
            builder.HasIndex(e => e.Name)
                   .IsUnique();

            // 🔹 Relazione 1:N con i blocchi
            builder.HasMany(e => e.Entities)
                   .WithOne(b => b.Owner)
                   .HasForeignKey(b => b.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}