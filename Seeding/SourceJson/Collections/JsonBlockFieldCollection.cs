using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Collections;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Rappresenta una collezione di campi JSON appartenenti a un blocco, garantendo unicità per nome e una sola chiave.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Ogni campo deve avere un nome univoco (case-insensitive).</item>
///   <item>Solo un campo può essere marcato come chiave per volta.</item>
///   <item>Il campo chiave può essere sostituito solo in modo esplicito.</item>
///   <item>Ogni campo deve appartenere al blocco che possiede questa collezione.</item>
/// </list>
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// La collezione non consente la rimozione diretta dei campi.  
/// Può essere modificata solo internamente al blocco <see cref="JsonBlock"/>.
/// </para>
internal sealed class JsonBlockFieldCollection
{
    private readonly HashSet<JsonField> _fields = new(JsonFieldComparer.Instance);
    private readonly JsonBlock _ownerBlock;

    public JsonBlockFieldCollection(JsonBlock ownerBlock)
    {
        _ownerBlock = ownerBlock ?? throw new ArgumentNullException(nameof(ownerBlock));
    }

    /// <summary>
    /// Restituisce tutti i campi come collezione in sola lettura.
    /// </summary>
    public IReadOnlyCollection<JsonField> Fields => _fields;

    /// <summary>
    /// Restituisce il campo chiave, se presente.
    /// </summary>
    public JsonField? GetKeyField()
        => _fields.FirstOrDefault(f => f.IsKey);

    /// <summary>
    /// Trova un campo per nome (case-insensitive).
    /// </summary>
    public JsonField? FindByName(string fieldName)
        => _fields.FirstOrDefault(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Aggiunge un campo alla collezione.
    /// - Verifica che il campo appartenga al blocco corretto.
    /// - Solleva eccezione se già presente.
    /// - Se è una chiave, applica i vincoli specifici.
    /// </summary>
    public void Add(JsonField field)
    {
        // 🟡 STEP 1: Verifica appartenenza al blocco
        EnsureFieldBelongsToCurrentBlock(field);

        // 🟡 STEP 2: Se è chiave, delega a AddKey (che include la logica di Add)
        if (field.IsKey)
        {
            AddKey(field);
            return;
        }

        // 🟡 STEP 3: Aggiunta semplice con verifica unicità nome
        if (!_fields.Add(field))
            throw new JsonFieldAlreadyExistsException(field.Name, _ownerBlock.Name);
    }

    /// <summary>
    /// Aggiunge o imposta un campo come chiave, verificando i vincoli.
    /// - Controlla appartenenza al blocco
    /// - Verifica conflitti con chiavi esistenti
    /// </summary>
    public void AddKey(JsonField field, bool force = false)
    {
        EnsureFieldBelongsToCurrentBlock(field);

        var existingKey = GetKeyField();

        // 🔁 STEP 1: Se già chiave attuale, non fare nulla
        if (existingKey != null && ReferenceEquals(existingKey, field))
            return;

        // 🔁 STEP 2: Se c'è un'altra chiave e non è forzato, solleva eccezione
        if (existingKey != null && !force)
            throw new JsonFieldKeyAlreadyPresentException(existingKey.Name, _ownerBlock.Name);

        // 🔁 STEP 3: Se forzato, rimuovi la chiave esistente e procedi
        if (existingKey != null && force)
            existingKey.IsKey = false;

        // 🔁 STEP 4: Aggiungi il campo (se non già presente)
        if (!_fields.Add(field))
            throw new JsonFieldAlreadyExistsException(field.Name, _ownerBlock.Name);

        field.IsKey = true;
    }

    /// <summary>
    /// Imposta un campo già presente come chiave, cercandolo per nome.
    /// </summary>
    public void SetKey(string fieldName, bool force = false)
    {
        var field = FindByName(fieldName);
        if (field == null)
            throw new JsonFieldNotFoundException(fieldName, _ownerBlock.Name);

        AddKey(field, force);
    }

    /// <summary>
    /// Rimuove lo stato di chiave dal campo attuale, se presente.
    /// </summary>
    public bool RemoveKey()
    {
        var existingKey = GetKeyField();
        if (existingKey == null)
            return false;

        existingKey.IsKey = false;
        return true;
    }

    public override string ToString()
    {
        const int maxFields = 3;
        var names = _fields.Select(f => f.Name).Take(maxFields).ToList();
        if (_fields.Count > maxFields)
            names.Add("...");
        return string.Join(", ", names);
    }

    // 🔒 VALIDAZIONI INTERNE 🔒

    /// <summary>
    /// Verifica che il campo appartenga al blocco proprietario della collezione.
    /// </summary>
    private void EnsureFieldBelongsToCurrentBlock(JsonField field)
    {
        if (!ReferenceEquals(field.JsonBlock, _ownerBlock))
            throw new InvalidOperationException(
                $"Il campo '{field.Name}' non appartiene al blocco '{_ownerBlock.Name}'."
            );
    }

    #region 🔒 Comparer

    /// <summary>
    /// Comparatore interno per garantire l’unicità basata su <c>Name</c>.
    /// </summary>
    private sealed class JsonFieldComparer : IEqualityComparer<JsonField>
    {
        public static readonly JsonFieldComparer Instance = new();

        public bool Equals(JsonField? x, JsonField? y)
            => x != null && y != null && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(JsonField obj)
            => obj.Name.ToLowerInvariant().GetHashCode();
    }

    #endregion
}