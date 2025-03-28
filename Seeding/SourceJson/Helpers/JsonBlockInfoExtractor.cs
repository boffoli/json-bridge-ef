using System.Text.Json;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.SourceJson.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers;

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
///   <item>I blocchi devono essere registrati nello schema tramite <see cref="JsonBlock.Create"/>.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Il metodo <see cref="ExtractJsonBlocks(JsonSchema)"/> è l’unica entrypoint pubblica.
/// La ricorsione avviene internamente, preservando relazioni e integrità.
/// </para>
internal static class JsonBlockExtractor
{
    /// <summary>
    /// Estrae tutti i blocchi JSON a partire da uno schema e ne costruisce l’albero padre-figlio.
    /// </summary>
    /// <param name="schema">Schema contenente il contenuto JSON da esplorare.</param>
    /// <returns>Lista piatta dei blocchi estratti, ciascuno già registrato nello schema.</returns>
    public static IReadOnlyList<JsonBlock> ExtractJsonBlocks(JsonSchema schema)
    {
        using var jsonDoc = JsonDocument.Parse(schema.JsonSchemaContent);
        var blocks = new List<JsonBlock>();

        // 🔁 Avvia ricorsione dal nodo root
        ExtractBlocksRecursive(jsonDoc.RootElement, schema, parentBlock: null, blocks);

        return blocks;
    }

    /// <summary>
    /// Estrae i blocchi JSON da un elemento e li collega ricorsivamente in modo padre-figlio.
    /// Usa factory method per garantire integrità e consistenza del dominio.
    /// </summary>
    /// <param name="element">Elemento JSON di partenza.</param>
    /// <param name="schema">Schema a cui i blocchi appartengono.</param>
    /// <param name="parentBlock">Blocco padre (null se radice).</param>
    /// <param name="candidates">Lista cumulativa dei blocchi estratti.</param>
    private static void ExtractBlocksRecursive(JsonElement element, JsonSchema schema, JsonBlock? parentBlock, List<JsonBlock> candidates)
    {
        // 🔒 Verifica che l'elemento sia un oggetto JSON valido
        if (element.ValueKind != JsonValueKind.Object)
            return;

        // 🔍 Cerca la sezione "properties" che definisce i blocchi figli
        if (!element.TryGetProperty("properties", out JsonElement properties))
            return;

        foreach (var property in properties.EnumerateObject())
        {
            string blockName = property.Name;

            // 🧱 CREAZIONE BLOCCO (schema + nome)
            var currentBlock = new JsonBlock(blockName, schema, validator: new JsonBlockValidator());

            // 🔁 RELAZIONE PADRE-FIGLIO (se applicabile)
            parentBlock?.AddChild(currentBlock);

            // 🧩 ESTRAZIONE CAMPI SE IL BLOCCO CONTIENE PROPRIETÀ
            if (property.Value.TryGetProperty("properties", out JsonElement subProperties))
            {
                foreach (var subProperty in subProperties.EnumerateObject())
                {
                    _ = new JsonField(
                        subProperty.Name,
                        currentBlock,
                        validator: new JsonFieldValidator()
                    );
                }
            }

            // ➕ Aggiunta del blocco alla lista dei risultati
            candidates.Add(currentBlock);

            // 🔁 RICORSIONE SU OGGETTI O ARRAY
            if (property.Value.TryGetProperty("type", out JsonElement typeElement))
            {
                string type = typeElement.GetString()!;
                if (type == "object")
                {
                    ExtractBlocksRecursive(property.Value, schema, currentBlock, candidates);
                }
                else if (type == "array" && property.Value.TryGetProperty("items", out JsonElement arrayItems))
                {
                    ExtractBlocksRecursive(arrayItems, schema, currentBlock, candidates);
                }
            }
        }
    }
}