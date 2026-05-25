using graphBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Repositories.Interfaces
{
    public interface IItemRepositoryGraph
    {
        Task<(IEnumerable<Item> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<(IEnumerable<Item> Items, int TotalCount)> GetByMediatypeAsync(string mediatype, int page, int pageSize);
        Task<Item?> GetByIdAsync(int id);
        Task<Item> AddAsync(Item item);
        Task<bool> UpdateAsync(Item item);
        Task<bool> DeleteAsync(int id);

        Task<List<Creator>> GetCreatorsByIdsAsync(List<int> ids);
        Task<List<Genre>> GetGenresByIdsAsync(List<int> ids);
        Task<List<Tag>> GetTagsByIdsAsync(List<int> ids);

        Task RemoveBook(Book book);
        Task RemoveBoardgame(Boardgame boardgame);

    }
}
