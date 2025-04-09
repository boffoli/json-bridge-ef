using System.Reflection;
using Microsoft.Extensions.Logging;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Seeding.Target.Helpers;

/// <summary>
/// Manager responsabile dell'analisi delle relazioni tra classi CLR mappate da EF,
/// con lo scopo di popolare proprietà scalari e collegamenti parent/child nel grafo OO.
/// </summary>
internal sealed class ClassNavigationManager(DbContext context,
                                             Dictionary<Type, ClassInfo> typeMap,
                                             IRepository<ClassProperty> propertyRepo,
                                             ILogger logger)
{
    private readonly DbContext _context = context;
    private readonly Dictionary<Type, ClassInfo> _typeMap = typeMap;
    private readonly IRepository<ClassProperty> _propertyRepo = propertyRepo;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Passo 4: Popola tutte le <see cref="ClassProperty"/> scalari
    /// e stabilisce i legami parent/child tra <see cref="ClassInfo"/>.
    /// </summary>
    /// <returns>Lista delle proprietà scalari rilevate.</returns>
    public List<ClassProperty> PopulateRelations()
    {
        var classProperties = new List<ClassProperty>();

        foreach (var (clrType, classInfo) in _typeMap)
        {
            foreach (var property in GetNavigableProperties(clrType))
            {
                var propName = property.Name;
                var propType = property.PropertyType;
                var fqName = $"{clrType.FullName}.{propName}";

                // Passo 4.1: Navigazione diretta verso tipo figlio (es. OrderDetail → Product)
                if (TryAddNavigationToChild(propType, classInfo, propName))
                    continue;

                // Passo 4.2: Navigazione inversa da collezione (es. Product ← OrderDetails)
                if (TryAddNavigationFromCollection(propType, classInfo, propName))
                    continue;

                // Passo 4.3: Proprietà scalare mappata da EF
                if (TryAddScalarProperty(clrType, classInfo, property, propName, fqName, out var classProp))
                {
                    classProperties.Add(classProp!);
                }
            }
        }

        return classProperties;
    }

    /// <summary>
    /// Passo 4.1: Aggiunge una relazione strutturale diretta (1:1) da <paramref name="classInfo"/> verso il tipo contenuto.
    /// </summary>
    private bool TryAddNavigationToChild(Type propType, ClassInfo parentInfo, string propName)
    {
        if (_typeMap.TryGetValue(propType, out var childClass))
        {
            childClass.AddParent(parentInfo); // classInfo → child
            _logger.LogInformation("🔗 Navigazione: {Parent} → {Child} via {Property}", parentInfo.Name, childClass.Name, propName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Passo 4.2: Aggiunge una relazione strutturale inversa da collezione (1:N) verso il tipo referenziato come aggregato.
    /// </summary>
    private bool TryAddNavigationFromCollection(Type propType, ClassInfo parentInfo, string propName)
    {
        if (propType.IsGenericType &&
            propType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
            _typeMap.TryGetValue(propType.GetGenericArguments()[0], out var childClass))
        {
            childClass.AddParent(parentInfo); // parentInfo → collection of childClass
            _logger.LogInformation("🔗 Navigazione inversa: {Parent} → {Child} via {Property}", parentInfo.Name, childClass.Name, propName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Passo 4.3: Tenta di riconoscere e mappare una proprietà scalare gestita da EF.
    /// </summary>
    private bool TryAddScalarProperty(Type clrType,
                                      ClassInfo classInfo,
                                      PropertyInfo property,
                                      string propName,
                                      string fqName,
                                      out ClassProperty? result)
    {
        result = null;

        var efEntityType = _context.Model.FindEntityType(clrType);
        var efProperty = efEntityType?.FindProperty(propName);
        if (efProperty == null)
            return false;

        var isKeyByEf = efEntityType.FindPrimaryKey()?.Properties.Any(p => p.Name == propName) == true;
        var isKeyByAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Any();
        var isKey = isKeyByEf || isKeyByAttr;

        if (isKeyByAttr && !isKeyByEf)
        {
            _logger.LogWarning("⚠️ La proprietà {Property} è decorata con [Key] ma non vista da EF come chiave.", fqName);
        }

        result = new ClassProperty(propName, classInfo, isKey);
        _propertyRepo.Add(result);
        _logger.LogInformation("🔧 Proprietà scalare: {Property}", fqName);
        return true;
    }

    /// <summary>
    /// Passo 4.0: Recupera solo proprietà pubbliche di istanza.
    /// </summary>
    private static PropertyInfo[] GetNavigableProperties(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
}