using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Seeding.Mappings.MappingConfigurations.Model;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Model;
using JsonBridgeEF.Seeding.Mappings.MappingRules.Validators;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Helpers
{
    /// <summary>
    /// Domain Helper: Permette la definizione interattiva delle regole di mapping tra proprietà JSON e proprietà delle classi target.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Consente la creazione guidata di associazioni tra proprietà JSON e proprietà del modello di destinazione (classi EF).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Utilizzato in fase di configurazione manuale tramite console o interfaccia CLI.</para>
    /// <para><b>Constraints:</b><br/>
    /// Richiede una lista di proprietà JSON valide e una proprietà di destinazione non nulla.</para>
    /// <para><b>Relationships:</b><br/>
    /// Crea istanze di <see cref="MappingRule"/> che vengono registrate all'interno di una <see cref="MappingConfiguration"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Da usare per la configurazione iniziale delle mappature prima della generazione del codice.</para>
    /// </remarks>
    internal static class MappingRuleConsole
    {
        /// <summary>
        /// Avvia una sessione interattiva per definire una regola di mapping tra un campo JSON e una proprietà target.
        /// </summary>
        /// <param name="jsonProperties">Lista delle proprietà JSON disponibili per il mapping.</param>
        /// <param name="classProp">Proprietà target per la quale definire il mapping.</param>
        /// <param name="mappingConfiguration">Configurazione di mapping corrente in cui registrare la regola.</param>
        /// <returns>Una nuova istanza di <see cref="MappingRule"/> o <c>null</c> se l'utente annulla.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Lista di proprietà JSON non nulla; proprietà target valida.</para>
        /// <para><b>Postconditions:</b> Una regola di mapping viene creata e validata, oppure l'operazione viene annullata.</para>
        /// <para><b>Side Effects:</b> Scrittura su console; creazione di una nuova entità di mapping.</para>
        /// </remarks>
        public static MappingRule? PromptMappingRuleForProperty(
            List<JsonProperty> jsonProperties,
            ClassProperty classProp,
            MappingConfiguration mappingConfiguration)
        {
            Console.WriteLine();
            Console.WriteLine($"🔗 Definizione del mapping per la proprietà:");
            Console.WriteLine($"   {classProp.}.{classProp.RootClass}.{classProp.Path}{classProp.Name}");
            Console.WriteLine("📜 Campi JSON disponibili per il mapping:");

            for (int i = 0; i < jsonProperties.Count; i++)
            {
                var jsonField = jsonProperties[i];
                Console.WriteLine($"  {i + 1}. {jsonField.Name}");
            }

            Console.Write("✏️ Seleziona il numero del campo JSON da associare (Invio per saltare): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("⏩ Nessuna selezione effettuata. Mapping saltato.");
                return null;
            }

            if (!int.TryParse(input, out int selection) || selection < 1 || selection > jsonProperties.Count)
            {
                Console.WriteLine("⚠️ Input non valido. Mapping non creato per questa proprietà.");
                return null;
            }

            var selectedJsonField = jsonProperties[selection - 1];

            var newRule = new MappingRule(
                description: $"{classProp.Namespace}.{classProp.RootClass}.{classProp.Path}{classProp.Name} mapped to {selectedJsonField.Name}",
                jsonEntityId: selectedJsonField.JsonEntityId,
                jsonPropertyId: selectedJsonField.Id,
                classInfoId: classProp.ClassInfoId,
                classPropertyId: classProp.Id,
                jsFormula: null,
                validator: new MappingRuleValidator()
            );

            Console.WriteLine($"✅ Regola creata con successo:");
            Console.WriteLine($"   {classProp.Namespace}.{classProp.RootClass}.{classProp.Path}{classProp.Name} <-- {selectedJsonField.Name}");

            return newRule;
        }
    }
}