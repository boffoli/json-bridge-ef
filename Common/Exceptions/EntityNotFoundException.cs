using System;

namespace JsonBridgeEF.Common.Exceptions
{
    /// <summary>
    /// Eccezione lanciata quando un'entità richiesta non viene trovata.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Inizializza una nuova istanza di <see cref="EntityNotFoundException"/> con un messaggio di errore predefinito.
        /// </summary>
        public EntityNotFoundException() : base("L'entità richiesta non è stata trovata.") { }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="EntityNotFoundException"/> con un messaggio di errore specifico.
        /// </summary>
        /// <param name="message">Messaggio che descrive l'errore.</param>
        public EntityNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="EntityNotFoundException"/> con un messaggio di errore e un'eccezione interna.
        /// </summary>
        /// <param name="message">Messaggio che descrive l'errore.</param>
        /// <param name="innerException">Eccezione interna che ha causato questo errore.</param>
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}