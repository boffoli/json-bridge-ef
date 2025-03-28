namespace JsonBridgeEF.Common.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione che segnala la mancanza o l'invalidità dello slug in un'entità.
    /// </para>
    /// <para><b>Constraints:</b>
    /// <list type="bullet">
    ///   <item>Lo slug deve essere non nullo e non vuoto.</item>
    /// </list>
    /// </para>
    /// </summary>
    public class InvalidSlugException : Exception
    {
        public InvalidSlugException()
            : base("❌ Lo slug non può essere nullo o vuoto.") { }

        public InvalidSlugException(string? slug)
            : base($"❌ Lo slug fornito non è valido: '{slug ?? "null"}'") { }
    }
}