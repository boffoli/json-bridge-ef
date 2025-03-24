using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common;
using JsonBridgeEF.Seeding.SourceJson.Collections;
using JsonBridgeEF.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta un blocco JSON, ovvero una sezione logica di uno schema composta da uno o più campi.
/// Ogni blocco appartiene a uno schema JSON e può essere indipendente (con una chiave) o connesso ad altri blocchi.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creato tramite il metodo statico <see cref="Create"/>.  
/// La registrazione all'interno dello <see cref="JsonSchema"/> avviene tramite il metodo <c>AddBlock</c> dello schema stesso.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Deve avere un nome non vuoto.</item>
///   <item>Deve essere registrato tramite <see cref="JsonSchema.AddBlock(JsonBlock)"/> per impostare correttamente lo schema.</item>
///   <item>La collezione dei campi JSON garantisce unicità e integrità interna.</item>
/// </list>
/// </para>
///
/// <para><b>Relationships:</b>
/// <list type="bullet">
///   <item>Appartiene a uno <see cref="JsonSchema"/> (relazione molti-a-uno).</item>
///   <item>Può contenere molti <see cref="JsonField"/> (relazione uno-a-molti).</item>
///   <item>Può avere blocchi padri e figli (relazione molti-a-molti ricorsiva).</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Il metodo <c>AddField</c> è riservato internamente a <see cref="JsonField"/> e presume che il campo sia già collegato al blocco.<br/>
/// Le relazioni con altri blocchi sono sempre bidirezionali.
/// Una volta aggiunte, non sono rimuovibili a runtime.
/// </para>
public sealed class JsonBlock : BaseEfEntity<JsonBlock>
{
    // 🔹 COSTRUTTORI 🔹

    /// <summary>
    /// Costruttore richiesto da Entity Framework Core.
    /// Inizializza anche la collezione dei campi con riferimento al blocco stesso.
    /// </summary>
    private JsonBlock() : base()
    {
        _jsonFields = new JsonBlockFieldCollection(this);
    }

    /// <summary>
    /// Costruttore privato usato dalla factory <see cref="Create"/>.
    /// Inizializza il validatore e la collezione dei campi.
    /// </summary>
    private JsonBlock(IValidateAndFix<JsonBlock>? validator)
        : base(validator)
    {
        _jsonFields = new JsonBlockFieldCollection(this);
    }

    // 🔹 FACTORY 🔹

    /// <summary>
    /// Crea un nuovo blocco JSON e lo registra nello schema specificato.
    /// </summary>
    /// <param name="schema">Schema di destinazione. Deve essere già valido.</param>
    /// <param name="name">Nome del blocco. Non può essere nullo o vuoto.</param>
    /// <param name="validator">Validatore opzionale da applicare al blocco.</param>
    /// <returns>Nuova istanza del blocco JSON, già registrata nello schema.</returns>
    public static JsonBlock Create(JsonSchema schema, string name, IValidateAndFix<JsonBlock>? validator = null)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Il nome del blocco non può essere vuoto.", nameof(name));

        var block = new JsonBlock(validator)
        {
            JsonSchema = schema,
            Name = name
        };

        // Tenta la registrazione: se fallisce, viene sollevata un’eccezione e il blocco non è considerato valido
        schema.AddBlock(block);

        return block;
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <inheritdoc/>
    protected override bool HasSlug => true;

    // 🔹 RELAZIONI EF 🔹

    [Required]
    internal int JsonSchemaId { get; }

    /// <summary>
    /// Schema JSON a cui appartiene il blocco.
    /// Viene assegnato automaticamente durante la registrazione con <see cref="JsonSchema.AddBlock"/>.
    /// </summary>
    [Required]
    public JsonSchema JsonSchema { get; internal set; } = null!;

    // 🔹 CAMPI 🔹

    /// <summary>
    /// Collezione interna dei campi JSON appartenenti al blocco.
    /// Inizializzata nel costruttore con riferimento al blocco stesso.
    /// </summary>
    private readonly JsonBlockFieldCollection _jsonFields;

    /// <summary>
    /// Espone i campi JSON in sola lettura.
    /// </summary>
    public IReadOnlyCollection<JsonField> JsonFields => _jsonFields.Fields;

    /// <summary>
    /// Indica se il blocco è indipendente, ovvero se ha un campo chiave associato.
    /// </summary>
    public bool IsIndependent => _jsonFields.GetKeyField() != null;

    // 🔹 RELAZIONI PADRE-FIGLIO 🔹

    private readonly HashSet<JsonBlock> _parentBlocks = [];
    private readonly HashSet<JsonBlock> _childBlocks = [];

    public IReadOnlyCollection<JsonBlock> ParentBlocks => _parentBlocks;
    public IReadOnlyCollection<JsonBlock> ChildBlocks => _childBlocks;

    /// <summary>
    /// Aggiunge un blocco figlio con relazione bidirezionale.
    /// </summary>
    public void AddChildBlock(JsonBlock childBlock)
    {
        if (_childBlocks.Add(childBlock))
            childBlock._parentBlocks.Add(this);
    }

    /// <summary>
    /// Aggiunge un blocco padre con relazione bidirezionale.
    /// </summary>
    public void AddParentBlock(JsonBlock parentBlock)
    {
        if (_parentBlocks.Add(parentBlock))
            parentBlock._childBlocks.Add(this);
    }

    // 🔹 GESTIONE CHIAVE 🔹

    public void MakeIndependent(JsonField keyField) => _jsonFields.AddKey(keyField);

    public void MakeIndependent(string fieldName) => _jsonFields.SetKey(fieldName);

    public bool MakeDependent() => _jsonFields.RemoveKey();

    public JsonField? GetKeyField() => _jsonFields.GetKeyField();

    // 🔹 USO INTERNO 🔹

    /// <summary>
    /// Aggiunge un campo alla collezione del blocco.  
    /// Presuppone che <c>JsonField.JsonBlock</c> sia già impostato e coerente.  
    /// Usato solo da <see cref="JsonField"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void AddField(JsonField field)
    {
        if (field.JsonBlock != this)
            throw new InvalidOperationException($"❌ Il campo '{field.SourceFieldPath}' non appartiene a questo blocco.");

        _jsonFields.Add(field);
    }

    // 🔹 VALIDAZIONE 🔹

    protected override void OnBeforeValidate() { }

    protected override void OnAfterValidate() { }

    // 🔹 TO STRING 🔹

    public override string ToString()
        => $"{Name} (Independent: {IsIndependent}, Fields: {_jsonFields}, Children: {ChildBlocks.Count})";
}