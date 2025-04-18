using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// Domain Class: Blocco JSON che rappresenta una sezione logica di uno schema,
/// contenente una collezione di campi univoci per nome, uno dei quali può essere marcato come chiave.
/// Partecipa inoltre a una struttura gerarchica tra blocchi JSON.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Ogni blocco appartiene a uno <see cref="JsonSchema"/> e funge da aggregate root locale per un insieme
/// coerente di <see cref="JsonField"/>.<br/>
/// Può anche essere collegato gerarchicamente ad altri blocchi.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creato tramite il costruttore dominio, fornendo nome e schema.
/// Il costruttore EF è riservato al framework.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il nome è obbligatorio e univoco all'interno dello schema.<br/>
/// - Solo un campo può essere marcato come chiave.<br/>
/// - Le relazioni padre-figlio tra blocchi devono essere coerenti e simmetriche.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Appartiene a uno <see cref="JsonSchema"/> (owner).<br/>
/// - Aggrega <see cref="JsonField"/> come entità figlie owned e keyed.<br/>
/// - Può avere blocchi genitori e figli tramite interfaccia <see cref="IHierarchical{JsonEntities}"/>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usare <see cref="AddEntity(JsonField)"/> per aggiungere un campo.<br/>
/// - Usare <see cref="GetKeyEntity()"/> per ottenere il campo chiave, se presente.<br/>
/// - Usare <see cref="AddChild(JsonEntities)"/> e <see cref="AddParent(JsonEntities)"/> per relazioni gerarchiche.</para>
/// </remarks>
public sealed class JsonEntities2 
    : BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities<JsonEntities, JsonSchema, JsonField>
{
    // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹
#pragma warning disable S1133
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    private JsonEntities() : base() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    // 🔹 COSTRUTTORE DOMINIO 🔹

    /// <summary>
    /// Domain Constructor: Inizializza un nuovo blocco JSON appartenente a uno schema.
    /// </summary>
    /// <param name="name">Nome del blocco (univoco all'interno dello schema).</param>
    /// <param name="schema">Schema JSON di appartenenza.</param>
    /// <param name="validator">Validatore opzionale per le regole di dominio.</param>
    public JsonEntities(string name, JsonSchema schema, IValidateAndFix<JsonEntity>? validator)
        : base(name, schema, validator)
    {
    }

    // 🔹 DOMAIN PROPERTIES 🔹

    /// <summary>
    /// Determina se il blocco è "indipendente", cioè se esiste almeno un campo JSON marcato come chiave.
    /// </summary>
    /// <returns><c>true</c> se è presente un campo chiave; altrimenti <c>false</c>.</returns>
    public bool IsIndependent()
    {
        // Se la collezione di campi ha una KeyEntity, significa che il blocco ha un campo marcato come chiave
        return KeyEntity != null;
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <inheritdoc />
    protected sealed override bool HasSlug => true;

    // 🔹 GESTIONE CHIAVE 🔹

/// <summary>
/// Imposta il campo specificato come chiave logica del blocco.
/// </summary>
/// <param name="keyField">Campo da marcare come chiave.</param>
/// <param name="force">Se true, sostituisce un’eventuale chiave esistente.</param>
public void MakeIndependent(JsonField keyField, bool force = false)
{
    // Usa la collezione per marcare il campo come chiave
    _keyedCollection.AddKey(keyField, force);
}

/// <summary>
/// Imposta come chiave il campo con il nome specificato.
/// </summary>
/// <param name="fieldName">Nome del campo da marcare come chiave.</param>
/// <param name="force">Se true, sostituisce un’eventuale chiave esistente.</param>
/// <exception cref="KeyedEntityNotFoundException">
/// Se il campo con il nome indicato non esiste nella collezione.
/// </exception>
public void MakeIndependent(string fieldName, bool force = false)
{
    _keyedCollection.MarkAsKey(fieldName, force);
}

/// <summary>
/// Rimuove la chiave logica dal blocco, rendendolo "dipendente".
/// </summary>
/// <returns><c>true</c> se una chiave è stata rimossa, <c>false</c> altrimenti.</returns>
public bool MakeDependent()
{
    return _keyedCollection.UnmarkAsKey();
}

    // 🔹 VALIDAZIONE 🔹

    /// <inheritdoc/>
    protected sealed override void OnBeforeValidate() { }

    /// <inheritdoc/>
    protected sealed override void OnAfterValidate() { }

    // 🔹 TO STRING 🔹

    /// <inheritdoc/>
    public override string ToString()
        => $"{Name} (Fields: {Entities.Count}, Key: {KeyEntity?.Name ?? "None"})";
}