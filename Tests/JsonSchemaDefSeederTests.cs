using Xunit;
using Moq;
using JsonBridgeEF.Seeding.SourceJson.Services;
using JsonBridgeEF.Validators;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio JsonSchemaSeeder.
    /// </summary>
    public class JsonSchemaSeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly JsonSchemaSeeder _seeder;

        public JsonSchemaSeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _seeder = new JsonSchemaSeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il seeding di uno schema JSON valido avvenga correttamente.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidSchema_ShouldSaveToRepository()
        {
            // Arrange
            var schema = new JsonSchema(new JsonSchemaValidator())
            {
                Name = "Test Schema",
                JsonSchemaIdentifier = "TestSchemaV1",
                Description = "Schema di test per validazione."
            };

            // Act
            var result = await _seeder.SeedAsync(schema);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Schema", result.Name);
            Assert.Equal("TestSchemaV1", result.JsonSchemaIdentifier);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione quando lo schema è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullSchema_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se il nome o l'identificatore è vuoto.
        /// </summary>
        [Theory]
        [InlineData("", "ValidIdentifier")]
        [InlineData("ValidName", "")]
        [InlineData("", "")]
        public async Task SeedAsync_InvalidSchema_ShouldThrowInvalidOperationException(string name, string identifier)
        {
            // Arrange
            var invalidSchema = new JsonSchema(new JsonSchemaValidator())
            {
                Name = name,
                JsonSchemaIdentifier = identifier,
                Description = "Schema con valori non validi"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _seeder.SeedAsync(invalidSchema));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync corregga automaticamente lo schema se possibile.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ShouldCallTryValidateAndFix()
        {
            // Arrange
            var schemaMock = new Mock<JsonSchema>(new JsonSchemaValidator()) { CallBase = true };

            // Act
            await _seeder.SeedAsync(schemaMock.Object);

            // Assert
            schemaMock.Verify(s => s.TryValidateAndFix(), Times.Once);
        }
    }
}