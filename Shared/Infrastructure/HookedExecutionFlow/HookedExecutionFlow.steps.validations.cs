namespace JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

/// <summary>
/// Fasi iniziali del flusso semantico, relative alla validazione e preparazione.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Queste API rappresentano l'inizio della catena fluente per la configurazione del flusso.</para>
/// <para>Constraints: Se si configura <c>WithOnStartHook</c>, non si può saltare la validazione.</para>
/// <para>Usage Notes: Usare per preparare e validare i dati prima dell'esecuzione centrale.</para>
/// </remarks>
/// <summary>
/// Fasi finali del flusso semantico, inclusi gli hook e la validazione post-esecuzione.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Invocato a completamento del flusso semantico, dopo l’azione core.</para>
/// <para>Constraints: <c>WithFinalValidation</c> è richiesto se <c>WithBeforeFinalValidationHook</c> è stato usato.</para>
/// <para>Usage Notes: Adatto per logiche di validazione finale o cleanup dopo l’esecuzione.</para>
/// </remarks>
internal static partial class HookedExecutionFlow
{
    private sealed partial class Steps<T>
    {
        /// <summary>
        /// Aggiunge un hook da eseguire prima della validazione.
        /// </summary>
        /// <param name="hook">Metodo invocato prima della validazione semantica.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il metodo deve essere virtual o abstract.</para>
        /// <para><b>Postconditions:</b> L’hook verrà eseguito prima di <c>WithInitialValidation</c>.</para>
        /// <para><b>Side Effects:</b> Nessuno per default.</para>
        /// </remarks>
        public IInitialValidationStep<T> WithOnStartFlowHook(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            EnsureMethodIsVirtualOrAbstract(hook);
            _onStartFlowHook.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta l’hook <c>BeforeInitialValidation</c>.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IInitialValidationStep<T> SkipOnStartFlowHook() => this;

        /// <summary>
        /// Definisce la logica di validazione iniziale.
        /// </summary>
        /// <param name="hook">Metodo di validazione da eseguire.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Deve essere definito se è stato usato <c>WithOnStartHook</c>.</para>
        /// <para><b>Postconditions:</b> La logica sarà eseguita durante <c>Execute</c>.</para>
        /// <para><b>Side Effects:</b> Validazione semantica o strutturale del target.</para>
        /// </remarks>
        public IPreActionStep<T> WithInitialValidation(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            _initialValidation.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta la fase di validazione iniziale.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IPreActionStep<T> SkipInitialValidation()
        {
            return this;
        }

        /// <summary>
        /// Definisce la logica di post-validazione da eseguire sul target.
        /// </summary>
        /// <param name="hook">Metodo di validazione finale.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <exception cref="InvalidOperationException">Se si salta la validazione dopo aver definito un hook BeforeFinalValidation.</exception>
        /// <remarks>
        /// <para><b>Preconditions:</b> Non chiamare se è stato usato <c>WithBeforeFinalValidationHook</c> e si intende saltare la validazione.</para>
        /// <para><b>Postconditions:</b> La logica sarà eseguita durante <c>Execute</c>.</para>
        /// <para><b>Side Effects:</b> Eventuali controlli finali sullo stato del target.</para>
        /// </remarks>
        public IOnCompleteFlowHookStep<T> WithFinalValidation(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            _finalValidation.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta la fase di post-validazione.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IOnCompleteFlowHookStep<T> SkipFinalValidation()
        {
            return this;
        }

        /// <summary>
        /// Aggiunge un hook da eseguire dopo la post-validazione.
        /// </summary>
        /// <param name="hook">Metodo da eseguire in chiusura del flusso.</param>
        /// <returns>Lo step fluente successivo.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il metodo deve essere virtual o abstract.</para>
        /// <para><b>Postconditions:</b> L’hook sarà eseguito come fase conclusiva.</para>
        /// <para><b>Side Effects:</b> Eventuali notifiche, eventi o log.</para>
        /// </remarks>
        public IOnCompleteFlowHookStep<T> WithOnCompleteFlowHook(Action<T> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            EnsureMethodIsVirtualOrAbstract(hook);
            _onCompleteFlowHook.Add(hook);
            return this;
        }

        /// <summary>
        /// Salta l’hook <c>OnComplete</c>.
        /// </summary>
        /// <returns>Lo step fluente successivo.</returns>
        public IOnCompleteFlowHookStep<T> SkipOnCompleteFlowHook() => this;
    }
}