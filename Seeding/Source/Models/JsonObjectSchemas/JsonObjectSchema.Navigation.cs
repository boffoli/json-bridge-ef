using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Shared.Navigation.Helpers;

namespace JsonBridgeEF.Seeding.Source.Model.JsonObjectSchemas
{
    /// <inheritdoc cref="IParentNavigableNode{TSelf}"/>
    /// <summary>
    /// Partial class di <see cref="JsonObjectSchema"/> responsabile della navigazione topologica verso i nodi genitori.
    /// </summary>
    internal sealed partial class JsonObjectSchema
    {
        // --- Campo di Navigazione ---
        
        /// <summary>
        /// Gestore interno della navigazione topologica per la gestione dei genitori.
        /// </summary>
        private readonly ParentNavigationManager<JsonObjectSchema, JsonProperty> _parentManager;

        // --- Proprietà di Navigazione ---

        /// <inheritdoc />
        public IReadOnlyCollection<JsonObjectSchema> Parents => _parentManager.Parents;

        /// <inheritdoc />
        public bool IsRoot => _parentManager.IsRoot;

        /// <summary>
        /// Configura e inizializza il ParentNavigationManager centralizzando la configurazione dei delegati.
        /// Questa configurazione definisce la logica che verrà eseguita nei vari hook.
        /// </summary>
        private void InitializeNavigation()
        {
            // Delegate personalizzato per OnAfterAddParentFlow:
            // Qui si richiama un metodo esistente (es. Touch) che non necessita di ricevere il parametro.
            _parentManager.OnAfterAddParentFlow = _ => this.Touch();
        }
        
        // --- Metodo per aggiungere un nodo genitore ---

        /// <inheritdoc />
        public void AddParent(JsonObjectSchema parent)
        {
            _parentManager.AddParent(parent);
            this.Touch();
        }
    }
}