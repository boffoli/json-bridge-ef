using JsonBridgeEF.Seeding.Target.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.DbContextInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Model;
using JsonBridgeEF.Shared.Navigation.Helpers;
using JsonBridgeEF.Shared.Navigation.Interfaces;

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
        #region Campi Privati

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

            // Inizializza la logica di navigazione: la configurazione dei delegati si trova nella partial Navigation.
            InitializeNavigation();

            // Collegamento 1:N con il DbContext
            DbContextInfo = dbContext;
            dbContext.AddTargetEntity(this);
        }

        #endregion

        #region Proprietà Strutturali

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

        /// <inheritdoc />
        /// <summary>
        /// Metodo hook eseguito automaticamente al termine del flusso di aggiunta di un'entità figlia (<see cref="ClassInfo"/>).
        /// </summary>
        /// <param name="child">L'entità figlia appena aggiunta.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il flusso di aggiunta è stato completato con successo.</para>
        /// <para><b>Postconditions:</b> Aggiorna lo stato interno tramite <see cref="Touch"/>.</para>
        /// <para><b>Side Effects:</b> La proprietà <c>UpdatedAt</c> viene aggiornata se implementato.</para>
        /// </remarks>
        protected sealed override void OnAfterAddChildFlow(ClassInfo child)
        {
            this.Touch();
        }

        /// <inheritdoc />
        /// <summary>
        /// Metodo hook eseguito automaticamente al termine del flusso di aggiunta di una proprietà (<see cref="ClassProperty"/>).
        /// </summary>
        /// <param name="child">La proprietà appena aggiunta.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Il flusso di aggiunta è stato completato con successo.</para>
        /// <para><b>Postconditions:</b> Aggiorna lo stato interno tramite <see cref="Touch"/>.</para>
        /// <para><b>Side Effects:</b> La proprietà <c>UpdatedAt</c> viene aggiornata se implementato.</para>
        /// </remarks>
        protected sealed override void OnAfterAddChildFlow(ClassProperty child)
        {
            this.Touch();
        }
    }
}