using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using JsonBridgeEF.Common.EfEntities.Managers;
using JsonBridgeEF.Common.Validators;

namespace JsonBridgeEF.Common.EfEntities.Base;

/// <summary>
/// Domain Class: Entità persistente con supporto alla gerarchia bidirezionale genitore-figlio,
/// costruita su Entity Framework Core.
/// </summary>
/// <typeparam name="TSelf">Tipo concreto dell’entità (self-reference).</typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Questa entità fa parte di una struttura gerarchica, come tassonomie, workflow, dipendenze o composizioni
/// ricorsive. Le relazioni sono gestite internamente tramite un manager specializzato.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Usare un costruttore protetto che riceve nome e validatore. Il manager è istanziato internamente.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Le relazioni sono mantenute simmetriche tramite <see cref="HierarchicalEntityManager{TSelf}"/>.<br/>
/// - Un'entità non può essere genitore/figlio di sé stessa.</para>
///
/// <para><b>Relationships:</b><br/>
/// - Implementa <see cref="IHierarchical{TSelf}"/> per navigare parent e child.<br/>
/// - Utilizza <see cref="HierarchicalEntityManager{TSelf}"/> per la logica di coerenza delle relazioni.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Adatto a scenari con dipendenze circolari controllate o strutture ad albero multiple.<br/>
/// - Non supporta la rimozione o la validazione dei cicli esplicita: la responsabilità è del dominio.</para>
/// </remarks>
public abstract class BaseEfHierarchicalEntity<TSelf>
    : BaseEfEntity<TSelf>, IHierarchical<TSelf>
    where TSelf : BaseEfHierarchicalEntity<TSelf>, IHierarchical<TSelf>
{
    private readonly HierarchicalEntityManager<TSelf> _manager;

    #region EF Core Constructor

#pragma warning disable S1133 // Deprecated code
    [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected BaseEfHierarchicalEntity()
    {
        _manager = new HierarchicalEntityManager<TSelf>((TSelf)this);
    }
#pragma warning restore CS8618
#pragma warning restore S1133

    #endregion

    #region Domain Constructor

    /// <summary>
    /// Costruttore protetto per inizializzare nome e validatore.
    /// </summary>
    /// <param name="name">Nome dell'entità.</param>
    /// <param name="validator">Validatore opzionale per le regole di dominio.</param>
    protected BaseEfHierarchicalEntity(string name, IValidateAndFix<TSelf>? validator)
        : base(name, validator)
    {
        _manager = new HierarchicalEntityManager<TSelf>((TSelf)this);
    }

    #endregion

    #region Hierarchical Implementation

    /// <inheritdoc/>
    public IReadOnlyCollection<TSelf> Parents => _manager.Parents;

    /// <inheritdoc/>
    public IReadOnlyCollection<TSelf> Children => _manager.Children;

    /// <inheritdoc/>
    public void AddChild(TSelf child) => _manager.AddChild(child);

    /// <inheritdoc/>
    public void AddParent(TSelf parent) => _manager.AddParent(parent);

    #endregion
}