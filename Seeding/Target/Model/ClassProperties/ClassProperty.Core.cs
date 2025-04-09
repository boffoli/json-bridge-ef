using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Seeding.Target.Interfaces;

namespace JsonBridgeEF.Seeding.Target.Model.Properties
{
    /// <inheritdoc cref="IClassProperty{TSelf, TParent}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta una proprietà associata a un oggetto <see cref="ClassModel"/> nel modello orientato agli oggetti.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Costruita tramite costruttore con nome, classe genitore e nome qualificato completo.</para>
    /// <para><b>Constraints:</b> Il nome qualificato completo non può essere nullo o vuoto.</para>
    /// <para><b>Relationships:</b> Associata a una classe genitore <see cref="ClassInfo"/> come parte del modello aggregato.</para>
    /// </remarks>
    internal sealed partial class ClassProperty : EntityProperty<ClassProperty, ClassInfo>,
                                                  IClassProperty<ClassProperty, ClassInfo>,
                                                  IDomainMetadata,
                                                  IEfEntity
    {
        private readonly string _fullyQualifiedPropertyName;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="ClassProperty"/>.
        /// </summary>
        /// <param name="name">Nome della proprietà.</param>
        /// <param name="parent">Classe proprietaria.</param>
        /// <param name="isKey">Indica se la proprietà è una chiave logica.</param>
        public ClassProperty(string name, ClassInfo parent, bool isKey = false)
            : base(name, parent, isKey)
        {
            ArgumentNullException.ThrowIfNull(parent);
            _fullyQualifiedPropertyName = $"{parent.ClassQualifiedName}.{name}";
            _metadata = new DomainMetadata(name);
        }

        /// <inheritdoc />
        public string FullyQualifiedPropertyName => _fullyQualifiedPropertyName;
    }
}