using Microsoft.EntityFrameworkCore;
using JsonBridgeEF.Config;
using JsonBridgeEF.SampleTargetModel;

namespace JsonBridgeEF.Data
{
    /// <summary>
    /// Contesto del database target che rappresenta il database reale dell'applicazione.
    /// </summary>
    internal class TargetDbContext(string databasePath) : DbContext
    {
        private readonly string _databasePath = databasePath;

        /// <summary>
        /// Costruttore vuoto richiesto per le migration.
        /// Carica automaticamente il percorso del database da DatabaseConfig.
        /// </summary>
        public TargetDbContext() : this(AppSettings.Get("Database:TargetDatabase")) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserContact> UserContacts { get; set; } = null!;
        public DbSet<Metadata> Metadata { get; set; } = null!;

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

            // ✅ Relazione 1:1 tra User e UserContact (opzionale)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Contact)
                .WithOne()
                .HasForeignKey<UserContact>(uc => uc.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Configurazione di FullContactInfo all'interno di UserContact
            modelBuilder.Entity<UserContact>()
                .Property(uc => uc.FullContactInfo)
                .IsRequired();

            // ✅ Configurazione di UserPreferences come Value Object dentro UserContact
            modelBuilder.Entity<UserContact>()
                .OwnsOne(uc => uc.Preferences, np =>
                {
                    np.Property(p => p.Theme).IsRequired();
                    np.Property(p => p.EmailNotifications).IsRequired();
                });

            // ✅ Configurazione della tabella Metadata
            modelBuilder.Entity<Metadata>()
                .HasKey(m => m.MetadataId);
        }
    }
}