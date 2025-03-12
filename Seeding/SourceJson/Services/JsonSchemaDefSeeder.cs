using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Services
{
    /// <summary>
    /// Servizio per il seeding dello schema JSON nel database.
    /// </summary>
    internal class JsonSchemaDefSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding della definizione dello schema JSON.
        /// </summary>
        /// <param name="schema">Lo schema JSON da salvare.</param>
        internal async Task<JsonSchemaDef> SeedAsync(JsonSchemaDef schema)
        {
            schema.EnsureValid();
            
            Console.WriteLine($"📂 Creazione dello schema JSON: {schema.JsonSchemaIdentifier}");
            GetRepository<JsonSchemaDef>().Add(schema);
            await SaveChangesAsync();
            return schema;
        }
    }
}