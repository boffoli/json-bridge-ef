using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Models
{
    internal partial class InstancePropertyProxy
    {
        // 🔹 UTILITIES

        #region Navigation
        /// <summary>
        /// Naviga attraverso proprietà annidate per ottenere l'oggetto target e la proprietà finale.
        /// </summary>
        private static (object? TargetInstance, PropertyInfo FinalProperty) NavigateToProperty(
            object rootInstance, string propertyPath, bool createMissingInstances)
        {
            if (rootInstance == null) throw new ArgumentNullException(nameof(rootInstance));
            if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentException("Il percorso non può essere vuoto.", nameof(propertyPath));

            string[] segments = propertyPath.Split('.');
            object? current = rootInstance;
            Type currentType = rootInstance.GetType();

            for (int i = 0; i < segments.Length; i++)
            {
                string name = segments[i];
                PropertyInfo? prop = currentType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new ArgumentException($"Property '{name}' non trovata in '{currentType.FullName}'.");

                if (i == segments.Length - 1)
                {
                    return (current, prop);
                }

                object? next = prop.GetValue(current);
                if (next == null)
                {
                    if (createMissingInstances)
                    {
                        next = Activator.CreateInstance(prop.PropertyType)
                            ?? throw new InvalidOperationException($"Impossibile creare istanza di '{prop.PropertyType.FullName}'.");
                        prop.SetValue(current, next);
                    }
                    else
                    {
                        return (null, prop);
                    }
                }

                current = next;
                currentType = prop.PropertyType;
            }

            throw new InvalidOperationException($"Navigazione impossibile per il percorso '{propertyPath}'.");
        }
        #endregion

        #region TypeResolution & PrimaryKey
        /// <summary>
        /// Risolve un tipo o una proprietà da nome completo.
        /// </summary>
        public static Type GetTypeByName(string fullyQualifiedName)
        {
            var type = Type.GetType(fullyQualifiedName);
            if (type != null)
                return type;

            int idx = fullyQualifiedName.LastIndexOf('.');
            if (idx > 0)
            {
                var className = fullyQualifiedName.Substring(0, idx);
                var propName = fullyQualifiedName.Substring(idx + 1);
                var classType = GetTypeByName(className);
                var propInfo = classType.GetProperty(propName);
                if (propInfo != null)
                    return propInfo.PropertyType;
            }

            throw new ArgumentException($"Type or Property '{fullyQualifiedName}' not found.", nameof(fullyQualifiedName));
        }

        /// <summary>
        /// Trova la proprietà marcata con [Key] nell'istanza.
        /// </summary>
        private PropertyInfo FindPrimaryKeyProperty()
        {
            var props = _instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var keyProps = new List<PropertyInfo>();
            foreach (var p in props)
            {
                if (p.GetCustomAttribute<KeyAttribute>() != null)
                    keyProps.Add(p);
            }

            return keyProps.Count switch
            {
                0 => throw new InvalidOperationException($"No primary key found in '{_instanceType.Name}'."),
                > 1 => throw new InvalidOperationException($"Multiple primary keys found in '{_instanceType.Name}'."),
                _ => keyProps[0]
            };
        }
        #endregion

        #region SampleGeneration
        /// <summary>
        /// Genera un valore di esempio per una proprietà basata sul nome completo.
        /// </summary>
        public static object GenerateSampleValue(string fullyQualifiedPropertyName)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedPropertyName))
                throw new ArgumentException("Il nome della proprietà non può essere nullo o vuoto.", nameof(fullyQualifiedPropertyName));

            Type propertyType = GetTypeByName(fullyQualifiedPropertyName);
            Type? underlying = Nullable.GetUnderlyingType(propertyType);
            if (underlying != null)
                propertyType = underlying;

            if (propertyType.IsPrimitive || propertyType == typeof(string) || propertyType == typeof(decimal))
            {
                return propertyType switch
                {
                    _ when propertyType == typeof(int) => new Random().Next(1, 1000),
                    _ when propertyType == typeof(long) => (long)new Random().Next(1, 100000),
                    _ when propertyType == typeof(bool) => true,
                    _ when propertyType == typeof(double) => 3.14,
                    _ when propertyType == typeof(float) => 1.23f,
                    _ when propertyType == typeof(decimal) => 99.99m,
                    _ when propertyType == typeof(string) => $"sample_{Guid.NewGuid():N}",
                    _ => throw new NotSupportedException($"Tipo '{propertyType.Name}' non supportato per valori di esempio.")
                };
            }

            var proxy = new InstancePropertyProxy(propertyType.FullName!);
            return proxy.GetInstance();
        }
        #endregion

        #region DebugPrint
        /// <summary>
        /// Stampa l'intera istanza e le sue proprietà ricorsivamente (debug).
        /// </summary>
        public void PrintInstance()
        {
            Console.WriteLine("\n🔍 Stampa dell'istanza:");
            Console.WriteLine($"\n📦 Classe: {ClassName}");
            Console.WriteLine($"   🆔 Pk: {Pk ?? "null"}");

            foreach (var propName in GetPropertyNames())
            {
                var value = this[propName];
                PrintProperty(propName, value, 1);
            }
        }

        /// <summary>
        /// Stampa una singola proprietà ricorsivamente a partire dal valore (debug).
        /// </summary>
        private static void PrintProperty(string propertyName, object? value, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4);
            if (value is null)
            {
                Console.WriteLine($"{indent}📌 {propertyName}: null");
                return;
            }

            var valueType = value.GetType();
            if (valueType.IsPrimitive || valueType == typeof(string) || valueType == typeof(decimal))
            {
                Console.WriteLine($"{indent}📌 {propertyName}: {value}");
                return;
            }

            Console.WriteLine($"{indent}📌 {propertyName}: {valueType.FullName}");
            foreach (var prop in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                var propValue = prop.GetValue(value);
                PrintProperty($"{propertyName}.{prop.Name}", propValue, indentLevel + 1);
            }
        }
        #endregion
    }
}
