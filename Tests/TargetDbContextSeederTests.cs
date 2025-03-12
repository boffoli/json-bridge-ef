using Xunit;
using Moq;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.TargetModel.Models;
using JsonBridgeEF.Seeding.TargetModel.Services;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio <see cref="TargetDbContextSeeder"/>.
    /// </summary>
    public class TargetDbContextSeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly TargetDbContextSeeder _seeder;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="TargetDbContextSeederTests"/>.
        /// </summary>
        public TargetDbContextSeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _seeder = new TargetDbContextSeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il metodo <see cref="TargetDbContextSeeder.SeedAsync"/> 
        /// salvi correttamente un'istanza valida di <see cref="TargetDbContextDef"/>.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidTargetDbContextDef_ShouldSaveToRepository()
        {
            // Arrange
            var targetDbContextDef = new TargetDbContextDef
            {
                Name = "TestDbContext",
                Namespace = "JsonBridgeEF.Data",
                Description = "Test Context"
            };

            // Act
            var result = await _seeder.SeedAsync(targetDbContextDef);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestDbContext", result.Name);
            Assert.Equal("JsonBridgeEF.Data", result.Namespace);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo <see cref="TargetDbContextSeeder.SeedAsync"/> 
        /// sollevi un'eccezione di tipo <see cref="ArgumentNullException"/> se il parametro è null.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullTargetDbContextDef_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!));
        }

        /// <summary>
        /// Verifica che il metodo <see cref="TargetDbContextSeeder.SeedAsync"/> 
        /// sollevi un'eccezione di tipo <see cref="InvalidOperationException"/> se il nome o il namespace è vuoto.
        /// </summary>
        /// <param name="name">Il nome del contesto target (potenzialmente vuoto).</param>
        /// <param name="ns">Il namespace del contesto target (potenzialmente vuoto).</param>
        [Theory]
        [InlineData("", "JsonBridgeEF.Data")]
        [InlineData("TestDbContext", "")]
        [InlineData("", "")]
        public async Task SeedAsync_InvalidTargetDbContextDef_ShouldThrowInvalidOperationException(string name, string ns)
        {
            // Arrange
            var invalidTargetDbContextDef = new TargetDbContextDef
            {
                Name = name,
                Namespace = ns,
                Description = "Invalid Context"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _seeder.SeedAsync(invalidTargetDbContextDef));
        }
    }
}