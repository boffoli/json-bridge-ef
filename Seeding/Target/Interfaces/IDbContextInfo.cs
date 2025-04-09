namespace JsonBridgeEF.Seeding.Target.Interfaces
{
    /// <summary>
    /// Domain Interface: Rappresenta i metadati di un DbContext necessari alla sua istanziazione dinamica,
    /// basandosi esclusivamente su <see cref="IClassComponent"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Fornisce le informazioni minime per istanziare a runtime un DbContext EF tramite riflessione,
    /// utilizzando il <see cref="IClassInfo{TSelf,TClassProperty}.ClassQualifiedName"/> (che identifica la classe completa,
    /// ad esempio "MyApp.Data.MyDbContext") e, eventualmente, la stringa di connessione. Inoltre, consente di associare
    /// dinamicamente le entità target al DbContext.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// L'interfaccia estende <see cref="IClassComponent"/>, per cui eredita la proprietà <see cref="IComponentModel.Name"/>,
    /// ed espone le proprietà aggiuntive necessarie per il mapping del DbContext.
    /// </para>
    /// </remarks>
    public interface IDbContextInfo<TClassInfo, TClassProperty> : IClassComponent
        where TClassInfo : class, IClassInfo<TClassInfo, TClassProperty>
        where TClassProperty : class, IClassProperty<TClassProperty, TClassInfo>
    {
        /// <summary>
        /// Namespace della classe DbContext.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Nome completo della classe DbContext, composto dal namespace e dal nome (es. "MyApp.Data.MyDbContext").
        /// </summary>
        string ClassQualifiedName { get; }

        /// <summary>
        /// Stringa di connessione da utilizzare (opzionale).
        /// </summary>
        string? ConnectionString { get; }

        /// <summary>
        /// Collezione delle entità target associate a questo DbContext.
        /// </summary>
        /// <remarks>
        /// Questa è l'unica proprietà aggiuntiva che consente di associare dinamicamente le entità al DbContext.
        /// </remarks>
        IReadOnlyCollection<TClassInfo> TargetEntities { get; }

        /// <summary>
        /// Aggiunge un'entità target al set gestito da questo DbContext.
        /// </summary>
        /// <param name="entity">L'entità target da aggiungere.</param>
        /// <remarks>
        /// Precondizione: <paramref name="entity"/> non deve essere null.
        /// </remarks>
        void AddTargetEntity(TClassInfo entity);
    }
}
