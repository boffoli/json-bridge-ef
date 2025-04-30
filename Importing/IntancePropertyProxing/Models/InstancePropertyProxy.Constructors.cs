using System;
using System.Reflection;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Models
{
    internal partial class InstancePropertyProxy
    {
        private readonly object _instance;
        private readonly Type _instanceType;
        private readonly PropertyInfo _primaryKeyProperty;

        /// <summary>
        /// Crea il proxy usando il costruttore pubblico senza parametri.
        /// </summary>
        public InstancePropertyProxy(string fullyQualifiedClassName)
            : this(fullyQualifiedClassName, [])
        {
        }

        /// <summary>
        /// Crea il proxy usando il costruttore pubblico con argomenti.
        /// </summary>
        public InstancePropertyProxy(string fullyQualifiedClassName, params object?[] constructorArgs)
        {
            _instanceType = GetTypeByName(fullyQualifiedClassName)
                ?? throw new ArgumentException($"Type '{fullyQualifiedClassName}' not found.", nameof(fullyQualifiedClassName));

            try
            {
                _instance = Activator.CreateInstance(_instanceType, constructorArgs)
                    ?? throw new InvalidOperationException($"Failed to create instance of '{fullyQualifiedClassName}'.");
            }
            catch (MissingMethodException ex)
            {
                throw new InvalidOperationException(
                    $"No matching constructor found on '{fullyQualifiedClassName}' for the provided arguments.", ex);
            }

            _primaryKeyProperty = FindPrimaryKeyProperty();
        }

        /// <summary>
        /// Restituisce l'istanza creata.
        /// </summary>
        public object GetInstance() => _instance;
    }
}