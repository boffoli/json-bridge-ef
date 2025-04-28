using JsonBridgeEF.Seeding.Target.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Target.Model.DbContextInfos
{
    /// <inheritdoc cref="IDbContextInfo{ClassInfo, ClassProperty}" />
    /// <summary>
    /// Domain Concept: Rappresenta i metadati principali di un <c>DbContext</c> necessari alla sua configurazione dinamica.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b><br/>
    /// Istanziazione tramite costruttore con nome, namespace, descrizione opzionale e stringa di connessione opzionale.</para>
    /// <para><b>Constraints:</b><br/>
    /// - <c>Name</c> e <c>Namespace</c> non possono essere nulli, vuoti o whitespace.<br/>
    /// - La <c>Description</c> è facoltativa e viene usata per arricchire i metadata di dominio.</para>
    /// <para><b>Relationships:</b><br/>
    /// - Implementa <see cref="IDbContextInfo{TClassInfo, TClassProperty}"/> per <see cref="ClassInfo"/> e <see cref="ClassProperty"/>.<br/>
    /// - Aggrega le istanze di <see cref="ClassInfo"/> su cui effettua il mapping EF.</para>
    /// <para><b>Usage Notes:</b><br/>
    /// Utilizzata nei generatori di codice e nei servizi di seeding per preparare e configurare un <c>DbContext</c> EF via riflessione.</para>
    /// </remarks>
    internal partial class DbContextInfo : IDbContextInfo<ClassInfo, ClassProperty>, IDomainMetadata, IEfEntity
    {
        private readonly List<ClassInfo> _targetEntities = [];

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="DbContextInfo"/>, impostando anche i metadata di dominio.
        /// </summary>
        /// <param name="name">Nome univoco del DbContext (es. “MyAppContext”).</param>
        /// <param name="namespace">Namespace in cui risiede la classe del DbContext.</param>
        /// <param name="description">Descrizione testuale del DbContext.</param>
        /// <param name="connectionString">Stringa di connessione opzionale al database.</param>
        /// <exception cref="ArgumentNullException">
        /// Sollevata se <paramref name="name"/> o <paramref name="namespace"/> è null, vuoto o whitespace.
        /// </exception>
        public DbContextInfo(
            string name,
            string @namespace,
            string? description,
            string? connectionString = null
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Il nome del DbContext non può essere nullo o vuoto.");
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "Il namespace del DbContext non può essere nullo o vuoto.");

            Name                = name;
            Namespace           = @namespace;
            ClassQualifiedName  = $"{@namespace}.{name}";
            ConnectionString    = connectionString;
            _metadata           = new DomainMetadata(name, description);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Namespace { get; }

        /// <inheritdoc />
        public string ClassQualifiedName { get; }

        /// <inheritdoc />
        public string? ConnectionString { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ClassInfo> TargetEntities => _targetEntities.AsReadOnly();

        /// <inheritdoc />
        public void AddTargetEntity(ClassInfo entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (!_targetEntities.Exists(e =>
                string.Equals(e.Name, entity.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(e.ClassQualifiedName, entity.ClassQualifiedName, StringComparison.Ordinal)))
            {
                _targetEntities.Add(entity);
                Touch(); // aggiorna UpdatedAt
            }
        }
    }
}