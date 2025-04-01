using JsonBridgeEF.SharedModel.EntityModel;

namespace JsonBridgeEF.SharedModel.Seeding.JsonModel
{
    /// <summary>
    /// Domain Interface: Componente JSON.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Interfaccia base per tutti i componenti di un grafo JSON tipizzato.</para>
    /// </remarks>
    public interface IJsonComponent
    {
        // Questa interfaccia funge da marker comune per tutti i componenti JSON.
    }

    /// <summary>
    /// Domain Interface: Oggetto JSON (schema di oggetto).
    /// </summary>
    /// <typeparam name="TSelf">
    /// Il tipo concreto dell'oggetto JSON. Deve ereditare da <c>IJsonObjectSchema&lt;TSelf, TJsonProperty&gt;</c>.
    /// </typeparam>
    /// <typeparam name="TJsonProperty">
    /// Il tipo concreto della proprietà JSON. Deve ereditare da <c>IJsonProperty&lt;TJsonProperty, TSelf&gt;</c>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta la definizione strutturale di un oggetto JSON, composto da proprietà e, facoltativamente, identificabile tramite una chiave logica.</para>
    /// <para><b>Relationships:</b><br/>
    /// - È posseduto da uno <see cref="IJsonSchema"/> (relazione bidirezionale obbligatoria).<br/>
    /// - Aggrega istanze di <typeparamref name="TJsonProperty"/>.<br/>
    /// - Estende <see cref="IJsonComponent"/> e implementa <see cref="IEntity{TSelf, TJsonProperty}"/>.</para>
    /// <para><b>Constraints:</b><br/>
    /// - Ogni oggetto JSON deve appartenere a uno <see cref="IJsonSchema"/> valido.<br/>
    /// - L’associazione con <c>Schema</c> è obbligatoria e non può essere nulla.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - I metodi <c>MakeIdentifiable</c> permettono di designare una proprietà chiave.<br/>
    /// - La proprietà <c>Schema</c> consente di risalire allo schema contenitore.</para>
    /// </remarks>
    public interface IJsonObjectSchema<TSelf, TJsonProperty> : IJsonComponent, IEntity<TSelf, TJsonProperty>
        where TSelf : class, IJsonObjectSchema<TSelf, TJsonProperty>
        where TJsonProperty : class, IJsonProperty<TJsonProperty, TSelf>
    {
        /// <summary>
        /// Schema JSON che possiede questo oggetto.
        /// </summary>
        /// <remarks>
        /// <b>Invariants:</b> Deve essere assegnato durante la costruzione e non può essere modificato in seguito.<br/>
        /// <b>Bidirectional &amp; Required:</b> Nessuna istanza di questo oggetto può esistere senza uno schema associato.
        /// </remarks>
        IJsonSchema<TSelf, TJsonProperty> Schema { get; }

        /// <summary>
        /// Rende identificabile l'oggetto designando una proprietà esistente come chiave logica.
        /// </summary>
        void MakeIdentifiable(IJsonProperty<TJsonProperty, TSelf> keyProperty, bool force = false);

        /// <summary>
        /// Rende identificabile l'oggetto designando come chiave la proprietà individuata per nome.
        /// </summary>
        void MakeIdentifiable(string propertyName, bool force = false);

        /// <summary>
        /// Rende l'oggetto non identificabile rimuovendo la chiave corrente.
        /// </summary>
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
    /// Rappresenta un campo di un oggetto JSON. Può essere marcato come chiave logica del proprio oggetto JSON di appartenenza.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Appartiene a un oggetto JSON.<br/>
    /// - Estende <see cref="IJsonComponent"/> e implementa <see cref="IEntityProperty{TSelf, TParent}"/>.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// - I metodi <c>MarkAsKey</c> e <c>UnmarkAsKey</c> controllano lo stato della chiave.</para>
    /// </remarks>
    public interface IJsonProperty<TSelf, TParent> : IJsonComponent, IEntityProperty<TSelf, TParent>
        where TSelf : class, IJsonProperty<TSelf, TParent>
        where TParent : class, IJsonObjectSchema<TParent, TSelf>
    {
        /// <summary>
        /// Designa questa proprietà come chiave logica.
        /// </summary>
        void MarkAsKey();

        /// <summary>
        /// Rimuove la designazione di chiave logica da questa proprietà.
        /// </summary>
        void UnmarkAsKey();
    }
}