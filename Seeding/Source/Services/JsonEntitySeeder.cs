using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;
using JsonBridgeEF.Seeding.Source.Helpers;
using JsonBridgeEF.Seeding.Source.Exceptions;

namespace JsonBridgeEF.Seeding.Source.Services;

/// <summary>
/// <b>Domain Concept:</b><br/>
/// Servizio per l'estrazione, composizione e promozione dei blocchi di uno schema JSON.
/// Responsabile della generazione e persistenza dei <see cref="JsonEntity"/> associati a un <see cref="JsonSchema"/>.
/// </summary>
/// <remarks>
/// <para><b>Usage Notes:</b> Questo servizio è usato nel processo di seeding iniziale per convertire uno schema in blocchi strutturati e persistenti.</para>
/// <para><b>Constraints:</b> È richiesto un <see cref="JsonSchema"/> valido e tracciato dal contesto corrente.</para>
/// </remarks>
internal sealed class JsonEntitySeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
{
    /// <summary>
    /// Blocchi generati dopo l’operazione di seeding.
    /// </summary>
    public IReadOnlyList<JsonEntity> SeededJsonEntities { get; private set; } = [];

    /// <summary>
    /// Estrae e salva i blocchi definiti nel contenuto JSON dello schema fornito.
    /// </summary>
    /// <param name="schema">Schema JSON di riferimento.</param>
    /// <returns>Lista dei blocchi generati e salvati.</returns>
    /// <exception cref="InvalidOperationException">Se lo schema ha già blocchi associati.</exception>
    /// <remarks>
    /// <para><b>Preconditions:</b> Lo schema deve essere valido e tracciato dal contesto EF.</para>
    /// <para><b>Postconditions:</b> I blocchi estratti vengono persistiti e assegnati a <see cref="SeededJsonEntities"/>.</para>
    /// <para><b>Side Effects:</b> Operazioni EF: aggiunta e salvataggio tramite repository.</para>
    /// </remarks>
    internal async Task<IReadOnlyList<JsonEntity>> SeedAsync(JsonSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        EnsureTracked(schema);

        Console.WriteLine($"📂 Avvio seeding dei blocchi per schema: {schema.Name}");

        var repository = GetRepository<JsonEntity>();

        // 🛑 Previene duplicazione
        if (await HasExistingJsonEntitiesAsync(schema.Id, repository))
            throw new InvalidOperationException($"❌ Lo schema '{schema.Name}' ha già blocchi definiti.");

        // 🧠 Estrae blocchi dal contenuto JSON
        var extractedJsonEntities = JsonEntityExtractor.ExtractJsonEntity(schema);

        if (extractedJsonEntities.Count > 0)
        {
            // 💾 Inserisce e salva i blocchi (cascade tramite navigazione)
            repository.AddRange(extractedJsonEntities);
            await SaveChangesAsync();

            Console.WriteLine($"✅ {extractedJsonEntities.Count} blocchi salvati.");
        }
        else
        {
            Console.WriteLine("ℹ️ Nessun blocco estratto dallo schema.");
        }

        SeededJsonEntities = extractedJsonEntities;
        return extractedJsonEntities;
    }

    /// <summary>
    /// Promuove un blocco generato a indipendente, assegnando il campo specificato come chiave logica.
    /// </summary>
    /// <param name="jsonEntityName">Nome del blocco da promuovere.</param>
    /// <param name="keyFieldName">Nome del campo da utilizzare come chiave.</param>
    /// <param name="schemaName">Nome dello schema a cui il blocco appartiene (usato in caso di errore).</param>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il blocco deve essere stato generato da <see cref="SeedAsync"/>.</para>
    /// <para><b>Postconditions:</b> Il blocco viene aggiornato e salvato come identificabile.</para>
    /// <para><b>Side Effects:</b> Persistenza tramite repository.</para>
    /// </remarks>
    internal async Task PromoteToIndependentAsync(string jsonEntityName, string keyFieldName, string schemaName)
    {
        var jsonEntity = FindJsonEntityByNameOrThrow(jsonEntityName, schemaName);

        try
        {
            jsonEntity.MakeIdentifiable(keyFieldName);
            Console.WriteLine($"🔑 Blocco '{jsonEntity.Name}' promosso a indipendente con chiave '{keyFieldName}'.");

            GetRepository<JsonEntity>().Update(jsonEntity);
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new JsonEntityPromotionException(jsonEntityName, keyFieldName, ex.Message);
        }
    }

    /// <summary>
    /// Verifica se esistono blocchi già associati allo schema.
    /// </summary>
    /// <param name="schemaId">ID dello schema da controllare.</param>
    /// <param name="repository">Repository per interrogare i blocchi.</param>
    /// <returns><c>true</c> se esistono blocchi già persistiti per quello schema; altrimenti <c>false</c>.</returns>
    private static async Task<bool> HasExistingJsonEntitiesAsync(int schemaId, IRepository<JsonEntity> repository)
    {
        var existingJsonEntities = await repository.FindAsync(b => b.Id == schemaId);
        return existingJsonEntities.Count > 0;
    }

    /// <summary>
    /// Cerca un blocco tra quelli generati, oppure solleva eccezione se non trovato.
    /// </summary>
    /// <param name="jsonEntityName">Nome del blocco da cercare.</param>
    /// <param name="schemaName">Nome dello schema a cui il blocco appartiene (per messaggio errore).</param>
    /// <returns>Il blocco trovato.</returns>
    /// <exception cref="JsonEntityNotFoundException">Se il blocco non è stato trovato tra quelli generati.</exception>
    private JsonEntity FindJsonEntityByNameOrThrow(string jsonEntityName, string schemaName)
    {
        return SeededJsonEntities.FirstOrDefault(b => b.Name.Equals(jsonEntityName, StringComparison.OrdinalIgnoreCase))
               ?? throw new JsonEntityNotFoundException(jsonEntityName, schemaName);
    }
}