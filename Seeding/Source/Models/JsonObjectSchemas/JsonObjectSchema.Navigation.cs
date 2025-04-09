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

        // --- Metodo per aggiungere un nodo genitore ---

        /// <inheritdoc />
        public void AddParent(JsonObjectSchema parent)
        {
            _parentManager.AddParent(parent);
            this.Touch();
        }
    }
}