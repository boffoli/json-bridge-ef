using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configura l'entità <see cref="JsonBlock"/> per Entity Framework.
    /// Definisce la struttura, i vincoli e le relazioni verso <see cref="JsonSchema"/> e <see cref="JsonField"/>.
    /// </summary>
    internal class JsonBlockConfig : IEntityTypeConfiguration<JsonBlock>
    {
        public void Configure(EntityTypeBuilder<JsonBlock> builder)
        {
            // 🔹 Chiave primaria
            builder.HasKey(b => b.Id);

            // 🔹 Nome obbligatorio e unico per schema
            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(b => new { b.JsonSchemaId, b.Name })
                   .IsUnique();

            // 🔹 Relazione con JsonSchema (1:N)
            builder.HasOne(b => b.JsonSchema)
                   .WithMany(s => s.JsonBlocks)
                   .HasForeignKey(b => b.JsonSchemaId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Relazione molti-a-molti auto-relazionale (padre-figlio tra blocchi)
            builder.HasMany(b => b.ChildBlocks)
                   .WithMany(b => b.ParentBlocks)
                   .UsingEntity(j => j.ToTable("JsonBlockRelationships"));

            // 🔹 Relazione con i campi (1:N)
            builder.HasMany(b => b.JsonFields)
                   .WithOne(f => f.JsonBlock)
                   .HasForeignKey(f => f.JsonBlockId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}