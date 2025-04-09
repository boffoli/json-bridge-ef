using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.SourceJson.Helpers;

namespace JsonBridgeEF.Seeding.Source.Services;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Servizio per l’estrazione e la persistenza dei campi di un blocco JSON all’interno di uno schema.
/// I <see cref="JsonField"/> vengono generati a partire dalla struttura del JSON associato.
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// I campi vengono generati tramite <see cref="JsonFieldExtractor"/> e tracciati tramite EF Core.
/// Questo seeder non genera blocchi, ma opera sui blocchi già tracciati all'interno dello schema.
/// </para>
internal sealed class JsonFieldSeeder : BaseDbService
{
    /// <summary>
    /// Inizializza il seeder dei campi JSON con il contesto di unità di lavoro corrente.
    /// </summary>
    public JsonFieldSeeder(IUnitOfWork unitOfWork) : base(unitOfWork) { }

    /// <summary>
    /// Estrae e salva i campi di un blocco specifico appartenente a uno schema.
    /// </summary>
    /// <param name="schema">Schema a cui appartiene il blocco.</param>
    /// <param name="block">Blocco da cui generare i campi.</param>
    /// <returns>Lista dei campi generati e salvati.</returns>
    internal async Task<List<JsonField>> SeedAsync(JsonSchema schema, JsonBlock block)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(block);

        // ✅ Validazione schema
        schema.EnsureValid();

        // 🔒 Verifica integrità relazionale (block → schema)
        JsonSchemaHelper.EnsureBlockBelongsToSchema(schema, block);

        Console.WriteLine($"📂 Generazione campi per blocco '{block.Name}' nello schema '{schema.Name}'");

        // 🧠 Estrazione dei campi dal blocco
        JsonFieldExtractor.ExtractJsonFields(schema, block);

        // 💾 Validazione e tracciamento dei campi
        var repository = GetRepository<JsonField>();
        foreach (var field in block.Entities)
        {
            field.EnsureValid();
            repository.Add(field);
        }

        // 💾 Persistenza dei campi
        await SaveChangesAsync();
        return [.. block.Entities];
    }

    /// <summary>
    /// Estrae e salva i campi per una lista di blocchi appartenenti a uno schema.
    /// </summary>
    /// <param name="schema">Schema di riferimento.</param>
    /// <param name="blocks">Lista di blocchi da processare.</param>
    /// <returns>Lista completa dei campi generati per tutti i blocchi.</returns>
    internal async Task<List<JsonField>> SeedAsync(JsonSchema schema, List<JsonBlock> blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        var allFields = new List<JsonField>();

        // 🔁 Estrazione campi per ciascun blocco
        foreach (var block in blocks)
        {
            var fields = await SeedAsync(schema, block);
            allFields.AddRange(fields);
        }

        return allFields;
    }
}