using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public class Item
    {
        public enum Media
        {
            book,
            boardgame
        }
        public int Id { get; set; }
        public decimal AverageStars { get; set; }
        public string Description { get; set; }
        public string MediaType { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public string ReviewSummary { get; set; }
    }
}
