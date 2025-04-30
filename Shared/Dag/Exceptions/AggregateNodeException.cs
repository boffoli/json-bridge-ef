using System;

namespace JsonBridgeEF.Shared.Dag.Exceptions
{
    /// <summary>
    /// Domain Concept: Eccezione base per anomalie semantiche nei nodi aggregati 
    /// (es. cicli, figli nulli, chiavi duplicate, self-reference, binding non valido, foglie duplicate).
    /// </summary>
    public abstract class AggregateNodeException : NodeException
    {
        /// <summary>Crea una nuova eccezione di tipo <see cref="AggregateNodeException"/>.</summary>
        protected AggregateNodeException(string message)
            : base(message)
        { }

        /// <summary>Crea una nuova eccezione di tipo <see cref="AggregateNodeException"/>, con eccezione interna.</summary>
        protected AggregateNodeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando si tenta di aggiungere un figlio nullo a un nodo aggregato.</summary>
    internal sealed class AggregateNodeNullChildException(string parentName) : AggregateNodeException($"❌ Il nodo aggregato '{parentName}' contiene un riferimento nullo a un figlio.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando il nodo aggregato tenta di aggiungere sé stesso come figlio o genitore.</summary>
    internal sealed class AggregateNodeSelfReferenceException(string nodeName) : AggregateNodeException($"❌ Il nodo aggregato '{nodeName}' non può riferirsi a sé stesso.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando viene rilevato un ciclo nella gerarchia dei nodi aggregati.</summary>
    internal sealed class AggregateNodeCycleException(string parentName, string childName) : AggregateNodeException($"❌ Ciclo rilevato tra i nodi '{parentName}' e '{childName}'.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando una foglia è già presente nel nodo aggregato.</summary>
    internal sealed class AggregateNodeDuplicateLeafException(string leafName) : AggregateNodeException($"❌ Il nodo aggregato contiene già la foglia '{leafName}'.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando viene rilevata una chiave duplicata tra i figli del nodo aggregato.</summary>
    internal sealed class AggregateNodeDuplicateChildKeyException(string parentName, string duplicatedKey) : AggregateNodeException($"❌ Il nodo aggregato '{parentName}' ha più figli con la chiave '{duplicatedKey}'.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando viene rilevato un binding non valido tra parent e child.</summary>
    internal sealed class AggregateNodeInvalidParentBindingException(string sourceName, string targetName) : AggregateNodeException($"❌ Binding non valido: '{sourceName}' ↔ '{targetName}'.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando viene rilevato un binding non valido tra child e parent.</summary>
    internal sealed class AggregateNodeInvalidChildBindingException(string sourceName, string targetName) : AggregateNodeException($"❌ Binding inverso non valido: '{sourceName}' ↔ '{targetName}'.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando il nodo aggregato non supporta la navigazione verso i genitori.</summary>
    internal sealed class AggregateNodeParentNavigationNotSupportedException(string nodeName) : AggregateNodeException($"❌ Il nodo aggregato '{nodeName}' non supporta la navigazione verso i genitori.")
    {
    }

    /// <summary>
    /// Domain Concept: Eccezione sollevata quando una foglia è associata a un parent non previsto.</summary>
    internal sealed class AggregateNodeNotExpectedParentException : AggregateNodeException
    {
        public AggregateNodeNotExpectedParentException(string leafName, string expectedParentName)
            : base($"❌ La foglia '{leafName}' non appartiene al parent atteso '{expectedParentName}'.")
        { }
    }

    /// <summary>
    /// Domain Concept: Factory per eccezioni semantiche specifiche dei nodi aggregati 
    /// e composizione con <see cref="NodeError"/> per errori generici di nodo.</summary>
    internal static class AggregateNodeError
    {
        /// <summary>Figlio nullo.</summary>
        public static AggregateNodeException NullChild(string parentName) =>
            new AggregateNodeNullChildException(parentName);

        /// <summary>Self-reference non ammessa.</summary>
        public static AggregateNodeException SelfReference(string nodeName) =>
            new AggregateNodeSelfReferenceException(nodeName);

        /// <summary>Ciclo parent-child rilevato.</summary>
        public static AggregateNodeException CycleDetected(string parentName, string childName) =>
            new AggregateNodeCycleException(parentName, childName);

        /// <summary>Foglia duplicata.</summary>
        public static AggregateNodeException DuplicateLeaf(string leafName) =>
            new AggregateNodeDuplicateLeafException(leafName);

        /// <summary>Chiave duplicata tra figli.</summary>
        public static AggregateNodeException DuplicateChildKey(string parentName, string duplicatedKey) =>
            new AggregateNodeDuplicateChildKeyException(parentName, duplicatedKey);

        /// <summary>Binding parent→child non valido.</summary>
        public static AggregateNodeException InvalidParentBinding(string sourceName, string targetName) =>
            new AggregateNodeInvalidParentBindingException(sourceName, targetName);

        /// <summary>Binding child→parent non valido.</summary>
        public static AggregateNodeException InvalidChildBinding(string sourceName, string targetName) =>
            new AggregateNodeInvalidChildBindingException(sourceName, targetName);

        /// <summary>Parent navigation non supportata.</summary>
        public static AggregateNodeException ParentNavigationNotSupported(string nodeName) =>
            new AggregateNodeParentNavigationNotSupportedException(nodeName);

        /// <summary>Foglia in parent non previsto.</summary>
        public static AggregateNodeException NotExpectedParent(string leafName, string expectedParentName) =>
            new AggregateNodeNotExpectedParentException(leafName, expectedParentName);
    }
}