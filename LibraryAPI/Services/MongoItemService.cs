using Azure;
using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using LibraryMongoDBBackend.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryAPI.Services
{
    public class MongoItemService : IMongoService<ItemMongo, ItemDto, CreateItemDto, UpdateItemDto, ItemDetailsDto>
    {
        private readonly MongoRepository<ItemMongo> _repository;
        private const int PageSize = 40;
        public MongoItemService(MongoDbContext context)
        {
            _repository = new MongoRepository<ItemMongo>(context, "Items");
        }

        public async Task<PagedResultDto<ItemDto>> GetPageAsync(int pageNumber)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var items = await _repository.GetPageAsync(pageNumber, PageSize);

            return new PagedResultDto<ItemDto>
            {
                Items = items.Items.Select(MapToDto),
                PageNumber = pageNumber,
                PageSize = PageSize,
                TotalCount = items.TotalCount
            };
        }

        public async Task<ItemDto> CreateAsync(CreateItemDto dto)
        {
            var itemCreation = await _repository.CreateAsync(new ItemMongo
            {
                _id = ObjectId.GenerateNewId(),
                Id = (await _repository.GetAllAsync())
                .Max(x => (int?)x.Id) + 1 ?? 1,
                Name = dto.Name,
                MediaType = dto.MediaType,
                ReleaseYear = dto.ReleaseYear,
                Description = dto.Description,
                Reviews = new List<ReviewMongo>(), // start with empty list of reviews
                ReviewSummary = "", // start with empty review summary
                Image = dto.Image,
                Language = new LanguageMongo { Id = dto.LanguageId ?? 0 }, // only set the ID, other fields will be populated when fetching
                Publisher = new PublisherMongo { Id = dto.PublisherId ?? 0 }, // only set the ID, other fields will be populated when fetching
                Creators = dto.CreatorIds.Select(id => new CreatorMongo { Id = id }).ToList(), // only set the IDs, other fields will be populated when fetching
                Genres = dto.GenreIds.Select(id => new GenreMongo { Id = id }).ToList(), // only set the IDs, other fields will be populated when fetching
                Tags = dto.TagIds.Select(id => new TagMongo { Id = id }).ToList(), // only set the IDs, other fields will be populated when fetching
                BookDetails = dto.MediaType == "book" ? new BookDetailsMongo
                {
                    ISBN = dto.Book.Isbn,
                    No_Of_Pages = dto.Book?.NoOfPages,
                    Version = dto.Book?.Version
                } : null, // only set book details if media type is book

                BoardgameDetails = dto.MediaType == "boardgame" ? new BoardgameDetailsMongo
                {
                    Age_Group = dto.Boardgame?.AgeGroup,
                    No_Of_Players = dto.Boardgame?.NoOfPlayers,
                    Play_Time = dto.Boardgame?.PlayTime
                } : null, // only set boardgame details if media type is boardgame

                AverageStars = 0 // start with average stars of 0 since there are no reviews yet
            });
            return MapToDto(itemCreation);
        }

        private async Task<string> GetMongoId(int id)
        {
            ItemMongo item = await _repository.GetByIdAsync(id);
            return item?._id.ToString();
        }

        public async Task<bool> UpdateAsync(UpdateItemDto dto, int id)
        {
            if(await GetByIdAsync(id) != null)
            {
                await _repository.UpdateAsync(id, new ItemMongo
                {
                    _id = ObjectId.Parse(await GetMongoId(id)),
                    Id = id,
                    Name = dto.Name,
                    ReleaseYear = dto.ReleaseYear,
                    MediaType = dto.MediaType,
                    Description = dto.Description,
                    Image = dto.Image,
                    Language = new LanguageMongo { Id = dto.LanguageId ?? 0 },
                    Publisher = new PublisherMongo { Id = dto.PublisherId ?? 0 },
                    Creators = dto.CreatorIds.Select(id => new CreatorMongo { Id = id }).ToList(),
                    Genres = dto.GenreIds.Select(id => new GenreMongo { Id = id }).ToList(),
                    Tags = dto.TagIds.Select(id => new TagMongo { Id = id }).ToList(),
                    BookDetails = dto.MediaType == "book" ? new BookDetailsMongo
                    {
                        ISBN = dto.Book.Isbn,
                        No_Of_Pages = dto.Book?.NoOfPages,
                        Version = dto.Book?.Version
                    } : null,
                    BoardgameDetails = dto.MediaType == "boardgame" ? new BoardgameDetailsMongo
                    {
                        Age_Group = dto.Boardgame?.AgeGroup,
                        No_Of_Players = dto.Boardgame?.NoOfPlayers,
                        Play_Time = dto.Boardgame?.PlayTime
                    } : null,
                });
                return true;
            } return false;
        }
        public async Task<ItemDetailsDto> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if(item == null)
                return null;

            return MapToItemDetailsDto(item);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try {  return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"Error deleting item with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<PagedResultDto<ItemDto>> GetByMediaType(
            string mediaType,
            int pageNumber)
            {
            pageNumber = Math.Max(pageNumber, 1);

            var items = await _repository.GetAllAsync();

            var filteredItems = items
                .Where(i => i.MediaType != null &&
                            i.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PagedResultDto<ItemDto>
            {
                Items = filteredItems.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = PageSize,
                TotalCount = filteredItems.Count
            };
        }


        // -----------------------------
        // DTO MAPPER
        // -----------------------------
        public ItemDto MapToDto(ItemMongo item)
        {
            return new ItemDto
            {
                Id = item.Id ?? 0,
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
                Id = item.Id ?? 0,
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