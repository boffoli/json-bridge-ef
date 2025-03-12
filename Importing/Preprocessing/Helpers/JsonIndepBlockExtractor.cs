using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Seeding.SourceJson.Models;
using Newtonsoft.Json.Linq;

namespace JsonBridgeEF.Importing.Preprocessing.Helpers
{
    /// <summary>
    /// Classe responsabile dell'estrazione dei blocchi indipendenti da un documento JSON.
    /// I blocchi indipendenti (quelli registrati in <see cref="JsonIndepBlockRegistry"/>)
    /// vengono estratti, rimossi dal nodo padre e sostituiti con una chiave esterna generata da <see cref="JsonIndepBlockInfo"/>.
    /// </summary>
    internal static class JsonIndepBlockExtractor
    {
        /// <summary>
        /// Estrae tutti i blocchi indipendenti registrati in <paramref name="expectedRegistry"/> dal documento JSON.
        /// I blocchi vengono rimossi dalla loro posizione originale e referenziati tramite una foreign key.
        /// </summary>
        /// <param name="root">Il documento JSON come <see cref="JObject"/>.</param>
        /// <param name="expectedRegistry">Il registro dei blocchi indipendenti attesi.</param>
        /// <returns>Un dizionario contenente i blocchi indipendenti estratti, organizzati per nome.</returns>
        public static Dictionary<string, List<JObject>> ExtractBlocks(JObject root, JsonIndepBlockRegistry expectedRegistry)
        {
            var extractedBlocks = new Dictionary<string, List<JObject>>();
            ExtractFromJObject(root, expectedRegistry, extractedBlocks);
            return extractedBlocks;
        }

        /// <summary>
        /// Processa ricorsivamente un <see cref="JObject"/> per individuare e rimuovere i blocchi indipendenti.
        /// Se un nodo corrisponde a un blocco indipendente, viene rimosso e sostituito con la foreign key generata.
        /// </summary>
        private static void ExtractFromJObject(JObject jObject, JsonIndepBlockRegistry expectedRegistry, Dictionary<string, List<JObject>> extractedBlocks)
        {
            var propertiesToReplace = new List<(string originalName, string foreignKeyName, JToken foreignKeyValue)>();

            foreach (JProperty prop in jObject.Properties().ToList())
            {
                if (expectedRegistry.ContainsBlock(prop.Name))
                {
                    var replacement = ProcessBlockProperty(prop, expectedRegistry, extractedBlocks);
                    if (replacement.HasValue)
                    {
                        propertiesToReplace.Add(replacement.Value);
                    }
                }
                else
                {
                    ProcessNestedProperty(prop, expectedRegistry, extractedBlocks);
                }
            }

            ApplyReplacements(jObject, propertiesToReplace);
        }

        /// <summary>
        /// Processa una proprietà che rappresenta un blocco indipendente.
        /// </summary>
        private static (string originalName, string foreignKeyName, JToken foreignKeyValue)? ProcessBlockProperty(
            JProperty prop, JsonIndepBlockRegistry expectedRegistry, Dictionary<string, List<JObject>> extractedBlocks)
        {
            var blockInfo = expectedRegistry.GetBlock(prop.Name);
            if (blockInfo == null)
                return null;

            if (prop.Value.Type == JTokenType.Object)
            {
                return ProcessObjectBlock(prop, blockInfo, extractedBlocks, expectedRegistry);
            }

            if (prop.Value.Type == JTokenType.Array)
            {
                return ProcessArrayBlock(prop, blockInfo, extractedBlocks, expectedRegistry);
            }

            return null;
        }

        /// <summary>
        /// Processa una proprietà annidata che non rappresenta un blocco indipendente.
        /// </summary>
        private static void ProcessNestedProperty(
            JProperty prop, JsonIndepBlockRegistry expectedRegistry, Dictionary<string, List<JObject>> extractedBlocks)
        {
            if (prop.Value.Type == JTokenType.Object)
            {
                ExtractFromJObject((JObject)prop.Value, expectedRegistry, extractedBlocks);
            }
            else if (prop.Value.Type == JTokenType.Array)
            {
                ExtractFromJArray((JArray)prop.Value, expectedRegistry, extractedBlocks);
            }
        }

        /// <summary>
        /// Processa un oggetto singolo che è stato identificato come blocco indipendente.
        /// </summary>
        private static (string originalName, string foreignKeyName, JToken foreignKeyValue) ProcessObjectBlock(
    JProperty prop, JsonIndepBlockInfo blockInfo, Dictionary<string, List<JObject>> extractedBlocks, JsonIndepBlockRegistry expectedRegistry)
        {
            JObject childObj = (JObject)prop.Value;
            ValidateBlockKey(childObj, blockInfo);

            AddBlock(extractedBlocks, blockInfo.Name, childObj);

            string foreignKeyName = blockInfo.ForeignKeyName;

            // Verifica che la chiave sia presente e non sia null
            if (!childObj.TryGetValue(blockInfo.KeyField, out JToken? foreignKeyValue) || foreignKeyValue is null)
            {
                throw new InvalidOperationException(
                    $"Il valore della chiave '{blockInfo.KeyField}' nel blocco '{blockInfo.Name}' è null o mancante.");
            }

            ExtractFromJObject(childObj, expectedRegistry, extractedBlocks);

            return (prop.Name, foreignKeyName, foreignKeyValue);
        }

        /// <summary>
        /// Processa un array di oggetti identificati come blocchi indipendenti.
        /// </summary>
        private static (string originalName, string foreignKeyName, JToken foreignKeyValue)? ProcessArrayBlock(
            JProperty prop, JsonIndepBlockInfo blockInfo, Dictionary<string, List<JObject>> extractedBlocks, JsonIndepBlockRegistry expectedRegistry)
        {
            var keyList = new List<JToken>();

            foreach (JToken item in (JArray)prop.Value)
            {
                if (item.Type == JTokenType.Object)
                {
                    JObject itemObj = (JObject)item;
                    ValidateBlockKey(itemObj, blockInfo);

                    // Controlla che il valore della chiave non sia null
                    if (!itemObj.ContainsKey(blockInfo.KeyField) || itemObj[blockInfo.KeyField] is null)
                    {
                        throw new InvalidOperationException(
                            $"Il valore della chiave '{blockInfo.KeyField}' nel blocco '{blockInfo.Name}' è null o mancante.");
                    }

                    keyList.Add(itemObj[blockInfo.KeyField]!);
                    AddBlock(extractedBlocks, blockInfo.Name, itemObj);
                    ExtractFromJObject(itemObj, expectedRegistry, extractedBlocks);
                }
            }

            return keyList.Count != 0 ? (prop.Name, blockInfo.ForeignKeyName, new JArray(keyList)) : null;
        }

        /// <summary>
        /// Processa ricorsivamente un <see cref="JArray"/> per individuare e rimuovere blocchi indipendenti.
        /// </summary>
        private static void ExtractFromJArray(JArray jArray, JsonIndepBlockRegistry expectedRegistry, Dictionary<string, List<JObject>> extractedBlocks)
        {
            foreach (JToken item in jArray)
            {
                if (item.Type == JTokenType.Object)
                {
                    ExtractFromJObject((JObject)item, expectedRegistry, extractedBlocks);
                }
                else if (item.Type == JTokenType.Array)
                {
                    ExtractFromJArray((JArray)item, expectedRegistry, extractedBlocks);
                }
            }
        }

        /// <summary>
        /// Aggiunge un blocco al dizionario dei blocchi indipendenti estratti.
        /// </summary>
        private static void AddBlock(Dictionary<string, List<JObject>> extractedBlocks, string blockName, JObject block)
        {
            if (!extractedBlocks.TryGetValue(blockName, out List<JObject>? value))
            {
                value = [];
                extractedBlocks[blockName] = value;
            }

            value.Add(block);
        }

        /// <summary>
        /// Valida che un oggetto abbia il campo chiave atteso per il blocco indipendente.
        /// </summary>
        private static void ValidateBlockKey(JObject block, JsonIndepBlockInfo blockInfo)
        {
            if (!block.ContainsKey(blockInfo.KeyField))
            {
                throw new InvalidOperationException($"Il blocco '{blockInfo.Name}' non contiene la chiave attesa '{blockInfo.KeyField}'.");
            }
        }

        /// <summary>
        /// Sostituisce le proprietà rimosse con la relativa foreign key.
        /// </summary>
        private static void ApplyReplacements(JObject jObject, List<(string originalName, string foreignKeyName, JToken foreignKeyValue)> replacements)
        {
            foreach (var (originalName, foreignKeyName, foreignKeyValue) in replacements)
            {
                jObject.Remove(originalName);
                jObject[foreignKeyName] = foreignKeyValue;
            }
        }
    }
}