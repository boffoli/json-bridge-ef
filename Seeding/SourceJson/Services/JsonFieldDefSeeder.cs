using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Services
{
    /// <summary>
    /// Servizio per il seeding dei campi JSON associati a uno schema.
    /// </summary>
    /// <remarks>
    /// Costruttore del servizio per il seeding dei campi JSON.
    /// </remarks>
    /// <param name="mappingDefService">Servizio di accesso ai dati.</param>
    internal class JsonFieldDefSeeder(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        /// <summary>
        /// Esegue il seeding dei campi JSON per uno schema specifico.
        /// </summary>
        /// <param name="jsonFilePath">Percorso del file JSON.</param>
        /// <param name="schema">Schema JSON di destinazione.</param>
        internal async Task<List<JsonFieldDef>> SeedAsync(string jsonFilePath, JsonSchemaDef schema)
        {
            if (string.IsNullOrWhiteSpace(jsonFilePath))
                throw new ArgumentNullException(nameof(jsonFilePath));
            schema.EnsureValid();

            Console.WriteLine($"📂 Creazione dei campi JSON per lo schema: {schema.JsonSchemaIdentifier}");

            var jsonFields = JsonFieldDefHelper.ExtractJsonFields(jsonFilePath, schema.Id);

            jsonFields.ForEach(f => f.EnsureValid());
            
            foreach (var field in jsonFields)
            {
                GetRepository<JsonFieldDef>().Add(field);
                await SaveChangesAsync();
            }

            return jsonFields;
        }
    }
}