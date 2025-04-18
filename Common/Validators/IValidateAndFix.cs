namespace JsonBridgeEF.Common.Validators
{
    /// <summary>
    /// Interfaccia generica per validare e correggere un'istanza.
    /// </summary>
    /// <typeparam name="TModel">Il tipo del modello da validare e correggere.</typeparam>
    public interface IValidateAndFix<TModel>
    {
        /// <summary>
        /// Verifica che l'istanza sia valida.
        /// Solleva un'eccezione se l'istanza non risulta valida.
        /// </summary>
        /// <param name="model">L'istanza da validare.</param>
        void EnsureValid(TModel model);

        /// <summary>
        /// Tenta di correggere eventuali anomalie nell'istanza.
        /// Se la correzione è impossibile, solleva un'eccezione.
        /// </summary>
        /// <param name="model">L'istanza da correggere.</param>
        void Fix(TModel model);
    }
}