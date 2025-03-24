using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Services;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Client;

/// <summary>
/// Servizio che gestisce il seeding dell'intero database.
/// </summary>
internal class SeedingPipelineService(IUnitOfWork unitOfWork)
{
    private readonly JsonSchemaSeeder _jsonSchemaSeeder = new(unitOfWork);
    private readonly JsonBlockSeeder _jsonBlockSeeder = new(unitOfWork);
    private readonly JsonFieldSeeder _jsonFieldSeeder = new(unitOfWork);
    private JsonSchema? _jsonSchema;

    /// <summary>
    /// Avvia il seeding completo del database: schema, blocchi, promozione, campi.
    /// </summary>
    internal async Task RunSeedingAsync()
    {
        Console.WriteLine("\n🚀 **Avvio del seeding del database...**\n");

        try
        {
            _jsonSchema = await SeedJsonSchemaAsync();

            var jsonBlocks = await SeedJsonBlocksAsync(_jsonSchema);
            await HandleBlockPromotionAsync(_jsonSchema);
            await SeedJsonFieldsAsync(_jsonSchema, jsonBlocks);

            Console.WriteLine("\n✅ **Seeding completato con successo.**\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ **Seeding interrotto a causa di un errore: {ex.Message}**\n");
        }
    }

    /// <summary>
    /// Esegue il seeding dello schema JSON tramite input utente.
    /// </summary>
    private async Task<JsonSchema> SeedJsonSchemaAsync()
    {
        Console.WriteLine("\n📌 **Avvio del seeding dello schema JSON...**");

        string schemaName = PromptUser("🔹 Inserisci il nome dello schema JSON: ");
        string sampleJsonFilePath = PromptUser("🔹 Inserisci il percorso del file JSON: ", mustExist: true);
        bool forceSave = false;

        while (true)
        {
            try
            {
                var schema = await _jsonSchemaSeeder.SeedFromSampleJsonFileAsync(schemaName, sampleJsonFilePath, forceSave);
                Console.WriteLine($"✅ **Schema JSON '{schema.Name}' salvato con successo.**\n");
                return schema;
            }
            catch (SchemaNameAlreadyExistsException ex)
            {
                Console.Write($"⚠️ {ex.Message} Vuoi sovrascriverlo? (y/n): ");
                if (UserWantsToContinue()) forceSave = true;
                else throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ **Errore nel seeding dello schema JSON: {ex.Message}**");
                throw;
            }
        }
    }

    /// <summary>
    /// Esegue il seeding dei blocchi JSON associati allo schema.
    /// </summary>
    private async Task<List<JsonBlock>> SeedJsonBlocksAsync(JsonSchema schema)
    {
        Console.WriteLine("\n📌 **Avvio del seeding dei blocchi JSON...**");
        var blocks = await _jsonBlockSeeder.SeedAsync(schema);
        Console.WriteLine($"✅ **{blocks.Count} blocchi JSON salvati.**\n");
        return blocks.ToList();
    }

    /// <summary>
    /// Gestisce interattivamente la promozione opzionale dei blocchi a indipendenti.
    /// </summary>
    private async Task HandleBlockPromotionAsync(JsonSchema schema)
    {
        Console.WriteLine("🔧 **Configurazione blocchi indipendenti**");

        foreach (var block in _jsonBlockSeeder.SeededBlocks)
        {
            Console.WriteLine($"\n🔹 Blocco: {block.Name}");

            var fields = block.JsonFields.ToList();
            var currentKey = fields.FirstOrDefault(f => f.IsKey);

            Console.WriteLine("   📄 Campi del blocco:");
            for (int i = 0; i < fields.Count; i++)
            {
                string marker = fields[i].IsKey ? "🗝️" : "  ";
                Console.WriteLine($"   {i + 1}. {marker} {fields[i].Name}");
            }

            if (currentKey != null)
            {
                Console.WriteLine($"   ℹ️ Attualmente il campo '{currentKey.Name}' è marcato come chiave.");
                Console.WriteLine("   ⚠️ La promozione potrebbe sovrascrivere la chiave esistente.");
            }

            Console.Write("   ➡️ Vuoi promuovere questo blocco a indipendente? (y/n): ");
            if (!UserWantsToContinue())
                continue;

            Console.Write("   🔑 Inserisci il numero del campo da usare come chiave: ");
            if (!int.TryParse(Console.ReadLine(), out int fieldIndex) || fieldIndex < 1 || fieldIndex > fields.Count)
            {
                Console.WriteLine("   ❌ Input non valido. Blocco non promosso.");
                continue;
            }

            string selectedFieldName = fields[fieldIndex - 1].Name;

            try
            {
                await _jsonBlockSeeder.PromoteToIndependentAsync(block.Name, selectedFieldName, schema.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Errore nella promozione: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Esegue il seeding dei campi per ciascun blocco.
    /// </summary>
    private async Task SeedJsonFieldsAsync(JsonSchema schema, List<JsonBlock> blocks)
    {
        Console.WriteLine("\n📌 **Avvio del seeding dei campi JSON...**");
        foreach (var block in blocks)
        {
            await _jsonFieldSeeder.SeedAsync(schema, block);
        }
        Console.WriteLine("✅ **Seeding dei campi JSON completato.**\n");
    }

    /// <summary>
    /// Chiede all'utente se desidera proseguire, con input 'y' o 'n'.
    /// </summary>
    private static bool UserWantsToContinue()
    {
        while (true)
        {
            Console.Write("➡️ Inserisci 'y' per confermare o 'n' per annullare: ");
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input == "y") return true;
            if (input == "n") return false;
            Console.WriteLine("⚠️ Input non valido. Inserisci 'y' o 'n'.");
        }
    }

    /// <summary>
    /// Richiede input da console con messaggio personalizzato e validazione opzionale del file.
    /// </summary>
    private static string PromptUser(string message, bool mustExist = false)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("⚠️ **L'input non può essere vuoto.** Riprova.");
                continue;
            }

            if (mustExist && !File.Exists(input))
            {
                Console.WriteLine($"❌ **File non trovato:** '{input}'. Inserisci un percorso valido.");
                continue;
            }

            return input;
        }
    }
}