using JsonBridgeEF.Data;

namespace JsonBridgeEF.ZZZ
{
    internal static class TargetPropertyResolverDemo
    {
        public static void RunResolverDemo(ApplicationDbContext dbContext)
        {
            var resolver = new TargetPropertyResolver(dbContext);

            // Parametri fissi
            string projectName = "Mapping Project - User Data";
            string schemaName = "UserDataV1";
            string sourceFieldPath = "utenti.nome_completo";

            var targetProp = resolver.ResolveTargetPropertyinition(
                projectName, schemaName, sourceFieldPath
            );

            if (targetProp == null)
            {
                Console.WriteLine("❌ Target property non trovata.");
            }
            else
            {
                Console.WriteLine("✅ Target property trovata!");
                Console.WriteLine($"   ID={targetProp.Id}");
                Console.WriteLine($"   Namespace={targetProp.Namespace}");
                Console.WriteLine($"   RootClass={targetProp.RootClass}");
                Console.WriteLine($"   Path={targetProp.Path}");
                Console.WriteLine($"   Name={targetProp.Name}");
            }
        }
    }
}