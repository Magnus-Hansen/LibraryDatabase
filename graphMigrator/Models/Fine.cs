using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Fine
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public required string Status { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Paid_date { get; set; }
        public DateTime Due_date { get; set; }
        public int Loan_id { get; set; }
    }
}
