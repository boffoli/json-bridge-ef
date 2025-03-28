using JsonBridgeEF.Common.EfEntities.Base;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.With;

/// <summary>
/// Domain Interface: Rappresenta un contenitore che gestisce una collezione di entità identificabili univocamente tramite nome.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità, che devono implementare <see cref="INamed"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Collezione identificata per nome, dove il nome rappresenta la chiave logica. Utile per configurazioni simboliche, alias, o lookup nominali.</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// Implementabile in qualsiasi oggetto che debba contenere entità con nome univoco.
/// Il confronto può essere case-insensitive, delegato alla collezione o a comparatori esterni.</para>
///
/// <para><b>Constraints:</b><br/>
/// - Il <c>Name</c> delle entità deve essere univoco all'interno della collezione.</para>
///
/// <para><b>Relationships:</b><br/>
/// Ogni entità deve implementare <see cref="INamed"/> per fornire una chiave nominale significativa.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// Usata in modelli configurabili o per mapping simbolici tra chiavi e comportamenti.</para>
/// </remarks>
public interface IWithNamedEntities<TEntity> : IWithEntities<TEntity>
    where TEntity : INamed
{
}