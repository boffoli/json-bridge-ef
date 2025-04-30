using System.Reflection;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Models
{
    internal partial class InstancePropertyProxy
    {
        /// <summary>
        /// Verifica l'esistenza di un metodo pubblico (istanza o statico).
        /// </summary>
        public bool MethodExists(string methodName, bool includeStatic = false)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeStatic) flags |= BindingFlags.Static;
            return _instanceType.GetMethod(methodName, flags) != null;
        }

        /// <summary>
        /// Invoca un metodo pubblico di istanza.
        /// </summary>
        public object? InvokeMethod(string methodName, params object?[] parameters)
        {
            var method = _instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Method '{methodName}' not found on '{_instanceType.Name}'.");
            return method.Invoke(_instance, parameters);
        }

        /// <summary>
        /// Invoca un metodo pubblico di istanza senza eccezioni.
        /// </summary>
        public bool TryInvokeMethod(string methodName, object?[] parameters, out object? result)
        {
            try
            {
                result = InvokeMethod(methodName, parameters);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Restituisce il tipo di ritorno di un metodo pubblico di istanza.
        /// </summary>
        public Type GetMethodReturnType(string methodName)
        {
            var method = _instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Method '{methodName}' not found on '{_instanceType.Name}'.");
            return method.ReturnType;
        }

        /// <summary>
        /// Elenca i nomi di tutti i metodi pubblici (istanza e, se richiesto, statici).
        /// </summary>
        public IEnumerable<string> GetMethodNames(bool includeStatic = false)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeStatic) flags |= BindingFlags.Static;
            return _instanceType.GetMethods(flags).Select(m => m.Name);
        }

        /// <summary>
        /// Restituisce gli attributi custom applicati a un metodo.
        /// </summary>
        public IEnumerable<Attribute> GetMethodAttributes(string methodName)
        {
            var method = _instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return method?.GetCustomAttributes() ?? Enumerable.Empty<Attribute>();
        }

        /// <summary>
        /// Restituisce i parametri di un metodo.
        /// </summary>
        public ParameterInfo[] GetMethodParameters(string methodName)
        {
            var method = _instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return method?.GetParameters() ?? Array.Empty<ParameterInfo>();
        }

        /// <summary>
        /// Verifica se un metodo è statico.
        /// </summary>
        public bool IsMethodStatic(string methodName)
        {
            var method = _instanceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            return method?.IsStatic ?? false;
        }
    }
}