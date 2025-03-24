using Xunit;
using Moq;
using JsonBridgeEF.Seeding.SourceJson.Services;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio <see cref="JsonFieldSeeder"/>.
    /// </summary>
    public class JsonFieldSeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly JsonFieldSeeder _seeder;

        /// <summary>
        /// Inizializza un'istanza di <see cref="JsonFieldSeederTests"/>.
        /// </summary>
        public JsonFieldSeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _seeder = new JsonFieldSeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il metodo <see cref="JsonFieldSeeder.SeedAsync"/> restituisca correttamente i campi JSON 
        /// quando viene fornito un file JSON valido e uno schema valido.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidJsonFileAndSchema_ShouldReturnFields()
        {
            // Arrange
            var schema = new JsonSchema
            {
                Id = 1,
                Name = "TestSchema",
                JsonSchemaIdentifier = "TestID"
            };

            string jsonFilePath = "valid.json"; // Questo file deve essere un mock

            // Act
            var result = await _seeder.SeedAsync(jsonFilePath, schema);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.SourceFieldPath == "utente.nome");
            Assert.Contains(result, f => f.SourceFieldPath == "utente.email");
        }

        /// <summary>
        /// Verifica che il metodo <see cref="JsonFieldSeeder.SeedAsync"/> lanci un'eccezione di tipo <see cref="ArgumentNullException"/>
        /// se il percorso del file JSON è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullJsonFilePath_ShouldThrowArgumentNullException()
        {
            // Arrange
            var schema = new JsonSchema { Id = 1, Name = "TestSchema", JsonSchemaIdentifier = "TestID" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!, schema));
        }

        /// <summary>
        /// Verifica che il metodo <see cref="JsonFieldSeeder.SeedAsync"/> lanci un'eccezione di tipo <see cref="ArgumentNullException"/>
        /// se lo schema JSON è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullSchema_ShouldThrowArgumentNullException()
        {
            // Arrange
            string jsonFilePath = "valid.json";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(jsonFilePath, null!));
        }
    }
}