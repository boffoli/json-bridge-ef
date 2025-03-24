using Bogus;
using JsonBridgeEF.DataAccess.Interfaces;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.SampleTargetModel;

namespace JsonBridgeEF.DataAccess.Helpers;

/// <summary>
/// Helper per la generazione casuale di dati nel database, inclusi utenti e regole di mappatura.
/// </summary>
internal static class RandomDataGenerator
{
    private static readonly string[] Themes = ["light", "dark"];

    #region **🔹 Generazione Utenti**

    /// <summary>
    /// Genera una lista di utenti casuali con dati realistici.
    /// </summary>
    /// <param name="count">Numero di utenti da generare.</param>
    /// <returns>Una lista di <see cref="User"/> generati casualmente.</returns>
    public static Task<List<User>> GenerateRandomUsersAsync(int count)
    {
        var faker = new Faker("it");
        var users = new List<User>();

        for (int i = 0; i < count; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var address = faker.Address.StreetAddress();
            var phoneNumber = faker.Phone.PhoneNumber();

            var contact = new UserContact
            {
                ContactId = i + 1, // Simulazione di una chiave autonoma
                FullContactInfo = $"{address} - {phoneNumber}".TrimEnd('-').Trim(),
                Preferences = new UserPreferences
                {
                    Theme = faker.PickRandom(Themes),
                    EmailNotifications = faker.Random.Bool()
                }
            };

            var user = new User
            {
                UserId = i + 1,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Contact = contact,
                ContactId = contact.ContactId
            };

            users.Add(user);
        }

        return Task.FromResult(users);
    }

    #endregion

    #region **🔹 Generazione Regole di Mapping**

    /// <summary>
    /// Genera una lista di regole di mappatura casuali utilizzando dati esistenti forniti dal <see cref="IMappingDataProvider"/>.
    /// </summary>
    /// <param name="dataProvider">L'istanza di <see cref="IMappingDataProvider"/> da cui recuperare le entità necessarie.</param>
    /// <param name="count">Numero di regole di mappatura da generare.</param>
    /// <returns>Una lista di <see cref="MappingRule"/> generati casualmente.</returns>
    /// <exception cref="InvalidOperationException">Se non ci sono dati sufficienti per generare le regole di mappatura.</exception>
    public static async Task<List<MappingRule>> GenerateRandomMappingsAsync(IMappingDataProvider dataProvider, int count)
    {
        // Recupera i dati necessari dal provider
        var mappingProjects = await dataProvider.GetMappingProjectsAsync();
        var jsonFields = await dataProvider.GetJsonFieldAsync();
        var targetEntities = await dataProvider.GetTargetPropertysAsync();

        // Verifica che ci siano abbastanza dati per generare le mappature
        if (mappingProjects.Count == 0 || jsonFields.Count == 0 || targetEntities.Count == 0)
        {
            throw new InvalidOperationException("Non ci sono abbastanza dati per generare regole di mappatura.");
        }

        var faker = new Faker("it");
        var mappings = new List<MappingRule>();

        // Generazione delle regole di mappatura casuali
        for (int i = 0; i < count; i++)
        {
            var mappingProject = faker.PickRandom(mappingProjects);
            var jsonField = faker.PickRandom(jsonFields);
            var targetProperty = faker.PickRandom(targetEntities);

            var mapping = new MappingRule(null)//??
            {
                MappingProjectId = mappingProject.Id, // ✅ Ora fa riferimento al MappingProject
                JsonFieldId = jsonField.Id,
                TargetPropertyId = targetProperty.Id,
                JsFormula = "function transform(value) { return value; }"
            };

            mappings.Add(mapping);
        }

        return mappings;
    }

    #endregion
}