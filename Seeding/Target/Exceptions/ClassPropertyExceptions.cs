namespace JsonBridgeEF.Seeding.Target.Exceptions
{
    using JsonBridgeEF.Shared.EntityModel.Exceptions;

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione base per tutti gli errori legati alla gestione delle proprietà di classe nel modello target.</para>
    /// </summary>
    public abstract class ClassPropertyException : EntityPropertyException
    {
        protected ClassPropertyException(string message) : base(message) { }

        protected ClassPropertyException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando il nome qualificato della proprietà non è corretto.</para>
    /// </summary>
    internal sealed class ClassPropertyInvalidNameException : ClassPropertyException
    {
        public ClassPropertyInvalidNameException(string propertyName)
            : base($"❌ Il nome qualificato della proprietà '{propertyName}' non è corretto.") { }

        public static ClassPropertyInvalidNameException InvalidName(string propertyName) =>
            new ClassPropertyInvalidNameException(propertyName);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Eccezione sollevata quando la descrizione della proprietà supera la lunghezza massima consentita.</para>
    /// </summary>
    internal sealed class ClassPropertyDescriptionTooLongException : ClassPropertyException
    {
        public ClassPropertyDescriptionTooLongException(string propertyName, int maxLength)
            : base($"❌ La descrizione della proprietà '{propertyName}' supera la lunghezza massima consentita di {maxLength} caratteri.") { }

        public static ClassPropertyDescriptionTooLongException TooLong(string propertyName, int maxLength) =>
            new ClassPropertyDescriptionTooLongException(propertyName, maxLength);
    }
    
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Factory per istanziare errori legati alla gestione delle proprietà di classe.</para>
    /// </summary>
    internal static class ClassPropertyError
    {
        /// <summary>
        /// Factory per nome qualificato della proprietà non valido.
        /// </summary>
        public static ClassPropertyException InvalidClassPropertyName(string propertyName) =>
            ClassPropertyInvalidNameException.InvalidName(propertyName);

        /// <summary>
        /// Factory per descrizione della proprietà troppo lunga.
        /// </summary>
        public static ClassPropertyException DescriptionTooLong(string propertyName, int maxLength) =>
            ClassPropertyDescriptionTooLongException.TooLong(propertyName, maxLength);
    }
}