namespace JsonBridgeEF.Shared.Infrastructure.HookedExecutionFlow;

/// <summary>
/// Domain Interface: Step per configurare hook prima della pre-validazione.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Hook opzionale prima della validazione iniziale.</para>
/// <para><b>Creation Strategy:</b> Primo step del builder.</para>
/// <para><b>Constraints:</b> Obbliga lo step di pre-validazione successivo.</para>
/// <para><b>Relationships:</b> Collegato a <see cref="IInitialValidationStep{T}"/>.</para>
/// <para><b>Usage Notes:</b> Metodo richiesto virtual/abstract.</para>
/// </remarks>
internal interface IOnStartFlowHookStep<T>
{
    IInitialValidationStep<T> WithOnStartFlowHook(Action<T> hook);
    IInitialValidationStep<T> SkipOnStartFlowHook();
}

/// <summary>
/// Domain Interface: Step di validazione iniziale.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Pre-validazione del target.</para>
/// <para><b>Creation Strategy:</b> Deriva da hook iniziale.</para>
/// <para><b>Constraints:</b> Obbligatorio se c'è hook precedente.</para>
/// <para><b>Relationships:</b> Prosegue su <see cref="IAfterInitialValidationStep{T}"/>.</para>
/// </remarks>
internal interface IInitialValidationStep<T>
{
    IPreActionStep<T> WithInitialValidation(Action<T> hook);
    IPreActionStep<T> SkipInitialValidation();
}

/// <summary>
/// Domain Interface: Hook prima della fase action.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Logica custom prima del action.</para>
/// <para><b>Creation Strategy:</b> Da step precedente.</para>
/// <para><b>Constraints:</b> Richiede lo step action dopo.</para>
/// <para><b>Relationships:</b> Con <see cref="IActionStep{T}"/>.</para>
/// </remarks>
internal interface IPreActionStep<T>
{
    IActionStep<T> WithOnPreActionHook(Action<T> hook);
    IActionStep<T> SkipOnPreActionHook();
}

/// <summary>
/// Domain Interface: Step obbligatorio del action.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Logica centrale del flusso.</para>
/// <para><b>Creation Strategy:</b> Obbligatorio sempre.</para>
/// <para><b>Constraints:</b> Non può essere nullo.</para>
/// <para><b>Relationships:</b> Porta a <see cref="IPostActionStep{T}"/>.</para>
/// </remarks>
internal interface IActionStep<T>
{
    IPostActionStep<T> WithAction(Action<T> action);
}

/// <summary>
/// Domain Interface: Hook post-action.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Azione opzionale dopo il action.</para>
/// <para><b>Creation Strategy:</b> Dopo il action.</para>
/// <para><b>Constraints:</b> Nessuna.</para>
/// <para><b>Relationships:</b> Porta a validazione finale.</para>
/// </remarks>
internal interface IPostActionStep<T>
{
    IFinalValidationStep<T> WithOnPostActionHook(Action<T> hook);
    IFinalValidationStep<T> SkipOnPostActionHook();
}

/// <summary>
/// Domain Interface: Step di validazione finale.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Validazione dopo il action.</para>
/// <para><b>Creation Strategy:</b> Da hook precedente.</para>
/// <para><b>Constraints:</b> Obbligatorio se hook precedente.</para>
/// <para><b>Relationships:</b> Va a <see cref="IOnCompleteFlowHookStep{T}"/>.</para>
/// </remarks>
internal interface IFinalValidationStep<T>
{
    IOnCompleteFlowHookStep<T> WithFinalValidation(Action<T> hook);
    IOnCompleteFlowHookStep<T> SkipFinalValidation();
}

/// <summary>
/// Domain Interface: Ultimo step opzionale con hook.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b> Azioni post-validazione.</para>
/// <para><b>Creation Strategy:</b> Conclusione del flusso.</para>
/// <para><b>Constraints:</b> Nessuna.</para>
/// <para><b>Usage Notes:</b> <c>Execute</c> è obbligatorio.</para>
/// </remarks>
internal interface IOnCompleteFlowHookStep<T>
{
    IOnCompleteFlowHookStep<T> WithOnCompleteFlowHook(Action<T> hook);
    IOnCompleteFlowHookStep<T> SkipOnCompleteFlowHook();
    void Execute(T target);
}