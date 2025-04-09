using JsonBridgeEF.Shared.Dag.Model;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Shared.EntityModel.Model
{
    /// <summary>
    /// Implementazione astratta di <see cref="IEntity{TSelf, TEntityProperty}"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Entità che aggrega proprietà e può essere parte di una gerarchia grafo.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// Creato tramite costruttore con nome valido. Le proprietà devono essere aggiunte tramite <c>AddChild</c>.</para>
    /// 
    /// <para><b>Constraints:</b><br/>
    /// - Una sola proprietà può essere designata come chiave logica.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TEntityProperty"/>.<br/>
    /// - Estende <see cref="AggregateNode{TSelf, TEntityProperty}"/>.<br/>
    /// - Implementa <see cref="IEntity{TSelf, TEntityProperty}"/>.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// - La proprietà <c>Properties</c> è un alias semantico per <c>ValueChildren</c>.<br/>
    /// - I vincoli di integrità vengono definiti tramite <c>AdditionalValidateAdd</c> e possono essere estesi tramite hook personalizzati.</para>
    /// </remarks>
    /// <typeparam name="TSelf">Il tipo concreto dell'entità.</typeparam>
    /// <typeparam name="TEntityProperty">Il tipo delle proprietà associate all'entità.</typeparam>
    internal abstract class Entity<TSelf, TEntityProperty>(string name)
        : AggregateNode<TSelf, TEntityProperty>(name), IEntity<TSelf, TEntityProperty>
        where TSelf : Entity<TSelf, TEntityProperty>, IEntity<TSelf, TEntityProperty>
        where TEntityProperty : class, IEntityProperty<TEntityProperty, TSelf>
    {
        // === Fields ===

        private TEntityProperty? _keyProperty;

        // === Properties ===

        /// <inheritdoc />
        public IReadOnlyCollection<TEntityProperty> Properties => base.ValueChildren;

        /// <inheritdoc />
        public TEntityProperty? GetKeyProperty() => _keyProperty;

        /// <inheritdoc />
        public bool IsIdentifiable() => _keyProperty is not null;

        // === Equality ===

        /// <inheritdoc />
        protected override bool EqualsCore(Node other)
        {
            // Confronto base su nome + confronto logico specifico
            return other is TSelf entity &&
                   string.Equals(Name, entity.Name, StringComparison.OrdinalIgnoreCase) &&
                   EqualsByValue(entity);
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
        /// Hook per confronto logico aggiuntivo, oltre al nome.
        /// </summary>
        /// <param name="other">Entità da confrontare.</param>
        /// <returns><c>true</c> se l'entità è logicamente equivalente a questa.</returns>
        protected abstract bool EqualsByValue(TSelf other);

        /// <summary>
        /// Hook per calcolare l'hash specifico nella sottoclasse.
        /// </summary>
        /// <returns>Hash code logico della sottoclasse.</returns>
        protected abstract int GetValueHashCode();

        // === Validation ===

        /// <inheritdoc />
        protected override void AdditionalValidateAdd(TEntityProperty child)
        {
            // Hook per validazioni personalizzate sulla proprietà.
            AdditionalCustomValidateProperty(child);

            // Se la proprietà è chiave, verifica unicità e aggiorna lo stato
            if (child.IsKey)
            {
                if (_keyProperty is not null)
                    throw new InvalidOperationException($"L'entità '{Name}' ha già una proprietà chiave: '{_keyProperty.Name}'.");
                _keyProperty = child;
            }
        }

        /// <inheritdoc />
        protected override void AdditionalValidateAdd(TSelf child)
        {
            // Hook per validazioni personalizzate sull'entità figlia.
            AdditionalCustomValidateEntity(child);
        }

        // === Hooks ===

        /// <summary>
        /// Hook astratto per eseguire validazioni aggiuntive specifiche per la proprietà.
        /// </summary>
        /// <param name="child">La proprietà da validare.</param>
        protected abstract void AdditionalCustomValidateProperty(TEntityProperty child);

        /// <summary>
        /// Hook astratto per eseguire validazioni aggiuntive specifiche sull'entità figlia.
        /// </summary>
        /// <param name="child">Il nodo aggregato da validare.</param>
        protected abstract void AdditionalCustomValidateEntity(TSelf child);
    }
}