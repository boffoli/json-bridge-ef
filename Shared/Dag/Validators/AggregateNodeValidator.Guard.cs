using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Validators;

internal static partial class AggregateNodeValidator
{
    /// <summary>
    /// Utility interna per eseguire controlli di validazione strutturale e semantica su nodi di un DAG.
    /// </summary>
    /// <remarks>
    /// <para>Creation Strategy: Invocata internamente nei metodi pubblici della classe <see cref="AggregateNodeValidator"/>.</para>
    /// <para>Constraints: Nessuno. Tutti i metodi sono statici e indipendenti.</para>
    /// <para>Relationships: Collabora con la logica di validazione topologica e di coerenza delle relazioni parentali e filiali.</para>
    /// <para>Usage Notes: I metodi sollevano eccezioni specifiche in caso di violazioni, evitando duplicazione del codice.</para>
    /// </remarks>
    private static class Guard
    {
        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Solleva eccezione se il valore specificato è <c>null</c>.
        /// </summary>
        /// <param name="target">Oggetto da verificare.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Nessuna.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="ArgumentNullException"/> se <paramref name="target"/> è <c>null</c>.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfNull<T>(T target)
        {
            ArgumentNullException.ThrowIfNull(target);
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Solleva eccezione se i due nodi sono uguali (auto-riferimento).
        /// </summary>
        /// <param name="source">Nodo sorgente.</param>
        /// <param name="target">Nodo destinazione.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Entrambi i nodi devono essere istanze valide.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se i nodi coincidono.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfSelfReference<T>(T source, T target)
            where T : class
        {
            if (source.Equals(target))
                throw new InvalidOperationException("Un nodo non può essere collegato a sé stesso.");
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Verifica l’integrità della relazione parentale in un grafo bidirezionale.
        /// </summary>
        /// <param name="source">Nodo che vuole aggiungere un genitore.</param>
        /// <param name="target">Nodo candidato come genitore.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il nodo <paramref name="source"/> deve implementare <see cref="IParentNavigableNode{TSelf}"/>.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se la relazione è già presente ma inconsistente (manca il riferimento inverso).</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfInvalidParentBinding<TSelf, TValue>(TSelf source, TSelf target)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (source is not IParentNavigableNode<TSelf> navSource)
                throw new InvalidOperationException("Il nodo corrente non supporta la navigazione dei genitori.");

            if (navSource.Parents.Contains(target) && !target.SelfChildren.Contains(source))
            {
                throw new InvalidOperationException("Relazione non valida: genitore già presente o inconsistente.");
            }
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Verifica l’integrità della relazione figlio in un grafo bidirezionale.
        /// </summary>
        /// <param name="source">Nodo che vuole aggiungere un figlio.</param>
        /// <param name="target">Nodo candidato come figlio.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Nessuna. Il metodo è tollerante alla mancanza della navigazione inversa.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se il figlio è già presente ma manca il collegamento inverso.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfInvalidChildBinding<TSelf, TValue>(TSelf source, TSelf target)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (!source.SelfChildren.Contains(target))
                return;

            if (target is not IParentNavigableNode<TSelf> navTarget || !navTarget.Parents.Contains(source))
            {
                throw new InvalidOperationException("Relazione non valida: figlio già presente o inconsistente.");
            }
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Verifica che il nodo foglia abbia il parent atteso.
        /// </summary>
        /// <param name="expectedParent">Nodo corrente atteso come parent.</param>
        /// <param name="child">Nodo foglia da verificare.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il nodo foglia deve essere stato costruito con il parent corretto.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se il parent non corrisponde.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfNotExpectedParent<TSelf, TValue>(TSelf expectedParent, TValue child)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (!Equals(child.Parent, expectedParent))
                throw new InvalidOperationException("Il nodo foglia deve essere costruito con il parent corretto.");
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Verifica la presenza duplicata di un nodo foglia.
        /// </summary>
        /// <param name="parent">Nodo aggregato padre.</param>
        /// <param name="child">Nodo foglia candidato.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Nessuna.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se la proprietà è già presente.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfDuplicateLeaf<TSelf, TValue>(TSelf parent, TValue child)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (parent.ValueChildren.Contains(child))
                throw new InvalidOperationException($"La proprietà '{child.Name}' è già presente nel nodo.");
        }

        /// <inheritdoc cref="XmlDocMethod"/>
        /// <summary>
        /// Verifica se l’aggiunta introdurrebbe un ciclo nel grafo.
        /// </summary>
        /// <param name="start">Nodo sorgente.</param>
        /// <param name="candidate">Nodo da aggiungere.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Entrambi i nodi devono essere istanze valide.</para>
        /// <para><b>Postconditions:</b> Solleva <see cref="InvalidOperationException"/> se l’aggiunta produrrebbe un ciclo.</para>
        /// <para><b>Side Effects:</b> Nessuno.</para>
        /// </remarks>
        public static void ThrowIfCycleDetected<TSelf, TValue>(TSelf start, TSelf candidate)
            where TSelf : class, IAggregateNode<TSelf, TValue>
            where TValue : class, IValueNode<TValue, TSelf>
        {
            if (Visit(start, candidate))
            {
                throw new InvalidOperationException("Aggiunta non consentita: creerebbe un ciclo nel grafo.");
            }

            static bool Visit(TSelf current, TSelf target)
            {
                foreach (var child in current.SelfChildren)
                {
                    if (child.Equals(target) || Visit(child, target))
                        return true;
                }
                return false;
            }
        }
    }
}