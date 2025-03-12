using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JsonBridgeEF.SampleTargetModel
{
    /// <summary>
    /// Entità principale che rappresenta un utente nel sistema.
    /// </summary>
    internal class User
    {
        /// <summary>
        /// Identificatore univoco dell'utente.
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Nome dell'utente.
        /// </summary>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Cognome dell'utente.
        /// </summary>
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo di residenza dell'utente.
        /// </summary>
        [Required]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Identificatore del contatto associato.
        /// </summary>
        public int? ContactId { get; set; }

        /// <summary>
        /// Contatti dell'utente, ora trattati come entità separata con chiave primaria autonoma.
        /// </summary>
        [ForeignKey(nameof(ContactId))]
        public UserContact? Contact { get; set; }
    }

    /// <summary>
    /// Contatti dell'utente, ora trattati come entità separata.
    /// </summary>
    internal class UserContact
    {
        /// <summary>
        /// Identificatore univoco del contatto.
        /// </summary>
        [Key]
        public int ContactId { get; set; }

        /// <summary>
        /// Informazioni complete di contatto, generate esternamente combinando indirizzo e telefono.
        /// Questo campo è salvato nel database.
        /// </summary>
        [Required]
        public string FullContactInfo { get; set; } = string.Empty;

        /// <summary>
        /// Preferenze dell'utente (Value Object, senza chiave primaria).
        /// </summary>
        public UserPreferences Preferences { get; set; } = new();
    }

    /// <summary>
    /// Preferenze dell'utente (Value Object, senza chiave primaria).
    /// </summary>
    [ComplexType]
    internal class UserPreferences
    {
        /// <summary>
        /// Tema selezionato dall'utente (chiaro/scuro).
        /// </summary>
        [Required]
        public string Theme { get; set; } = "chiaro";

        /// <summary>
        /// Indica se l'utente ha attivato le notifiche email.
        /// </summary>
        [Required]
        public bool EmailNotifications { get; set; } = false;
    }

    /// <summary>
    /// Metadati generali relativi ai dati dell'applicazione.
    /// </summary>
    internal class Metadata
    {
        /// <summary>
        /// Identificatore del metadato.
        /// </summary>
        [Key]
        public int MetadataId { get; set; }

        /// <summary>
        /// Descrizione dei dati contenuti nel JSON.
        /// </summary>
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}