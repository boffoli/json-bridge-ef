namespace JsonBridgeEF.Seeding.Target.ClassProperties.Model
{
    /// <inheritdoc cref="IEfEntity"/>
    /// <summary>
    /// Partial class di <see cref="ClassProperty"/> che espone l'identificatore per la persistenza Entity Framework.
    /// </summary>
    internal sealed partial class ClassProperty
    {
        /// <inheritdoc />
        public int Id { get; internal set; }

        /// <summary>
        /// Chiave esterna verso la classe proprietaria (<see cref="ClassInfo"/>).
        /// </summary>
        public int ParentId { get; internal set; }
    }
}