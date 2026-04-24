using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public DateTime Loan_date { get; set; }
        public DateTime Due_date { get; set; }
        public DateTime? Return_date { get; set; }
        public required string Status { get; set; }
        public int Loaner_id { get; set; }
        public int Inventory_id { get; set; }
    }
}
