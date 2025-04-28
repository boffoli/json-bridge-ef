namespace JsonBridgeEF.Seeding.Source.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutte le anomalie relazionali tra blocchi JSON.</para>
    /// </summary>
    public abstract class JsonRelationshipException : JsonEntityException
    {
        protected JsonRelationshipException(string message) : base(message) { }
        protected JsonRelationshipException(string message, Exception? inner) : base(message, inner ?? new Exception()) { }
    }

    /// <summary>
    /// Eccezione per autoreferenza padre-figlio.
    /// </summary>
    internal sealed class JsonSelfParentException : JsonRelationshipException
    {
        public JsonSelfParentException(string blockName)
            : base($"❌ Il blocco '{blockName}' non può essere padre di sé stesso.") { }
    }

    /// <summary>
    /// Eccezione per riferimento circolare tra blocchi.
    /// </summary>
    internal sealed class JsonCircularReferenceException : JsonRelationshipException
    {
        public JsonCircularReferenceException(string source, string target)
            : base($"❌ Rilevato ciclo diretto tra '{source}' e '{target}'.") { }
    }

    /// <summary>
    /// Eccezione per relazione invertita padre-figlio.
    /// </summary>
    internal sealed class JsonInvertedParentingException : JsonRelationshipException
    {
        public JsonInvertedParentingException(string parent, string child)
            : base($"❌ Relazione invertita: '{parent}' non può essere figlio di '{child}'.") { }
    }

    /// <summary>
    /// Eccezione per relazione ridondante già definita.
    /// </summary>
    internal sealed class JsonRedundantRelationshipException : JsonRelationshipException
    {
        public JsonRedundantRelationshipException(string parent, string child)
            : base($"❌ La relazione padre-figlio '{parent} → {child}' è già stata definita.") { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per istanziare eccezioni relazionali tra blocchi JSON.</para>
    /// </summary>
    internal static class JsonRelationshipError
    {
        public static JsonRelationshipException SelfParent(string blockName) =>
            new JsonSelfParentException(blockName);

        public static JsonRelationshipException CircularReference(string source, string target) =>
            new JsonCircularReferenceException(source, target);

        public static JsonRelationshipException InvertedParenting(string parent, string child) =>
            new JsonInvertedParentingException(parent, child);

        public static JsonRelationshipException Redundant(string parent, string child) =>
            new JsonRedundantRelationshipException(parent, child);

        public static JsonRelationshipException WithInner(string message, Exception inner) =>
            new JsonRelationshipGenericException(message, inner);
    }

    internal sealed class JsonRelationshipGenericException : JsonRelationshipException
    {
        public JsonRelationshipGenericException(string message, Exception inner)
            : base(message, inner) { }
    }
}