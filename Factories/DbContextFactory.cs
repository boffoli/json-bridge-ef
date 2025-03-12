using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace JsonBridgeEF.Factories
{
    /// <summary>
    /// Factory per la creazione di DbContext basata sul nome del DbContext.
    /// </summary>
    public static class DbContextFactory
    {
        /// <summary>
        /// Crea un'istanza del DbContext specificato dal suo nome.
        /// </summary>
        /// <param name="dbContextName">Il nome del DbContext (es. "TargetDbContext").</param>
        /// <returns>Un'istanza del DbContext.</returns>
        /// <exception cref="InvalidOperationException">
        /// Se non viene trovato un DbContext con il nome specificato o se non è possibile crearne un'istanza.
        /// </exception>
        public static DbContext CreateDbContext(string dbContextName)
        {
            // Cerca nel assembly corrente un tipo che eredita da DbContext e corrisponde al nome fornito.
            var dbContextType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t) &&
                                     t.Name.Equals(dbContextName, StringComparison.OrdinalIgnoreCase));

            if (dbContextType == null)
            {
                throw new InvalidOperationException($"DbContext with name '{dbContextName}' not found.");
            }

            // Crea un'istanza del DbContext usando il costruttore parameterless.
            var instance = Activator.CreateInstance(dbContextType) as DbContext;
            if (instance == null)
            {
                throw new InvalidOperationException($"Could not create an instance of DbContext '{dbContextName}'.");
            }

            return instance;
        }
    }
}