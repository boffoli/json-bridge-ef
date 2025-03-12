using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.Helpers
{
    /// <summary>
    /// Provides static methods for introspection of the target model
    /// and for evaluating the validity of a <see cref="TargetPropertyDef"/>.
    /// </summary>
    public static class TargetModelInspector
    {
        /// <summary>
        /// Retrieves a type given its full name (namespace + class name).
        /// </summary>
        /// <param name="fullTypeName">The full name of the type.</param>
        /// <returns>The <see cref="Type"/> if found, otherwise <see langword="null"/>.</returns>
        public static Type? GetTypeByName(string fullTypeName)
        {
            return Type.GetType(fullTypeName);
        }

        /// <summary>
        /// Determines whether a type with the given full name exists.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type.</param>
        /// <returns><see langword="true"/> if the type exists, otherwise <see langword="false"/>.</returns>
        public static bool TypeExists(string fullTypeName)
        {
            return GetTypeByName(fullTypeName) != null;
        }

        /// <summary>
        /// Determines whether a given namespace exists in the loaded assemblies.
        /// </summary>
        /// <param name="namespaceValue">The namespace to check.</param>
        /// <returns><see langword="true"/> if at least one type belongs to the given namespace, otherwise <see langword="false"/>.</returns>
        public static bool NamespaceExists(string namespaceValue)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Any(t => t.Namespace == namespaceValue);
        }

        /// <summary>
        /// Determines whether a public property exists within a specified type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="propertyName">The name of the property to search for.</param>
        /// <returns><see langword="true"/> if the property exists, otherwise <see langword="false"/>.</returns>
        public static bool PropertyExistsInType(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) != null;
        }

        /// <summary>
        /// Resolves the final type along the specified path, starting from the given type.
        /// </summary>
        /// <param name="startingType">The root type from which to start navigation.</param>
        /// <param name="typePath">The navigation path as a dot-separated string (e.g., "Address.City.Street").</param>
        /// <returns>The <see cref="Type"/> at the end of the path, or <see langword="null"/> if the path is invalid.</returns>
        public static Type? GetFinalTypeFromPath(Type startingType, string? typePath)
        {
            if (string.IsNullOrWhiteSpace(typePath))
                return startingType;

            string[] pathSegments = typePath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            return NavigateTypePath(startingType, pathSegments, 0);
        }

        /// <summary>
        /// Retrieves the <see cref="PropertyInfo"/> of the specified property within a given type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The <see cref="PropertyInfo"/> if found, otherwise <see langword="null"/>.</returns>
        public static PropertyInfo? GetPropertyInfo(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Determines whether a type contains at least one property with the <see cref="KeyAttribute"/>.
        /// </summary>
        /// <param name="type">The type to analyze.</param>
        /// <returns><see langword="true"/> if the type contains at least one property with <see cref="KeyAttribute"/>, otherwise <see langword="false"/>.</returns>
        public static bool HasKeyAttribute(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Any(p => p.GetCustomAttribute<KeyAttribute>() != null);
        }

        /// <summary>
        /// Recursively navigates through the segments of a type path to resolve the final type.
        /// </summary>
        /// <param name="currentType">The current type being examined.</param>
        /// <param name="pathSegments">An array of type names forming the navigation path.</param>
        /// <param name="index">The current position in the <paramref name="pathSegments"/> array.</param>
        /// <returns>The <see cref="Type"/> at the end of the path, or <see langword="null"/> if the path is invalid.</returns>
        private static Type? NavigateTypePath(Type currentType, string[] pathSegments, int index)
        {
            if (index >= pathSegments.Length)
                return currentType;

            PropertyInfo? prop = currentType.GetProperty(pathSegments[index], BindingFlags.Public | BindingFlags.Instance);
            return prop != null ? NavigateTypePath(prop.PropertyType, pathSegments, index + 1) : null;
        }
    }
}