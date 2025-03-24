using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.ZZZ;

internal static class TargetPropertyHierarchyBuilder
{
    /// <summary>
    /// Costruisce una gerarchia di proprietà target raggruppate per RootClass.
    /// Per ogni TargetProperty, se il campo Path è vuoto, la proprietà è di livello radice;
    /// se non lo è, il Path viene suddiviso e la proprietà viene inserita come foglia nell'albero.
    /// </summary>
    /// <param name="definitions">La lista di TargetProperty ottenuta dal database.</param>
    /// <returns>
    /// Un dizionario in cui la chiave è la RootClass e il valore è il nodo radice dell'albero delle proprietà target.
    /// </returns>
    internal static Dictionary<string, TargetPropertyNode2> BuildHierarchy(IEnumerable<TargetProperty> definitions)
    {
        var hierarchy = new Dictionary<string, TargetPropertyNode2>();

        foreach (var def in definitions)
        {
            // Assicuriamoci di avere un nodo radice per la RootClass corrente
            if (!hierarchy.TryGetValue(def.RootClass, out var rootNode))
            {
                rootNode = new TargetPropertyNode2();
                hierarchy[def.RootClass] = rootNode;
            }

            // Se il campo Path è vuoto, la proprietà va direttamente come figlia del nodo radice.
            if (string.IsNullOrWhiteSpace(def.Path))
            {
                // Se esiste già un nodo con quel nome, lo sovrascriviamo o lo aggiorniamo.
                rootNode.Children[def.Name] = new TargetPropertyNode2 { Propertyinition = def };
            }
            else
            {
                // Se Path non è vuoto, lo suddividiamo (ad es. "ShippingAddress.City")
                var parts = def.Path.Split('.');
                var currentNode = rootNode;
                foreach (var part in parts)
                {
                    if (!currentNode.Children.TryGetValue(part, out var childNode))
                    {
                        childNode = new TargetPropertyNode2();
                        currentNode.Children[part] = childNode;
                    }
                    currentNode = childNode;
                }
                // Aggiungiamo infine la proprietà vera e propria come nodo foglia
                currentNode.Children[def.Name] = new TargetPropertyNode2 { Propertyinition = def };
            }
        }

        return hierarchy;
    }
}

internal class TargetPropertyNode2
{
    /// <summary>
    /// Se la proprietà è effettivamente definita (cioè il nodo foglia contiene un mapping), 
    /// Propertyinition è non-null.
    /// </summary>
    public TargetProperty? Propertyinition { get; set; }

    /// <summary>
    /// I figli di questo nodo, indicizzati per il nome della proprietà.
    /// </summary>
    public Dictionary<string, TargetPropertyNode2> Children { get; set; } = [];
}