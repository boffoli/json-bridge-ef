using JsonBridgeEF.Seeding.Source.Facade.Dtos;

namespace JsonBridgeEF.Seeding.Source.Facade.Interfaces
{
    /// <summary>
    /// Domain Interface: facade unico per orchestrare il seeding completo di 
    /// schema, entità e proprietà JSON a partire da un file di esempio.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nasconde i tre servizi interni (schema, entity, property) dietro un’unica API.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato come classe interna con visibilità a livello di assembly.</para>
    /// <para><b>Constraints:</b><br/>
    /// Accetta solo percorso di file JSON di esempio, nome dello schema e flag di forzatura.</para>
    /// <para><b>Relationships:</b><br/>
    /// Collabora con <c>JsonSchemaSeeder</c>, <c>JsonEntitySeeder</c> e <c>JsonPropertySeeder</c>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Restituisce sempre un <see cref="JsonSeedingResultDto"/> contenente l’intero grafo seedato.</para>
    /// </remarks>
    public interface IJsonSeedingFacade
    {
        /// <summary>
        /// Esegue il seeding completo: crea lo schema, ne estrae i blocchi e i campi, 
        /// persiste tutto e ritorna il risultato aggregato.
        /// </summary>
        /// <param name="schemaName">Nome logico da assegnare allo schema.</param>
        /// <param name="sampleJsonFilePath">Percorso del file JSON di esempio.</param>
        /// <param name="forceSave">
        /// Se <c>true</c>, forza il salvataggio anche in caso di duplicato di contenuto.
        /// </param>
        /// <returns>
        /// Un <see cref="JsonSeedingResultDto"/> con schema, entità e proprietà
        /// generate e salvate.
        /// </returns>
        Task<JsonSeedingResultDto> SeedAsync(
            string schemaName,
            string sampleJsonFilePath,
            bool forceSave = false);
    }
}