using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Exceptions;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Model
{
    /// <summary>
    /// Domain Class: Nodo base identificabile in una struttura a grafo.
    /// </summary>
    /// <typeparam name="TSelf">Tipo concreto del nodo che implementa <see cref="INode"/>.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta l'unità fondamentale del grafo, con identificazione univoca tramite il nome.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto, accetta un validatore opzionale per la verifica a runtime.</para>
    /// <para><b>Invariants:</b><br/>
    /// - Nome non nullo, non vuoto e immutabile (validato esternamente).</para>
    /// <para><b>Relationships:</b><br/>
    /// - Punto di partenza comune per nodi aggregati e foglia.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Il validatore può essere iniettato per eseguire controlli aggiuntivi senza modificare la logica del nodo.</para>
    /// </remarks>
    internal abstract class Node<TSelf> : INode, IEquatable<Node<TSelf>>
        where TSelf : INode
    {
        private readonly IValidateAndFix<TSelf>? _validator;

        /// <inheritdoc />
        [Required]
        public string Name { get; }

        /// <summary>
        /// Costruttore protetto: opzionalmente accetta un validatore per delegare i controlli.
        /// </summary>
        /// <param name="name">Nome del nodo (obbligatorio ma validato esternamente).</param>
        /// <param name="validator">Validatore opzionale per la validazione del nodo.</param>
        protected Node(string name, IValidateAndFix<TSelf>? validator = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw NodeError.InvalidName(GetType().Name);

            Name = name;
            _validator = validator;

            if (_validator is not null)
            {
                if (GetType() != typeof(TSelf))
                    throw new InvalidOperationException(
                        $"Type mismatch: expected {typeof(TSelf)}, got {GetType()}.");

                _validator.EnsureValid((TSelf)(object)this);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Node<TSelf> other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(Node<TSelf>? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return other is not null && EqualsCore(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => GetHashCodeCore();

        /// <summary>
        /// Implementazione astratta del confronto logico tra nodi.
        /// </summary>
        /// <param name="other">Altro nodo da confrontare.</param>
        /// <returns><c>true</c> se i nodi sono logicamente equivalenti.</returns>
        protected abstract bool EqualsCore(Node<TSelf> other);

        /// <summary>
        /// Implementazione astratta del calcolo dell'hash code.
        /// </summary>
        /// <returns>Valore hash coerente con <see cref="EqualsCore(Node{TSelf})"/>.</returns>
        protected abstract int GetHashCodeCore();

        /// <summary>
        /// Operatore di uguaglianza basato su <see cref="Equals(Node{TSelf}?)"/>.
        /// </summary>
        public static bool operator ==(Node<TSelf>? left, Node<TSelf>? right)
            => Equals(left, right);

        /// <summary>
        /// Operatore di disuguaglianza basato su <see cref="Equals(Node{TSelf}?)"/>.
        /// </summary>
        public static bool operator !=(Node<TSelf>? left, Node<TSelf>? right)
            => !Equals(left, right);
    }
}