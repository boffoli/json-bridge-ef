using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Common.Repositories;

namespace JsonBridgeEF.Seeding.SourceJson.Services;

/// <summary>
/// <b>Domain Concept:</b><br/>
/// Servizio per l'estrazione, composizione e promozione dei blocchi di uno schema JSON.
/// Responsabile della generazione e persistenza dei <see cref="JsonBlock"/> associati a un <see cref="JsonSchema"/>.
/// </summary>
internal sealed class JsonBlockSeeder : BaseDbService
{
    /// <summary>
    /// Blocchi generati dopo l’operazione di seeding.
    /// </summary>
    public IReadOnlyList<JsonBlock> SeededBlocks { get; private set; } = [];

    /// <summary>
    /// Inizializza il seeder.
    /// </summary>
    public JsonBlockSeeder(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    /// <summary>
    /// Estrae e salva i blocchi definiti nel contenuto JSON dello schema fornito.
    /// </summary>
    /// <param name="schema">Schema JSON di riferimento.</param>
    /// <returns>Lista dei blocchi generati e salvati.</returns>
    /// <exception cref="InvalidOperationException">Se lo schema ha già blocchi associati.</exception>
    internal async Task<IReadOnlyList<JsonBlock>> SeedAsync(JsonSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        schema.EnsureValid();
        EnsureTracked(schema);

        Console.WriteLine($"📂 Avvio seeding dei blocchi per schema: {schema.Name}");

        var repository = GetRepository<JsonBlock>();

        // 🛑 Previene duplicazione
        if (await HasExistingBlocksAsync(schema.Id, repository))
            throw new InvalidOperationException($"❌ Lo schema '{schema.Name}' ha già blocchi definiti.");

        // 🧠 Estrae blocchi dal contenuto JSON
        var extractedBlocks = JsonBlockExtractor.ExtractJsonBlocks(schema);

        if (extractedBlocks.Count > 0)
        {
            // 💾 Inserisce e salva i blocchi (cascade tramite navigazione)
            repository.AddRange(extractedBlocks);
            await SaveChangesAsync();

            Console.WriteLine($"✅ {extractedBlocks.Count} blocchi salvati.");
        }
        else
        {
            Console.WriteLine("ℹ️ Nessun blocco estratto dallo schema.");
        }

        SeededBlocks = extractedBlocks;
        return extractedBlocks;
    }

    /// <summary>
    /// Promuove un blocco generato a indipendente, assegnando il campo specificato come chiave logica.
    /// </summary>
    /// <param name="blockName">Nome del blocco da promuovere.</param>
    /// <param name="keyFieldName">Nome del campo da utilizzare come chiave.</param>
    /// <param name="schemaName">Nome dello schema a cui il blocco appartiene (usato in caso di errore).</param>
    internal async Task PromoteToIndependentAsync(string blockName, string keyFieldName, string schemaName)
    {
        var block = FindBlockByNameOrThrow(blockName, schemaName);

        try
        {
            block.MakeIndependent(keyFieldName);
            Console.WriteLine($"🔑 Blocco '{block.Name}' promosso a indipendente con chiave '{keyFieldName}'.");

            GetRepository<JsonBlock>().Update(block);
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new BlockPromotionException(blockName, keyFieldName, ex.Message);
        }
    }

    /// <summary>
    /// Verifica se esistono blocchi già associati allo schema.
    /// </summary>
    private static async Task<bool> HasExistingBlocksAsync(int schemaId, IRepository<JsonBlock> repository)
    {
        var existingBlocks = await repository.FindAsync(b => b.JsonSchemaId == schemaId);
        return existingBlocks.Count > 0;
    }

    /// <summary>
    /// Cerca un blocco tra quelli generati, oppure solleva eccezione se non trovato.
    /// </summary>
    /// <param name="blockName">Nome del blocco da cercare.</param>
    /// <param name="schemaName">Nome dello schema a cui il blocco appartiene (per messaggio errore).</param>
    private JsonBlock FindBlockByNameOrThrow(string blockName, string schemaName)
    {
        return SeededBlocks.FirstOrDefault(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase))
               ?? throw new BlockNotFoundException(blockName, schemaName);
    }
}