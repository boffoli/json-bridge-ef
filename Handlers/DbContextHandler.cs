using JsonBridgeEF.Config;
using JsonBridgeEF.Data;

namespace JsonBridgeEF.Handlers
{
    /// <summary>
    /// Gestisce i DbContext dell'applicazione.
    /// </summary>
    internal class DbContextHandler
    {
        /// <summary>
        /// Il contesto delle mappature, sempre fisso per default.
        /// </summary>
        public ApplicationDbContext ApplicationContext { get; private set; }

        /// <summary>
        /// Il contesto del database target, configurabile dinamicamente.
        /// </summary>
        public TargetDbContext TargetContext { get; private set; }

        /// <summary>
        /// Inizializza il gestore con i percorsi dei database specificati.
        /// </summary>
        public DbContextHandler()
        {
            // Usa DatabaseConfig per ottenere il percorso corretto
            string mappingDbPath = AppSettings.Get("Database:ApplicationDbPath");
            string targetDbPath = AppSettings.Get("Database:TargetDbPath");

            // Inizializza i contesti con il percorso completo
            ApplicationContext = new ApplicationDbContext(mappingDbPath);
            TargetContext = new TargetDbContext(targetDbPath);
        }

        /// <summary>
        /// Permette di cambiare dinamicamente il TargetDbContext.
        /// </summary>
        /// <param name="newDbPath">Percorso del nuovo database target.</param>
        public void ChangeTargetDbContext(string newDbPath)
        {
            if (string.IsNullOrWhiteSpace(newDbPath))
            {
                throw new ArgumentException("Il percorso del database target non può essere vuoto.", nameof(newDbPath));
            }

            TargetContext = new TargetDbContext(newDbPath);
            Console.WriteLine($"✅ Switched to TargetDbContext: {newDbPath}");
        }

        /// <summary>
        /// Permette di cambiare dinamicamente il MappingDbContext (se mai fosse necessario).
        /// </summary>
        /// <param name="newDbPath">Percorso del nuovo database delle mappature.</param>
        public void ChangeMappingDbContext(string newDbPath)
        {
            if (string.IsNullOrWhiteSpace(newDbPath))
            {
                throw new ArgumentException("Il percorso del database delle mappature non può essere vuoto.", nameof(newDbPath));
            }

            ApplicationContext = new ApplicationDbContext(newDbPath);
            Console.WriteLine($"✅ Switched to MappingDbContext: {newDbPath}");
        }
    }
}