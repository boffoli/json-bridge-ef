using JsonBridgeEF.Client;
using JsonBridgeEF.Handlers;
using JsonBridgeEF.Common.UnitOfWorks;

namespace JsonBridgeEF
{
    /// <summary>
    /// Entry point principale per l'applicazione della console.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Metodo principale asincrono per avviare l'applicazione della console.
        /// </summary>
        public static async Task Main()
        {
            Console.WriteLine("🚀 Avvio di JsonBridgeEF...");

            // Inizializza il gestore dei DbContext
            var dbContextHandler = new DbContextHandler();

            // Crea UnitOfWork basandosi su ApplicationDbContext gestito dal handler
            using var unitOfWork = new UnitOfWork(dbContextHandler.ApplicationContext);

            // Creazione del servizio di orchestrazione e avvio del workflow
            var pipelineService = new SeedingPipelineService(unitOfWork);
            await pipelineService.RunSeedingAsync();

            Console.WriteLine("✅ Processo completato con successo!");
        }
    }
}