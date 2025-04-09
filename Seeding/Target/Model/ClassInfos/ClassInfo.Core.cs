using JsonBridgeEF.Seeding.Target.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Navigation.Interfaces;
using JsonBridgeEF.Shared.Navigation.Helpers;

namespace JsonBridgeEF.Seeding.Target.Model.ClassInfos
{
    /// <inheritdoc cref="IClassInfo{TSelf, TClassProperty}"/>
    /// <summary>
    /// Concrete Domain Class: rappresenta una classe nel modello orientato agli oggetti, composta da proprietà strutturate.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Costruita tramite costruttore esplicito con nome, namespace e contesto EF di riferimento. Il nome qualificato viene derivato internamente.</para>
    /// <para><b>Constraints:</b> Tutti i parametri obbligatori; la classe deve appartenere a un solo <see cref="DbContextInfo"/>.</para>
    /// <para><b>Relationships:</b> Aggrega <see cref="ClassProperty"/> e appartiene a un <see cref="DbContextInfo"/> come entità target.</para>
    /// </remarks>
    internal sealed partial class ClassInfo : Entity<ClassInfo, ClassProperty>,
                                               IClassInfo<ClassInfo, ClassProperty>,
                                               IParentNavigableNode<ClassInfo>,
                                               IDomainMetadata,
                                               IEfEntity
    {
        #region Campi

        private readonly string _namespace;
        private readonly string _classQualifiedName;

        #endregion

        #region Costruttore

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="ClassInfo"/>, associandola al relativo <see cref="DbContextInfo"/>.
        /// Il nome qualificato completo viene derivato automaticamente.
        /// </summary>
        /// <param name="name">Nome univoco della classe.</param>
        /// <param name="namespace">Namespace della classe (es. <c>MyApp.Models</c>).</param>
        /// <param name="dbContext">Contesto al quale la classe appartiene (non può essere null).</param>
        /// <exception cref="ArgumentNullException">
        /// Sollevata se <paramref name="namespace"/> o <paramref name="dbContext"/> è null o vuoto.
        /// </exception>
        public ClassInfo(string name, string @namespace, DbContextInfo dbContext)
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "Il namespace non può essere nullo o vuoto.");
            if (dbContext is null)
                throw new ArgumentNullException(nameof(dbContext), "Il contesto DbContext associato non può essere nullo.");

            _namespace = @namespace;
            _classQualifiedName = $"{@namespace}.{name}";
            _metadata = new DomainMetadata(name);
            _parentManager = new ParentNavigationManager<ClassInfo, ClassProperty>(this);

            // Collegamento 1:N con il DbContext
            DbContextInfo = dbContext;
            dbContext.AddTargetEntity(this);
        }

        #endregion

        #region Proprietà strutturali

        /// <inheritdoc />
        public string Namespace => _namespace;

        /// <inheritdoc />
        public string ClassQualifiedName => _classQualifiedName;

        #endregion

        #region Navigazione verso DbContextInfo

        /// <summary>
        /// Navigazione verso il <see cref="DbContextInfo"/> di appartenenza.
        /// </summary>
        public DbContextInfo DbContextInfo { get; }

        #endregion

        #region Validazione

        /// <inheritdoc />
        protected override void AdditionalCustomValidateEntity(ClassInfo child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        /// <inheritdoc />
        protected override void AdditionalCustomValidateProperty(ClassProperty child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        #endregion
    }
}