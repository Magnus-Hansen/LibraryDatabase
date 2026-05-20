using LibrarySQLBackend.Context;
using LibrarySQLBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySQLBackend.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetReviewTextsByItemIdAsync(int itemId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ItemId == itemId &&
                            r.Text != null &&
                            r.Text.Trim() != string.Empty)
                .OrderByDescending(r => r.NoOfStars)
                .Select(r => $"{r.NoOfStars} stars: {r.Text}")
                .ToListAsync();
        }
    }
}
