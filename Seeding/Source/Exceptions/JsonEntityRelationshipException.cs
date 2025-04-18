namespace JsonBridgeEF.Seeding.Source.Exceptions;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Eccezione specifica per errori nella definizione di relazioni tra blocchi JSON,
/// come cicli, autoreferenze o inversioni logiche padre-figlio.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Sollevata da <see cref="Support.RelationshipGuard"/> quando una relazione tra blocchi è logicamente invalida.
/// </para>
///
/// <para><b>Example:</b><br/>
/// <c>throw new JsonEntityRelationshipException("Un blocco non può essere padre di sé stesso.");</c>
/// </para>
/// </summary>
public sealed class JsonEntityRelationshipException : Exception
{
    /// <summary>
    /// Crea una nuova istanza con il messaggio di errore specificato.
    /// </summary>
    public JsonEntityRelationshipException(string message)
        : base(message) { }

    /// <summary>
    /// Crea una nuova istanza con un messaggio e un’eccezione interna.
    /// </summary>
    public JsonEntityRelationshipException(string message, Exception innerException)
        : base(message, innerException) { }
}