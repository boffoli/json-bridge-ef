namespace JsonBridgeEF.Seeding.SourceJson.Exceptions;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione base per tutti gli errori legati alla gestione dei JSON Schema.
/// </para>
/// </summary>
public class JsonSchemaException : Exception
{
    public JsonSchemaException(string message) : base(message) { }

    public JsonSchemaException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione specifica che segnala la presenza di uno schema JSON con lo stesso nome nel database.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il nome dello schema deve essere univoco.</item>
/// </list>
/// </para>
/// </summary>
internal class SchemaNameAlreadyExistsException : JsonSchemaException
{
    public string SchemaName { get; }

    public SchemaNameAlreadyExistsException(string schemaName)
        : base($"❌ Lo schema '{schemaName}' esiste già nel database.")
    {
        SchemaName = schemaName;
    }
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione che segnala la presenza nel database di uno schema JSON con contenuto identico.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il contenuto JSON dello schema deve essere univoco rispetto agli schemi esistenti.</item>
/// </list>
/// </para>
/// </summary>
internal class SchemaContentAlreadyExistsException : JsonSchemaException
{
    public string ExistingSchemaName { get; }

    public SchemaContentAlreadyExistsException(string existingSchemaName)
        : base($"❌ Uno schema identico ('{existingSchemaName}') esiste già nel database.")
    {
        ExistingSchemaName = existingSchemaName;
    }
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione che indica che un file JSON specificato non è stato trovato nel percorso atteso.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il percorso del file deve essere valido e il file deve esistere fisicamente.</item>
/// </list>
/// </para>
/// </summary>
internal class JsonFileNotFoundException : JsonSchemaException
{
    public string FilePath { get; }

    public JsonFileNotFoundException(string filePath)
        : base($"❌ File JSON non trovato: {filePath}")
    {
        FilePath = filePath;
    }
}

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione che segnala che il contenuto JSON fornito è vuoto o non valido.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il contenuto JSON deve essere non nullo e ben formato.</item>
/// </list>
/// </para>
/// </summary>
internal class InvalidJsonContentException : JsonSchemaException
{
    public InvalidJsonContentException()
        : base("❌ Il contenuto JSON non può essere vuoto.") { }
}