using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public class Boardgame : Item
    {
        public int Id { get; set; }
        public string NoOfPlayers { get; set; }
        public string PlayTime { get; set; }
        public string AgeGroup { get; set; }
    }
}
