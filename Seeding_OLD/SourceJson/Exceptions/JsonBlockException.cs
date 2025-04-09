namespace JsonBridgeEF.Seeding.SourceJson.Exceptions;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Classe base astratta per tutte le eccezioni legate alla gestione dei blocchi JSON.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene utilizzata come base per eccezioni semantiche come blocchi mancanti o promozioni fallite.
/// </para>
/// </summary>
internal abstract class JsonBlockException(string message) : InvalidOperationException(message)
{
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando si tenta di aggiungere un blocco con nome già esistente all’interno dello schema.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene sollevata da <c>JsonSchemaBlockCollection</c> quando si viola il vincolo di unicità del nome.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new BlockAlreadyExistsException("Indirizzo");</c>
/// </para>
/// </summary>
internal class BlockAlreadyExistsException(string blockName)
    : JsonBlockException($"❌ Esiste già un blocco con il nome '{blockName}'.")
{
    /// <summary>
    /// Nome del blocco duplicato.
    /// </summary>
    public string BlockName { get; } = blockName;
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando un blocco specificato non è stato trovato all’interno dello schema JSON.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Sollevata tipicamente da metodi come <c>GetBlock(string name)</c> su <see cref="JsonSchema"/>.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new BlockNotFoundException("Utente", "Anagrafica");</c>
/// </para>
/// </summary>
internal class BlockNotFoundException(string blockName, string schemaName)
    : JsonBlockException($"❌ Blocco '{blockName}' non trovato nello schema '{schemaName}'.")
{
    /// <summary>
    /// Nome del blocco mancante.
    /// </summary>
    public string BlockName { get; } = blockName;

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
/// Viene tipicamente generata da metodi come <c>MakeIndependent</c> su <see cref="JsonBlock"/>
/// quando le condizioni per la promozione non sono soddisfatte.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new BlockPromotionException("Cliente", "CodiceFiscale", "Campo non valido");</c>
/// </para>
/// </summary>
internal class BlockPromotionException(string blockName, string keyFieldName, string reason)
    : JsonBlockException($"❌ Impossibile promuovere il blocco '{blockName}' come indipendente con chiave '{keyFieldName}': {reason}")
{
    /// <summary>
    /// Nome del blocco coinvolto.
    /// </summary>
    public string BlockName { get; } = blockName;

    /// <summary>
    /// Nome del campo proposto come chiave.
    /// </summary>
    public string KeyFieldName { get; } = keyFieldName;
}