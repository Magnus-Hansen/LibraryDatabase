using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Book
    {
        public int Id { get; set; }
        public required string ISBN { get; set; }
        public required int No_of_pages { get; set; }
        public string? Version { get; set; }
        public int Item_id { get; set; }
    }
}
