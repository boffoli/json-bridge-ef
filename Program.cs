using JsonBridgeEF.Client;
using JsonBridgeEF.Seeding.SourceJson.Helpers;

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

            // Creazione del registro dei blocchi indipendenti per il processamento JSON
            var registry = new JsonIndepBlockRegistry();
            registry.AddBlock("utenti", "id_utente");
            registry.AddBlock("contatti", "id_contatto");
            registry.AddBlock("metadati", "id_metadato");

            // Creazione del servizio di orchestrazione e avvio del workflow
            var pipelineService = new FullPipelineService(registry);
            await pipelineService.RunFullPipelineAsync();

            Console.WriteLine("✅ Processo completato con successo!");
        }
    }
}