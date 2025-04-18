using System.Text.Json;
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
        using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
        var jsonEntity = new List<JsonEntity>();

        // 🔁 Avvia ricorsione dal nodo root
        ExtractJsonEntityRecursive(jsonDoc.RootElement, schema, parentJsonEntity: null, jsonEntity);

        return jsonEntity;
    }

    /// <summary>
    /// Estrae i blocchi JSON da un elemento e li collega ricorsivamente in modo padre-figlio.
    /// Usa factory method per garantire integrità e consistenza del dominio.
    /// </summary>
    /// <param name="element">Elemento JSON di partenza.</param>
    /// <param name="schema">Schema a cui i blocchi appartengono.</param>
    /// <param name="parentJsonEntity">Blocco padre (null se radice).</param>
    /// <param name="candidates">Lista cumulativa dei blocchi estratti.</param>
    private static void ExtractJsonEntityRecursive(JsonElement element, JsonSchema schema, JsonEntity? parentJsonEntity, List<JsonEntity> candidates)
    {
        // 🔒 Verifica che l'elemento sia un oggetto JSON valido
        if (element.ValueKind != JsonValueKind.Object)
            return;

        // 🔍 Cerca la sezione "properties" che definisce i blocchi figli
        if (!element.TryGetProperty("properties", out JsonElement properties))
            return;

        foreach (var property in properties.EnumerateObject())
        {
            string jsonEntityName = property.Name;

            // 📝 Descrizione automatica del blocco
            var entityDescription = $"Blocco JSON '{jsonEntityName}' estratto dallo schema.";

            // 🧱 CREAZIONE BLOCCO (schema + nome + descrizione)
            var currentJsonEntity = new JsonEntity(
                name: jsonEntityName,
                schema: schema,
                description: entityDescription,
                validator: new JsonEntityValidator()
            );

            // 🔁 RELAZIONE PADRE-FIGLIO (se applicabile)
            parentJsonEntity?.AddChild(currentJsonEntity);

            // 🧩 ESTRAZIONE CAMPI SE IL BLOCCO CONTIENE PROPRIETÀ
            if (property.Value.TryGetProperty("properties", out JsonElement subProperties))
            {
                foreach (var subProperty in subProperties.EnumerateObject())
                {
                    // 📝 Descrizione automatica o da attributo "description"
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

            // ➕ Aggiunta del blocco alla lista dei risultati
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
            }
        }
    }
}