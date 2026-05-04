using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMongoDBBackend.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> CreateAsync(T entity);

        Task<T?> GetByIdAsync(string id);

        Task<List<T>> GetAllAsync();

        Task<bool> UpdateAsync(string id, T entity);

        Task<bool> DeleteAsync(string id);
    }
}
