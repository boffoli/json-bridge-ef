using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Target.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Model;

namespace JsonBridgeEF.Seeding.Target.Model.ClassInfos
{
    /// <inheritdoc cref="IClassInfo{TSelf, TClassProperty}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta una classe nel modello orientato agli oggetti, composta da proprietà strutturate.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Iniezione via costruttore di nome, namespace, descrizione, contesto EF e validator.</para>
    /// <para><b>Constraints:</b><br/>
    /// - <paramref name="name"/> non può essere null, vuoto o whitespace.<br/>
    /// - <paramref name="namespace"/> non può essere null, vuoto o whitespace.<br/>
    /// - <paramref name="dbContext"/> non può essere null.<br/>
    /// - <paramref name="description"/> è opzionale e può essere null o vuoto.<br/>
    /// - <paramref name="validator"/> deve implementare <see cref="IValidateAndFix{ClassInfo}"/>.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Aggrega <see cref="ClassProperty"/> come proprietà figlie.<br/>
    /// - Appartiene a un <see cref="DbContextInfo"/> come entità target.<br/>
    /// - Utilizza <see cref="IValidateAndFix{ClassInfo}"/> per la validazione a runtime.</para>
    /// </remarks>
    internal sealed partial class ClassInfo
        : Entity<ClassInfo, ClassProperty>,
          IClassInfo<ClassInfo, ClassProperty>,
          IDomainMetadata,
          IEfEntity
    {

        /// <summary>
        /// Campo che incapsula i metadati di dominio tramite il value object <see cref="DomainMetadata"/>.
        /// </summary>
        private readonly DomainMetadata _metadata;
        private readonly string _namespace;
        private readonly string _classQualifiedName;

        /// <summary>
        /// Costruisce una nuova istanza di <see cref="ClassInfo"/>, validandola immediatamente.
        /// </summary>
        /// <param name="name">Nome univoco della classe (non null/empty).</param>
        /// <param name="namespace">Namespace della classe (es. “MyApp.Models”), non null/empty.</param>
        /// <param name="description">Descrizione testuale opzionale della classe.</param>
        /// <param name="dbContext">Contesto EF associato (non null).</param>
        /// <param name="validator">
        /// Validator da iniettare per <see cref="ClassInfo"/>; implementa
        /// <see cref="IValidateAndFix{ClassInfo}"/> e viene eseguito subito dopo la creazione.</param>
        /// <exception cref="ArgumentNullException">
        /// Se <paramref name="namespace"/> o <paramref name="dbContext"/> è null, oppure <paramref name="name"/> è null/empty.</exception>
        public ClassInfo(
            string name,
            string @namespace,
            string? description,
            DbContextInfo dbContext,
            IValidateAndFix<ClassInfo> validator
        ) : base(name, validator)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "Il namespace non può essere nullo o vuoto.");

            _namespace = @namespace;
            _classQualifiedName = $"{@namespace}.{name}";

            _metadata = new(name, description);

            DbContextInfo = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            dbContext.AddTargetEntity(this);

            // Imposta la descrizione nel metadata
            this.Description = description;
        }

        /// <inheritdoc />
        public string Namespace => _namespace;

        /// <inheritdoc />
        public string ClassQualifiedName => _classQualifiedName;

        /// <inheritdoc />
        public DbContextInfo DbContextInfo { get; }

        /// <inheritdoc />
        protected sealed override void OnAfterAddChildFlow(ClassInfo child) => Touch();

        /// <inheritdoc />
        protected sealed override void OnAfterAddChildFlow(ClassProperty child) => Touch();
    }
}