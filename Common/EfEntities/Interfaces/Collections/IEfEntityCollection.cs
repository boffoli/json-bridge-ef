using JsonBridgeEF.Common.EfEntities.Base;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.Collections;

/// <summary>
/// Domain Interface: Contratto per collezioni nominali Entity Framework che garantiscono unicità per nome.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono implementare <see cref="INamed"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Interfaccia base per collezioni contenenti entità con nome univoco.
/// Fornisce metodi per aggiunta e ricerca, senza imporre vincoli di ownership o chiave logica.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Le entità devono avere un nome univoco (case-insensitive).<br/>
/// - Può essere implementata da collezioni standalone o collezioni specializzate.</para>
/// </remarks>
public interface IEfEntityCollection<TEntity>
    where TEntity : INamed
{
    /// <summary>
    /// Collezione in sola lettura delle entità contenute.
    /// </summary>
    IReadOnlyCollection<TEntity> Entities { get; }

    /// <summary>
    /// Cerca un’entità per nome, ignorando maiuscole e minuscole.
    /// </summary>
    /// <param name="name">Nome dell’entità da cercare.</param>
    /// <returns>Entità trovata, oppure <c>null</c> se assente.</returns>
    TEntity? FindByName(string name);

    /// <summary>
    /// Aggiunge un’entità alla collezione, garantendo l’unicità del nome.
    /// </summary>
    /// <param name="entity">Entità da aggiungere.</param>
    void Add(TEntity entity);
}