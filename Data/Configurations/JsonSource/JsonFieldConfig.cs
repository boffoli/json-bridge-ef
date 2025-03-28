using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations.JsonSource
{
    /// <summary>
    /// Configura l'entità <see cref="JsonField"/> per Entity Framework.
    /// Definisce i vincoli obbligatori, la relazione col blocco padre e l’unicità dei percorsi.
    /// </summary>
    internal class JsonFieldConfig : IEntityTypeConfiguration<JsonField>
    {
        public void Configure(EntityTypeBuilder<JsonField> builder)
        {
            // 🔹 Chiave primaria
            builder.HasKey(f => f.Id);

            // 🔹 La FK verso JsonBlock è obbligatoria
            builder.Property(f => f.OwnerId)
                   .IsRequired();

            // 🔹 Relazione con JsonBlock (1:N) - Cascade delete
            builder.HasOne(f => f.Owner)
                   .WithMany(b => b.Entities)
                   .HasForeignKey(f => f.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}