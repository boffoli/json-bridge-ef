using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JsonBridgeEF.Common;
using JsonBridgeEF.Seeding.SourceJson.Collections;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta la definizione di uno schema JSON.  
/// Centralizza la struttura e i blocchi utilizzati nel processo di mapping JSON-to-Entity.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creato tramite il metodo statico <see cref="Create"/>.  
/// I costruttori sono privati o riservati a Entity Framework Core.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Il nome deve essere non nullo e non vuoto.</item>
///   <item>Il contenuto JSON è obbligatorio e salvato come testo (TEXT).</item>
///   <item>I blocchi devono essere aggiunti solo tramite il metodo interno <see cref="AddBlock"/>.</item>
/// </list>
/// </para>
///
/// <para><b>Relationships:</b>
/// <list type="bullet">
///   <item>Contiene molti <see cref="JsonBlock"/> (relazione uno-a-molti).</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// I blocchi vengono registrati nello schema tramite il metodo <see cref="AddBlock"/>,  
/// invocato internamente dalla factory di <see cref="JsonBlock"/>.  
/// La collezione non consente la rimozione diretta.
/// </para>
public sealed class JsonSchema : BaseEfEntity<JsonSchema>
{
    // 🔹 COSTRUTTORI 🔹

    /// <summary>
    /// Costruttore richiesto da Entity Framework Core.
    /// </summary>
    private JsonSchema() : base(null)
    {
        _jsonBlocks = new JsonSchemaBlockCollection(this);
    }

    /// <summary>
    /// Costruttore privato utilizzato dalla factory <see cref="Create"/>.
    /// </summary>
    private JsonSchema(string name, string jsonContent, IValidateAndFix<JsonSchema>? validator)
        : base(validator)
    {
        Name = name;
        JsonSchemaContent = jsonContent;
        _jsonBlocks = new JsonSchemaBlockCollection(this);
    }

    // 🔹 FACTORY 🔹

    /// <summary>
    /// Crea una nuova definizione di schema JSON.
    /// </summary>
    /// <param name="name">Nome dello schema.</param>
    /// <param name="jsonContent">Contenuto JSON (stringa completa).</param>
    /// <param name="validator">Validatore opzionale da applicare allo schema.</param>
    /// <returns>Nuova istanza dello schema JSON.</returns>
    public static JsonSchema Create(string name, string jsonContent, IValidateAndFix<JsonSchema>? validator = null)
    {
        // Validazioni d'ingresso
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Il nome dello schema non può essere vuoto.", nameof(name));

        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new ArgumentException("Il contenuto JSON dello schema non può essere vuoto.", nameof(jsonContent));

        return new JsonSchema(name, jsonContent, validator);
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <inheritdoc/>
    protected override bool HasSlug => true;

    // 🔹 PROPRIETÀ PERSISTENTI 🔹

    /// <summary>
    /// Contenuto JSON dello schema.
    /// </summary>
    [Required]
    [Column(TypeName = "TEXT")] // Compatibile con SQLite
    public string JsonSchemaContent { get; private set; } = string.Empty;

    // 🔹 COLLEZIONE BLOCCHI 🔹

    private readonly JsonSchemaBlockCollection _jsonBlocks;

    /// <summary>
    /// Collezione di blocchi associati a questo schema.
    /// </summary>
    public IReadOnlyCollection<JsonBlock> JsonBlocks => _jsonBlocks.Items;

    // 🔹 USO INTERNO 🔹

    /// <summary>
    /// Registra un blocco all'interno dello schema.
    /// Deve essere invocato solo dalla factory <see cref="JsonBlock.Create"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void AddBlock(JsonBlock block)
    {
        _jsonBlocks.Add(block);
    }

    // 🔹 VALIDAZIONE 🔹

    protected override void OnBeforeValidate() { }
    protected override void OnAfterValidate() { }

    // 🔹 TO STRING 🔹

    public override string ToString()
        => $"{Name} (Blocks: {_jsonBlocks})";
}