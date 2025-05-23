using Jint;
using System.Text.RegularExpressions;

namespace JsonBridgeEF.Seeding.Mappings.MappingRules.Helpers
{
    /// <summary>
    /// Gestisce l'esecuzione di una formula JavaScript per trasformare valori.
    /// Normalizza qualsiasi input per assicurare che sia una funzione eseguibile.
    /// </summary>
    /// <typeparam name="TValue">Il tipo del valore trasformato.</typeparam>
    public partial class JintFormula<TValue>
    {
        private readonly Engine _engine;
        private readonly string _functionName;

        /// <summary>
        /// Regex utilizzata per estrarre la definizione di una funzione JavaScript.
        /// </summary>
        private readonly Regex FunctionRegex = MyRegex();

        /// <summary>
        /// Formula di default: restituisce il valore inalterato.
        /// Questa costante può essere modificata per cambiare il comportamento predefinito.
        /// </summary>
        private readonly string DefaultJsFormula = "function transform(value) { return 'ciao'; }";

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="JintFormula{TValue}"/> e compila la funzione JavaScript.
        /// Se la formula è nulla o vuota, viene utilizzata la formula di default.
        /// </summary>
        /// <param name="jsFormula">La stringa contenente la formula JavaScript.</param>
        public JintFormula(string? jsFormula)
        {
            jsFormula = string.IsNullOrWhiteSpace(jsFormula) ? DefaultJsFormula : jsFormula;

            // Estrai il nome della funzione e il corpo del codice
            (string functionName, string functionBody) = ParseFunction(jsFormula);

            _functionName = functionName;
            string wrappedFunction = $"function {functionName}(value) {{ {functionBody} }}";

            _engine = new Engine();
            _engine.Execute(wrappedFunction);
        }

        /// <summary>
        /// Applica la trasformazione definita nello script JavaScript all'input fornito.
        /// </summary>
        /// <param name="inputValue">Il valore da trasformare.</param>
        public TValue Compute(TValue inputValue)
        {
            try
            {
                var result = _engine.Invoke(_functionName, inputValue);
                object? resultObj = result.ToObject();

                if (resultObj == null)
                {
                    Console.WriteLine($"⚠️ La funzione '{_functionName}' ha restituito `null`. Restituzione del valore originale.");
                    return inputValue; // Se la funzione JS restituisce null, usa il valore originale
                }

                return resultObj is TValue castedResult
                    ? castedResult
                    : (TValue)Convert.ChangeType(resultObj, typeof(TValue));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Errore nella trasformazione JavaScript: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Estrae il nome della funzione e il corpo del codice dallo script JavaScript fornito.
        /// Se non esiste una funzione esplicita, lo racchiude in una funzione generata dinamicamente.
        /// </summary>
        /// <param name="jsFormula">La formula JavaScript originale.</param>
        private (string functionName, string functionBody) ParseFunction(string jsFormula)
        {
            var match = FunctionRegex.Match(jsFormula);
            if (match.Success)
            {
                string extractedName = match.Groups["name"].Value.Trim();
                string extractedBody = match.Groups["body"].Value.Trim();
                return (extractedName, extractedBody);
            }

            // Se non è stata fornita una funzione valida, incapsula il codice in una funzione generata
            string autoGeneratedName = $"func_{Guid.NewGuid():N}";
            return (autoGeneratedName, $"return {jsFormula};");
        }

        [GeneratedRegex(@"function\s+(?<name>\w+)\s*\((?<params>[^)]*)\)\s*\{(?<body>[\s\S]*)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled, "it-IT")]
        private static partial Regex MyRegex();
    }
}