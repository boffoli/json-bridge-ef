namespace JsonBridgeEF.Seeding.Target.Model.DbContextInfos
{
    /// <inheritdoc cref="IEfEntity"/>
    /// <summary>
    /// Partial class di <see cref="DbContextInfo"/> responsabile della persistenza tecnica.
    /// </summary>
    internal partial class DbContextInfo
    {
        /// <inheritdoc />
        public int Id { get; internal set; }
    }
}