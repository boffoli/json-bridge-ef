using JsonBridgeEF.Seeding.Target.Interfaces;
using JsonBridgeEF.Seeding.Target.Model.ClassInfos;
using JsonBridgeEF.Seeding.Target.Model.Properties;
using JsonBridgeEF.Shared.Domain.Interfaces;
using JsonBridgeEF.Shared.Domain.Model;
using JsonBridgeEF.Shared.EfPersistance.Interfaces;

namespace JsonBridgeEF.Seeding.Target.Model.DbContextInfos
{
    /// <inheritdoc cref="IDbContextInfo{TClassInfo, TClassProperty}" />
    /// <summary>
    /// Domain Concept: Rappresenta i metadati principali di un <c>DbContext</c> necessari alla sua istanziazione dinamica.
    /// </summary>
    /// <remarks>
    /// <para><b>Creation Strategy:</b> Istanziata tramite costruttore con <c>Name</c>, <c>Namespace</c> e opzionalmente <c>ConnectionString</c>.</para>
    /// <para><b>Constraints:</b> Il <c>Name</c> e il <c>Namespace</c> devono essere valorizzati (non null o vuoti).</para>
    /// <para><b>Relationships:</b> Implementa <see cref="IDbContextInfo{TClassInfo, TClassProperty}"/> specializzata su <see cref="ClassInfo"/> e <see cref="ClassProperty"/>.<br/>
    /// Aggrega istanze di <see cref="ClassInfo"/> come entità target per il mapping.</para>
    /// <para><b>Usage Notes:</b> Utilizzata per generare e configurare dinamicamente un <c>DbContext</c> EF via riflessione, definendone anche la connessione e le entità gestite.</para>
    /// </remarks>
    internal partial class DbContextInfo : IDbContextInfo<ClassInfo, ClassProperty>, IDomainMetadata, IEfEntity
    {
        private readonly List<ClassInfo> _targetEntities = [];

        /// <inheritdoc />
        public DbContextInfo(string name, string @namespace, string? connectionString = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(@namespace))
                throw new ArgumentNullException(nameof(@namespace));

            Name = name;
            Namespace = @namespace;
            ClassQualifiedName = $"{@namespace}.{name}";
            ConnectionString = connectionString;
            _metadata = new DomainMetadata(name);
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Namespace del DbContext.
        /// </summary>
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
