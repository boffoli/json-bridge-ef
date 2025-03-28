namespace JsonBridgeEF.Common.EfEntities.Managers;

using System;
using System.Collections.Generic;
using JsonBridgeEF.Common.EfEntities.Base;
using JsonBridgeEF.Common.EfEntities.Exceptions;
using JsonBridgeEF.Common.EfEntities.Interfaces.Entities;

/// <summary>
/// Domain Manager: Gestisce la logica di marcatura, controllo e sostituzione di entità con chiave logica.
/// </summary>
/// <typeparam name="TEntity">Tipo di entità nominale con supporto a chiave logica.</typeparam>
/// <remarks>
/// Inizializza il manager con riferimento alla collezione da monitorare.
/// </remarks>
/// <param name="entities">Collezione di entità da monitorare per controllare la presenza.</param>
internal sealed class KeyedEntityCollectionManager<TEntity>(IReadOnlyCollection<TEntity> entities)
    where TEntity : class, INamed, IKeyed
{
    private readonly IReadOnlyCollection<TEntity> _entities = entities ?? throw new ArgumentNullException(nameof(entities));
    private TEntity? _key;

    /// <summary>
    /// Entità attualmente marcata come chiave logica, se presente.
    /// </summary>
    public TEntity? KeyEntity => _key;

    /// <summary>
    /// Aggiunge un'entità alla collezione, con eventuale delega a <see cref="AddKey"/> se marcata come chiave.
    /// Esegue internamente il controllo: se l'entità non è già presente, la aggiunge usando il delegato addToCollection.
    /// </summary>
    /// <param name="entity">Entità da aggiungere.</param>
    /// <param name="addToCollection">Delegato per eseguire l'aggiunta fisica nella collezione.</param>
    public void Add(TEntity entity, Action<TEntity> addToCollection)
    {
        if (entity.IsKey)
        {
            AddKey(entity, force: false, addToCollection);
        }
        else
        {
            if (!_entities.Contains(entity))
                addToCollection(entity);
        }
    }

    /// <summary>
    /// Aggiunge un'entità e la imposta come chiave logica, con gestione opzionale del conflitto.
    /// Il manager verifica internamente se l’entità è già presente; se non lo è, invoca addToCollection.
    /// </summary>
    /// <param name="entity">Entità da aggiungere o marcare come chiave.</param>
    /// <param name="force">Se true, rimpiazza la chiave esistente.</param>
    /// <param name="addToCollection">Delegato per eseguire l'aggiunta fisica nella collezione.</param>
    /// <exception cref="KeyedEntityKeyAlreadyPresentException">Se esiste già una chiave e force è false.</exception>
    public void AddKey(TEntity entity, bool force, Action<TEntity> addToCollection)
    {
        if (_key is not null && !ReferenceEquals(_key, entity))
        {
            if (!force)
                throw new KeyedEntityKeyAlreadyPresentException(_key.Name);
            _key.UnmarkAsKey();
        }

        if (!_entities.Contains(entity))
            addToCollection(entity);

        _key = entity;
        _key.MarkAsKey();
    }

    /// <summary>
    /// Marca come chiave un'entità già esistente trovata per nome.
    /// </summary>
    /// <param name="name">Nome dell'entità da marcare.</param>
    /// <param name="force">Se true, sovrascrive una chiave già esistente.</param>
    /// <param name="resolver">Funzione per cercare l'entità per nome.</param>
    /// <exception cref="KeyedEntityNotFoundException">Se l'entità non viene trovata.</exception>
    public void MarkAsKey(string name, bool force, Func<string, TEntity?> resolver)
    {
        var entity = resolver(name)
            ?? throw new KeyedEntityNotFoundException(name);
        AddKey(entity, force, _ => { /* no-op: l'entità è già presente */ });
    }

    /// <summary>
    /// Rimuove la chiave logica attualmente marcata, se presente.
    /// </summary>
    /// <returns>True se una chiave è stata rimossa, false altrimenti.</returns>
    public bool UnmarkAsKey()
    {
        if (_key is null)
            return false;
        _key.UnmarkAsKey();
        _key = null;
        return true;
    }
}