using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Shared.Dag.Exceptions;
using JsonBridgeEF.Shared.Dag.Interfaces;

namespace JsonBridgeEF.Shared.Dag.Validators;

/// <summary>
/// Domain Concept: Validatore base per tutti i nodi del grafo (<see cref="INode"/>).
/// </summary>
/// <remarks>
/// <para><b>Creation Strategy:</b><br/>
/// Istanziato una sola volta a livello di servizio.</para>
/// <para><b>Constraints:</b><br/>
/// Il nome deve essere non nullo, non vuoto e semanticamente valido.</para>
/// <para><b>Relationships:</b><br/>
/// Utilizzato da tutti i validatori specializzati di nodi e proprietà.</para>
/// <para><b>Usage Notes:</b><br/>
/// Applicabile a ogni istanza di <see cref="INode"/> o sue derivate.</para>
/// </remarks>
internal class NodeValidator : IValidateAndFix<INode>
{
    /// <inheritdoc />
    public void EnsureValid(INode node)
    {
        // Verifica che il nome non sia nullo o vuoto
        if (string.IsNullOrWhiteSpace(node.Name))
        {
            throw NodeError.InvalidName(node.GetType().Name);
        }
    }

    /// <inheritdoc />
    public void Fix(INode node)
    {
        // Nessuna correzione automatica prevista per il nome
    }
}