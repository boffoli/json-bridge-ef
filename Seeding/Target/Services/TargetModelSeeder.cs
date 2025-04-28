using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Target.Helpers;
using JsonBridgeEF.Seeding.Target.Exceptions;

namespace JsonBridgeEF.Seeding.Target.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Seeder unificato per generare <see cref="DbContextInfo"/>, <see cref="ClassInfo"/> e <see cref="ClassProperty"/> da un assembly.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere istanziato tramite injection passando <see cref="IUnitOfWork"/>, <see cref="ILogger{TargetModelSeeder}"/> 
    /// e <see cref="IValidateAndFix{ClassInfo}"/>.</para>
    /// <para><b>Constraints:</b><br/>
    /// Richiede che l'assembly contenga un solo <see cref="DbContext"/> nel namespace specificato 
    /// e che abbia un costruttore senza parametri.</para>
    /// <para><b>Relationships:</b><br/>
    /// Popola le entità <see cref="DbContextInfo"/>, <see cref="ClassInfo"/>, <see cref="ClassProperty"/> 
    /// nel contesto persistente, inclusi i legami parent/child tra le classi 
    /// tramite analisi delle proprietà di navigazione.</para>
    /// </remarks>
    internal class TargetModelSeeder : BaseDbService
    {
        private readonly ILogger<TargetModelSeeder> _logger;
        private readonly IValidateAndFix<ClassInfo> _classInfoValidator;

        /// <summary>
        /// Costruisce un nuovo seeder per il modello target.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work per la persistenza.</param>
        /// <param name="logger">Logger per il seeder.</param>
        /// <param name="classInfoValidator">Validator iniettato per <see cref="ClassInfo"/>.</param>
        public TargetModelSeeder(
            IUnitOfWork unitOfWork,
            ILogger<TargetModelSeeder> logger,
            IValidateAndFix<ClassInfo> classInfoValidator
        ) : base(unitOfWork)
        {
            _logger = logger;
            _classInfoValidator = classInfoValidator;
        }

        /// <summary>
        /// Esegue il seeding del modello a partire da un assembly.
        /// </summary>
        /// <param name="assembly">Assembly contenente il <see cref="DbContext"/> EF.</param>
        /// <param name="dbContextNamespace">Namespace in cui cercare il <see cref="DbContext"/>.</param>
        /// <param name="defaultDescription">
        /// Descrizione opzionale da applicare a tutte le <see cref="ClassInfo"/>; 
        /// se nulla, verrà lasciata vuota.</param>
        /// <returns>Il <see cref="DbContextInfo"/> appena creato.</returns>
        public async Task<DbContextInfo> SeedAsync(
            Assembly assembly,
            string dbContextNamespace,
            string? defaultDescription = null
        )
        {
            // Passo 1: istanzia dinamicamente il DbContext
            var context = CreateDbContextFromAssembly(assembly, dbContextNamespace);

            // Passo 2: costruisce l'entità DbContextInfo
            var dbContextInfo = CreateDbContextInfo(context);
            GetRepository<DbContextInfo>().Add(dbContextInfo);

            // Passo 3: costruisce ClassInfo per tutte le entità mappate
            var typeToInfo = CreateClassInfos(context, dbContextInfo, defaultDescription);

            // Passo 4: analizza proprietà CLR per costruire ClassProperty e relazioni parent/child
            var graphManager = new ClassNavigationManager(
                context,
                typeToInfo,
                GetRepository<ClassProperty>(),
                _logger
            );
            var classProperties = graphManager.PopulateRelations();

            // Passo 5: salva in unica transazione
            await SaveChangesAsync();

            _logger.LogInformation("""
                ✅ Seeder completato:
                - {Classes} classi
                - {Properties} proprietà scalari
                - {Relations} relazioni parent/child
                """,
                typeToInfo.Count,
                classProperties.Count,
                typeToInfo.Values.Sum(ci => ci.Parents.Count));

            return dbContextInfo;
        }

        private static DbContext CreateDbContextFromAssembly(Assembly assembly, string ns)
        {
            var candidates = assembly.GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(DbContext).IsAssignableFrom(t) &&
                    t.Namespace == ns
                )
                .ToList();

            // Gestione di caso in cui non troviamo DbContext
            if (candidates.Count == 0)
                throw DbContextInfoError.DbContextNotFound(ns); // Eccezione per il DbContext non trovato

            // Gestione di caso in cui ci sono più di un DbContext
            if (candidates.Count > 1)
            {
                // Controllo che tutti i FullName siano non nulli prima di passarli
                var fullNames = candidates
                    .Select(c => c.FullName)
                    .Where(f => f != null) // Filtra i null
                    .Cast<string>() // Cast per garantire che l'array risultante sia di tipo string[]
                    .ToArray(); // Converte in array di stringhe non nulli (string[])

                // Verifica se fullNames è vuoto
                if (fullNames.Length == 0)
                {
                    throw DbContextInfoError.DbContextNotFound(ns); // Non ci sono DbContext con FullName valido
                }

                throw DbContextInfoError.MultipleDbContextsFound(ns, fullNames); // Eccezione per più di un DbContext
            }

            // Assicurati che `FullName` non sia null prima di passarlo
            var className = candidates.Single().FullName;
            if (string.IsNullOrEmpty(className))
            {
                throw DbContextInfoError.DbContextInstantiationFailed("Nome classe vuoto"); // Fallimento instanziazione
            }

            // Tentativo di istanziare il DbContext
            if (Activator.CreateInstance(candidates.Single()) is not DbContext context)
                throw DbContextInfoError.DbContextInstantiationFailed(className); // Lancia eccezione se instanziazione fallita

            return context;
        }

        private DbContextInfo CreateDbContextInfo(DbContext context)
        {
            var name = context.GetType().Name;
            var @namespace = context.GetType().Namespace!;
            var connectionString = context.Database.GetConnectionString();

            _logger.LogInformation("📂 Creazione contesto EF: {Context}", name);
            // qui passiamo null perché non disponiamo di descrizione per DbContext
            return new DbContextInfo(name, @namespace, null, connectionString);
        }

        private Dictionary<Type, ClassInfo> CreateClassInfos(
            DbContext context,
            DbContextInfo dbContextInfo,
            string? defaultDescription
        )
        {
            var typeToInfo = new Dictionary<Type, ClassInfo>();

            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType?.Namespace == null)
                    continue;

                _logger.LogInformation("🏷️ Rilevata classe: {Class}", clrType.FullName);

                // Costruisce e valida immediatamente la ClassInfo
                var classInfo = new ClassInfo(
                    clrType.Name,
                    clrType.Namespace,
                    defaultDescription,
                    dbContextInfo,
                    _classInfoValidator
                );

                typeToInfo[clrType] = classInfo;
                GetRepository<ClassInfo>().Add(classInfo);
            }

            return typeToInfo;
        }
    }
}