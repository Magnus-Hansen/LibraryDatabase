using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public partial class Loan
    {
        public enum LoanStatus
        {
            overdue,
            active,
            returned
        }
        public int Id { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int LoanerId { get; set; }
        public int InventoryId { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
