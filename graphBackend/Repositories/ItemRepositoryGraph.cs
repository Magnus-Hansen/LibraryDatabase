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
                    AverageStars = node.Properties.GetValueOrDefault("average_stars").As<decimal?>(),
                    Description = node.Properties.GetValueOrDefault("description").As<string>(),
                    MediaType = node.Properties.GetValueOrDefault("media_type").As<string>(),
                    Image = node.Properties.GetValueOrDefault("image").As<string>(),
                    Name = node.Properties.GetValueOrDefault("name").As<string>(),
                    ReleaseYear = node.Properties.GetValueOrDefault("release_year").As<int>(),
                    ReviewSummary = node.Properties.GetValueOrDefault("review_summary").As<string>()
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
            WITH i, l

            OPTIONAL MATCH (i)-[:PUBLISHED_BY]->(p:Publisher)
            WITH i, l, p

            OPTIONAL MATCH (i)-[:CREATED_BY]->(c:Creator)
            WITH i, l, p, collect(DISTINCT c) as creators

            OPTIONAL MATCH (i)-[:GENRE_IS]->(g:Genre)
            WITH i, l, p, creators, collect(DISTINCT g) as genres

            OPTIONAL MATCH (i)-[:TAGGED_AS]->(t:Tag)
            WITH i, l, p, creators, genres, collect(DISTINCT t) as tags

            OPTIONAL MATCH (b:Book)-[:IS_BOOK]->(i)
            OPTIONAL MATCH (bg:Boardgame)-[:IS_BOARDGAME]->(i)

            RETURN i, l, p, creators, genres, tags, b, bg
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
                await transaction.RunAsync("CREATE (i:Item {id: $id, description: $description, " +
                    "media_type: $media_type, image: $image, name: $name, release_year: $release_year, review_summary: $review_summary})",
                new
                {
                    id = nextId,
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

        public async Task<List<Creator>> GetCreatorsByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Creator>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            var result = await session.RunAsync(@"
            MATCH (c:Creator)
            WHERE c.id IN $ids
            RETURN c
            ", new { ids });

            var creators = new List<Creator>();

            while (await result.FetchAsync())
            {
                var node = result.Current["c"].As<INode>();

                creators.Add(new Creator
                {
                    Id = node.Properties["id"].As<int>(),
                    FirstName = node.Properties.GetValueOrDefault("first_name")?.As<string>(),
                    LastName = node.Properties.GetValueOrDefault("last_name")?.As<string>()
                });
            }

            return creators;
        }

        public async Task<List<Genre>> GetGenresByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Genre>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            var result = await session.RunAsync(@"
            MATCH (g:Genre)
            WHERE g.id IN $ids
            RETURN g
            ", new { ids });

            var genres = new List<Genre>();

            while (await result.FetchAsync())
            {
                var node = result.Current["g"].As<INode>();

                genres.Add(new Genre
                {
                    Id = node.Properties["id"].As<int>(),
                    Name = node.Properties.GetValueOrDefault("name")?.As<string>()
                });
            }

            return genres;
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Tag>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            var result = await session.RunAsync(@"
            MATCH (t:Tag)
            WHERE t.id IN $ids
            RETURN t
            ", new { ids });

            var tags = new List<Tag>();

            while (await result.FetchAsync())
            {
                var node = result.Current["t"].As<INode>();

                tags.Add(new Tag
                {
                    Id = node.Properties["id"].As<int>(),
                    Name = node.Properties.GetValueOrDefault("name")?.As<string>()
                });
            }

            return tags;
        }
        public async Task RemoveBook(Book book)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync(@"
                MATCH (b:Book {id: $id})
                DETACH DELETE b
                ", new
                {
                    id = book.Id
                });
            });
        }

        public async Task RemoveBoardgame(Boardgame boardgame)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            await session.ExecuteWriteAsync(async transaction =>
            {
                await transaction.RunAsync(@"
                MATCH (bg:Boardgame {id: $id})
                DETACH DELETE bg
                ", new
                {
                    id = boardgame.Id
                });
            });
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
                Id = node.Properties.GetValueOrDefault("id").As<int>(),
                Description = node.Properties.GetValueOrDefault("description").As<string>(),
                MediaType = node.Properties.GetValueOrDefault("media_type").As<string>(),
                Image = node.Properties.GetValueOrDefault("image").As<string>(),
                Name = node.Properties.GetValueOrDefault("name").As<string>(),
                ReleaseYear = node.Properties.GetValueOrDefault("release_year").As<int?>(),
                ReviewSummary = node.Properties.GetValueOrDefault("review_summary").As<string>()
            };
        }
        private Language? MapLanguage(object value)
        {
            if (value is not INode node) return null;
            return new Language
            {
                Id = node.Properties["id"].As<int>(),
                Language1 = node.Properties["name"].As<string>()
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
                Id = node.Properties["id"].As<int>(),
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
                Id = node.Properties["id"].As<int>(),
                NoOfPlayers = node.Properties["no_of_players"].As<string>(),
                PlayTime = node.Properties["play_time"].As<string>(),
                AgeGroup = node.Properties["age_group"].As<string>()
            };
        }
    }
}
