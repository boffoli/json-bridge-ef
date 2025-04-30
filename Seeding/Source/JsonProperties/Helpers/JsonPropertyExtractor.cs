using System.Text.Json;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonEntities.Validators;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonProperty = JsonBridgeEF.Seeding.Source.JsonProperties.Model.JsonProperty;

namespace JsonBridgeEF.Seeding.Source.JsonProperties.Helpers;

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
    public static IReadOnlyList<IJsonProperty> ExtractJsonProperties(IJsonSchema schema, IJsonEntity jsonEntity)
    {
        using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
        return ExtractJsonProperties(jsonDoc.RootElement, jsonEntity);
    }

    public static IReadOnlyList<JsonProperty> ExtractJsonProperties(JsonElement root, IJsonEntity jsonEntity)
    {
        var jsonEntityElement = FindJsonEntities(root, jsonEntity.Name);
        if (!jsonEntityElement.HasValue)
            throw JsonEntityError.JsonBlockNotFound(jsonEntity.Name);

        var fields = new List<JsonProperty>();
        ExtractJsonPropertiesRecursive(jsonEntityElement.Value, jsonEntity, fields);
        return fields;
    }

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

    private static void ExtractJsonPropertiesRecursive(JsonElement element, IJsonEntity jsonEntity, List<JsonProperty> results)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return;

        if (!element.TryGetProperty("properties", out JsonElement properties))
            return;

        foreach (var property in properties.EnumerateObject())
        {
            if (!property.Value.TryGetProperty("type", out JsonElement typeElement))
                throw JsonEntityError.MissingTypeForProperty(property.Name, jsonEntity.Name);

            string type = typeElement.GetString()!;

            if (type is "object" or "array")
                continue;

            var description = property.Value.TryGetProperty("description", out var descElement)
                ? descElement.GetString() ?? $"Campo '{property.Name}'"
                : $"Campo '{property.Name}' di tipo '{type}'";

            var field = new JsonProperty(
                name: property.Name,
                parent: (IJsonEntity<JsonEntity, JsonProperty>)jsonEntity,
                isKey: false,
                description: description,
                validator: new JsonPropertyValidator()
            );

            results.Add(field);
        }
    }
}