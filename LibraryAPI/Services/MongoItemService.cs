using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryAPI.Services
{
    public class MongoItemService : IMongoService
    {
        private readonly IMongoCollection<ItemMongo> _items;

        public MongoItemService(IMongoDatabase database)
        {
            _items = database.GetCollection<ItemMongo>("Items");
        }

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<ItemDto> CreateAsync(ItemMongo item)
        {
            await _items.InsertOneAsync(item);

            return MapToItemDto(item);
        }

        // -----------------------------
        // GET ALL
        // -----------------------------
        public async Task<List<ItemDto>> GetAllAsync()
        {
            var items = await _items
                .Find(_ => true)
                .ToListAsync();

            return items.Select(MapToItemDto).ToList();
        }

        // -----------------------------
        // GET BY ID
        // -----------------------------
        public async Task<ItemDetailsDto?> GetByIdAsync(int id)
        {
            var item = await _items
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (item == null)
                return null;

            return MapToItemDetailsDto(item);
        }

        // -----------------------------
        // GET BY NAME
        // -----------------------------
        public async Task<List<ItemDto>> GetByNameAsync(string name)
        {
            var filter = Builders<ItemMongo>.Filter.Regex(
                x => x.Name,
                new MongoDB.Bson.BsonRegularExpression(name, "i")
            );

            var items = await _items
                .Find(filter)
                .ToListAsync();

            return items.Select(MapToItemDto).ToList();
        }

        // -----------------------------
        // FILTER BY MEDIA TYPE
        // -----------------------------
        public async Task<List<ItemDto>> GetByMediaTypeAsync(string mediaType)
        {
            var items = await _items
                .Find(x => x.MediaType.ToLower() == mediaType.ToLower())
                .ToListAsync();

            return items.Select(MapToItemDto).ToList();
        }

        // -----------------------------
        // UPDATE
        // -----------------------------
        public async Task<bool> UpdateAsync(int id, ItemMongo updatedItem)
        {
            updatedItem._id = ObjectId.Empty;

            var result = await _items.ReplaceOneAsync(
                x => x.Id == id,
                updatedItem
            );

            return result.ModifiedCount > 0;
        }

        // -----------------------------
        // DELETE
        // -----------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _items.DeleteOneAsync(x => x.Id == id);

            return result.DeletedCount > 0;
        }

        // -----------------------------
        // DTO MAPPER
        // -----------------------------
        private static ItemDto MapToItemDto(ItemMongo item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                ReleaseYear = item.ReleaseYear,
                MediaType = item.MediaType,
                AverageStars = (decimal)item.AverageStars
            };
        }

        private static ItemDetailsDto MapToItemDetailsDto(ItemMongo item)
        {
            var itemDto = new ItemDetailsDto
            {
                Id = item.Id,
                Name = item.Name,
                ReleaseYear = item.ReleaseYear,
                Description = item.Description,
                ReviewSummary = item.ReviewSummary,
                MediaType = item.MediaType,
                Image = item.Image,
                AverageStars = (decimal?)item.AverageStars,
                Language = item.Language.Name,
                Publisher = item.Publisher?.Name,
                Creators = item.Creators.Select(c => $"{c.FirstName} {c.LastName}").ToList(),
                Genres = item.Genres.Select(g => g.Name ?? "").ToList(),
                Tags = item.Tags.Select(t => t.Name ?? "").ToList()
            };
            if (item.MediaType == "book")
            {
                itemDto.BookDetails = new BookDto
                {
                    Isbn = item.BookDetails.ISBN,
                    NoOfPages = item.BookDetails.No_Of_Pages,
                    Version = item.BookDetails.Version
                };
            }
            else if (item.MediaType == "boardgame")
            {
                itemDto.BoardgameDetails = new BoardgameDto
                {
                    NoOfPlayers = item.BoardgameDetails?.No_Of_Players,
                    PlayTime = item.BoardgameDetails?.Play_Time,
                    AgeGroup = item.BoardgameDetails?.Age_Group
                };
            }
            return itemDto;
        }
    }
}