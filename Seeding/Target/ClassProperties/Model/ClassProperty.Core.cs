using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Target.ClassInfos.Model;
using JsonBridgeEF.Seeding.Target.ClassProperties.Interfaces;
using JsonBridgeEF.Seeding.Target.ClassProperties.Validators;
using JsonBridgeEF.Shared.EntityModel.Model;

namespace JsonBridgeEF.Seeding.Target.ClassProperties.Model
{
    /// <inheritdoc cref="IClassProperty{TSelf, TParent}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta una proprietà associata a un oggetto <see cref="ClassInfo"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Iniezione via costruttore di nome, classe genitore, flag chiave, descrizione e validatore.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Il <paramref name="name"/> non può essere nullo, vuoto o whitespace.<br/>
    /// - Il <paramref name="parent"/> non può essere null.<br/>
    /// - La <paramref name="description"/> è opzionale (null viene trattato come stringa vuota).</para>
    /// <para><b>Relationships:</b><br/>
    /// - Associata all’entità <see cref="ClassInfo"/> come foglia nel modello aggregato.<br/>
    /// - Collabora con il validator <see cref="ClassPropertyValidator"/> per applicare i vincoli.</para>
    /// </remarks>
    /// <param name="name">Nome della proprietà.</param>
    /// <param name="parent">Classe proprietaria.</param>
    /// <param name="isKey">Se <c>true</c>, la proprietà viene marcata come chiave logica.</param>
    /// <param name="description">Descrizione della proprietà.</param>
    /// <param name="validator">
    /// Validator da iniettare per <see cref="ClassProperty"/>; se null verrà usato <see cref="ClassPropertyValidator"/>.</param>
    internal sealed partial class ClassProperty(
        string name,
        ClassInfo parent,
        bool isKey,
        string? description,
        IValidateAndFix<ClassProperty>? validator
    )
        : EntityProperty<ClassProperty, ClassInfo>(
            name,
            parent,
            isKey,
            validator ?? new ClassPropertyValidator()
          ),
          IClassProperty<ClassProperty, ClassInfo>
    {
        private readonly string _fullyQualifiedPropertyName = $"{parent.ClassQualifiedName}.{name}";

        /// <inheritdoc />
        public string FullyQualifiedPropertyName => _fullyQualifiedPropertyName;
    }
}