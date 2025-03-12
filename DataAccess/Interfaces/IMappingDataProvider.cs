using JsonBridgeEF.Seeding.Mappings.Models;
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
        /// <returns>Una lista di <see cref="JsonSchemaDef"/> disponibili nel database.</returns>
        Task<List<JsonSchemaDef>> GetJsonSchemaDefAsync();

        /// <summary>
        /// Restituisce tutte le definizioni di campi JSON disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="JsonFieldDef"/> disponibili nel database.</returns>
        Task<List<JsonFieldDef>> GetJsonFieldDefAsync();

        /// <summary>
        /// Restituisce tutte le definizioni di entità target disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="TargetPropertyDef"/> disponibili nel database.</returns>
        Task<List<TargetPropertyDef>> GetTargetPropertyDefsAsync();

        /// <summary>
        /// Restituisce tutti i progetti di mapping disponibili nel database.
        /// </summary>
        /// <returns>Una lista di <see cref="MappingProject"/> disponibili nel database.</returns>
        Task<List<MappingProject>> GetMappingProjectsAsync();
    }
}