using System.Text.Json;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.SourceJson.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Helper per l'estrazione e la generazione automatica dei <see cref="JsonField"/> da uno schema JSON di esempio.
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
internal static class JsonFieldExtractor
{
    // 🔹 ENTRY POINT 🔹

    /// <summary>
    /// Estrae i campi JSON associati a un blocco specifico, cercandoli nel contenuto di uno <see cref="JsonSchema"/>.
    /// </summary>
    public static IReadOnlyList<JsonField> ExtractJsonFields(JsonSchema schema, JsonBlock block)
    {
        using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
        return ExtractJsonFields(jsonDoc.RootElement, block);
    }

    /// <summary>
    /// Estrae i campi JSON associati a un blocco da un <see cref="JsonElement"/>.
    /// </summary>
    public static IReadOnlyList<JsonField> ExtractJsonFields(JsonElement root, JsonBlock block)
    {
        var blockElement = FindJsonBlock(root, block.Name);
        if (!blockElement.HasValue)
            return [];

        var fields = new List<JsonField>();
        ExtractJsonFieldsRecursive(blockElement.Value, block, fields);
        return fields;
    }

    // 🔹 RICERCA BLOCCO 🔹

    /// <summary>
    /// Naviga nel documento JSON per raggiungere il nodo associato al blocco, usando il suo nome come path.
    /// </summary>
    private static JsonElement? FindJsonBlock(JsonElement root, string blockPath)
    {
        var pathSegments = blockPath.Split('.');
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

    // 🔹 ESTRATTORE CAMPI 🔹

    /// <summary>
    /// Estrae ricorsivamente i campi JSON da un nodo <paramref name="element"/> e li registra nel blocco.
    /// Utilizza la factory <see cref="JsonField.Create"/> per garantire l’integrità.
    /// </summary>
    private static void ExtractJsonFieldsRecursive(JsonElement element, JsonBlock block, List<JsonField> results)
    {
        // 🔒 Verifica che il blocco contenga proprietà
        if (element.ValueKind != JsonValueKind.Object) return;

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

            // ✅ Campo valido: creazione tramite factory e registrazione
            var field = new JsonField(
                property.Name,
                block,
                validator: new JsonFieldValidator()
            );

            results.Add(field); // ➕ Aggiunta alla lista (il blocco è già aggiornato internamente)
        }
    }
}