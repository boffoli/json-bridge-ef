using JsonBridgeEF.Seeding.SourceJson.Helpers;
using Newtonsoft.Json.Linq;
using JsonBridgeEF.Importing.Preprocessing.Helpers;

namespace JsonBridgeEF.Importing.Preprocessing.Services
{
    /// <summary>
    /// Servizio riusabile per la validazione, estrazione e ordinamento dei blocchi indipendenti da un documento JSON.
    /// </summary>
    /// <remarks>
    /// Costruttore che accetta un registro personalizzato di blocchi indipendenti.
    /// </remarks>
    internal class JsonProcessor(JsonIndepBlockRegistry registry)
    {
        private readonly JsonIndepBlockRegistry _registry = registry ?? throw new ArgumentNullException(nameof(registry));

        /// <summary>
        /// Processa un file JSON, estrae e ordina i blocchi indipendenti.
        /// </summary>
        /// <param name="inputFilePath">Percorso del file JSON da elaborare.</param>
        /// <returns>Un JObject contenente i blocchi indipendenti ordinati.</returns>
        public JObject ProcessJsonFile(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"❌ File non trovato: {inputFilePath}");
            }

            // Legge il file e lo converte in JObject
            JObject root = JObject.Parse(File.ReadAllText(inputFilePath));

            return ProcessJsonObject(root);
        }

        /// <summary>
        /// Processa un JSON direttamente da una stringa.
        /// </summary>
        /// <param name="jsonContent">Contenuto JSON in formato stringa.</param>
        /// <returns>Un JObject contenente i blocchi indipendenti ordinati.</returns>
        public JObject ProcessJsonString(string jsonContent)
        {
            JObject root = JObject.Parse(jsonContent);
            return ProcessJsonObject(root);
        }

        /// <summary>
        /// Processa un JObject, validando e ordinando i blocchi indipendenti.
        /// </summary>
        /// <param name="root">L'oggetto JSON da elaborare.</param>
        /// <returns>Un JObject contenente i blocchi indipendenti ordinati.</returns>
        private JObject ProcessJsonObject(JObject root)
        {
            ArgumentNullException.ThrowIfNull(root);

            try
            {
                // **Fase 1: Validazione**
                JsonIndepBlockValidator.Validate(root, _registry);

                // **Fase 2: Estrazione**
                Dictionary<string, List<JObject>> indepBlocks = JsonIndepBlockExtractor.ExtractBlocks(root, _registry);

                // **Fase 3: Ordinamento**
                List<KeyValuePair<string, List<JObject>>> sortedBlocks = JsonIndepBlockSorter.SortBlocksTopologically(indepBlocks);

                // Creazione del nuovo JSON ordinato
                return new JObject
                {
                    ["independent_blocks"] = JObject.FromObject(sortedBlocks.ToDictionary(kv => kv.Key, kv => kv.Value))
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"❌ Errore durante l'elaborazione del JSON: {ex.Message}", ex);
            }
        }
    }
}