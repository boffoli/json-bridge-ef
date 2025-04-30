using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Helpers;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Services
{
    /// <summary>
    /// Domain Concept: Servizio per il seeding dei blocchi JSON estratti da uno schema.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: iniettato tramite <see cref="IUnitOfWork"/>; lavora su tipi concreti o interfacce.</para>
    /// <para>Constraints: lo schema deve essere tracciato e non contenere blocchi preesistenti.</para>
    /// <para>Relationships: utilizza <see cref="JsonEntityExtractor"/> e 
    /// <see cref="IRepository{TEntity}"/>.</para>
    /// <para>Usage Notes: va usato dopo la creazione dello schema per popolazione dei blocchi.</para>
    /// </remarks>
    internal sealed class JsonEntitySeeder : BaseDbService
    {
        /// <summary>
        /// Blocchi generati durante l’ultima esecuzione di <see cref="SeedAsync{TEntity,TProp}"/>.
        /// </summary>
        public IReadOnlyList<JsonEntity> SeededJsonEntities { get; private set; } = Array.Empty<JsonEntity>();

        public JsonEntitySeeder(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Estrae e salva i blocchi JSON dallo schema specificato.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Tipo concreto del blocco JSON; implementa <see cref="IJsonEntity{TEntity,TProp}"/>.
        /// </typeparam>
        /// <typeparam name="TProp">
        /// Tipo concreto della proprietà JSON; implementa <see cref="IJsonProperty{TProp,TEntity}"/>.
        /// </typeparam>
        /// <param name="schema">
        /// Schema da cui estrarre i blocchi, in forma di <see cref="IJsonSchema{TEntity}"/>.
        /// </param>
        /// <returns>
        /// Lista di blocchi come <see cref="IJsonEntity{TEntity,TProp}"/>.
        /// </returns>
        /// <exception cref="JsonEntityAlreadyExistsException">
        /// Se lo schema ha già blocchi associati.
        /// </exception>
        internal async Task<IReadOnlyList<IJsonEntity<TEntity, TProp>>> SeedAsync<TEntity, TProp>(
            IJsonSchema<TEntity> schema)
            where TEntity : class, IJsonEntity<TEntity, TProp>
            where TProp   : class, IJsonProperty<TProp, TEntity>
        {
            ArgumentNullException.ThrowIfNull(schema);
            EnsureTracked(schema);

            Console.WriteLine($"📂 Avvio seeding dei blocchi per schema: {schema.Name}");

            var repo = GetRepository<TEntity>();

            // ❌ Previene duplicazione controllando via interfaccia
            if (await HasExistingJsonEntitiesAsync<TEntity, TProp>(schema.Id, repo))
                throw JsonEntityAlreadyExistsException.AlreadyExists(schema.Name);

            // 🧠 Estrazione dei JsonEntity concreti
            var extracted = JsonEntityExtractor.Extract(
                (IJsonSchema<JsonEntity>)schema,
                entityValidator: null,
                propertyValidator: null
            );

            if (extracted.Count > 0)
            {
                repo.AddRange(extracted.Cast<TEntity>());
                await SaveChangesAsync();
                Console.WriteLine($"✅ {extracted.Count} blocchi salvati.");
            }
            else
            {
                Console.WriteLine("ℹ️ Nessun blocco estratto dallo schema.");
            }

            SeededJsonEntities = extracted;
            return extracted.Cast<IJsonEntity<TEntity, TProp>>().ToList();
        }

        /// <summary>
        /// Verifica se esistono già blocchi per lo schema specificato.
        /// </summary>
        private static Task<bool> HasExistingJsonEntitiesAsync<TEntity, TProp>(
            int schemaId,
            IRepository<TEntity> repository)
            where TEntity : class, IJsonEntity<TEntity, TProp>
            where TProp   : class, IJsonProperty<TProp, TEntity>
        {
            // Usiamo ExistsAsync sul campo Schema.Id esposto dall'interfaccia IJsonEntity
            return repository.ExistsAsync(e => e.Schema.Id == schemaId);
        }

        /// <summary>
        /// Promuove un blocco a entità identificabile assegnandogli la chiave logica.
        /// </summary>
        internal async Task PromoteToIndependentAsync(
            string jsonEntityName,
            string keyFieldName,
            string schemaName)
        {
            var jsonEntity = SeededJsonEntities
                .FirstOrDefault(e => e.Name.Equals(jsonEntityName, StringComparison.OrdinalIgnoreCase))
                ?? throw JsonEntityNotFoundException.NotFound(jsonEntityName, schemaName);

            try
            {
                jsonEntity.MakeIdentifiable(keyFieldName);
                Console.WriteLine(
                    $"🔑 Blocco '{jsonEntity.Name}' promosso a indipendente con chiave '{keyFieldName}'.");

                GetRepository<JsonEntity>().Update(jsonEntity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw JsonEntityPromotionException
                    .PromotionFailed(jsonEntityName, keyFieldName, ex.Message);
            }
        }
    }
}