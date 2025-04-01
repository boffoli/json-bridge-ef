using System;
using System.Collections.Generic;
// Si assume che le interfacce del modello JSON siano definite in questo namespace.
using JsonBridgeEF.SharedModel.Seeding.JsonModel;
// Si assume che le interfacce del modello Class siano definite in questo namespace.
using JsonBridgeEF.SharedModel.ClassModel;
using JsonBridgeEF.SharedModel.Seeding.ClassModel;

namespace JsonBridgeEF.Seeding.Mapping.Abstractions
{
    /// <summary>
    /// Interfaccia che rappresenta il progetto di mapping, contenitore centrale di tutte le regole di mapping.
    /// Raggruppa le regole che collegano i componenti JSON alle classi target.
    /// </summary>
    public interface IMappingProject
    {
        /// <summary>
        /// Nome univoco del progetto di mapping.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Collezione di regole di mapping a livello di blocco.
        /// Ogni regola definisce come un blocco JSON viene mappato su una classe target.
        /// </summary>
        IReadOnlyCollection<IBlockMappingRule> BlockMappingRules { get; }

        /// <summary>
        /// Aggiunge una nuova regola di mapping a livello di blocco al progetto.
        /// </summary>
        /// <param name="blockMappingRule">La regola di mapping del blocco da aggiungere.</param>
        void AddBlockMappingRule(IBlockMappingRule blockMappingRule);
    }

    /// <summary>
    /// Interfaccia per la regola di mapping che lega un blocco JSON (fonte) a una classe target (destinazione).
    /// Il blocco JSON è rappresentato da un oggetto conforme a IJsonObjectSchema, mentre la classe target è
    /// rappresentata da un IClassModel.
    /// </summary>
    public interface IBlockMappingRule
    {
        /// <summary>
        /// Nome univoco della regola di mapping per il blocco.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Il blocco JSON sorgente da mappare, ad es. un oggetto che implementa IJsonObjectSchema.
        /// </summary>
        IJsonObjectSchema JsonBlock { get; }

        /// <summary>
        /// La classe target in cui verranno mappati i dati.
        /// Rappresenta la struttura del modello di destinazione (IClassModel).
        /// </summary>
        IClassModel TargetClass { get; }

        /// <summary>
        /// Collezione di regole di mapping a livello di campo associate a questo blocco.
        /// Ogni regola definisce il mapping di un singolo campo (IJsonProperty) su una proprietà (IClassProperty)
        /// della classe target.
        /// </summary>
        IReadOnlyCollection<IFieldMappingRule> FieldMappingRules { get; }

        /// <summary>
        /// Aggiunge una nuova regola di mapping a livello di campo a questo blocco.
        /// </summary>
        /// <param name="fieldMappingRule">La regola di mapping del campo da aggiungere.</param>
        void AddFieldMappingRule(IFieldMappingRule fieldMappingRule);
    }

    /// <summary>
    /// Interfaccia per la regola di mapping a livello di campo.
    /// Collega un campo JSON (IJsonProperty) a una proprietà della classe target (IClassProperty),
    /// eventualmente applicando una trasformazione ai dati (espressa tramite una formula).
    /// </summary>
    public interface IFieldMappingRule
    {
        /// <summary>
        /// Nome univoco della regola di mapping per il campo.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Il campo JSON sorgente da mappare.
        /// Implementa IJsonProperty e rappresenta un campo all'interno di un blocco JSON.
        /// </summary>
        IJsonProperty JsonField { get; }

        /// <summary>
        /// La proprietà della classe target in cui verrà mappato il valore del campo JSON.
        /// Implementa IClassProperty.
        /// </summary>
        IClassProperty TargetProperty { get; }

        /// <summary>
        /// Eventuale formula o logica di trasformazione da applicare al valore del campo JSON
        /// prima che venga assegnato alla proprietà target.
        /// Può essere nulla o vuota se non è necessaria alcuna trasformazione.
        /// </summary>
        string TransformationFormula { get; }
    }

    /// <summary>
    /// Interfaccia per una regola di mapping composita (merged mapping rule).
    /// Utilizzata quando i dati per un'unica istanza della classe target sono distribuiti su più blocchi JSON.
    /// In questo caso, le regole di mapping a livello di blocco vengono aggregate in base a un campo chiave comune.
    /// </summary>
    public interface IMergedMappingRule
    {
        /// <summary>
        /// Nome univoco della regola di mapping composita.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Nome del campo chiave comune utilizzato per identificare ed unire i dati da blocchi JSON differenti.
        /// Es. "id" o "email".
        /// </summary>
        string CommonKeyField { get; }

        /// <summary>
        /// La classe target in cui verranno aggregati i dati provenienti dai vari blocchi JSON.
        /// Rappresentata da un IClassModel.
        /// </summary>
        IClassModel TargetClass { get; }

        /// <summary>
        /// Collezione di regole di mapping a livello di blocco che contribuiscono al mapping composito.
        /// Ogni regola specifica il mapping di un blocco JSON contenente una parte dei dati.
        /// </summary>
        IReadOnlyCollection<IBlockMappingRule> BlockMappingRules { get; }

        /// <summary>
        /// Aggiunge una nuova regola di mapping a livello di blocco alla regola composita.
        /// </summary>
        /// <param name="blockMappingRule">La regola di mapping del blocco da aggiungere.</param>
        void AddBlockMappingRule(IBlockMappingRule blockMappingRule);
    }
}