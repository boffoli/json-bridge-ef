using System.Reflection;
using JsonBridgeEF.Importing.IntancePropertyProxing.Models;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Helpers{
    /// <summary>
    /// Helper statico per la gestione delle operazioni di navigazione, generazione di dati e stampa per InstancePropertyProxy.
    /// </summary>
    internal static class InstancePropertyProxyHelper
    {
        // ===========================================================
        // 🔍 NAVIGAZIONE DEL MODELLO
        // ===========================================================

        /// <summary>
        /// Naviga attraverso proprietà annidate per ottenere l'oggetto target e la proprietà finale.
        /// </summary>
        public static (object? targetInstance, PropertyInfo finalProperty) NavigateToProperty(
            object rootInstance, string propertyPath, bool createMissingInstances)
        {
            ArgumentNullException.ThrowIfNull(rootInstance);
            if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentException("Property path cannot be null or empty.", nameof(propertyPath));

            string[] pathSegments = propertyPath.Split('.');
            object? currentInstance = rootInstance;
            Type currentType = rootInstance.GetType();

            for (int i = 0; i < pathSegments.Length; i++)
            {
                string segment = pathSegments[i];
                PropertyInfo? property = currentType.GetProperty(segment)
                    ?? throw new ArgumentException($"Property '{segment}' not found in '{currentType.Name}'.");

                if (i == pathSegments.Length - 1)
                {
                    return (currentInstance, property);
                }

                object? nextInstance = property.GetValue(currentInstance);

                if (nextInstance == null)
                {
                    if (createMissingInstances)
                    {
                        nextInstance = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(currentInstance, nextInstance);
                    }
                    else
                    {
                        return (null, property);
                    }
                }

                currentInstance = nextInstance;
                currentType = property.PropertyType;
            }

            throw new InvalidOperationException($"Could not navigate property path '{propertyPath}'.");
        }

        // ===========================================================
        // 🛠️ GENERAZIONE DI DATI
        // ===========================================================

        /// <summary>
        /// Genera un valore di esempio per un tipo specificato.
        /// </summary>
        public static object GenerateSampleValue(string fullyQualifiedPropertyName)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedPropertyName))
                throw new ArgumentException("Fully qualified property name cannot be null or empty.", nameof(fullyQualifiedPropertyName));

            Type propertyType = InstancePropertyProxy.GetTypeByName(fullyQualifiedPropertyName);

            if (Nullable.GetUnderlyingType(propertyType) is Type underlyingType)
            {
                propertyType = underlyingType;
            }

            if (propertyType.IsPrimitive || propertyType == typeof(string) || propertyType == typeof(decimal))
            {
                return propertyType switch
                {
                    Type t when t == typeof(int) => new Random().Next(1, 1000),
                    Type t when t == typeof(long) => (long)new Random().Next(1, 100000),
                    Type t when t == typeof(bool) => true,
                    Type t when t == typeof(double) => 3.14,
                    Type t when t == typeof(float) => 1.23f,
                    Type t when t == typeof(decimal) => 99.99m,
                    Type t when t == typeof(string) => $"sample_{Guid.NewGuid().ToString("N")[..6]}",
                    _ => throw new NotSupportedException($"Tipo '{propertyType.Name}' non supportato per valori di esempio.")
                };
            }

            var accessor = new InstancePropertyProxy(propertyType.FullName!);
            return accessor.GetInstance();
        }

        // ===========================================================
        // 🖨️ STAMPA E DEBUG
        // ===========================================================

        /// <summary>
        /// Stampa l'intera istanza e le sue proprietà ricorsivamente.
        /// </summary>
        public static void PrintInstance(InstancePropertyProxy instanceAccessor)
        {
            ArgumentNullException.ThrowIfNull(instanceAccessor);

            Console.WriteLine("\n🔍 Stampa dell'istanza:");

            // Stampa il nome della classe e la PK con protezione contro i valori null
            Console.WriteLine($"\n📦 Classe: {instanceAccessor.GetClassName()}");
            Console.WriteLine($"   🆔 Pk: {instanceAccessor.Pk ?? "null"}"); // Protezione contro il null

            // Stampa le proprietà dell'oggetto, ignorando il nome della classe per evitare duplicazioni
            foreach (var propertyName in instanceAccessor.GetPropertyNames())
            {
                object? value = instanceAccessor[propertyName]; // Accettiamo il valore nullable
                PrintProperty(propertyName, value, 1);
            }
        }

        /// <summary>
        /// Stampa una proprietà, esplorando gli oggetti annidati ricorsivamente.
        /// </summary>
        public static void PrintProperty(string propertyName, object? value, int indentLevel)
        {
            string indent = new(' ', indentLevel * 4);

            if (value is null)
            {
                Console.WriteLine($"{indent}📌 {propertyName}: null");
                return;
            }

            Type valueType = value.GetType();

            if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(decimal))
            {
                Console.WriteLine($"{indent}📌 {propertyName}: {value}");
            }
            else
            {
                Console.WriteLine($"{indent}📌 {propertyName}: {valueType.FullName}");

                foreach (var prop in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!prop.CanRead) continue;

                    object? propValue = prop.GetValue(value);
                    PrintProperty($"{propertyName}.{prop.Name}", propValue, indentLevel + 1);
                }
            }
        }
    }
}