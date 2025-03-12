using Microsoft.Extensions.Configuration;

namespace JsonBridgeEF.Config
{
    /// <summary>
    /// Classe centralizzata per l'accesso alle impostazioni dell'applicazione.
    /// </summary>
    public static class AppSettings
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        /// <summary>
        /// Recupera un valore dalla configurazione dato un percorso chiave.
        /// </summary>
        public static string Get(string configurationKey) => Configuration[configurationKey] ?? string.Empty;
    }
}