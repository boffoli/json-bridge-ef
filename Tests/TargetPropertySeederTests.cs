using Xunit;
using Moq;
using JsonBridgeEF.SampleTargetModel;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Seeding.TargetModel.Services;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio TargetPropertySeeder.
    /// </summary>
    public class TargetPropertySeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly TargetPropertySeeder _seeder;

        public TargetPropertySeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _seeder = new TargetPropertySeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync salvi correttamente le proprietà target per un contesto valido.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidTargetDbContextInfo_ShouldSaveProperties()
        {
            // Arrange
            var targetDbContextInfo = new TargetDbContextInfo
            {
                Id = 1,
                Name = "TestDbContext",
                Namespace = "JsonBridgeEF.Data",
                Description = "Test Context"
            };

            string targetNamespace = "JsonBridgeEF.Models.TargetDb";
            Type referenceEntityType = typeof(User); // Qualsiasi classe valida nel namespace

            // Act
            var result = await _seeder.SeedAsync(targetDbContextInfo, targetNamespace, referenceEntityType);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se il contesto target è null.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullTargetDbContextInfo_ShouldThrowArgumentNullException()
        {
            // Arrange
            string targetNamespace = "JsonBridgeEF.Models.TargetDb";
            Type referenceEntityType = typeof(User);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!, targetNamespace, referenceEntityType));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se il namespace target è nullo o vuoto.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task SeedAsync_InvalidNamespace_ShouldThrowArgumentNullException(string? targetNamespace)
        {
            // Arrange
            var targetDbContextInfo = new TargetDbContextInfo { Id = 1, Name = "TestDbContext", Namespace = "JsonBridgeEF.Data" };
            Type referenceEntityType = typeof(User);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(targetDbContextInfo, targetNamespace!, referenceEntityType));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se il tipo di riferimento è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullReferenceEntityType_ShouldThrowArgumentNullException()
        {
            // Arrange
            var targetDbContextInfo = new TargetDbContextInfo { Id = 1, Name = "TestDbContext", Namespace = "JsonBridgeEF.Data" };
            string targetNamespace = "JsonBridgeEF.Models.TargetDb";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(targetDbContextInfo, targetNamespace, null!));
        }
    }
}