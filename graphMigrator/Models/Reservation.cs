using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int Loaner_id { get; set; }
        public int Item_id { get; set; }
        public required string Status { get; set; }
        public required int Queue_number { get; set; }
    }
}
