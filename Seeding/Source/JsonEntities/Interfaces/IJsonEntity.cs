using JsonBridgeEF.Seeding.Source.JsonComponents.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonProperties.Interfaces;
using JsonBridgeEF.Seeding.Source.JsonSchemas.Interfaces;
using JsonBridgeEF.Shared.EntityModel.Interfaces;

namespace JsonBridgeEF.Seeding.Source.JsonEntities.Interfaces
{
    /// <summary>
    /// Domain Interface: Rappresenta un'entità JSON non tipizzata nel modello.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Modella una generica entità JSON (es. oggetto) senza vincoli di tipo sulle proprietà o sullo schema.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Implementata dalle entità concrete per supportare accesso polimorfico e non generico.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Deve supportare la promozione a entità identificabile, con validazione coerente sul nome della proprietà chiave.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Collegata a uno <see cref="IJsonSchema"/> e contiene proprietà di tipo <see cref="IJsonProperty"/>.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata nei servizi e helper generici per iterare su entità in maniera astratta.</para>
    /// </remarks>
    public interface IJsonEntity : IJsonComponent
    {
        /// <summary>
        /// Lo schema JSON (non-generico) a cui questa entità appartiene.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Collegamento strutturale allo schema di appartenenza.<br/>
        /// <b>Access:</b> Read-only.
        /// </remarks>
        IJsonSchema Schema { get; }

        /// <summary>
        /// Rende identificabile l'oggetto designando come chiave una proprietà individuata per nome.
        /// </summary>
        /// <param name="propertyName">Nome della proprietà da promuovere a chiave logica.</param>
        /// <param name="force">Se <c>true</c>, forza l'assegnazione anche se già identificabile.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> Deve esistere una proprietà con quel nome nell’entità.</para>
        /// <para><b>Postconditions:</b> L’entità sarà marcata come identificabile.</para>
        /// </remarks>
        void MakeIdentifiable(string propertyName, bool force = false);

        /// <summary>
        /// Rende l'oggetto non identificabile rimuovendo la chiave logica attualmente impostata.
        /// </summary>
        /// <returns><c>true</c> se almeno una chiave è stata rimossa, <c>false</c> altrimenti.</returns>
        /// <remarks>
        /// <para><b>Preconditions:</b> Deve essere presente almeno una proprietà marcata come chiave.</para>
        /// <para><b>Postconditions:</b> Nessuna proprietà sarà marcata come chiave.</para>
        /// </remarks>
        bool MakeNonIdentifiable();
    }

    /// <summary>
    /// Domain Interface: Oggetto JSON (schema di oggetto) con tipizzazione forte.
    /// </summary>
    /// <typeparam name="TSelf">Il tipo concreto dell'entità.</typeparam>
    /// <typeparam name="TJsonProperty">Il tipo concreto delle proprietà JSON associate.</typeparam>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Rappresenta una definizione strutturata e fortemente tipizzata di un oggetto JSON.</para>
    ///
    /// <para><b>Creation Strategy:</b><br/>
    /// Deve essere implementato da una classe che modella una specifica entità JSON concreta.</para>
    ///
    /// <para><b>Constraints:</b><br/>
    /// Il tipo entità deve essere autoreferenziale, e le proprietà devono legarsi fortemente all'entità stessa.</para>
    ///
    /// <para><b>Relationships:</b><br/>
    /// Fa parte di uno <see cref="IJsonSchema{TSelf}"/> e contiene proprietà di tipo <see cref="IJsonProperty{TJsonProperty, TSelf}"/>.</para>
    ///
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzato nei servizi generici per operare su modelli fortemente tipizzati e coerenti.</para>
    /// </remarks>
    public interface IJsonEntity<TSelf, TJsonProperty> : IJsonEntity, IEntity<TSelf, TJsonProperty>
        where TSelf : class, IJsonEntity<TSelf, TJsonProperty>
        where TJsonProperty : class, IJsonProperty<TJsonProperty, TSelf>
    {
        /// <summary>
        /// Lo schema JSON fortemente tipizzato a cui questa entità appartiene.
        /// </summary>
        /// <remarks>
        /// <b>Purpose:</b> Collegamento tipizzato allo schema.<br/>
        /// <b>Access:</b> Read-only, sovrascrive il membro non generico.
        /// </remarks>
        new IJsonSchema<TSelf> Schema { get; }

        /// <summary>
        /// Rende identificabile l'oggetto designando una proprietà esistente come chiave logica.
        /// </summary>
        /// <param name="keyProperty">La proprietà da designare come chiave.</param>
        /// <param name="force">Se <c>true</c>, forza l’operazione anche se già identificabile.</param>
        /// <remarks>
        /// <para><b>Preconditions:</b> La proprietà deve appartenere all'entità.</para>
        /// <para><b>Postconditions:</b> La proprietà sarà marcata come chiave.</para>
        /// </remarks>
        void MakeIdentifiable(IJsonProperty<TJsonProperty, TSelf> keyProperty, bool force = false);
    }
}