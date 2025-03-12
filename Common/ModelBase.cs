using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Common
{
    /// <summary>
    /// Classe base astratta per tutte le entità del modello.
    /// Fornisce metodi per validare e correggere l'entità tramite un validatore iniettato.
    /// </summary>
    /// <typeparam name="TModel">Il tipo del modello derivato.</typeparam>
    /// <remarks>
    /// Costruttore protetto che richiede un validatore, opzionale (default null).
    /// Questo costruttore è l'unico e, grazie al valore predefinito, permette a EF di creare l'istanza.
    /// </remarks>
    /// <param name="validator">Il validatore da iniettare per questa entità.</param>
    public abstract class ModelBase<TModel>(IValidateAndFix<TModel>? validator = null) where TModel : ModelBase<TModel>
    {
        private readonly IValidateAndFix<TModel>? _validator = validator;

        /// <summary>
        /// Verifica che l'entità sia valida.
        /// Solleva un'eccezione se l'entità non è valida.
        /// </summary>
        public void EnsureValid() => _validator?.EnsureValid((TModel)this);

        /// <summary>
        /// Tenta di correggere l'entità.
        /// Se la correzione non è possibile, solleva un'eccezione.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Sollevata se la correzione non è stata possibile o l'entità rimane invalida.
        /// </exception>
        public void Fix()
        {
            if (_validator == null)
                throw new InvalidOperationException("No validator available to fix the entity.");

            _validator.Fix((TModel)this);
        }

        /// <summary>
        /// Esegue la validazione; se questa fallisce, tenta di correggere l'entità e poi riprova a validarla.
        /// Se, anche dopo la correzione, l'entità non risulta valida, solleva un'eccezione.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Sollevata se la validazione iniziale e il successivo tentativo di correzione falliscono.
        /// </exception>
        public void TryValidateAndFix()
        {
            try
            {
                EnsureValid();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Validation failed: {ex.Message}. Attempting to fix...");
                Fix();
                EnsureValid();
            }
        }
    }
}