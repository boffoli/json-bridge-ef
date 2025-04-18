using System.Reflection;
using Microsoft.Extensions.Logging;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Seeding.Target.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Manager responsabile dell'analisi delle relazioni tra classi CLR mappate da EF,
/// con lo scopo di popolare proprietà scalari e collegamenti parent/child nel grafo OO.
/// </para>
/// </summary>
internal sealed class ClassNavigationManager(
    DbContext context,
    Dictionary<Type, ClassInfo> typeMap,
    IRepository<ClassProperty> propertyRepo,
    ILogger logger)
{
    private readonly DbContext _context = context;
    private readonly Dictionary<Type, ClassInfo> _typeMap = typeMap;
    private readonly IRepository<ClassProperty> _propertyRepo = propertyRepo;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Popola le <see cref="ClassProperty"/> scalari e le relazioni parent/child tra le entità <see cref="ClassInfo"/>.
    /// </summary>
    /// <returns>Lista delle proprietà scalari identificate e mappate.</returns>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il dizionario dei tipi deve essere popolato.</para>
    /// <para><b>Postconditions:</b> Le entità avranno relazioni e proprietà scalari assegnate.</para>
    /// <para><b>Side Effects:</b> Aggiornamento del repository di proprietà e logging.</para>
    /// </remarks>
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

                if (TryAddNavigationToChild(propType, classInfo, propName))
                    continue;

                if (TryAddNavigationFromCollection(propType, classInfo, propName))
                    continue;

                if (TryAddScalarProperty(clrType, classInfo, property, propName, fqName, out var classProp))
                {
                    classProperties.Add(classProp!);
                }
            }
        }

        return classProperties;
    }

    /// <summary>
    /// Aggiunge una relazione diretta 1:1 da <paramref name="parentInfo"/> verso il tipo figlio se presente nel grafo.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il tipo figlio deve esistere nella mappa dei tipi.</para>
    /// <para><b>Postconditions:</b> La relazione parent → child viene tracciata.</para>
    /// <para><b>Side Effects:</b> Logging della relazione creata.</para>
    /// </remarks>
    private bool TryAddNavigationToChild(Type propType, ClassInfo parentInfo, string propName)
    {
        if (_typeMap.TryGetValue(propType, out var childClass))
        {
            childClass.AddParent(parentInfo);
            _logger.LogInformation("🔗 Navigazione: {Parent} → {Child} via {Property}", parentInfo.Name, childClass.Name, propName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Aggiunge una relazione inversa da una collezione (1:N) verso il tipo contenuto.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> La proprietà deve essere una ICollection&lt;T&gt; dove T è un tipo noto nel grafo.</para>
    /// <para><b>Postconditions:</b> Il tipo contenuto aggiunge il tipo corrente come parent.</para>
    /// <para><b>Side Effects:</b> Logging della relazione inversa creata.</para>
    /// </remarks>
    private bool TryAddNavigationFromCollection(Type propType, ClassInfo parentInfo, string propName)
    {
        if (propType.IsGenericType &&
            propType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
            _typeMap.TryGetValue(propType.GetGenericArguments()[0], out var childClass))
        {
            childClass.AddParent(parentInfo);
            _logger.LogInformation("🔗 Navigazione inversa: {Parent} → {Child} via {Property}", parentInfo.Name, childClass.Name, propName);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Identifica e registra una proprietà scalare tracciata da EF Core.
    /// </summary>
    /// <param name="clrType">Tipo della classe di appartenenza.</param>
    /// <param name="classInfo">Istanza del grafo da aggiornare.</param>
    /// <param name="property">Riflessione sulla proprietà da valutare.</param>
    /// <param name="propName">Nome della proprietà.</param>
    /// <param name="fqName">Nome completamente qualificato della proprietà.</param>
    /// <param name="result">Proprietà risultante, se riconosciuta.</param>
    /// <returns><c>true</c> se la proprietà è stata riconosciuta come scalare; altrimenti <c>false</c>.</returns>
    /// <remarks>
    /// <para><b>Preconditions:</b> Il modello EF deve contenere la proprietà.</para>
    /// <para><b>Postconditions:</b> La proprietà viene aggiunta al grafo e tracciata dal repository.</para>
    /// <para><b>Side Effects:</b> Persistenza nel repository e log diagnostico.</para>
    /// </remarks>
    private bool TryAddScalarProperty(Type clrType,
                                      ClassInfo classInfo,
                                      PropertyInfo property,
                                      string propName,
                                      string fqName,
                                      out ClassProperty? result)
    {
        result = null;

        // Recupera il tipo EF tracciato per la classe CLR
        var efEntityType = _context.Model.FindEntityType(clrType);
        if (efEntityType is null)
            return false;

        // Recupera la proprietà mappata da EF
        var efProperty = efEntityType.FindProperty(propName);
        if (efProperty is null)
            return false;

        // Verifica se EF o l'attributo [Key] indicano che è una chiave
        var isKeyByEf = efEntityType.FindPrimaryKey()?.Properties.Any(p => p.Name == propName) == true;
        var isKeyByAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Any();
        var isKey = isKeyByEf || isKeyByAttr;

        if (isKeyByAttr && !isKeyByEf)
        {
            _logger.LogWarning("⚠️ La proprietà {Property} è decorata con [Key] ma non vista da EF come chiave.", fqName);
        }

        // Descrizione iniziale: può essere estesa in futuro tramite attributi custom
        var description = string.Empty;

        // Crea la proprietà e la aggiunge al repository
        result = new ClassProperty(propName, classInfo, isKey, description, validator: null);
        _propertyRepo.Add(result);
        _logger.LogInformation("🔧 Proprietà scalare: {Property}", fqName);

        return true;
    }

    /// <summary>
    /// Recupera tutte le proprietà pubbliche di istanza della classe specificata.
    /// </summary>
    /// <param name="type">Tipo da ispezionare.</param>
    /// <returns>Proprietà pubbliche di istanza.</returns>
    private static PropertyInfo[] GetNavigableProperties(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
}