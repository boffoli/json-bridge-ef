using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.Collections;

/// <summary>
/// Domain Interface: Contratto per collezioni nominali che supportano una singola chiave logica.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità nominali, che devono implementare <see cref="INamed"/> e <see cref="IKeyed"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Espone operazioni per la gestione di un’entità marcata come chiave logica (<c>IsKey = true</c>) in collezioni con vincolo di unicità nominale.</para>
///
/// <para><b>Constraints:</b><br/>
/// - La chiave logica è unica per collezione.<br/>
/// - La sostituzione è consentita solo se <paramref name="force"/> è impostato a true.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// Per l’implementazione di riferimento, vedere <see cref="Collections.EfKeyedEntityCollection{TEntity}"/>.</para>
/// </remarks>
public interface IEfKeyedEntityCollection<TEntity> : IEfEntityCollection<TEntity>
    where TEntity : INamed, IKeyed
{
    /// <summary>
    /// Entità attualmente marcata come chiave logica, se presente.
    /// </summary>
    TEntity? KeyEntity { get; }

    /// <summary>
    /// Aggiunge un’entità e la imposta come chiave logica, con eventuale sovrascrizione.
    /// </summary>
    /// <param name="entity">Entità da marcare come chiave.</param>
    /// <param name="force">Se true, sovrascrive l’eventuale chiave esistente.</param>
    void AddKey(TEntity entity, bool force = false);

    /// <summary>
    /// Imposta come chiave un’entità esistente, cercandola per nome.
    /// </summary>
    /// <param name="name">Nome dell’entità da marcare.</param>
    /// <param name="force">Se true, sovrascrive l’eventuale chiave esistente.</param>
    void MarkAsKey(string name, bool force = false);

    /// <summary>
    /// Rimuove la marcatura di chiave logica, se presente.
    /// </summary>
    /// <returns><c>true</c> se è stata rimossa una chiave logica; <c>false</c> altrimenti.</returns>
    bool UnmarkAsKey();
}