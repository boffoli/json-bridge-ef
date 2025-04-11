using JsonBridgeEF.Seeding.SourceJson.Exceptions;
using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Componente di validazione relazionale tra blocchi JSON.  
/// Garantisce l’integrità logica impedendo relazioni cicliche o incoerenti tra blocchi padre e figlio.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Usato internamente da <see cref="JsonBlock"/> prima dell’aggiunta di blocchi figli o padri.
/// </para>
///
/// <para><b>Constraints:</b>
/// <list type="bullet">
///   <item>Un blocco non può essere padre di sé stesso.</item>
///   <item>Un blocco non può essere padre del proprio padre (inversione diretta).</item>
///   <item>Un blocco non può essere padre di un antenato indiretto (relazione ciclica).</item>
/// </list>
/// </para>
///
/// <para><b>Testability:</b><br/>
/// Essendo una classe statica, è testabile separatamente in <c>RelationshipGuardTests</c>.
/// </para>
/// </summary>
internal static class RelationshipGuard
{
    /// <summary>
    /// Verifica che il blocco padre e il blocco figlio non siano la stessa istanza.
    /// </summary>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="BlockRelationshipException">Se padre e figlio coincidono.</exception>
    public static void EnsureNotSelfReference(JsonBlock parent, JsonBlock child)
    {
        if (ReferenceEquals(parent, child))
            throw new BlockRelationshipException("Un blocco non può essere padre di sé stesso.");
    }

    /// <summary>
    /// Verifica che il blocco figlio non sia già padre diretto del blocco proposto come padre.
    /// </summary>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="BlockRelationshipException">Se il figlio è già padre del padre.</exception>
    public static void EnsureNotReverseReference(JsonBlock parent, JsonBlock child)
    {
        if (child.ParentBlocks.Contains(parent))
        {
            throw new BlockRelationshipException(
                $"Relazione non valida: il blocco '{child.Name}' è già padre del blocco '{parent.Name}' (inversione diretta).");
        }
    }

    /// <summary>
    /// Verifica che il blocco figlio non sia un antenato ricorsivo del padre proposto.
    /// </summary>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="BlockRelationshipException">Se si crea un ciclo nella gerarchia.</exception>
    public static void EnsureNotRecursive(JsonBlock parent, JsonBlock child)
    {
        if (IsRecursiveReference(parent, child))
        {
            throw new BlockRelationshipException(
                $"Relazione non valida: aggiungendo il blocco '{child.Name}' come figlio di '{parent.Name}' si creerebbe una dipendenza ciclica.");
        }
    }

    /// <summary>
    /// Esegue tutte le validazioni prima di aggiungere una relazione padre-figlio.
    /// </summary>
    public static void Validate(JsonBlock parent, JsonBlock child)
    {
        EnsureNotSelfReference(parent, child);
        EnsureNotReverseReference(parent, child);
        EnsureNotRecursive(parent, child);
    }

    /// <summary>
    /// Verifica ricorsivamente se il blocco figlio è già un antenato del padre.
    /// </summary>
    private static bool IsRecursiveReference(JsonBlock parent, JsonBlock child)
    {
        if (child.ParentBlocks.Contains(parent))
            return true;

        foreach (var grandparent in child.ParentBlocks)
        {
            if (IsRecursiveReference(parent, grandparent))
                return true;
        }

        return false;
    }
}