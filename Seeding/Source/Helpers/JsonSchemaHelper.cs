using System.Text.Json;
using JsonBridgeEF.Common.EfEntities.Exceptions;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Source.Exceptions;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Seeding.Source.Model.JsonSchemas;

namespace JsonBridgeEF.Seeding.Source.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Helper statico per la gestione e validazione di schemi JSON nell’ambito del modello concettuale.
/// Si occupa di coerenza tra contenuto, nome e relazioni tra schemi, oggetti e proprietà.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// Tutti i metodi sono privi di stato e destinati a essere utilizzati in seeding, import o validazioni manuali.</para>
///
/// <para><b>Constraints:</b><br/>
/// Richiede repository compatibili con <see cref="IRepository{JsonSchema}"/>.  
/// Gli oggetti coinvolti devono rispettare la semantica DDD e le relazioni di ownership.</para>
/// </summary>
internal static class JsonSchemaHelper
{
    #region Validazioni

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Verifica che il nome di uno schema sia valido e univoco nel contesto del repository.
    /// </para>
    /// <param name="schemaName">Nome proposto per lo schema.</param>
    /// <param name="repository">Repository su cui effettuare il controllo.</param>
    /// <exception cref="JsonSchemaError">Se il nome è nullo o vuoto.</exception>
    /// <exception cref="SchemaNameAlreadyExistsException">Se il nome è già presente.</exception>
    public static async Task EnsureSchemaNameIsValidAsync(string schemaName, IRepository<JsonSchema> repository)
    {
        if (string.IsNullOrWhiteSpace(schemaName))
            throw JsonSchemaError.InvalidName("Tipo dello schema");
            
        if (await repository.ExistsAsync(s => s.Name == schemaName))
            throw JsonSchemaError.NameAlreadyExists(schemaName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Verifica che il contenuto di uno schema JSON sia unico, evitando duplicazioni semantiche.
    /// </para>
    /// <param name="jsonSchemaContent">Contenuto JSON da validare.</param>
    /// <param name="repository">Repository per il controllo.</param>
    /// <param name="forceSave">Indica se ignorare il vincolo in caso di duplicato.</param>
    /// <exception cref="SchemaContentAlreadyExistsException">Se il contenuto è già presente e non è forzato il salvataggio.</exception>
    public static async Task EnsureSchemaContentIsUniqueAsync(string jsonSchemaContent, IRepository<JsonSchema> repository, bool forceSave)
    {
        var existing = await repository.FirstOrDefaultAsync(s => s.JsonSchemaContent == jsonSchemaContent);
        if (existing is not null && !forceSave)
            throw JsonSchemaError.ContentAlreadyExists(existing.Name);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Verifica che il file JSON fornito esista sul filesystem.
    /// </para>
    /// <param name="filePath">Percorso assoluto del file.</param>
    /// <exception cref="JsonSchemaError">Se il percorso è nullo o vuoto.</exception>
    /// <exception cref="JsonFileNotFoundException">Se il file non è trovato.</exception>
    public static void EnsureFileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw JsonSchemaError.InvalidFilePath();

        if (!File.Exists(filePath))
            throw JsonSchemaError.FileNotFound(filePath);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Verifica che un contenuto JSON sia ben formato secondo la sintassi.
    /// </para>
    /// <param name="jsonContent">Contenuto da validare.</param>
    /// <exception cref="InvalidJsonContentException">Se il contenuto è nullo o non valido.</exception>
    public static void EnsureJsonContentIsValid(string? jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
            throw JsonSchemaError.InvalidJsonContent("Il contenuto JSON è nullo o vuoto.");

        try
        {
            using var doc = JsonDocument.Parse(jsonContent);
        }
        catch (JsonException ex)
        {
            throw JsonSchemaError.InvalidJsonContent("Il contenuto JSON non è valido.", ex);
        }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Garantisce che un’entità JSON appartenga effettivamente allo schema previsto.
    /// </para>
    /// <param name="schema">Schema di riferimento.</param>
    /// <param name="jsonEntity">Entità da validare.</param>
    /// <exception cref="EntityOwnershipMismatchException">Se la relazione tra blocco e schema è incoerente.</exception>
    public static void EnsureJsonEntityBelongsToSchema(JsonSchema schema, JsonEntity jsonEntity)
    {
        if (!ReferenceEquals(jsonEntity.Schema, schema))
            throw new EntityOwnershipMismatchException(jsonEntity.Name, schema.Name);
    }

    #endregion

    #region Generazione e Lettura

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Genera uno schema JSON valido a partire da un esempio di JSON di input.
    /// </para>
    /// <param name="sampleJsonContent">Contenuto di esempio.</param>
    /// <returns>Lo schema JSON generato.</returns>
    /// <exception cref="InvalidJsonContentException">Se l’esempio è malformato.</exception>
    public static string GenerateSchemaFromSample(string sampleJsonContent)
    {
        EnsureJsonContentIsValid(sampleJsonContent);
        var schema = NJsonSchema.JsonSchema.FromSampleJson(sampleJsonContent);
        return schema.ToJson();
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Legge un file JSON da disco in modo asincrono e restituisce il contenuto.
    /// </para>
    /// <param name="filePath">Percorso del file.</param>
    /// <returns>Contenuto del file come stringa.</returns>
    /// <exception cref="JsonFileNotFoundException">Se il file non esiste.</exception>
    public static async Task<string> ReadJsonFileContentAsync(string filePath)
    {
        EnsureFileExists(filePath);
        return await File.ReadAllTextAsync(filePath);
    }

    #endregion
}