using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.With;

/// <summary>
/// Domain Interface: Rappresenta un contenitore che gestisce una collezione di entità identificabili per nome
/// e con la possibilità di designare una come chiave logica.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità, che devono implementare <see cref="IKeyed"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Estende la semantica delle collezioni nominali introducendo una chiave logica centrale,
/// utile per distinguere l’entità principale o attiva tra le altre.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Implementabile nei contenitori che richiedono selezione di una entità dominante.
/// La selezione viene mantenuta a livello di proprietà o gestita dinamicamente da una collezione specializzata.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Solo un'entità può essere marcata con <c>IsKey = true</c>.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// Usata in scenari di selezione attiva, configurazioni primarie o fallback logici.
/// I metodi `MarkAsKey` supportano un parametro `force` con valore predefinito `false` per controllare la sostituzione.</para>
/// </remarks>
public interface IWithKeyedEntities<TEntity> : IWithEntities<TEntity>
    where TEntity : IKeyed
{
    /// <summary>
    /// Restituisce l’entità attualmente designata come chiave logica.
    /// </summary>
    TEntity? GetKeyEntity();
}