using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Model;
using JsonBridgeEF.Shared.EntityModel.Interfaces;
using JsonBridgeEF.Shared.Navigation.Helpers;

namespace JsonBridgeEF.Shared.EntityModel.Model
{
    /// <inheritdoc cref="IEntity{TSelf, TEntityProperty}"/>
    /// <summary>
    /// <para><b>Concrete Domain Class:</b><br/>
    /// Implementazione base per un’entità che aggrega proprietà e supporta navigazione topologica verso i genitori.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// Creato tramite costruttore con nome valido e validatore opzionale. Le proprietà vengono aggiunte tramite <c>AddChild</c>,
    /// i genitori tramite <c>AddParent</c>.</para>
    /// 
    /// <para><b>Constraints:</b><br/>
    /// - Solo una proprietà può essere designata come chiave logica.<br/>
    /// - La gerarchia parentale non deve creare cicli.</para>
    /// 
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TEntityProperty"/> (<see cref="IAggregateNode{TSelf, TEntityProperty}"/>).<br/>
    /// - Implementa <see cref="IEntity{TSelf, TEntityProperty}"/>, che eredita la navigazione genitori da <see cref="IParentNavigableNode{TSelf}"/>.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// - La proprietà <c>Properties</c> è un alias semantico per <see cref="IAggregateNode{TSelf, TEntityProperty}.ValueChildren"/>.<br/>
    /// - I vincoli su figli e genitori vengono applicati tramite hook <see cref="OnBeforeChildAdded(TSelf)"/>,
    ///   <see cref="OnBeforeChildAdded(TEntityProperty)"/> e dalla logica di <see cref="ParentNavigationManager{TNode, TValue}"/>.</para>
    /// </summary>
    /// <typeparam name="TSelf">Tipo concreto dell’entità.</typeparam>
    /// <typeparam name="TEntityProperty">Tipo delle proprietà aggregate.</typeparam>
    internal abstract class Entity<TSelf, TEntityProperty> 
        : AggregateNode<TSelf, TEntityProperty>,
          IEntity<TSelf, TEntityProperty>
        where TSelf           : Entity<TSelf, TEntityProperty>, IEntity<TSelf, TEntityProperty>
        where TEntityProperty: class, IEntityProperty<TEntityProperty, TSelf>
    {
        // === Fields ===

        private readonly ParentNavigationManager<TSelf, TEntityProperty> _parentManager;
        private TEntityProperty? _keyProperty;

        // === Constructor ===

        /// <summary>
        /// Crea una nuova istanza di <see cref="Entity{TSelf,TEntityProperty}"/>,
        /// inizializzando il gestore per la navigazione verso i genitori.
        /// </summary>
        /// <param name="name">Nome univoco dell’entità.</param>
        /// <param name="validator">Validatore opzionale per il tipo concreto <typeparamref name="TSelf"/>.</param>
        protected Entity(
            string name,
            IValidateAndFix<TSelf>? validator
        ) : base(name, validator)
        {
            _parentManager = new ParentNavigationManager<TSelf, TEntityProperty>((TSelf)this);
        }

        // === IAggregateNode / IEntity ===

        /// <inheritdoc/>
        /// <remarks>Alias semantico per <see cref="IAggregateNode{TSelf, TEntityProperty}.ValueChildren"/>.</remarks>
        public IReadOnlyCollection<TEntityProperty> Properties => ValueChildren;

        /// <inheritdoc/>
        public TEntityProperty? GetKeyProperty() => _keyProperty;

        /// <inheritdoc/>
        public bool IsIdentifiable() => _keyProperty is not null;

        // === Parent Navigation (ereditata da IEntity) ===

        /// <inheritdoc/>
        public IReadOnlyCollection<TSelf> Parents => _parentManager.Parents;

        /// <inheritdoc/>
        public bool IsRoot => _parentManager.IsRoot;

        /// <inheritdoc/>
        public void AddParent(TSelf parent) => _parentManager.AddParent(parent);

        // === Equality ===

        protected sealed override bool EqualsCore(Node<TSelf> other)
        {
            return other is TSelf entity
                && string.Equals(Name, entity.Name, StringComparison.OrdinalIgnoreCase)
                && EqualsByValue(entity);
        }

        protected sealed override int GetHashCodeCore()
        {
            int hash = StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            return HashCode.Combine(hash, GetValueHashCode());
        }

        /// <summary>
        /// Hook per confronto logico aggiuntivo nella sottoclasse.
        /// </summary>
        protected abstract bool EqualsByValue(TSelf other);

        /// <summary>
        /// Hook per calcolo hash logico nella sottoclasse.
        /// </summary>
        protected abstract int GetValueHashCode();

        // === Validation Hooks ===

        protected sealed override void OnBeforeChildAdded(TSelf child)
        {
            OnBeforeEntityAdded(child);
        }

        protected sealed override void OnBeforeChildAdded(TEntityProperty child)
        {
            OnBeforePropertyAdded(child);
            if (child.IsKey)
            {
                if (_keyProperty is not null)
                    throw new InvalidOperationException(
                        $"L'entità '{Name}' ha già una proprietà chiave: '{_keyProperty.Name}'.");
                _keyProperty = child;
            }
        }

        /// <summary>
        /// Hook per validazioni specifiche prima di aggiungere una entità figlia.
        /// </summary>
        protected virtual void OnBeforeEntityAdded(TSelf child) { }

        /// <summary>
        /// Hook per validazioni specifiche prima di aggiungere una proprietà figlia.
        /// </summary>
        protected virtual void OnBeforePropertyAdded(TEntityProperty child) { }
    }
}