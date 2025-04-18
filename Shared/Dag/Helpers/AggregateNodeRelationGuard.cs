using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Helpers
{
    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Helper statico per la validazione strutturale e semantica delle relazioni
    /// tra nodi aggregati in un grafo aciclico diretto (DAG).</para>
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Classe statica; tutti i metodi possono essere invocati direttamente senza istanziare.</para>
    /// <para><b>Constraints:</b><br/>
    /// I tipi devono implementare i contratti <see cref="IAggregateNode{TSelf,TValue}"/>
    /// e <see cref="IValueNode{TValue,TSelf}"/>, e facoltativamente <see cref="IParentNavigableNode{TSelf}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Evita duplicazione di codice spostando qui tutti i controlli “guard”.<br/>
    /// - Solleva eccezioni specifiche in caso di violazioni di pre‑ o post‑condizioni.</para>
    /// </remarks>
    internal static class AggregateNodeRelationGuard
    {
        // ====================== PUBLIC VALIDATION METHODS ======================

        /// <summary>
        /// Verifica se è possibile aggiungere un genitore a un nodo aggregato.
        /// </summary>
        public static void EnsureCanAddParent<TSelf, TValue>(
            TSelf current,
            TSelf newParent)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newParent);
            ThrowIfSelfReference(current, newParent);
            ThrowIfInvalidParentBinding<TSelf, TValue>(current, newParent);
            ThrowIfCycleDetected<TSelf, TValue>(current, newParent);
        }

        /// <summary>
        /// Verifica se è possibile aggiungere un nodo aggregato figlio.
        /// </summary>
        public static void EnsureCanAddChild<TSelf, TValue>(
            TSelf current,
            TSelf newChild)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newChild);
            ThrowIfSelfReference(current, newChild);
            ThrowIfInvalidChildBinding<TSelf, TValue>(current, newChild);
            ThrowIfCycleDetected<TSelf, TValue>(current, newChild);
        }

        /// <summary>
        /// Verifica se è possibile aggiungere un nodo foglia al nodo aggregato corrente.
        /// </summary>
        public static void EnsureCanAddChild<TSelf, TValue>(
            TSelf aggregator,
            TValue newChild)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newChild);
            ThrowIfNotExpectedParent(aggregator, newChild);
            ThrowIfDuplicateLeaf(aggregator, newChild);
        }

        // ====================== PRIVATE GUARD METHODS ======================

        private static void ThrowIfNull<T>(T target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
        }

        private static void ThrowIfSelfReference<T>(T source, T target)
            where T : class
        {
            if (ReferenceEquals(source, target) || source.Equals(target))
                throw new InvalidOperationException("Un nodo non può essere collegato a sé stesso.");
        }

        private static void ThrowIfInvalidParentBinding<TSelf, TValue>(
            TSelf source,
            TSelf target)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            if (source is not IParentNavigableNode<TSelf> navSource)
                throw new InvalidOperationException("Il nodo corrente non supporta la navigazione verso i genitori.");

            if (navSource.Parents.Contains(target) && !target.SelfChildren.Contains(source))
            {
                throw new InvalidOperationException("Relazione non valida: genitore già presente o incoerente.");
            }
        }

        private static void ThrowIfInvalidChildBinding<TSelf, TValue>(
            TSelf source,
            TSelf target)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            if (!source.SelfChildren.Contains(target))
                return;

            if (target is not IParentNavigableNode<TSelf> navTarget
                || !navTarget.Parents.Contains(source))
            {
                throw new InvalidOperationException("Relazione non valida: figlio già presente o incoerente.");
            }
        }

        private static void ThrowIfNotExpectedParent<TSelf, TValue>(
            TSelf expectedParent,
            TValue child)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            if (!ReferenceEquals(child.Parent, expectedParent))
                throw new InvalidOperationException("Il nodo foglia deve essere costruito con il parent corretto.");
        }

        private static void ThrowIfDuplicateLeaf<TSelf, TValue>(
            TSelf parent,
            TValue child)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            if (parent.ValueChildren.Contains(child))
                throw new InvalidOperationException($"La proprietà '{child.Name}' è già presente nel nodo.");
        }

        private static void ThrowIfCycleDetected<TSelf, TValue>(
            TSelf start,
            TSelf candidate)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            if (DetectCycle<TSelf, TValue>(start, candidate))
                throw new InvalidOperationException("Aggiunta non consentita: creerebbe un ciclo nel grafo.");
        }

        private static bool DetectCycle<TSelf, TValue>(
            TSelf current,
            TSelf target)
            where TSelf  : class, IAggregateNode<TSelf, TValue>
            where TValue: class, IValueNode<TValue, TSelf>
        {
            foreach (var child in current.SelfChildren)
            {
                if (ReferenceEquals(child, target) || DetectCycle<TSelf, TValue>(child, target))
                    return true;
            }
            return false;
        }
    }
}