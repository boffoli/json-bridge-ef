using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonBridgeEF.Seeding.Mappings.Models;

namespace JsonBridgeEF.Data.Configurations
{
    /// <summary>
    /// Configurazione della tabella <see cref="MappingRule"/> per Entity Framework.
    /// Definisce le relazioni con le entità collegate, gli indici e i vincoli di unicità.
    /// </summary>
    internal class MappingRuleConfig : IEntityTypeConfiguration<MappingRule>
    {
        /// <summary>
        /// Configura la mappatura per l'entità <see cref="MappingRule"/>.
        /// </summary>
        /// <param name="builder">Il costruttore dell'entità per definire la configurazione.</param>
        public void Configure(EntityTypeBuilder<MappingRule> builder)
        {
            // 1️⃣ Definisce la chiave primaria della tabella
            // 🔹 Garantisce che ogni record abbia un identificativo univoco.
            builder.HasKey(e => e.Id);

            // 2️⃣ Relazione con MappingProject (FK obbligatoria)
            // 🔹 Ogni MappingRule deve appartenere a un solo MappingProject.
            // 🔹 Un MappingProject può avere più MappingRule.
            builder.HasOne(e => e.MappingProject)
                   .WithMany(mp => mp.MappingRules)
                   .HasForeignKey(e => e.MappingProjectId)
                   .IsRequired();

            // 3️⃣ Relazione con JsonFieldDef (FK obbligatoria)
            // 🔹 Ogni MappingRule deve essere associata a un JsonFieldDef.
            // 🔹 Uno stesso JsonFieldDef può essere riutilizzato in più MappingRule.
            builder.HasOne(e => e.JsonFieldDef)
                   .WithMany()
                   .HasForeignKey(e => e.JsonFieldDefId)
                   .IsRequired();

            // 4️⃣ Relazione con TargetPropertyDef (FK obbligatoria)
            // 🔹 Ogni MappingRule deve essere associata a un TargetPropertyDef.
            // 🔹 Un TargetPropertyDef può essere utilizzato solo una volta in MappingRule.
            builder.HasOne(e => e.TargetPropertyDef)
                   .WithMany()
                   .HasForeignKey(e => e.TargetPropertyDefId)
                   .IsRequired();

            // 5️⃣ Configura la proprietà JsFormula
            // 🔹 Garantisce che JsFormula sia sempre valorizzata.
            // 🔹 Limita la lunghezza massima a 1024 caratteri per evitare valori eccessivi.
            builder.Property(e => e.JsFormula)
                   .IsRequired()
                   .HasMaxLength(1024);

            // 6️⃣ Vincolo di unicità su (MappingProjectId, JsonFieldDefId, TargetPropertyDefId)
            // 🔹 Garantisce che non possano esistere due MappingRule con la stessa combinazione
            //     di MappingProject, JsonFieldDef e TargetPropertyDef.
            builder.HasIndex(e => new 
            { 
                e.MappingProjectId, 
                e.JsonFieldDefId, 
                e.TargetPropertyDefId 
            }).IsUnique();

            // 7️⃣ Vincolo di unicità su TargetPropertyDefId
            // 🔹 Garantisce che un TargetPropertyDef possa apparire solo una volta in MappingRule.
            builder.HasIndex(e => e.TargetPropertyDefId)
                   .IsUnique();
        }
    }
}