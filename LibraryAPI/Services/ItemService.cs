using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using LibrarySQLBackend.Models;
using LibrarySQLBackend.Repositories.Interfaces;

namespace LibraryAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IAiSummaryService _aiSummaryService;

        public ItemService(IItemRepository itemRepository, IReviewRepository reviewRepository, IAiSummaryService aiSummaryService)
        {
            _itemRepository = itemRepository;
            _reviewRepository = reviewRepository;
            _aiSummaryService = aiSummaryService;
        }

        private const int PageSize = 40;


        public async Task<ItemDetailsDto?> GetByIdAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                return null;

            var itemDto = new ItemDetailsDto
            {
                Id = item.Id,
                Name = item.Name,
                ReleaseYear = item.ReleaseYear,
                Description = item.Description,
                ReviewSummary = item.ReviewSummary,
                MediaType = item.MediaType,
                Image = item.Image,
                AverageStars = item.AverageStars,
                Language = item.Language?.Language1,
                Publisher = item.Publisher?.Name,
                Creators = item.Creators.Select(c => $"{c.FirstName} {c.LastName}").ToList(),
                Genres = item.Genres.Select(g => g.Name ?? "").ToList(),
                Tags = item.Tags.Select(t => t.Name ?? "").ToList()
            };
                if (item.MediaType == "book")
            {
                itemDto.BookDetails = new BookDto
                {
                    Isbn = item.Book?.Isbn,
                    NoOfPages = item.Book?.NoOfPages,
                    Version = item.Book?.Version
                };
            }
            else if (item.MediaType == "boardgame")
            {
                itemDto.BoardgameDetails = new BoardgameDto
                {
                    NoOfPlayers = item.Boardgame?.NoOfPlayers,
                    PlayTime = item.Boardgame?.PlayTime,
                    AgeGroup = item.Boardgame?.AgeGroup
                };
            }
            return itemDto;
        }

        public async Task<ItemDetailsDto> AddAsync(CreateItemDto dto)
        {
            var creators = await _itemRepository.GetCreatorsByIdsAsync(dto.CreatorIds);
            var genres = await _itemRepository.GetGenresByIdsAsync(dto.GenreIds);
            var tags = await _itemRepository.GetTagsByIdsAsync(dto.TagIds);

            var item = new Item
            {
                Name = dto.Name,
                ReleaseYear = (ushort?)dto.ReleaseYear,
                Description = dto.Description,
                MediaType = dto.MediaType,
                Image = dto.Image,
                LanguageId = (int)dto.LanguageId,
                PublisherId = (int)dto.PublisherId,

                Creators = creators,
                Genres = genres,
                Tags = tags,

                Book = dto.MediaType == "Book" && dto.Book != null
            ? new Book
            {
                Isbn = dto.Book.Isbn,
                NoOfPages = (ushort?)dto.Book.NoOfPages,
                Version = dto.Book.Version
            }
            : null,

                Boardgame = dto.MediaType == "Boardgame" && dto.Boardgame != null
            ? new Boardgame
            {
                NoOfPlayers = dto.Boardgame?.NoOfPlayers,
                PlayTime = dto.Boardgame?.PlayTime,
                AgeGroup = dto.Boardgame?.AgeGroup
            }
            : null
            };

            var createdItem = await _itemRepository.AddAsync(item);

            return await GetByIdAsync(createdItem.Id)
                ?? throw new Exception("Created item could not be found.");
        }

        public async Task<bool> UpdateAsync(int id, UpdateItemDto dto)
        {
            var existingItem = await _itemRepository.GetByIdAsync(id);

            if (existingItem == null)
            {
                return false;
            }

            var mediaType = dto.MediaType?.ToLower();

            existingItem.Name = dto.Name;
            existingItem.ReleaseYear = (ushort?)dto.ReleaseYear;
            existingItem.Description = dto.Description;
            existingItem.MediaType = mediaType;
            existingItem.Image = dto.Image;

            if (dto.LanguageId.HasValue)
            {
                existingItem.LanguageId = dto.LanguageId.Value;
            }

            if (dto.PublisherId.HasValue)
            {
                existingItem.PublisherId = dto.PublisherId.Value;
            }

            var creators = await _itemRepository.GetCreatorsByIdsAsync(dto.CreatorIds);
            var genres = await _itemRepository.GetGenresByIdsAsync(dto.GenreIds);
            var tags = await _itemRepository.GetTagsByIdsAsync(dto.TagIds);

            existingItem.Creators.Clear();
            foreach (var creator in creators)
            {
                existingItem.Creators.Add(creator);
            }

            existingItem.Genres.Clear();
            foreach (var genre in genres)
            {
                existingItem.Genres.Add(genre);
            }

            existingItem.Tags.Clear();
            foreach (var tag in tags)
            {
                existingItem.Tags.Add(tag);
            }

            if (mediaType == "book")
            {
                if (existingItem.Boardgame != null)
                {
                    _itemRepository.RemoveBoardgame(existingItem.Boardgame);
                    existingItem.Boardgame = null;
                }

                if (existingItem.Book == null)
                {
                    existingItem.Book = new Book();
                }

                existingItem.Book.Isbn = dto.Book?.Isbn;
                existingItem.Book.NoOfPages = (ushort?)dto.Book?.NoOfPages;
                existingItem.Book.Version = dto.Book?.Version;
            }
            else if (mediaType == "boardgame")
            {
                if (existingItem.Book != null)
                {
                    _itemRepository.RemoveBook(existingItem.Book);
                    existingItem.Book = null;
                }

                if (existingItem.Boardgame == null)
                {
                    existingItem.Boardgame = new Boardgame();
                }

                existingItem.Boardgame.NoOfPlayers = dto.Boardgame?.NoOfPlayers;
                existingItem.Boardgame.PlayTime = dto.Boardgame?.PlayTime;
                existingItem.Boardgame.AgeGroup = dto.Boardgame?.AgeGroup;
            }

            return await _itemRepository.UpdateAsync(existingItem);
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return (false, "Item was not found.");
            }

            if (await _itemRepository.HasInventoryAsync(id))
            {
                return (false, "Item cannot be deleted because it still has inventory copies.");
            }

            var deleted = await _itemRepository.DeleteAsync(id);

            if (!deleted)
            {
                return (false, "Item could not be deleted.");
            }

            return (true, "Item was deleted successfully.");
        }

        public async Task<ReviewSummaryResultDto?> GenerateReviewSummaryAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);

            if (item == null)
            {
                return null;
            }

            var reviews = await _reviewRepository.GetReviewTextsByItemIdAsync(id);

            if (!reviews.Any())
            {
                throw new InvalidOperationException("Cannot generate a review summary because this item has no review text.");
            }

            var summary = await _aiSummaryService.GenerateReviewSummaryAsync(
                item.Name ?? "Unknown item",
                reviews);

            if (string.IsNullOrWhiteSpace(summary))
            {
                throw new InvalidOperationException("The AI service returned an empty summary.");
            }

            var updated = await _itemRepository.UpdateReviewSummaryAsync(id, summary);

            if (!updated)
            {
                return null;
            }

            return new ReviewSummaryResultDto
            {
                Title = item.Name,
                ReviewSummary = summary
            };
        }


        public async Task<PagedResultDto<ItemDto>> GetAllAsync(int pageNumber)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var result = await _itemRepository.GetAllPagedAsync(pageNumber, PageSize);

            return new PagedResultDto<ItemDto>
            {
                Items = result.Items.Select(MapToItemDto),
                PageNumber = pageNumber,
                PageSize = PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<PagedResultDto<ItemDto>> GetByMediaTypeAsync(string mediaType, int pageNumber)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var normalizedMediaType = mediaType.ToLower();

            var result = await _itemRepository.GetByMediaTypePagedAsync(
                normalizedMediaType,
                pageNumber,
                PageSize);

            return new PagedResultDto<ItemDto>
            {
                Items = result.Items.Select(MapToItemDto),
                PageNumber = pageNumber,
                PageSize = PageSize,
                TotalCount = result.TotalCount
            };
        }

        private static ItemDto MapToItemDto(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                ReleaseYear = item.ReleaseYear,
                MediaType = item.MediaType,
                AverageStars = item.AverageStars
            };
        }


    }
}
