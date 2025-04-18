namespace JsonBridgeEF.Seeding.Source.Exceptions;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Classe base astratta per tutte le eccezioni legate alla gestione dei blocchi JSON.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene utilizzata come base per eccezioni semantiche come blocchi mancanti o promozioni fallite.
/// </para>
/// </summary>
internal abstract class JsonEntitiesException(string message) : InvalidOperationException(message)
{
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando si tenta di aggiungere un blocco con nome già esistente all’interno dello schema.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene sollevata da <c>JsonSchemaJsonEntityCollection</c> quando si viola il vincolo di unicità del nome.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new JsonEntityAlreadyExistsException("Indirizzo");</c>
/// </para>
/// </summary>
internal class JsonEntityAlreadyExistsException(string jsonEntityName)
    : JsonEntitiesException($"❌ Esiste già un blocco con il nome '{jsonEntityName}'.")
{
    /// <summary>
    /// Nome del blocco duplicato.
    /// </summary>
    public string JsonEntityName { get; } = jsonEntityName;
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando un blocco specificato non è stato trovato all’interno dello schema JSON.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Sollevata tipicamente da metodi come <c>GetJsonEntity(string name)</c> su <see cref="JsonSchema"/>.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new JsonEntityNotFoundException("Utente", "Anagrafica");</c>
/// </para>
/// </summary>
internal class JsonEntityNotFoundException(string jsonEntityName, string schemaName)
    : JsonEntitiesException($"❌ Blocco '{jsonEntityName}' non trovato nello schema '{schemaName}'.")
{
    /// <summary>
    /// Nome del blocco mancante.
    /// </summary>
    public string JsonEntityName { get; } = jsonEntityName;

    /// <summary>
    /// Nome dello schema in cui è avvenuta la ricerca.
    /// </summary>
    public string SchemaName { get; } = schemaName;
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando fallisce il tentativo di promuovere un blocco JSON come indipendente.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene tipicamente generata da metodi come <c>MakeIndependent</c> su <see cref="JsonEntities"/>
/// quando le condizioni per la promozione non sono soddisfatte.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new JsonEntityPromotionException("Cliente", "CodiceFiscale", "Campo non valido");</c>
/// </para>
/// </summary>
internal class JsonEntityPromotionException(string jsonEntityName, string keyFieldName, string reason)
    : JsonEntitiesException($"❌ Impossibile promuovere il blocco '{jsonEntityName}' come indipendente con chiave '{keyFieldName}': {reason}")
{
    /// <summary>
    /// Nome del blocco coinvolto.
    /// </summary>
    public string JsonEntityName { get; } = jsonEntityName;

    /// <summary>
    /// Nome del campo proposto come chiave.
    /// </summary>
    public string KeyFieldName { get; } = keyFieldName;
}