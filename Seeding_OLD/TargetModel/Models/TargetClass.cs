using System.ComponentModel;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

namespace JsonBridgeEF.Seeding.TargetModel.Models;
{
    /// <summary>
    /// Domain Class: Rappresenta una “classe di destinazione” (analogamente a un Blocco JSON) 
    /// che funge da aggregate root per un insieme coerente di <see cref="TargetProperty"/>.
    /// Può, facoltativamente, far parte di una gerarchia (genitore-figlio) se lo scenario lo prevede.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Ogni <c>TargetClass</c> appartiene a un <see cref="TargetDbContextInfo"/> e aggrega 
    /// una collezione di <see cref="TargetProperty"/> (entità figlie “owned”).</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere creato tramite il costruttore dominio, fornendo nome e owner (<see cref="TargetDbContextInfo"/>).
    /// Il costruttore EF è riservato al framework.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// - Il nome è obbligatorio e univoco all’interno del contesto owner.<br/>
    /// - Solo una <see cref="TargetProperty"/> può essere marcata come chiave.<br/>
    /// - Eventuali relazioni padre-figlio tra <c>TargetClass</c> devono essere coerenti e simmetriche 
    ///   (solo se hai bisogno di una gerarchia di “classi target”).</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a uno <see cref="TargetDbContextInfo"/> (owner).<br/>
    /// - Aggrega <see cref="TargetProperty"/> come entità figlie “owned” e “keyed”.<br/>
    /// - Se previsto, può avere classi genitrici e figlie, analogamente a <see cref="IHierarchical{T}"/>.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// - Usare <see cref="AddEntity(TargetProperty)"/> per aggiungere una proprietà.<br/>
    /// - Usare <see cref="GetKeyEntity()"/> per ottenere la property marcata come chiave, se presente.<br/>
    /// - Eventuali metodi <c>AddChild</c>, <c>AddParent</c> per la gestione di una gerarchia di TargetClass.</para>
    /// </remarks>
    public sealed class TargetClass 
        : BaseEfHierarchicalOwnedEntityWithKeyedOwnedEntities<TargetClass, TargetDbContextInfo, TargetProperty>
    {
        // 🔹 COSTRUTTORE RISERVATO A EF CORE 🔹
#pragma warning disable S1133
        [Obsolete("Reserved for EF Core materialization only", error: false)]
#pragma warning disable CS8618
        [EditorBrowsable(EditorBrowsableState.Never)]
        private TargetClass() : base() 
        {
            // EF lo userà per materializzare
        }
#pragma warning restore CS8618
#pragma warning restore S1133

        // 🔹 COSTRUTTORE DOMINIO 🔹

        /// <summary>
        /// Domain Constructor: Inizializza una nuova "classe target" appartenente a un contesto DB.
        /// </summary>
        /// <param name="name">Nome della classe (univoco all’interno dell’owner).</param>
        /// <param name="dbContextInfo">Contesto EF target di appartenenza.</param>
        /// <param name="validator">Validatore opzionale per le regole di dominio.</param>
        public TargetClass(
            string name,
            TargetDbContextInfo dbContextInfo,
            IValidateAndFix<TargetClass>? validator = null
        ) : base(name, dbContextInfo, validator)
        {
        }

        // 🔹 DOMAIN PROPERTIES (se servono proprietà specifiche) 🔹

        /// <summary>
        /// Determina se la classe è "indipendente", cioè se esiste almeno una property marcata come chiave.
        /// </summary>
        public bool IsIndependent() => KeyEntity != null;

        // 🔹 CONFIGURAZIONE / SLUG 🔹

        /// <inheritdoc />
        protected sealed override bool HasSlug => true;

        // 🔹 GESTIONE CHIAVE 🔹

        /// <summary>
        /// Imposta la property specificata come chiave logica della classe.
        /// </summary>
        /// <param name="keyProp">Property da marcare come chiave.</param>
        /// <param name="force">Se true, sostituisce un’eventuale chiave esistente.</param>
        public void MakeIndependent(TargetProperty keyProp, bool force = false)
        {
            _keyedCollection.AddKey(keyProp, force);
        }

        /// <summary>
        /// Imposta come chiave la property con il nome specificato.
        /// </summary>
        /// <param name="propertyName">Nome della property da marcare come chiave.</param>
        /// <param name="force">Se true, sostituisce un’eventuale chiave esistente.</param>
        public void MakeIndependent(string propertyName, bool force = false)
        {
            _keyedCollection.MarkAsKey(propertyName, force);
        }

        /// <summary>
        /// Rimuove la chiave logica, rendendo la classe "dipendente".
        /// </summary>
        /// <returns><c>true</c> se una chiave è stata rimossa, <c>false</c> altrimenti.</returns>
        public bool MakeDependent()
        {
            return _keyedCollection.UnmarkAsKey();
        }

        // 🔹 VALIDAZIONE (override se necessario) 🔹

        /// <inheritdoc/>
        protected sealed override void OnBeforeValidate() { /* ... */ }

        /// <inheritdoc/>
        protected sealed override void OnAfterValidate() { /* ... */ }

        // 🔹 TO STRING (opzionale) 🔹

        /// <inheritdoc/>
        public override string ToString()
            => $"{Name} (Properties: {Entities.Count}, Key: {KeyEntity?.Name ?? "None"})";
    }
}