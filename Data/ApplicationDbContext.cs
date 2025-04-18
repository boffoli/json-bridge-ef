using Microsoft.EntityFrameworkCore;
using JsonBridgeEF.Config;
using JsonBridgeEF.Data.Configurations;
using JsonBridgeEF.ZZZ;
using JsonBridgeEF.Seeding.Mapping.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;
using JsonBridgeEF.Data.Configurations.JsonSource;

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
        public DbSet<JsonSchema> JsonSchemas { get; set; } = null!;
        public DbSet<JsonField> JsonFields { get; set; } = null!;
        public DbSet<TargetDbContextInfo> TargetDbContextInfos { get; set; } = null!;
        public DbSet<TargetProperty> TargetProperties { get; set; } = null!;
        public DbSet<MappingProject> MappingProjects { get; set; } = null!;
        public DbSet<MappingRule> MappingRules { get; set; } = null!;
        public DbSet<EntityKeyMapping> EntityKeyMapping { get; set; } = null!;

        protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
            }
        }

        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Caricamento delle configurazioni da file separati
            modelBuilder.ApplyConfiguration(new JsonSchemaConfig());
            modelBuilder.ApplyConfiguration(new JsonEntitiesConfig());
            modelBuilder.ApplyConfiguration(new JsonFieldConfig());
            modelBuilder.ApplyConfiguration(new TargetDbContextInfoConfig());
            modelBuilder.ApplyConfiguration(new TargetPropertyConfig());
            modelBuilder.ApplyConfiguration(new MappingProjectConfig());
            modelBuilder.ApplyConfiguration(new MappingRuleConfig());
            modelBuilder.ApplyConfiguration(new EntityKeyMappingConfig());
        }
    }
}