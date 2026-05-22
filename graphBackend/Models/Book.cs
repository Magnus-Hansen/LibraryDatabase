using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Models
{
    public class Book : Item
    {
        public int Id { get; set; }
        public string Isbn { get; set; }
        public int? NoOfPages { get; set; }
        public string Version { get; set; }
    }
}
