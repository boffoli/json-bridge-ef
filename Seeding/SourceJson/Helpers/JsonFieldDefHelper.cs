using System.Text.Json;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers
{
    /// <summary>
    /// Helper per l'estrazione e la gestione dei campi JSON nel processo di mapping.
    /// </summary>
    internal static class JsonFieldDefHelper
    {
        /// <summary>
        /// Legge un file JSON, filtra le proprietà di sistema ed estrae i campi JSON associandoli a un JSON Schema.
        /// </summary>
        /// <param name="filePath">Il percorso del file JSON.</param>
        /// <param name="jsonSchemaId">L'ID dello schema JSON a cui associare i campi.</param>
        /// <returns>Lista dei campi JSON estratti.</returns>
        /// <exception cref="FileNotFoundException">Se il file non esiste.</exception>
        public static List<JsonFieldDef> ExtractJsonFields(string filePath, int jsonSchemaId)
        {
            var jsonElement = ReadJsonFile(filePath); // 🔹 Legge il file JSON
            var jsonFields = new List<JsonFieldDef>();

            foreach (var rootProperty in FilterOutSystemProperties(jsonElement))
            {
                ExtractJsonFieldsRecursive(rootProperty.Value, rootProperty.Name, jsonFields, jsonSchemaId);
            }

            return jsonFields;
        }

        /// <summary>
        /// Legge un file JSON e restituisce l'elemento radice come <see cref="JsonElement"/>.
        /// </summary>
        /// <param name="filePath">Percorso del file JSON.</param>
        /// <returns>Elemento radice del JSON.</returns>
        /// <exception cref="FileNotFoundException">Se il file non esiste.</exception>
        private static JsonElement ReadJsonFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"❌ JSON file not found: {filePath}");

            string jsonContent = File.ReadAllText(filePath);
            return JsonDocument.Parse(jsonContent).RootElement;
        }

        /// <summary>
        /// Filtra le proprietà JSON eliminando quelle considerate di sistema (es. quelle che iniziano con '_').
        /// </summary>
        /// <param name="jsonElement">Elemento JSON da analizzare.</param>
        /// <returns>Un elenco di proprietà JSON senza le proprietà di sistema.</returns>
        private static IEnumerable<JsonProperty> FilterOutSystemProperties(JsonElement jsonElement)
        {
            return jsonElement.EnumerateObject().Where(p => !p.Name.StartsWith('_'));
        }

        /// <summary>
        /// Determina se un campo JSON può essere considerato una chiave (se il nome inizia con "id_").
        /// </summary>
        /// <param name="fieldPath">Il percorso completo del campo JSON.</param>
        /// <returns>True se è una chiave, altrimenti false.</returns>
        private static bool IsPotentialKeyField(string fieldPath)
        {
            var parts = fieldPath.Split('.');
            return parts[^1].StartsWith("id_"); // Usa l'ultima parte del percorso per la verifica
        }

        /// <summary>
        /// Metodo ricorsivo per estrarre i campi JSON e aggiungerli alla lista, associando ogni campo a un JSON Schema.
        /// </summary>
        /// <param name="element">Elemento JSON da elaborare.</param>
        /// <param name="currentPath">Path corrente per costruire il <see cref="JsonFieldDef"/>.</param>
        /// <param name="jsonFields">Lista in cui aggiungere i campi estratti.</param>
        /// <param name="jsonSchemaId">L'ID dello schema JSON a cui associare i campi.</param>
        private static void ExtractJsonFieldsRecursive(JsonElement element, string currentPath, List<JsonFieldDef> jsonFields, int jsonSchemaId)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        string fieldPath = $"{currentPath}.{property.Name}";
                        ExtractJsonFieldsRecursive(property.Value, fieldPath, jsonFields, jsonSchemaId);
                    }
                    break;

                case JsonValueKind.Array:
                    if (element.GetArrayLength() > 0)
                    {
                        ExtractJsonFieldsRecursive(element[0], currentPath, jsonFields, jsonSchemaId);
                    }
                    break;

                default:
                    jsonFields.Add(new JsonFieldDef(new JsonFieldDefValidator())
                    {
                        SourceFieldPath = currentPath,
                        JsonSchemaDefId = jsonSchemaId, // 🔹 Assegna la FK dello schema JSON
                        IsKey = IsPotentialKeyField(currentPath),
                        Description = $"Campo estratto automaticamente da '{currentPath}'"
                    });
                    break;
            }
        }
    }
}