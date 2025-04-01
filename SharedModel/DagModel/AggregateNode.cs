using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JsonBridgeEF.SharedModel.DagModel
{
    /// <inheritdoc cref="IAggregateNode{TSelf, TValue}"/>
    /// <summary>
    /// Domain Class: Nodo aggregato che contiene altri nodi nel grafo.
    /// </summary>
    internal abstract class AggregateNode<TSelf, TValue>(string name)
        : Node<TSelf, TValue>(name), IAggregateNode<TSelf, TValue>
        where TSelf : AggregateNode<TSelf, TValue>, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        private readonly List<TSelf> _selfChildren = [];
        private readonly List<TValue> _valueChildren = [];

        /// <inheritdoc />
        public IReadOnlyCollection<TSelf> SelfChildren => new ReadOnlyCollection<TSelf>(_selfChildren);

        /// <inheritdoc />
        public IReadOnlyCollection<TValue> ValueChildren => new ReadOnlyCollection<TValue>(_valueChildren);

        /// <inheritdoc />
        public void AddChild(TSelf child)
        {
            // Precondition: verifica null
            ArgumentNullException.ThrowIfNull(child);

            // Precondition: evita auto-ciclo diretto tramite confronto logico
            if (Equals(child))
                throw new InvalidOperationException("Un nodo non può essere figlio di sé stesso.");

            // Precondition: verifica duplicati logici (basato su Equals)
            if (_selfChildren.Contains(child))
                throw new InvalidOperationException($"Il nodo aggregato '{child.Name}' è già presente come figlio.");

            // Controllo imprescindibile della ciclicità (basato su Equals)
            if (!CheckCycleConstraint(child))
                throw new InvalidOperationException("Aggiunta non consentita: creerebbe un ciclo nel grafo.");

            // Hook: validazione semantica specifica nelle sottoclassi
            AdditionalValidateAdd(child);

            // Inserimento nodo figlio aggregato
            _selfChildren.Add(child);
        }

        /// <inheritdoc />
        public void AddChild(TValue child)
        {
            // Precondition: verifica null
            ArgumentNullException.ThrowIfNull(child);

            // Precondition: verifica parent corretto tramite confronto logico
            if (!Equals(child.Parent))
                throw new InvalidOperationException("Il nodo foglia deve essere costruito con il parent corretto.");

            // Precondition: verifica duplicati logici (basato su Equals)
            if (_valueChildren.Contains(child))
                throw new InvalidOperationException($"La proprietà '{child.Name}' è già presente nel nodo.");

            // Hook: validazione semantica specifica nelle sottoclassi
            AdditionalValidateAdd(child);

            // Inserimento nodo figlio foglia
            _valueChildren.Add(child);
        }

        /// <summary>
        /// Hook astratto per la validazione semantica dei nodi aggregati.
        /// </summary>
        protected abstract void AdditionalValidateAdd(TSelf child);

        /// <summary>
        /// Hook astratto per la validazione semantica dei nodi foglia.
        /// </summary>
        protected abstract void AdditionalValidateAdd(TValue child);

        /// <summary>
        /// Controllo imprescindibile di ciclicità basato su Equals.
        /// </summary>
        protected bool CheckCycleConstraint(TSelf child)
        {
            return CycleValidator.IsAcyclic(this, child);
        }

        /// <summary>
        /// Classe ausiliaria privata per verificare ciclicità logica.
        /// </summary>
        private static class CycleValidator
        {
            public static bool IsAcyclic(AggregateNode<TSelf, TValue> root, TSelf candidate)
            {
                return !ContainsRecursive(root, candidate);
            }

            private static bool ContainsRecursive(AggregateNode<TSelf, TValue> node, TSelf target)
            {
                foreach (var child in node.SelfChildren)
                {
                    if (child.Equals(target) || ContainsRecursive(child, target))
                        return true;
                }
                return false;
            }
        }
    }
}