using Microsoft.EntityFrameworkCore;
using JsonBridgeEF.Config;
using JsonBridgeEF.Data.Configurations;
using JsonBridgeEF.ZZZ;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Data
{
    /// <summary>
    /// DbContext per la gestione delle regole di mapping dei dati e per il target database.
    /// Utilizza le nuove classi modello per definire le regole di mapping e le associazioni.
    /// Il percorso del database viene determinato dal parametro "databasePath".
    /// </summary>
    internal class ApplicationDbContext(string databasePath) : DbContext
    {
        private readonly string _databasePath = databasePath;

        /// <summary>
        /// Costruttore predefinito richiesto per le migrazioni.
        /// Carica automaticamente il percorso del database da DatabaseConfig.
        /// </summary>
        public ApplicationDbContext() : this(AppSettings.Get("Database:ApplicationDbPath")) { }

        // DbSet per le tabelle di definizione e mapping
        public DbSet<JsonSchemaDef> JsonSchemaDefs { get; set; } = null!;
        public DbSet<JsonFieldDef> JsonFieldDefs { get; set; } = null!;
        public DbSet<TargetDbContextDef> TargetDbContextDefs { get; set; } = null!;
        public DbSet<TargetPropertyDef> TargetPropertyDefs { get; set; } = null!;
        public DbSet<MappingProject> MappingProjects { get; set; } = null!;
        public DbSet<MappingRule> MappingRules { get; set; } = null!;
        public DbSet<EntityKeyMapping> EntityKeyMappings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Caricamento delle configurazioni da file separati
            modelBuilder.ApplyConfiguration(new JsonSchemaDefConfig());
            modelBuilder.ApplyConfiguration(new JsonIndepBlockInfoConfig());
            modelBuilder.ApplyConfiguration(new JsonFieldDefConfig());
            modelBuilder.ApplyConfiguration(new TargetDbContextDefConfig());
            modelBuilder.ApplyConfiguration(new TargetPropertyDefConfig());
            modelBuilder.ApplyConfiguration(new MappingProjectConfig());
            modelBuilder.ApplyConfiguration(new MappingRuleConfig());
            modelBuilder.ApplyConfiguration(new EntityKeyMappingConfig());
        }
    }
}