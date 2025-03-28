namespace JsonBridgeEF.Common.EfEntities.Exceptions;

/// <summary>
/// Eccezione sollevata quando un'entità non appartiene al proprietario atteso.
/// </summary>
public class EntityOwnershipMismatchException(string entityName, string expectedOwnerName)
    : Exception($"L'entità '{entityName}' non appartiene al proprietario '{expectedOwnerName}'.")
{
}