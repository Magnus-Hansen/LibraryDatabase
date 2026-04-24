using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Review
    {
        public int Loaner_id { get; set; }
        public int Item_id { get; set; }
        public float No_of_stars { get; set; }
        public string? Text { get; set; }
    }
}
