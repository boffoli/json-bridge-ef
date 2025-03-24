using Xunit;
using Moq;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Seeding.Mappings.Helpers;
using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;
using JsonBridgeEF.Common.Repositories;
using JsonBridgeEF.Seeding.Mappings.Services;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio <see cref="MappingRuleSeeder"/>.
    /// </summary>
    public class MappingRuleSeederTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRepository<MappingRule>> _mockRepository;
        private readonly Mock<IMappingRuleConsole> _mockRuleConsole;
        private readonly MappingRuleSeeder _seeder;

        public MappingRuleSeederTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRepository<MappingRule>>();
            _mockRuleConsole = new Mock<IMappingRuleConsole>();

            // Configura il mock per il repository di MappingRule
            _mockUnitOfWork.Setup(u => u.Repository<MappingRule>())
                           .Returns(_mockRepository.Object);

            _seeder = new MappingRuleSeeder(_mockUnitOfWork.Object);
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync salvi correttamente le regole di mapping valide.
        /// </summary>
        [Fact]
        public async Task SeedAsync_ValidInputs_ShouldSaveMappingRules()
        {
            // Arrange
            var jsonFields = new List<JsonField>
            {
                new() { Id = 1, SourceFieldPath = "utente.nome" },
                new() { Id = 2, SourceFieldPath = "utente.email" }
            };

            var targetProperties = new List<TargetProperty>
            {
                new() { Id = 101, Namespace = "JsonBridgeEF.Models", RootClass = "User", Name = "FirstName" },
                new() { Id = 102, Namespace = "JsonBridgeEF.Models", RootClass = "User", Name = "Email" }
            };

            var mappingProject = new MappingProject
            {
                Id = 1,
                Name = "Test Mapping Project",
                JsonSchemaId = 10,
                TargetDbContextInfoId = 20
            };

            var expectedRules = new List<MappingRule>
            {
                new() { MappingProjectId = mappingProject.Id, JsonFieldId = 1, TargetPropertyId = 101, JsFormula = "" },
                new() { MappingProjectId = mappingProject.Id, JsonFieldId = 2, TargetPropertyId = 102, JsFormula = "" }
            };

            // Simula il comportamento della console di mapping
            _mockRuleConsole.SetupSequence(rc => rc.PromptMappingRuleForProperty(It.IsAny<List<JsonField>>(), It.IsAny<TargetProperty>(), It.IsAny<MappingProject>()))
                            .Returns(expectedRules[0])
                            .Returns(expectedRules[1]);

            // Act
            var result = await _seeder.SeedAsync(jsonFields, targetProperties, mappingProject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(r => r.Add(It.IsAny<MappingRule>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync restituisca una lista vuota se l'utente non seleziona alcun mapping.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NoMappings_ShouldReturnEmptyList()
        {
            // Arrange
            var jsonFields = new List<JsonField>
            {
                new() { Id = 1, SourceFieldPath = "utente.nome" }
            };

            var targetProperties = new List<TargetProperty>
            {
                new() { Id = 101, Namespace = "JsonBridgeEF.Models", RootClass = "User", Name = "FirstName" }
            };

            var mappingProject = new MappingProject
            {
                Id = 1,
                Name = "Test Mapping Project",
                JsonSchemaId = 10,
                TargetDbContextInfoId = 20
            };

            // Simula il comportamento della console quando nessun mapping viene selezionato
            _mockRuleConsole.Setup(rc => rc.PromptMappingRuleForProperty(It.IsAny<List<JsonField>>(), It.IsAny<TargetProperty>(), It.IsAny<MappingProject>()))
                            .Returns((MappingRule?)null);

            // Act
            var result = await _seeder.SeedAsync(jsonFields, targetProperties, mappingProject);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.Add(It.IsAny<MappingRule>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se la lista di campi JSON è nulla.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullJsonFields_ShouldThrowArgumentNullException()
        {
            // Arrange
            var targetProperties = new List<TargetProperty>();
            var mappingProject = new MappingProject();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(null!, targetProperties, mappingProject));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se la lista di proprietà target è nulla.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullTargetPropertys_ShouldThrowArgumentNullException()
        {
            // Arrange
            var jsonFields = new List<JsonField>();
            var mappingProject = new MappingProject();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(jsonFields, null!, mappingProject));
        }

        /// <summary>
        /// Verifica che il metodo SeedAsync lanci un'eccezione se il progetto di mapping è nullo.
        /// </summary>
        [Fact]
        public async Task SeedAsync_NullMappingProject_ShouldThrowArgumentNullException()
        {
            // Arrange
            var jsonFields = new List<JsonField>();
            var targetProperties = new List<TargetProperty>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _seeder.SeedAsync(jsonFields, targetProperties, null!));
        }
    }

    /// <summary>
    /// Interfaccia per il comportamento mockabile di <see cref="MappingRuleConsole"/>.
    /// </summary>
    internal interface IMappingRuleConsole
    {
        /// <summary>
        /// Simula l'interazione con l'utente per selezionare una regola di mapping.
        /// </summary>
        /// <param name="jsonFields">Lista dei campi JSON disponibili.</param>
        /// <param name="targetProp">Proprietà target su cui effettuare il mapping.</param>
        /// <param name="mappingProject">Progetto di mapping corrente.</param>
        /// <returns>La regola di mapping selezionata oppure <c>null</c> se nessun mapping è stato scelto.</returns>
        MappingRule? PromptMappingRuleForProperty(
            List<JsonField> jsonFields,
            TargetProperty targetProp,
            MappingProject mappingProject);
    }
}