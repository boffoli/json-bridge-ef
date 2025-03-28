namespace JsonBridgeEF.Common.EfEntities.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si tenta di aggiungere un elemento già esistente nella collezione.
    /// </summary>
    public class NamedEntityAlreadyExistsException(string itemName, string ownerName) : Exception($"L'elemento '{itemName}' esiste già nell'aggregato '{ownerName}'.")
    {
    }
}
