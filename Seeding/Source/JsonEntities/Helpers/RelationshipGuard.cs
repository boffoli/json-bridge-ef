using JsonBridgeEF.Seeding.Source.JsonEntities.Exceptions;
using JsonBridgeEF.Seeding.Source.JsonEntities.Model;
using JsonBridgeEF.Seeding.Source.JsonProperties.Model;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Helpers;

/// <summary>
/// <para><b>Domain Concept:</b><br/>
/// Componente di validazione relazionale tra blocchi JSON.  
/// Garantisce l’integrità logica impedendo relazioni cicliche o incoerenti tra blocchi padre e figlio.
/// </para>
///
/// <para><b>Usage:</b><br/>
/// Usato internamente da <see cref="JsonEntity"/> prima dell’aggiunta di blocchi figli o padri.
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
    /// <para><b>Domain Concept:</b><br/>
    /// Impedisce che un'entità venga aggiunta come padre di sé stessa.
    /// </para>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="JsonEntityRelationshipException">Se padre e figlio coincidono.</exception>
    public static void EnsureNotSelfReference(JsonEntity parent, JsonEntity child)
    {
        if (ReferenceEquals(parent, child))
            throw JsonRelationshipError.SelfParent(child.Name);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Impedisce l'inversione diretta della relazione padre-figlio.
    /// </para>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="JsonRelationshipException">
    /// Se il blocco figlio è già padre diretto del blocco proposto come padre.
    /// </exception>
    public static void EnsureNotReverseReference(JsonEntity parent, JsonEntity child)
    {
        if (child.Parents.Contains(parent))
        {
            throw JsonRelationshipError.InvertedParenting(parent.Name, child.Name);
        }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Impedisce la formazione di cicli nella gerarchia padre-figlio.
    /// </para>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    /// <exception cref="JsonEntityRelationshipException">
    /// Se si crea un ciclo implicando che il padre sia un suo stesso discendente.
    /// </exception>
    public static void EnsureNotRecursive(JsonEntity parent, JsonEntity child)
    {
        if (IsRecursiveReference(parent, child))
        {
            throw JsonRelationshipError.CircularReference(parent.Name, child.Name);
        }
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Esegue tutte le validazioni logiche necessarie prima di stabilire una relazione padre-figlio.
    /// </para>
    /// <param name="parent">Blocco padre proposto.</param>
    /// <param name="child">Blocco figlio proposto.</param>
    public static void Validate(JsonEntity parent, JsonEntity child)
    {
        EnsureNotSelfReference(parent, child);
        EnsureNotReverseReference(parent, child);
        EnsureNotRecursive(parent, child);
    }

    /// <summary>
    /// <para><b>Domain Concept:</b><br/>
    /// Controlla ricorsivamente se il blocco figlio proposto è un antenato del padre.
    /// </para>
    /// <param name="parent">Nodo che si vuole impostare come genitore.</param>
    /// <param name="child">Nodo che si vuole impostare come figlio.</param>
    /// <returns><c>true</c> se il padre proposto è già nella catena di antenati del figlio; altrimenti <c>false</c>.</returns>
    private static bool IsRecursiveReference(JsonEntity parent, JsonEntity child)
    {
        foreach (var grandparent in child.Parents)
        {
            if (ReferenceEquals(parent, grandparent))
                return true;

            if (IsRecursiveReference(parent, grandparent))
                return true;
        }

        return false;
    }
}
