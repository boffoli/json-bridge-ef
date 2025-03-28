using Microsoft.EntityFrameworkCore;
using JsonBridgeEF.Data;
using JsonBridgeEF.Seeding.Mapping.Models;
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
        /// Restituisce la <see cref="TargetProperty"/> corrispondente,
        /// oppure null se non viene trovata.
        /// </summary>
        /// <param name="projectName">Nome del progetto di mapping (univoco).</param>
        /// <param name="schemaName">Identificatore logico dello schema JSON.</param>
        /// <param name="sourceFieldPath">Percorso del campo JSON da cercare.</param>
        /// <returns>
        /// L'oggetto <see cref="TargetProperty"/> corrispondente alla regola di mapping
        /// che collega <paramref name="sourceFieldPath"/> con la proprietà target,
        /// oppure <c>null</c> se non esiste.
        /// </returns>
        public TargetProperty? ResolveTargetPropertyinition(
            string projectName,
            string schemaName,
            string sourceFieldPath)
        {
            // 1) Trova il progetto di mapping
            var project = FindMappingProjectByName(projectName);
            if (project == null) return null; // se non lo trova, esce

            // 2) Trova lo schema
            var schema = FindSchemaByName(schemaName);
            if (schema == null) return null;

            // 3) Trova il campo JSON
            var jsonField = FindJsonField(schema.Id, sourceFieldPath);
            if (jsonField == null) return null;

            // 4) Trova la mapping rule e carica la TargetProperty
            var rule = FindMappingRule(project.Id, jsonField.Id, includeTargetProperty: true);
            if (rule == null) return null;

            return rule.TargetProperty;
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
                Console.WriteLine($"❌ [ResolveTargetPropertyinition] Nessun MappingProject trovato con Name='{projectName}'");
                return null;
            }

            return project;
        }

        private JsonSchema? FindSchemaByName(string schemaName)
        {
            var schema = dbContext.JsonSchemas
                .FirstOrDefault(s => s.Name == schemaName);

            if (schema == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyinition] Nessuno schema trovato con namer='{schemaName}'");
                return null;
            }

            return schema;
        }

        private JsonField? FindJsonField(int schemaId, string sourceFieldPath)
        {
            var jsonField = dbContext.JsonFields
                .FirstOrDefault(f => f.JsonSchemaId == schemaId && f.SourceFieldPath == sourceFieldPath);

            if (jsonField == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyinition] Nessun JsonField per path='{sourceFieldPath}' in schemaId={schemaId}");
                return null;
            }

            return jsonField;
        }

        private MappingRule? FindMappingRule(int projectId, int jsonFieldId, bool includeTargetProperty)
        {
            IQueryable<MappingRule> query = dbContext.MappingRules;

            if (includeTargetProperty)
                query = query.Include(r => r.TargetProperty);

            var rule = query.FirstOrDefault(r => r.MappingProjectId == projectId && r.JsonFieldId == jsonFieldId);

            if (rule == null)
            {
                Console.WriteLine($"❌ [ResolveTargetPropertyinition] Nessuna MappingRule trovata per projectId={projectId}, fieldId={jsonFieldId}");
                return null;
            }

            return rule;
        }
    }
}