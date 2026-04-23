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
        public async Task MigrateItems(List<Item> items, Neo4jService neo4j)
        {
            foreach (var item in items)
            {
                await neo4j.CreateItem(item);
            }
        }
        public async Task MigrateCreators(List<Creator> creators, Neo4jService neo4j)
        {
            foreach (var loan in creators)
            {
                await neo4j.CreateCreator(loan);
            }
        }
    }
}
