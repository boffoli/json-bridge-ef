using Microsoft.EntityFrameworkCore;
using JsonBridgeEF.Data;
using JsonBridgeEF.Seeding.Mappings.Models;
using JsonBridgeEF.Seeding.SourceJson.Models;
using JsonBridgeEF.Seeding.TargetModel.Models;

namespace JsonBridgeEF.ZZZ
{
    /// <summary>
    /// Classe che risolve e restituisce la definizione di una proprietà target
    /// partendo dal nome di un progetto, dal nome logico di uno schema
    /// e dal percorso di un campo JSON.
    /// </summary>
    internal class TargetPropertyResolver(ApplicationDbContext dbContext)
    {

        /// <summary>
        /// Restituisce la <see cref="TargetPropertyDef"/> corrispondente,
        /// oppure null se non viene trovata.
        /// </summary>
        /// <param name="projectName">Nome del progetto di mapping (univoco).</param>
        /// <param name="schemaIdentifier">Identificatore logico dello schema JSON.</param>
        /// <param name="sourceFieldPath">Percorso del campo JSON da cercare.</param>
        /// <returns>
        /// L'oggetto <see cref="TargetPropertyDef"/> corrispondente alla regola di mapping
        /// che collega <paramref name="sourceFieldPath"/> con la proprietà target,
        /// oppure <c>null</c> se non esiste.
        /// </returns>
        public TargetPropertyDef? ResolveTargetPropertyDefinition(
            string projectName,
            string schemaIdentifier,
            string sourceFieldPath)
        {
            // 1) Trova il progetto di mapping
            var project = FindMappingProjectByName(projectName);
            if (project == null) return null; // se non lo trova, esce

            // 2) Trova lo schema
            var schema = FindSchemaByIdentifier(schemaIdentifier);
            if (schema == null) return null;

            // 3) Trova il campo JSON
            var jsonFieldDef = FindJsonField(schema.Id, sourceFieldPath);
            if (jsonFieldDef == null) return null;

            // 4) Trova la mapping rule e carica la TargetPropertyDef
            var rule = FindMappingRule(project.Id, jsonFieldDef.Id, includeTargetProperty: true);
            if (rule == null) return null;

            return rule.TargetPropertyDef;
        }

        // -------------------------------------------------------------
        //   METODI PRIVATI DI SUPPORTO
        // -------------------------------------------------------------

        private MappingProject? FindMappingProjectByName(string projectName)
        {
            var project = dbContext.MappingProjects
                .FirstOrDefault(p => p.Name == projectName);

            if (project == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyDefinition] Nessun MappingProject trovato con Name='{projectName}'");
                return null;
            }

            return project;
        }

        private JsonSchemaDef? FindSchemaByIdentifier(string schemaIdentifier)
        {
            var schema = dbContext.JsonSchemaDefs
                .FirstOrDefault(s => s.JsonSchemaIdentifier == schemaIdentifier);

            if (schema == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyDefinition] Nessuno schema trovato con identifier='{schemaIdentifier}'");
                return null;
            }

            return schema;
        }

        private JsonFieldDef? FindJsonField(int schemaId, string sourceFieldPath)
        {
            var jsonFieldDef = dbContext.JsonFieldDefs
                .FirstOrDefault(f => f.JsonSchemaDefId == schemaId && f.SourceFieldPath == sourceFieldPath);

            if (jsonFieldDef == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyDefinition] Nessun JsonFieldDef per path='{sourceFieldPath}' in schemaId={schemaId}");
                return null;
            }

            return jsonFieldDef;
        }

        private MappingRule? FindMappingRule(int projectId, int jsonFieldDefId, bool includeTargetProperty)
        {
            IQueryable<MappingRule> query = dbContext.MappingRules;

            if (includeTargetProperty)
                query = query.Include(r => r.TargetPropertyDef);

            var rule = query.FirstOrDefault(r => r.MappingProjectId == projectId && r.JsonFieldDefId == jsonFieldDefId);

            if (rule == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyDefinition] Nessuna MappingRule trovata per projectId={projectId}, fieldId={jsonFieldDefId}");
                return null;
            }

            return rule;
        }
    }
}