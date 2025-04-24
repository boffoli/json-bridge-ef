using JsonBridgeEF.Shared.Dag.Exceptions;
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
        public static void EnsureCanAddParent<TSelf, TValue>(TSelf current, TSelf newParent)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newParent);
            ThrowIfSelfReference(current, newParent);
            ThrowIfInvalidParentBinding<TSelf, TValue>(current, newParent);
            ThrowIfCycleDetected<TSelf, TValue>(current, newParent);
        }

        public static void EnsureCanAddChild<TSelf, TValue>(TSelf current, TSelf newChild)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newChild);
            ThrowIfSelfReference(current, newChild);
            ThrowIfInvalidChildBinding<TSelf, TValue>(current, newChild);
            ThrowIfCycleDetected<TSelf, TValue>(current, newChild);
        }

        public static void EnsureCanAddChild<TSelf, TValue>(TSelf aggregator, TValue newChild)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            ThrowIfNull(newChild);
            ThrowIfNotExpectedParent(aggregator, newChild);
            ThrowIfDuplicateLeaf(aggregator, newChild);
        }

        private static void ThrowIfNull<T>(T target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
        }

        private static void ThrowIfSelfReference<T>(T source, T target) where T : class
        {
            if (ReferenceEquals(source, target) || source.Equals(target))
                throw AggregateNodeValidationException.SelfReference(source?.ToString() ?? "(null)");
        }

        private static void ThrowIfInvalidParentBinding<TSelf, TValue>(TSelf source, TSelf target)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (source is not IParentNavigableNode<TSelf> navSource)
                throw AggregateNodeValidationException.ParentNavigationNotSupported(source?.ToString() ?? "(null)");

            if (navSource.Parents.Contains(target) && !target.SelfChildren.Contains(source))
                throw AggregateNodeValidationException.InvalidParentBinding(source.Name, target.Name);
        }

        private static void ThrowIfInvalidChildBinding<TSelf, TValue>(TSelf source, TSelf target)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (!source.SelfChildren.Contains(target))
                return;

            if (target is not IParentNavigableNode<TSelf> navTarget || !navTarget.Parents.Contains(source))
                throw AggregateNodeValidationException.InvalidChildBinding(source.Name, target.Name);
        }

        private static void ThrowIfNotExpectedParent<TSelf, TValue>(TSelf expectedParent, TValue child)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (!ReferenceEquals(child.Parent, expectedParent))
                throw AggregateNodeValidationException.NotExpectedParent(child.Name, expectedParent.Name);
        }

        private static void ThrowIfDuplicateLeaf<TSelf, TValue>(TSelf parent, TValue child)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (parent.ValueChildren.Contains(child))
                throw AggregateNodeValidationException.DuplicateLeaf(child.Name);
        }

        private static void ThrowIfCycleDetected<TSelf, TValue>(TSelf start, TSelf candidate)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (DetectCycle<TSelf, TValue>(start, candidate))
                throw AggregateNodeValidationException.CycleDetected(start.Name, candidate.Name);
        }

        private static bool DetectCycle<TSelf, TValue>(TSelf current, TSelf target)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
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
