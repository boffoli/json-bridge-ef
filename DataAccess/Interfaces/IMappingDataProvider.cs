using JsonBridgeEF.Seeding.Mapping.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.DataAccess.Interfaces
{
    /// <summary>
    /// Interfaccia per la fornitura di dati necessari alla generazione delle regole di mapping.
    /// Fornisce metodi per recuperare definizioni di tipi JSON, campi JSON, entità target e progetti di mapping.
    /// </summary>
    internal interface IMappingDataProvider
    {
        /// <summary>
        /// Restituisce tutte le definizioni di tipi JSON disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="JsonSchema"/> disponibili nel database.</returns>
        Task<List<JsonSchema>> GetJsonSchemaAsync();

        /// <summary>
        /// Restituisce tutte le definizioni di campi JSON disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="JsonField"/> disponibili nel database.</returns>
        Task<List<JsonField>> GetJsonFieldAsync();

        /// <summary>
        /// Restituisce tutte le definizioni di entità target disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="TargetProperty"/> disponibili nel database.</returns>
        Task<List<TargetProperty>> GetTargetPropertysAsync();

        /// <summary>
        /// Restituisce tutti i progetti di mapping disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="MappingProject"/> disponibili nel database.</returns>
        Task<List<MappingProject>> GetMappingProjectsAsync();
    }
}