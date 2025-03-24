using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Helper statico dedicato alla validazione e gestione dei JSON Schema.
/// Contiene metodi per verificare nomi, contenuti, esistenza di file e generazione dinamica.
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Tutti i metodi sono statici e privi di stato.  
/// Utilizzare prima <c>EnsureFileExists</c> e <c>EnsureJsonContentIsValid</c> prima di creare o registrare uno schema.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// Richiede un repository compatibile con <see cref="IRepository{JsonSchema}"/> per eseguire le validazioni.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Non instanziabile. Usato come helper statico all’interno dei processi di seeding o validazione.
/// </para>
internal static class JsonSchemaHelper
{
    /// <summary>
    /// Verifica che il nome fornito per uno schema sia valido e univoco.
    /// </summary>
    /// <param name="schemaName">Nome proposto per il nuovo schema.</param>
    /// <param name="repository">Repository per verificare la presenza nel database.</param>
    /// <exception cref="ArgumentException">Se il nome è nullo o vuoto.</exception>
    /// <exception cref="SchemaNameAlreadyExistsException">Se il nome è già presente nel database.</exception>
    public static async Task ValidateSchemaNameAsync(string schemaName, IRepository<JsonSchema> repository)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw new ArgumentException("Il nome dello schema non può essere vuoto.", nameof(schemaName));

        if (await repository.ExistsAsync(s => s.Name == schemaName))
            throw new SchemaNameAlreadyExistsException(schemaName);
    }

    /// <summary>
    /// Verifica che il contenuto JSON non sia già presente in un altro schema.
    /// </summary>
    /// <param name="jsonSchemaContent">Contenuto dello schema da confrontare.</param>
    /// <param name="repository">Repository per accedere agli schemi esistenti.</param>
    /// <param name="forceSave">Se true, bypassa la validazione.</param>
    /// <exception cref="SchemaContentAlreadyExistsException">Se lo schema esiste e <c>forceSave</c> è false.</exception>
    public static async Task ValidateSchemaContentAsync(string jsonSchemaContent, IRepository<JsonSchema> repository, bool forceSave)
    {
        var existingSchema = await repository.FirstOrDefaultAsync(s => s.JsonSchemaContent == jsonSchemaContent);
        if (existingSchema != null && !forceSave)
            throw new SchemaContentAlreadyExistsException(existingSchema.Name);
    }

    /// <summary>
    /// Verifica l’esistenza fisica del file JSON specificato.
    /// </summary>
    /// <param name="filePath">Percorso completo del file da controllare.</param>
    /// <exception cref="ArgumentException">Se il percorso è nullo o vuoto.</exception>
    /// <exception cref="JsonFileNotFoundException">Se il file non esiste sul disco.</exception>
    public static void EnsureFileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("❌ Percorso file JSON non valido o inesistente.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new JsonFileNotFoundException(filePath);
    }

    /// <summary>
    /// Verifica che il contenuto JSON fornito sia sintatticamente valido.
    /// </summary>
    /// <param name="jsonContent">Contenuto JSON da validare.</param>
    /// <exception cref="InvalidJsonContentException">Se il contenuto è nullo o vuoto.</exception>
    public static void EnsureJsonContentIsValid(string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new InvalidJsonContentException();
    }

    /// <summary>
    /// Genera uno schema JSON a partire da un esempio di JSON.
    /// </summary>
    /// <param name="sampleJsonContent">Contenuto JSON di esempio.</param>
    /// <returns>Schema JSON generato in formato stringa.</returns>
    /// <exception cref="InvalidJsonContentException">Se il contenuto è vuoto o non valido.</exception>
    public static string GenerateJsonSchemaFromSample(string sampleJsonContent)
    {
        EnsureJsonContentIsValid(sampleJsonContent);
        var schema = NJsonSchema.JsonSchema.FromSampleJson(sampleJsonContent);
        return schema.ToJson();
    }

    /// <summary>
    /// Carica il contenuto JSON da un file su disco.
    /// </summary>
    /// <param name="filePath">Percorso assoluto del file JSON.</param>
    /// <returns>Contenuto completo del file come stringa.</returns>
    /// <exception cref="JsonFileNotFoundException">Se il file non esiste.</exception>
    public static async Task<string> ReadJsonFileAsync(string filePath)
    {
        EnsureFileExists(filePath);
        return await File.ReadAllTextAsync(filePath);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Verifica che il blocco specificato sia effettivamente collegato allo schema fornito.<br/>
    /// Utilizza il riferimento oggetto e non gli identificatori numerici.
    /// </para>
    ///
    /// <para><b>Usage:</b><br/>
    /// Può essere utilizzato nei seeder o nei validatori per assicurare coerenza tra blocco e schema.
    /// </para>
    ///
    /// <param name="schema">Istanza dello schema JSON atteso.</param>
    /// <param name="block">Istanza del blocco da verificare.</param>
    /// <exception cref="InvalidOperationException">Se <paramref name="block"/> non è associato a <paramref name="schema"/>.</exception>
    public static void EnsureBlockBelongsToSchema(JsonSchema schema, JsonBlock block)
    {
        if (!ReferenceEquals(block.JsonSchema, schema))
        {
            throw new InvalidOperationException(
                $"❌ Il blocco '{block.Name}' non è associato allo schema '{schema.Name}'.");
        }
    }
}