using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public partial class Inventory
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Barcode { get; set; }
        public string Placement { get; set; }
    }
}
