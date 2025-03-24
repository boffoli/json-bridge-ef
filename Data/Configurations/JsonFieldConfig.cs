using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations
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

            // 🔹 Campo obbligatorio e lunghezza massima
            builder.Property(f => f.SourceFieldPath)
                   .IsRequired()
                   .HasMaxLength(500);

            // 🔹 La FK verso JsonBlock è obbligatoria
            builder.Property(f => f.JsonBlockId)
                   .IsRequired();

            // 🔹 Relazione con JsonBlock (1:N) - Cascade delete
            builder.HasOne(f => f.JsonBlock)
                   .WithMany(b => b.JsonFields)
                   .HasForeignKey(f => f.JsonBlockId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Indice unico sul percorso del campo all’interno del blocco
            builder.HasIndex(f => new { f.JsonBlockId, f.SourceFieldPath })
                   .IsUnique();
        }
    }
}