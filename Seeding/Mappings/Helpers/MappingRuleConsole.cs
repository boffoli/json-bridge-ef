using JsonBridgeEF.Seeding.Mapping.Models;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Seeding.Mapping.Helpers
{
    /// <summary>
    /// Classe helper per la definizione interattiva delle regole di mapping tra campi JSON e proprietà target del database.
    /// L'utente seleziona il JSON field da associare a ciascuna proprietà target, creando dinamicamente una regola di mapping.
    /// </summary>
    internal static class MappingRuleConsole
    {
        /// <summary>
        /// Avvia una sessione interattiva per definire una regola di mapping tra un campo JSON e una proprietà target.
        /// Mostra all'utente l'elenco dei campi JSON disponibili e gli permette di selezionarne uno per la proprietà target specificata.
        /// </summary>
        /// <param name="jsonFields">Lista dei campi JSON disponibili per il mapping.</param>
        /// <param name="targetProp">Proprietà target del database per la quale creare il mapping.</param>
        /// <param name="mappingProject">Progetto di mapping corrente in cui registrare la regola.</param>
        /// <returns>Un oggetto <see cref="MappingRule"/> contenente la regola creata oppure <c>null</c> se l'utente non seleziona un mapping.</returns>
        public static MappingRule? PromptMappingRuleForProperty(
            List<JsonField> jsonFields,
            TargetProperty targetProp,
            MappingProject mappingProject)
        {
            Console.WriteLine();
            Console.WriteLine($"🔗 Definizione del mapping per la proprietà target:");
            Console.WriteLine($"   {targetProp.Namespace}.{targetProp.RootClass}.{targetProp.Path}{targetProp.Name}");
            Console.WriteLine("📜 Campi JSON disponibili per il mapping:");

            // Mostra l'elenco dei JSON fields disponibili (usando il nome)
            for (int i = 0; i < jsonFields.Count; i++)
            {
                var jsonField = jsonFields[i];
                Console.WriteLine($"  {i + 1}. {jsonField.Name}");
            }

            // Richiesta input all'utente
            Console.Write("✏️ Seleziona il numero del campo JSON da associare (Invio per saltare): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("⏩ Nessuna selezione effettuata. Mapping saltato.");
                return null;
            }

            // Validazione dell'input
            if (!int.TryParse(input, out int selection) || selection < 1 || selection > jsonFields.Count)
            {
                Console.WriteLine("⚠️ Input non valido. Mapping non creato per questa proprietà.");
                return null;
            }

            // Creazione della regola di mapping con il campo JSON selezionato
            var selectedJsonField = jsonFields[selection - 1];
            var newRule = new MappingRule(new MappingRuleValidator())
            {
                MappingProjectId = mappingProject.Id,
                JsonFieldId = selectedJsonField.Id,
                TargetPropertyId = targetProp.Id,
                JsFormula = "" // JS formula vuota per ora
            };

            Console.WriteLine($"✅ Regola creata con successo:");
            Console.WriteLine($"   {targetProp.Namespace}.{targetProp.RootClass}.{targetProp.Path}.{targetProp.Name} <-- {selectedJsonField.Name}");

            return newRule;
        }
    }
}