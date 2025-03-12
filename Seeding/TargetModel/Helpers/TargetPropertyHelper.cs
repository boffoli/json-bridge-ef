using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema; // Per ForeignKeyAttribute
using JsonBridgeEF.Validators;
using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.TargetModel.Helpers
{
    /// <summary>
    /// Helper per la generazione automatica delle definizioni di proprietà target.
    /// Permette di esplorare le entità di un dato namespace e creare definizioni di proprietà.
    /// </summary>
    internal static class TargetPropertyHelper
    {
        /// <summary>
        /// Genera una lista di <see cref="TargetPropertyDef"/> per tutte le entità presenti
        /// nel namespace specificato.
        /// </summary>
        /// <param name="targetDbContextDefId">L'ID del contesto di destinazione.</param>
        /// <param name="targetNamespace">Il namespace contenente le entità target.</param>
        /// <param name="referenceEntityType">Un tipo di riferimento per determinare l'assembly da cui caricare le entità.</param>
        /// <returns>Una lista di <see cref="TargetPropertyDef"/> per il seeding nel database.</returns>
        public static List<TargetPropertyDef> GenerateTargetProperties(int targetDbContextDefId, string targetNamespace, Type referenceEntityType)
        {
            var definitions = new List<TargetPropertyDef>();

            var assembly = Assembly.GetAssembly(referenceEntityType);
            var entityTypes = assembly?
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace == targetNamespace && !t.IsAbstract)
                .Where(t => t.GetProperties().Any(p => p.GetCustomAttribute<KeyAttribute>() != null)) // 🔹 Filtra solo classi con almeno una proprietà [Key]
                .ToList();

            if (entityTypes == null || entityTypes.Count == 0)
            {
                Console.WriteLine($"⚠️ Nessuna entità con chiave primaria trovata nel namespace '{targetNamespace}'.");
                return definitions;
            }

            foreach (var entityType in entityTypes)
            {
                ProcessEntityType(definitions, entityType, targetDbContextDefId, entityType.Name, "");
            }

            return definitions;
        }

        /// <summary>
        /// Elabora un'entità e le sue proprietà per generare definizioni di <see cref="TargetPropertyDef"/>.
        /// </summary>
        private static void ProcessEntityType(
            List<TargetPropertyDef> definitions,
            Type entityType,
            int targetDbContextDefId,
            string rootClass,
            string parentPath)
        {
            var properties = entityType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead
                            && p.GetGetMethod() != null
                            && !p.GetGetMethod()!.IsStatic
                            && p.GetIndexParameters().Length == 0)
                .ToList();

            var propertyToFkTarget = BuildForeignKeyMap(properties);

            foreach (var property in properties)
            {
                // **Salta le proprietà decorate con [ForeignKey(nameof(...))]**
                if (ShouldSkipForeignKeyReference(property, propertyToFkTarget))
                    continue;

                string currentPath = string.IsNullOrEmpty(parentPath)
                    ? property.Name
                    : $"{parentPath}.{property.Name}";

                // **Se è un tipo complesso, naviga ricorsivamente**
                if (IsComplexType(property, entityType))
                {
                    ProcessEntityType(definitions, property.PropertyType, targetDbContextDefId, rootClass, currentPath);
                    continue;
                }

                TryCreateTargetPropertyDef(definitions, property, targetDbContextDefId, rootClass, parentPath);
            }
        }

        /// <summary>
        /// Crea una mappa delle proprietà decorate con [ForeignKey(nameof(...))].
        /// </summary>
        private static Dictionary<PropertyInfo, string?> BuildForeignKeyMap(IEnumerable<PropertyInfo> properties)
        {
            return properties.ToDictionary(
                prop => prop,
                prop => prop.GetCustomAttribute<ForeignKeyAttribute>()?.Name
            );
        }

        /// <summary>
        /// Determina se una proprietà deve essere **scartata** perché è decorata con `[ForeignKey(nameof(...))]`.
        /// </summary>
        private static bool ShouldSkipForeignKeyReference(
            PropertyInfo property,
            Dictionary<PropertyInfo, string?> propertyToFkTarget)
        {
            if (propertyToFkTarget[property] != null)
            {
                Console.WriteLine($"❌ Scartata '{property.Name}' perché decorata con [ForeignKey]");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determina se una proprietà è un tipo complesso e deve essere navigata ricorsivamente.
        /// </summary>
        private static bool IsComplexType(PropertyInfo property, Type entityType)
        {
            return property.PropertyType.Namespace == entityType.Namespace
                   && !property.PropertyType.IsEnum
                   && !property.PropertyType.IsPrimitive
                   && property.PropertyType != typeof(string);
        }

        /// <summary>
        /// Crea un <see cref="TargetPropertyDef"/> per le proprietà primitive (inclusi gli ID Foreign Key).
        /// </summary>
        private static void TryCreateTargetPropertyDef(
            List<TargetPropertyDef> definitions,
            PropertyInfo property,
            int targetDbContextDefId,
            string rootClass,
            string parentPath)
        {
            var newDef = new TargetPropertyDef(new TargetPropertyDefValidator())
            {
                TargetDbContextDefId = targetDbContextDefId,
                Namespace = property.DeclaringType?.Namespace ?? "",
                RootClass = rootClass,
                Path = parentPath ?? "",
                Name = property.Name
            };

            try
            {
                newDef.EnsureValid();
                definitions.Add(newDef);
                Console.WriteLine($"✅ Aggiunta proprietà: {newDef.FullyQualifiedPropertyName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ TargetPropertyDef non valido: {newDef.FullyQualifiedPropertyName}");
                Console.WriteLine($"   🛑 Errore: {ex.Message}");
            }
        }
    }
}