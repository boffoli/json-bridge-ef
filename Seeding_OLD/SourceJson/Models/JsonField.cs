using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Seeding.SourceJson.Models;

/// <summary>
/// Domain Entity: Campo JSON appartenente a un <see cref="JsonEntities"/>, identificato da un nome univoco
/// e da un percorso logico all'interno della struttura JSON. Può essere designato come chiave logica del blocco.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Un <c>JsonField</c> rappresenta una proprietà di un oggetto JSON, mappata come sotto-elemento di un <see cref="JsonEntities"/>.
/// Ogni campo ha un nome, un percorso sorgente nel JSON, e può essere utilizzato come identificatore logico (chiave).</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere creato tramite costruttore esplicito fornendo nome, blocco di appartenenza, e percorso sorgente.<br/>
/// La registrazione nel blocco avviene automaticamente nel costruttore base.</para>
///
/// <para><b>Constraints:</b><br/>
/// - <c>Name</c> e <c>SourceFieldPath</c> sono obbligatori.<br/>
/// - <c>SourceFieldPath</c> non può essere vuoto o nullo.<br/>
/// - Il blocco di appartenenza deve essere valido e coerente con il tipo <see cref="JsonEntities"/>.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Appartiene a un <see cref="JsonEntities"/> che implementa <see cref="IWithKeyedEntities{JsonField}"/>.<br/>
/// - Registrato automaticamente nella collezione del blocco al momento della creazione.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// Utilizzare per modellare proprietà dinamiche dei blocchi JSON, eventualmente marcabili come chiave logica.
/// Il valore <see cref="IsKey"/> viene gestito dalla collezione nel blocco.</para>
public sealed class JsonField2 : BaseEfKeyedOwnedEntity<JsonField, JsonEntities>
{
    // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹

    /// <summary>
    /// Infrastructure Constructor: Riservato a Entity Framework Core per la materializzazione da database.
    /// </summary>
#pragma warning disable S1133
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    private JsonField() : base()
    {
        // ⚠️ EF Core popolerà i campi tramite reflection
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    // 🔹 COSTRUTTORE PUBBLICO 🔹

    /// <summary>
    /// Domain Constructor: Crea un nuovo campo JSON con nome, blocco e percorso sorgente.
    /// </summary>
    /// <param name="name">Nome univoco del campo all’interno del blocco.</param>
    /// <param name="jsonEntity">Blocco JSON a cui il campo appartiene.</param>
    /// <param name="validator">Validatore opzionale per la logica di dominio.</param>
    public JsonField(string name, JsonEntities jsonEntity, IValidateAndFix<JsonField>? validator)
        : base(name, jsonEntity, validator)
    {
    }

    // 🔹 CONFIGURAZIONE 🔹

    /// <summary>
    /// Disattiva la generazione automatica dello slug per i campi JSON.
    /// </summary>
    protected sealed override bool HasSlug => false;

    // 🔹 VALIDAZIONE 🔹

    /// <inheritdoc/>
    protected sealed override void OnBeforeValidate() { }

    /// <inheritdoc/>
    protected sealed override void OnAfterValidate() { }
}