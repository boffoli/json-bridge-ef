using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;
using System.Collections.Generic;

namespace JsonBridgeEF.Common.EfEntities.Interfaces.Collections;

/// <summary>
/// Domain Interface: Contratto per collezioni specializzate in entità <see cref="IOwned{TOwner}"/> con unicità per nome
/// e coerenza con l’owner di riferimento.
/// </summary>
/// <typeparam name="TEntity">
/// Tipo delle entità contenute, che devono implementare <see cref="INamed"/> e <see cref="IOwned{TOwner}"/>.
/// </typeparam>
/// <typeparam name="TOwner">
/// Tipo dell’entità proprietaria (aggregate root), che implementa <see cref="IOwner{TEntity}"/>.
/// </typeparam>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Contratto per collezioni che rappresentano insiemi consistenti di entità appartenenti a un owner univoco.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Espone solo le operazioni pubbliche rilevanti (es. <c>Add</c>, <c>Entities</c>, <c>FindByName</c>).<br/>
/// - I vincoli di appartenenza all’owner sono implementativi e non fanno parte del contratto esterno.</para>
/// </remarks>
public interface IEfOwnedEntityCollection<TEntity, TOwner> : IEfEntityCollection<TEntity>
    where TEntity : INamed, IOwned<TOwner>
    where TOwner : class, IOwner<TEntity>
{
}