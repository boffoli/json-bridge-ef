using System;
using System.ComponentModel.DataAnnotations;

namespace JsonBridgeEF.SharedModel.DagModel
{
    /// <inheritdoc cref="INode"/>
    /// <summary>
    /// Domain Class: Nodo base identificabile in una struttura a grafo.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta l'unità fondamentale del grafo, con identificazione univoca tramite il nome.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Inizializzato tramite costruttore protetto con nome non nullo.</para>
    /// <para><b>Invariants:</b><br/>
    /// - Nome non nullo, non vuoto, immutabile.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Punto di partenza comune per nodi aggregati e foglia.</para>
    /// </remarks>
    internal abstract class Node : INode, IEquatable<Node>
    {
        /// <inheritdoc />
        [Required]
        public string Name { get; }

        /// <summary>
        /// Costruttore protetto: impone la validazione del nome come identificatore univoco.
        /// </summary>
        /// <param name="name">Nome del nodo (obbligatorio e immutabile).</param>
        /// <exception cref="ArgumentException">Se <paramref name="name"/> è nullo o vuoto.</exception>
        protected Node(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome non può essere nullo o vuoto.", nameof(name));

            Name = name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            // Ottimizzazione: controllo di identità referenziale
            if (ReferenceEquals(this, obj))
                return true;

            // Controllo compatibilità tipo, poi delega all'overload tipizzato
            return obj is Node other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(Node? other)
        {
            // Identità referenziale prima di tutto
            if (ReferenceEquals(this, other))
                return true;

            // Verifica null e delega a EqualsCore per confronto logico
            return other is not null && EqualsCore(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // Delega al metodo astratto per il calcolo coerente dell'hash
            return GetHashCodeCore();
        }

        /// <summary>
        /// Implementazione astratta del confronto di uguaglianza logica.
        /// </summary>
        /// <param name="other">Altro nodo da confrontare.</param>
        /// <returns><c>true</c> se i nodi sono logicamente equivalenti.</returns>
        protected abstract bool EqualsCore(Node other);

        /// <summary>
        /// Implementazione astratta del calcolo dell'hash code.
        /// </summary>
        /// <returns>Valore hash coerente con <see cref="EqualsCore"/>.</returns>
        protected abstract int GetHashCodeCore();

        /// <summary>
        /// Operatore di uguaglianza basato su <see cref="Equals(Node?)"/>.
        /// </summary>
        public static bool operator ==(Node? left, Node? right) => Equals(left, right);

        /// <summary>
        /// Operatore di disuguaglianza basato su <see cref="Equals(Node?)"/>.
        /// </summary>
        public static bool operator !=(Node? left, Node? right) => !Equals(left, right);
    }

    /// <inheritdoc cref="INode{TAggregate, TValue}" />
    /// <summary>
    /// Domain Class: Nodo generico tipizzato per relazioni aggregato/figlio.
    /// </summary>
    /// <typeparam name="TAggregate">Tipo del nodo aggregato.</typeparam>
    /// <typeparam name="TValue">Tipo del nodo foglia.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Nodo base dotato di tipizzazione generica, utile per costruire strutture fortemente tipizzate.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="Node"/> e implementa <see cref="INode{TAggregate, TValue}"/>.</para>
    /// </remarks>
    internal abstract class Node<TAggregate, TValue>(string name)
        : Node(name), INode<TAggregate, TValue>
        where TAggregate : class, IAggregateNode<TAggregate, TValue>
        where TValue : class, IValueNode<TValue, TAggregate>
    {
        // Classe di supporto per la tipizzazione nei modelli aggregati e foglia
    }
}