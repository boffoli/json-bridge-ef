using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Entità persistente che rappresenta un elemento posseduto da un aggregato,
/// con supporto completo per Entity Framework Core.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell'entità derivata (self-reference).</typeparam>
/// <typeparam name="TOwner">Tipo dell'entità proprietaria (aggregate root).</typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Questa classe rappresenta un'entità "owned", cioè parte strutturale e logica di un aggregate root.
/// Stabilisce una relazione molti-a-uno con l'owner.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Deve essere istanziata tramite factory method o costruttore protetto, specificando nome e owner.
/// Il costruttore vuoto è riservato alla materializzazione EF.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il riferimento a <c>Owner</c> è obbligatorio e non può essere null.<br/>
/// - <c>OwnerId</c> è gestito da EF Core come chiave esterna.<br/>
/// - L’owner deve implementare <see cref="IOwner{TSelf}"/> per supportare l’inserimento.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Implementa <see cref="IOwned{TOwner}"/>.<br/>
/// - L’owner è responsabile della gestione della collezione e della coerenza dell’oggetto figlio.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Usare per modellare configurazioni, parametri, attributi, strategie, sezioni, ecc.<br/>
/// - EF Core popola <c>Owner</c> e <c>OwnerId</c> automaticamente. L’associazione avviene anche nel costruttore.</para>
/// </remarks>
public abstract class BaseEfOwnedEntity<TSelf, TOwner> 
    : BaseEfEntity<TSelf>, IOwned<TOwner>
    where TSelf : BaseEfOwnedEntity<TSelf, TOwner>
    where TOwner : IOwner<TSelf>
{
    // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹

#pragma warning disable S1133 // deprecated
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618 // will be initialized by EF
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfOwnedEntity() : base()
    {
        // ⚠️ EF Core materialization only
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    // 🔹 COSTRUTTORE PROTETTO CON OWNER 🔹

    /// <summary>
    /// Domain Constructor: Inizializza l'entità con nome e owner.
    /// </summary>
    /// <param name="name">Nome dell'entità.</param>
    /// <param name="owner">Istanza del proprietario (aggregate root).</param>
    /// <param name="validator">Validatore opzionale per la business rule dell’entità.</param>
    protected BaseEfOwnedEntity(string name, TOwner owner, IValidateAndFix<TSelf>? validator)
        : base(name, validator)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));

        // ✅ Registrazione automatica all’interno del proprietario
        owner.AddEntity((TSelf)this);
    }

    // 🔹 IMPLEMENTAZIONE IOwned<TOwner> 🔹

    /// <inheritdoc />
    [Required]
    public TOwner Owner { get; private set; }

    /// <inheritdoc />
    public int OwnerId { get; private set; }
}