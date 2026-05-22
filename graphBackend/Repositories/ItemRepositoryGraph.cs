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

            var query = @"
            MATCH (i:Item {id: $id})

            OPTIONAL MATCH (i)-[:HAS_LANGUAGE]->(l:Language)
            OPTIONAL MATCH (i)-[:PUBLISHED_BY]->(p:Publisher)
            OPTIONAL MATCH (i)-[:CREATED_BY]->(c:Creator)
            OPTIONAL MATCH (i)-[:HAS_GENRE]->(g:Genre)
            OPTIONAL MATCH (i)-[:TAGGED_AS]->(t:Tag)
            OPTIONAL MATCH (i)-[:IS_BOOK]->(b:Book)
            OPTIONAL MATCH (i)-[:IS_BOARDGAME]->(bg:Boardgame)

            RETURN i,
                   l,
                   p,
                   collect(DISTINCT c) as creators,
                   collect(DISTINCT g) as genres,
                   collect(DISTINCT t) as tags,
                   b,
                   bg
            ";
            var result = await session.RunAsync(query, new { id });
            if (!await result.FetchAsync())
                return null;

            var record = result.Current;

            var item = MapItem(record["i"].As<INode>());

            item.Language = MapLanguage(record["l"]);
            item.Publisher = MapPublisher(record["p"]);

            item.Creators = MapList(record["creators"], MapCreator);
            item.Genres = MapList(record["genres"], MapGenre);
            item.Tags = MapList(record["tags"], MapTag);

            item.Book = MapBook(record["b"]);
            item.Boardgame = MapBoardgame(record["bg"]);

            return item;
        }

        public async Task<Item> AddAsync(Item item)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            int nextId = await new Services.IdGenerator(_driver, _database).GetNextId("Item");
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
        private List<T> MapList<T>(object value, Func<INode, T> mapper)
        {
            return value.As<List<INode>>()
                .Select(mapper)
                .ToList();
        }
        private Item MapItem(INode node)
        {
            return new Item
            {
                Id = node.Properties["id"].As<int>(),
                AverageStars = node.Properties["average_stars"].As<decimal>(),
                Description = node.Properties["description"].As<string>(),
                MediaType = node.Properties["media_type"].As<string>(),
                Image = node.Properties["image"].As<string>(),
                Name = node.Properties["name"].As<string>(),
                ReleaseYear = node.Properties["release_year"].As<int?>(),
                ReviewSummary = node.Properties["review_summary"].As<string>()
            };
        }
        private Language? MapLanguage(object value)
        {
            if (value is not INode node) return null;
            return new Language
            {
                Id = node.Properties["id"].As<int>(),
                Language1 = node.Properties["language"].As<string>()
            };
        }
        private Publisher? MapPublisher(object value)
        {
            if (value is not INode node) return null;
            return new Publisher
            {
                Id = node.Properties["id"].As<int>(),
                Name = node.Properties["name"].As<string>()
            };
        }
        private Creator MapCreator(INode node)
        {
            return new Creator
            {
                Id = node.Properties["id"].As<int>(),
                FirstName = node.Properties["first_name"].As<string>(),
                LastName = node.Properties["last_name"].As<string>()
            };
        }
        private Genre MapGenre(INode node)
        {
            return new Genre
            {
                Id = node.Properties["id"].As<int>(),
                Name = node.Properties["name"].As<string>()
            };
        }
        private Tag MapTag(INode node)
        {
            return new Tag
            {
                Id = node.Properties["id"].As<int>(),
                Name = node.Properties["name"].As<string>()
            };
        }
        private Book? MapBook(object value)
        {
            if (value is not INode node) return null;
            return new Book
            {
                Isbn = node.Properties["ISBN"].As<string>(),
                NoOfPages = node.Properties["no_of_pages"].As<int?>(),
                Version = node.Properties["version"].As<string>()
            };
        }
        private Boardgame? MapBoardgame(object value)
        {
            if (value is not INode node) return null;
            return new Boardgame
            {
                NoOfPlayers = node.Properties["no_of_players"].As<string>(),
                PlayTime = node.Properties["play_time"].As<string>(),
                AgeGroup = node.Properties["age_group"].As<string>()
            };
        }
    }
}
