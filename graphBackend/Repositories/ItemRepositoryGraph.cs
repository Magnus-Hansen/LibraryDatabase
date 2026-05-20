using graphBackend.Models;
using graphBackend.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Neo4j.Driver;

namespace graphBackend.Repositories
{
    public class ItemRepositoryGraph : IItemRepositoryGraph
    {
        private readonly IDriver _driver;
        private readonly string _database;
        public ItemRepositoryGraph(IDriver driver, IConfiguration configuration)
        {
            _driver = driver;
            _database = configuration["Neo4j:Database"] ?? "neo4j";
        }
        public async Task DeleteItem(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync("MATCH (i:Item {id: $id}) DETACH DELETE i", new { id });
            });
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var items = new List<Item>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            var result = await session.RunAsync("MATCH (i:Item) RETURN i");
            while (await result.FetchAsync())
            {
                var node = result.Current["i"].As<INode>();
                items.Add(new Item
                {
                    Id = node.Properties["id"].As<int>(),
                    AverageStars = node.Properties["avarage_stars"].As<decimal>(),
                    Description = node.Properties["description"].As<string>(),
                    MediaType = node.Properties["media_type"].As<string>(),
                    Image = node.Properties["image"].As<string>(),
                    Name = node.Properties["name"].As<string>(),
                    ReleaseYear = node.Properties["release_year"].As<int>(),
                    ReviewSummary = node.Properties["review_summary"].As<string>()
                });
            }
            return items;
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            var result = await session.RunAsync("MATCH (i:Item {id: $id}) RETURN i", new { id });
            if (await result.FetchAsync())
            {
                var node = result.Current["i"].As<INode>();
                return new Item
                {
                    Id = node.Properties["id"].As<int>(),
                    AverageStars = node.Properties["avarage_stars"].As<decimal>(),
                    Description = node.Properties["description"].As<string>(),
                    MediaType = node.Properties["media_type"].As<string>(),
                    Image = node.Properties["image"].As<string>(),
                    Name = node.Properties["name"].As<string>(),
                    ReleaseYear = node.Properties["release_year"].As<int>(),
                    ReviewSummary = node.Properties["review_summary"].As<string>()
                };
            }
            return null;
        }

        public async Task<Item> AddAsync(Item item)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            int nextId = await new Services.IdGenerator(_driver, _database).GetNextId("Loaner");
            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync("CREATE (i:Item {id: $id, avarage_stars: $avarage_stars, description: $description, " +
                    "media_type: $media_type, image: $image, name: $name, release_year: $release_year, review_summary: $review_summary})",
                new
                {
                    id = nextId,
                    avarage_stars = item.AverageStars,
                    description = item.Description,
                    media_type = item.MediaType,
                    image = item.Image,
                    name = item.Name,
                    release_year = item.ReleaseYear,
                    review_summary = item.ReviewSummary
                });
            });
            return new Item
            {
                Id = nextId,
                AverageStars = item.AverageStars,
                Description = item.Description,
                MediaType = item.MediaType,
                Image = item.Image,
                Name = item.Name,
                ReleaseYear = item.ReleaseYear,
                ReviewSummary = item.ReviewSummary
            };
        }

        public async Task<bool> UpdateAsync(Item item)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync("MATCH (i:Item {id: $id}) " +
                    "SET i.avarage_stars = $avarage_stars, i.description = $description, i.media_type = $media_type, " +
                    "i.image = $image, i.name = $name, i.release_year = $release_year, i.review_summary = $review_summary",
                new
                {
                    id = item.Id,
                    avarage_stars = item.AverageStars,
                    description = item.Description,
                    media_type = item.MediaType,
                    image = item.Image,
                    name = item.Name,
                    release_year = item.ReleaseYear,
                    review_summary = item.ReviewSummary
                });
            });
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync("MATCH (i:Item {id: $id}) DETACH DELETE i", new { id });
            });
            return true;
        }

        public Task<List<Creator>> GetCreatorsByIdsAsync(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<Genre>> GetGenresByIdsAsync(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetTagsByIdsAsync(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public void RemoveBook(Book book)
        {
            throw new NotImplementedException();
        }

        public void RemoveBoardgame(Boardgame boardgame)
        {
            throw new NotImplementedException();
        }
    }
}
