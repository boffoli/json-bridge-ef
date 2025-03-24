using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.SourceJson.Helpers;

namespace JsonBridgeEF.Seeding.SourceJson.Services;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Servizio per il seeding degli schemi JSON nel database.
/// Opera come orchestratore della creazione e tracciamento degli oggetti <see cref="JsonSchema"/>.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Utilizza helper statici per validazione e generazione dello schema.
/// Il salvataggio avviene tramite <see cref="BaseDbService.SaveChangesAsync"/>.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// <list type="bullet">
///   <item>Il nome dello schema deve essere unico e valido.</item>
///   <item>Il contenuto JSON deve essere valido e non duplicato (a meno di forzatura).</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Questo servizio non salva direttamente i blocchi e i campi.
/// Assume che siano tracciati automaticamente partendo dallo schema.
/// Le operazioni di salvataggio persistono tutta la gerarchia tracciata.
/// </para>
internal sealed class JsonSchemaSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
{
    /// <summary>
    /// Genera e salva un JSON Schema partendo da un file JSON di esempio.
    /// </summary>
    /// <param name="schemaName">Nome dello schema da salvare.</param>
    /// <param name="sampleJsonFilePath">Percorso del file JSON di esempio.</param>
    /// <param name="forceSave">Se true, consente il salvataggio anche se lo stesso contenuto è già presente.</param>
    internal async Task<JsonSchema> SeedFromSampleJsonFileAsync(string schemaName, string sampleJsonFilePath, bool forceSave = false)
    {
        // 📄 Controllo esistenza e lettura file
        JsonSchemaHelper.EnsureFileExists(sampleJsonFilePath);
        string sampleJsonContent = await JsonSchemaHelper.ReadJsonFileAsync(sampleJsonFilePath);

        // ↪️ Delega al metodo che gestisce direttamente contenuto stringa
        return await SeedFromSampleJsonAsync(schemaName, sampleJsonContent, forceSave);
    }

    /// <summary>
    /// Genera e salva un JSON Schema partendo da una stringa JSON di esempio.
    /// </summary>
    /// <param name="schemaName">Nome dello schema da salvare.</param>
    /// <param name="sampleJsonContent">Contenuto JSON da cui generare lo schema.</param>
    /// <param name="forceSave">Se true, consente il salvataggio anche se lo stesso contenuto è già presente.</param>
    internal async Task<JsonSchema> SeedFromSampleJsonAsync(string schemaName, string sampleJsonContent, bool forceSave = false)
    {
        // 🧬 Genera lo schema da esempio
        string generatedSchema = JsonSchemaHelper.GenerateJsonSchemaFromSample(sampleJsonContent);

        // ↪️ Delega al metodo che salva uno schema completo in formato stringa
        return await SeedFromSchemaAsync(schemaName, generatedSchema, forceSave);
    }

    /// <summary>
    /// Salva direttamente un JSON Schema esistente letto da un file.
    /// </summary>
    /// <param name="schemaName">Nome dello schema da salvare.</param>
    /// <param name="jsonSchemaFilePath">Percorso del file JSON Schema.</param>
    /// <param name="forceSave">Se true, consente il salvataggio anche se lo stesso contenuto è già presente.</param>
    internal async Task<JsonSchema> SeedFromSchemaFileAsync(string schemaName, string jsonSchemaFilePath, bool forceSave = false)
    {
        // 📄 Controllo esistenza e lettura file
        JsonSchemaHelper.EnsureFileExists(jsonSchemaFilePath);
        string jsonSchemaContent = await JsonSchemaHelper.ReadJsonFileAsync(jsonSchemaFilePath);

        // ↪️ Delega al metodo che salva direttamente uno schema da stringa
        return await SeedFromSchemaAsync(schemaName, jsonSchemaContent, forceSave);
    }

    /// <summary>
    /// Salva direttamente un JSON Schema esistente, fornito come stringa.
    /// </summary>
    /// <param name="schemaName">Nome dello schema da salvare.</param>
    /// <param name="jsonSchemaContent">Contenuto JSON Schema.</param>
    /// <param name="forceSave">Se true, consente il salvataggio anche se lo stesso contenuto è già presente.</param>
    internal async Task<JsonSchema> SeedFromSchemaAsync(string schemaName, string jsonSchemaContent, bool forceSave = false)
    {
        var repo = GetRepository<JsonSchema>();

        // ✅ Validazione nome univoco
        await JsonSchemaHelper.ValidateSchemaNameAsync(schemaName, repo);

        // ✅ Validazione contenuto
        JsonSchemaHelper.EnsureJsonContentIsValid(jsonSchemaContent);
        await JsonSchemaHelper.ValidateSchemaContentAsync(jsonSchemaContent, repo, forceSave);

        // 🧱 Composizione dell'entità dominio
        var jsonSchema = JsonSchema.Create(schemaName, jsonSchemaContent);

        // 💾 Persistenza
        return await SaveSchemaAsync(jsonSchema);
    }

    /// <summary>
    /// Crea e salva un nuovo oggetto JsonSchema già validato.
    /// Questa operazione garantisce che l'intero grafo (schema, blocchi, campi) venga tracciato e persistito.
    /// </summary>
    /// <param name="jsonSchema">Oggetto schema da salvare.</param>
    private async Task<JsonSchema> SaveSchemaAsync(JsonSchema jsonSchema)
    {
        // 🏷 Tracciamento dello schema (e cascata su blocchi e campi)
        GetRepository<JsonSchema>().Add(jsonSchema);

        // 💾 Salvataggio atomico
        await SaveChangesAsync();

        Console.WriteLine($"📂 Schema JSON salvato: {jsonSchema.Name}");
        return jsonSchema;
    }
}