using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.DataAccess.Helpers;
using JsonBridgeEF.SampleTargetModel;

namespace JsonBridgeEF.DataAccess
{
    /// <summary>
    /// Servizio per l'interrogazione e la gestione dei dati nel database target.
    /// Include operazioni CRUD e la generazione di dati casuali.
    /// </summary>
    internal class TargetDbService(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {
        #region **🔹 Operazioni CRUD sugli Utenti**

        /// <summary>
        /// Restituisce tutti gli utenti presenti nel database, includendo i contatti e le preferenze.
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync() =>
            await GetRepository<User>().GetAllAsync();

        /// <summary>
        /// Cerca un utente per ID, includendo i contatti e le preferenze.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await GetRepository<User>().GetByIdAsync(id);
        }

        /// <summary>
        /// Aggiunge un nuovo utente al database, gestendo correttamente i contatti e le preferenze.
        /// </summary>
        public async Task AddUserAsync(User user)
        {
            if (user.Contact != null)
            {
                GetRepository<UserContact>().Add(user.Contact);
            }

            GetRepository<User>().Add(user);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna un utente esistente e le relative entità correlate senza sovrascrivere oggetti di navigazione.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await GetUserByIdAsync(user.UserId) ?? throw new KeyNotFoundException($"Nessun utente trovato con ID {user.UserId}.");

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;

            // Aggiornamento dei contatti, se presenti
            if (user.Contact != null)
            {
                if (existingUser.Contact == null)
                {
                    existingUser.Contact = new UserContact
                    {
                        ContactId = user.UserId, // Associazione con l'utente
                        FullContactInfo = user.Contact.FullContactInfo,
                        Preferences = user.Contact.Preferences
                    };
                }
                else
                {
                    existingUser.Contact.FullContactInfo = user.Contact.FullContactInfo;
                    existingUser.Contact.Preferences.Theme = user.Contact.Preferences.Theme;
                    existingUser.Contact.Preferences.EmailNotifications = user.Contact.Preferences.EmailNotifications;
                }
            }

            GetRepository<User>().Update(existingUser);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un utente dal database tramite ID e rimuove il contatto associato.
        /// </summary>
        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"Nessun utente trovato con ID {id}.");

            if (user.ContactId.HasValue)
            {
                var contact = await GetRepository<UserContact>().GetByIdAsync(user.ContactId.Value);
                if (contact != null)
                {
                    GetRepository<UserContact>().Remove(contact);
                }
            }

            GetRepository<User>().Remove(user);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Elimina tutti gli utenti e i loro contatti dal database.
        /// </summary>
        public async Task ClearAllUsersAsync()
        {
            var allUsers = await GetRepository<User>().GetAllAsync();
            var allContacts = await GetRepository<UserContact>().GetAllAsync();

            if (allUsers.Count > 0)
            {
                GetRepository<User>().RemoveRange(allUsers);
            }

            if (allContacts.Count > 0)
            {
                GetRepository<UserContact>().RemoveRange(allContacts);
            }

            await SaveChangesAsync();
        }

        #endregion

        #region **🔹 Popolamento Dati**

        /// <summary>
        /// Popola il database con utenti e dati casuali verosimili.
        /// </summary>
        public async Task PopulateWithRandomUsersAsync(int count = 10)
        {
            var users = await RandomDataGenerator.GenerateRandomUsersAsync(count);

            foreach (var user in users)
            {
                if (user.Contact != null)
                {
                    GetRepository<UserContact>().Add(user.Contact);
                }
                GetRepository<User>().Add(user);
            }

            await SaveChangesAsync();
        }

        #endregion
    }
}