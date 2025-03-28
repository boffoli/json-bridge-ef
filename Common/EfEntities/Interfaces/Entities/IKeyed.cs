namespace JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

/// <summary>
/// Domain Interface: Rappresenta un'entità che può essere marcata come "chiave logica"
/// all'interno di una collezione gestita da un aggregate root.
/// </summary>
/// <remarks>
/// <para><b>Domain Concept:</b><br/>
/// Le entità che implementano <c>IKeyed</c> possono essere designate come elemento principale
/// o rappresentativo all’interno di una collezione di entità, assumendo il ruolo di "chiave logica".</para>
///
/// <para><b>Creation Strategy:</b><br/>
/// L'attributo di chiave è impostabile solo tramite metodi espliciti, per evitare ambiguità nei setter automatici.
/// </para>
///
/// <para><b>Constraints:</b><br/>
/// - Solo un'entità per collezione può avere <c>IsKey = true</c>.<br/>
/// - Il dominio o il gestore della collezione è responsabile del rispetto di questo vincolo.</para>
///
/// <para><b>Usage Notes:</b><br/>
/// - Utilizzata in contesti dove un solo elemento rappresenta la configurazione attiva, predefinita o principale.<br/>
/// - La modifica dello stato di chiave logica deve avvenire tramite chiamate esplicite a <c>MarkAsKey()</c> e <c>UnmarkAsKey()</c>.</para>
/// </remarks>
public interface IKeyed
{
    /// <summary>
    /// Indica se l’entità è attualmente marcata come chiave logica.
    /// </summary>
    bool IsKey { get; }

    /// <summary>
    /// Marca l’entità come chiave logica nella collezione.
    /// </summary>
    void MarkAsKey();

    /// <summary>
    /// Rimuove la marcatura come chiave logica.
    /// </summary>
    void UnmarkAsKey();
}