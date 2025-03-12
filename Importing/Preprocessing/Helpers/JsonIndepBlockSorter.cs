using JsonBridgeEF.Seeding.SourceJson.Models;
using Newtonsoft.Json.Linq;

namespace JsonBridgeEF.Importing.Preprocessing.Helpers
{
    /// <summary>
    /// Helper per ordinare i blocchi indipendenti topologicamente partendo dalle foglie.
    /// </summary>
    internal static class JsonIndepBlockSorter
    {
        /// <summary>
        /// Ordina i blocchi indipendenti topologicamente, elaborando prima le foglie.
        /// </summary>
        public static List<KeyValuePair<string, List<JObject>>> SortBlocksTopologically(Dictionary<string, List<JObject>> blocks)
        {
            var sorted = new List<KeyValuePair<string, List<JObject>>>();

            Console.WriteLine("\n🔍 Inizializzazione grafo delle dipendenze...");
            var (inDegree, adjacencyList) = BuildDependencyGraph(blocks);

            PrintDependencies(adjacencyList);
            PrintInDegree(inDegree);

            var queue = InitializeQueue(inDegree);
            ProcessQueue(queue, adjacencyList, inDegree, blocks, sorted);

            // ** 🔥 Fase di Ripristino: Se rimangono blocchi orfani, li aggiungiamo alla fine **
            EnsureAllBlocksAreAdded(blocks, sorted);

            Console.WriteLine("\n✅ Ordinamento completato.");
            return sorted;
        }

        /// <summary>
        /// Costruisce il grafo delle dipendenze analizzando le foreign key nei blocchi.
        /// </summary>
        private static (Dictionary<string, int>, Dictionary<string, List<string>>) BuildDependencyGraph(Dictionary<string, List<JObject>> blocks)
        {
            var inDegree = blocks.Keys.ToDictionary(k => k, _ => 0);
            var adjacencyList = new Dictionary<string, List<string>>();

            foreach (var (blockName, items) in blocks)
            {
                foreach (var propertyName in items.SelectMany(item => item.Properties().Select(p => p.Name)))
                {
                    Console.WriteLine($"🛠 Analizzando proprietà: {propertyName} in {blockName}");

                    if (!JsonIndepBlockInfo.IsForeignKeyFormat(propertyName)) continue;

                    var (referencedBlockName, _) = JsonIndepBlockInfo.ExtractForeignKeyComponents(propertyName);
                    Console.WriteLine($"🔗 Trovata FK: {propertyName} → {referencedBlockName}");

                    if (!blocks.ContainsKey(referencedBlockName)) continue;

                    AddDependency(adjacencyList, referencedBlockName, blockName);
                    inDegree[blockName]++;
                }
            }
            return (inDegree, adjacencyList);
        }

        /// <summary>
        /// Aggiunge una dipendenza tra due blocchi.
        /// </summary>
        private static void AddDependency(Dictionary<string, List<string>> adjacencyList, string parent, string child)
        {
            if (!adjacencyList.ContainsKey(parent))
                adjacencyList[parent] = [];

            if (!adjacencyList[parent].Contains(child))
                adjacencyList[parent].Add(child);
        }

        /// <summary>
        /// Inizializza la coda con i blocchi che non hanno dipendenze (foglie).
        /// </summary>
        private static Queue<string> InitializeQueue(Dictionary<string, int> inDegree)
        {
            return new Queue<string>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
        }

        /// <summary>
        /// Processa la coda per generare l'ordinamento topologico.
        /// </summary>
        private static void ProcessQueue(Queue<string> queue, Dictionary<string, List<string>> adjacencyList,
            Dictionary<string, int> inDegree, Dictionary<string, List<JObject>> blocks, List<KeyValuePair<string, List<JObject>>> sorted)
        {
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                sorted.Add(new KeyValuePair<string, List<JObject>>(current, blocks[current]));
                Console.WriteLine($"✅ Aggiunto '{current}' all'ordine finale.");

                if (!adjacencyList.ContainsKey(current)) continue;

                foreach (string dependent in adjacencyList[current])
                {
                    inDegree[dependent]--;
                    if (inDegree[dependent] == 0)
                    {
                        queue.Enqueue(dependent);
                    }
                }
            }
        }

        /// <summary>
        /// Se rimangono blocchi che non sono stati aggiunti, li inserisce alla fine.
        /// </summary>
        private static void EnsureAllBlocksAreAdded(Dictionary<string, List<JObject>> blocks, List<KeyValuePair<string, List<JObject>>> sorted)
        {
            var sortedKeys = new HashSet<string>(sorted.Select(kv => kv.Key));

            foreach (var block in blocks)
            {
                if (!sortedKeys.Contains(block.Key))
                {
                    Console.WriteLine($"⚠️ Blocco '{block.Key}' non ordinato, aggiunto in coda.");
                    sorted.Add(new KeyValuePair<string, List<JObject>>(block.Key, block.Value));
                }
            }
        }

        /// <summary>
        /// Stampa a console tutte le dipendenze trovate tra i blocchi indipendenti.
        /// </summary>
        private static void PrintDependencies(Dictionary<string, List<string>> adjacencyList)
        {
            Console.WriteLine("\n🔗 Dipendenze trovate tra i blocchi indipendenti:");
            foreach (var entry in adjacencyList)
            {
                Console.WriteLine($"  {entry.Key} dipende da: {string.Join(", ", entry.Value)}");
            }
        }

        /// <summary>
        /// Stampa i valori di inDegree per il debug.
        /// </summary>
        private static void PrintInDegree(Dictionary<string, int> inDegree)
        {
            Console.WriteLine("\n📊 Valori di inDegree:");
            foreach (var kvp in inDegree)
            {
                Console.WriteLine($"   {kvp.Key} → {kvp.Value}");
            }
        }
    }
}