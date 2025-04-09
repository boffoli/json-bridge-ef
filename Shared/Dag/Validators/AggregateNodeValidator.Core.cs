using JsonBridgeEF.Shared.Dag.Interfaces;
using JsonBridgeEF.Shared.Navigation.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore statico per relazioni tra nodi aggregati in un grafo aciclico diretto (DAG).
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Questa classe è statica e i metodi sono utilizzabili direttamente.</para>
/// <para>Constraints: I tipi devono rispettare i contratti <c>IAggregateNode&lt;TSelf, TValue&gt;</c> e <c>IValueNode&lt;TValue, TSelf&gt;</c>.</para>
/// <para>Relationships: Opera su strutture che implementano <see cref="IAggregateNode{TSelf, TValue}"/> e opzionalmente <see cref="IParentNavigableNode{TSelf}"/>.</para>
/// <para>Usage Notes: Garantisce l’integrità strutturale evitando riferimenti ciclici o incoerenze bidirezionali.</para>
/// </remarks>
internal static partial class AggregateNodeValidator
{
    /// <summary>
    /// Verifica se è possibile aggiungere un genitore a un nodo corrente.
    /// </summary>
    /// <param name="current">Il nodo corrente.</param>
    /// <param name="newParent">Il nodo candidato come genitore.</param>
    /// <typeparam name="TSelf">Tipo concreto del nodo aggregato.</typeparam>
    /// <typeparam name="TValue">Tipo concreto del nodo foglia.</typeparam>
    /// <remarks>
    /// <para><b>Preconditions:</b> <paramref name="newParent"/> non deve essere nullo; non può coincidere con <paramref name="current"/>.</para>
    /// <para><b>Postconditions:</b> Solleva eccezioni se la relazione è duplicata, inconsistente o ciclica.</para>
    /// <para><b>Side Effects:</b> Nessuno.</para>
    /// </remarks>
    public static void EnsureCanAddParent<TSelf, TValue>(
        TSelf current,
        TSelf newParent)
        where TSelf : class, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        Guard.ThrowIfNull(newParent);
        Guard.ThrowIfSelfReference(current, newParent);
        Guard.ThrowIfInvalidParentBinding<TSelf, TValue>(current, newParent);
        Guard.ThrowIfCycleDetected<TSelf, TValue>(current, newParent);
    }

    /// <summary>
    /// Verifica se è possibile aggiungere un nodo aggregato figlio.
    /// </summary>
    /// <param name="current">Il nodo corrente.</param>
    /// <param name="newChild">Il nodo candidato come figlio.</param>
    /// <typeparam name="TSelf">Tipo concreto del nodo aggregato.</typeparam>
    /// <typeparam name="TValue">Tipo concreto del nodo foglia.</typeparam>
    /// <remarks>
    /// <para><b>Preconditions:</b> <paramref name="newChild"/> non deve essere nullo; non può coincidere con <paramref name="current"/>.</para>
    /// <para><b>Postconditions:</b> Solleva eccezioni se la relazione è duplicata, inconsistente o ciclica.</para>
    /// <para><b>Side Effects:</b> Nessuno.</para>
    /// </remarks>
    public static void EnsureCanAddChild<TSelf, TValue>(
        TSelf current,
        TSelf newChild)
        where TSelf : class, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        Guard.ThrowIfNull(newChild);
        Guard.ThrowIfSelfReference(current, newChild);
        Guard.ThrowIfInvalidChildBinding<TSelf, TValue>(current, newChild);
        Guard.ThrowIfCycleDetected<TSelf, TValue>(current, newChild);
    }

    /// <summary>
    /// Verifica se è possibile aggiungere un nodo foglia al nodo aggregato corrente.
    /// </summary>
    /// <param name="valueChild">Il nodo corrente (parent dichiarato nella foglia).</param>
    /// <param name="newChild">Il nodo foglia da aggiungere.</param>
    /// <typeparam name="TSelf">Tipo concreto del nodo aggregato.</typeparam>
    /// <typeparam name="TValue">Tipo concreto del nodo foglia.</typeparam>
    /// <remarks>
    /// <para><b>Preconditions:</b> <paramref name="newChild"/> non deve essere nullo e deve riferirsi al parent corretto.</para>
    /// <para><b>Postconditions:</b> Solleva eccezioni se il nodo foglia è duplicato o il parent non è coerente.</para>
    /// <para><b>Side Effects:</b> Nessuno.</para>
    /// </remarks>
    public static void EnsureCanAddChild<TSelf, TValue>(
        TSelf valueChild,
        TValue newChild)
        where TSelf : class, IAggregateNode<TSelf, TValue>
        where TValue : class, IValueNode<TValue, TSelf>
    {
        Guard.ThrowIfNull(newChild);
        Guard.ThrowIfNotExpectedParent(valueChild, newChild);
        Guard.ThrowIfDuplicateLeaf(valueChild, newChild);
    }
}