using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Validators;
using JsonBridgeEF.Shared.EntityModel.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Model;

namespace JsonBridgeEF.Shared.EntityModel.Validators;

/// <summary>
/// Domain Concept: Validatore per proprietà entità (<see cref="EntityProperty{TSelf,TEntity}"/>).
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Usato nei validatori di alto livello (es. <c>JsonPropertyValidator</c>).</para>
/// <para>Constraints: Il nome deve essere valido, la relazione parent deve essere coerente.</para>
/// <para>Relationships: Deriva da <see cref="ValueNodeValidator{TSelf,TAggregate}"/>.</para>
/// <para>Usage Notes: Può essere usato per tutte le entità che estendono <see cref="IEntityProperty{TSelf,TEntity}"/>.</para>
/// </remarks>
internal class EntityPropertyValidator<TSelf, TEntity>
    : ValueNodeValidator<TSelf, TEntity>, IValidateAndFix<IEntityProperty<TSelf, TEntity>>
    where TSelf : class, IEntityProperty<TSelf, TEntity>
    where TEntity : class, IEntity<TEntity, TSelf>
{
    /// <inheritdoc />
    public void EnsureValid(IEntityProperty<TSelf, TEntity> entityProperty)
    {
        base.EnsureValid(entityProperty);
        // Non ci sono ulteriori vincoli oltre quelli definiti nella base
    }

    /// <inheritdoc />
    public void Fix(IEntityProperty<TSelf, TEntity> entityProperty)
    {
        base.Fix(entityProperty);
        // Nessuna correzione automatica aggiuntiva
    }
}