using JsonBridgeEF.Seeding.Mappings.Services;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Data;
using JsonBridgeEF.SampleTargetModel;
using JsonBridgeEF.Config;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Services;
using JsonBridgeEF.Importing.IntancePropertyProxing.Services;
using JsonBridgeEF.Importing.Preprocessing.Services;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;
using JsonBridgeEF.Seeding.TargetModel.Services;

namespace JsonBridgeEF.Client
{
    /// <summary>
    /// Servizio orchestratore per l'esecuzione del workflow completo: seeding del database,
    /// processamento JSON e gestione delle istanze dinamiche.
    /// </summary>
    internal class FullPipelineService
    {
        private readonly JsonSchemaDefSeeder _jsonSchemaDefSeeder;
        private readonly JsonFieldDefSeeder _jsonFieldDefSeeder;
        private readonly TargetDbContextSeeder _targetDbContextSeeder;
        private readonly TargetPropertySeeder _targetPropertySeeder;
        private readonly MappingProjectSeeder _mappingProjectSeeder;
        private readonly MappingRuleSeeder _mappingRuleSeeder;
        private readonly JsonProcessor _jsonProcessor;
        private readonly InstancePropertyProxyDemo _instancePropertyProxyDemo;

        /// <summary>
        /// Inizializza il servizio con tutte le dipendenze necessarie.
        /// </summary>
        /// <param name="registry">Registro dei blocchi indipendenti per il processamento JSON.</param>
        public FullPipelineService(JsonIndepBlockRegistry registry)
        {
            // Creazione del DbContext con il percorso fornito
            var dbContext = new ApplicationDbContext(AppSettings.Get("Database:ApplicationDbPath"));
            // Creazione dell'UnitOfWork per la gestione del database
            using var unitOfWork = new UnitOfWork(dbContext);

            // Inizializzazione dei servizi di seeding
            _jsonSchemaDefSeeder = new JsonSchemaDefSeeder(unitOfWork);
            _jsonFieldDefSeeder = new JsonFieldDefSeeder(unitOfWork);
            _targetDbContextSeeder = new TargetDbContextSeeder(unitOfWork);
            _targetPropertySeeder = new TargetPropertySeeder(unitOfWork);
            _mappingProjectSeeder = new MappingProjectSeeder(unitOfWork);
            _mappingRuleSeeder = new MappingRuleSeeder(unitOfWork);

            // Inizializzazione dei servizi di processamento e gestione dinamica
            _jsonProcessor = new JsonProcessor(registry);
            _instancePropertyProxyDemo = new InstancePropertyProxyDemo(dbContext);
        }

        /// <summary>
        /// Esegue il workflow completo: seeding del database, processamento JSON e gestione delle istanze.
        /// </summary>
        public async Task RunFullPipelineAsync()
        {
            Console.WriteLine("🚀 Avvio del processo completo...");

            // 1. Eseguire il pre-processamento JSON
            ExecuteJsonPreprocessing();
            Console.WriteLine("✅ Preprocessing completato con successo.");

            // 2. Eseguire il seed di source e target
            var jsonSchemaDef = await SeedJsonSchema();
            var jsonFieldDefs = await SeedJsonFields(jsonSchemaDef);
            var targetDbContextDef = await SeedTargetDbContext();
            var targetPropertyDefs = await SeedTargetProperties(targetDbContextDef);

            // 3. Eseguire il seed del mapping
            var mappingProject = await SeedMappingProject(jsonSchemaDef, targetDbContextDef);
            await SeedMappingRules(jsonFieldDefs, targetPropertyDefs, mappingProject);
            Console.WriteLine("✅ Seeding completato con successo.");

            // 4. Eseguire la gestione delle istanze dinamiche
            ExecuteInstanceProxyDemo();
            Console.WriteLine("✅ Demo intance-proxy completata con successo.");

        }

        /// <summary>
        /// Esegue il seeding dello schema JSON.
        /// </summary>
        private async Task<JsonSchemaDef> SeedJsonSchema()
        {
            await _jsonSchemaDefSeeder.ClearAll<JsonSchemaDef>();
            Console.WriteLine("📌 Seeding dello schema JSON...");
            var schema = new JsonSchemaDef
            {
                Name = "TestSchema",
                JsonSchemaIdentifier = "TestID"
            };
            return await _jsonSchemaDefSeeder.SeedAsync(schema);
        }

        /// <summary>
        /// Esegue il seeding dei campi JSON.
        /// </summary>
        private async Task<List<JsonFieldDef>> SeedJsonFields(JsonSchemaDef schema)
        {
            await _jsonSchemaDefSeeder.ClearAll<JsonFieldDef>();

            Console.WriteLine("📌 Seeding dei campi JSON...");
            string jsonFilePath = "Tests/JsonFiles/processed_data.json";
            return await _jsonFieldDefSeeder.SeedAsync(jsonFilePath, schema);
        }

        /// <summary>
        /// Esegue il seeding del contesto del database target.
        /// </summary>
        private async Task<TargetDbContextDef> SeedTargetDbContext()
        {
            await _jsonSchemaDefSeeder.ClearAll<TargetDbContextDef>();

            Console.WriteLine("📌 Seeding del contesto del database target...");
            var targetDbContextDef = new TargetDbContextDef
            {
                Name = "TestDbContext",
                Namespace = "JsonBridgeEF.Data",
                Description = "Test Context"
            };
            return await _targetDbContextSeeder.SeedAsync(targetDbContextDef);
        }

        /// <summary>
        /// Esegue il seeding delle proprietà target.
        /// </summary>
        private async Task<List<TargetPropertyDef>> SeedTargetProperties(TargetDbContextDef targetDbContextDef)
        {
            await _jsonSchemaDefSeeder.ClearAll<TargetPropertyDef>();

            Console.WriteLine("📌 Seeding delle proprietà target...");
            string targetNamespace = "JsonBridgeEF.SampleTargetModel";
            Type referenceEntityType = typeof(User); // Sostituisci con un'entità del namespace corretto
            return await _targetPropertySeeder.SeedAsync(targetDbContextDef, targetNamespace, referenceEntityType);
        }

        /// <summary>
        /// Esegue il seeding del progetto di mapping.
        /// </summary>
        private async Task<MappingProject> SeedMappingProject(JsonSchemaDef schema, TargetDbContextDef targetDbContextDef)
        {
            await _jsonSchemaDefSeeder.ClearAll<MappingProject>();

            Console.WriteLine("📌 Seeding del progetto di mapping...");
            var mappingProject = new MappingProject
            {
                Name = "Mapping Project - User Data",
                JsonSchemaDefId = schema.Id,
                TargetDbContextDefId = targetDbContextDef.Id
            };
            return await _mappingProjectSeeder.SeedAsync(mappingProject);
        }

        /// <summary>
        /// Esegue il seeding delle regole di mapping.
        /// </summary>
        private async Task<List<MappingRule>> SeedMappingRules(
            List<JsonFieldDef> jsonFieldDefs,
            List<TargetPropertyDef> targetPropertyDefs,
            MappingProject mappingProject)
        {
            await _jsonSchemaDefSeeder.ClearAll<MappingRule>();

            Console.WriteLine("📌 Seeding delle regole di mapping...");
            return await _mappingRuleSeeder.SeedAsync(jsonFieldDefs, targetPropertyDefs, mappingProject);
        }

        /// <summary>
        /// Esegue il processamento JSON utilizzando il servizio JsonProcessor.
        /// </summary>
        private void ExecuteJsonPreprocessing()
        {
            Console.WriteLine("🛠️ Avvio del processamento JSON...");
            string inputFilePath = "Tests/JsonFiles/data.json";
            string outputFilePath = "Tests/JsonFiles/processed_data.json";

            try
            {
                var output = _jsonProcessor.ProcessJsonFile(inputFilePath);
                Console.WriteLine("📜 Risultato del processamento JSON:");
                Console.WriteLine(output.ToString());

                File.WriteAllText(outputFilePath, output.ToString());
                Console.WriteLine($"📄 Output salvato in: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Errore nel processamento JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// Esegue la gestione delle istanze dinamiche utilizzando InstancePropertyProxyDemo.
        /// </summary>
        private void ExecuteInstanceProxyDemo()
        {
            Console.WriteLine("🛠️ Avvio della gestione delle istanze dinamiche...");
            _instancePropertyProxyDemo.Run();
        }
    }
}