using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JsonBridgeEF.Importing.IntancePropertyProxing.Helpers;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Models
{
    /// <summary>
    /// Classe per accedere dinamicamente a un'istanza di una classe e alle sue proprietà, inclusa la chiave primaria.
    /// </summary>
    internal class InstancePropertyProxy
    {
        private readonly object _instance;
        private readonly Type _instanceType;
        private readonly PropertyInfo _primaryKeyProperty;

        // ===========================================================
        // 🔹 COSTRUTTORE & INIZIALIZZAZIONE
        // ===========================================================

        public InstancePropertyProxy(string fullyQualifiedClassName)
        {
            _instanceType = GetTypeByName(fullyQualifiedClassName)
                ?? throw new ArgumentException($"Type '{fullyQualifiedClassName}' not found.", nameof(fullyQualifiedClassName));

            _instance = Activator.CreateInstance(_instanceType)
                ?? throw new InvalidOperationException($"Failed to create instance of '{fullyQualifiedClassName}'.");

            _primaryKeyProperty = GetPrimaryKeyProperty();
        }

        public object GetInstance() => _instance;

        // ===========================================================
        // 🔹 GESTIONE DELLE PROPRIETÀ
        // ===========================================================

        public object? Pk
        {
            get => _primaryKeyProperty.GetValue(_instance);
            set => _primaryKeyProperty.SetValue(_instance, value);
        }

        public object? this[string propertyPath]
        {
            get
            {
                var (targetInstance, finalProperty) = InstancePropertyProxyHelper.NavigateToProperty(_instance, propertyPath, createMissingInstances: false);
                return targetInstance == null ? null : finalProperty.GetValue(targetInstance);
            }
            set
            {
                var (targetInstance, finalProperty) = InstancePropertyProxyHelper.NavigateToProperty(_instance, propertyPath, createMissingInstances: true);
                targetInstance?.GetType().GetProperty(finalProperty.Name)?.SetValue(targetInstance, value);
            }
        }

        public Type GetPropertyType(string propertyPath)
        {
            var (_, finalProperty) = InstancePropertyProxyHelper.NavigateToProperty(_instance, propertyPath, createMissingInstances: false);
            return finalProperty.PropertyType;
        }

        public string GetClassName() => _instanceType.Name;

        public IEnumerable<string> GetPropertyNames()
        {
            return _instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
        }

        private PropertyInfo GetPrimaryKeyProperty()
        {
            var keyProperties = _instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null)
                .ToList();

            return keyProperties.Count switch
            {
                0 => throw new InvalidOperationException($"No primary key found in '{_instanceType.Name}'."),
                > 1 => throw new InvalidOperationException($"Multiple primary keys found in '{_instanceType.Name}'."),
                _ => keyProperties[0]
            };
        }

        public static Type GetTypeByName(string fullyQualifiedName)
        {
            Type? type = Type.GetType(fullyQualifiedName);
            if (type != null) return type;

            int lastDotIndex = fullyQualifiedName.LastIndexOf('.');
            if (lastDotIndex > 0)
            {
                string className = fullyQualifiedName[..lastDotIndex];
                string propertyName = fullyQualifiedName[(lastDotIndex + 1)..];

                Type? classType = GetTypeByName(className);
                PropertyInfo? propertyInfo = classType?.GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    return propertyInfo.PropertyType;
                }
            }

            throw new ArgumentException($"Type or Property '{fullyQualifiedName}' not found.", nameof(fullyQualifiedName));
        }

        // ===========================================================
        // 🛠️ GENERAZIONE DI DATI
        // ===========================================================

        /// <summary>
        /// Genera un valore di esempio per una proprietà basata sul suo tipo.
        /// </summary>
        /// <param name="fullyQualifiedPropertyName">Nome completo della proprietà.</param>
        /// <returns>Un valore di esempio appropriato al tipo della proprietà.</returns>
        public static object GenerateSampleValue(string fullyQualifiedPropertyName)
        {
            return InstancePropertyProxyHelper.GenerateSampleValue(fullyQualifiedPropertyName);
        }

        // ===========================================================
        // 🖨️ STAMPA & DEBUG
        // ===========================================================

        /// <summary>
        /// Stampa l'intera istanza e le sue proprietà ricorsivamente.
        /// </summary>
        public void PrintInstance()
        {
            InstancePropertyProxyHelper.PrintInstance(this);
        }

        /// <summary>
        /// Stampa una proprietà, esplorando gli oggetti annidati ricorsivamente.
        /// </summary>
        public static void PrintProperty(string propertyName, object? value, int indentLevel)
        {
            InstancePropertyProxyHelper.PrintProperty(propertyName, value, indentLevel);
        }
    }
}