using System.Text.Json;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonEntities.Validators;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonProperty = JsonBridgeEF.Seeding.Source.JsonProperties.Model.JsonProperty;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Helpers
{
    /// <summary>
    /// Domain Concept: Componente helper per l’estrazione ricorsiva dei blocchi JSON da uno schema.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Classe statica con un entry point pubblico `Extract`.</para>
    /// <para>Constraints: Richiede che lo schema contenga un contenuto JSON valido e conforme a NJsonSchema.</para>
    /// <para>Relationships: Produce <see cref="JsonEntity"/> e <see cref="JsonProperty"/> a partire da uno <see cref="IJsonSchema{JsonEntity}"/> concreto.</para>
    /// <para>Usage Notes: Utilizzato internamente nel processo di seeding per generare blocchi strutturati da uno schema JSON.</para>
    /// </remarks>
    internal static class JsonEntityExtractor
    {
        /// <summary>
        /// Estrae tutte le entità (`JsonEntity`) da uno schema concreto.
        /// </summary>
        /// <param name="schema">Lo schema concreto da cui estrarre.</param>
        /// <param name="entityValidator">Validator opzionale per le entità; se nullo usa <see cref="JsonEntityValidator"/>.</param>
        /// <param name="propertyValidator">Validator opzionale per le proprietà; se nullo usa <see cref="JsonPropertyValidator"/>.</param>
        /// <returns>Lista delle entità JSON generate e collegate allo schema.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il contenuto JSON dello schema deve essere valido e rappresentare un oggetto con proprietà.</para>
        /// <para><b>Postconditions:</b> Ogni entità creata viene collegata allo schema e validata.</para>
        /// <para><b>Side Effects:</b> Vengono costruiti oggetti dominio (`JsonEntity` e `JsonProperty`) e popolata la gerarchia padre-figlio.</para>
        /// </remarks>
        public static IReadOnlyList<JsonEntity> Extract(
            IJsonSchema<JsonEntity> schema,
            IValidateAndFix<JsonEntity>? entityValidator   = null,
            IValidateAndFix<JsonProperty>? propertyValidator = null)
        {
            entityValidator   ??= new JsonEntityValidator();
            propertyValidator ??= new JsonPropertyValidator();

            try
            {
                using var doc = JsonDocument.Parse(schema.JsonSchemaContent);
                var output = new List<JsonEntity>();
                Recurse(doc.RootElement, schema, parent: null, output, entityValidator, propertyValidator);
                return output;
            }
            catch (JsonException ex)
            {
                throw JsonSchemaError.InvalidJsonContent("Errore nel parsing del JSON dello schema.", ex);
            }
        }

        /// <summary>
        /// Metodo ricorsivo interno che costruisce blocchi e campi a partire da un nodo JSON.
        /// </summary>
        /// <param name="element">Elemento JSON corrente.</param>
        /// <param name="schema">Schema di riferimento.</param>
        /// <param name="parent">Blocco padre (se presente).</param>
        /// <param name="output">Lista aggregata di blocchi risultanti.</param>
        /// <param name="entityValidator">Validatore per i blocchi.</param>
        /// <param name="propertyValidator">Validatore per i campi.</param>
        private static void Recurse(
            JsonElement element,
            IJsonSchema<JsonEntity> schema,
            JsonEntity? parent,
            List<JsonEntity> output,
            IValidateAndFix<JsonEntity> entityValidator,
            IValidateAndFix<JsonProperty> propertyValidator)
        {
            if (element.ValueKind != JsonValueKind.Object) return;
            if (!element.TryGetProperty("properties", out var props)) return;

            foreach (var prop in props.EnumerateObject())
            {
                // Creo la JsonEntity concreta (si auto-registra nello schema)
                var ent = new JsonEntity(
                    name: prop.Name,
                    schema: schema,
                    description: $"Blocco JSON '{prop.Name}'",
                    validator: entityValidator
                );
                parent?.AddChild(ent);
                output.Add(ent);

                // Creo tutte le JsonProperty figlie
                if (prop.Value.TryGetProperty("properties", out var subProps))
                {
                    foreach (var sp in subProps.EnumerateObject())
                    {
                        _ = new JsonProperty(
                            name: sp.Name,
                            parent: ent,
                            isKey: false,
                            description: sp.Value.TryGetProperty("description", out var d)
                                ? d.GetString()!
                                : $"Campo '{sp.Name}'",
                            validator: propertyValidator
                        );
                    }
                }

                // Ricorsione su object/array
                if (prop.Value.TryGetProperty("type", out var typeEl))
                {
                    var t = typeEl.GetString();
                    if (t == "object")
                        Recurse(prop.Value, schema, ent, output, entityValidator, propertyValidator);
                    else if (t == "array" && prop.Value.TryGetProperty("items", out var items))
                        Recurse(items, schema, ent, output, entityValidator, propertyValidator);
                    else
                        throw JsonEntityError.InvalidTypeProperty(ent.Name, prop.Name, t ?? "unknown");
                }
            }
        }
    }
}