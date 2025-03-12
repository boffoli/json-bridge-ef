using JsonBridgeEF.Common;
using JsonBridgeEF.Common.UnitOfWorks;

namespace JsonBridgeEF.DataAccess
{
    /// <summary>
    /// Servizio per la gestione delle regole di mapping nel database.
    /// Gestisce operazioni CRUD sulle regole di mapping, validazione e gestione delle definizioni.
    /// </summary>
    internal class MappingUtilityService(IUnitOfWork unitOfWork) : BaseDbService(unitOfWork)
    {

        /// <summary>
        /// Stampa lo schema del modello Entity Framework, mostrando le entità e le loro proprietà.
        /// </summary>
        public void PrintDbContextModel()
        {
            _dbContextInspector.PrintDbContextModel();
        }

        /// <summary>
        /// Stampa lo schema effettivo del database, mostrando le tabelle e le colonne reali.
        /// </summary>
        public void PrintDatabaseSchema()
        {
            _dbContextInspector.PrintDatabaseSchema();
        }
    }
}