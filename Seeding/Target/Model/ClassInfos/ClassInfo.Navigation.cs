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
        private readonly ParentNavigationManager<ClassInfo, ClassProperty> _parentManager;

        // --- Proprietà di Navigazione ---
        /// <inheritdoc />
        public IReadOnlyCollection<ClassInfo> Parents => _parentManager.Parents;

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

        /// <inheritdoc />
        public void AddParent(ClassInfo parent)
        {
            _parentManager.AddParent(parent);
        }
    }
}