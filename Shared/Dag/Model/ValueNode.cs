using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Dag.Validators;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <inheritdoc cref="IValueNode{TSelf, TAggregate}"/>
    /// <summary>
    /// Domain Class: Nodo terminale associato a un aggregato.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto con nome, aggregato parent e validatore opzionale.</para>
    /// <para><b>Constraints:</b><br/>
    /// Il parent non può essere <c>null</c> e deve implementare <see cref="IAggregateNode{TAggregate, TSelf}"/>.</para>
    /// <para><b>Relationships:</b><br/>
    /// Questo nodo conosce solo il proprio aggregato <see cref="Parent"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Il validatore <see cref="ValueNodeValidator{TSelf, TAggregate}"/> può essere iniettato per controlli a runtime.</para>
    /// </remarks>
    /// <remarks>
    /// Costruttore protetto: inizializza nome, parent e validatore opzionale.
    /// </remarks>
    /// <param name="name">Nome del nodo foglia.</param>
    /// <param name="parent">Aggregato proprietario.</param>
    /// <param name="validator">Validatore opzionale per il tipo concreto <typeparamref name="TSelf"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Se <paramref name="parent"/> è <c>null</c>.
    /// </exception>
    internal abstract class ValueNode<TSelf, TAggregate>(
        string name,
        TAggregate parent,
        IValidateAndFix<TSelf>? validator
        ) : Node<TSelf>(name, validator), IValueNode<TSelf, TAggregate>
        where TSelf     : ValueNode<TSelf, TAggregate>, IValueNode<TSelf, TAggregate>
        where TAggregate: class, IAggregateNode<TAggregate, TSelf>
    {
        private readonly TAggregate _parent = parent ?? throw new ArgumentNullException(nameof(parent));

        /// <inheritdoc />
        [Required]
        public TAggregate Parent => _parent;
    }
}