using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Source.Interfaces
{
    /// <summary>
    /// Domain Interface: Componente JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Interfaccia base per tutti i componenti di un grafo JSON tipizzato, fungendo da marker per gli elementi del modello JSON.</para>
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementata direttamente dalle classi che rappresentano componenti strutturali di un oggetto JSON.</para>
    /// <para><b>Constraints:</b><br/>
    /// Nessun requisito di implementazione specifico, si tratta esclusivamente di un marker per la categorizzazione.</para>
    /// <para><b>Relationships:</b><br/>
    /// Estende <see cref="IComponentModel"/> per integrare le funzionalità comuni del modello a entità, come l’identificazione nominale.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata per raggruppare tutti gli elementi del modello JSON che condividono caratteristiche comuni di struttura del modello.</para>
    /// </remarks>
    public interface IJsonComponent : IComponentModel
    {
        // Questa interfaccia funge da marker comune per tutti i componenti JSON.
    }

    /// <summary>
    /// Domain Interface: Oggetto JSON (schema di oggetto).
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto dell'oggetto JSON. Deve ereditare da <see cref="IJsonObjectSchema{TSelf, TJsonProperty}"/>.
    /// </typeparam>
    /// <typeparam name="TJsonProperty">
    /// Il tipo concreto della proprietà JSON. Deve ereditare da <see cref="IJsonProperty{TJsonProperty, TSelf}"/>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta la definizione strutturale di un oggetto JSON, composto da proprietà e, opzionalmente, identificabile tramite una chiave logica.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Ogni oggetto JSON deve essere associato a uno <see cref="IJsonSchema{TSelf, TJsonProperty}"/>, ma la navigazione è gestita dallo schema che mantiene la collezione degli oggetti (relazione unidirezionale dal lato schema).<br/>
    /// - Aggrega istanze di <typeparamref name="TJsonProperty"/>.<br/>
    /// - Estende <see cref="IJsonComponent"/> e implementa <see cref="IEntity{TSelf, TJsonProperty}"/> per la gestione delle proprietà.</para>
    /// <para><b>Constraints:</b><br/>
    /// - In fase di costruzione, ogni oggetto JSON deve essere associato a uno schema valido.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - I metodi <c>MakeIdentifiable</c> consentono di designare una proprietà come chiave logica, utile per l'identificazione univoca.</para>
    /// </remarks>
    public interface IJsonObjectSchema<TSelf, TJsonProperty> : IJsonComponent, IEntity<TSelf, TJsonProperty>
        where TSelf : class, IJsonObjectSchema<TSelf, TJsonProperty>
        where TJsonProperty : class, IJsonProperty<TJsonProperty, TSelf>
    {
        /// <summary>
        /// Rende identificabile l'oggetto designando una proprietà esistente come chiave logica.
        /// </summary>
        /// <param name="keyProperty">La proprietà JSON da designare come chiave logica.</param>
        /// <param name="force">Indica se forzare la designazione anche se l'oggetto è già identificabile.</param>
        void MakeIdentifiable(IJsonProperty<TJsonProperty, TSelf> keyProperty, bool force = false);

        /// <summary>
        /// Rende identificabile l'oggetto designando come chiave la proprietà individuata per nome.
        /// </summary>
        /// <param name="propertyName">Il nome della proprietà da designare come chiave logica.</param>
        /// <param name="force">Indica se forzare la designazione anche se l'oggetto è già identificabile.</param>
        void MakeIdentifiable(string propertyName, bool force = false);

        /// <summary>
        /// Rende l'oggetto non identificabile rimuovendo la chiave logica attualmente impostata.
        /// </summary>
        /// <returns><c>true</c> se la rimozione della chiave è avvenuta con successo; <c>false</c> in caso contrario.</returns>
        bool MakeNonIdentifiable();
    }

    /// <summary>
    /// Domain Interface: Proprietà JSON.
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto della proprietà JSON. Deve ereditare da <c>IJsonProperty&lt;TSelf, TParent&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TParent">
    /// Il tipo dell'oggetto JSON proprietario. Deve ereditare da <c>IJsonObjectSchema&lt;TParent, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta un campo o attributo di un oggetto JSON, che può essere designato come chiave logica per l'identificazione univoca.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a un oggetto JSON che implementa <see cref="IJsonObjectSchema{TParent, TSelf}"/>.<br/>
    /// - Estende <see cref="IJsonComponent"/> e implementa <see cref="IEntityProperty{TSelf, TParent}"/> per supportare l'ownership e l'identificazione.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - I metodi <c>MarkAsKey</c> e <c>UnmarkAsKey</c> controllano la designazione della proprietà come chiave logica.</para>
    /// </remarks>
    public interface IJsonProperty<TSelf, TParent> : IJsonComponent, IEntityProperty<TSelf, TParent>
        where TSelf : class, IJsonProperty<TSelf, TParent>
        where TParent : class, IJsonObjectSchema<TParent, TSelf>
    {
        /// <summary>
        /// Designa questa proprietà come chiave logica per l'oggetto JSON proprietario.
        /// </summary>
        /// <remarks>
        /// <para>Usage Notes: Questo metodo imposta la proprietà come identificatore univoco dell'oggetto JSON.</para>
        /// </remarks>
        void MarkAsKey();

        /// <summary>
        /// Rimuove la designazione di chiave logica da questa proprietà.
        /// </summary>
        /// <remarks>
        /// <para>Usage Notes: Utilizzare questo metodo per annullare la designazione della chiave logica, se necessario.</para>
        /// </remarks>
        void UnmarkAsKey();
    }
}
