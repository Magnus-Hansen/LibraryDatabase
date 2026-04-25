using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphMigrator.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string? No_of_players { get; set; }
        public string? Play_time { get; set; }
        public string? Age_group { get; set; }
        public int Item_id { get; set; }
    }
}
