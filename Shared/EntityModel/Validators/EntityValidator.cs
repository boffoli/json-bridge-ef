using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Validators;
using JsonBridgeEF.Shared.EntityModel.Exceptions;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Shared.EntityModel.Validators
{
    /// <summary>
    /// Domain Concept: Validatore per entità aggregate (<see cref="IEntity{TSelf, TEntityProperty}"/>).
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Usato come validatore per tutte le entità che estendono <see cref="Entity{TSelf, TEntityProperty}"/>.</para>
    /// <para><b>Constraints:</b><br/>
    /// Valida la coerenza interna delle proprietà e l’unicità della chiave logica.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="AggregateNodeValidator{TSelf, TEntityProperty}"/> 
    /// e implementa <see cref="IValidateAndFix{IEntity{TSelf, TEntityProperty}}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Include controlli sui genitori e auto‐referenza per mantenere l’integrità di un DAG.</para>
    /// </remarks>
    internal class EntityValidator<TSelf, TEntityProperty>
        : AggregateNodeValidator<TSelf, TEntityProperty>,
          IValidateAndFix<IEntity<TSelf, TEntityProperty>>
        where TSelf           : class, IEntity<TSelf, TEntityProperty>
        where TEntityProperty: class, IEntityProperty<TEntityProperty, TSelf>
    {
        /// <inheritdoc/>
        public void EnsureValid(IEntity<TSelf, TEntityProperty> entity)
        {
            base.EnsureValid(entity);
            ValidateKey(entity);
            ValidateParents(entity);
            ValidateSelfReference(entity);
        }

        /// <inheritdoc/>
        public void Fix(IEntity<TSelf, TEntityProperty> entity)
        {
            base.Fix(entity);
        }

        // ======================== CHIAVE ========================

        private static void ValidateKey(IEntity<TSelf, TEntityProperty> entity)
        {
            if (entity.IsIdentifiable() && entity.GetKeyProperty() is null)
                throw EntityError.MissingKey(entity.Name);
        }

        // ==================== PARENT / CHILD INTEGRITY ====================

        private static void ValidateParents(IEntity<TSelf, TEntityProperty> entity)
        {
            if (entity.Parents is null)
                throw EntityError.NullParentCollection(entity.Name);

            if (entity.Parents.Distinct().Count() != entity.Parents.Count)
                throw EntityError.DuplicateParents(entity.Name);
        }

        private static void ValidateSelfReference(IEntity<TSelf, TEntityProperty> entity)
        {
            var self = (TSelf)entity;

            if (entity.Parents.Contains(self) || entity.SelfChildren.Contains(self))
                throw EntityError.SelfReference(entity.Name);
        }
    }
}
