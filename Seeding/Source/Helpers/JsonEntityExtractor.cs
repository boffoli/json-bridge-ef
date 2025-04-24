using System.Text.Json;
using JsonBridgeEF.Seeding.Source.Exceptions;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Source.Validators;

namespace JsonBridgeEF.Seeding.Source.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Estrazione automatica dei blocchi JSON a partire da uno schema.
/// Ogni blocco può contenere campi e avere una relazione padre-figlio con altri blocchi.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// I blocchi vengono estratti dalla struttura "properties" del JSON schema,
/// istanziati tramite factory method e registrati nello schema.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Solo i nodi JSON di tipo "object" o "array" generano blocchi figli.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Il metodo <see cref="ExtractJsonEntity(JsonSchema)"/> è l’unica entrypoint pubblica.
/// La ricorsione avviene internamente, preservando relazioni e integrità.
/// </para>
internal static class JsonEntityExtractor
{
    /// <summary>
    /// Estrae tutti i blocchi JSON a partire da uno schema e ne costruisce l’albero padre-figlio.
    /// </summary>
    /// <param name="schema">Schema contenente il contenuto JSON da esplorare.</param>
    /// <returns>Lista piatta dei blocchi estratti, ciascuno già registrato nello schema.</returns>
    public static IReadOnlyList<JsonEntity> ExtractJsonEntity(JsonSchema schema)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
            var jsonEntity = new List<JsonEntity>();

            // 🔁 Avvia ricorsione dal nodo root
            ExtractJsonEntityRecursive(jsonDoc.RootElement, schema, parentJsonEntity: null, jsonEntity);

            return jsonEntity;
        }
        catch (JsonException ex)
        {
            throw JsonSchemaError.InvalidJsonContent("Errore durante il parsing del contenuto JSON dello schema.", ex);
        }
        catch (ArgumentNullException ex)
        {
            throw JsonSchemaError.InvalidJsonContent("Il contenuto dello schema JSON è nullo.", ex);
        }
    }

    private static void ExtractJsonEntityRecursive(JsonElement element, JsonSchema schema, JsonEntity? parentJsonEntity, List<JsonEntity> candidates)
    {
        if (element.ValueKind != JsonValueKind.Object)
            return;

        if (!element.TryGetProperty("properties", out JsonElement properties))
            return;

        foreach (var property in properties.EnumerateObject())
        {
            string jsonEntityName = property.Name;
            var entityDescription = $"Blocco JSON '{jsonEntityName}' estratto dallo schema.";

            var currentJsonEntity = new JsonEntity(
                name: jsonEntityName,
                schema: schema,
                description: entityDescription,
                validator: new JsonEntityValidator()
            );

            parentJsonEntity?.AddChild(currentJsonEntity);

            if (property.Value.TryGetProperty("properties", out JsonElement subProperties))
            {
                foreach (var subProperty in subProperties.EnumerateObject())
                {
                    var fieldDescription = subProperty.Value.TryGetProperty("description", out var descElement)
                        ? descElement.GetString() ?? $"Campo '{subProperty.Name}'"
                        : $"Campo '{subProperty.Name}' di tipo primitivo";

                    _ = new Model.JsonProperties.JsonProperty(
                        name: subProperty.Name,
                        parent: currentJsonEntity,
                        isKey: false,
                        description: fieldDescription,
                        validator: new JsonPropertyValidator()
                    );
                }
            }

            candidates.Add(currentJsonEntity);

            // 🔁 RICORSIONE SU OGGETTI O ARRAY
            if (property.Value.TryGetProperty("type", out JsonElement typeElement))
            {
                string type = typeElement.GetString()!;
                if (type == "object")
                {
                    ExtractJsonEntityRecursive(property.Value, schema, currentJsonEntity, candidates);
                }
                else if (type == "array" && property.Value.TryGetProperty("items", out JsonElement arrayItems))
                {
                    ExtractJsonEntityRecursive(arrayItems, schema, currentJsonEntity, candidates);
                }
                else
                {
                    throw JsonEntityError.InvalidTypeProperty(
                        entityName: currentJsonEntity.Name,
                        propertyName: property.Name,
                        type: type
                    );
                }
            }
        }
    }
}