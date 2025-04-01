using JsonBridgeEF.SharedModel.DagModel;
using JsonBridgeEF.SharedModel.EntityModel;
using JsonBridgeEF.SharedModel.Infrastructure;
using System;
using System.Collections.Generic;

namespace JsonBridgeEF.SharedModel.Seeding.ClassModel
{
    /// <inheritdoc cref="IClassModel{TSelf, TClassProperty}"/>
    /// <summary>
    /// Implementazione concreta di <see cref="IClassModel{TSelf, TClassProperty}"/> per il modello orientato agli oggetti.
    /// </summary>
    /// <remarks>
    /// <para><b>Domain Concept:</b><br/>
    /// Una classe rappresenta un nodo aggregato che contiene proprietà e fornisce informazioni strutturali,
    /// quali il namespace e il nome completo della classe.</para>
    /// 
    /// <para><b>Creation Strategy:</b><br/>
    /// L'istanza viene creata tramite un costruttore che richiede un nome univoco, il namespace e il nome qualificato completo,
    /// garantendo che questi valori siano validi e immutabili.</para>
    /// 
    /// <para><b>Usage Notes:</b><br/>
    /// La proprietà <c>Properties</c> è un alias per <c>ValueChildren</c> e la gestione delle proprietà avviene tramite i metodi
    /// ereditati dalla classe base. Le informazioni strutturali specifiche (Namespace e ClassQualifiedName) sono fornite da proprietà dedicate.</para>
    /// </remarks>
    internal sealed class ClassModel : Entity<ClassModel, ClassProperty>,
        IClassModel<ClassModel, ClassProperty>,
        IParentNavigableNode<ClassModel>
    {
        // === Fields ===

        private readonly string _namespace;
        private readonly string _classQualifiedName;
        private readonly ParentNavigationManager<ClassModel, ClassProperty> _parentManager;

        // === Constructor ===

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="ClassModel"/>.
        /// </summary>
        /// <param name="name">Nome univoco della classe.</param>
        /// <param name="ns">Namespace della classe (es. <c>MyApp.Models</c>).</param>
        /// <param name="classQualifiedName">Nome completo della classe, composto da namespace e nome (es. <c>MyApp.Models.User</c>).</param>
        /// <exception cref="ArgumentNullException">Sollevata se <paramref name="ns"/> o <paramref name="classQualifiedName"/> è null o vuoto.</exception>
        public ClassModel(string name, string ns, string classQualifiedName)
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(ns))
                throw new ArgumentNullException(nameof(ns), "Il namespace non può essere nullo o vuoto.");
            if (string.IsNullOrWhiteSpace(classQualifiedName))
                throw new ArgumentNullException(nameof(classQualifiedName), "Il nome qualificato della classe non può essere nullo o vuoto.");

            _namespace = ns;
            _classQualifiedName = classQualifiedName;
            _parentManager = new(this);
        }

        // === Properties ===

        /// <inheritdoc />
        public string Namespace => _namespace;

        /// <inheritdoc />
        public string ClassQualifiedName => _classQualifiedName;

        /// <inheritdoc />
        public IReadOnlyCollection<ClassModel> Parents => _parentManager.Parents;

        /// <inheritdoc />
        public bool IsRoot => _parentManager.IsRoot;

        // === Parent Navigation ===

        /// <inheritdoc />
        public void AddParent(ClassModel parent)
        {
            _parentManager.AddParent(parent, bidirectional: true);
        }

        // === Validation Hooks ===

        /// <inheritdoc />
        protected override void AdditionalCustomValidateEntity(ClassModel child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        /// <inheritdoc />
        protected override void AdditionalCustomValidateProperty(ClassProperty child)
        {
            // Nessuna validazione aggiuntiva per default.
        }

        // === Equality Overrides ===

        /// <inheritdoc />
        protected override bool EqualsByValue(ClassModel other)
        {
            // Due classi sono considerate logicamente uguali se appartengono allo stesso namespace
            // e hanno lo stesso nome qualificato completo (case-insensitive).
            return string.Equals(Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ClassQualifiedName, other.ClassQualifiedName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        protected override int GetValueHashCode()
        {
            // Combina l'hash del namespace e del nome qualificato in modo coerente con EqualsByValue.
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(Namespace),
                StringComparer.OrdinalIgnoreCase.GetHashCode(ClassQualifiedName));
        }
    }
}