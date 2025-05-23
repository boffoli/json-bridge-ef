namespace JsonBridgeEF.Seeding.Source.JsonEntities.Model
{
    /// <inheritdoc cref="IEfEntity"/>
    internal sealed partial class JsonEntity
    {
        // --- Proprietà EfEntity ---

        /// <inheritdoc />
        /// <summary>
        /// Identificatore univoco utilizzato dall'Entity Framework per la persistenza.
        /// </summary>
        public int Id { get; private set;}

        /// <summary>
        /// Chiave esterna che fa riferimento all'identificatore dello schema JSON proprietario.
        /// </summary>
        public int JsonSchemaId { get; private set; }
    }
    
}