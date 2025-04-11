using JsonBridgeEF.Shared.Dag.Model;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Shared.EntityModel.Model
{
    /// <inheritdoc cref="IEntity{TSelf, TEntityProperty}"/>
    /// <summary>
    /// <para><b>Concrete Domain Class:</b><br/>
    /// Implementazione base per un'entità che aggrega proprietà e può essere parte di un DAG.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// Creato tramite costruttore con nome valido. Le proprietà vengono aggiunte tramite <c>AddChild</c>.</para>
    /// 
    /// <para><b>Constraints:</b><br/>
    /// - Solo una proprietà può essere designata come chiave logica.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TEntityProperty"/>.<br/>
    /// - Estende <see cref="AggregateNode{TSelf, TEntityProperty}"/>.<br/>
    /// - Implementa <see cref="IEntity{TSelf, TEntityProperty}"/>.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// - La proprietà <c>Properties</c> è un alias semantico per <c>ValueChildren</c>.<br/>
    /// - I vincoli di integrità vengono applicati tramite <c>OnBeforeChildAdded</c> e hook custom.</para>
    /// </summary>
    /// <typeparam name="TSelf">Tipo concreto dell'entità.</typeparam>
    /// <typeparam name="TEntityProperty">Tipo delle proprietà aggregate.</typeparam>
    internal abstract class Entity<TSelf, TEntityProperty>(string name)
        : AggregateNode<TSelf, TEntityProperty>(name), IEntity<TSelf, TEntityProperty>
        where TSelf : Entity<TSelf, TEntityProperty>, IEntity<TSelf, TEntityProperty>
        where TEntityProperty : class, IEntityProperty<TEntityProperty, TSelf>
    {
        // === Fields ===

        private TEntityProperty? _keyProperty;

        // === Properties ===

        /// <inheritdoc />
        /// <remarks>Alias semantico per <c>ValueChildren</c>.</remarks>
        public IReadOnlyCollection<TEntityProperty> Properties => base.ValueChildren;

        /// <inheritdoc />
        public TEntityProperty? GetKeyProperty() => _keyProperty;

        /// <inheritdoc />
        public bool IsIdentifiable() => _keyProperty is not null;

        // === Equality ===

        /// <inheritdoc />
        /// <remarks>
        /// <para><b>Comparison Logic:</b> Confronta nome e valore logico.</para>
        /// </remarks>
        protected sealed override bool EqualsCore(Node other)
        {
            return other is TSelf entity &&
                   string.Equals(Name, entity.Name, StringComparison.OrdinalIgnoreCase) &&
                   EqualsByValue(entity);
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para><b>Hashing Strategy:</b> Combina nome e valore logico specifico.</para>
        /// </remarks>
        protected sealed override int GetHashCodeCore()
        {
            int hash = StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            return HashCode.Combine(hash, GetValueHashCode());
        }

        /// <summary>
        /// <b>Logical Equality Hook</b><br/>
        /// Determina se due entità sono logicamente equivalenti (oltre al nome).
        /// </summary>
        /// <param name="other">Entità da confrontare.</param>
        /// <returns><c>true</c> se logicamente equivalenti, altrimenti <c>false</c>.</returns>
        protected abstract bool EqualsByValue(TSelf other);

        /// <summary>
        /// <b>Logical Hash Hook</b><br/>
        /// Calcola l'hash logico personalizzato per la sottoclasse.
        /// </summary>
        /// <returns>Hash code della parte logica dell'entità.</returns>
        protected abstract int GetValueHashCode();

        // === Validation ===

        /// <inheritdoc />
        /// <remarks>
        /// Invoca <see cref="OnBeforeEntityAdded"/> per applicare vincoli specifici sull'entità figlia.
        /// </remarks>
        protected sealed override void OnBeforeChildAdded(TSelf child)
        {
            OnBeforeEntityAdded(child);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Esegue validazioni su <paramref name="child"/> e gestisce l'unicità della proprietà chiave:
        /// <list type="bullet">
        ///   <item><description>Invoca <see cref="OnBeforePropertyAdded"/> per logica personalizzata.</description></item>
        ///   <item><description>Se <c>child.IsKey</c> è true, verifica unicità e aggiorna <c>_keyProperty</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Sollevata se viene aggiunta più di una proprietà chiave.
        /// </exception>
        protected sealed override void OnBeforeChildAdded(TEntityProperty child)
        {
            OnBeforePropertyAdded(child);

            if (child.IsKey)
            {
                if (_keyProperty is not null)
                    throw new InvalidOperationException($"L'entità '{Name}' ha già una proprietà chiave: '{_keyProperty.Name}'.");

                _keyProperty = child;
            }
        }

        // === Hooks ===

        /// <summary>
        /// Hook per validazioni specifiche sull’entità aggregata prima dell'aggiunta.
        /// </summary>
        /// <param name="child">Entità figlia da validare.</param>
        protected virtual void OnBeforeEntityAdded(TSelf child){}

        /// <summary>
        /// Hook per validazioni specifiche sulla proprietà prima dell'aggiunta.
        /// </summary>
        /// <param name="child">Proprietà da validare.</param>
        protected virtual void OnBeforePropertyAdded(TEntityProperty child){}
    }
}