using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.Mapping.Models;

namespace JsonBridgeEF.Data.Configurations.Mapping
{
    /// <summary>
    /// Configures the <see cref="MappingRule"/> entity for Entity Framework.
    /// Defines primary keys, required properties, and relationships.
    /// </summary>
    internal class MappingRuleConfig : IEntityTypeConfiguration<MappingRule>
    {
        public void Configure(EntityTypeBuilder<MappingRule> builder)
        {
            // 1️⃣ Define primary key
            builder.HasKey(e => e.Id);

            // 2️⃣ Required properties
            builder.Property(e => e.JsFormula)
                   .IsRequired()
                   .HasMaxLength(1000);

            // 3️⃣ Relationship with MappingProject (N:1)
            // 🔹 Each MappingRule must be associated with one MappingProject.
            // 🔹 One MappingProject can have multiple MappingRules.
            // 🔹 If a MappingProject is deleted, all its MappingRules are deleted (Cascade).
            builder.HasOne(e => e.MappingProject)
                   .WithMany(p => p.MappingRules)
                   .HasForeignKey(e => e.MappingProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // 4️⃣ Relationship with JsonField (N:1)
            // 🔹 Each MappingRule must be linked to one JsonField.
            // 🔹 One JsonField can be reused in multiple MappingRules.
            // 🔹 If a JsonField is deleted, the MappingRule remains (Restrict).
            builder.HasOne(e => e.JsonField)
                   .WithMany()
                   .HasForeignKey(e => e.JsonFieldId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 5️⃣ Relationship with TargetProperty (N:1)
            // 🔹 Each MappingRule must be linked to one TargetProperty.
            // 🔹 One TargetProperty can be used in multiple MappingRules.
            // 🔹 If a TargetProperty is deleted, the MappingRules remain (Restrict).
            builder.HasOne(e => e.TargetProperty)
                   .WithMany(p => p.MappingRules) // Explicit relationship
                   .HasForeignKey(e => e.TargetPropertyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            // 6️⃣ Unique constraint on (MappingProjectId, JsonFieldId, TargetPropertyId)
            // Ensures that the same source-target mapping is not duplicated within a project.
            builder.HasIndex(e => new { e.MappingProjectId, e.JsonFieldId, e.TargetPropertyId })
                   .IsUnique();
        }
    }
}