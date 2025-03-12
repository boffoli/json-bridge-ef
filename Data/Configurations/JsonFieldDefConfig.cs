using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configures the <see cref="JsonFieldDef"/> entity for Entity Framework.
    /// Defines the primary key, required properties, unique constraints,
    /// and the 1:N relationship with <see cref="JsonSchemaDef"/> via its foreign key.
    /// The navigation is unidirectional (from JsonSchemaDef to JsonFieldDef).
    /// </summary>
    internal class JsonFieldDefConfig : IEntityTypeConfiguration<JsonFieldDef>
    {
        /// <summary>
        /// Configures the database schema for <see cref="JsonFieldDef"/>.
        /// </summary>
        /// <param name="builder">The entity type builder for <see cref="JsonFieldDef"/>.</param>
        public void Configure(EntityTypeBuilder<JsonFieldDef> builder)
        {
            // Define primary key
            builder.HasKey(e => e.Id);

            // Configure required properties with constraints
            builder.Property(e => e.SourceFieldPath)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(e => e.JsonSchemaDefId)
                   .IsRequired();

            // Define relationship 1:N using only the foreign key.
            // Each JsonFieldDef is associated with one JsonSchemaDef,
            // and the navigation is unidirectional (from JsonSchemaDef to JsonFieldDef).
            builder.HasOne<JsonSchemaDef>()
                   .WithMany(s => s.JsonFieldDefs)
                   .HasForeignKey(e => e.JsonSchemaDefId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint on (JsonSchemaDefId, SourceFieldPath)
            builder.HasIndex(e => new { e.JsonSchemaDefId, e.SourceFieldPath })
                   .IsUnique();
        }
    }
}