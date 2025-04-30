using JsonBridgeEF.Seeding.Source.JsonComponents.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces
{
    /// <summary>
    /// Domain Interface: Marker comune per tutte le proprietà JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Indica che un oggetto rappresenta una proprietà di un'entità JSON, indipendentemente dalla struttura interna.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementato direttamente o indirettamente dalle proprietà JSON concrete.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessun requisito specifico; funge esclusivamente da marker.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IJsonComponent"/> per integrare il modello di componente JSON.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utile per operazioni polimorfiche su collezioni eterogenee di proprietà JSON.</para>
    /// </remarks>
    public interface IJsonProperty : IJsonComponent
    {
        /// <summary>
        /// Designa questa proprietà come chiave logica per l'oggetto JSON proprietario.
        /// </summary>
        /// <remarks>
        /// <para><b>Usage Notes:</b> Imposta la proprietà come identificatore univoco.</para>
        /// </remarks>
        void MarkAsKey();

        /// <summary>
        /// Rimuove la designazione di chiave logica da questa proprietà.
        /// </summary>
        /// <remarks>
        /// <para><b>Usage Notes:</b> Annulla la designazione di chiave logica.</para>
        /// </remarks>
        void UnmarkAsKey();
    }

    /// <summary>
    /// Domain Interface: Proprietà JSON tipizzata fortemente.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della proprietà JSON. Deve ereditare da <see cref="IJsonProperty{TSelf, TParent}"/>.
    /// </typeparam>
    /// <typeparam name="TParent">
    /// Il tipo dell'oggetto JSON proprietario. Deve ereditare da <see cref="IJsonEntity{TParent, TSelf}"/>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un campo o attributo di un oggetto JSON, con supporto per l’ownership e l’identificazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// Appartiene a <typeparamref name="TParent"/> e implementa <see cref="IEntityProperty{TSelf, TParent}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - Estende <see cref="IJsonProperty"/> per ereditare <c>MarkAsKey</c>/<c>UnmarkAsKey</c>.<br/>
    /// - Aggiunge le funzionalità fortemente tipizzate di <see cref="IEntityProperty{TSelf, TParent}"/>.</para>
    /// </remarks>
    public interface IJsonProperty<TSelf, TParent> : IJsonProperty, IEntityProperty<TSelf, TParent>
        where TSelf : class, IJsonProperty<TSelf, TParent>
        where TParent : class, IJsonEntity<TParent, TSelf>
    {
        // qui non serve più ripetere MarkAsKey/UnmarkAsKey
    }
}