using System.Text.Json;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Source.Validators;

namespace JsonBridgeEF.Seeding.Source.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Helper per l'estrazione e la generazione automatica dei <see cref="JsonProperty"/> da uno schema JSON di esempio.
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// I campi vengono estratti solo se di tipo primitivo. Oggetti e array sono ignorati (trattati come blocchi separati).  
/// L’estrazione è guidata dal nome del blocco, che funge da path nel documento JSON.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il blocco deve esistere all’interno del JSON.</item>
///   <item>Solo campi di tipo primitivo sono considerati validi.</item>
/// </list>
/// </para>
/// </summary>
internal static class JsonPropertyExtractor
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Punto di accesso principale per estrarre i campi da un blocco specifico all'interno di uno <see cref="JsonSchema"/>.
    /// </para>
    /// <param name="schema">Lo schema JSON di origine.</param>
    /// <param name="jsonEntity">Il blocco (entità) per cui estrarre i campi.</param>
    /// <returns>Lista dei campi validi estratti dal blocco.</returns>
    public static IReadOnlyList<Model.JsonProperties.JsonProperty> ExtractJsonProperties(JsonSchema schema, JsonEntity jsonEntity)
    {
        using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
        return ExtractJsonProperties(jsonDoc.RootElement, jsonEntity);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Estrae i campi di un blocco JSON a partire da un elemento già parsato.
    /// </para>
    /// <param name="root">Radice dell’albero JSON.</param>
    /// <param name="jsonEntity">Entità target per cui si vogliono estrarre i campi.</param>
    /// <returns>Lista dei campi trovati all’interno del blocco specificato.</returns>
    public static IReadOnlyList<Model.JsonProperties.JsonProperty> ExtractJsonProperties(JsonElement root, JsonEntity jsonEntity)
    {
        var jsonEntityElement = FindJsonEntities(root, jsonEntity.Name);
        if (!jsonEntityElement.HasValue)
            return [];

        var fields = new List<Model.JsonProperties.JsonProperty>();
        ExtractJsonPropertiesRecursive(jsonEntityElement.Value, jsonEntity, fields);
        return fields;
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Naviga il documento JSON fino al blocco corrispondente al path indicato.
    /// </para>
    /// <param name="root">Radice del documento JSON.</param>
    /// <param name="jsonEntityPath">Path dell'entità nel documento (es. "person.address").</param>
    /// <returns>Il nodo JSON corrispondente al blocco, se trovato; altrimenti <c>null</c>.</returns>
    private static JsonElement? FindJsonEntities(JsonElement root, string jsonEntityPath)
    {
        var pathSegments = jsonEntityPath.Split('.');
        JsonElement current = root;

        foreach (var segment in pathSegments)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty("properties", out current))
                return null;

            if (!current.TryGetProperty(segment, out current))
                return null;
        }

        return current;
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Estrae ricorsivamente i campi di tipo primitivo da un nodo JSON.
    /// Utilizza la factory <see cref="JsonProperty"/> per istanziare ogni campo.
    /// </summary>
    /// <param name="element">Elemento JSON corrispondente al blocco.</param>
    /// <param name="jsonEntity">Blocco (entità) a cui assegnare i campi.</param>
    /// <param name="results">Lista da popolare con i campi estratti.</param>
    private static void ExtractJsonPropertiesRecursive(JsonElement element, JsonEntity jsonEntity, List<Model.JsonProperties.JsonProperty> results)
    {
        // 🔒 Verifica che il blocco contenga proprietà
        if (element.ValueKind != JsonValueKind.Object)
            return;

        if (!element.TryGetProperty("properties", out JsonElement properties))
            return;

        // 🔁 Itera sulle proprietà del nodo per identificare i campi semplici
        foreach (var property in properties.EnumerateObject())
        {
            if (!property.Value.TryGetProperty("type", out JsonElement typeElement))
                continue;

            string type = typeElement.GetString()!;

            // 🚫 Esclude oggetti o array (gestiti altrove come blocchi)
            if (type is "object" or "array")
                continue;

            // 📝 Descrizione generata in automatico (fallback semantico)
            var description = property.Value.TryGetProperty("description", out var descElement)
                ? descElement.GetString() ?? $"Campo '{property.Name}'"
                : $"Campo '{property.Name}' di tipo '{type}'";

            // ✅ Campo valido
            var field = new Model.JsonProperties.JsonProperty(
                name: property.Name,
                parent: jsonEntity,
                isKey: false,
                description: description,
                validator: new JsonPropertyValidator()
            );

            results.Add(field); // ➕ Aggiunta alla lista
        }
    }
}