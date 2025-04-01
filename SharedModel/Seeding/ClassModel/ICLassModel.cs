using JsonBridgeEF.SharedModel.EntityModel;

namespace JsonBridgeEF.SharedModel.Seeding.ClassModel
{
    /// <summary>
    /// Domain Interface: Componente del modello a classi.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Interfaccia marker comune per tutti i componenti del modello orientato agli oggetti (OO).</para>
    /// </remarks>
    public interface IClassComponent
    {
        // Questa interfaccia funge da marker per i componenti del modello OO.
    }

    /// <summary>
    /// Domain Interface: Classe del modello orientato agli oggetti.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della classe. Deve ereditare da <c>IClassModel&lt;TSelf, TClassProperty&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TClassProperty">
    /// Il tipo concreto della proprietà della classe. Deve ereditare da <c>IClassProperty&lt;TClassProperty, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Una classe rappresenta un nodo aggregato che contiene proprietà e fornisce informazioni strutturali
    /// come il namespace e il nome completo della classe.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega istanze di <typeparamref name="TClassProperty"/>.<br/>
    /// - Implementa <see cref="IEntity{TSelf, TClassProperty}"/> per la gestione delle proprietà.<br/>
    /// - Estende <see cref="IClassComponent"/> per coerenza semantica nel modello OO.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata per rappresentare classi C# o entità analoghe all’interno di un grafo OO.</para>
    /// </remarks>
    public interface IClassModel<TSelf, TClassProperty> : IClassComponent, IEntity<TSelf, TClassProperty>
        where TSelf : class, IClassModel<TSelf, TClassProperty>
        where TClassProperty : class, IClassProperty<TClassProperty, TSelf>
    {
        /// <summary>
        /// Namespace della classe (es. <c>MyApp.Models</c>).
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Nome completo della classe, composto da namespace + nome (es. <c>MyApp.Models.User</c>).
        /// </summary>
        string ClassQualifiedName { get; }
    }

    /// <summary>
    /// Domain Interface: Proprietà della classe OO.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della proprietà. Deve ereditare da <c>IClassProperty&lt;TSelf, TParent&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TParent">
    /// Il tipo della classe proprietaria. Deve ereditare da <c>IClassModel&lt;TParent, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Una proprietà rappresenta un nodo foglia appartenente a una classe, tipicamente corrispondente a un campo semplice,
    /// e può essere designata come chiave logica.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a una classe che implementa <see cref="IClassModel{TSelf, TClassProperty}"/>.<br/>
    /// - Estende <see cref="IClassComponent"/> e implementa <see cref="IEntityProperty{TSelf, TParent}"/> per supporto a ownership e identificazione.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Il nome qualificato completo della proprietà, accessibile tramite <c>FullyQualifiedPropertyName</c>, è utile per identificazioni univoche.</para>
    /// </remarks>
    public interface IClassProperty<TSelf, TParent> : IClassComponent, IEntityProperty<TSelf, TParent>
        where TSelf : class, IClassProperty<TSelf, TParent>
        where TParent : class, IClassModel<TParent, TSelf>
    {
        /// <summary>
        /// Nome qualificato completo della proprietà (es. <c>MyApp.Models.User.FirstName</c>).
        /// </summary>
        string FullyQualifiedPropertyName { get; }
    }
}