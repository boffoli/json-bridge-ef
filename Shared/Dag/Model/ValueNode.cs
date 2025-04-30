using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Exceptions;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <inheritdoc cref="IValueNode{TSelf,TAggregate}"/>
    /// <summary>
    /// Domain Class: Nodo di valore (leaf) in un grafo aggregato.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un nodo senza figli, che appartiene a un nodo aggregato.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto con nome e parent obbligatorio.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Deve avere un parent non nullo.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Eredita da <see cref="Node{TSelf}"/> per nome e uguaglianza.<br/>
    /// - Implementa <see cref="IValueNode{TSelf,TAggregate}"/> per l’accesso al parent.</para>
    /// </remarks>
    internal abstract class ValueNode<TSelf, TAggregate>
        : Node<TSelf>, IValueNode<TSelf, TAggregate>
        where TSelf      : ValueNode<TSelf, TAggregate>, IValueNode<TSelf, TAggregate>
        where TAggregate : class, IAggregateNode<TAggregate, TSelf>
    {
        private readonly TAggregate _parent;

        /// <summary>
        /// Costruttore protetto: imposta nome, parent e validatore opzionale.
        /// </summary>
        /// <param name="name">Nome del nodo di valore.</param>
        /// <param name="parent">
        /// Parent aggregato, di tipo <see cref="IAggregateNode{TAggregate,TSelf}"/>.
        /// </param>
        /// <param name="validator">Validatore opzionale per il tipo concreto <typeparamref name="TSelf"/>.</param>
        protected ValueNode(
            string name,
            IAggregateNode<TAggregate, TSelf> parent,
            IValidateAndFix<TSelf>? validator = null)
            : base(name, validator)
        {
            if (parent is null)
                throw ValueNodeError.NullParent(name);

            _parent = (TAggregate)parent;
        }

        /// <inheritdoc />
        public TAggregate Parent => _parent;
    }
}