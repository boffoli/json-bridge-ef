using JsonBridgeEF.Seeding.Target.ClassComponents.Interfaces;
using JsonBridgeEF.Seeding.Target.ClassProperties.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Target.ClassInfos.Interfaces
{
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
}