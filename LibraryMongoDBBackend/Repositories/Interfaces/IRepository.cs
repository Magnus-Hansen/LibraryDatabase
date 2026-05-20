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

       
        Task<T?> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync();

        Task<bool> UpdateAsync(int id, T entity);

        Task<bool> DeleteAsync(int id);
    }
}
