using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Seeding.SourceJson.Models;
using Newtonsoft.Json.Linq;

namespace JsonBridgeEF.Importing.Preprocessing.Helpers
{
    /// <summary>
    /// Classe responsabile della validazione dei blocchi indipendenti in un documento JSON.
    /// </summary>
    internal static class JsonIndepBlockValidator
    {
        /// <summary>
        /// Valida tutti i blocchi indipendenti presenti nel documento JSON.
        /// Se vengono trovati errori, lancia un'unica eccezione con tutti i problemi rilevati.
        /// </summary>
        /// <param name="root">Il documento JSON da validare.</param>
        /// <param name="expectedRegistry">Il registro contenente i blocchi indipendenti attesi.</param>
        /// <exception cref="InvalidOperationException">Se vengono trovati blocchi non validi.</exception>
        public static void Validate(JObject root, JsonIndepBlockRegistry expectedRegistry)
        {
            var validationErrors = new List<string>();
            ValidateJObject(root, expectedRegistry, validationErrors);

            if (validationErrors.Count != 0)
            {
                throw new InvalidOperationException(
                    $"La validazione ha rilevato errori nei blocchi indipendenti:\n{string.Join("\n", validationErrors)}"
                );
            }
        }

        /// <summary>
        /// Processa ricorsivamente un JObject per validare la presenza e la struttura dei blocchi indipendenti.
        /// </summary>
        private static void ValidateJObject(JObject jObject, JsonIndepBlockRegistry expectedRegistry, List<string> validationErrors)
        {
            foreach (JProperty prop in jObject.Properties().ToList())
            {
                if (expectedRegistry.ContainsBlock(prop.Name))
                {
                    var blockInfo = expectedRegistry.GetBlock(prop.Name);
                    if (blockInfo != null)
                    {
                        ValidateBlockProperty(prop, blockInfo, validationErrors);
                    }
                }

                // **Processa comunque i figli, anche se il nodo corrente è errato**
                ValidateNestedProperty(prop, expectedRegistry, validationErrors);
            }
        }

        /// <summary>
        /// Processa una proprietà che rappresenta un blocco indipendente e verifica la presenza della chiave.
        /// </summary>
        private static void ValidateBlockProperty(JProperty prop, JsonIndepBlockInfo blockInfo, List<string> validationErrors)
        {
            if (prop.Value.Type == JTokenType.Object)
            {
                ValidateObjectBlock((JObject)prop.Value, blockInfo, validationErrors);
            }
            else if (prop.Value.Type == JTokenType.Array)
            {
                ValidateArrayBlock((JArray)prop.Value, blockInfo, validationErrors);
            }
        }

        /// <summary>
        /// Valida un oggetto singolo che è stato identificato come blocco indipendente.
        /// </summary>
        private static void ValidateObjectBlock(JObject obj, JsonIndepBlockInfo blockInfo, List<string> validationErrors)
        {
            if (!obj.ContainsKey(blockInfo.KeyField))
            {
                validationErrors.Add($"❌ Il blocco '{blockInfo.Name}' non contiene la chiave obbligatoria '{blockInfo.KeyField}'.");
            }
        }

        /// <summary>
        /// Valida un array di oggetti identificati come blocchi indipendenti.
        /// </summary>
        private static void ValidateArrayBlock(JArray array, JsonIndepBlockInfo blockInfo, List<string> validationErrors)
        {
            foreach (JToken item in array)
            {
                if (item.Type == JTokenType.Object)
                {
                    ValidateObjectBlock((JObject)item, blockInfo, validationErrors);
                }
                else
                {
                    validationErrors.Add($"❌ Il blocco '{blockInfo.Name}' contiene un elemento non valido nell'array.");
                }
            }
        }

        /// <summary>
        /// Processa una proprietà annidata che non rappresenta un blocco indipendente.
        /// **Ora viene SEMPRE eseguita, indipendentemente dagli errori nei nodi superiori.**
        /// </summary>
        private static void ValidateNestedProperty(JProperty prop, JsonIndepBlockRegistry expectedRegistry, List<string> validationErrors)
        {
            if (prop.Value.Type == JTokenType.Object)
            {
                ValidateJObject((JObject)prop.Value, expectedRegistry, validationErrors);
            }
            else if (prop.Value.Type == JTokenType.Array)
            {
                ValidateJArray((JArray)prop.Value, expectedRegistry, validationErrors);
            }
        }

        /// <summary>
        /// Processa ricorsivamente un JArray per validare blocchi indipendenti.
        /// </summary>
        private static void ValidateJArray(JArray array, JsonIndepBlockRegistry expectedRegistry, List<string> validationErrors)
        {
            foreach (JToken item in array)
            {
                if (item.Type == JTokenType.Object)
                {
                    ValidateJObject((JObject)item, expectedRegistry, validationErrors);
                }
                else if (item.Type == JTokenType.Array)
                {
                    ValidateJArray((JArray)item, expectedRegistry, validationErrors);
                }
            }
        }
    }
}