namespace JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

/// <summary>
/// Domain Interface: Rappresenta un'entità che partecipa a una struttura gerarchica
/// bidirezionale (genitore-figlio), abilitando la navigazione verso i parent e i child.
/// </summary>
/// <typeparam name="TSelf">
/// Tipo concreto dell'entità che implementa la gerarchia (self-reference).
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Le entità che implementano <c>IHierarchical</c> fanno parte di una struttura
/// ad albero o grafo aciclico in cui ciascuna entità può avere più genitori e più figli.
/// </para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Da implementare nelle entità che necessitano di relazioni bidirezionali padre-figlio.
/// Le collezioni dovrebbero essere inizializzate nel costruttore e non esposte in scrittura.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// - Un'entità non può essere suo proprio genitore o figlio.<br/>
/// - Le relazioni dovrebbero essere simmetriche e mantenute coerenti.
/// </para>
///
/// <para><b>Relationships:</b><br/>
/// Ogni entità può far parte di più catene gerarchiche. Le relazioni vanno mantenute coerenti
/// tramite chiamate a <see cref="AddParent"/> e <see cref="AddChild"/>.
/// </para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Utile per modellare tassonomie, alberi delle dipendenze, workflow gerarchici, ecc.<br/>
/// - Compatibile con Entity Framework Core con relazioni molti-a-molti ricorsive.
/// </para>
/// </remarks>
public interface IHierarchical<TSelf>
    where TSelf : IHierarchical<TSelf>
{
    /// <summary>
    /// Collezione delle entità genitrici (nodi superiori nella gerarchia).
    /// </summary>
    IReadOnlyCollection<TSelf> Parents { get; }

    /// <summary>
    /// Collezione delle entità figlie (nodi inferiori nella gerarchia).
    /// </summary>
    IReadOnlyCollection<TSelf> Children { get; }

    /// <summary>
    /// Aggiunge un'entità come figlia, stabilendo la relazione gerarchica.
    /// </summary>
    /// <param name="child">Entità da aggiungere come figlia.</param>
    void AddChild(TSelf child);

    /// <summary>
    /// Aggiunge un'entità come genitore, stabilendo la relazione gerarchica.
    /// </summary>
    /// <param name="parent">Entità da aggiungere come genitore.</param>
    void AddParent(TSelf parent);
}