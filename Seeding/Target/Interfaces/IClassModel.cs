using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Target.Interfaces
{
    /// <summary>
    /// Domain Interface: Marker per i componenti del modello a classi.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Indica che l'elemento appartiene al modello orientato agli oggetti (OO).</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementata direttamente dalle classi che rappresentano componenti strutturali del modello.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessuna logica, funge esclusivamente da marker per la categorizzazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IComponentModel"/> per supportare l’identificazione nominale nel grafo del modello a entità.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata per raggruppare logicamente tutte le entità o componenti del modello OO che condividono caratteristiche comuni di struttura e identificazione.</para>
    /// </remarks>
    public interface IClassComponent : IComponentModel
    {
        // Questa interfaccia funge da marker per i componenti del modello OO.
    }

    /// <summary>
    /// Domain Interface: Rappresenta una classe nel modello orientato agli oggetti.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della classe. Deve ereditare da <c>IClassInfo&lt;TSelf, TClassProperty&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TClassProperty">
    /// Il tipo concreto della proprietà della classe. Deve ereditare da <c>IClassProperty&lt;TClassProperty, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Una classe nel modello OO rappresenta un nodo aggregato che contiene proprietà e fornisce informazioni strutturali, 
    /// come il namespace e il nome completo della classe.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TClassProperty"/>.<br/>
    /// - Implementa <see cref="IEntity{TSelf, TClassProperty}"/> per la gestione delle proprietà.<br/>
    /// - Estende <see cref="IClassComponent"/> per garantire la coerenza nel modello OO.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata per rappresentare classi C# o entità analoghe all’interno di un grafo OO, offrendo informazioni strutturali complete.
    /// <br/><br/>
    /// <b>Nota:</b> Ogni classe deve essere creata con un riferimento al proprio <c>DbContextInfo</c>, che la gestisce e ne definisce il contesto di persistenza.
    /// </para>
    /// </remarks>
    public interface IClassInfo<TSelf, TClassProperty> : IClassComponent, IEntity<TSelf, TClassProperty>
        where TSelf : class, IClassInfo<TSelf, TClassProperty>
        where TClassProperty : class, IClassProperty<TClassProperty, TSelf>
    {
        /// <summary>
        /// Namespace della classe (es. <c>MyApp.Models</c>).
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Nome completo della classe, composto dal namespace e dal nome (es. <c>MyApp.Models.User</c>).
        /// </summary>
        string ClassQualifiedName { get; }
    }

    /// <summary>
    /// Domain Interface: Rappresenta una proprietà di una classe nel modello orientato agli oggetti.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della proprietà. Deve ereditare da <c>IClassProperty&lt;TSelf, TParent&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TParent">
    /// Il tipo della classe proprietaria. Deve ereditare da <c>IClassInfo&lt;TParent, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Una proprietà rappresenta un nodo foglia appartenente a una classe, tipicamente corrispondente a un campo o attributo,
    /// e può essere designata come chiave logica.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a una classe che implementa <see cref="IClassInfo{TSelf, TClassProperty}"/>.<br/>
    /// - Estende <see cref="IClassComponent"/> e implementa <see cref="IEntityProperty{TSelf, TParent}"/> per supportare l'ownership e l'identificazione univoca.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Il nome qualificato completo della proprietà, accessibile tramite <c>FullyQualifiedPropertyName</c>, facilita l'identificazione univoca all'interno del modello.</para>
    /// </remarks>
    public interface IClassProperty<TSelf, TParent> : IClassComponent, IEntityProperty<TSelf, TParent>
        where TSelf : class, IClassProperty<TSelf, TParent>
        where TParent : class, IClassInfo<TParent, TSelf>
    {
        /// <summary>
        /// Nome qualificato completo della proprietà (es. <c>MyApp.Models.User.FirstName</c>).
        /// </summary>
        string FullyQualifiedPropertyName { get; }
    }
}