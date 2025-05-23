using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Model;
using JsonBridgeEF.Shared.EntityModel.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Validators;

namespace JsonBridgeEF.Shared.EntityModel.Model
{
    /// <summary>
    /// Implementazione astratta di <see cref="IEntityProperty{TSelf, TEntity}"/>.
    /// </summary>
    /// <remarks>
    /// Fornisce il costruttore base per una proprietà di entità, 
    /// programmando verso l’interfaccia per il parent.
    /// </remarks>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della proprietà che estende questa classe.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// Il tipo dell’entità proprietaria, che implementa <see cref="IEntity{TEntity, TSelf}"/>.
    /// </typeparam>
    /// <param name="name">Nome della proprietà (obbligatorio e immutabile).</param>
    /// <param name="parent">
    /// Entità proprietaria, dipendente dall’astrazione <see cref="IEntity{TEntity,TSelf}"/>.
    /// </param>
    /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
    /// <param name="validator">Validatore opzionale per questa proprietà.</param>
    internal abstract class EntityProperty<TSelf, TEntity>(
        string name,
        IEntity<TEntity, TSelf> parent,
        bool isKey,
        IValidateAndFix<TSelf>? validator
    ) : ValueNode<TSelf, TEntity>(
            name,
            (TEntity)parent!,
            validator
        ), IEntityProperty<TSelf, TEntity>
        where TSelf : EntityProperty<TSelf, TEntity>, IEntityProperty<TSelf, TEntity>
        where TEntity : class, IEntity<TEntity, TSelf>
    {
        private bool _isKey = isKey;

        /// <summary>
        /// Indica se questa proprietà è designata come chiave logica.
        /// </summary>
        /// <remarks>
        /// <b>Access:</b> La proprietà è di sola lettura per il contratto di dominio, 
        /// ma se necessario può essere modificata tramite il metodo <see cref="SetKeyStatus(bool)"/>.
        /// </remarks>
        public bool IsKey => _isKey;

        /// <summary>
        /// Modifica lo stato della proprietà chiave.
        /// </summary>
        /// <param name="isKey">Il nuovo stato chiave.</param>
        /// <remarks>
        /// Questo metodo permette di modificare il flag che indica se la proprietà è una chiave logica.
        /// Considera che la modifica dello stato chiave potrebbe avere impatti sul dominio.
        /// </remarks>
        protected void SetKeyStatus(bool isKey)
        {
            _isKey = isKey;
        }

        /// <inheritdoc />
        protected sealed override bool EqualsCore(Node<TSelf> other)
        {
            if (other is not EntityProperty<TSelf, TEntity> otherProp)
                return false;

            // Uguaglianza logica di base (nome)
            if (!string.Equals(Name, otherProp.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            // Delega all’hook specifico
            return EqualsByValue((TSelf)otherProp);
        }

        /// <inheritdoc />
        protected sealed override int GetHashCodeCore()
        {
            // Hashcode base sul nome
            int hash = StringComparer.OrdinalIgnoreCase.GetHashCode(Name);

            // Combina con hash specifico
            return HashCode.Combine(hash, GetValueHashCode());
        }

        /// <summary>
        /// Hook per confronto logico specifico delle sottoclassi.
        /// </summary>
        /// <param name="other">La proprietà da confrontare.</param>
        /// <returns><c>true</c> se i valori logici specifici coincidono.</returns>
        protected abstract bool EqualsByValue(TSelf other);

        /// <summary>
        /// Hook per calcolare l'hash specifico nella sottoclassa.
        /// </summary>
        /// <returns>Hash code logico della sottoclasse.</returns>
        protected abstract int GetValueHashCode();
    }
}
