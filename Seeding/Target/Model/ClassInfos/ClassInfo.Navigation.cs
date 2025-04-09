using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Navigation.Helpers;

namespace JsonBridgeEF.Seeding.Target.Model.ClassInfos
{
    /// <summary>
    /// Partial class di <see cref="ClassInfo"/> dedicata alla navigazione dei nodi genitori.
    /// </summary>
    internal sealed partial class ClassInfo
    {
        // --- Campo di Navigazione ---

        /// <summary>
        /// Gestore interno della navigazione topologica per la gestione dei genitori.
        /// </summary>
        private readonly ParentNavigationManager<ClassInfo, ClassProperty> _parentManager;

        // --- Proprietà di Navigazione ---

        /// <inheritdoc />
        public IReadOnlyCollection<ClassInfo> Parents => _parentManager.Parents;

        /// <inheritdoc />
        public bool IsRoot => _parentManager.IsRoot;

        /// <inheritdoc />
        public void AddParent(ClassInfo parent)
        {
            _parentManager.AddParent(parent);
            this.Touch(); // aggiorna UpdatedAt
        }

        /// <inheritdoc />
        protected override void OnBeforeExecution(ClassInfo child)
        {
            // Aggiunge il nodo corrente come genitore del figlio
            _parentManager.AddParent(child);
        }

        /// <inheritdoc />
        protected override void OnAfterExecution(ClassInfo child)
        {
            this.Touch(); // aggiorna UpdatedAt
        }
    }
}