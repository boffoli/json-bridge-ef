using JsonBridgeEF.SharedModel.DagModel;

namespace JsonBridgeEF.SharedModel.EntityModel
{
    /// <summary>
    /// Implementazione astratta di <see cref="IEntityProperty{TSelf, TEntity}"/>.
    /// </summary>
    /// <remarks>
    /// Costruttore base per la proprietà.
    /// </remarks>
    /// <param name="name">Nome della proprietà (obbligatorio e immutabile).</param>
    /// <param name="parent">Entità proprietaria (obbligatoria e immutabile).</param>
    /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
    internal abstract class EntityProperty<TSelf, TEntity>(string name, TEntity parent, bool isKey = false)
        : ValueNode<TSelf, TEntity>(name, parent), IEntityProperty<TSelf, TEntity>
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
        protected override bool EqualsCore(Node other)
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
        protected override int GetHashCodeCore()
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
        /// Hook per calcolare l'hash specifico nella sottoclasse.
        /// </summary>
        /// <returns>Hash code logico della sottoclasse.</returns>
        protected abstract int GetValueHashCode();
    }
}