using JsonBridgeEF.Data;
using JsonBridgeEF.Importing.IntancePropertyProxing.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Services
{
    /// <summary>
    /// Classe demo per la gestione dinamica delle istanze tramite InstanceAccessor.
    /// </summary>
    internal class InstancePropertyProxyDemo
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<string, InstancePropertyProxy> _instanceAccessors;

        internal InstancePropertyProxyDemo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _instanceAccessors = [];
        }

        public void Run()
        {
            Console.WriteLine("\n🚀 Avvio della demo di InstanceAccessor...");

            List<TargetPropertyDef> targetProperties = LoadTargetProperties();

            var propertiesByRootClass = targetProperties
                .GroupBy(p => p.ClassQualifiedName)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var classQualifiedName in propertiesByRootClass.Keys)
            {
                Console.WriteLine($"\n📌 Creazione istanza per {classQualifiedName}...");

                try
                {
                    var accessor = new InstancePropertyProxy(classQualifiedName);
                    _instanceAccessors[classQualifiedName] = accessor;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Errore nella creazione di '{classQualifiedName}': {ex.Message}");
                }
            }

            foreach (var (classQualifiedName, accessor) in _instanceAccessors)
            {
                if (!propertiesByRootClass.ContainsKey(classQualifiedName)) continue;

                foreach (var propertyDef in propertiesByRootClass[classQualifiedName])
                {
                    object sampleValue = InstancePropertyProxy.GenerateSampleValue(propertyDef.FullyQualifiedPropertyName); Console.WriteLine($"   🔹 Imposto '{propertyDef.FullyQualifiedPropertyName}' = {sampleValue}");
                    accessor[propertyDef.PropertyPathName] = sampleValue;
                }
            }

            PrintInstances();
        }

        private List<TargetPropertyDef> LoadTargetProperties()
        {
            return [.. _dbContext.TargetPropertyDefs];
        }

        private void PrintInstances()
        {
            Console.WriteLine("\n🔍 Stampa delle istanze create:");

            foreach (var accessor in _instanceAccessors.Values)
            {
                accessor.PrintInstance();
            }

            Console.WriteLine("\n✅ Demo completata!");
        }
    }
}