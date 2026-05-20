using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int Item_id { get; set; }
        public required string Status { get; set; }
        public required string Barcode { get; set; }
        public string? Placement { get; set; }
    }
}
