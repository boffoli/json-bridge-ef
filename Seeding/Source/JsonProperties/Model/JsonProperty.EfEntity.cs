namespace JsonBridgeEF.Seeding.Source.JsonProperties.Model
{
    /// <inheritdoc cref="IEfEntity"/>
    /// <summary>
    /// Partial class di <see cref="JsonProperty"/> responsabile dell'implementazione dell'identificativo Entity Framework (<see cref="IEfEntity"/>).
    /// </summary>
    internal sealed partial class JsonProperty
    {
        // --- Proprietà EfEntity ---

        /// <inheritdoc />
        /// <summary>
        /// Identificatore univoco utilizzato dall'Entity Framework per la persistenza.
        /// </summary>
        public int Id { get; private set; }


        /// <summary>
        /// Chiave esterna che fa riferimento all'oggetto JSON proprietario.
        /// </summary>
        public int JsonEntityId { get; private set; }
    }
}