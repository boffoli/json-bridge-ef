namespace JsonBridgeEF.Seeding.Source.Model.JsonSchemas
{
    /// <inheritdoc cref="IEfEntity"/>
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> responsabile dell'implementazione dell'identificativo Entity Framework (<see cref="IEfEntity"/>).
    /// </summary>
    internal sealed partial class JsonSchema
    {
        // --- Proprietà EfEntity ---

        /// <inheritdoc />
        /// <summary>
        /// Identificatore univoco utilizzato dall'Entity Framework per la persistenza.
        /// </summary>
        public int Id { get; private set; }
    }
}