using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxShift.Models
{
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position()
        { 
        }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Position Copy()
        {
            return new Position(Row, Column);
        }

    }
}
