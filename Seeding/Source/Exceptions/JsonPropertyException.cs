namespace JsonBridgeEF.Seeding.Source.Exceptions;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione base astratta per tutte le anomalie relative ai campi JSON all’interno dei blocchi.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Viene estesa per gestire casi specifici come duplicazione, chiave già presente o campo non trovato.
/// </para>
/// </summary>
internal abstract class JsonPropertyException(string fieldName, string? jsonEntityName, string message)
    : InvalidOperationException(message)
{
    /// <summary>
    /// Nome del campo coinvolto nell'errore.
    /// </summary>
    public string FieldName { get; } = fieldName;

    /// <summary>
    /// Nome del blocco a cui il campo appartiene (se noto).
    /// </summary>
    public string? JsonEntityName { get; } = jsonEntityName;
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando un campo con lo stesso nome è già presente all’interno del blocco.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Tipicamente sollevata da <c>TryAddField</c> o <c>AddKey</c> durante la creazione dei campi.
/// </para>
/// </summary>
internal class JsonFieldAlreadyExistsException(string fieldName, string? jsonEntityName)
    : JsonPropertyException(fieldName, jsonEntityName,
        $"❌ Il campo '{fieldName}' non può essere aggiunto perché esiste già nel blocco '{jsonEntityName ?? "?"}'.")
{ }

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando un campo chiave è già presente e non è stato richiesto il sovrascrittibile.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Sollevata da <c>AddKey</c> quando si tenta di impostare un nuovo campo come chiave senza forzare la sostituzione.
/// </para>
/// </summary>
internal class JsonFieldKeyAlreadyPresentException(string fieldName, string? jsonEntityName)
    : JsonPropertyException(fieldName, jsonEntityName,
        $"❌ Esiste già un campo chiave diverso nel blocco '{jsonEntityName ?? "?"}': '{fieldName}'.")
{ }

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione sollevata quando si tenta di impostare come chiave un campo non presente nel blocco.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Sollevata da <c>SetKey</c> quando il campo specificato non è stato trovato nella collezione del blocco.
/// </para>
/// </summary>
internal class JsonFieldNotFoundException(string fieldName, string? jsonEntityName)
    : JsonPropertyException(fieldName, jsonEntityName,
        $"❌ Il campo '{fieldName}' non è stato trovato nel blocco '{jsonEntityName ?? "?"}'.")
{ }