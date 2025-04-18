using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Validators;
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
            // 1) validazione base di figli e chiave
            base.EnsureValid(entity);
            ValidateKey(entity);

            // 2) validazione gerarchia parentale
            ValidateParents(entity);
            ValidateSelfReference(entity);
        }

        /// <inheritdoc/>
        public void Fix(IEntity<TSelf, TEntityProperty> entity)
        {
            base.Fix(entity);
            // Nessuna correzione aggiuntiva a questo livello
        }

        // ======================== CHIAVE ========================

        private static void ValidateKey(IEntity<TSelf, TEntityProperty> entity)
        {
            if (entity.IsIdentifiable() && entity.GetKeyProperty() == null)
                throw new ValidationException(
                    $"L'entità '{entity.Name}' è marcata come identificabile ma non espone una proprietà chiave.");
        }

        // ==================== PARENT / CHILD INTEGRITY ====================

        private static void ValidateParents(IEntity<TSelf, TEntityProperty> entity)
        {
            var parents = entity.Parents ?? throw new ValidationException("La collezione dei genitori non può essere nulla.");
            if (parents.Distinct().Count() != parents.Count)
                throw new ValidationException("Sono presenti genitori duplicati nella collezione.");
        }

        private static void ValidateSelfReference(IEntity<TSelf, TEntityProperty> entity)
        {
            // l'entità stessa è sempre di tipo TSelf
            var self = (TSelf)entity;

            if (entity.Parents.Contains(self) || entity.SelfChildren.Contains(self))
                throw new ValidationException(
                    $"Un'entità non può essere suo proprio genitore o figlio: '{entity.Name}'.");
        }
    }
}