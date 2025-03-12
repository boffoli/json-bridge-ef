using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonBridgeEF.Seeding.SourceJson.Helpers;
using JsonBridgeEF.Importing.Preprocessing.Services;

namespace JsonBridgeEF.Tests
{
    /// <summary>
    /// Test per il servizio <see cref="JsonProcessor"/>.
    /// </summary>
    public class JsonProcessorTests
    {
        private readonly JsonProcessor _processor;
        private const string IndependentBlocksKey = "independent_blocks"; // 🔹 Definisce una costante per il nome della chiave JSON

        /// <summary>
        /// Inizializza un'istanza di <see cref="JsonProcessorTests"/> con un registro di test preconfigurato.
        /// </summary>
        public JsonProcessorTests()
        {
            // Inizializza il registro dei blocchi indipendenti per i test come variabile locale
            var testRegistry = new JsonIndepBlockRegistry();
            testRegistry.AddBlock("utenti", "id_utente");
            testRegistry.AddBlock("contatti", "id_contatto");
            testRegistry.AddBlock("metadati", "id_metadato");

            _processor = new JsonProcessor(testRegistry);
        }

        /// <summary>
        /// Verifica che il servizio elabori correttamente un JSON valido letto da un file,
        /// restituendo i blocchi indipendenti ordinati.
        /// </summary>
        [Fact]
        public async Task ProcessJsonFile_ValidJson_ShouldReturnOrderedBlocks()
        {
            // Arrange
            string testJson = @"
            {
                ""utenti"": [{""id_utente"": 1, ""nome"": ""Mario""}],
                ""contatti"": [{""id_contatto"": 10, ""telefono"": ""123456789""}],
                ""metadati"": [{""id_metadato"": 100, ""descrizione"": ""Test data""}]
            }";

            string tempFilePath = Path.GetRandomFileName();
            await File.WriteAllTextAsync(tempFilePath, testJson);

            // Act
            JObject result = _processor.ProcessJsonFile(tempFilePath);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(IndependentBlocksKey)); // ✅ Usa la costante
            Assert.Equal(3, result[IndependentBlocksKey]?.Count()); // ✅ Usa la costante

            File.Delete(tempFilePath);
        }

        /// <summary>
        /// Verifica che il servizio elabori correttamente un JSON valido letto direttamente da una stringa,
        /// restituendo i blocchi indipendenti ordinati.
        /// </summary>
        [Fact]
        public void ProcessJsonString_ValidJson_ShouldReturnOrderedBlocks()
        {
            // Arrange
            string testJson = @"
            {
                ""utenti"": [{""id_utente"": 1, ""nome"": ""Mario""}],
                ""contatti"": [{""id_contatto"": 10, ""telefono"": ""123456789""}],
                ""metadati"": [{""id_metadato"": 100, ""descrizione"": ""Test data""}]
            }";

            // Act
            JObject result = _processor.ProcessJsonString(testJson);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(IndependentBlocksKey)); // ✅ Usa la costante
            Assert.Equal(3, result[IndependentBlocksKey]?.Count()); // ✅ Usa la costante
        }

        /// <summary>
        /// Verifica che venga sollevata un'eccezione di tipo <see cref="FileNotFoundException"/>
        /// se il file JSON specificato non esiste.
        /// </summary>
        [Fact]
        public void ProcessJsonFile_NonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            string nonExistentFilePath = "non_existing_file.json";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _processor.ProcessJsonFile(nonExistentFilePath));
        }

        /// <summary>
        /// Verifica che venga sollevata un'eccezione di tipo <see cref="JsonReaderException"/>
        /// se il JSON fornito è non valido o malformato.
        /// </summary>
        [Fact]
        public void ProcessJsonString_InvalidJson_ShouldThrowJsonException()
        {
            // Arrange
            string invalidJson = "{ \"utenti\": [ { \"id_utente\": 1, "; // JSON malformato

            // Act & Assert
            Assert.Throws<JsonReaderException>(() => _processor.ProcessJsonString(invalidJson));
        }

        /// <summary>
        /// Verifica che venga sollevata un'eccezione di tipo <see cref="ArgumentException"/>
        /// se il contenuto JSON è una stringa vuota o nulla.
        /// </summary>
        /// <param name="jsonContent">Il contenuto JSON fornito come input.</param>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ProcessJsonString_EmptyOrNull_ShouldThrowArgumentException(string? jsonContent)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _processor.ProcessJsonString(jsonContent!));
        }
    }
}