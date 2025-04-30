using JsonBridgeEF.Seeding.Target.ClassComponents.Interfaces;
using JsonBridgeEF.Seeding.Target.ClassInfos.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Target.ClassProperties.Interfaces
{
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