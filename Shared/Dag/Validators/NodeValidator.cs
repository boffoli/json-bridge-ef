using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore base per tutti i nodi del grafo (<see cref="Node"/>).
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Istanziato una sola volta a livello di servizio.</para>
/// <para>Constraints: Il nome deve essere non nullo, non vuoto e semanticamente valido.</para>
/// <para>Relationships: Utilizzato da tutti i validatori specializzati di nodi e proprietà.</para>
/// <para>Usage Notes: Applicabile a ogni istanza di <see cref="INode"/> o sue derivate.</para>
/// </remarks>
internal class NodeValidator : IValidateAndFix<INode>
{
    /// <inheritdoc />
    public void EnsureValid(INode node)
    {
        if (string.IsNullOrWhiteSpace(node.Name))
            throw new ValidationException("Il nome del nodo non può essere nullo o vuoto.");
    }

    /// <inheritdoc />
    public void Fix(INode node)
    {
        // Nessuna correzione automatica prevista per il nome
    }
}
