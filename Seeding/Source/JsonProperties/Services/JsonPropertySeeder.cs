using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Source.JsonProperties.Helpers;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Helpers;

namespace JsonBridgeEF.Seeding.Source.JsonProperties.Services
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Servizio per l’estrazione e la persistenza dei campi di un blocco JSON all’interno di uno schema.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// I campi vengono generati tramite <see cref="JsonPropertyExtractor"/> e tracciati tramite EF Core.
    /// Questo seeder non genera blocchi, ma opera sui blocchi già tracciati all'interno dello schema.</para>
    /// </summary>
    /// <remarks>
    /// Inizializza il seeder dei campi JSON con il contesto di unità di lavoro corrente.
    /// </remarks>
    internal sealed class JsonPropertySeeder : BaseDbService
    {
        public JsonPropertySeeder(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Estrae e salva i campi di un blocco specifico appartenente a uno schema tipizzato.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Il tipo concreto dell’entità JSON (blocco).
        /// </typeparam>
        /// <typeparam name="TProp">
        /// Il tipo concreto della proprietà JSON.
        /// </typeparam>
        /// <param name="schema">
        /// Schema tipizzato da cui estrarre (<see cref="IJsonSchema{TEntity}"/>).
        /// </param>
        /// <param name="jsonEntity">
        /// Blocco tipizzato da cui generare i campi (<see cref="IJsonEntity{TEntity,TProp}"/>).
        /// </param>
        /// <returns>
        /// Lista delle proprietà generate e salvate come <see cref="IJsonProperty{TProp,TEntity}"/>.
        /// </returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Lo schema e il blocco devono essere non null e coerenti tra loro.</para>
        /// <para><b>Postconditions:</b> I campi vengono estratti e persistiti tramite il repository EF.</para>
        /// <para><b>Side Effects:</b> Salvataggio su database dei <see cref="IJsonProperty{TProp,TEntity}"/>.</para>
        /// </remarks>
        internal async Task<List<IJsonProperty<TProp, TEntity>>> SeedAsync<TEntity, TProp>(
            IJsonSchema<TEntity> schema,
            IJsonEntity<TEntity, TProp> jsonEntity)
            where TEntity : class, IJsonEntity<TEntity, TProp>
            where TProp : class, IJsonProperty<TProp, TEntity>
        {
            if (schema == null)
                throw JsonSchemaError.InvalidSchemaReference();

            if (jsonEntity == null)
                throw JsonEntityError.InvalidJsonEntity();

            // Verifica che il blocco appartenga allo schema
            JsonSchemaHelper.EnsureJsonEntityBelongsToSchema(schema, jsonEntity);

            Console.WriteLine(
                $"📂 Generazione campi per blocco '{jsonEntity.Name}' nello schema '{schema.Name}'");

            // 🧠 Estrazione dei campi
            JsonPropertyExtractor.ExtractJsonProperties(schema, jsonEntity);

            // 💾 Persistenza
            var repo = GetRepository<TProp>();
            var saved = new List<IJsonProperty<TProp, TEntity>>();

            foreach (var i in jsonEntity.ValueChildren)
            {
                if (i is not TProp prop)
                    throw new InvalidCastException($"Ci aspettavamo proprietà di tipo {typeof(TProp).Name}.");

                repo.Add(prop);
                saved.Add(prop);
            }

            await SaveChangesAsync();
            return saved;
        }

        /// <summary>
        /// Estrae e salva i campi per una collezione di blocchi tipizzati appartenenti a uno schema.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Il tipo concreto dell’entità JSON (blocco).
        /// </typeparam>
        /// <typeparam name="TProp">
        /// Il tipo concreto della proprietà JSON.
        /// </typeparam>
        /// <param name="schema">
        /// Schema tipizzato di riferimento (<see cref="IJsonSchema{TEntit}"/>).
        /// </param>
        /// <param name="jsonEntities">
        /// Collezione di blocchi tipizzati (<see cref="IJsonEntity{TEntity,TProp}"/>).
        /// </param>
        /// <returns>
        /// Lista completa delle proprietà generate per tutti i blocchi.
        /// </returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> La lista di entità deve essere non nulla.</para>
        /// <para><b>Postconditions:</b> Tutti i campi validi vengono salvati nel contesto.</para>
        /// <para><b>Side Effects:</b> Persistenza multipla di <see cref="IJsonProperty{TProp,TEntity}"/>.</para>
        /// </remarks>
        internal async Task<List<IJsonProperty<TProp, TEntity>>> SeedAsync<TEntity, TProp>(
            IJsonSchema<TEntity> schema,
            IEnumerable<IJsonEntity<TEntity, TProp>> jsonEntities)
            where TEntity : class, IJsonEntity<TEntity, TProp>
            where TProp : class, IJsonProperty<TProp, TEntity>
        {
            if (jsonEntities == null || !jsonEntities.Any())
                return new List<IJsonProperty<TProp, TEntity>>();

            var all = new List<IJsonProperty<TProp, TEntity>>();
            foreach (var block in jsonEntities)
            {
                var fields = await SeedAsync(schema, block);
                all.AddRange(fields);
            }
            return all;
        }
    }
}