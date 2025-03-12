using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JsonBridgeEF.Helpers
{
    /// <summary>
    /// Handler per l'ispezione del modello configurato da Entity Framework in un DbContext.
    /// Fornisce metodi per ottenere l'elenco dei tipi (modelli) registrati e delle loro proprietà pubbliche,
    /// oltre a ispezionare lo schema reale del database.
    /// </summary>
    /// <remarks>
    /// Inizializza l'inspector con il DbContext specificato.
    /// </remarks>
    /// <param name="dbContext">Il contesto del database da analizzare.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal class DbContextInspector(DbContext dbContext)
    {
        private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        /// <summary>
        /// Stampa lo schema del modello Entity Framework, elencando per ogni modello registrato il suo fully qualified name e le sue proprietà.
        /// Include anche proprietà annidate.
        /// </summary>
        public void PrintDbContextModel()
        {
            var schema = GetModelsAndProperties();
            Console.WriteLine("\n📌 **Modello del DbContext (EF Core)**");
            foreach (var model in schema)
            {
                Console.WriteLine($"\n🔹 Modello: {model.Key}");
                foreach (var property in model.Value)
                {
                    Console.WriteLine($"   ➜ {property}");
                }
            }
        }

        /// <summary>
        /// Stampa lo schema effettivo del database, elencando le tabelle e le colonne con i loro tipi di dati.
        /// (Questa implementazione è specifica per SQLite; modifica la query se usi un altro DB.)
        /// </summary>
        public void PrintDatabaseSchema()
        {
            using var connection = _dbContext.Database.GetDbConnection();
            connection.Open();

            Console.WriteLine("\n📌 **Schema delle Tabelle nel Database**");

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT name FROM sqlite_master 
                WHERE type='table' 
                ORDER BY name;
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"\n🔹 Tabella: {reader.GetString(0)}");

                using var columnCommand = connection.CreateCommand();
                columnCommand.CommandText = $"PRAGMA table_info({reader.GetString(0)});";

                using var columnReader = columnCommand.ExecuteReader();
                while (columnReader.Read())
                {
                    string columnName = columnReader.GetString(1);
                    string columnType = columnReader.GetString(2);
                    Console.WriteLine($"   ➜ {columnName} ({columnType})");
                }
            }
        }

        /// <summary>
        /// Ottiene i modelli e le relative proprietà configurati nel DbContext.
        /// Restituisce un dizionario in cui la chiave è il fully qualified name del modello e il valore è la lista delle proprietà.
        /// </summary>
        /// <returns>Un dizionario contenente i modelli e le loro proprietà.</returns>
        private Dictionary<string, List<string>> GetModelsAndProperties()
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var modelType in _dbContext.Model.GetEntityTypes())
            {
                if (modelType.IsOwned()) // Esclude i modelli configurati come "owned"
                    continue;

                // Utilizza il fully qualified name del modello (o il nome se FullName non è disponibile)
                string modelQualifiedName = modelType.ClrType.FullName ?? modelType.ClrType.Name;
                var propertyNames = new List<string>();

                ExploreProperties(modelType.ClrType, propertyNames, string.Empty);
                result[modelQualifiedName] = propertyNames;
            }

            return result;
        }

        /// <summary>
        /// Esplora ricorsivamente le proprietà di un modello, aggiungendo le proprietà elementari e navigando nei tipi complessi.
        /// Supporta anche le collezioni generiche.
        /// </summary>
        /// <param name="type">Il tipo del modello da esplorare.</param>
        /// <param name="properties">La lista delle proprietà (in formato stringa) da aggiornare.</param>
        /// <param name="parentPrefix">Prefisso per proprietà annidate (es. "Address.").</param>
        private static void ExploreProperties(Type type, List<string> properties, string parentPrefix)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Type propertyType = property.PropertyType;
                string propertyName = string.IsNullOrEmpty(parentPrefix)
                    ? property.Name
                    : $"{parentPrefix}.{property.Name}";

                if (IsPrimitiveType(propertyType))
                {
                    properties.Add(propertyName);
                }
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType)
                         && propertyType.IsGenericType
                         && propertyType.GetGenericArguments().Length == 1
                         && !IsPrimitiveType(propertyType.GetGenericArguments()[0]))
                {
                    // Gestisce collezioni di oggetti complessi
                    var itemType = propertyType.GetGenericArguments()[0];
                    properties.Add(propertyName + "[]");
                    ExploreProperties(itemType, properties, propertyName + "[*]");
                }
                else
                {
                    ExploreProperties(propertyType, properties, propertyName);
                }
            }
        }

        /// <summary>
        /// Determina se un tipo è considerato "primitivo" o assimilabile a un primitivo.
        /// Include: tipi primitivi, string, decimal, DateTime, DateTimeOffset, TimeSpan, ecc.
        /// </summary>
        private static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(TimeSpan);
        }
    }
}