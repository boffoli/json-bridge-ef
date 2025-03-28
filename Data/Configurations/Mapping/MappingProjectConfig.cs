using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.Mapping.Models;

namespace JsonBridgeEF.Data.Configurations.Mapping
{
    /// <summary>
    /// Configures the <see cref="MappingProject"/> entity for Entity Framework.
    /// Defines:
    /// - 1:1 relationships with <see cref="JsonSchema"/> and <see cref="TargetDbContextInfo"/> (without cascade delete).
    /// - 1:N relationship with <see cref="MappingRule"/> (with cascade delete).
    /// - Unique constraint on <see cref="MappingProject.Name"/>.
    /// </summary>
    internal class MappingProjectConfig : IEntityTypeConfiguration<MappingProject>
    {
        public void Configure(EntityTypeBuilder<MappingProject> builder)
        {
            // 🔹 Primary key
            builder.HasKey(e => e.Id);

            // 🔹 Required properties
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            // 🔹 1:1 Relationship with JsonSchema (no cascade delete)
            builder.HasOne(e => e.JsonSchema)
                   .WithOne() // No navigation back to MappingProject
                   .HasForeignKey<MappingProject>(e => e.JsonSchemaId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 🔹 1:1 Relationship with TargetDbContextInfo (no cascade delete)
            builder.HasOne(e => e.TargetDbContextInfo)
                   .WithOne() // No navigation back to MappingProject
                   .HasForeignKey<MappingProject>(e => e.TargetDbContextInfoId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 🔹 1:N Relationship with MappingRule (cascade delete enabled)
            builder.HasMany(e => e.MappingRules)
                   .WithOne(r => r.MappingProject)
                   .HasForeignKey(r => r.MappingProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Unique index on Name
            builder.HasIndex(e => e.Name)
                   .IsUnique();
        }
    }
}