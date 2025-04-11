namespace JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

/// <summary>
/// Entry point statico per configurare un flusso di esecuzione semantico vincolato su un target.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Utilizzare <c>For&lt;T&gt;()</c> per ottenere un costruttore con step vincolati.</para>
/// <para>Constraints: Ogni step segue un ordine logico, con possibilità di hook opzionali, ma una action action obbligatoria.</para>
/// <para>Relationships: Interagisce tramite l’interfaccia <c>IAfterPostActionStep&lt;T&gt;</c> per la fase finale.</para>
/// <para>Usage Notes: Flusso pensato per orchestrare validazioni semantiche e side-effect in modo dichiarativo.</para>
/// </remarks>
internal static partial class HookedExecutionFlow
{
    /// <summary>
    /// Inizializza il builder per il flusso di esecuzione relativo al tipo <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Tipo su cui applicare i vari step del flusso semantico.</typeparam>
    /// <returns>Builder vincolato all'ordine semantico degli step.</returns>
    public static IOnStartFlowHookStep<T> For<T>() => new Steps<T>();

    /// <summary>
    /// Implementazione interna del costruttore fluente a step per il flusso semantico con hook.
    /// </summary>
    /// <typeparam name="T">Tipo su cui eseguire gli step.</typeparam>
    /// <remarks>
    /// <para>Questa classe implementa direttamente tutte le interfacce della sequenza fluente.</para>
    /// </remarks>
    private sealed partial class Steps<T> :
        IOnStartFlowHookStep<T>,
        IInitialValidationStep<T>,
        IPreActionStep<T>,
        IActionStep<T>,
        IPostActionStep<T>,
        IFinalValidationStep<T>,
        IOnCompleteFlowHookStep<T>
    {
        private readonly List<Action<T>> _onStartFlowHook = [];
        private readonly List<Action<T>> _initialValidation = [];
        private readonly List<Action<T>> _preActionHook = [];
        private Action<T> _action = _ => { }; // Action action sempre valorizzata
        private readonly List<Action<T>> _postActionHook = [];
        private readonly List<Action<T>> _finalValidation = [];
        private readonly List<Action<T>> _onCompleteFlowHook = [];

        /// <summary>
        /// Verifica che il metodo usato come hook sia dichiarato virtual o abstract.
        /// </summary>
        /// <param name="hook">Delegato del metodo.</param>
        /// <exception cref="ArgumentException">Lanciata se il metodo non è virtual né abstract.</exception>
        private static void EnsureMethodIsVirtualOrAbstract(Action<T> hook)
        {
            var methodInfo = hook.Method;
            if (!(methodInfo.IsVirtual || methodInfo.IsAbstract))
                throw new ArgumentException($"The method '{methodInfo.Name}' must be virtual or abstract to be used as a hook.");
        }

        /// <summary>
        /// Esegue il flusso completo sul target, rispettando l’ordine semantico e i vincoli imposti.
        /// </summary>
        /// <param name="target">Oggetto su cui eseguire il flusso.</param>
        /// <exception cref="InvalidOperationException">Sollevata se <c>WithAction</c> non è stato invocato.</exception>
        public void Execute(T target)
        {
            if (_action is null)
                throw new InvalidOperationException("Action action must be defined before execution.");

            // Fase 1
            foreach (var act in _onStartFlowHook) act(target);
            foreach (var act in _initialValidation) act(target);
            // Fase 2
            foreach (var act in _preActionHook) act(target);
            _action(target);
            foreach (var act in _postActionHook) act(target);

            // Fase 3
            foreach (var act in _finalValidation) act(target);
            foreach (var act in _onCompleteFlowHook) act(target);
        }
    }
}