using Xunit;
using Moq;
using JsonBridgeEF.Seeding.Mapping.Models;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Mapping.Services;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Common.Repositories;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio <see cref="MappingProjectSeeder"/>.
    /// </summary>
    public class MappingProjectSeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<MappingProject>> _mockRepository;
        private readonly MappingProjectSeeder _seeder;

        public MappingProjectSeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRepository<MappingProject>>();

            // Configura il mock per restituire il repository fittizio
            _mockUnitOfWork.Setup(u => u.Repository<MappingProject>())
                           .Returns(_mockRepository.Object);

            _seeder = new MappingProjectSeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il metodo <see cref="MappingProjectSeeder.SeedAsync"/> crei e salvi correttamente un progetto di mapping.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidMappingProject_ShouldCreateAndSaveMappingProject()
        {
            // Arrange
            var mappingProject = new MappingProject(new MappingProjectValidator())
            {
                Name = "Test Mapping Project",
                JsonSchemaId = 10,
                TargetDbContextInfoId = 1
            };

            // Simula l'aggiunta al repository
            _mockRepository.Setup(r => r.Add(It.IsAny<MappingProject>()));

            // Act
            var result = await _seeder.SeedAsync(mappingProject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Mapping Project", result.Name);
            Assert.Equal(10, result.JsonSchemaId);
            Assert.Equal(1, result.TargetDbContextInfoId);

            _mockRepository.Verify(r => r.Add(It.IsAny<MappingProject>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Verifica che il metodo <see cref="MappingProjectSeeder.SeedAsync"/> sollevi un'eccezione se il progetto è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullMappingProject_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!));
        }

        /// <summary>
        /// Verifica che il metodo <see cref="MappingProjectSeeder.SeedAsync"/> sollevi un'eccezione se il nome è vuoto.
        /// </summary>
        [Fact]
        public async Task SeedAsync_EmptyName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mappingProject = new MappingProject(new MappingProjectValidator())
            {
                Name = "",
                JsonSchemaId = 10,
                TargetDbContextInfoId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _seeder.SeedAsync(mappingProject));
        }

        /// <summary>
        /// Verifica che il metodo <see cref="MappingProjectSeeder.SeedAsync"/> sollevi un'eccezione se l'ID dello schema JSON è non valido.
        /// </summary>
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task SeedAsync_InvalidJsonSchemaId_ShouldThrowInvalidOperationException(int invalidSchemaId)
        {
            // Arrange
            var mappingProject = new MappingProject(new MappingProjectValidator())
            {
                Name = "Test Mapping Project",
                JsonSchemaId = invalidSchemaId,
                TargetDbContextInfoId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _seeder.SeedAsync(mappingProject));
        }

        /// <summary>
        /// Verifica che il metodo <see cref="MappingProjectSeeder.SeedAsync"/> chiami il metodo <see cref="TryValidateAndFix"/>.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ShouldCallTryValidateAndFix()
        {
            // Arrange
            var mappingProjectMock = new Mock<MappingProject>(new MappingProjectValidator()) { CallBase = true };

            // Act
            await _seeder.SeedAsync(mappingProjectMock.Object);

            // Assert
            mappingProjectMock.Verify(m => m.TryValidateAndFix(), Times.Once);
        }
    }
}