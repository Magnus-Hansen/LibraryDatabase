using graphMigrator.Models;

namespace graphMigrator
{
    public class Mapper
    {
        public async Task MigrateUsers(List<Loaner> loaners, Neo4jService neo4j)
        {
            foreach (var loaner in loaners)
            {
                await neo4j.CreateUser(loaner);
            }
        }
        public async Task MigrateLanguages(List<Language> languages, Neo4jService neo4j)
        {
            foreach (var language in languages)
            {
                await neo4j.CreateLanguage(language);
            }
        }
    }
}
