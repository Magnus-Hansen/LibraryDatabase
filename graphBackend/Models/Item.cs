using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public class Item
    {
        public int Id { get; set; }
        public decimal? AverageStars { get; set; }
        public string Description { get; set; }
        public string MediaType { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int? ReleaseYear { get; set; }
        public string ReviewSummary { get; set; }
        public Language? Language { get; set; }
        public Publisher? Publisher { get; set; }
        public List<Creator> Creators { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public Book? Book { get; set; }
        public Boardgame? Boardgame { get; set; }
        public int? LanguageId { get; set; }
        public int? PublisherId { get; set; }
    }
}
