namespace JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

/// <summary>
/// Fasi centrali del flusso semantico, incluse configurazioni action e hook associati.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Queste API sono invocate durante la costruzione fluente di un flusso esecutivo.</para>
/// <para>Constraints: È obbligatorio chiamare <c>WithAction</c>; gli hook sono opzionali ma vincolano la sequenza.</para>
/// <para>Usage Notes: Usare questi step per eseguire logiche centrali del dominio e prepararne gli effetti.</para>
/// </remarks>
internal static partial class HookedExecutionFlow
{
    private sealed partial class Steps<T>
    {
        /// <summary>
        /// Aggiunge un hook da eseguire prima della logica centrale.
        /// </summary>
        /// <param name="hook">Metodo da invocare prima della Action.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il metodo deve essere virtual o abstract.</para>
        /// <para><b>Postconditions:</b> Il metodo sarà invocato prima di <c>WithAction</c>.</para>
        /// <para><b>Side Effects:</b> Nessuno (solo aggiunta alla sequenza interna).</para>
        /// </remarks>
        public IActionStep<T> WithOnPreActionHook(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            EnsureMethodIsVirtualOrAbstract(hook);
            _preActionHook.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta la configurazione dell’hook <c>PreAction</c>.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IActionStep<T> SkipOnPreActionHook() => this;

        /// <summary>
        /// Definisce l’azione action obbligatoria del flusso.
        /// </summary>
        /// <param name="action">Azione principale da eseguire sul target.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <exception cref="InvalidOperationException">Se è stato aggiunto un hook PreAction ma si omette <c>WithAction</c>.</exception>
        /// <remarks>
        /// <para><b>Preconditions:</b> Nessun vincolo diretto, ma è obbligatorio definire questa fase.</para>
        /// <para><b>Postconditions:</b> L’azione sarà invocata esattamente una volta durante <c>Execute</c>.</para>
        /// <para><b>Side Effects:</b> action logics / mutazioni del target.</para>
        /// </remarks>
        public IPostActionStep<T> WithAction(Action<T> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            return this;
        }

        /// <summary>
        /// Aggiunge un hook da eseguire subito dopo la Action.
        /// </summary>
        /// <param name="hook">Metodo post-action da invocare.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il metodo deve essere virtual o abstract.</para>
        /// <para><b>Postconditions:</b> Il metodo sarà eseguito dopo <c>WithAction</c>.</para>
        /// <para><b>Side Effects:</b> Nessuno per default.</para>
        /// </remarks>
        public IFinalValidationStep<T> WithOnPostActionHook(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            EnsureMethodIsVirtualOrAbstract(hook);
            _postActionHook.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta la configurazione dell’hook <c>PostAction</c>.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IFinalValidationStep<T> SkipOnPostActionHook() => this;
    }
}