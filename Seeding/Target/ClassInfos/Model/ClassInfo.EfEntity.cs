namespace JsonBridgeEF.Seeding.Target.ClassInfos.Model
{
    /// <inheritdoc cref="IEfEntity"/>
    /// <summary>
    /// Partial class di <see cref="ClassInfo"/> responsabile dell'implementazione dell'interfaccia per Entity Framework.
    /// </summary>
    internal sealed partial class ClassInfo
    {
        /// <inheritdoc />
        /// <summary>
        /// Identificatore univoco utilizzato dall'Entity Framework per la persistenza.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Chiave esterna verso il <see cref="DbContextInfo"/> di appartenenza.
        /// </summary>
        public int DbContextInfoId { get; internal set; }
    }
}