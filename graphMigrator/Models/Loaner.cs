using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class Loaner
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string CPR { get; set; }
        public string? Tlf { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
