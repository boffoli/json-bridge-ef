using System.Reflection;

namespace JsonBridgeEF.Importing.IntancePropertyProxing.Models
{
    internal partial class InstancePropertyProxy
    {

        /// <summary>
        /// Nome della classe dell'istanza.
        /// </summary>
        public string ClassName => _instanceType.Name;

        /// <summary>
        /// Proprietà per leggere/scrivere la chiave primaria.
        /// </summary>
        public object? Pk
        {
            get => _primaryKeyProperty.GetValue(_instance);
            set => _primaryKeyProperty.SetValue(_instance, value);
        }

        /// <summary>
        /// Indexer per leggere/scrivere percorsi di proprietà (anche annidati).
        /// </summary>
        public object? this[string propertyPath]
        {
            get
            {
                var (target, prop) = NavigateToProperty(
                    _instance, propertyPath, createMissingInstances: false);
                return target == null ? null : prop.GetValue(target);
            }
            set
            {
                var (target, prop) = NavigateToProperty(
                    _instance, propertyPath, createMissingInstances: true);
                if (target != null && prop.SetMethod?.IsPublic == true)
                    prop.SetValue(target, value);
            }
        }

        /// <summary>
        /// Restituisce il tipo della proprietà indicata.
        /// </summary>
        public Type GetPropertyType(string propertyPath)
        {
            var (_, prop) = NavigateToProperty(
                _instance, propertyPath, createMissingInstances: false);
            return prop.PropertyType;
        }

        /// <summary>
        /// Elenca i nomi di tutte le proprietà pubbliche dell'istanza.
        /// </summary>
        public IEnumerable<string> GetPropertyNames() =>
            _instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Select(p => p.Name);

        /// <summary>
        /// Verifica se una proprietà esiste.
        /// </summary>
        public bool PropertyExists(string propertyPath)
        {
            try
            {
                var (inst, prop) = NavigateToProperty(
                    _instance, propertyPath, createMissingInstances: false);
                return inst != null && prop != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se la proprietà è scrivibile.
        /// </summary>
        public bool IsPropertyWritable(string propertyPath)
        {
            var (_, prop) = NavigateToProperty(
                _instance, propertyPath, createMissingInstances: false);
            return prop?.SetMethod?.IsPublic == true;
        }

        /// <summary>
        /// Restituisce gli attributi custom applicati alla proprietà.
        /// </summary>
        public IEnumerable<Attribute> GetPropertyAttributes(string propertyPath)
        {
            var (_, prop) = NavigateToProperty(
                _instance, propertyPath, createMissingInstances: false);
            return prop.GetCustomAttributes();
        }

        /// <summary>
        /// Verifica se la proprietà è una collezione (escluse stringhe).
        /// </summary>
        public bool IsPropertyCollection(string propertyPath)
        {
            var (_, prop) = NavigateToProperty(
                _instance, propertyPath, createMissingInstances: false);
            var type = prop.PropertyType;
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
                   && type != typeof(string);
        }

        /// <summary>
        /// Verifica se la proprietà è nullable.
        /// </summary>
        public bool IsPropertyNullable(string propertyPath)
        {
            var (_, prop) = NavigateToProperty(
                _instance, propertyPath, createMissingInstances: false);
            var type = prop.PropertyType;
            if (!type.IsValueType) return true;
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Tentativo di lettura della proprietà senza eccezioni.
        /// </summary>
        public bool TryGetProperty(string propertyPath, out object? value)
        {
            try
            {
                value = this[propertyPath];
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Tentativo di scrittura della proprietà senza eccezioni.
        /// </summary>
        public bool TrySetProperty(string propertyPath, object? value)
        {
            try
            {
                this[propertyPath] = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}