using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySQLBackend.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<string>> GetReviewTextsByItemIdAsync(int itemId);
    }
}
