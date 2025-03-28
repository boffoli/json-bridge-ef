namespace JsonBridgeEF.Common.EfEntities.Exceptions;

/// <summary>
/// Eccezione di dominio base per tutte le operazioni su collezioni con chiave logica.
/// </summary>
public abstract class KeyedEntityException(string message) : Exception(message)
{
}

/// <summary>
/// Eccezione sollevata quando si tenta di impostare una chiave mentre è già presente una entità chiave.
/// </summary>
public class KeyedEntityKeyAlreadyPresentException(string keyName)
    : KeyedEntityException($"Un'entità chiave ('{keyName}') è già presente nella collezione.")
{
}

/// <summary>
/// Eccezione sollevata quando un'entità non viene trovata nella collezione.
/// </summary>
public class KeyedEntityNotFoundException(string entityName)
    : KeyedEntityException($"L'entità '{entityName}' non è stata trovata nella collezione.")
{
}