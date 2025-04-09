using Microsoft.Extensions.Configuration;

namespace JsonBridgeEF.Config
{
    /// <summary>
    /// Domain Concept: Classe centralizzata per l'accesso alle impostazioni dell'applicazione.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Classe statica che inizializza una sola volta l'oggetto <c>IConfigurationRoot</c> all'avvio dell'applicazione.</para>
    /// <para>Constraints: Il file <c>appsettings.json</c> deve essere presente nella directory corrente e formattato correttamente.</para>
    /// <para>Relationships: Collabora con il sistema di configurazione di .NET per fornire impostazioni centralizzate.</para>
    /// <para>Usage Notes: Utilizzare il metodo <c>Get</c> per recuperare le impostazioni tramite una chiave configurata.</para>
    /// </remarks>
    public static class AppSettings
    {
        /// <summary>
        /// Istanza statica di <see cref="IConfigurationRoot"/> inizializzata con il file di configurazione <c>appsettings.json</c>.
        /// </summary>
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        /// <summary>
        /// Recupera un valore dalla configurazione dato un percorso chiave.
        /// </summary>
        /// <param name="configurationKey">La chiave della configurazione (es. "Configurators:Directory").</param>
        /// <returns>
        /// Il valore associato alla chiave, oppure una stringa vuota se la chiave non esiste.
        /// </returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il file <c>appsettings.json</c> deve essere presente e correttamente formattato.</para>
        /// <para><b>Postconditions:</b> Restituisce il valore configurato o una stringa vuota in caso di mancanza del valore.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static string Get(string configurationKey) => Configuration[configurationKey] ?? string.Empty;
    }
}