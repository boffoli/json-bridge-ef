using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configura l'entità <see cref="JsonSchemaDef"/> per Entity Framework.
    /// Definisce la chiave primaria e i vincoli di unicità.
    /// La relazione con <see cref="JsonFieldDef"/> e <see cref="JsonIndepBlockInfo"/> viene gestita dalle entità dipendenti.
    /// </summary>
    internal class JsonSchemaDefConfig : IEntityTypeConfiguration<JsonSchemaDef>
    {
        public void Configure(EntityTypeBuilder<JsonSchemaDef> builder)
        {
            // 🔹 Definisce la chiave primaria
            builder.HasKey(e => e.Id);

            // 🔹 Proprietà obbligatorie
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.JsonSchemaIdentifier)
                   .IsRequired()
                   .HasMaxLength(255);

            // 🔹 Indice univoco per JsonSchemaIdentifier
            builder.HasIndex(e => e.JsonSchemaIdentifier)
                   .IsUnique();
        }
    }
}