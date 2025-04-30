using JsonBridgeEF.Shared.EntityModel.Exceptions;

namespace JsonBridgeEF.Seeding.Target.ClassInfos.Exceptions
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione delle classi nel modello target.</para>
    /// </summary>
    public abstract class ClassInfoException : EntityException
    {
        protected ClassInfoException(string message) : base(message) { }

        protected ClassInfoException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione generica per errori semantici legati alle classi nel modello target.</para>
    /// <para><b>Usage:</b><br/>
    /// Usata come fallback interno da factory statiche su <see cref="ClassInfoException"/>.</para>
    /// </summary>
    internal sealed class ClassInfoGenericException : ClassInfoException
    {
        public ClassInfoGenericException(string message) : base(message) { }
        public ClassInfoGenericException(string message, Exception inner) : base(message, inner) { }
    }

    // ==== Factory di eccezioni generiche per ClassInfo ====
    internal static class ClassInfoError
    {
        // ==== Factory generica per ClassInfo ====
        public static ClassInfoException InvalidClassInfo() =>
            new ClassInfoGenericException($"❌ La classe target fornita non è valida (null o vuota).");

        // === Eccezioni specifiche per ClassInfo ===

        public static ClassInfoException InvalidClassQualifiedName(string expected, string qualifiedName) =>
            new ClassInfoGenericException($"❌ Il ClassQualifiedName '{qualifiedName}' non corrisponde a '{expected}'.");

        public static ClassInfoException InvalidClassName(string className) =>
            new ClassInfoGenericException($"❌ Il nome della classe '{className}' non è un identificatore C# valido.");

        public static ClassInfoException ClassNotFound(string qualifiedName) =>
            new ClassInfoGenericException($"❌ Impossibile trovare il tipo '{qualifiedName}' nelle assembly caricate.");

        public static ClassInfoException InvalidDbContext() =>
            new ClassInfoGenericException($"❌ La classe deve essere associata a un DbContextInfo valido.");

        public static ClassInfoException DescriptionTooLong(string classInfoName, int maxLength) =>
            new ClassInfoGenericException($"❌ La descrizione della classe '{classInfoName}' supera la lunghezza massima consentita di {maxLength} caratteri.");
    }
}