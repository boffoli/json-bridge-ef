using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Collections;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Collezione di blocchi JSON associati a uno schema.
/// Garantisce unicità per nome e verifica la coerenza tra blocco e schema.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Ogni blocco deve avere un nome univoco (case-insensitive).</item>
///   <item>Ogni blocco deve appartenere esattamente allo schema proprietario.</item>
///   <item>I blocchi possono essere indipendenti o dipendenti, ma non duplicati.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// Non consente la rimozione diretta dei blocchi.<br/>
/// Deve essere manipolata solo internamente da <see cref="JsonSchema"/>.
/// </para>
internal sealed class JsonSchemaBlockCollection
{
    private readonly JsonSchema _owner;
    private readonly HashSet<JsonBlock> _blocks = new(JsonBlockComparer.Instance);

    /// <summary>
    /// Inizializza la collezione per un determinato schema proprietario.
    /// </summary>
    /// <param name="owner">Istanza dello schema proprietario.</param>
    public JsonSchemaBlockCollection(JsonSchema owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /// <summary>
    /// Restituisce tutti i blocchi in sola lettura.
    /// </summary>
    public IReadOnlyCollection<JsonBlock> Items => _blocks;

    /// <summary>
    /// Restituisce i blocchi indipendenti (con chiave definita).
    /// </summary>
    public IEnumerable<JsonBlock> IndependentBlocks => _blocks.Where(b => b.IsIndependent);

    /// <summary>
    /// Restituisce i blocchi dipendenti (senza chiave).
    /// </summary>
    public IEnumerable<JsonBlock> DependentBlocks => _blocks.Where(b => !b.IsIndependent);

    /// <summary>
    /// Aggiunge un blocco alla collezione.
    /// Verifica la coerenza dello schema e l'unicità del nome.
    /// </summary>
    /// <param name="block">Blocco da aggiungere.</param>
    /// <exception cref="ArgumentNullException">Se il blocco è null.</exception>
    /// <exception cref="InvalidOperationException">Se il blocco appartiene a un altro schema.</exception>
    /// <exception cref="BlockAlreadyExistsException">Se un blocco con lo stesso nome è già presente.</exception>
    public void Add(JsonBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);
        EnsureBelongsToThisSchema(block);
        EnsureUniqueName(block);
        _blocks.Add(block);
    }

    /// <summary>
    /// Verifica che il blocco sia associato allo schema proprietario.
    /// </summary>
    private void EnsureBelongsToThisSchema(JsonBlock block)
    {
        if (!ReferenceEquals(block.JsonSchema, _owner))
            throw new InvalidOperationException($"❌ Il blocco '{block.Name}' non appartiene allo schema '{_owner.Name}'.");
    }

    /// <summary>
    /// Verifica che non esista già un blocco con lo stesso nome.
    /// </summary>
    private void EnsureUniqueName(JsonBlock block)
    {
        if (_blocks.Contains(block))
            throw new BlockAlreadyExistsException(block.Name);
    }

    public override string ToString()
    {
        const int maxBlocks = 3;
        var names = _blocks.Select(b => b.Name).Take(maxBlocks).ToList();
        if (_blocks.Count > maxBlocks)
            names.Add("...");
        return string.Join(", ", names);
    }

    #region 🔒 Comparer

    /// <summary>
    /// Comparatore interno per garantire l’unicità basata su <c>Name</c>.
    /// </summary>
    private sealed class JsonBlockComparer : IEqualityComparer<JsonBlock>
    {
        public static readonly JsonBlockComparer Instance = new();

        public bool Equals(JsonBlock? x, JsonBlock? y)
            => x != null && y != null && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(JsonBlock obj)
            => obj.Name.ToLowerInvariant().GetHashCode();
    }

    #endregion
}