using System.Reflection;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.Target.Helpers;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JsonBridgeEF.Seeding.Target.Services;

/// <inheritdoc />
/// <summary>
/// Seeder unificato per generare <see cref="DbContextInfo"/>, <see cref="ClassInfo"/> e <see cref="ClassProperty"/> da un assembly.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Deve essere istanziato tramite injection passando <see cref="IUnitOfWork"/> e <see cref="ILogger"/>.</para>
/// <para>Constraints: Richiede che l'assembly contenga un solo DbContext nel namespace specificato e che abbia un costruttore senza parametri.</para>
/// <para>Relationships: Popola le entità <see cref="DbContextInfo"/>, <see cref="ClassInfo"/>, <see cref="ClassProperty"/> nel contesto persistente,
/// inclusi i legami parent/child tra le classi tramite analisi delle proprietà di navigazione.</para>
/// </remarks>
internal class TargetModelSeeder(IUnitOfWork unitOfWork, ILogger<TargetModelSeeder> logger)
    : BaseDbService(unitOfWork)
{
    public async Task<DbContextInfo> SeedAsync(Assembly assembly, string dbContextNamespace)
    {
        // Passo 1: istanzia dinamicamente il DbContext
        var context = CreateDbContextFromAssembly(assembly, dbContextNamespace);

        // Passo 2: costruisce l'entità DbContextInfo
        var dbContextInfo = CreateDbContextInfo(context);
        GetRepository<DbContextInfo>().Add(dbContextInfo);

        // Passo 3: costruisce ClassInfo per tutte le entità mappate
        var typeToInfo = CreateClassInfos(context, dbContextInfo);

        // Passo 4: analizza proprietà CLR per costruire ClassProperty e relazioni parent/child
        var graphManager = new ClassNavigationManager(context, typeToInfo, GetRepository<ClassProperty>(), logger);
        var classProperties = graphManager.PopulateRelations();

        // Passo 5: salva in unica transazione
        await SaveChangesAsync();

        logger.LogInformation("""
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
            .Where(t => t.IsClass && !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t) && t.Namespace == ns)
            .ToList();

        if (candidates.Count == 0)
            throw new InvalidOperationException($"❌ Nessun DbContext trovato nel namespace '{ns}'.");

        if (candidates.Count > 1)
            throw new InvalidOperationException($"❌ Più di un DbContext trovato in '{ns}': {string.Join(", ", candidates.Select(t => t.FullName))}");

        if (Activator.CreateInstance(candidates.Single()) is not DbContext context)
            throw new InvalidOperationException($"❌ Impossibile istanziare {candidates.Single().FullName}.");

        return context;
    }

    private DbContextInfo CreateDbContextInfo(DbContext context)
    {
        var name = context.GetType().Name;
        var @namespace = context.GetType().Namespace!;
        var connectionString = context.Database.GetConnectionString();

        logger.LogInformation("📂 Creazione contesto EF: {Context}", name);
        return new DbContextInfo(name, @namespace, connectionString);
    }

    private Dictionary<Type, ClassInfo> CreateClassInfos(DbContext context, DbContextInfo dbContextInfo)
    {
        var typeToInfo = new Dictionary<Type, ClassInfo>();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            var clrNamespace = clrType.Namespace;
            if (clrNamespace is null)
                continue;

            logger.LogInformation("🏷️ Rilevata classe: {Class}", clrType.FullName);
            var classInfo = new ClassInfo(clrType.Name, clrNamespace, dbContextInfo);
            typeToInfo[clrType] = classInfo;
            GetRepository<ClassInfo>().Add(classInfo);
        }

        return typeToInfo;
    }
}
