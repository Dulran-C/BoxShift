using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxShift.Models
{
    public class Level
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Rows { get; set; } = new();
    }
}
